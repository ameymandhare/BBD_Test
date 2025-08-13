using Microsoft.EntityFrameworkCore;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL.Repository
{
    public class SupplierRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public SupplierRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<MSupplier>> Get(string? supplierCode, string? searchText, int? id)
        {
            var query = appDbContext.MSuppliers
                .Where(x => x.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(supplierCode))
            {
                query = query.Where(x => x.SupplierCode == supplierCode);
            }

            if (id >= 0)
            {
                query = query.Where(x => x.Id == id);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string lowerSearch = searchText.ToLower();
                query = query.Where(x => x.SupplierName.ToLower().Contains(lowerSearch) ||
                                         x.SupplierCode.ToLower().Contains(lowerSearch));
            }

            return await query
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<int> Add(MSupplier supplier)
        {
            appDbContext.MSuppliers.Add(supplier);
            int added = await appDbContext.SaveChangesAsync();
            return added;
        }

        public async Task<int> Update(MSupplier supplier)
        {
            appDbContext.MSuppliers.Update(supplier);
            return await appDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a list of new suppliers in bulk.
        /// </summary>
        public async Task BulkAdd(List<MSupplier> suppliers)
        {
            try
            {
                logger.WriteInfo($"Bulk inserting {suppliers.Count} suppliers.");
                await appDbContext.MSuppliers.AddRangeAsync(suppliers);
                await appDbContext.SaveChangesAsync();
                logger.WriteInfo("Bulk insert completed.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in SupplierRepository.BulkAdd: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a dictionary of active suppliers by a list of IDs.
        /// </summary>
        public async Task<Dictionary<int, MSupplier>> Getlist(List<int> ids)
        {
            try
            {
                logger.WriteInfo($"Fetching suppliers by Ids: {string.Join(",", ids)}");
                var result = await appDbContext.MSuppliers
                    .Where(x => ids.Contains(x.Id) && x.IsActive)
                    .ToDictionaryAsync(x => x.Id);

                logger.WriteInfo($"Fetched {result.Count} suppliers.");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in SupplierRepository.Getlist: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Inserts a list of suppliers in bulk and logs the result.
        /// </summary>
        public async Task<string> BulkInsertSuppliers(List<MSupplier> suppliers)
        {
            try
            {
                logger.WriteInfo($"Starting bulk insert for {suppliers.Count} suppliers.");
                await appDbContext.MSuppliers.AddRangeAsync(suppliers);
                await appDbContext.SaveChangesAsync();
                logger.WriteInfo("Bulk insert completed successfully.");
                return $"Imported {suppliers.Count} suppliers successfully.";
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in SupplierRepository.BulkInsertSuppliers: {ex.Message}");
                throw;
            }
        }
    }
}