using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using YIPLCrimping.DAL;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.BAL.Service
{
    public class SupplierService
    {
        private readonly SupplierRepository supplierRepository;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public SupplierService(SupplierRepository supplierRepository)
        {
            this.supplierRepository = supplierRepository;
        }

        public async Task<List<MSupplier>> Get(JObject requestData)
        {
            string? supplierCode = requestData["supplierCode"]?.ToString();
            string? searchText = requestData["searchText"]?.ToString();
            int? id = requestData["id"]?.ToObject<int>();

            var suppliers = await supplierRepository.Get(supplierCode, searchText, id);
            logger.WriteInfo($"Retrieved {suppliers.Count} suppliers from repository.");
            return suppliers;
        }

        public async Task<JObject> Add(MSupplier supplierData)
        {
            var result = new JObject();
            if (string.IsNullOrWhiteSpace(supplierData.SupplierCode))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierCodeRequired_1252, 400, ErrorNumber.SupplierCodeRequired);
            }

            if (string.IsNullOrWhiteSpace(supplierData.SupplierName))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierNameRequired_1253, 400, ErrorNumber.SupplierNameRequired);
            }
            if (supplierData.CreatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);
            }
            supplierData.CreatedDate = DateTime.Now;
            supplierData.IsActive = true;

            var addSupplier = await supplierRepository.Add(supplierData);
            if (addSupplier > 0)
            {
                logger.WriteInfo($"Supplier {supplierData.Id}, {supplierData.SupplierName} is added successfully.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.SupplierAddedSuccessfully);
                result.Add("Message", SuccessDescription.SupplierAddedSuccessfully_2001);
            }
            else
            {
                logger.WriteInfo($"Supplier {addSupplier} could not be added.");
                result.Add("Success", true);
                result.Add("Message", ErrorDescription.SupplierFailedToAdd_1255);
            }
            return result;
        }

        public async Task<JObject> Update(MSupplier supplierData)
        {
            var result = new JObject();

            // === Validation ===
            if (string.IsNullOrWhiteSpace(supplierData.SupplierCode))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierCodeRequired_1252, 400, ErrorNumber.SupplierCodeRequired);
            }

            if (string.IsNullOrWhiteSpace(supplierData.SupplierName))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierNameRequired_1253, 400, ErrorNumber.SupplierNameRequired);
            }

            if (supplierData.ModifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierModifiedByIdRequired_1258, 400, ErrorNumber.SupplierModifiedByID);
            }

            // === Get existing ===
            var existing = await supplierRepository.Get("", "", supplierData.Id);
            if (existing == null || existing.Count <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierNotFound_1254, 404, ErrorNumber.SupplierNotFound);
            }

            // === Update fields ===
            existing.FirstOrDefault().SupplierName = supplierData.SupplierName;
            existing.FirstOrDefault().SupplierCode = supplierData.SupplierCode;
            existing.FirstOrDefault().SupplierCode = supplierData.SupplierCode;

            existing.FirstOrDefault().ModifiedById = supplierData.ModifiedById;
            existing.FirstOrDefault().ModifiedDate = DateTime.Now;

            // === Save ===
            int rowsAffected = await supplierRepository.Update(supplier: existing.FirstOrDefault());

            if (rowsAffected > 0)
            {
                logger.WriteInfo($"Supplier updated successfully: Id={existing.FirstOrDefault().Id}, Name={existing.FirstOrDefault().SupplierName}");
                result.Add("Success", true);
                result.Add("SuccessNumber", 2002);
                result.Add("Message", "Supplier updated successfully.");
            }
            else
            {
                logger.WriteError($"Failed to update supplier: Id={existing.FirstOrDefault().Id}");
                result.Add("Success", false);
                result.Add("Message", "Failed to update supplier.");
            }

            return result;
        }

        public async Task<JObject> Delete(int supplierId, int updatedById)
        {
            var result = new JObject();
            if (supplierId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierCodeRequired_1252, 400, ErrorNumber.SupplierCodeRequired);
            }

            if (updatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.UpdatedByIdRequired_1259, 400, ErrorNumber.SupplierModifiedByID);
            }

            var existing = await supplierRepository.Get("", "", supplierId);
            if (existing == null)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierNotFound_1254, 404, ErrorNumber.SupplierNotFound);
            }

            if (!existing.FirstOrDefault().IsActive)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierAlreadyExists_1260, 409, ErrorNumber.SupplierAlreadyExists);
            }

            existing.FirstOrDefault().IsActive = false;
            existing.FirstOrDefault().ModifiedById = updatedById;
            existing.FirstOrDefault().ModifiedDate = DateTime.Now;

            var rowsAffected = await supplierRepository.Update(existing.FirstOrDefault());
            if (rowsAffected > 0)
            {
                logger.WriteInfo($"Supplier deleted (soft): Id={supplierId}");

                result.Add("Success", true);
                result.Add("SuccessNumber", 2003);
                result.Add("Message", "Supplier deleted successfully.");
            }
            else
            {
                logger.WriteInfo($"Supplier delete failed: Id={supplierId}");

                result.Add("Success", false);
                result.Add("Message", "Failed to delete supplier.");
            }
            return result;
        }

        /// <summary>
        /// Adds or updates a batch of suppliers.
        /// </summary>
        /// <param name="suppliers">List of suppliers to process.</param>
        /// <returns>Result object with success and skipped records.</returns>
        public async Task<JObject> BulkAddOrUpdate(List<MSupplier> suppliers)
        {
            var result = new JObject();
            var skipped = new JArray();
            int processed = 0;

            if (suppliers == null || !suppliers.Any())
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.NoSupplierRecordsProvided_13001, 400, ErrorNumber.NoSupplierRecordsProvided);

            var existingIds = suppliers.Where(s => s.Id > 0).Select(s => s.Id).ToList();
            var existingSuppliers = await supplierRepository.Getlist(existingIds);
            var newSuppliers = new List<MSupplier>();

            foreach (var supplier in suppliers)
            {
                bool isValid = true;

                if (string.IsNullOrWhiteSpace(supplier.SupplierName))
                {
                    skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierNameIsRequired_1302, 400, ErrorNumber.SupplierNameIsRequired));
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(supplier.SupplierCode))
                {
                    skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierCodeIsRequired_1303, 400, ErrorNumber.SupplierCodeIsRequired));
                    isValid = false;
                }

                if (!isValid) continue;

                if (supplier.Id > 0)
                {
                    // Update
                    if (supplier.ModifiedById == null || supplier.ModifiedById <= 0)
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired));
                        continue;
                    }

                    if (existingSuppliers.TryGetValue(supplier.Id, out var existing))
                    {
                        existing.SupplierName = supplier.SupplierName;
                        existing.SupplierCode = supplier.SupplierCode;
                        existing.ModifiedById = supplier.ModifiedById;
                        existing.ModifiedDate = DateTime.UtcNow;

                        await supplierRepository.Update(existing);
                        processed++;
                        logger.WriteInfo($"[BulkUpdate] Updated supplier: Id={existing.Id}");
                    }
                    else
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierNotFound_1254, 400, ErrorNumber.SupplierNotFound));
                    }
                }
                else
                {
                    // Create
                    if (supplier.CreatedById == null || supplier.CreatedById <= 0)
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired));
                        continue;
                    }

                    newSuppliers.Add(new MSupplier
                    {
                        SupplierName = supplier.SupplierName?.Trim(),
                        SupplierCode = supplier.SupplierCode?.Trim(),
                        CreatedById = supplier.CreatedById,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    });

                    processed++;
                }
            }

            if (newSuppliers.Any())
            {
                await supplierRepository.BulkAdd(newSuppliers);
                logger.WriteInfo($"[BulkAdd] Inserted {newSuppliers.Count} new suppliers.");
            }

            result.Add("Success", true);
            result.Add("SuccessNumber", SuccessNumber.SupplierBulkAddOrUpdateSuccessfully);
            result.Add("Message", SuccessDescription.SupplierBulkAddOrUpdateSuccessfully_3011);
            result.Add("TotalProcessed", processed);
            result.Add("TotalSkipped", skipped.Count());
            if (skipped.Count > 0)
            {
                result.Add("SkippedDetails", skipped);
            }

            return result;
        }

        /// <summary>
        /// Uploads suppliers in bulk via Excel file.
        /// </summary>
        /// <param name="file">Excel file</param>
        /// <param name="userId">User performing the upload</param>
        /// <returns>Upload result response</returns>
        public async Task<JObject> SupplierBulkUpload(IFormFile file, int userId)
        {
            var result = new JObject();

            if (file == null || file.Length == 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.NoFileUploaded_1035, 400, ErrorNumber.NoFileUploaded);

            if (userId <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);

            List<MSupplier> suppliers;
            try
            {
                suppliers = ExcelHelper.ReadSupplierFromExcel(file.OpenReadStream());
                logger.WriteInfo($"[BulkUpload] Read {suppliers.Count} suppliers from Excel.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"[BulkUpload] Error reading Excel: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.InvalidOrEmptyExcelData_1036, 400, ErrorNumber.InvalidOrEmptyExcelData);
            }

            if (suppliers == null || !suppliers.Any())
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.InvalidOrEmptyExcelData_1036, 400, ErrorNumber.InvalidOrEmptyExcelData);

            foreach (var supplier in suppliers)
            {
                supplier.SupplierName = supplier.SupplierName?.Trim();
                supplier.SupplierCode = supplier.SupplierCode?.Trim();
                supplier.CreatedById = userId;
                supplier.CreatedDate = DateTime.UtcNow;
                supplier.IsActive = true;
            }

            try
            {
                await supplierRepository.BulkInsertSuppliers(suppliers);

                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.SupplierBulkUploadSuccess);
                result.Add("Message", SuccessDescription.SupplierBulkUploadSuccess_3012);
                result.Add("ImportedCount", suppliers.Count);
                logger.WriteInfo($"[BulkUpload] Successfully uploaded {suppliers.Count} suppliers by user {userId}.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"[BulkUpload] Upload failed: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.SupplierBulkUploadFailed_1304, 500, ErrorNumber.SupplierBulkUploadFailed);
            }

            return result;
        }
    }
}