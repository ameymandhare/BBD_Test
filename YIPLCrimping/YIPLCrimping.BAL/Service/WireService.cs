using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using YIPLCrimping.DAL;
using YIPLCrimping.DAL.Models;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper;

namespace YIPLCrimping.BAL.Service
{
    public class WireService
    {
        private readonly WireRepository _wireRepository;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public WireService(WireRepository wireCombinationRepository)
        {
            this._wireRepository = wireCombinationRepository;
        }

        public async Task<List<MWireType>> GetWireType(JObject requestData)
        {
            string? wireTypeCode = requestData["wireTypeCode"]?.ToString();
            string? searchText = requestData["searchText"]?.ToString();
            int? id = requestData["id"]?.ToObject<int>();

            var wireTypes = await _wireRepository.GetWireType(wireTypeCode, searchText, id);
            logger.WriteInfo($"Retrieved {wireTypes.Count} wire types from repository.");
            return wireTypes;
        }

        public async Task<JObject> AddWireType(MWireType wireTypeData)
        {
            var result = new JObject();
            if (string.IsNullOrWhiteSpace(wireTypeData.WireTypeCode) || wireTypeData.WireTypeCode.Length < 3)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeCodeRequired_1402, 400, ErrorNumber.WireTypeCodeRequired);
            }

