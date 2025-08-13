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
    [Route("Wire")]
    [ApiController]
    public class WireController : ControllerBase
    {
        private readonly WireService _wireService;
        private readonly AppDbContext appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ActivityLoggerHelper _loggerHelper;

        private readonly YIPLCrimping.Helper.YIPLCrimping.Helper.Logger logger = YIPLCrimping.Helper.YIPLCrimping.Helper.Logger.Instance;
        [NonAction]
        public override void InitializeController()
        {
        }
        public WireController(AppDbContext DBContext, WireService service, IHttpContextAccessor httpContextAccessor, ActivityLoggerHelper loggerHelper)
        {
            this.appDbContext = DBContext;
            this._wireService = service;
            this._httpContextAccessor = httpContextAccessor;
            this._loggerHelper = loggerHelper;
        }

        /// <summary>
        /// Get a wiretype list with filters
        /// </summary>
        /// <param name="requestData">Fetch WireType Data</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 24-07-2025
        [HttpPost("WireType/get")]
        [AllowAnonymous]
        public async Task<JsonResult> GetWireType(JObject requestData)
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
                var result = await _wireService.GetWireType(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestData.ToString(), JsonConvert.SerializeObject(result));

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
                logger.WriteError($"Exception in GetWireTypes: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Add a new wiretype
        /// </summary>
        /// <param name="requestData">WireType data to add</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 24-07-2025
        [HttpPost("WireType/add")]
        [AllowAnonymous]
        public async Task<JsonResult> AddWireType([FromBody] MWireType requestData)
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

                var response = await _wireService.AddWireType(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(requestData);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                    return await base.FinalizeEmpty();
                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Update an existing wiretype
        /// </summary>
        /// <param name="requestData">WireType data to update</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 24-07-2025
        [HttpPost("WireType/update")]
        [AllowAnonymous]
        public async Task<JsonResult> UpdateWireType([FromBody] MWireType requestData)
        {
            try
            {
                if (requestData == null || requestData.Id <= 0)
                    return await base.FinalizeEmpty();

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

                var response = await _wireService.UpdateWireType(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(requestData);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());
                if (response == null)
                    return await base.FinalizeEmpty();
                return await base.FinalizeJObject(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in Update WireType API: {ex}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Delete a wiretype
        /// </summary>
        /// <param name="requestData">WireType data to delete (requires Id and ModifiedById)</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 24-07-2025
        [HttpPost("WireType/delete")]
        [AllowAnonymous]
        public async Task<JsonResult> DeleteWireType([FromBody] MWireType requestData)
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

                var response = await _wireService.DeleteWireType(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(requestData);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());
                if (response == null)
                    return await base.FinalizeEmpty();
                return await base.FinalizeJObject(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in Delete WireType API: {ex}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Bulk add or update wiretypes
        /// </summary>
        /// <param name="wireTypes">List of wiretypes to add/update</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 25-07-2025
        [HttpPost("WireType/bulkAddOrUpdate")]
        [AllowAnonymous]
        public async Task<JsonResult> BulkAddOrUpdateWireType([FromBody] List<MWireType> wireTypes)
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

                var response = await _wireService.BulkAddOrUpdate(wireTypes);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(wireTypes);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());
                if (response == null)
                    return await base.FinalizeEmpty();

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in WireTypeController.BulkAddOrUpdateWireType: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Bulk upload wiretypes from file
        /// </summary>
        /// <param name="file">Upload file</param>
        /// <param name="userId">User ID performing the action</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 25-07-2025
        [HttpPost("WireType/bulkUpload")]
        [DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<JsonResult> WireTypeBulkUpload(IFormFile file, [FromForm] int userId)
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

                var response = await _wireService.WireTypeBulkUpload(file, userId);

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
                logger.WriteError($"Exception in WireTypeController.WireTypeBulkUpload: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Get a wiresize list with filters
        /// </summary>
        /// <param name="requestData">Fetch WireSize Data</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 25-07-2025
        [HttpPost("WireSize/get")]
        public async Task<JsonResult> GetWireSize(JObject requestData)
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

                var result = await _wireService.GetWireSize(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestData.ToString(), JsonConvert.SerializeObject(result));

                return result == null || result.Count == 0
                    ? await base.FinalizeEmpty()
                    : await base.FinalizeMultiple(result);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in GetWireSizes: {ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Add a new wiresize
        /// </summary>
        /// <param name="requestData">WireSize data to add</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 25-07-2025
        [HttpPost("WireSize/add")]
        public async Task<JsonResult> AddWireSize([FromBody] MWireSize requestData)
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

                var response = await _wireService.AddWireSize(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), JsonConvert.SerializeObject(requestData), response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Update an existing wiresize
        /// </summary>
        /// <param name="requestData">WireSize data to update</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 25-07-2025
        [HttpPost("WireSize/update")]
        public async Task<JsonResult> UpdateWireSize([FromBody] MWireSize requestData)
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
                var response = await _wireService.UpdateWireSize(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), JsonConvert.SerializeObject(requestData), response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in Update WireSize: {ex}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Delete a wiresize
        /// </summary>
        /// <param name="requestData">WireSize data to delete (requires Id and ModifiedById)</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 25-07-2025
        [HttpPost("WireSize/delete")]
        public async Task<JsonResult> DeleteWireSize([FromBody] MWireSize requestData)
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

                var response = await _wireService.DeleteWireSize(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), JsonConvert.SerializeObject(requestData), response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }
                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in Delete WireSize: {ex}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Bulk add or update wiresizes
        /// </summary>
        /// <param name="wireSizes">List of wiresizes to add/update</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 25-07-2025
        [HttpPost("WireSize/bulkAddOrUpdate")]
        public async Task<JsonResult> BulkAddOrUpdateWireSize([FromBody] List<MWireSize> wireSizes)
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

                var response = await _wireService.BulkAddOrUpdateWireSize(wireSizes);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), JsonConvert.SerializeObject(wireSizes), response?.ToString());
                return response == null
                    ? await base.FinalizeEmpty()
                    : await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in BulkAddOrUpdate WireSize: {ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Bulk upload wiresizes from file
        /// </summary>
        /// <param name="file">Upload file</param>
        /// <param name="userId">User ID performing the action</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 25-07-2025
        [HttpPost("WireSize/bulkUpload")]
        public async Task<JsonResult> WireSizeBulkUpload(IFormFile file, [FromForm] int userId)
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

                var response = await _wireService.WireSizeBulkUpload(file, userId);

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
                return response == null
                    ? await base.FinalizeEmpty()
                    : await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in WireSize BulkUpload: {ex.Message}");
                return await base.FinalizeException(ex);
            }
        }
    }
}