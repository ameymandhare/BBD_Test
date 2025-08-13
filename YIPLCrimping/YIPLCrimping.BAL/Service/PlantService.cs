using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using YIPLCrimping.DAL;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.BAL.Service
{
    public class PlantService
    {
        private readonly PlantRepository plantRepository;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public PlantService(PlantRepository plantRepository)
        {
            this.plantRepository = plantRepository;
        }

        public async Task<List<MPlant>> Get(JObject requestData)
        {
            try
            {
                string? plantCode = requestData["plantCode"]?.ToString();
                string? plantName = requestData["plantName"]?.ToString();
                string? city = requestData["city"]?.ToString();
                int? id = requestData["id"]?.ToObject<int>();

                var plants = await plantRepository.GetPlants(plantCode, plantName,city, id);
                logger.WriteInfo($"Retrieved {plants.Count} plants from repository.");
                return plants;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MasterService.GetPlants: {ex.Message}");
                throw;
            }
        }

        public async Task<JObject> Add(MPlant plant)
        {
            var result = new JObject();

            if (string.IsNullOrWhiteSpace(plant.PlantCode) || plant.PlantCode.Length < 3)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.PlantCodeIsRequired_1026, 400, ErrorNumber.PlantCodeIsRequired);
            }

            if (string.IsNullOrWhiteSpace(plant.PlantName))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.PlantNameIsRequired_1027, 400, ErrorNumber.PlantNameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(plant.City))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CityIsRequired_1048, 400, ErrorNumber.CityIsRequired);
            }

            if (plant.CreatedById == null || plant.CreatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.PlantCreatedByIdRequired_1028, 400, ErrorNumber.PlantCreatedByIdRequired);
            }

            var isPlantExist = await plantRepository.GetByCode(plant.PlantCode);
            if (isPlantExist != null)
            {
                string error = string.Format(ErrorDescription.PlantCodeAlreadyExists_1029, plant.PlantCode);
                return ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.PlantCodeAlreadyExists);
            }

            var added = await plantRepository.AddPlant(plant);
            if (added > 0)
            {
                logger.WriteInfo($"Plant {plant.PlantCode}, {plant.PlantName} added successfully.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.PlantAddedSuccessfully);
                result.Add("Message", SuccessDescription.PlantAddedSuccessfully_2009);
            }
            else
            {
                result.Add("Success", false);
                result.Add("Message", ErrorDescription.FailedToAddPlant_1030);
            }

            return result;
        }
        public async Task<JObject> Update(MPlant plant)
        {
            var result = new JObject();

            // Validate PlantCode
            if (plant.Id == null || plant.Id <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.IdIsRequired_1047, 400, ErrorNumber.IdIsRequired);
            }
            if (plant.ModifiedById == null || plant.ModifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired);
            }
            // Get existing plant
            var existingPlant = await plantRepository.GetById(plant.Id);

            if (existingPlant != null)
            {
                // Update fields only if provided
                if (!string.IsNullOrWhiteSpace(plant.PlantName))
                {
                    existingPlant.PlantName = plant.PlantName;
                }

                if (!string.IsNullOrWhiteSpace(plant.City))
                {
                    existingPlant.City = plant.City;
                }

                if (!string.IsNullOrEmpty(plant.PlantCode))
                {
                    if (existingPlant.PlantCode != plant.PlantCode)
                    {
                        var isPlantExist = await plantRepository.GetByCode(plant.PlantCode);
                        if (isPlantExist != null)
                        {
                            string error = string.Format(ErrorDescription.PlantCodeAlreadyExists_1029, plant.PlantCode);
                            return ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.PlantCodeAlreadyExists);
                        }
                        else
                        {
                            existingPlant.PlantCode = plant.PlantCode;
                        }
                    }
                    else
                    {
                        existingPlant.PlantCode = plant.PlantCode;
                    }
                }

                existingPlant.ModifiedById = plant.ModifiedById;
                existingPlant.ModifiedDate = DateTime.UtcNow;

                int updated = await plantRepository.Update(existingPlant);

                if (updated > 0)
                {
                    logger.WriteInfo($"Plant {existingPlant.PlantCode} updated successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.PlantUpdatedSuccessfully);
                    result.Add("Message", SuccessDescription.PlantUpdatedSuccessfully_2010);
                }
                else
                {
                    logger.WriteInfo($"Plant {existingPlant.PlantCode} could not be updated.");
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToUpdatePlant_1031, 500, ErrorNumber.FailedToUpdatePlant);
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.PlantNotFound_1032, plant.PlantCode),
                    404,
                    ErrorNumber.PlantNotFound
                );
            }

            return result;
        }
        public async Task<JObject> Delete(JObject plantData)
        {
            var result = new JObject();
            var id = plantData["id"].ToObject<int>();
            var modifiedById = plantData["modifiedById"]?.ToObject<int>();

            //if (string.IsNullOrWhiteSpace(plantCode))
            //{
            //    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.PlantCodeIsRequired_1026, 400, ErrorNumber.PlantCodeIsRequired);
            //}
            if (id <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.IdIsRequired_1047, 400, ErrorNumber.IdIsRequired);
            }
            if (modifiedById == null || modifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired);
            }

            var existingPlant = await plantRepository.GetById(id);

            if (existingPlant != null)
            {
                existingPlant.IsActive = false;
                existingPlant.ModifiedById = modifiedById;
                existingPlant.ModifiedDate = DateTime.UtcNow;

                int updated = await plantRepository.Update(existingPlant);

                if (updated > 0)
                {
                    logger.WriteInfo($"Plant {existingPlant.PlantCode} deleted (deactivated) successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.PlantDeletedSuccessfully);
                    result.Add("Message", SuccessDescription.PlantDeletedSuccessfully_2011);
                }
                else
                {
                    logger.WriteInfo($"Plant {existingPlant.PlantCode} could not be deactivated.");
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToUpdatePlant_1031, 500, ErrorNumber.FailedToUpdatePlant);
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.PlantNotFound_1032, id),
                    404,
                    ErrorNumber.PlantNotFound
                );
            }

            return result;
        }

        public async Task<JObject> BulkAddOrUpdate(List<MPlant> plants)
        {
            var result = new JObject();

            if (plants == null || !plants.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoPlantRecordsProvided_1033,
                    400,
                    ErrorNumber.NoPlantRecordsProvided
                );
            }

            var currentUtc = DateTime.UtcNow;

            // Separate updates and inserts
            var updateIds = plants.Where(p => p.Id > 0).Select(p => p.Id).ToList();
            var existingMap = await plantRepository.GetExistingPlantsById(updateIds);

            var newPlants = new List<MPlant>();
            var updatedPlants = new List<MPlant>();
            var duplicateCodes = new List<string>();

            foreach (var plant in plants)
            {
                if (plant.Id > 0 && existingMap.ContainsKey(plant.Id))
                {
                    if (plant.ModifiedById <= 0)
                    {
                        return ErrorResponseWrapper.CreateErrorResponse(
                           ErrorDescription.ModifiedByIdIsRequired_1057,
                            400,
                            ErrorNumber.ModifiedByIdIsRequired
                        );
                    }
                    var existing = existingMap[plant.Id];
                    if (!string.IsNullOrWhiteSpace(plant.PlantCode))
                    {
                        if (existing.PlantCode != plant.PlantCode)
                        {
                            var isPlantExist = await plantRepository.GetByCode(plant.PlantCode);
                            if (isPlantExist != null)
                            {
                                duplicateCodes.Add(plant.PlantCode);
                                continue;
                            }
                            else
                            {
                                existing.PlantCode = plant.PlantCode;
                            }
                        }
                        else
                        {
                            existing.PlantCode = plant.PlantCode;
                        }
                    }

                    //existing.PlantCode = plant.PlantCode;
                    existing.PlantName = !string.IsNullOrWhiteSpace(plant.PlantName) ? plant.PlantName : existing.PlantName;
                    existing.City = !string.IsNullOrWhiteSpace(plant.City) ? plant.City : existing.City; // Add City update
                    existing.ModifiedById = plant.ModifiedById;
                    existing.ModifiedDate = currentUtc;

                    updatedPlants.Add(existing);
                }
                else
                {
                    if (plant.CreatedById <= 0)
                    {
                        return ErrorResponseWrapper.CreateErrorResponse(
                            ErrorDescription.CreatedByIdIsRequired_1009,
                            400,
                            ErrorNumber.CreatedByIdIsRequired
                        );
                    }
                    // Insert
                    var isPlantExist = await plantRepository.GetByCode(plant.PlantCode);
                    if (isPlantExist != null)
                    {
                        duplicateCodes.Add(plant.PlantCode);
                        continue; // Skip this one
                    }

                    var newPlant = new MPlant
                    {
                        PlantCode = plant.PlantCode,
                        PlantName = plant.PlantName,
                        City = plant.City,
                        CreatedById = plant.CreatedById,
                        CreatedDate = currentUtc,
                        IsActive = true
                    };

                    newPlants.Add(newPlant);
                }
            }
            if (newPlants.Count > 0 || updatedPlants.Count > 0)
            {
                int processed = await plantRepository.BulkAddOrUpdatePlant(newPlants, updatedPlants);

                if (processed > 0)
                {
                    logger.WriteInfo($"Bulk add/update completed. {processed} records processed.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.PlantBulkAddOrUpdateSuccess);
                    string addMessage = $"{newPlants.Count} record{(newPlants.Count == 1 ? "" : "s")} added";
                    string updateMessage = $"{updatedPlants.Count} record{(updatedPlants.Count == 1 ? "" : "s")} updated";
                    string skipMessage = $"{duplicateCodes.Count} duplicate record{(duplicateCodes.Count == 1 ? "" : "s")} skipped";

                    result.Add("Message", $"Bulk add/update completed. {addMessage}, {updateMessage}, and {skipMessage}.");
                    //result.Add("Message", $"Bulk add/update completed. {newPlants.Count} records are added, {updatedPlants.Count} are updated and {duplicateCodes.Count} are duplicate records.");
                    result.Add("ProcessedCount", processed);

                    if (duplicateCodes.Any())
                    {
                        result.Add("SkippedDuplicates", JArray.FromObject(duplicateCodes.Distinct()));
                        result.Add("WarningMessage", $"Skipped {duplicateCodes.Count} duplicate PlantCode(s): {string.Join(", ", duplicateCodes.Distinct())}");
                    }
                }
                else
                {
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.PlantBulkAddOrUpdateFailed_1034,
                        500,
                        ErrorNumber.PlantBulkAddOrUpdateFailed
                    );
                }
            }
            if (duplicateCodes.Count > 0 && newPlants.Count <= 0 && updatedPlants.Count <= 0)
            {
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.PlantBulkAddOrUpdateSuccess);
                string duplicateList = string.Join(", ", duplicateCodes.Distinct());
                string duplicateLabel = duplicateCodes.Count == 1 ? "duplicate PlantCode was" : "duplicate PlantCodes were";

                result.Add("Message", $"{duplicateCodes.Count} {duplicateLabel} found and skipped. Please provide unique PlantCode values. Skipped codes: {duplicateList}");
            }
            return result;
        }

        //public async Task<JObject> PlantBulkUpload(IFormFile file, int userId)
        //{
        //    var result = new JObject();

        //    if (file == null || file.Length == 0)
        //    {
        //        return ErrorResponseWrapper.CreateErrorResponse(
        //            ErrorDescription.NoFileUploaded_1035,
        //            400,
        //            ErrorNumber.NoFileUploaded
        //        );
        //    }

        //    if (userId <= 0)
        //    {
        //        return ErrorResponseWrapper.CreateErrorResponse(
        //            ErrorDescription.CreatedByIdIsRequired_1009,
        //            400,
        //            ErrorNumber.CreatedByIdIsRequired
        //        );
        //    }

        //    List<MPlant> plants;

        //    try
        //    {
        //        plants = ExcelHelper.ReadPlantFromExcel(file.OpenReadStream());
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.WriteError($"Error reading Excel file: {ex.Message}");
        //        return ErrorResponseWrapper.CreateErrorResponse(
        //            ErrorDescription.InvalidOrEmptyExcelData_1036,
        //            400,
        //            ErrorNumber.InvalidOrEmptyExcelData
        //        );
        //    }

        //    if (plants == null || !plants.Any())
        //    {
        //        return ErrorResponseWrapper.CreateErrorResponse(
        //            ErrorDescription.InvalidOrEmptyExcelData_1036,
        //            400,
        //            ErrorNumber.InvalidOrEmptyExcelData
        //        );
        //    }

        //    try
        //    {
        //        logger.WriteInfo($"Importing {plants.Count} plants from Excel for user {userId}.");

        //        List<MPlant> validPlants = new List<MPlant>();
        //        List<string> duplicateCodes = new List<string>();

        //        foreach (var plant in plants)
        //        {
        //            var existing = await plantRepository.GetByCode(plant.PlantCode);
        //            if (existing == null)
        //            {
        //                validPlants.Add(plant);
        //            }
        //            else
        //            {
        //                duplicateCodes.Add(plant.PlantCode);
        //            }
        //        }

        //        if (validPlants.Any())
        //        {
        //            //string message = await plantRepository.PlantBulkUploadAsync(validPlants, userId);
        //            string message = await plantRepository.PlantBulkUploadAsync(validPlants, userId, duplicateCodes.Count);

        //            result.Add("Success", true);
        //            result.Add("SuccessNumber", SuccessNumber.PlantBulkUploadSuccess);
        //            result.Add("Message", message);
        //            result.Add("InsertedCount", validPlants.Count);
        //        }
        //        else
        //        {
        //            result.Add("Success", false);
        //            result.Add("Message", "No new plants were added. All records had duplicate Plant Codes.");
        //            result.Add("DuplicatePlantCodes", string.Join(", ", duplicateCodes));
        //            return result;
        //        }

        //        if (duplicateCodes.Any())
        //        {
        //            result.Add("DuplicatePlantCodes", string.Join(", ", duplicateCodes));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.WriteError($"Error in PlantBulkUploadAsync: {ex.Message}");
        //        return ErrorResponseWrapper.CreateErrorResponse(
        //            ErrorDescription.PlantBulkUploadFailed_1037,
        //            500,
        //            ErrorNumber.PlantBulkUploadFailed
        //        );
        //    }

        //    return result;
        //}

        public async Task<JObject> PlantBulkUpload(IFormFile file, int userId)
        {
            var result = new JObject();

            if (file == null || file.Length == 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoFileUploaded_1035,
                    400,
                    ErrorNumber.NoFileUploaded
                );
            }

            if (userId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.CreatedByIdIsRequired_1009,
                    400,
                    ErrorNumber.CreatedByIdIsRequired
                );
            }

            List<MPlant> plants;

            try
            {
                plants = ExcelHelper.ReadPlantFromExcel(file.OpenReadStream());
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error reading Excel file: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.InvalidOrEmptyExcelData_1036,
                    400,
                    ErrorNumber.InvalidOrEmptyExcelData
                );
            }

            if (plants == null || !plants.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.InvalidOrEmptyExcelData_1036,
                    400,
                    ErrorNumber.InvalidOrEmptyExcelData
                );
            }

            try
            {
                logger.WriteInfo($"Importing {plants.Count} plants from Excel for user {userId}.");

                List<MPlant> validPlants = new List<MPlant>();
                List<string> duplicateCodes = new List<string>();
                int skippedCount = 0;
                int skippedPlantNameCount = 0;
                int skippedCityCount = 0;

                foreach (var plant in plants)
                {
                    if (string.IsNullOrWhiteSpace(plant.PlantCode) || plant.PlantCode.Length < 3)
                    {
                        skippedCount++;
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(plant.PlantName))
                    {
                        skippedPlantNameCount++;
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(plant.City))
                    {
                        skippedCityCount++;
                        continue;
                    }
                    var existing = await plantRepository.GetByCode(plant.PlantCode);
                    if (existing == null)
                    {
                        validPlants.Add(plant);
                    }
                    else
                    {
                        duplicateCodes.Add(plant.PlantCode);
                    }
                }

                if (validPlants.Any())
                {
                    var totalSkipped = skippedCount + skippedPlantNameCount + skippedCityCount;
                    string message = await plantRepository.PlantBulkUploadAsync(validPlants, userId, duplicateCodes.Count, totalSkipped);

                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.PlantBulkUploadSuccess);
                    result.Add("Message", message);
                    result.Add("InsertedCount", validPlants.Count);
                    result.Add("SkippedCount", skippedCount);
                }
                else
                {
                    result.Add("Success", false);
                    result.Add("Message", "No new plants were added. All records were either duplicates or had empty Plant Codes.");
                    result.Add("DuplicatePlantCodes", string.Join(", ", duplicateCodes));
                    result.Add("SkippedCount", skippedCount);
                    return result;
                }

                if (duplicateCodes.Any())
                {
                    result.Add("DuplicatePlantCodes", string.Join(", ", duplicateCodes));
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in PlantBulkUploadAsync: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.PlantBulkUploadFailed_1037,
                    500,
                    ErrorNumber.PlantBulkUploadFailed
                );
            }

            return result;
        }
    }
}