            if (string.IsNullOrWhiteSpace(wireTypeData.WireTypeName))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeNameRequired_1403, 400, ErrorNumber.WireTypeNameRequired);
            }

            if (wireTypeData.CreatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);
            }

            var existingWithSameCode = await _wireRepository.GetWireType(wireTypeData.WireTypeCode, null, null);
            if (existingWithSameCode.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeCodeAlreadyExists_1412, 400, ErrorNumber.WireTypeCodeAlreadyExists);
            }
            wireTypeData.CreatedDate = DateTime.Now;
            wireTypeData.IsActive = true;

            var addWireType = await _wireRepository.AddWireType(wireTypeData);
            if (addWireType > 0)
            {
                logger.WriteInfo($"WireType {wireTypeData.Id}, {wireTypeData.WireTypeName} is added successfully.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.WireTypeAddedSuccessfully);
                result.Add("Message", SuccessDescription.WireTypeAddedSuccessfully_2101);
            }
            else
            {
                logger.WriteInfo($"WireType {addWireType} could not be added.");
                result.Add("Success", false);
                result.Add("Message", ErrorDescription.WireTypeFailedToAdd_1405);
            }
            return result;
        }

        public async Task<JObject> UpdateWireType(MWireType wireTypeData)
        {
            var result = new JObject();

            if (string.IsNullOrWhiteSpace(wireTypeData.WireTypeCode) || wireTypeData.WireTypeCode.Length < 3)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeCodeRequired_1402, 400, ErrorNumber.WireTypeCodeRequired);
            }

            if (string.IsNullOrWhiteSpace(wireTypeData.WireTypeName))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeNameRequired_1403, 400, ErrorNumber.WireTypeNameRequired);
            }

            if (wireTypeData.ModifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeModifiedByIdRequired_1408, 400, ErrorNumber.WireTypeModifiedByIdRequired);
            }

            var existing = await _wireRepository.GetWireType("", "", wireTypeData.Id);
            if (existing == null || existing.Count <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeNotFound_1404, 404, ErrorNumber.WireTypeNotFound);
            }

            var existingWithSameCode = await _wireRepository.GetWireType(wireTypeData.WireTypeCode, null, null);
            if (existingWithSameCode.Any(x => x.Id != wireTypeData.Id))
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireTypeCodeAlreadyExists_1412,
                    400,
                    ErrorNumber.WireTypeCodeAlreadyExists);
            }

            existing.FirstOrDefault().WireTypeName = wireTypeData.WireTypeName;
            existing.FirstOrDefault().WireTypeCode = wireTypeData.WireTypeCode;
            existing.FirstOrDefault().ModifiedById = wireTypeData.ModifiedById;
            existing.FirstOrDefault().ModifiedDate = DateTime.Now;

            int rowsAffected = await _wireRepository.UpdateWireType(existing.FirstOrDefault());

            if (rowsAffected > 0)
            {
                logger.WriteInfo($"WireType updated successfully: Id={existing.FirstOrDefault().Id}, Name={existing.FirstOrDefault().WireTypeName}");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.WireTypeUpdatedSuccessfully);
                result.Add("Message", SuccessDescription.WireTypeUpdatedSuccessfully_2102);
            }
            else
            {
                logger.WriteError($"Failed to update wire type: Id={existing.FirstOrDefault().Id}");
                result.Add("Success", false);
                result.Add("Message", ErrorDescription.WireTypeFailedToUpdate_1406);
            }

            return result;
        }

        public async Task<JObject> DeleteWireType(MWireType wireTypeData)
        {
            var result = new JObject();

            // Validate input
            if (wireTypeData == null || wireTypeData.Id <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireTypeIdRequired_1401,
                    400,
                    ErrorNumber.WireTypeIdRequired
                );
            }

            if (wireTypeData.ModifiedById == null || wireTypeData.ModifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireTypeModifiedByIdRequired_1408,
                    400,
                    ErrorNumber.WireTypeModifiedByIdRequired
                );
            }

            // Get existing wire type
            var existingWireTypes = await _wireRepository.GetWireType(null, null, wireTypeData.Id);
            var existingWireType = existingWireTypes.FirstOrDefault();

            if (existingWireType == null)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireTypeNotFound_1404,
                    404,
                    ErrorNumber.WireTypeNotFound
                );
            }

            // Check if already inactive
            if (!existingWireType.IsActive)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireTypeAlreadyInactive_1409,
                    409,
                    ErrorNumber.WireTypeAlreadyInactive
                );
            }

            // Perform soft delete
            existingWireType.IsActive = false;
            existingWireType.ModifiedById = wireTypeData.ModifiedById;
            existingWireType.ModifiedDate = DateTime.UtcNow;

            int updated = await _wireRepository.UpdateWireType(existingWireType);

            if (updated > 0)
            {
                logger.WriteInfo($"WireType {existingWireType.Id} deleted (deactivated) successfully.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.WireTypeDeletedSuccessfully);
                result.Add("Message", SuccessDescription.WireTypeDeletedSuccessfully_2103);
            }
            else
            {
                logger.WriteError($"WireType {existingWireType.Id} could not be deactivated.");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireTypeFailedToDelete_1407,
                    500,
                    ErrorNumber.WireTypeFailedToDelete
                );
            }

            return result;
        }

        public async Task<JObject> BulkAddOrUpdate(List<MWireType> wireTypes)
        {
            var result = new JObject();
            var skipped = new JArray();
            int addedRecordsCount = 0;
            int updatedRecordsCount = 0;

            if (wireTypes == null || !wireTypes.Any())
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.NoWireTypeRecordsProvided_1410, 400, ErrorNumber.NoWireTypeRecordsProvided);

            var allWireTypes = await _wireRepository.GetWireType(null, null, null);
            var existingCodes = allWireTypes.Select(x => x.WireTypeCode.ToLower()).ToList();

            var existingIds = wireTypes.Where(w => w.Id > 0).Select(w => w.Id).ToList();
            var existingWireTypes = await _wireRepository.GetWireTypelist(existingIds);
            var newWireTypes = new List<MWireType>();

            foreach (var wireType in wireTypes)
            {
                bool isValid = true;

                if (string.IsNullOrWhiteSpace(wireType.WireTypeName))
                {
                    skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeNameRequired_1403, 400, ErrorNumber.WireTypeNameRequired));
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(wireType.WireTypeCode) || wireType.WireTypeCode.Length < 3)
                {
                    skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeCodeRequired_1402, 400, ErrorNumber.WireTypeCodeRequired));
                    isValid = false;
                }

                if (!isValid) continue;

                if (wireType.Id > 0)
                {
                    if (wireType.ModifiedById == null || wireType.ModifiedById <= 0)
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired));
                        continue;
                    }

                    if (existingWireTypes.TryGetValue(wireType.Id, out var existing))
                    {
                        if (allWireTypes.Any(x => x.Id != wireType.Id && x.WireTypeCode.Equals(wireType.WireTypeCode, StringComparison.OrdinalIgnoreCase)))
                        {
                            skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeCodeAlreadyExists_1412, 400, ErrorNumber.WireTypeCodeAlreadyExists));
                            continue;
                        }

                        existing.WireTypeName = wireType.WireTypeName;
                        existing.WireTypeCode = wireType.WireTypeCode;
                        existing.ModifiedById = wireType.ModifiedById;
                        existing.ModifiedDate = DateTime.UtcNow;

                        await _wireRepository.UpdateWireType(existing);
                        updatedRecordsCount++;
                        logger.WriteInfo($"[BulkUpdate] Updated wire type: Id={existing.Id}");
                    }
                    else
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeNotFound_1404, 400, ErrorNumber.WireTypeNotFound));
                    }
                }
                else
                {
                    bool isDuplicateInBatch = allWireTypes
                   .Any(x => x != wireType && x.WireTypeCode.Equals(wireType.WireTypeCode, StringComparison.OrdinalIgnoreCase));

                    if (isDuplicateInBatch)
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeCodeDuplicateInBatch_1413, 400, ErrorNumber.WireTypeCodeDuplicateInBatch));
                        continue;
                    }

                    if (wireType.CreatedById == null || wireType.CreatedById <= 0)
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired));
                        continue;
                    }

                    newWireTypes.Add(new MWireType
                    {
                        WireTypeName = wireType.WireTypeName?.Trim(),
                        WireTypeCode = wireType.WireTypeCode?.Trim(),
                        CreatedById = wireType.CreatedById,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    });

                    addedRecordsCount++;
                }
            }

            if (newWireTypes.Any())
            {
                await _wireRepository.BulkAdd(newWireTypes);
                logger.WriteInfo($"[BulkAdd] Inserted {newWireTypes.Count} new wire types.");
            }

            result.Add("Success", true);
            result.Add("SuccessNumber", SuccessNumber.WireTypeBulkAddOrUpdateSuccessfully);
            //result.Add("Message", SuccessDescription.WireTypeBulkAddOrUpdateSuccessfully_2111);
            string addMessage = $"{addedRecordsCount} record{(addedRecordsCount == 1 ? "" : "s")} added";
            string updateMessage = $"{updatedRecordsCount} record{(updatedRecordsCount == 1 ? "" : "s")} updated";
            string skipMessage = $"{skipped.Count} duplicate record{(skipped.Count == 1 ? "" : "s")} skipped";

            result.Add("Message", $"Bulk add/update completed. {addMessage}, {updateMessage}, and {skipMessage}.");

            result.Add("TotalProcessed", addedRecordsCount + updatedRecordsCount);
            result.Add("TotalSkipped", skipped.Count());
            if (skipped.Count > 0)
            {
                result.Add("SkippedDetails", skipped);
            }

            return result;
        }

        //public async Task<JObject> WireTypeBulkUpload(IFormFile file, int userId)
        //{
        //    var result = new JObject();

        //    if (file == null || file.Length == 0)
        //        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.NoFileUploaded_1035, 400, ErrorNumber.NoFileUploaded);

        //    if (userId <= 0)
        //        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);

        //    List<MWireType> wireTypes;
        //    try
        //    {
        //        wireTypes = ExcelHelper.ReadWireTypeFromExcel(file.OpenReadStream());
        //        logger.WriteInfo($"[BulkUpload] Read {wireTypes.Count} wire types from Excel.");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.WriteError($"[BulkUpload] Error reading Excel: {ex.Message}");
        //        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.InvalidOrEmptyExcelData_1036, 400, ErrorNumber.InvalidOrEmptyExcelData);
        //    }

        //    if (wireTypes == null || !wireTypes.Any())
        //        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.InvalidOrEmptyExcelData_1036, 400, ErrorNumber.InvalidOrEmptyExcelData);

        //    foreach (var wireType in wireTypes)
        //    {
        //        wireType.WireTypeName = wireType.WireTypeName?.Trim();
        //        wireType.WireTypeCode = wireType.WireTypeCode?.Trim();
        //        wireType.CreatedById = userId;
        //        wireType.CreatedDate = DateTime.UtcNow;
        //        wireType.IsActive = true;
        //    }

        //    try
        //    {
        //        await _wireRepository.BulkInsertWireTypes(wireTypes);

        //        result.Add("Success", true);
        //        result.Add("SuccessNumber", SuccessNumber.WireTypeBulkUploadSuccess);
        //        result.Add("Message", SuccessDescription.WireTypeBulkUploadSuccess_2112);
        //        result.Add("ImportedCount", wireTypes.Count);
        //        logger.WriteInfo($"[BulkUpload] Successfully uploaded {wireTypes.Count} wire types by user {userId}.");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.WriteError($"[BulkUpload] Upload failed: {ex.Message}");
        //        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireTypeBulkUploadFailed_1411, 500, ErrorNumber.WireTypeBulkUploadFailed);
        //    }

        //    return result;
        //}

        //public async Task<JObject> WireTypeBulkUpload(IFormFile file, int userId)
        //{
        //    var result = new JObject();

        //    if (file == null || file.Length == 0)
        //        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.NoFileUploaded_1035, 400, ErrorNumber.NoFileUploaded);

        //    if (userId <= 0)
        //        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);

        //    List<MWireType> wireTypes;
        //    try
        //    {
        //        wireTypes = ExcelHelper.ReadWireTypeFromExcel(file.OpenReadStream());
        //        logger.WriteInfo($"[BulkUpload] Read {wireTypes.Count} wire types from Excel.");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.WriteError($"[BulkUpload] Error reading Excel: {ex.Message}");
        //        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.InvalidOrEmptyExcelData_1036, 400, ErrorNumber.InvalidOrEmptyExcelData);
        //    }

        //    if (wireTypes == null || !wireTypes.Any())
        //        return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.InvalidOrEmptyExcelData_1036, 400, ErrorNumber.InvalidOrEmptyExcelData);

        //    // Get all existing wire type codes for validation
        //    var existingWireTypes = await _wireRepository.GetWireType(null, null, null);
        //    var existingCodes = existingWireTypes.Select(x => x.WireTypeCode?.ToLower()).ToList();

        //    // Filter out duplicates in the file itself
        //    var uniqueWireTypes = new List<MWireType>();
        //    var seenCodes = new HashSet<string>();
        //    var duplicatesInFile = new List<string>();
        //    var invalidRecords = new List<string>();

        //    foreach (var wireType in wireTypes)
        //    {
        //        // Validate required fields
        //        if (string.IsNullOrWhiteSpace(wireType.WireTypeCode) || wireType.WireTypeCode.Length < 3)
        //        {
        //            invalidRecords.Add("Missing WireTypeCode or its less that 3 Character");
        //            continue;
        //        }

        //        if (string.IsNullOrWhiteSpace(wireType.WireTypeName))
        //        {
        //            invalidRecords.Add($"{wireType.WireTypeCode}: Missing WireTypeName");
        //            continue;
        //        }

        //        var codeLower = wireType.WireTypeCode.Trim().ToLower();

        //        // Check for duplicates in the same file
        //        if (seenCodes.Contains(codeLower))
        //        {
        //            duplicatesInFile.Add(wireType.WireTypeCode);
        //            continue;
        //        }

        //        seenCodes.Add(codeLower);

        //        // Check if code already exists in database
        //        if (existingCodes.Contains(codeLower))
        //        {
        //            duplicatesInFile.Add(wireType.WireTypeCode + " (already exists)");
        //            continue;
        //        }

        //        // Prepare valid record
        //        wireType.WireTypeName = wireType.WireTypeName?.Trim();
        //        wireType.WireTypeCode = wireType.WireTypeCode?.Trim();
        //        wireType.CreatedById = userId;
        //        wireType.CreatedDate = DateTime.UtcNow;
        //        wireType.IsActive = true;

        //        uniqueWireTypes.Add(wireType);
        //    }

        //    if (!uniqueWireTypes.Any())
        //    {
        //        string errorMessage = "No valid records to import. ";
        //        if (duplicatesInFile.Count > 0) errorMessage += $"{duplicatesInFile.Count} duplicates found. ";
        //        if (invalidRecords.Count > 0) errorMessage += $"{invalidRecords.Count} invalid records found.";

        //        return ErrorResponseWrapper.CreateErrorResponse(errorMessage.Trim(), 400, ErrorNumber.NoValidWireTypesToImport);
        //    }

        //    try
        //    {
        //        await _wireRepository.BulkInsertWireTypes(uniqueWireTypes);

        //        result.Add("Success", true);
        //        result.Add("SuccessNumber", SuccessNumber.WireTypeBulkUploadSuccess);
        //        //result.Add("Message", SuccessDescription.WireTypeBulkUploadSuccess_2112);
        //        result.Add("Message", $"Successfully added {uniqueWireTypes.Count} new WireTypes. {duplicatesInFile.Count} duplicates were skipped.");
        //        result.Add("ImportedCount", uniqueWireTypes.Count);

        //        if (duplicatesInFile.Count > 0)
        //        {
        //            result.Add("DuplicateCount", duplicatesInFile.Count);
        //            result.Add("DuplicateDetails", JArray.FromObject(duplicatesInFile.Take(10))); // Limit to first 10 for response
        //        }

        //        if (invalidRecords.Count > 0)
        //        {
        //            result.Add("InvalidCount", invalidRecords.Count);
        //            result.Add("InvalidDetails", JArray.FromObject(invalidRecords.Take(10))); // Limit to first 10
        //        }

        //        logger.WriteInfo($"[BulkUpload] Successfully uploaded {uniqueWireTypes.Count} wire types by user {userId}. " +
        //                         $"{duplicatesInFile.Count} duplicates and {invalidRecords.Count} invalid records found.");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.WriteError($"[BulkUpload] Upload failed: {ex.Message}");
        //        return ErrorResponseWrapper.CreateErrorResponse(
        //            $"{ErrorDescription.WireTypeBulkUploadFailed_1411}. Details: {ex.Message}",
        //            500,
        //            ErrorNumber.WireTypeBulkUploadFailed);
        //    }

        //    return result;
        //}

        public async Task<JObject> WireTypeBulkUpload(IFormFile file, int userId)
        {
            var result = new JObject();

            if (file == null || file.Length == 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.NoFileUploaded_1035, 400, ErrorNumber.NoFileUploaded);

            if (userId <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);

            List<MWireType> wireTypes;
            try
            {
                wireTypes = ExcelHelper.ReadWireTypeFromExcel(file.OpenReadStream());
                logger.WriteInfo($"[BulkUpload] Read {wireTypes.Count} wire types from Excel.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"[BulkUpload] Error reading Excel: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.InvalidOrEmptyExcelData_1036, 400, ErrorNumber.InvalidOrEmptyExcelData);
            }

            if (wireTypes == null || !wireTypes.Any())
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.InvalidOrEmptyExcelData_1036, 400, ErrorNumber.InvalidOrEmptyExcelData);

            var existingWireTypes = await _wireRepository.GetWireType(null, null, null);
            var existingCodes = existingWireTypes.Select(x => x.WireTypeCode?.ToLower()).ToList();

            var uniqueWireTypes = new List<MWireType>();
            var seenCodes = new HashSet<string>();
            var duplicatesInFile = new List<string>();
            var invalidRecords = new List<string>();

            int skippedEmptyCode = 0;
            int skippedInvalidLength = 0;

            foreach (var wireType in wireTypes)
            {
                if (string.IsNullOrWhiteSpace(wireType.WireTypeCode) )
                {
                    skippedEmptyCode++;
                    invalidRecords.Add("Missing WireTypeCode");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(wireType.WireTypeName))
                {
                    skippedEmptyCode++;
                    invalidRecords.Add("Missing WireTypeName");
                    continue;
                }

                wireType.WireTypeCode = wireType.WireTypeCode.Trim();

                if (wireType.WireTypeCode.Length < 3)
                {
                    skippedInvalidLength++;
                    invalidRecords.Add($"{wireType.WireTypeCode}: WireTypeCode less than 3 characters");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(wireType.WireTypeName))
                {
                    invalidRecords.Add($"{wireType.WireTypeCode}: Missing WireTypeName");
                    continue;
                }

                wireType.WireTypeName = wireType.WireTypeName?.Trim();
                var codeLower = wireType.WireTypeCode.ToLower();

                if (seenCodes.Contains(codeLower))
                {
                    duplicatesInFile.Add(wireType.WireTypeCode + " (duplicate in file)");
                    continue;
                }

                if (existingCodes.Contains(codeLower))
                {
                    duplicatesInFile.Add(wireType.WireTypeCode + " (already exists)");
                    continue;
                }

                seenCodes.Add(codeLower);

                wireType.CreatedById = userId;
                wireType.CreatedDate = DateTime.UtcNow;
                wireType.IsActive = true;

                uniqueWireTypes.Add(wireType);
            }

            //if (!uniqueWireTypes.Any())
            //{
            //    string errorMessage = "No valid records to import. ";
            //    if (duplicatesInFile.Count > 0) errorMessage += $"{duplicatesInFile.Count} duplicate(s) found. ";
            //    if (invalidRecords.Count > 0) errorMessage += $"{invalidRecords.Count} invalid record(s) found.";

            //    return ErrorResponseWrapper.CreateErrorResponse(errorMessage.Trim(), 400, ErrorNumber.NoValidWireTypesToImport);
            //}

            try
            {
                await _wireRepository.BulkInsertWireTypes(uniqueWireTypes);

                string message = $"Successfully added {uniqueWireTypes.Count} new WireType(s). " +
                                 $"{duplicatesInFile.Count} duplicate(s) and {skippedEmptyCode + skippedInvalidLength} record(s) with invalid or empty WireTypeCode or empty WireTypeName were skipped.";

                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.WireTypeBulkUploadSuccess);
                result.Add("Message", message);
                result.Add("ImportedCount", uniqueWireTypes.Count);
                result.Add("DuplicateCount", duplicatesInFile.Count);
                result.Add("SkippedEmptyCodeCount", skippedEmptyCode);
                result.Add("SkippedInvalidLengthCount", skippedInvalidLength);
                result.Add("InvalidCount", invalidRecords.Count);

                if (duplicatesInFile.Any())
                    result.Add("DuplicateDetails", JArray.FromObject(duplicatesInFile.Take(10)));

                if (invalidRecords.Any())
                    result.Add("InvalidDetails", JArray.FromObject(invalidRecords.Take(10)));

                logger.WriteInfo($"[BulkUpload] Successfully uploaded {uniqueWireTypes.Count} wire types by user {userId}. " +
                                 $"{duplicatesInFile.Count} duplicates, {skippedEmptyCode} empty codes, and {skippedInvalidLength} short codes were skipped.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"[BulkUpload] Upload failed: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    $"{ErrorDescription.WireTypeBulkUploadFailed_1411}. Details: {ex.Message}",
                    500,
                    ErrorNumber.WireTypeBulkUploadFailed);
            }

            return result;
        }


        public async Task<List<MWireSize>> GetWireSize(JObject requestData)
        {
            string wireSizeCode = requestData["wireSizeCode"]?.ToString();
            string searchText = requestData["searchText"]?.ToString();
            int? id = requestData["id"]?.ToObject<int>();
            decimal? wireSize = requestData["wireSize"]?.ToObject<decimal>();

            var wireSizes = await _wireRepository.GetWireSize(wireSizeCode, searchText, id, wireSize);
            logger.WriteInfo($"Retrieved {wireSizes.Count} wire sizes");
            return wireSizes;
        }

        public async Task<JObject> AddWireSize(MWireSize wireSizeData)
        {
            var result = new JObject();

            if (string.IsNullOrWhiteSpace(wireSizeData.WireSizeCode) || wireSizeData.WireSizeCode.Length < 3)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireSizeCodeRequired_1501,
                    400,
                    ErrorNumber.WireSizeCodeRequired);
            }

            if (wireSizeData.WireSize <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireSizeInvalid_1502,
                    400,
                    ErrorNumber.WireSizeInvalid);
            }

            if (wireSizeData.CreatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.CreatedByIdIsRequired_1009,
                    400,
                    ErrorNumber.CreatedByIdIsRequired);
            }

            var existingWithSameCode = await _wireRepository.GetWireSize(wireSizeData.WireSizeCode, null, null, null);
            if (existingWithSameCode.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.WireSizeCodeAlreadyExists_1514, 400, ErrorNumber.WireSizeCodeAlreadyExists);
            }

            wireSizeData.CreatedDate = DateTime.Now;
            wireSizeData.IsActive = true;

            try
            {
                int added = await _wireRepository.AddWireSize(wireSizeData);
                if (added > 0)
                {
                    logger.WriteInfo($"WireSize {wireSizeData.WireSizeCode} added successfully");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.WireSizeAddedSuccessfully);
                    result.Add("Message", SuccessDescription.WireSizeAddedSuccessfully_2201);
                }
                else
                {
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.WireSizeAddFailed_1503,
                        500,
                        ErrorNumber.WireSizeAddFailed);
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"AddWireSize failed: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.DatabaseOperationFailed_1504,
                    500,
                    ErrorNumber.DatabaseOperationFailed);
            }

            return result;
        }

        public async Task<JObject> UpdateWireSize(MWireSize wireSizeData)
        {
            var result = new JObject();

            if (wireSizeData.Id <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireSizeIdRequired_1505,
                    400,
                    ErrorNumber.WireSizeIdRequired);
            }

            if (string.IsNullOrWhiteSpace(wireSizeData.WireSizeCode) || wireSizeData.WireSizeCode.Length < 3)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireSizeCodeRequired_1501,
                    400,
                    ErrorNumber.WireSizeCodeRequired);
            }

            if (wireSizeData.ModifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.ModifiedByIdIsRequired_1057,
                    400,
                    ErrorNumber.ModifiedByIdIsRequired);
            }

            var existing = (await _wireRepository.GetWireSize(null, null, wireSizeData.Id, null)).FirstOrDefault();
            if (existing == null)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireSizeNotFound_1506,
                    404,
                    ErrorNumber.WireSizeNotFound);
            }

            var existingWithSameCode = await _wireRepository.GetWireSize(wireSizeData.WireSizeCode, null, null, null);
            if (existingWithSameCode.Any(x => x.Id != wireSizeData.Id))
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireSizeCodeAlreadyExists_1514,
                    400,
                    ErrorNumber.WireSizeCodeAlreadyExists);
            }

            existing.WireSizeCode = wireSizeData.WireSizeCode;
            existing.WireSize = wireSizeData.WireSize;
            existing.ModifiedById = wireSizeData.ModifiedById;
            existing.ModifiedDate = DateTime.Now;

            try
            {
                int updated = await _wireRepository.UpdateWireSize(existing);
                if (updated > 0)
                {
                    logger.WriteInfo($"WireSize {wireSizeData.Id} updated successfully");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.WireSizeUpdatedSuccessfully);
                    result.Add("Message", SuccessDescription.WireSizeUpdatedSuccessfully_2202);
                }
                else
                {
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.WireSizeUpdateFailed_1507,
                        500,
                        ErrorNumber.WireSizeUpdateFailed);
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"UpdateWireSize failed: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.DatabaseOperationFailed_1504,
                    500,
                    ErrorNumber.DatabaseOperationFailed);
            }

            return result;
        }

        public async Task<JObject> DeleteWireSize(MWireSize wireSizeData)
        {
            var result = new JObject();

            if (wireSizeData.Id <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireSizeIdRequired_1505,
                    400,
                    ErrorNumber.WireSizeIdRequired);
            }

            if (wireSizeData.ModifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.ModifiedByIdIsRequired_1057,
                    400,
                    ErrorNumber.ModifiedByIdIsRequired);
            }

            var existing = (await _wireRepository.GetWireSize(null, null, wireSizeData.Id, null)).FirstOrDefault();
            if (existing == null)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireSizeNotFound_1506,
                    404,
                    ErrorNumber.WireSizeNotFound);
            }

            if (!existing.IsActive)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.WireSizeAlreadyInactive_1508,
                    409,
                    ErrorNumber.WireSizeAlreadyInactive);
            }

            existing.IsActive = false;
            existing.ModifiedById = wireSizeData.ModifiedById;
            existing.ModifiedDate = DateTime.Now;

            try
            {
                int updated = await _wireRepository.UpdateWireSize(existing);
                if (updated > 0)
                {
                    logger.WriteInfo($"WireSize {wireSizeData.Id} deactivated successfully");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.WireSizeDeletedSuccessfully);
                    result.Add("Message", SuccessDescription.WireSizeDeletedSuccessfully_2203);
                }
                else
                {
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.WireSizeDeleteFailed_1509,
                        500,
                        ErrorNumber.WireSizeDeleteFailed);
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"DeleteWireSize failed: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.DatabaseOperationFailed_1504,
                    500,
                    ErrorNumber.DatabaseOperationFailed);
            }

            return result;
        }

        public async Task<JObject> BulkAddOrUpdateWireSize(List<MWireSize> wireSizes)
        {
            var result = new JObject();
            var skipped = new JArray();
            int addedRecordCount = 0;
            int updatedRecordCount = 0;

            if (wireSizes == null || !wireSizes.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoWireSizesProvided_1510,
                    400,
                    ErrorNumber.NoWireSizesProvided);
            }

            var allWireSizes = await _wireRepository.GetWireSize(null, null, null, null);
            var existingCodes = allWireSizes.Select(x => x.WireSizeCode.ToLower()).ToList();

            var existingIds = wireSizes.Where(x => x.Id > 0).Select(x => x.Id).ToList();
            var existingWireSizes = await _wireRepository.GetWireSizeList(existingIds);
            var newWireSizes = new List<MWireSize>();

            foreach (var wireSize in wireSizes)
            {
                try
                {
                    bool isValid = true;
                    var error = new JObject();

                    if (string.IsNullOrWhiteSpace(wireSize.WireSizeCode) || wireSize.WireSizeCode.Length < 3)
                    {
                        error.Add("Error", ErrorDescription.WireSizeCodeRequired_1501);
                        error.Add("ErrorNumber", ErrorNumber.WireSizeCodeRequired);
                        isValid = false;
                    }

                    if (wireSize.WireSize <= 0)
                    {
                        error.Add("Error", ErrorDescription.WireSizeInvalid_1502);
                        error.Add("ErrorNumber", ErrorNumber.WireSizeInvalid);
                        isValid = false;
                    }

                    if (!isValid)
                    {
                        skipped.Add(error);
                        continue;
                    }

                    if (wireSize.Id > 0) // Update existing
                    {
                        if (wireSize.ModifiedById <= 0)
                        {
                            error.Add("Error", ErrorDescription.ModifiedByIdIsRequired_1057);
                            error.Add("ErrorNumber", ErrorNumber.ModifiedByIdIsRequired);
                            skipped.Add(error);
                            continue;
                        }

                        if (existingWireSizes.TryGetValue(wireSize.Id, out var existing))
                        {
                            if (allWireSizes.Any(x => x.Id != wireSize.Id && x.WireSizeCode.Equals(wireSize.WireSizeCode, StringComparison.OrdinalIgnoreCase)))
                            {
                                error.Add("Error", ErrorDescription.WireSizeCodeAlreadyExists_1514);
                                error.Add("ErrorNumber", ErrorNumber.WireSizeCodeAlreadyExists);
                                skipped.Add(error);
                                continue;
                            }

                            existing.WireSizeCode = wireSize.WireSizeCode;
                            existing.WireSize = wireSize.WireSize;
                            existing.ModifiedById = wireSize.ModifiedById;
                            existing.ModifiedDate = DateTime.Now;

                            await _wireRepository.UpdateWireSize(existing);
                            updatedRecordCount++;
                            logger.WriteInfo($"Updated wire size {wireSize.Id}");
                        }
                        else
                        {
                            error.Add("Error", ErrorDescription.WireSizeNotFound_1506);
                            error.Add("ErrorNumber", ErrorNumber.WireSizeNotFound);
                            skipped.Add(error);
                        }
                    }
                    else // Add new
                    {
                        var duplicateCodeInBatch = allWireSizes.Where(x => x.WireSizeCode.Equals(wireSize.WireSizeCode, StringComparison.OrdinalIgnoreCase)).ToList();

                        if (duplicateCodeInBatch.Count > 0)
                        {
                            error.Add("Error", ErrorDescription.WireSizeCodeDuplicateInBatch_1516);
                            error.Add("ErrorNumber", ErrorNumber.WireSizeCodeDuplicateInBatch);
                            skipped.Add(error);
                            continue;
                        }

                        if (wireSize.CreatedById <= 0)
                        {
                            error.Add("Error", ErrorDescription.CreatedByIdIsRequired_1009);
                            error.Add("ErrorNumber", ErrorNumber.CreatedByIdIsRequired);
                            skipped.Add(error);
                            continue;
                        }

                        newWireSizes.Add(new MWireSize
                        {
                            WireSizeCode = wireSize.WireSizeCode,
                            WireSize = wireSize.WireSize,
                            CreatedById = wireSize.CreatedById,
                            CreatedDate = DateTime.Now,
                            IsActive = true
                        });
                        addedRecordCount++;
                    }
                }
                catch (Exception ex)
                {
                    logger.WriteError($"Error processing wire size: {ex.Message}");
                    var error = new JObject
                {
                    { "Error", $"Processing error: {ex.Message}" },
                    { "ErrorNumber", ErrorNumber.DatabaseOperationFailed }
                };
                    skipped.Add(error);
                }
            }

            if (newWireSizes.Any())
            {
                await _wireRepository.BulkAddWireSize(newWireSizes);
                logger.WriteInfo($"Added {newWireSizes.Count} new wire sizes");
            }

            result.Add("Success", true);
            result.Add("SuccessNumber", SuccessNumber.WireSizeBulkProcessedSuccessfully);
            //result.Add("Message", SuccessDescription.WireSizeBulkProcessedSuccessfully_2211);
            string addMessage = $"{addedRecordCount} record{(addedRecordCount == 1 ? "" : "s")} added";
            string updateMessage = $"{updatedRecordCount} record{(updatedRecordCount == 1 ? "" : "s")} updated";
            string skipMessage = $"{skipped.Count} duplicate record{(skipped.Count == 1 ? "" : "s")} skipped";

            result.Add("Message", $"Bulk add/update completed. {addMessage}, {updateMessage}, and {skipMessage}.");

            result.Add("ProcessedCount", addedRecordCount + updatedRecordCount);
            result.Add("SkippedCount", skipped.Count);
            if (skipped.Count > 0)
            {
                result.Add("SkippedItems", skipped);
            }

            return result;
        }


        public async Task<JObject> WireSizeBulkUpload(IFormFile file, int userId)
        {
            var result = new JObject();

            if (file == null || file.Length == 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoFileUploaded_1035,
                    400,
                    ErrorNumber.NoFileUploaded);
            }

            if (userId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.CreatedByIdIsRequired_1009,
                    400,
                    ErrorNumber.CreatedByIdIsRequired);
            }

            List<MWireSize> wireSizes;
            try
            {
                wireSizes = ExcelHelper.ReadWireSizeFromExcel(file.OpenReadStream());
                logger.WriteInfo($"[WireSize BulkUpload] Read {wireSizes?.Count ?? 0} wire sizes from Excel.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"[WireSize BulkUpload] Error reading Excel: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.InvalidOrEmptyExcelData_1036,
                    400,
                    ErrorNumber.InvalidOrEmptyExcelData);
            }

            if (wireSizes == null || !wireSizes.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoValidWireSizes_1512,
                    400,
                    ErrorNumber.NoValidWireSizes);
            }

            var existingWireSizes = await _wireRepository.GetWireSize(null, null, null, null);
            var existingCodes = existingWireSizes.Select(x => x.WireSizeCode?.ToLower()).ToList();

            var uniqueWireSizes = new List<MWireSize>();
            var seenCodes = new HashSet<string>();
            var duplicatesInFile = new List<string>();
            var invalidRecords = new List<string>();

            int skippedEmptyCode = 0;
            int skippedInvalidLength = 0;

            foreach (var wireSize in wireSizes)
            {
                if (string.IsNullOrWhiteSpace(wireSize.WireSizeCode) )
                {
                    skippedEmptyCode++;
                    invalidRecords.Add("Missing WireSizeCode");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(wireSize.WireSizeCode))
                {
                    skippedEmptyCode++;
                    invalidRecords.Add("Missing WireSize");
                    continue;
                }
                wireSize.WireSizeCode = wireSize.WireSizeCode.Trim();

                if (wireSize.WireSizeCode.Length < 3)
                {
                    skippedInvalidLength++;
                    invalidRecords.Add($"{wireSize.WireSizeCode}: WireSizeCode less than 3 characters");
                    continue;
                }

                if (wireSize.WireSize <= 0)
                {
                    invalidRecords.Add($"{wireSize.WireSizeCode}: Invalid WireSize value");
                    continue;
                }

                var codeLower = wireSize.WireSizeCode.ToLower();

                if (seenCodes.Contains(codeLower))
                {
                    duplicatesInFile.Add(wireSize.WireSizeCode + " (duplicate in file)");
                    continue;
                }

                if (existingCodes.Contains(codeLower))
                {
                    duplicatesInFile.Add(wireSize.WireSizeCode + " (already exists)");
                    continue;
                }

                seenCodes.Add(codeLower);

                wireSize.CreatedById = userId;
                wireSize.CreatedDate = DateTime.UtcNow;
                wireSize.IsActive = true;

                uniqueWireSizes.Add(wireSize);
            }

            //if (!uniqueWireSizes.Any())
            //{
            //    string errorMessage = "No valid records to import. ";
            //    if (duplicatesInFile.Count > 0) errorMessage += $"{duplicatesInFile.Count} duplicate(s) found. ";
            //    if (invalidRecords.Count > 0) errorMessage += $"{invalidRecords.Count} invalid record(s) found.";

            //    return ErrorResponseWrapper.CreateErrorResponse(
            //        errorMessage.Trim(),
            //        400,
            //        ErrorNumber.NoValidWireSizesToImport);
            //}

            try
            {
                await _wireRepository.BulkInsertWireSizes(uniqueWireSizes);

                string message = $"Successfully added {uniqueWireSizes.Count} new WireSize(s). " +
                                 $"{duplicatesInFile.Count} duplicate(s) and {skippedEmptyCode + skippedInvalidLength} record(s) with invalid or empty WireSizeCode or empty WireSize were skipped.";

                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.WireSizeUploadSuccessfully);
                result.Add("Message", message);
                result.Add("ImportedCount", uniqueWireSizes.Count);
                result.Add("DuplicateCount", duplicatesInFile.Count);
                result.Add("SkippedEmptyCodeCount", skippedEmptyCode);
                result.Add("SkippedInvalidLengthCount", skippedInvalidLength);
                result.Add("InvalidCount", invalidRecords.Count);

                if (duplicatesInFile.Any())
                {
                    result.Add("DuplicateDetails", JArray.FromObject(duplicatesInFile.Take(10)));
                }

                if (invalidRecords.Any())
                {
                    result.Add("InvalidDetails", JArray.FromObject(invalidRecords.Take(10)));
                }

                logger.WriteInfo($"[WireSize BulkUpload] Successfully uploaded {uniqueWireSizes.Count} wire sizes. " +
                                 $"{duplicatesInFile.Count} duplicates, {skippedEmptyCode} empty codes, and {skippedInvalidLength} short codes were skipped.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"[WireSize BulkUpload] Upload failed: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    $"{ErrorDescription.WireSizeUploadFailed_1513}. Details: {ex.Message}",
                    500,
                    ErrorNumber.WireSizeUploadFailed);
            }

            return result;
        }

    }
}