using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using YIPLCrimping.BAL;
using YIPLCrimping.BAL.Service;
using YIPLCrimping.DAL.Models;
using YIPLCrimping.Helper;


namespace YIPLCrimpingAPI.Controllers
{
    [Route("CrimpingStandard")]
    [ApiController]
    public class CrimpingStandardController : ControllerBase
    {
        private readonly CrimpingStandardService crimpingStandardService;
        private readonly AppDbContext appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ActivityLoggerHelper _loggerHelper;
        private readonly YIPLCrimping.Helper.YIPLCrimping.Helper.Logger logger = YIPLCrimping.Helper.YIPLCrimping.Helper.Logger.Instance;
        [NonAction]
        public override void InitializeController()
        {
        }
        public CrimpingStandardController(AppDbContext DBContext, CrimpingStandardService crimpingStandardService, IHttpContextAccessor httpContextAccessor, ActivityLoggerHelper loggerHelper)
        {
            this.appDbContext = DBContext;
            this.crimpingStandardService = crimpingStandardService;
            this._httpContextAccessor = httpContextAccessor;
            this._loggerHelper = loggerHelper;
        }

        [HttpPost("Add")]
        public async Task<JsonResult> Add([FromBody] CrimpingStandardRequestVM importData)
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

                var response = await crimpingStandardService.ImportTerminalData(importData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(importData);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }



        [HttpGet("Get")]
        public async Task<JsonResult> GetTerminals([FromBody]JObject filters)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // Authorization check
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

                var result = await crimpingStandardService.GetAllTerminalsWithDetails(filters);
               

                //Logging activity
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(
                    Convert.ToInt32(employeeId),
                    Convert.ToInt32(plantId),
                    "GetAllTerminals",
                    JsonConvert.SerializeObject(result));

                return new JsonResult(new
                {
                    Success = true,
                    Data = result,
                    Count = result.Count
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message
                })
                { StatusCode = 500 };
            }
        }

        [HttpGet("GetAll")]
        public async Task<JsonResult> GetTerminals_Sp([FromBody] JObject filters)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // Authorization check
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

                var result = await crimpingStandardService.GetAllTerminalsWithDetails_SP(filters);


                //Logging activity
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(
                    Convert.ToInt32(employeeId),
                    Convert.ToInt32(plantId),
                    "GetAllTerminals",
                    JsonConvert.SerializeObject(result));

                return new JsonResult(new
                {
                    Success = true,
                    Data = result,
                    Count = result.Count
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Success = false,
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message
                })
                { StatusCode = 500 };
            }
        }


        [HttpPost("Update")]
        public async Task<JsonResult> Update([FromBody] CrimpingStandardRequestVM updateData)
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

                var response = await crimpingStandardService.UpdateTerminal(updateData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(updateData);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        [HttpPost("Delete")]
        public async Task<JsonResult> Delete([FromBody] JObject payload)
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

                int terminalId = payload["id"]?.ToObject<int>() ?? 0;
                int modifiedById = payload["modifiedById"]?.ToObject<int>() ?? 0;

                var response = await crimpingStandardService.DeleteTerminal(terminalId, modifiedById);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), $"Delete Id={terminalId}", response?.ToString());

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        [HttpPost("BulkAddUpdate")]
        public async Task<JsonResult> BulkAddUpdate([FromBody] List<CrimpingStandardRequestVM> bulkUpdateData)
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

                var response = await crimpingStandardService.BulkAddUpdateTerminals(bulkUpdateData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(bulkUpdateData);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        //[HttpGet("Get")]
        //public async Task<JsonResult> GetTerminal(string terminalNo)
        //{
        //    try
        //    {
        //        // Authentication
        //        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        //        if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
        //        {
        //            return new JsonResult(new
        //            {
        //                Success = false,
        //                Message = "Authorization token is missing or invalid."
        //            })
        //            { StatusCode = 401 };
        //        }

        //        token = token.Substring("Bearer ".Length);

        //        // Business Logic
        //        var response = await crimpingStandardService.GetTerminalByNo(terminalNo);

        //        // Activity Logging
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var jwtToken = tokenHandler.ReadJwtToken(token);
        //        var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
        //        var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

        //        await _loggerHelper.LogActivity(
        //            Convert.ToInt32(employeeId),
        //            Convert.ToInt32(plantId),
        //            $"Retrieve terminal: {terminalNo}",
        //            response?.ToString()
        //        );

        //        return await base.FinalizeSingle(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return await base.FinalizeException(ex);
        //    }
        //}

        [HttpPost("BulkUpload/get")]
        public async Task<JsonResult> BulkUpload([FromForm] IFormFile file)
        {
            try
            {
                //var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                //if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                //{
                //    return new JsonResult(new
                //    {
                //        Success = false,
                //        Message = "Authorization token is missing or invalid."
                //    })
                //    { StatusCode = 401 };
                //}

                //token = token.Substring("Bearer ".Length);

                var response = await crimpingStandardService.BulkUpload(file);

                //var tokenHandler = new JwtSecurityTokenHandler();
                //var jwtToken = tokenHandler.ReadJwtToken(token);
                //var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                //var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                //var requestJson = JsonConvert.SerializeObject(importData);
                //await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());
                if(response.HasValues)
                    return await base.FinalizeSingle(response);
                else
                    return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }
    }
}
