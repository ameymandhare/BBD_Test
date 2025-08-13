using Microsoft.EntityFrameworkCore;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL.Repository
{
    public class DepartmentRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly CommonDbContext commonDbContext;

        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public DepartmentRepository(AppDbContext appDbContext, CommonDbContext commonDbContext)
        {
            this.appDbContext = appDbContext;
            this.commonDbContext = commonDbContext;
        }

        public async Task<int> Add(MDepartment department)
        {
            department.IsActive = true;
            department.CreatedDate = DateTime.UtcNow;

            await appDbContext.MDepartments.AddAsync(department);
            int added = await appDbContext.SaveChangesAsync();

            return added;
        }

        public async Task<bool> DepartmentNameIsExists(string deptName)
        {
            return await appDbContext.MDepartments
                .AnyAsync(x => x.DeptName.ToLower() == deptName.ToLower() && x.IsActive == true);
        }

        public async Task<Dictionary<int, MDepartment>> Getlist(List<int> ids)
        {
            logger.WriteInfo($"Fetching departments by Ids: {string.Join(",", ids)}");
            var result = await appDbContext.MDepartments
                .Where(x => ids.Contains(x.Id) && x.IsActive)
                .ToDictionaryAsync(x => x.Id);
            logger.WriteInfo($"Fetched {result.Count} departments.");
            return result;
        }

        public async Task<MDepartment?> Get(int id)
        {
            return await appDbContext.MDepartments.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
        }

        public async Task<int> Update(MDepartment department)
        {
            appDbContext.MDepartments.Update(department);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<List<MDepartment>> Get(int? id, string? departmentName)
        {
            var query = appDbContext.MDepartments
                .Where(x => x.IsActive)
                .AsQueryable();

            if (id.HasValue && id.Value >= 0)
            {
                query = query.Where(x => x.Id == id.Value);
            }

            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                string search = departmentName.ToLower();
                query = query.Where(x => x.DeptName.ToLower().Contains(search));
            }

            return await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
        }

        public async Task<int> BulkAddOrUpdate(List<MDepartment> departments)
        {
            int processed = 0;
            var updateIds = departments.Where(d => d.Id > 0).Select(d => d.Id).ToList();

            var existingMap = await appDbContext.MDepartments
                .Where(d => updateIds.Contains(d.Id) && d.IsActive == true)
                .ToDictionaryAsync(d => d.Id);

            foreach (var incoming in departments)
            {
                if (incoming.Id > 0 && existingMap.ContainsKey(incoming.Id))
                {
                    var existing = existingMap[incoming.Id];
                    existing.DeptName = incoming.DeptName;
                    existing.ModifiedDate = DateTime.UtcNow;
                    existing.ModifiedById = incoming.ModifiedById;
                    processed++;
                }
                else
                {
                    var newDept = new MDepartment
                    {
                        DeptName = incoming.DeptName,
                        CreatedDate = DateTime.UtcNow,
                        CreatedById = incoming.CreatedById,
                        IsActive = true
                    };
                    await appDbContext.MDepartments.AddAsync(newDept);
                    processed++;
                }
            }

            await appDbContext.SaveChangesAsync();
            return processed;
        }

        public async Task<string> BulkUpload(List<MDepartment> departments, int userId)
        {
            //await Delete(0, userId).ConfigureAwait(false);

            foreach (var dept in departments)
            {
                dept.CreatedById = userId;
                dept.CreatedDate = DateTime.UtcNow;
                dept.IsActive = true;
                await appDbContext.MDepartments.AddAsync(dept);
            }

            await appDbContext.SaveChangesAsync();
            return $"Imported {departments.Count} departments successfully.";
        }

        public async Task<bool> Delete(int id, int updatedBy)
        {
            if (id > 0)
            {
                logger.WriteDebug($"Attempting to soft delete MDepartment with ID {id} in MasterRepository.DeleteDepartmentAsync.");

                var department = await appDbContext.MDepartments.FirstOrDefaultAsync(x => x.Id == id);
                if (department == null)
                    return false;

                department.IsActive = false;
                department.ModifiedById = updatedBy;
                department.ModifiedDate = DateTime.UtcNow;

                appDbContext.MDepartments.Update(department);
            }
            else
            {
                logger.WriteDebug("Soft deleting all active MDepartments using raw SQL.");

                await appDbContext.Database.ExecuteSqlRawAsync(@"
                UPDATE M_Department
                SET IsActive = 0,
                    ModifiedDate = GETUTCDATE(),
                    ModifiedById = {0}
                WHERE IsActive = 1", updatedBy);
            }

            await appDbContext.SaveChangesAsync();
            return true;
        }
    }
}