using Microsoft.EntityFrameworkCore;
using YIPLCrimping.DAL.Models;

namespace YIPLCrimping.DAL.Repository
{
    public class MachineRepository
    {
        private readonly AppDbContext appDbContext;

        //private readonly YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public MachineRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<MMachine>> GetMachines(string? machineName, int? id, int? plantId)
        {
            try
            {
                var query = appDbContext.MMachines
                    .Include(m => m.Plant)
                    .Where(x => x.IsActive)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(machineName))
                {
                    query = query.Where(x => x.MachineName.ToLower().Contains(machineName.ToLower()));
                }

                if (id.HasValue && id > 0)
                {
                    query = query.Where(x => x.Id == id.Value);
                }

                if (plantId.HasValue && plantId > 0)
                {
                    query = query.Where(x => x.PlantId == plantId.Value);
                }

                var list = await query.OrderBy(x => x.Id).Select(x => new MMachine
                {
                    Id = x.Id,
                    MachineName = x.MachineName,
                    MachineCost = Convert.ToDecimal(x.MachineCost),
                    PlantId = x.PlantId,
                    Plant = x.Plant,
                    CreatedById = x.CreatedById,
                    CreatedDate = x.CreatedDate,
                    ModifiedById = x.ModifiedById,
                    ModifiedDate = x.ModifiedDate,
                    IsActive = x.IsActive
                }).ToListAsync();
                list = list.OrderByDescending(x => x.CreatedDate).ToList();
                return list;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MachineRepository.GetMachines: {ex.Message}");
                throw;
            }
        }

        public async Task<int> AddMachine(MMachine machine)
        {
            machine.IsActive = true;
            machine.CreatedDate = DateTime.UtcNow;

            await appDbContext.MMachines.AddAsync(machine);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<MMachine> MachineNameExistsAsync(string name)
        {
            var query = appDbContext.MMachines
                    .Where(x => x.IsActive)
                    .AsQueryable();
            var list = await query.OrderBy(x => x.Id).Select(x => new MMachine
            {
                Id = x.Id,
                MachineName = x.MachineName,
                MachineCost = Convert.ToDecimal(x.MachineCost),
                PlantId = x.PlantId,
                Plant = x.Plant,
                CreatedById = x.CreatedById,
                CreatedDate = x.CreatedDate,
                ModifiedById = x.ModifiedById,
                ModifiedDate = x.ModifiedDate,
                IsActive = x.IsActive
            }).ToListAsync();
            var result = list.FirstOrDefault(c => c.MachineName == name);
            return result;
        }

        public async Task<MMachine?> GetById(int machineId)
        {
            var query = appDbContext.MMachines
                    .Where(x => x.IsActive)
                    .AsQueryable();
            var list = await query.OrderBy(x => x.Id).Select(x => new MMachine
            {
                Id = x.Id,
                MachineName = x.MachineName,
                MachineCost = Convert.ToDecimal(x.MachineCost),
                PlantId = x.PlantId,
                Plant = x.Plant,
                CreatedById = x.CreatedById,
                CreatedDate = x.CreatedDate,
                ModifiedById = x.ModifiedById,
                ModifiedDate = x.ModifiedDate,
                IsActive = x.IsActive
            }).ToListAsync();
            var result = list.FirstOrDefault(c => c.Id == machineId);
            return result;
        }

        public async Task<int> Update(MMachine machine)
        {
            appDbContext.MMachines.Update(machine);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<Dictionary<int, MMachine>> GetExistingMachinesById(List<int> ids)
        {
            var query = appDbContext.MMachines
                    .Where(x => x.IsActive)
                    .AsQueryable();
            var list = await query.OrderBy(x => x.Id).Select(x => new MMachine
            {
                Id = x.Id,
                MachineName = x.MachineName,
                MachineCost = Convert.ToDecimal(x.MachineCost),
                PlantId = x.PlantId,
                Plant = x.Plant,
                CreatedById = x.CreatedById,
                CreatedDate = x.CreatedDate,
                ModifiedById = x.ModifiedById,
                ModifiedDate = x.ModifiedDate,
                IsActive = x.IsActive
            }).ToListAsync();
            var result = list.Where(m => ids.Contains(m.Id))
                .ToDictionary(m => m.Id);
            return result;
        }

        public async Task<int> BulkAddOrUpdateMachine(List<MMachine> newMachines, List<MMachine> updatedMachines)
        {
            int processed = 0;
            if (newMachines?.Any() == true)
            {
                await appDbContext.MMachines.AddRangeAsync(newMachines);
                processed += newMachines.Count;
            }
            if (updatedMachines?.Any() == true)
            {
                appDbContext.MMachines.UpdateRange(updatedMachines);
                processed += updatedMachines.Count;
            }
            await appDbContext.SaveChangesAsync();
            return processed;
        }

        //public async Task<string> MachineBulkUploadAsync(List<MMachine> machines, int userId)
        //{
        //    try
        //    {
        //        //await DeleteMachineAsync(0, userId).ConfigureAwait(false);
        //        foreach (var m in machines)
        //        {
        //            m.MachineCost = Convert.ToDecimal(m.MachineCost);
        //            m.Plant = null;
        //            m.CreatedById = userId;
        //            m.CreatedDate = DateTime.UtcNow;
        //            m.IsActive = true;
        //            await appDbContext.MMachines.AddAsync(m);
        //        }
        //        await appDbContext.SaveChangesAsync();
        //        return $"Imported {machines.Count} machines successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.WriteError($"Error in MachineBulkUploadAsync: {ex.Message}");
        //        throw;
        //    }
        //}

        public async Task<string> MachineBulkUploadAsync(List<MMachine> machines, int userId, int duplicateCount)
        {
            try
            {
                //await DeleteMachineAsync(0, userId).ConfigureAwait(false);
                foreach (var m in machines)
                {
                    m.MachineCost = Convert.ToDecimal(m.MachineCost);
                    m.Plant = null;
                    m.CreatedById = userId;
                    m.CreatedDate = DateTime.UtcNow;
                    m.IsActive = true;
                    await appDbContext.MMachines.AddAsync(m);
                }
                await appDbContext.SaveChangesAsync();
                if (machines.Count > 0)
                {
                    return $"Successfully added {machines.Count} new machines(s). {duplicateCount} duplicate(s) were skipped.";
                }
                else
                {
                    return $"All {duplicateCount} machines(s) already exist. No new machines were added.";
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MachineBulkUploadAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteMachineAsync(int id, int updatedBy)
        {
            try
            {
                if (id > 0)
                {
                    var machine = await appDbContext.MMachines.FirstOrDefaultAsync(x => x.Id == id);
                    if (machine == null) return false;
                    machine.IsActive = false;
                    machine.ModifiedById = updatedBy;
                    machine.ModifiedDate = DateTime.UtcNow;
                    appDbContext.MMachines.Update(machine);
                }
                else
                {
                    await appDbContext.Database.ExecuteSqlRawAsync(@"
                                                            UPDATE M_Machine
                                                            SET IsActive = 0,
                                                                ModifiedDate = GETUTCDATE(),
                                                                ModifiedById = {0}
                                                            WHERE IsActive = 1", updatedBy);
                }
                await appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in DeleteMachineAsync: {ex.Message}");
                throw;
            }
        }
    }
}