using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using YIPLCrimping.DAL;
using YIPLCrimping.DAL.Models;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper;

namespace YIPLCrimping.BAL.Service
{
    public class MachineService
    {
        private readonly MachineRepository machineRepository;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;
        public MachineService(MachineRepository machineRepository)
        {
            this.machineRepository = machineRepository;
        }
        public async Task<List<MMachine>> Get(JObject requestData)
        {
            try
            {
                string? machineName = requestData["machineName"]?.ToString();
                int? id = requestData["id"]?.ToObject<int>();
                int? plantId = requestData["plantId"]?.ToObject<int>();

                var machines = await machineRepository.GetMachines(machineName, id, plantId);
                logger.WriteInfo($"Retrieved {machines.Count} machines from repository.");
                return machines;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MachineService.GetMachines: {ex.Message}");
                throw;
            }
        }
        public async Task<JObject> Add(MMachine machine)
        {
            var result = new JObject();

            if (string.IsNullOrWhiteSpace(machine.MachineName))
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.MachineNameIsRequired_1025,
                    400,
                    ErrorNumber.MachineNameIsRequired
                );
            }

            if (machine.MachineCost <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.MachineCostInvalid_1014,
                    400,
                    ErrorNumber.MachineCostInvalid
                );
            }

            if (machine.PlantId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.MachinePlantIdIsRequired_1015,
                    400,
                    ErrorNumber.MachinePlantIdIsRequired
                );
            }

            if (machine.CreatedById == null || machine.CreatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.MachineCreatedByIdRequired_1016,
                    400,
                    ErrorNumber.MachineCreatedByIdRequired
                );
            }

            var isMachineExist = await machineRepository.MachineNameExistsAsync(machine.MachineName);
            if (isMachineExist != null)
            {
                string error = string.Format(ErrorDescription.MachineNameAlreadyExists_1017, machine.MachineName);
                return ErrorResponseWrapper.CreateErrorResponse(
                    error,
                    409,
                    ErrorNumber.MachineNameAlreadyExists
                );
            }

            var added = await machineRepository.AddMachine(machine);
            if (added > 0)
            {
                logger.WriteInfo($"Machine {machine.MachineName} added successfully.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.MachineAddedSuccessfully);
                result.Add("Message", SuccessDescription.MachineAddedSuccessfully_2004);
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.FailedToAddMachine_1018,
                    500,
                    ErrorNumber.FailedToAddMachine
                );
            }

            return result;
        }
        public async Task<JObject> Update(MMachine machine)
        {
            var result = new JObject();

            if (machine.Id == null || machine.Id <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.IdIsRequired_1047, 400, ErrorNumber.IdIsRequired);
            }

            if (machine.ModifiedById == null || machine.ModifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.ModifiedByIdIsRequired_1057,
                    400,
                    ErrorNumber.ModifiedByIdIsRequired
                );
            }

            var existingMachine = await machineRepository.GetById(machine.Id);

            if (existingMachine != null)
            {
                // Update only provided fields
                if (!string.IsNullOrEmpty(machine.MachineName))
                {
                    if (existingMachine.MachineName != machine.MachineName)
                    {
                        var isMachineExist = await machineRepository.MachineNameExistsAsync(machine.MachineName);
                        if (isMachineExist != null)
                        {
                            string error = string.Format(ErrorDescription.MachineNameAlreadyExists_1017, machine.MachineName);
                            return ErrorResponseWrapper.CreateErrorResponse(
                                error,
                                409,
                                ErrorNumber.MachineNameAlreadyExists
                            );
                        }
                        else
                        {
                            existingMachine.MachineName = machine.MachineName;
                            logger.WriteInfo($"Machine name updated to {machine.MachineName}.");
                        }
                    }
                    else
                    {
                        existingMachine.MachineName = machine.MachineName;
                    }
                }
                if (machine.MachineCost > 0)
                {
                    existingMachine.MachineCost = Convert.ToDecimal(machine.MachineCost);
                }

                if (machine.PlantId > 0)
                {
                    existingMachine.PlantId = machine.PlantId;
                    existingMachine.Plant = null;
                }

                existingMachine.ModifiedById = machine.ModifiedById;
                existingMachine.ModifiedDate = DateTime.UtcNow;

                int updated = await machineRepository.Update(existingMachine);

                if (updated > 0)
                {
                    logger.WriteInfo($"Machine {existingMachine.MachineName} updated successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.MachineUpdatedSuccessfully);
                    result.Add("Message", SuccessDescription.MachineUpdatedSuccessfully_2005);
                }
                else
                {
                    logger.WriteInfo($"Machine {existingMachine.MachineName} could not be updated.");
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.FailedToUpdateMachine_1019,
                        500,
                        ErrorNumber.FailedToUpdateMachine
                    );
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.MachineNotFound_1020, machine.MachineName),
                    404,
                    ErrorNumber.MachineNotFound
                );
            }

            return result;
        }
        public async Task<JObject> Delete(JObject machineData)
        {
            var result = new JObject();
            var machineId = machineData["machineId"].ToObject<int>();
            var modifiedById = machineData["modifiedById"]?.ToObject<int>();

            if (machineId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.MachineIdIsRequired_1013,
                    400,
                    ErrorNumber.MachineIdIsRequired
                );
            }

            if (modifiedById == null || modifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.ModifiedByIdIsRequired_1057,
                    400,
                    ErrorNumber.ModifiedByIdIsRequired
                );
            }

            var existingMachine = await machineRepository.GetById(machineId);

            if (existingMachine != null)
            {
                existingMachine.IsActive = false;
                existingMachine.ModifiedById = modifiedById;
                existingMachine.ModifiedDate = DateTime.UtcNow;

                int updated = await machineRepository.Update(existingMachine);

                if (updated > 0)
                {
                    logger.WriteInfo($"Machine {existingMachine.MachineName} deactivated successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.MachineDeletedSuccessfully);
                    result.Add("Message", SuccessDescription.MachineDeletedSuccessfully_2006);
                }
                else
                {
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.FailedToUpdateMachine_1019,
                        500,
                        ErrorNumber.FailedToUpdateMachine
                    );
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.MachineNotFound_1020, machineId),
                    404,
                    ErrorNumber.MachineNotFound
                );
            }

            return result;
        }
        public async Task<JObject> BulkAddOrUpdate(List<MMachine> machines)
        {
            var result = new JObject();

            if (machines == null || !machines.Any())
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoMachineRecordsProvided_1020,
                    400,
                    ErrorNumber.NoMachineRecordsProvided
                );

            var currentUtc = DateTime.UtcNow;
            var updateIds = machines.Where(m => m.Id > 0).Select(m => m.Id).ToList();
            var existingMap = await machineRepository.GetExistingMachinesById(updateIds);

            var newList = new List<MMachine>();
            var updateList = new List<MMachine>();
            var duplicateCodes = new List<string>();

            foreach (var m in machines)
            {
                if (m.Id > 0 && existingMap.TryGetValue(m.Id, out var existing))
                {
                    if (m.ModifiedById <= 0)
                    {
                        return ErrorResponseWrapper.CreateErrorResponse(
                           ErrorDescription.ModifiedByIdIsRequired_1057,
                            400,
                            ErrorNumber.ModifiedByIdIsRequired
                        );
                    }
                    if (existing.MachineName != m.MachineName)
                    {
                        var isMachineExist = await machineRepository.MachineNameExistsAsync(m.MachineName);
                        if (isMachineExist != null)
                        {
                            string error = string.Format(ErrorDescription.MachineNameAlreadyExists_1017, m.MachineName);
                            return ErrorResponseWrapper.CreateErrorResponse(
                                error,
                                409,
                                ErrorNumber.MachineNameAlreadyExists
                            );
                        }
                        else
                        {
                            existing.MachineName = m.MachineName;
                            logger.WriteInfo($"Machine name updated to {m.MachineName}.");
                        }
                    }
                    else
                    {
                        existing.MachineName = m.MachineName;
                    }
                    existing.MachineCost = Convert.ToDecimal(m.MachineCost);
                    existing.PlantId = m.PlantId;
                    existing.Plant = null;
                    existing.ModifiedById = m.ModifiedById;
                    existing.ModifiedDate = currentUtc;
                    existing.IsActive = m.IsActive;

                    updateList.Add(existing);
                }
                else
                {
                    if (m.CreatedById <= 0)
                    {
                        return ErrorResponseWrapper.CreateErrorResponse(
                            ErrorDescription.CreatedByIdIsRequired_1009,
                            400,
                            ErrorNumber.CreatedByIdIsRequired
                        );
                    }
                    var isMachineExist = await machineRepository.MachineNameExistsAsync(m.MachineName);
                    if (isMachineExist != null)
                    {
                        duplicateCodes.Add(m.MachineName);
                        continue; // Skip this one
                    }
                    newList.Add(new MMachine
                    {
                        MachineName = m.MachineName,
                        MachineCost = Convert.ToDecimal(m.MachineCost),
                        PlantId = m.PlantId,
                        CreatedById = m.CreatedById,
                        CreatedDate = currentUtc,
                        IsActive = true
                    });
                }
            }

            int processed = await machineRepository.BulkAddOrUpdateMachine(newList, updateList);

            if (processed > 0)
            {
                logger.WriteInfo($"Bulk add/update machines completed. {processed} records processed.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.MachineBulkAddOrUpdateSuccess);
                result.Add("Message", SuccessDescription.MachineBulkAddOrUpdateSuccess_2007);
                result.Add("ProcessedCount", processed);
                if (duplicateCodes.Any())
                {
                    result.Add("SkippedDuplicates", JArray.FromObject(duplicateCodes.Distinct()));
                    result.Add("WarningMessage", $"Skipped {duplicateCodes.Count} duplicate MachineName(s): {string.Join(", ", duplicateCodes.Distinct())}");
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.MachineBulkAddOrUpdateFailed_1021,
                    500,
                    ErrorNumber.MachineBulkAddOrUpdateFailed
                );
            }
            return result;
        }

        public async Task<JObject> BulkUpload(IFormFile file, int userId)
        {
            var result = new JObject();

            if (file == null || file.Length == 0)
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoFileUploaded_1035,
                    400,
                    ErrorNumber.NoFileUploaded
                );

            if (userId == null || userId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.CreatedByIdIsRequired_1009,
                    400,
                    ErrorNumber.CreatedByIdIsRequired
                );
            }

            List<MMachine> machines;
            try
            {
                machines = ExcelHelper.ReadMachineFromExcel(file.OpenReadStream());
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

            if (machines == null || !machines.Any())
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.InvalidOrEmptyExcelData_1036,
                    400,
                    ErrorNumber.InvalidOrEmptyExcelData
                );

            try
            {
                List<MMachine> validMachine = new List<MMachine>();
                List<string> duplicateMachine = new List<string>();

                foreach (var machine in machines)
                {
                    var existing = await machineRepository.MachineNameExistsAsync(machine.MachineName);
                    if (existing == null)
                    {
                        validMachine.Add(machine);
                    }
                    else
                    {
                        duplicateMachine.Add(machine.MachineName);
                    }
                }
                if (validMachine.Any())
                {
                    logger.WriteInfo($"Importing {validMachine.Count} machines from Excel for user {userId}.");
                    string message = await machineRepository.MachineBulkUploadAsync(machines, userId, duplicateMachine.Count);

                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.MachineBulkUploadSuccess);
                    result.Add("Message", message);
                }
                else
                {
                    result.Add("Success", false);
                    result.Add("Message", "No new machines were added. All records had duplicate machine names");
                    result.Add("DuplicateMachineNames", string.Join(", ", duplicateMachine));
                    return result;
                }
                if (duplicateMachine.Any())
                {
                    result.Add("DuplicateMachineNames", string.Join(", ", duplicateMachine));
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in MachineBulkUploadAsync: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.MachineBulkUploadFailed_1024,
                    500,
                    ErrorNumber.MachineBulkUploadFailed
                );
            }

            return result;
        }
    }
}