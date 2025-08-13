using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL.Repository
{
    public class PlantRepository
    {
        private readonly AppDbContext appDbContext;

        //private readonly YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public PlantRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<bool> PlantCodeExistsAsync(string plantCode)
        {
            return await appDbContext.MPlants.AnyAsync(p => p.PlantCode == plantCode);
        }
        public async Task<List<MPlant>> GetPlants(string? plantCode, string? plantName,string city, int? id)
        {
            try
            {
                var query = appDbContext.MPlants
                    .Where(x => x.IsActive)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(plantCode))
                {
                    query = query.Where(x => x.PlantCode != null && x.PlantCode.ToLower().Contains(plantCode.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(plantName))
                {
                    query = query.Where(x => x.PlantName != null && x.PlantName.ToLower().Contains(plantName.ToLower()));
                }
                if (!string.IsNullOrWhiteSpace(city))
                {
                    query = query.Where(x => x.City != null && x.City.ToLower().Contains(city.ToLower()));
                }

                if (id.HasValue && id > 0)
                {
                    query = query.Where(x => x.Id == id.Value);
                }

                return await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MasterRepository.GetPlants: {ex.Message}");
                throw;
            }
        }

        public async Task<int> AddPlant(MPlant plant)
        {
            plant.IsActive = true;
            plant.CreatedDate = DateTime.UtcNow;

            await appDbContext.MPlants.AddAsync(plant);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<MPlant?> GetByCode(string plantCode)
        {
            return await appDbContext.MPlants
                .FirstOrDefaultAsync(p => p.PlantCode == plantCode && p.IsActive);
        }

        public async Task<MPlant?> GetById(int id)
        {
            return await appDbContext.MPlants
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> Update(MPlant plant)
        {
            appDbContext.MPlants.Update(plant);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<Dictionary<int, MPlant>> GetExistingPlantsById(List<int> ids)
        {
            return await appDbContext.MPlants
                .Where(x => ids.Contains(x.Id) && x.IsActive)
                .ToDictionaryAsync(x => x.Id);
        }

        public async Task<int> BulkAddOrUpdatePlant(List<MPlant> newPlants, List<MPlant> updatedPlants)
        {
            int processed = 0;

            if (newPlants?.Any() == true)
            {
                await appDbContext.MPlants.AddRangeAsync(newPlants);
                processed += newPlants.Count;
            }

            if (updatedPlants?.Any() == true)
            {
                appDbContext.MPlants.UpdateRange(updatedPlants);
                processed += updatedPlants.Count;
            }

            await appDbContext.SaveChangesAsync();
            return processed;
        }

        //public async Task<string> PlantBulkUploadAsync(List<MPlant> plants, int userId, int duplicateCount)
        //{
        //    try
        //    {
        //        //await DeletePlantAsync(0, userId).ConfigureAwait(false);
        //        foreach (var plant in plants)
        //        {
        //            plant.CreatedById = userId;
        //            plant.CreatedDate = DateTime.UtcNow;
        //            plant.IsActive = true;
        //            await appDbContext.MPlants.AddAsync(plant);
        //        }
        //        await appDbContext.SaveChangesAsync();
        //        //return $"Imported {plants.Count} plants successfully.";
        //        if (plants.Count > 0)
        //        {
        //            return $"Successfully added {plants.Count} new plant(s). {duplicateCount} duplicate(s) were skipped.";
        //        }
        //        else
        //        {
        //            return $"All {duplicateCount} plant(s) already exist. No new plants were added.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.WriteError($"Error in PlantBulkUploadAsync: {ex.Message}");
        //        throw;
        //    }
        //}

        public async Task<string> PlantBulkUploadAsync(List<MPlant> plants, int userId, int duplicateCount, int totalSkipped)
        {
            try
            {
                foreach (var plant in plants)
                {
                    plant.CreatedById = userId;
                    plant.CreatedDate = DateTime.UtcNow;
                    plant.IsActive = true;
                    await appDbContext.MPlants.AddAsync(plant);
                }
                await appDbContext.SaveChangesAsync();

                if (plants.Count > 0)
                {
                    return $"Successfully added {plants.Count} new plant(s). {duplicateCount} duplicate(s) and {totalSkipped} record(s) with empty PlantCode/ Plant Name/ City were skipped.";
                }
                else
                {
                    return $"All {duplicateCount} plant(s) were duplicates and {totalSkipped} record(s) had empty PlantCode/ Plant Name/ City. No new plants were added.";
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in PlantBulkUploadAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeletePlantAsync(int id, int updatedBy)
        {
            try
            {
                if (id > 0)
                {
                    logger.WriteDebug($"Attempting to soft delete MPlant with ID {id} in MasterRepository.DeletePlantAsync.");

                    var plant = await appDbContext.MPlants.FirstOrDefaultAsync(x => x.Id == id);
                    if (plant == null)
                        return false;

                    plant.IsActive = false;
                    plant.ModifiedById = updatedBy;
                    plant.ModifiedDate = DateTime.UtcNow;

                    appDbContext.MPlants.Update(plant);
                }
                else
                {
                    logger.WriteDebug("Soft deleting all active MPlants using raw SQL.");

                    await appDbContext.Database.ExecuteSqlRawAsync(@"
        UPDATE M_Plant
        SET IsActive = 0,
            UpdatedOn = GETUTCDATE(),
            UpdatedBy = {0}
        WHERE IsActive = 1", updatedBy);
                }

                await appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MasterRepository.DeletePlantAsync: {ex.Message}");
                throw;
            }
        }
    }
}