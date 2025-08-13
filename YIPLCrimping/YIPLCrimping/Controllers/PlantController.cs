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

namespace YIPLCrimpingAPI.Controllers
{
    [Route("plant")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly PlantService plantService;
        private readonly AppDbContext appDbContext;
        private readonly ActivityLoggerHelper _loggerHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly YIPLCrimping.Helper.YIPLCrimping.Helper.Logger logger = YIPLCrimping.Helper.YIPLCrimping.Helper.Logger.Instance;

        [NonAction]
        public override void InitializeController()
        {
        }

        public PlantController(AppDbContext DBContext, PlantService plantService, IHttpContextAccessor httpContextAccessor, ActivityLoggerHelper loggerHelper)
        {
            this.appDbContext = DBContext;
            this.plantService = plantService;
            this._loggerHelper = loggerHelper;
            this._httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieves a list of plants based on the provided request data.
        /// </summary>
        /// <param name="requestData">
        /// A <see cref="JObject"/> containing the request parameters used to filter or query plants.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing the list of plants that match the query criteria.
        /// If no plants are found, an empty result is returned.
        /// </returns>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-23</date>
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

                var result = await plantService.Get(requestData);

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
                logger.WriteError($"Exception in GetPlants: {ex.InnerException?.Message ?? ex.Message}");
                var errorWrapper = new ApiResponseWrapper<string>(null);
                return null;
            }
        }

        /// <summary>
        /// Adds a new plant to the system.
        /// </summary>
        /// <param name="plantData">A <see cref="JObject"/> containing PlantCode, PlantName, and CreatedById.</param>
        /// <returns>A <see cref="JsonResult"/> with success or error details.</returns>
        /// <remarks>
        /// Returns HTTP 409 if the PlantCode already exists, or HTTP 400 for validation errors.
        /// </remarks>
        /// <author>Developer: John Doe</author>
        /// <date>2025-07-22</date>
        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<JsonResult> Add([FromBody] MPlant plantData)
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

                var response = await plantService.Add(plantData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(plantData);
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
        /// Updates an existing plant’s information based on the provided data.
        /// </summary>
        /// <param name="plantData">
        /// A <see cref="JObject"/> containing PlantCode (required), PlantName, IsActive, and ModifiedById.
        /// Only the fields provided will be updated.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating success, validation failure, or not found.
        /// </returns>
        /// <remarks>
        /// The update requires a valid PlantCode to identify the plant.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-22</date>
        [HttpPut("update")]
        [AllowAnonymous]
        public async Task<JsonResult> Update([FromBody] MPlant plantData)
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

                var response = await plantService.Update(plantData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(plantData);
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
        /// Deactivates (soft-deletes) a plant based on the provided PlantCode.
        /// </summary>
        /// <param name="plantData">
        /// A <see cref="JObject"/> containing the PlantCode of the plant to delete and the ModifiedById of the user performing the action.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating whether the plant was successfully deactivated,
        /// or an error message if the plant was not found or validation failed.
        /// </returns>
        /// <remarks>
        /// This operation does not permanently delete the plant but sets its IsActive flag to false (soft delete).
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-22</date>
        [HttpPut("delete")]
        [AllowAnonymous]
        public async Task<JsonResult> Delete([FromBody] JObject plantData)
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

                var response = await plantService.Delete(plantData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(plantData);
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
        /// Adds or updates multiple plant records in bulk.
        /// </summary>
        /// <param name="plants">
        /// A <see cref="List{MPlant}"/> containing plant records to be added or updated.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating the number of plants processed successfully,
        /// or an error message if validation fails or the operation fails.
        /// </returns>
        /// <remarks>
        /// This operation performs a bulk upsert (add or update) on plant data.
        /// Records with a valid ID will be updated, and others will be inserted as new.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-23</date>
        [HttpPost("bulkaddorupdate")]
        [AllowAnonymous]
        public async Task<JsonResult> BulkAddOrUpdate([FromBody] List<MPlant> plants)
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

                var response = await plantService.BulkAddOrUpdate(plants);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(plants);
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
                logger.WriteError($"Exception in BulkAddOrUpdatePlants: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Uploads and imports plant data from an Excel file.
        /// </summary>
        /// <param name="file">
        /// An <see cref="IFormFile"/> representing the Excel file containing plant data.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the upload.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating success or failure of the bulk upload operation.
        /// </returns>
        /// <remarks>
        /// Reads plant records from an uploaded Excel file, validates them, and saves new records.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-23</date>
        [HttpPost("bulkupload")]
        [DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<JsonResult> PlantBulkUpload([FromForm] IFormFile file, [FromForm] int userId)
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

                var response = await plantService.PlantBulkUpload(file, userId);

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
                {
                    return await base.FinalizeEmpty();
                }

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in PlantBulkUpload: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }
    }
}