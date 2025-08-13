using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using YIPLCrimping.BAL;
using YIPLCrimping.BAL.Service;
using YIPLCrimping.DAL.Models;
using YIPLCrimping.Helper;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.API.Controllers
{
    [Route("Security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly SecurityService securityService;
        private readonly AppDbContext appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ActivityLoggerHelper _loggerHelper;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        [NonAction]
        public override void InitializeController()
        {
        }

        public SecurityController(AppDbContext DBContext, SecurityService securityService, IHttpContextAccessor httpContextAccessor, ActivityLoggerHelper loggerHelper)
        {
            this.appDbContext = DBContext;
            this.securityService = securityService;
            this._httpContextAccessor = httpContextAccessor;
            this._loggerHelper = loggerHelper;
        }

        #region userManagement

        /// <summary>
        /// Retrieves a list of users based on the provided request data.
        /// </summary>
        /// <param name="requestData">
        /// A <see cref="JObject"/> containing the request parameters used to filter or query users.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing the list of users that match the query criteria.
        /// If no users are found, an empty result is returned.
        /// </returns>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-22</date>
        [HttpPost("get")]
        [AllowAnonymous]
        public async Task<JsonResult> GetUsers(JObject requestData)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // ✅ Check if token is missing
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return new JsonResult(new
                    {
                        Success = false,
                        Message = "Authorization token is missing or invalid."
                    })
                    { StatusCode = 401 };
                }

                // ✅ Remove "Bearer " prefix
                token = token.Substring("Bearer ".Length);
                var result = await securityService.Get(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestData.ToString(), JsonConvert.SerializeObject(result));

                if (result.Count < 0)
                {
                    return await base.FinalizeEmpty();
                }
                else
                {
                    return await base.FinalizeMultiple(result);
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in GetUsers: {ex.InnerException}");
                var errorWrapper = new ApiResponseWrapper<string>(null);
                return null;
            }
        }

        /// <summary>
        /// Adds a new user to the system based on the provided user data.
        /// </summary>
        /// <param name="user">
        /// A <see cref="JObject"/> containing user information including required fields such as
        /// EmployeeId, UserName, RoleCode, Plant, DepartmentId, and CreatedById.
        /// </param>
        /// <returns>
        /// An <see cref="JsonResult"/> containing a success message if the user is added successfully,
        /// or an appropriate error response if validation fails or an exception occurs.
        /// </returns>
        /// <remarks>
        /// Validates the input payload for required fields and value correctness.
        /// Returns HTTP 409 if the EmployeeId already exists, or HTTP 400 for validation errors.
        /// </remarks>
        /// <author>Developer: John Doe</author>
        /// <date>2025-07-21</date>
        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<JsonResult> AddUser([FromBody] JObject user)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // ✅ Check if token is missing
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return new JsonResult(new
                    {
                        Success = false,
                        Message = "Authorization token is missing or invalid."
                    })
                    { StatusCode = 401 };
                }

                // ✅ Remove "Bearer " prefix
                token = token.Substring("Bearer ".Length);

                var response = await securityService.Add(user);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(user);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                else
                {
                    return await base.FinalizeSingle(response);
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Updates an existing user’s information based on the provided data.
        /// </summary>
        /// <param name="user">
        /// A <see cref="JObject"/> containing user data such as EmployeeId, UserName, Email, RoleCode, Plant,
        /// DepartmentId, IsActive, and ModifiedById. Only the fields provided will be updated.
        /// </param>
        /// <returns>
        /// An <see cref="JsonResult"/> that returns a success message if the update is successful,
        /// a not found message if the user does not exist, or a validation error if the input is invalid.
        /// </returns>
        /// <remarks>
        /// The update requires a valid EmployeeId to identify the user.
        /// Only provided fields are updated; all others remain unchanged.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-22</date>
        [HttpPut("update")]
        [AllowAnonymous]
        public async Task<JsonResult> UpdateUser([FromBody] JObject user)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // ✅ Check if token is missing
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return new JsonResult(new
                    {
                        Success = false,
                        Message = "Authorization token is missing or invalid."
                    })
                    { StatusCode = 401 };
                }

                // ✅ Remove "Bearer " prefix
                token = token.Substring("Bearer ".Length);

                var response = await securityService.Update(user);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(user);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                else
                {
                    return await base.FinalizeSingle(response);
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Deactivates (soft-deletes) a user based on the provided EmployeeId.
        /// </summary>
        /// <param name="user">
        /// A <see cref="JObject"/> containing the EmployeeId of the user to delete and the ModifiedById of the user performing the action.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating whether the user was successfully deactivated,
        /// or an error message if the user was not found or validation failed.
        /// </returns>
        /// <remarks>
        /// This operation does not permanently delete the user but sets their IsActive flag to false (soft delete).
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-22</date>
        [HttpPut("delete")]
        [AllowAnonymous]
        public async Task<JsonResult> DeleteUser([FromBody] JObject user)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // ✅ Check if token is missing
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return new JsonResult(new
                    {
                        Success = false,
                        Message = "Authorization token is missing or invalid."
                    })
                    { StatusCode = 401 };
                }

                // ✅ Remove "Bearer " prefix
                token = token.Substring("Bearer ".Length);

                var response = await securityService.Delete(user);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(user);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                else
                {
                    return await base.FinalizeSingle(response);
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Authenticates a user based on the provided EmployeeId and returns a JWT token along with user profile details.
        /// </summary>
        /// <param name="employeeId">
        /// The unique identifier of the user attempting to log in.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing a JWT token and user metadata if login is successful,
        /// or an error response if the user is not found or the EmployeeId is missing.
        /// </returns>
        /// <remarks>
        /// Checks for the user's existence in both the common and application databases.
        /// If found, generates and returns a JWT token with user profile data.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-21</date>
        [HttpGet("login")]
        [AllowAnonymous]
        public async Task<JsonResult> Login([FromQuery] string employeeId)
        {
            try
            {
                var response = await securityService.Login(employeeId);
                await _loggerHelper.LogActivity(
            userId: response?["EmployeeId"]?.Value<int>(),
            plantId: response?["PlantId"]?.Value<int>(),
            request: JsonConvert.SerializeObject(new { employeeId }),
            response: response?.ToString()
        );

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                else
                {
                    return await base.FinalizeSingle(response);
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        [HttpPost("bulkaddorupdate")]
        [AllowAnonymous]
        public async Task<JsonResult> BulkAddOrUpdate([FromBody] List<UserAccount> users)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return new JsonResult(new
                    {
                        Success = false,
                        Message = "Authorization token is missing or invalid."
                    })
                    { StatusCode = 401 };
                }

                token = token.Substring("Bearer ".Length);

                var response = await securityService.BulkAddOrUpdate(users);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(users);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in BulkAddOrUpdateUsers: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        #endregion userManagement

        #region Role Master

        /// <summary>
        /// Retrieves a list of all roles in the system.
        /// </summary>
        /// <returns>
        /// A <see cref="JsonResult"/> containing a list of roles.
        /// If no roles are found, an empty result is returned.
        /// </returns>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-23</date>
        [HttpGet("role/get")]
        [AllowAnonymous]
        public async Task<JsonResult> GetRoles()
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // ✅ Check if token is missing
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return new JsonResult(new
                    {
                        Success = false,
                        Message = "Authorization token is missing or invalid."
                    })
                    { StatusCode = 401 };
                }

                // ✅ Remove "Bearer " prefix
                token = token.Substring("Bearer ".Length);
                var response = await securityService.GetRolesAsync();

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), "", JsonConvert.SerializeObject(response));

                if (response == null || !response.Any())
                {
                    return await base.FinalizeEmpty();
                }
                else
                {
                    return await base.FinalizeMultiple(response);
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Adds a new role to the system.
        /// </summary>
        /// <param name="roleData">
        /// A <see cref="JObject"/> containing required fields such as RoleName and CreatedById.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating success or failure.
        /// </returns>
        /// <remarks>
        /// Returns HTTP 409 if the RoleName already exists, or HTTP 400 for validation errors.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-23</date>
        [HttpPost("role/add")]
        [AllowAnonymous]
        public async Task<JsonResult> AddRole([FromBody] JObject roleData)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // ✅ Check if token is missing
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return new JsonResult(new
                    {
                        Success = false,
                        Message = "Authorization token is missing or invalid."
                    })
                    { StatusCode = 401 };
                }

                // ✅ Remove "Bearer " prefix
                token = token.Substring("Bearer ".Length);

                var response = await securityService.AddRoleAsync(roleData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(roleData);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                else
                {
                    return await base.FinalizeSingle(response);
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Updates an existing role’s information.
        /// </summary>
        /// <param name="roleData">
        /// A <see cref="JObject"/> containing fields such as Id, RoleName, ModifiedById, and IsActive.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating success or failure.
        /// </returns>
        /// <remarks>
        /// Only provided fields will be updated. Requires a valid Role ID.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-23</date>
        [HttpPut("role/update")]
        [AllowAnonymous]
        public async Task<JsonResult> UpdateRole([FromBody] JObject roleData)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // ✅ Check if token is missing
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return new JsonResult(new
                    {
                        Success = false,
                        Message = "Authorization token is missing or invalid."
                    })
                    { StatusCode = 401 };
                }

                // ✅ Remove "Bearer " prefix
                token = token.Substring("Bearer ".Length);

                var response = await securityService.UpdateRoleAsync(roleData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(roleData);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                else
                {
                    return await base.FinalizeSingle(response);
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Soft deletes (deactivates) a role by its ID.
        /// </summary>
        /// <param name="roleId">
        /// The ID of the role to delete.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating success or failure.
        /// </returns>
        /// <remarks>
        /// Role is not permanently deleted but marked inactive.
        /// Deletion will fail if the role is used by any user.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-23</date>
        [HttpPost("role/delete")]  // Changed from HttpDelete to HttpPost
        [AllowAnonymous]
        public async Task<JsonResult> DeleteRole([FromBody] JObject deleteRequest)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // Check if token is missing
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return new JsonResult(new
                    {
                        Success = false,
                        Message = "Authorization token is missing or invalid."
                    })
                    { StatusCode = 401 };
                }

                // Remove "Bearer " prefix
                token = token.Substring("Bearer ".Length);

                // Validate request
                if (deleteRequest == null ||
                    !deleteRequest.ContainsKey("RoleId") ||
                    !deleteRequest.ContainsKey("ModifiedById"))
                {
                    return new JsonResult(new
                    {
                        Success = false,
                        Message = "RoleId and ModifiedById are required in request body."
                    })
                    { StatusCode = 400 };
                }

                var roleId = deleteRequest["RoleId"].Value<int>();
                var modifiedById = deleteRequest["ModifiedById"].Value<int>();

                var response = await securityService.DeleteRoleAsync(roleId, modifiedById);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(
                    Convert.ToInt32(employeeId),
                    Convert.ToInt32(plantId),
                    deleteRequest.ToString(),
                    response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                else
                {
                    return await base.FinalizeSingle(response);
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        #endregion Role Master

        #region activity logs

        /// <summary>
        /// Get activity logs with filters
        /// </summary>
        /// <param name="requestData">Fetch ActivityLog data</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 28-07-2025
        [HttpPost("ActivityLog/get")]
        [AllowAnonymous]
        public async Task<JsonResult> GetActivityLogs(JObject requestData)
        {
            try
            {
                var result = await securityService.GetActivityLogs(requestData);
                if (result == null || result.Count == 0)
                {
                    return await base.FinalizeEmpty();
                }
                else
                {
                    return await base.FinalizeMultiple(result);
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in GetActivityLogs: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Add a new activity log
        /// </summary>
        /// <param name="requestData">ActivityLog data to add</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 28-07-2025
        [HttpPost("ActivityLog/add")]
        [AllowAnonymous]
        public async Task<JsonResult> AddActivityLog([FromBody] ActivityLog requestData)
        {
            try
            {
                if (requestData == null)
                {
                    return await base.FinalizeEmpty();
                }

                var result = await securityService.AddActivityLog(requestData);
                return await base.FinalizeSingle(result);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        #endregion activity logs
    }
}