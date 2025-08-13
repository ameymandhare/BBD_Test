using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL.Repository
{
    public class ShapeRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly CommonDbContext commonDbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        //private readonly YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public ShapeRepository(AppDbContext appDbContext, CommonDbContext commonDbContext, IWebHostEnvironment hostEnvironment)
        {
            this.appDbContext = appDbContext;
            this.commonDbContext = commonDbContext;
            _hostingEnvironment = hostEnvironment;
        }

        public async Task<int> Add(MCrimpingShape shape)
        {
            shape.IsActive = true;
            shape.CreatedDate = DateTime.UtcNow;
            await appDbContext.MCrimpingShapes.AddAsync(shape);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> Update(MCrimpingShape shape)
        {
            appDbContext.MCrimpingShapes.Update(shape);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<List<MCrimpingShape>> Get(string? name, string? searchText, int? id)
        {
            var query = appDbContext.MCrimpingShapes
                .Where(s => s.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(s => s.Name == name);

            if (id.HasValue && id > 0)
                query = query.Where(s => s.Id == id);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string lowerSearch = searchText.ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(lowerSearch));
            }

            var list = await query
        .OrderByDescending(s => s.CreatedDate)
        .ToListAsync();

            foreach (var item in list)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(item.ImageUrl))
                    {
                        string relativePath = item.ImageUrl.TrimStart('/');
                        string fullPath = Path.Combine(_hostingEnvironment.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                        if (File.Exists(fullPath))
                        {
                            byte[] imageBytes = await File.ReadAllBytesAsync(fullPath);
                            item.Base64Image = Convert.ToBase64String(imageBytes);
                            item.Base64FileName = Path.GetFileName(fullPath);
                            //item.ImageUrl = $"data:image/png;base64,{item.Base64Image}";
                        }
                        else
                        {
                            item.Base64Image = null;
                            item.Base64FileName = null;
                            //item.ImageUrl = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    item.Base64Image = null;
                    item.Base64FileName = null;
                }
            }
            return list;
        }

        public async Task<MCrimpingShape?> GetById(int id)
        {
            return await appDbContext.MCrimpingShapes
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
        }

        public bool NameExists(string name, int? excludeId = null)
        {
            return appDbContext.MCrimpingShapes
                .Any(s => s.IsActive && s.Name == name && (!excludeId.HasValue || s.Id != excludeId));
        }

        public async Task<int> BulkAddOrUpdate(List<MCrimpingShape> shapes)
        {
            int processed = 0;

            var updateIds = shapes.Where(s => s.Id > 0).Select(s => s.Id).ToList();

            // Using synchronous call
            var existingShapes = appDbContext.MCrimpingShapes
                .Where(s => updateIds.Contains(s.Id) && s.IsActive)
                .ToDictionary(s => s.Id);

            foreach (var shape in shapes)
            {
                if (shape.Id > 0 && existingShapes.ContainsKey(shape.Id))
                {
                    var existing = existingShapes[shape.Id];
                    existing.Name = shape.Name;
                    existing.ModifiedById = shape.ModifiedById;
                    existing.ModifiedDate = DateTime.UtcNow;
                    processed++;
                }
                else
                {
                    var newShape = new MCrimpingShape
                    {
                        Name = shape.Name,
                        CreatedById = shape.CreatedById,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    };

                    // Synchronous add
                    appDbContext.MCrimpingShapes.Add(newShape);
                    processed++;
                }
            }

            // Synchronous save
            appDbContext.SaveChanges();

            return await Task.FromResult(processed);
        }

        public Task<string> BulkUpload(List<MCrimpingShape> shapes, int userId)
        {
            // Fetch all existing shapes once to compare (synchronously)
            var existingShapes = appDbContext.MCrimpingShapes
                .Where(s => s.IsActive)
                .ToList();

            int addedCount = 0;
            int duplicateCount = 0;

            foreach (var shape in shapes)
            {
                // Check if shape with the same name already exists (case-insensitive)
                bool alreadyExists = existingShapes
                    .Any(s => s.Name.Trim().ToLower() == shape.Name.Trim().ToLower());

                if (!alreadyExists)
                {
                    shape.CreatedById = userId;
                    shape.CreatedDate = DateTime.UtcNow;
                    shape.IsActive = true;
                    appDbContext.MCrimpingShapes.Add(shape); // synchronous add
                    addedCount++;
                }
                else
                {
                    duplicateCount++;
                }
            }

            if (addedCount > 0)
            {
                appDbContext.SaveChanges();
                return Task.FromResult($"Successfully added {addedCount} new shape(s). {duplicateCount} duplicate(s) were skipped.");
            }
            else
            {
                return Task.FromResult($"All {duplicateCount} shape(s) already exist. No new shapes were added.");
            }
        }
    }
}