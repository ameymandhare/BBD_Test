using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using YIPLCrimping.BAL;
using YIPLCrimping.BAL.Service;
using YIPLCrimping.DAL.Models;

namespace YIPLCrimpingAPI.Controllers
{
    [Route("machine")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly MachineService machineService;
        private readonly AppDbContext appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ActivityLoggerHelper _loggerHelper;

        private readonly YIPLCrimping.Helper.YIPLCrimping.Helper.Logger logger = YIPLCrimping.Helper.YIPLCrimping.Helper.Logger.Instance;

        [NonAction]
        public override void InitializeController()
        {
        }

        public MachineController(AppDbContext DBContext, MachineService machineService, IHttpContextAccessor httpContextAccessor, ActivityLoggerHelper loggerHelper)
        {
            this.appDbContext = DBContext;
            this.machineService = machineService;
            this._httpContextAccessor = httpContextAccessor;
            this._loggerHelper = loggerHelper;
        }

        /// <summary>
        /// Retrieves a list of machines based on the provided request data.
        /// </summary>
        /// <param name="requestData">
        /// A <see cref="JObject"/> containing the request parameters used to filter or query machines.
        /// Supported parameters include <c>machineName</c>, <c>id</c>, and <c>plantId</c>.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing the list of machines that match the query criteria.
        /// If no machines are found, an empty result is returned.
        /// </returns>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-24</date>

        [HttpPost("get")]
        [AllowAnonymous]
        public async Task<JsonResult> Get(JObject requestData)
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
                var result = await machineService.Get(requestData);

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
            catch (Exception ex)
            {
                logger.WriteError($"Exception in GetMachines: {ex.InnerException?.Message ?? ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Adds a new machine to the system.
        /// </summary>
        /// <param name="machineData">
        /// A <see cref="MMachine"/> object containing MachineName, MachineCost, PlantId, and CreatedById.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> with success or error details, including HTTP 409 if the MachineName already exists,
        /// or HTTP 400 for validation errors.
        /// </returns>
        /// <remarks>
        /// Ensures that all required fields are present and valid before adding the machine.
        /// </remarks>
        /// <author>Developer: John Doe</author>
        /// <date>2025-07-24</date>

        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<JsonResult> Add([FromBody] MMachine machineData)
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

                var response = await machineService.Add(machineData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(machineData);
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
        /// Updates an existing machine’s information based on the provided data.
        /// </summary>
        /// <param name="machineData">
        /// A <see cref="MMachine"/> object containing MachineName (optional), MachineCost, PlantId, IsActive, and ModifiedById.
        /// MachineName is used to identify the machine to update.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating success, validation failure, or not found.
        /// </returns>
        /// <remarks>
        /// The update requires a valid MachineName to identify the machine.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-24</date>
        [HttpPut("update")]
        [AllowAnonymous]
        public async Task<JsonResult> Update([FromBody] MMachine machineData)
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

                var response = await machineService.Update(machineData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(machineData);
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
        /// Deactivates (soft-deletes) a machine based on the provided MachineName.
        /// </summary>
        /// <param name="machineData">
        /// A <see cref="JObject"/> containing the MachineName of the machine to delete and the ModifiedById of the user performing the action.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating whether the machine was successfully deactivated,
        /// or an error message if the machine was not found or validation failed.
        /// </returns>
        /// <remarks>
        /// This operation does not permanently delete the machine but sets its IsActive flag to false (soft delete).
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-24</date>
        [HttpPut("delete")]
        [AllowAnonymous]
        public async Task<JsonResult> Delete([FromBody] JObject machineData)
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

                var response = await machineService.Delete(machineData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(machineData);
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
        /// Adds or updates multiple machine records in bulk.
        /// </summary>
        /// <param name="machines">
        /// A <see cref="List{MMachine}"/> containing machine records to be added or updated.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating the number of machines processed successfully,
        /// or an error message if validation fails or the operation fails.
        /// </returns>
        /// <remarks>
        /// This operation performs a bulk upsert (add or update) on machine data.
        /// Records with a valid ID will be updated, and others will be inserted as new.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-24</date>
        [HttpPost("bulkaddorupdate")]
        [AllowAnonymous]
        public async Task<JsonResult> BulkAddOrUpdate([FromBody] List<MMachine> machines)
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

                var response = await machineService.BulkAddOrUpdate(machines);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(machines);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                    return await base.FinalizeEmpty();
                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in MachineBulkAddOrUpdate: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Uploads and imports machine data from an Excel file.
        /// </summary>
        /// <param name="file">
        /// An <see cref="IFormFile"/> representing the Excel file containing machine data.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the upload.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating success or failure of the bulk upload operation.
        /// </returns>
        /// <remarks>
        /// Reads machine records from an uploaded Excel file, validates them, and saves new records.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-24</date>
        [HttpPost("bulkupload")]
        [DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<JsonResult> BulkUpload([FromForm] IFormFile file, [FromForm] int userId)
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

                var response = await machineService.BulkUpload(file, userId);

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
            catch (Exception ex)
            {
                logger.WriteError($"Exception in MachineBulkUpload: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }
    }
}