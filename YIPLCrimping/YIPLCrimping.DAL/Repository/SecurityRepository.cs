using Microsoft.EntityFrameworkCore;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL.Repository
{
    public class SecurityRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly CommonDbContext commonDbContext;

        //private readonly YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public SecurityRepository(AppDbContext appDbContext, CommonDbContext commonDbContext)
        {
            this.appDbContext = appDbContext;
            this.commonDbContext = commonDbContext;
        }

        #region User Management

        public async Task<List<UserAccount>> Get(string? employeeId, string? searchText, int? id)
        {
            try
            {
                var query = appDbContext.UserAccount.Include(plant => plant.MPlant).Include(dept => dept.Department).Include(role => role.MRoleCode)
                    .Where(x => x.IsActive)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(employeeId))
                {
                    query = query.Where(x => x.EmployeeId == employeeId);
                }

                if (id > 0)
                {
                    query = query.Where(x => x.Id == id);
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    string lowerSearch = searchText.ToLower();
                    query = query.Where(x => x.UserName.ToLower().Contains(lowerSearch) ||
                                             x.Email.ToLower().Contains(lowerSearch));
                }

                return await query
                    .OrderBy(x => x.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MasterRepository.GetUsersAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<int> Add(UserAccount user)
        {
            user.IsActive = true;
            user.CreatedDate = DateTime.UtcNow;

            await appDbContext.UserAccount.AddAsync(user);
            int added = await appDbContext.SaveChangesAsync();

            return added;
        }

        public async Task<bool> EmployeeIdExistsAsync(string employeeId)
        {
            return await appDbContext.UserAccount
                .AnyAsync(x => x.EmployeeId == employeeId && x.IsActive == true);
        }

        public async Task<int> Update(UserAccount user)
        {
            appDbContext.UserAccount.Update(user);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<string?> Login(string employeeId)
        {
            try
            {
                var user = await appDbContext.UserAccount.Include(role => role.MRoleCode)
                    .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.IsActive == true);

                if (user == null)
                    return null;

                return TokenHelper.GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in SecurityRepository.AuthenticateAndGenerateTokenAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<UserDetails> GetCommonDbUser(string employeeId)
        {
            var user = await commonDbContext.UserDetails.FirstOrDefaultAsync(employee => employee.Title == employeeId);
            return user;
        }

        public async Task<UserAccount> Get(string employeeId)
        {
            var user = await appDbContext.UserAccount
                .Include(role => role.MRoleCode)
                .Include(dept => dept.Department)
                .Include(plant => plant.MPlant)
                .FirstOrDefaultAsync(employee => employee.EmployeeId == employeeId && employee.IsActive);
            return user;
        }

        public async Task<UserAccount> GetById(int id)
        {
            var user = await appDbContext.UserAccount
                .FirstOrDefaultAsync(employee => employee.Id == id && employee.IsActive);
            return user;
        }

        public async Task<Dictionary<int, UserAccount>> GetExistingUsersById(List<int?> ids)
        {
            var nonNullIds = ids.Where(id => id.HasValue).Select(id => id.Value).ToList();
            return await appDbContext.UserAccount
                .Where(u => nonNullIds.Contains(u.Id ?? 0))
                .ToDictionaryAsync(u => u.Id ?? 0);
        }

        public async Task<int> BulkAddOrUpdateUsers(List<UserAccount> newUsers, List<UserAccount> updatedUsers)
        {
            int processed = 0;

            if (newUsers?.Any() == true)
            {
                await appDbContext.UserAccount.AddRangeAsync(newUsers);
                processed += newUsers.Count;
            }

            if (updatedUsers?.Any() == true)
            {
                appDbContext.UserAccount.UpdateRange(updatedUsers);
                processed += updatedUsers.Count;
            }

            await appDbContext.SaveChangesAsync();
            return processed;
        }

        #endregion User Management

        #region Role Master

        public async Task<List<MRole>> GetRolesAsync()
        {
            try
            {
                return await appDbContext.MRoles
                    .Where(x => x.IsActive == true)
                    .OrderBy(x => x.RoleName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MasterRepository.GetRolesAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<MRole> GetRoleAsync(int roleId)
        {
            return await appDbContext.MRoles
                .FirstOrDefaultAsync(r => r.Id == roleId && r.IsActive);
        }

        public async Task<bool> RoleNameExistsAsync(string roleName)
        {
            try
            {
                return await appDbContext.MRoles
                    .AnyAsync(r => r.RoleName.ToLower() == roleName.ToLower() && r.IsActive);
            }
            catch (Exception ex)
            {
                throw; // Just 'throw' is better than 'throw ex' to preserve stack trace
            }
        }

        public async Task<bool> IsRoleInUseAsync(int roleId)
        {
            return await appDbContext.UserAccount
                .AnyAsync(u => u.RoleCode == roleId && u.IsActive);
        }

        public async Task<int> AddRoleAsync(MRole role)
        {
            await appDbContext.MRoles.AddAsync(role);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateRoleAsync(MRole role)
        {
            appDbContext.MRoles.Update(role);
            return await appDbContext.SaveChangesAsync();
        }

        #endregion Role Master

        #region activity logs

        /// <summary>
        /// Retrieves activity logs based on optional filters
        /// </summary>
        /// <param name="userId">Filter by UserId</param>
        /// <param name="plantId">Filter by PlantId</param>
        /// <param name="fromDate">Start date</param>
        /// <param name="toDate">End date</param>
        /// <returns>List of matching activity logs</returns>
        public async Task<List<ActivityLog>> GetActivityLogs(int? userId, int? plantId, DateTime? fromDate, DateTime? toDate)
        {
            var query = appDbContext.ActivityLogs.AsQueryable();

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId);

            if (plantId.HasValue)
                query = query.Where(x => x.PlantId == plantId);

            if (fromDate.HasValue)
                query = query.Where(x => x.CreatedDate >= fromDate);

            if (toDate.HasValue)
                query = query.Where(x => x.CreatedDate <= toDate);

            return await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
        }

        /// <summary>
        /// Adds a new activity log to the database
        /// </summary>
        /// <param name="log">ActivityLog entity to add</param>
        /// <returns>Number of records affected</returns>
        public async Task<int> AddActivityLog(ActivityLog log)
        {
            try
            {
                appDbContext.ActivityLogs.Add(log);
                int added = await appDbContext.SaveChangesAsync();
                return added;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion activity logs
    }
}