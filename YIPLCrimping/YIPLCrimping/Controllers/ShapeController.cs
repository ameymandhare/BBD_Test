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
    [Route("Shape")]
    [ApiController]
    public class ShapeController : ControllerBase
    {
        private readonly ShapeService shapeService;
        private readonly AppDbContext appDbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ActivityLoggerHelper _loggerHelper;
        private readonly YIPLCrimping.Helper.YIPLCrimping.Helper.Logger logger = YIPLCrimping.Helper.YIPLCrimping.Helper.Logger.Instance;

        [NonAction]
        public override void InitializeController()
        {
        }

        public ShapeController(AppDbContext DBContext, ShapeService shapeService, IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor, ActivityLoggerHelper loggerHelper)
        {
            this.appDbContext = DBContext;
            this.shapeService = shapeService;
            this._hostingEnvironment = hostEnvironment;
            this._httpContextAccessor = httpContextAccessor;
            this._loggerHelper = loggerHelper;
        }

        /// <summary>
        /// Adds a new crimping shape to the system.
        /// </summary>
        /// <param name="shapeData">The shape data to be added.</param>
        /// <param name="imageFile">An optional image file representing the shape.</param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing the result of the add operation. Includes success or failure status.
        /// </returns>
        /// <author>Developer: Mrunal Khanvilkar</author>
        /// <date>2025-07-22</date>
        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<JsonResult> Add([FromForm] MCrimpingShape shapeData, IFormFile? imageFile)
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

                var taskResult = await shapeService.Add(shapeData, imageFile);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(new
                {
                    ShapeData = shapeData,
                    FileName = imageFile?.FileName,
                });
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, taskResult?.ToString());

                if (taskResult != null)
                {
                    return await base.FinalizeSingle(taskResult);
                }
                else
                {
                    return await base.FinalizeEmpty();
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Updates an existing shape and optionally updates its associated image.
        /// </summary>
        /// <param name="shapeData">The shape data to be updated.</param>
        /// <param name="imageFile">Optional image file to replace the existing one.</param>
        /// <returns>
        /// A <see cref="JsonResult"/> representing the result of the update operation.
        /// </returns>
        /// <author>Developer: Mrunal Khanvilkar</author>
        /// <date>2025-07-22</date>
        [HttpPost("update")]
        [AllowAnonymous]
        public async Task<JsonResult> Update([FromForm] MCrimpingShape shapeData, IFormFile? imageFile)
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

                var taskResult = await shapeService.Update(shapeData, imageFile);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(new
                {
                    ShapeData = shapeData,
                    FileName = imageFile?.FileName,
                });

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, taskResult?.ToString());

                if (taskResult != null)
                {
                    return await base.FinalizeSingle(taskResult);
                }
                else
                {
                    return await base.FinalizeEmpty();
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Retrieves a list of shapes based on the provided request parameters.
        /// </summary>
        /// <param name="requestData">A <see cref="JObject"/> containing filtering or search criteria such as name, ID, or search text.</param>
        /// <returns>A <see cref="JsonResult"/> containing a list of shapes that match the specified criteria.</returns>
        /// <author>Developer: Mrunal Khanvilkar</author>
        /// <date>2025-07-22</date>
        [HttpPost("get")]
        [AllowAnonymous]
        public async Task<JsonResult> Get([FromBody] JObject requestData)
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
                var taskResult = await shapeService.Get(requestData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var logResult = taskResult.Select(x => new { x.Id, x.Name, x.ImageUrl, x.Base64FileName, x.IsActive, x.CreatedDate, x.ModifiedDate, x.ModifiedById }).ToList();

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestData.ToString(), JsonConvert.SerializeObject(logResult));

                if (taskResult != null && taskResult.Any())
                {
                    return await base.FinalizeMultiple(taskResult);
                }
                else
                {
                    return await base.FinalizeEmpty();
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in GetShapes: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Deactivates (soft deletes) a crimping shape based on the provided ID and modifiedById.
        /// </summary>
        /// <param name="shapeData">
        /// A <see cref="JObject"/> containing:
        /// - <c>id</c>: The unique identifier of the shape to deactivate.
        /// - <c>modifiedById</c>: The ID of the user performing the delete action.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating the result of the operation:
        /// success message if deleted, or appropriate error if not found or failed.
        /// </returns>
        /// <author>Developer: Mrunal Khanvilkar</author>
        /// <date>2025-07-22</date>

        [HttpPut("delete")]
        [AllowAnonymous]
        public async Task<JsonResult> Delete([FromBody] JObject shapeData)
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

                var taskResult = await shapeService.Delete(shapeData);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(shapeData);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, taskResult?.ToString());

                if (taskResult != null)
                {
                    return await base.FinalizeSingle(taskResult);
                }
                else
                {
                    return await base.FinalizeEmpty();
                }
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Adds or updates multiple shape records in bulk.
        /// </summary>
        /// <param name="shapes">
        /// A <see cref="List{MShape}"/> containing shape records to be added or updated.
        /// </param>
        /// <returns>
        /// A <see cref="JsonResult"/> indicating the number of shapes processed successfully,
        /// or an error message if validation fails or the operation fails.
        /// </returns>
        /// <remarks>
        /// This operation performs a bulk upsert (add or update) on shape data.
        /// Records with a valid ID will be updated, and others will be inserted as new.
        /// </remarks>
        /// <author>Developer: Suyash Bongale</author>
        /// <date>2025-07-23</date>
        [HttpPost("bulkaddorupdate")]
        [AllowAnonymous]
        public async Task<JsonResult> BulkAddOrUpdate([FromBody] List<MCrimpingShape> shapes)
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

                var taskResult = await shapeService.BulkAddOrUpdate(shapes);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;
                var requestJson = JsonConvert.SerializeObject(shapes);
                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, taskResult?.ToString());

                if (taskResult != null)
                {
                    return await base.FinalizeSingle(taskResult);
                }
                else
                {
                    return await base.FinalizeEmpty();
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in BulkAddOrUpdateShapes: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Bulk uploads crimping shapes from an uploaded file.
        /// </summary>
        /// <param name="file">The Excel or CSV file containing shape records.</param>
        /// <param name="userId">The ID of the user performing the upload.</param>
        /// <returns>A <see cref="JsonResult"/> indicating the result of the bulk upload.</returns>
        /// <author>Developer: Mrunal Khanvilkar</author>
        /// <date>2025-07-24</date>
        [HttpPost("bulkupload")]
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

                var taskResult = await shapeService.BulkUpload(file, userId);

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token); // Not validated
                var employeeId = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
                var plantId = jwtToken.Claims.FirstOrDefault(c => c.Type == "PlantId")?.Value;

                var requestJson = JsonConvert.SerializeObject(new
                {
                    FileName = file?.FileName,
                    UserId = userId
                });

                await _loggerHelper.LogActivity(Convert.ToInt32(employeeId), Convert.ToInt32(plantId), requestJson, taskResult?.ToString());

                if (taskResult != null)
                {
                    return await base.FinalizeSingle(taskResult);
                }
                else
                {
                    return await base.FinalizeEmpty();
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in ShapeBulkUpload: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }
    }
}