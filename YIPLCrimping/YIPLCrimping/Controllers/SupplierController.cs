using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using YIPLCrimping.BAL.Service;
using YIPLCrimping.DAL.Models;
using YIPLCrimping.Helper;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimpingAPI.Controllers
{
    [Route("Supplier")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly SupplierService supplierService;
        private readonly AppDbContext appDbContext;
        private readonly YIPLCrimping.Helper.YIPLCrimping.Helper.Logger logger = YIPLCrimping.Helper.YIPLCrimping.Helper.Logger.Instance;
        [NonAction]
        public override void InitializeController()
        {
        }
        public SupplierController(AppDbContext DBContext, SupplierService supplierService)
        {
            this.appDbContext = DBContext;
            this.supplierService = supplierService;
        }
        /// <summary>
        /// Get list of supplier, here optional filters also added like id(PK), suppliername, suppliercode etc
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns>list of supplier in json</returns>
        /// Developer - Shivaji Mali
        /// Date - 22-07-2025
        [HttpPost("get")]
        [AllowAnonymous]
        public async Task<JsonResult> Get(JObject requestData)
        {
            try
            {
                var result = await supplierService.Get(requestData);
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
                logger.WriteError($"Exception in GetSuppliers: {ex.InnerException?.Message ?? ex.Message}");
                var errorWrapper = new ApiResponseWrapper<string>(null);
                return null;
            }
        }

        /// <summary>
        /// Add single entity of supplier in jobject request format
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns>success / fail message</returns>
        /// Developer - Shivaji Mali
        /// Date - 23-07-2025
        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<JsonResult> Add([FromBody] MSupplier requestData)
        {
            try
            {
                if (requestData == null)
                {
                    return await base.FinalizeEmpty();
                }
                var result = await supplierService.Add(requestData);

                return await base.FinalizeSingle(result);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }
        /// <summary>
        /// Update a single supplier entity
        /// </summary>
        /// <param name="requestData">Updated supplier data</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shivaji Mali
        /// Date - 23-07-2025
        [HttpPost("update")]
        [AllowAnonymous]
        public async Task<JsonResult> Update([FromBody] MSupplier requestData)
        {
            try
            {
                if (requestData == null || requestData.Id <= 0)
                    return await base.FinalizeEmpty();

                var result = await supplierService.Update(requestData);
                return await base.FinalizeJObject(result);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in Update Supplier API: {ex}");
                return await base.FinalizeException(ex);
            }
        }
        /// <summary>
        /// Delete a supplier (soft delete) based on Id
        /// </summary>
        /// <param name="requestData">Must include Id and UpdatedById</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shivaji Mali
        /// Date - 23-07-2025
        [HttpPost("delete")]
        [AllowAnonymous]
        public async Task<JsonResult> Delete([FromBody] MSupplier requestData)
        {
            try
            {
                if (requestData == null || requestData.Id <= 0 || requestData.ModifiedById == null || requestData.ModifiedById <= 0)
                    return await base.FinalizeEmpty();

                var result = await supplierService.Delete(requestData.Id, requestData.ModifiedById.Value);

                return await base.FinalizeJObject(result);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in Delete Supplier API: {ex}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Add or update multiple suppliers in bulk.
        /// </summary>
        /// <param name="requestData">if include Id and UpdatedById else Adds</param>
        /// <returns>Success or failure response</returns>
        /// Developer - Shubham Sadalage
        /// Date - 24-07-2025
        [HttpPost("bulkAddOrUpdate")]
        [AllowAnonymous]
        public async Task<JsonResult> BulkAddOrUpdateSupplier([FromBody] List<MSupplier> suppliers)
        {
            try
            {
                var response = await supplierService.BulkAddOrUpdate(suppliers);

                if (response == null)
                    return await base.FinalizeEmpty();

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in SupplierController.BulkAddOrUpdateSupplier: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }

        /// <summary>
        /// Uploads suppliers in bulk via Excel file.
        /// </summary>
        /// <param name="file">Excel file</param>
        /// <param name="userId">User performing the upload</param>
        /// <returns>Status of the upload</returns>
        /// <author>Shubham</author>
        /// <date>2025-07-24</date>
        [HttpPost("bulkUpload")]
        [DisableRequestSizeLimit]
        [AllowAnonymous]
        public async Task<JsonResult> SupplierBulkUpload(IFormFile file, int userId)
        {
            try
            {
                var response = await supplierService.SupplierBulkUpload(file, userId);

                if (response == null)
                {
                    return await base.FinalizeEmpty();
                }

                return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Exception in SupplierController.SupplierBulkUpload: {ex.InnerException?.Message ?? ex.Message}");
                return await base.FinalizeException(ex);
            }
        }
    }
}