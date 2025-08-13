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
    /// <summary>
    /// API Controller for managing customer-related operations such as fetching,
    /// adding, updating, and soft-deleting customers.
    /// </summary>
    [Route("customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService customerService;
        private readonly AppDbContext appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ActivityLoggerHelper _loggerHelper;
        private readonly YIPLCrimping.Helper.YIPLCrimping.Helper.Logger logger = YIPLCrimping.Helper.YIPLCrimping.Helper.Logger.Instance;

        [NonAction]
        public override void InitializeController()
        {
        }

        public CustomerController(AppDbContext DBContext, CustomerService customerService, IHttpContextAccessor httpContextAccessor, ActivityLoggerHelper loggerHelper)
        {
            this.appDbContext = DBContext;
            this.customerService = customerService;
            this._httpContextAccessor = httpContextAccessor;
            this._loggerHelper = loggerHelper;
        }

        /// <summary>
        /// Retrieves a list of customers based on the provided filter criteria.
        /// </summary>
        /// <param name="requestData">JSON object containing filter parameters</param>
        /// <returns>List of matching customers</returns>
        /// <author>Tabusum Attar</author>
        /// <date>2025-07-22</date>
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

                var result = await customerService.Get(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestData.ToString(), JsonConvert.SerializeObject(result));

                if (result.Count < 0)
                {
                    return await base.FinalizeEmpty();
                }

                return await base.FinalizeMultiple(result);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in CustomerController.Get: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Adds a new customer.
        /// </summary>
        /// <param name="customer">Customer details</param>
        /// <returns>Added customer or error message</returns>
        /// <author>Tabusum Attar</author>
        /// <date>2025-07-22</date>
        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<JsonResult> Add([FromBody] MCustomer customer)
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

                var response = await customerService.Add(customer);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(customer);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in CustomerController.Add: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Updates an existing customer's details.
        /// </summary>
        /// <param name="customer">Updated customer data</param>
        /// <returns>Updated customer or error</returns>
        /// <author>Tabusum Attar</author>
        /// <date>2025-07-22</date>
        [HttpPut("update")]
        [AllowAnonymous]
        public async Task<JsonResult> Update([FromBody] MCustomer customer)
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

                var response = await customerService.Update(customer);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(customer);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in CustomerController.Update: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Performs soft delete on a customer.
        /// </summary>
        /// <param name="customerData">JSON with CustomerId</param>
        /// <returns>Status or deleted customer info</returns>
        /// <author>Tabusum Attar</author>
        /// <date>2025-07-22</date>
        [HttpPut("delete")]
        [AllowAnonymous]
        public async Task<JsonResult> Delete([FromBody] JObject customerData)
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

                var response = await customerService.Delete(customerData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), customerData.ToString(), response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in CustomerController.Delete: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Adds or updates multiple customers in bulk.
        /// </summary>
        /// <param name="customers">List of customer records</param>
        /// <returns>Result of the operation</returns>
        ///  <author>Tabusum Attar</author>
        /// <date>2025-07-23</date>
        [HttpPost("bulkaddorupdate")]
        [AllowAnonymous]
        public async Task<JsonResult> BulkAddOrUpdate([FromBody] List<MCustomer> customers)
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

                var response = await customerService.BulkAddOrUpdate(customers);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(customers);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, response?.ToString());

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in CustomerController.BulkAddOrUpdateCustomer: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Uploads customers in bulk via Excel file.
        /// </summary>
        /// <param name="file">Excel file</param>
        /// <param name="userId">User performing the upload</param>
        /// <returns>Status of the upload</returns>
        /// <author>Tabusum Attar</author>
        /// <date>2025-07-23</date>
        [HttpPost("bulkUpload")]
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

                var response = await customerService.BulkUpload(file, userId);

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
                logger.WriteError($"Exception in CustomerController.CustomerBulkUpload: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }
    }
}