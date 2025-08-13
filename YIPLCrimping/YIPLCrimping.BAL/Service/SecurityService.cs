using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using YIPLCrimping.DAL.Models;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.BAL.Service
{
    public class SecurityService
    {
        private readonly SecurityRepository securityRepository;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public SecurityService(SecurityRepository securityRepository)
        {
            this.securityRepository = securityRepository;
        }

        #region User Management

        public async Task<List<UserAccount>> Get(JObject requestData)
        {
            try
            {
                string? employeeId = requestData["employeeId"]?.ToString();
                int? id = requestData["id"]?.ToObject<int>();
                string? searchText = requestData["searchText"]?.ToString();

                var users = await securityRepository.Get(employeeId, searchText, id);
                logger.WriteInfo($"Retrieved {users.Count} users from repository.");
                return users;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MasterService.GetUsersAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<JObject> Add(JObject userData)
        {
            var result = new JObject();
            var user = JsonConvert.DeserializeObject<UserAccount>(userData.ToString());
            if (string.IsNullOrWhiteSpace(user.EmployeeId))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.EmployeeIdIsRequired_1003, 400, ErrorNumber.EmployeeIdIsRequired);
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.UserNameIsRequired_1004, 400, ErrorNumber.UserNameIsRequired);
            }

            if (user.UserName.Any(char.IsDigit))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.UserNameInvalid_1005, 400, ErrorNumber.UserNameInvalid);
            }

            if (user.RoleCode <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.RoleCodeIsRequired_1006, 400, ErrorNumber.RoleCodeIsRequired);
            }

            if (user.Plant <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.PlantIsRequired_1007, 400, ErrorNumber.PlantIsRequired);
            }

            if (user.DepartmentId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.DepartmentIdIsRequired_1008, 400, ErrorNumber.DepartmentIdIsRequired);
            }

            if (user.CreatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                // Basic email pattern check
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(user.Email, emailPattern))
                {
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.EmailInvalid_1012, 400, ErrorNumber.EmailInvalid);
                }
            }

            if (!string.IsNullOrWhiteSpace(user.EmployeeId))
            {
                bool exists = await securityRepository.EmployeeIdExistsAsync(user.EmployeeId);
                if (exists)
                {
                    string error = string.Format(ErrorDescription.EmployeeIdAlreadyExists_1001, user.EmployeeId);
                    return ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.EmployeeIdAlreadyExists);
                }
            }
            var userExistInCommonDb = await securityRepository.GetCommonDbUser(user.EmployeeId);
            if (userExistInCommonDb != null)
            {
                var addedUser = await securityRepository.Add(user);

                if (addedUser > 0)
                {
                    logger.WriteInfo($"User {user.Id}, {user.UserName} is added successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.UserAddedSuccessfully);
                    result.Add("Message", SuccessDescription.UserAddedSuccessfully_2001);
                }
                else
                {
                    logger.WriteInfo($"User {user.Id} could not be added.");
                    result.Add("Success", true);
                    result.Add("Message", ErrorDescription.FailedToAddUser_1002);
                }
            }
            else
            {
                logger.WriteInfo($"User {user.EmployeeId} not found in common database.");
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.UserNotFound_1010, user.EmployeeId),
                    404,
                    ErrorNumber.UserNotFound
                );
            }
            return result;
        }

        public async Task<JObject> Update(JObject userData)
        {
            var result = new JObject();
            var user = JsonConvert.DeserializeObject<UserAccount>(userData.ToString());

            // Validate employeeId
            if (user.Id == null || user.Id <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.EmployeeIdIsRequired_1003, 400, ErrorNumber.EmployeeIdIsRequired);
            }

            if (user.ModifiedById == null || user.ModifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired);
            }
            // Check if user exists
            var existingUser = await securityRepository.GetById(user.Id ?? 0);

            if (existingUser != null)
            {
                // Update fields only if provided
                if (!string.IsNullOrWhiteSpace(user.UserName))
                {
                    if (user.UserName.Any(char.IsDigit))
                    {
                        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.UserNameInvalid_1005, 400, ErrorNumber.UserNameInvalid);
                    }

                    existingUser.UserName = user.UserName;
                }

                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    // Basic email pattern check
                    var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                    if (!Regex.IsMatch(user.Email, emailPattern))
                    {
                        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.EmailInvalid_1012, 400, ErrorNumber.EmailInvalid);
                    }

                    existingUser.Email = user.Email;
                }

                if (user.RoleCode > 0)
                {
                    existingUser.RoleCode = user.RoleCode;
                }

                if (user.Plant > 0)
                {
                    existingUser.Plant = user.Plant;
                }

                if (user.DepartmentId > 0)
                {
                    existingUser.DepartmentId = user.DepartmentId;
                }

                // Set metadata
                existingUser.ModifiedById = user.ModifiedById;
                existingUser.ModifiedDate = DateTime.UtcNow;

                // Save changes
                int updated = await securityRepository.Update(existingUser);

                if (updated > 0)
                {
                    logger.WriteInfo($"User {existingUser.Id}, {existingUser.UserName} updated successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.UserUpdatedSuccessfully);
                    result.Add("Message", SuccessDescription.UserUpdatedSuccessfully_2002);
                }
                else
                {
                    logger.WriteInfo($"User {existingUser.Id} could not be updated.");
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToUpdateUser_1011, 500, ErrorNumber.FailedToUpdateUser);
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.UserNotFound_1010, user.EmployeeId),
                    404,
                    ErrorNumber.UserNotFound
                );
            }
            return result;
        }

        public async Task<JObject> Delete(JObject userData)
        {
            var result = new JObject();
            var employeeId = userData["employeeId"].ToObject<int>();
            var modifiedById = userData["modifiedById"].ToObject<int>();

            // Validate employeeId
            //if (string.IsNullOrWhiteSpace(employeeId))
            //{
            //    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.EmployeeIdIsRequired_1003, 400, ErrorNumber.EmployeeIdIsRequired);
            //}
            if (employeeId == null || employeeId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.EmployeeIdIsRequired_1003, 400, ErrorNumber.EmployeeIdIsRequired);
            }

            if (modifiedById == null || modifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired);
            }
            // Check if user exists
            var existingUser = await securityRepository.GetById(employeeId);

            if (existingUser != null)
            {
                // Set metadata
                existingUser.IsActive = false;
                existingUser.ModifiedById = modifiedById;
                existingUser.ModifiedDate = DateTime.UtcNow;

                // Save changes
                int updated = await securityRepository.Update(existingUser);

                if (updated > 0)
                {
                    logger.WriteInfo($"User {existingUser.Id}, {existingUser.UserName} deleted successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.UserDeletedSuccessfully);
                    result.Add("Message", SuccessDescription.UserDeletedSuccessfully_2003);
                }
                else
                {
                    logger.WriteInfo($"User {existingUser.Id} could not be updated.");
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToUpdateUser_1011, 500, ErrorNumber.FailedToUpdateUser);
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.UserNotFound_1010, employeeId),
                    404,
                    ErrorNumber.UserNotFound
                );
            }
            return result;
        }

        public async Task<JObject> Login(string employeeId)
        {
            var result = new JObject();
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                logger.WriteInfo("LoginAsync() called with empty employeeId.");
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.EmployeeIdIsRequired_1003, 400, ErrorNumber.EmployeeIdIsRequired);
            }

            var commonDbUser = await securityRepository.GetCommonDbUser(employeeId);
            if (commonDbUser != null)
            {
                var user = await securityRepository.Get(employeeId);
                if (user != null)
                {
                    var token = await securityRepository.Login(employeeId);
                    if (token != null)
                    {
                        result.Add("Token", token);
                        result.Add("EmployeeId", user.Id);
                        result.Add("EmployeeCode", user.EmployeeId);
                        result.Add("UserName", user.UserName);
                        result.Add("RoleCode", user.RoleCode);
                        result.Add("RoleName", user.MRoleCode.RoleName);
                        result.Add("PlantId", user.Plant);
                        result.Add("PlantCode", user.MPlant.PlantCode);
                        result.Add("PlantName", user.MPlant.PlantName);
                        result.Add("DepartmentId", user.DepartmentId);
                        result.Add("DepartmentName", user.Department.DeptName);
                    }
                }
                else
                {
                    logger.WriteInfo($"Login failed for EmployeeId: {employeeId}. User not found in common database.");
                    return ErrorResponseWrapper.CreateErrorResponse(string.Format(ErrorDescription.UserNotFound_1010, employeeId), 404, ErrorNumber.UserNotFound);
                }
            }
            else
            {
                logger.WriteInfo($"Login failed for EmployeeId: {employeeId}. User not found in common database.");
                return ErrorResponseWrapper.CreateErrorResponse(string.Format(ErrorDescription.UserNotFound_1010, employeeId), 404, ErrorNumber.UserNotFound);
            }
            return result;
        }

        public async Task<JObject> BulkAddOrUpdate(List<UserAccount> users)
        {
            var result = new JObject();

            if (users == null || !users.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoUserRecordsProvided_1035,
                    400,
                    ErrorNumber.NoUserRecordsProvided
                );
            }

            var currentUtc = DateTime.UtcNow;
            var updateIds = users.Where(u => u.Id > 0).Select(u => u.Id).ToList();
            var existingMap = await securityRepository.GetExistingUsersById(updateIds);

            var newUsers = new List<UserAccount>();
            var updatedUsers = new List<UserAccount>();
            var duplicateEmployeeIds = new List<string>();
            var validationErrors = new List<string>();

            foreach (var user in users)
            {
                // Validate required fields for new users
                if (user.Id <= 0) // New user
                {
                    if (string.IsNullOrWhiteSpace(user.EmployeeId))
                    {
                        validationErrors.Add($"EmployeeId is required for new user");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(user.UserName))
                    {
                        validationErrors.Add($"UserName is required for new user (EmployeeId: {user.EmployeeId})");
                        continue;
                    }

                    if (user.RoleCode <= 0)
                    {
                        validationErrors.Add($"RoleCode is required for new user (EmployeeId: {user.EmployeeId})");
                        continue;
                    }

                    if (user.Plant <= 0)
                    {
                        validationErrors.Add($"Plant is required for new user (EmployeeId: {user.EmployeeId})");
                        continue;
                    }

                    if (user.DepartmentId <= 0)
                    {
                        validationErrors.Add($"DepartmentId is required for new user (EmployeeId: {user.EmployeeId})");
                        continue;
                    }

                    if (user.CreatedById <= 0)
                    {
                        validationErrors.Add($"CreatedById is required for new user (EmployeeId: {user.EmployeeId})");
                        continue;
                    }

                    // Check if user exists in common DB
                    var userExistInCommonDb = await securityRepository.GetCommonDbUser(user.EmployeeId);
                    if (userExistInCommonDb == null)
                    {
                        validationErrors.Add($"User {user.EmployeeId} not found in common database");
                        continue;
                    }

                    // Check for duplicate EmployeeId in this batch
                    if (newUsers.Any(u => u.EmployeeId == user.EmployeeId) ||
                        await securityRepository.EmployeeIdExistsAsync(user.EmployeeId))
                    {
                        duplicateEmployeeIds.Add(user.EmployeeId);
                        continue;
                    }

                    var newUser = new UserAccount
                    {
                        EmployeeId = user.EmployeeId,
                        UserName = user.UserName,
                        Email = user.Email,
                        RoleCode = user.RoleCode,
                        Plant = user.Plant,
                        DepartmentId = user.DepartmentId,
                        CreatedById = user.CreatedById,
                        CreatedDate = currentUtc,
                        IsActive = true
                    };
                    newUsers.Add(newUser);
                }
                else // Existing user
                {
                    if (!existingMap.TryGetValue(user.Id.Value, out var existingUser))
                    {
                        validationErrors.Add($"User with ID {user.Id} not found");
                        continue;
                    }

                    if (user.ModifiedById <= 0)
                    {
                        validationErrors.Add($"ModifiedById is required for existing user (ID: {user.Id})");
                        continue;
                    }

                    // Update fields if provided
                    if (!string.IsNullOrWhiteSpace(user.UserName))
                        existingUser.UserName = user.UserName;

                    if (!string.IsNullOrWhiteSpace(user.Email))
                        existingUser.Email = user.Email;

                    if (user.RoleCode > 0)
                        existingUser.RoleCode = user.RoleCode;

                    if (user.Plant > 0)
                        existingUser.Plant = user.Plant;

                    if (user.DepartmentId > 0)
                        existingUser.DepartmentId = user.DepartmentId;

                    existingUser.ModifiedById = user.ModifiedById;
                    existingUser.ModifiedDate = currentUtc;

                    updatedUsers.Add(existingUser);
                }
            }

            if (validationErrors.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    $"Validation errors: {string.Join("; ", validationErrors)}",
                    400,
                    ErrorNumber.UserValidationFailed
                );
            }

            if (newUsers.Count > 0 || updatedUsers.Count > 0)
            {
                int processed = await securityRepository.BulkAddOrUpdateUsers(newUsers, updatedUsers);

                if (processed > 0)
                {
                    logger.WriteInfo($"Bulk user add/update completed. {processed} records processed.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.UserBulkAddOrUpdateSuccess);

                    string addMessage = $"{newUsers.Count} record{(newUsers.Count == 1 ? "" : "s")} added";
                    string updateMessage = $"{updatedUsers.Count} record{(updatedUsers.Count == 1 ? "" : "s")} updated";
                    string skipMessage = $"{duplicateEmployeeIds.Count} duplicate record{(duplicateEmployeeIds.Count == 1 ? "" : "s")} skipped";

                    result.Add("Message", $"Bulk user add/update completed. {addMessage}, {updateMessage}, and {skipMessage}.");
                    result.Add("ProcessedCount", processed);

                    if (duplicateEmployeeIds.Any())
                    {
                        result.Add("SkippedDuplicates", JArray.FromObject(duplicateEmployeeIds.Distinct()));
                        result.Add("WarningMessage", $"Skipped {duplicateEmployeeIds.Count} duplicate EmployeeId(s): {string.Join(", ", duplicateEmployeeIds.Distinct())}");
                    }
                }
                else
                {
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.UserBulkAddOrUpdateFailed_1036,
                        500,
                        ErrorNumber.UserBulkAddOrUpdateFailed
                    );
                }
            }
            else if (duplicateEmployeeIds.Count > 0)
            {
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.UserBulkAddOrUpdateSuccess);
                string duplicateList = string.Join(", ", duplicateEmployeeIds.Distinct());
                string duplicateLabel = duplicateEmployeeIds.Count == 1 ? "duplicate EmployeeId was" : "duplicate EmployeeIds were";

                result.Add("Message", $"{duplicateEmployeeIds.Count} {duplicateLabel} found and skipped. Please provide unique EmployeeId values. Skipped IDs: {duplicateList}");
            }

            return result;
        }
        #endregion User Management

        #region Role Master

        public async Task<List<MRole>> GetRolesAsync()
        {
            try
            {
                var roles = await securityRepository.GetRolesAsync();
                logger.WriteInfo($"Retrieved {roles.Count} roles from repository.");
                return roles;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MasterRepository.GetRolesAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<JObject> AddRoleAsync(JObject roleData)
        {
            var result = new JObject();
            var role = JsonConvert.DeserializeObject<MRole>(roleData.ToString());

            if (string.IsNullOrWhiteSpace(role.RoleName))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.RoleNameIsRequired_1101, 400, ErrorNumber.RoleNameIsRequired);
            }
            if (role.CreatedById == null || role.CreatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);
            }
            // Check if role name already exists
            bool exists = await securityRepository.RoleNameExistsAsync(role.RoleName);
            if (exists)
            {
                string error = string.Format(ErrorDescription.RoleNameAlreadyExists_1102, role.RoleName);
                return ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.RoleNameAlreadyExists);
            }

            if (role.CreatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);
            }

            role.IsActive = true;
            role.CreatedDate = DateTime.UtcNow;

            int added = await securityRepository.AddRoleAsync(role);

            if (added > 0)
            {
                logger.WriteInfo($"Role {role.Id}, {role.RoleName} is added successfully.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.RoleAddedSuccessfully);
                result.Add("Message", SuccessDescription.RoleAddedSuccessfully_2101);
            }
            else
            {
                logger.WriteInfo($"Role {role.RoleName} could not be added.");
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToAddRole_1103, 500, ErrorNumber.FailedToAddRole);
            }

            return result;
        }

        public async Task<JObject> UpdateRoleAsync(JObject roleData)
        {
            var result = new JObject();
            var role = JsonConvert.DeserializeObject<MRole>(roleData.ToString());

            if (role.Id <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.RoleIdIsRequired_1104, 400, ErrorNumber.RoleIdIsRequired);
            }
            if (role.ModifiedById == null || role.ModifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired);
            }

            // Check if role exists
            var existingRole = await securityRepository.GetRoleAsync(role.Id);

            if (existingRole == null)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.RoleNotFound_1105, role.Id),
                    404,
                    ErrorNumber.RoleNotFound
                );
            }

            // Validate role name if provided
            if (!string.IsNullOrWhiteSpace(role.RoleName))
            {
                // Check if new role name is different and already exists
                if (!existingRole.RoleName.Equals(role.RoleName, StringComparison.OrdinalIgnoreCase))
                {
                    bool nameExists = await securityRepository.RoleNameExistsAsync(role.RoleName);
                    if (nameExists)
                    {
                        string error = string.Format(ErrorDescription.RoleNameAlreadyExists_1102, role.RoleName);
                        return ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.RoleNameAlreadyExists);
                    }
                }

                existingRole.RoleName = role.RoleName;
            }

            // Set metadata
            existingRole.ModifiedById = role.ModifiedById;
            existingRole.ModifiedDate = DateTime.UtcNow;
            existingRole.IsActive = true;

            int updated = await securityRepository.UpdateRoleAsync(existingRole);

            if (updated > 0)
            {
                logger.WriteInfo($"Role {existingRole.Id}, {existingRole.RoleName} updated successfully.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.RoleUpdatedSuccessfully);
                result.Add("Message", SuccessDescription.RoleUpdatedSuccessfully_2102);
            }
            else
            {
                logger.WriteInfo($"Role {existingRole.Id} could not be updated.");
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToUpdateRole_1106, 500, ErrorNumber.FailedToUpdateRole);
            }

            return result;
        }

        public async Task<JObject> DeleteRoleAsync(int roleId, int modifiedById)
        {
            var result = new JObject();

            if (roleId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.RoleIdIsRequired_1104, 400, ErrorNumber.RoleIdIsRequired);
            }

            if (modifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired);
            }

            // Check if role exists
            var existingRole = await securityRepository.GetRoleAsync(roleId);

            if (existingRole == null)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.RoleNotFound_1105, roleId),
                    404,
                    ErrorNumber.RoleNotFound
                );
            }

            // Check if role is being used by any user
            bool isInUse = await securityRepository.IsRoleInUseAsync(roleId);
            if (isInUse)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.RoleInUseCannotDelete_1107,
                    400,
                    ErrorNumber.RoleInUseCannotDelete
                );
            }

            // Soft delete
            existingRole.IsActive = false;
            existingRole.ModifiedById = modifiedById;
            existingRole.ModifiedDate = DateTime.UtcNow;

            int deleted = await securityRepository.UpdateRoleAsync(existingRole);

            if (deleted > 0)
            {
                logger.WriteInfo($"Role {existingRole.Id}, {existingRole.RoleName} deleted successfully by user {modifiedById}.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.RoleDeletedSuccessfully);
                result.Add("Message", SuccessDescription.RoleDeletedSuccessfully_2103);
            }
            else
            {
                logger.WriteInfo($"Role {existingRole.Id} could not be deleted by user {modifiedById}.");
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToDeleteRole_1108, 500, ErrorNumber.FailedToDeleteRole);
            }

            return result;
        }

        #endregion Role Master

        #region Activity Log

        public async Task<List<ActivityLog>> GetActivityLogs(JObject requestData)
        {
            int? userId = requestData["userId"]?.ToObject<int>();
            int? plantId = requestData["plantId"]?.ToObject<int>();
            DateTime? fromDate = requestData["fromDate"]?.ToObject<DateTime>();
            DateTime? toDate = requestData["toDate"]?.ToObject<DateTime>();

            var logs = await securityRepository.GetActivityLogs(userId, plantId, fromDate, toDate);
            logger.WriteInfo($"Retrieved {logs.Count} activity logs from repository.");
            return logs;
        }

        public async Task<JObject> AddActivityLog(ActivityLog logData)
        {
            var result = new JObject();

            if (logData.UserId == null || logData.UserId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse("UserId is required.", 400, 1001);
            }

            logData.CreatedDate = DateTime.Now;

            var added = await securityRepository.AddActivityLog(logData);
            if (added > 0)
            {
                logger.WriteInfo($"ActivityLog {logData.Id} added successfully.");
                result.Add("Success", true);
                result.Add("Message", "Activity log added successfully.");
            }
            else
            {
                logger.WriteInfo("ActivityLog could not be added.");
                result.Add("Success", false);
                result.Add("Message", "Failed to add activity log.");
            }

            return result;
        }

        #endregion Activity Log
    }
}