using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using YIPLCrimping.BAL;
using YIPLCrimping.BAL.Service;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimpingAPI.Controllers
{
    [Route("department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentService departmentService;
        private readonly AppDbContext appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ActivityLoggerHelper _loggerHelper;
        private readonly YIPLCrimping.Helper.YIPLCrimping.Helper.Logger logger = YIPLCrimping.Helper.YIPLCrimping.Helper.Logger.Instance;

        [NonAction]
        public override void InitializeController()
        {
        }

        public DepartmentController(AppDbContext DBContext, DepartmentService departmentService, IHttpContextAccessor httpContextAccessor, ActivityLoggerHelper loggerHelper)
        {
            this.appDbContext = DBContext;
            this.departmentService = departmentService;
            this._httpContextAccessor = httpContextAccessor;
            this._loggerHelper = loggerHelper;
        }

        /// <summary>
        /// Adds a new department to the system based on the provided department data.
        /// </summary>
        /// <param name="department">
        /// A <see cref="JObject"/> containing department information including required fields such as
        /// DeptName and CreatedById.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing a success message if the department is added successfully,
        /// or an appropriate error response if validation fails or an exception occurs.
        /// </returns>
        /// <remarks>
        /// Validates the input payload for required fields and checks for duplicate department names.
        /// Returns HTTP 409 if the department name already exists, or HTTP 400 for validation errors.
        /// </remarks>
        /// <author>Developer: Aditi Patil</author>
        /// <date>2025-07-22</date>
        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<JsonResult> Add([FromBody] MDepartment department)
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

            var response = await departmentService.Add(department);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
            var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
            var requestJson = JsonConvert.SerializeObject(department);
            await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

            if (response == null)
                return await base.FinalizeEmpty();
            return await base.FinalizeSingle(response);
        }

        /// <summary>
        /// Updates an existing department's information based on the provided data.
        /// </summary>
        /// <param name="department">
        /// A <see cref="JObject"/> containing department data such as Id, DeptName, IsActive, and ModifiedById.
        /// Only the fields provided will be updated.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> that returns a success message if the update is successful,
        /// a not found message if the department does not exist, or a validation error if the input is invalid.
        /// </returns>
        /// <remarks>
        /// The update requires a valid department Id.
        /// Only provided fields are updated; all others remain unchanged.
        /// </remarks>
        /// <author>Developer: Aditi Patil</author>
        /// <date>2025-07-22</date>
        [HttpPut("update")]
        [AllowAnonymous]
        public async Task<JsonResult> Update([FromBody] MDepartment department)
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

            var response = await departmentService.Update(department);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
            var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
            var requestJson = JsonConvert.SerializeObject(department);
            await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

            if (response == null)
                return await base.FinalizeEmpty();
            return await base.FinalizeSingle(response);
        }

        /// <summary>
        /// Deactivates (soft-deletes) a department based on the provided departmentId.
        /// </summary>
        /// <param name="department">
        /// A <see cref="JObject"/> containing the departmentId of the department to delete and the modifiedById of the user performing the action.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating whether the department was successfully deactivated,
        /// or an error message if the department was not found or validation failed.
        /// </returns>
        /// <remarks>
        /// This operation does not permanently delete the department but sets its IsActive flag to false (soft delete).
        /// </remarks>
        /// <author>Developer: Aditi Patil</author>
        /// <date>2025-07-22</date>
        [HttpPut("delete")]
        [AllowAnonymous]
        public async Task<JsonResult> Delete([FromBody] JObject department)
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

            var response = await departmentService.Delete(department);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
            var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
            var requestJson = JsonConvert.SerializeObject(department);
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

        /// <summary>
        /// Retrieves a list of departments based on the provided request data.
        /// </summary>
        /// <param name="requestData">
        /// A <see cref="JObject"/> containing the request parameters used to filter or query departments.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing the list of departments that match the query criteria.
        /// </returns>
        /// <author>Developer: Aditi Patil</author>
        /// <date>2025-07-22</date>
        [HttpPost("get")]
        [AllowAnonymous]
        public async Task<JsonResult> Get(JObject requestData)
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
            var result = await departmentService.Get(requestData);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
            var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

            await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestData.ToString(), JsonConvert.SerializeObject(result));

            if (result.Count == 0)
            {
                return await base.FinalizeEmpty();
            }
            else
            {
                return await base.FinalizeMultiple(result);
            }
        }

        /// <summary>
        /// Adds or updates multiple department records in bulk.
        /// </summary>
        /// <param name="departments">
        /// A <see cref="List{MDepartment}"/> containing department records to be added or updated.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating the number of departments processed successfully,
        /// or an error message if validation fails or the operation fails.
        /// </returns>
        /// <remarks>
        /// This operation performs a bulk upsert (add or update) on department data.
        /// Records with a valid ID will be updated, and others will be inserted as new.
        /// </remarks>
        /// <author>Developer: Aditi Patil</author>
        /// <date>2025-07-23</date>
        [HttpPost("bulkaddorupdate")]
        [AllowAnonymous]
        public async Task<JsonResult> BulkAddOrUpdate([FromBody] List<MDepartment> departments)
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

            var response = await departmentService.BulkAddOrUpdate(departments);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
            var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
            var requestJson = JsonConvert.SerializeObject(departments);
            await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

            if (response == null)
                return await base.FinalizeEmpty();
            else
                return await base.FinalizeSingle(response);
        }

        /// <summary>
        /// Uploads and imports department data from an Excel file.
        /// </summary>
        /// <param name="file">Excel file containing department data.</param>
        /// <param name="userId">User ID performing the upload.</param>
        /// <returns>Success or failure response.</returns>
        /// <author>Developer: Aditi Patil</author>
        /// <date>2025-07-23</date>
        [HttpPost("BulkUpload")]
        [DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<JsonResult> BulkUpload(IFormFile file, int userId)
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

            var response = await departmentService.BulkUpload(file, userId);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
            var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

            var requestJson = JsonConvert.SerializeObject(new
            {
                FileName = file?.FileName,
                UserId = userId
            });

            await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

            if (response == null)
                return await base.FinalizeEmpty();

            return await base.FinalizeSingle(response);
        }
    }
}