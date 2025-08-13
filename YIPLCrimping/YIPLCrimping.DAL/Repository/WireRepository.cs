using Microsoft.EntityFrameworkCore;
using YIPLCrimping.DAL.Models;

namespace YIPLCrimping.DAL.Repository
{
    public class WireRepository
    {
        private readonly AppDbContext appDbContext;

        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public WireRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        /// <summary>
        /// Retrieves wire types based on optional filters (wire type code, search text, or ID)
        /// </summary>
        /// <param name="wireTypeCode">Filter by wire type code (optional)</param>
        /// <param name="searchText">Search text to filter by name or code (optional)</param>
        /// <param name="id">Filter by specific wire type ID (optional)</param>
        /// <returns>List of matching active wire types</returns>
        public async Task<List<MWireType>> GetWireType(string? wireTypeCode, string? searchText, int? id)
        {
            var query = appDbContext.MWireTypes
                .Where(x => x.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(wireTypeCode))
            {
                query = query.Where(x => x.WireTypeCode == wireTypeCode);
            }

            if (id.HasValue && id > 0)
            {
                query = query.Where(x => x.Id == id);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string lowerSearch = searchText.ToLower();
                query = query.Where(x => x.WireTypeName.ToLower().Contains(lowerSearch) ||
                                         x.WireTypeCode.ToLower().Contains(lowerSearch));
            }

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new wire type to the database
        /// </summary>
        /// <param name="wireType">Wire type entity to add</param>
        /// <returns>Number of records affected</returns>
        public async Task<int> AddWireType(MWireType wireType)
        {
            appDbContext.MWireTypes.Add(wireType);
            int added = await appDbContext.SaveChangesAsync();
            return added;
        }

        /// <summary>
        /// Updates an existing wire type in the database
        /// </summary>
        /// <param name="wireType">Wire type entity with updated values</param>
        /// <returns>Number of records affected</returns>
        public async Task<int> UpdateWireType(MWireType wireType)
        {
            appDbContext.MWireTypes.Update(wireType);
            return await appDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Bulk inserts multiple wire types in a single operation
        /// </summary>
        /// <param name="wireTypes">List of wire type entities to insert</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task BulkAdd(List<MWireType> wireTypes)
        {
            try
            {
                logger.WriteInfo($"Bulk inserting {wireTypes.Count} wire types.");
                await appDbContext.MWireTypes.AddRangeAsync(wireTypes);
                await appDbContext.SaveChangesAsync();
                logger.WriteInfo("Bulk insert completed.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in WireTypeRepository.BulkAdd: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a dictionary of wire types keyed by their IDs
        /// </summary>
        /// <param name="ids">List of wire type IDs to retrieve</param>
        /// <returns>Dictionary of wire types with ID as key</returns>
        public async Task<Dictionary<int, MWireType>> GetWireTypelist(List<int> ids)
        {
            try
            {
                logger.WriteInfo($"Fetching wire types by Ids: {string.Join(",", ids)}");
                var result = await appDbContext.MWireTypes
                    .Where(x => ids.Contains(x.Id) && x.IsActive)
                    .ToDictionaryAsync(x => x.Id);

                logger.WriteInfo($"Fetched {result.Count} wire types.");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in WireTypeRepository.Getlist: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Bulk inserts wire types and returns a success message
        /// </summary>
        /// <param name="wireTypes">List of wire type entities to insert</param>
        /// <returns>Success message with count of inserted records</returns>
        public async Task<string> BulkInsertWireTypes(List<MWireType> wireTypes)
        {
            try
            {
                logger.WriteInfo($"Starting bulk insert for {wireTypes.Count} wire types.");
                await appDbContext.MWireTypes.AddRangeAsync(wireTypes);
                await appDbContext.SaveChangesAsync();
                logger.WriteInfo("Bulk insert completed successfully.");
                return $"Imported {wireTypes.Count} wire types successfully.";
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in WireTypeRepository.BulkInsertWireTypes: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves wire sizes based on optional filters (code, search text, ID, or size)
        /// </summary>
        /// <param name="wireSizeCode">Filter by wire size code (optional)</param>
        /// <param name="searchText">Search text to filter by code or size (optional)</param>
        /// <param name="id">Filter by specific wire size ID (optional)</param>
        /// <param name="wireSize">Filter by exact wire size value (optional)</param>
        /// <returns>List of matching active wire sizes</returns>
        public async Task<List<MWireSize>> GetWireSize(string wireSizeCode, string searchText, int? id, decimal? wireSize)
        {
            try
            {
                logger.WriteInfo($"Getting wire sizes - Code: {wireSizeCode}, Search: {searchText}, ID: {id}, Size: {wireSize}");

                var query = appDbContext.MWireSizes
                    .Where(x => x.IsActive)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(wireSizeCode))
                {
                    query = query.Where(x => x.WireSizeCode == wireSizeCode);
                }

                if (id.HasValue && id > 0)
                {
                    query = query.Where(x => x.Id == id);
                }

                if (wireSize.HasValue)
                {
                    query = query.Where(x => x.WireSize == wireSize);
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    string search = searchText.ToLower();
                    query = query.Where(x =>
                        x.WireSizeCode.ToLower().Contains(search) ||
                        x.WireSize.ToString().Contains(search));
                }

                var result = await query
                    .OrderByDescending(x => x.CreatedDate)
                    .ThenByDescending(x => x.Id).ToListAsync();

                logger.WriteInfo($"Found {result.Count} wire sizes");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in GetWireSize: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds a new wire size to the database with automatic timestamps
        /// </summary>
        /// <param name="wireSize">Wire size entity to add</param>
        /// <returns>Number of records affected</returns>
        public async Task<int> AddWireSize(MWireSize wireSize)
        {
            try
            {
                logger.WriteInfo($"Adding new wire size: {wireSize.WireSizeCode}");

                wireSize.CreatedDate = DateTime.Now;
                wireSize.IsActive = true;

                appDbContext.MWireSizes.Add(wireSize);
                int result = await appDbContext.SaveChangesAsync();

                logger.WriteInfo($"Added wire size ID: {wireSize.Id}");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in AddWireSize: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing wire size in the database with modification timestamp
        /// </summary>
        /// <param name="wireSize">Wire size entity with updated values</param>
        /// <returns>Number of records affected</returns>
        public async Task<int> UpdateWireSize(MWireSize wireSize)
        {
            try
            {
                logger.WriteInfo($"Updating wire size ID: {wireSize.Id}");

                wireSize.ModifiedDate = DateTime.Now;
                appDbContext.MWireSizes.Update(wireSize);
                int result = await appDbContext.SaveChangesAsync();

                logger.WriteInfo($"Updated wire size ID: {wireSize.Id}");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in UpdateWireSize: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Bulk inserts multiple wire sizes with automatic timestamps
        /// </summary>
        /// <param name="wireSizes">List of wire size entities to insert</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task BulkAddWireSize(List<MWireSize> wireSizes)
        {
            try
            {
                logger.WriteInfo($"Bulk adding {wireSizes.Count} wire sizes");

                foreach (var wireSize in wireSizes)
                {
                    wireSize.CreatedDate = DateTime.Now;
                    wireSize.IsActive = true;
                }

                await appDbContext.MWireSizes.AddRangeAsync(wireSizes);
                await appDbContext.SaveChangesAsync();

                logger.WriteInfo($"Successfully added {wireSizes.Count} wire sizes");
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in BulkAddWireSize: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a dictionary of wire sizes keyed by their IDs
        /// </summary>
        /// <param name="ids">List of wire size IDs to retrieve</param>
        /// <returns>Dictionary of wire sizes with ID as key</returns>
        public async Task<Dictionary<int, MWireSize>> GetWireSizeList(List<int> ids)
        {
            try
            {
                logger.WriteInfo($"Getting wire size list for IDs: {string.Join(",", ids)}");

                var result = await appDbContext.MWireSizes
                    .Where(x => ids.Contains(x.Id) && x.IsActive)
                    .ToDictionaryAsync(x => x.Id);

                logger.WriteInfo($"Found {result.Count} wire sizes");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in GetWireSizeList: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Bulk inserts wire sizes and returns a success message with count
        /// </summary>
        /// <param name="wireSizes">List of wire size entities to insert</param>
        /// <returns>Success message with count of inserted records</returns>
        public async Task<string> BulkInsertWireSizes(List<MWireSize> wireSizes)
        {
            try
            {
                logger.WriteInfo($"Starting bulk insert of {wireSizes.Count} wire sizes");

                foreach (var wireSize in wireSizes)
                {
                    wireSize.CreatedDate = DateTime.Now;
                    wireSize.IsActive = true;
                }

                await appDbContext.MWireSizes.AddRangeAsync(wireSizes);
                int inserted = await appDbContext.SaveChangesAsync();

                string message = $"Successfully inserted {inserted} wire sizes";
                logger.WriteInfo(message);
                return message;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in BulkInsertWireSizes: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a wire size by its unique code
        /// </summary>
        /// <param name="wireSizeCode">Wire size code to search for</param>
        /// <returns>Matching wire size entity or null if not found</returns>
        public async Task<MWireSize> GetWireSizeByCode(string wireSizeCode)
        {
            try
            {
                logger.WriteInfo($"Getting wire size by code: {wireSizeCode}");

                var result = await appDbContext.MWireSizes
                    .FirstOrDefaultAsync(x => x.WireSizeCode == wireSizeCode && x.IsActive);

                if (result == null)
                {
                    logger.WriteInfo($"Wire size not found for code: {wireSizeCode}");
                }
                else
                {
                    logger.WriteInfo($"Found wire size ID: {result.Id} for code: {wireSizeCode}");
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in GetWireSizeByCode: {ex.Message}");
                throw;
            }
        }
    }
}