using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using YIPLCrimping.DAL;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.BAL.Service
{
    public class DepartmentService
    {
        private readonly DepartmentRepository departmentRepository;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public DepartmentService(DepartmentRepository departmentRepository)
        {
            this.departmentRepository = departmentRepository;
        }

        public async Task<JObject> Add(MDepartment department)
        {
            var result = new JObject();

            if (string.IsNullOrWhiteSpace(department.DeptName))
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.DepartmentNameIsRequired_1201, 400, ErrorNumber.DepartmentNameIsRequired);
            }

            if (department.CreatedById == null || department.CreatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);
            }

            bool exists = await departmentRepository.DepartmentNameIsExists(department.DeptName);
            if (exists)
            {
                string error = string.Format(ErrorDescription.DepartmentAlreadyExists_1202, department.DeptName);
                return ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.DepartmentAlreadyExists);
            }

            department.CreatedDate = DateTime.UtcNow;
            department.IsActive = true;

            var addedId = await departmentRepository.Add(department);

            if (addedId > 0)
            {
                logger.WriteInfo($"Department {addedId}, {department.DeptName} added successfully.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.DepartmentAddedSuccessfully);
                result.Add("Message", SuccessDescription.DepartmentAddedSuccessfully_2101);
            }
            else
            {
                logger.WriteInfo($"Department {department.DeptName} could not be added.");
                result.Add("Success", false);
                result.Add("Message", ErrorDescription.FailedToAddDepartment_1203);
            }

            return result;
        }

        public async Task<JObject> Update(MDepartment department)
        {
            var result = new JObject();

            if (department.Id <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.DepartmentIdIsRequired_1008, 400, ErrorNumber.DepartmentIdIsRequired);
            }
            if (department.ModifiedById == null || department.ModifiedById <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1210, 400, ErrorNumber.ModifiedByIdIsRequired);

            var existingDept = await departmentRepository.Get(department.Id);

            if (existingDept == null)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.DepartmentNotFound_1204, department.Id), 404, ErrorNumber.DepartmentNotFound);
            }

            if (!string.IsNullOrWhiteSpace(department.DeptName) &&
                !string.Equals(existingDept.DeptName.Trim(), department.DeptName.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                bool nameExists = await departmentRepository.DepartmentNameIsExists(department.DeptName.Trim());

                if (nameExists)
                {
                    return ErrorResponseWrapper.CreateErrorResponse(
                        string.Format(ErrorDescription.DepartmentAlreadyExists_1202, department.DeptName), 409, ErrorNumber.DepartmentAlreadyExists);
                }

                existingDept.DeptName = department.DeptName.Trim();
            }

            //if (existingDept.IsActive != department.IsActive)
            //    existingDept.IsActive = department.IsActive;

            existingDept.ModifiedById = department.ModifiedById;
            existingDept.ModifiedDate = DateTime.UtcNow;

            var updated = await departmentRepository.Update(existingDept);

            if (updated > 0)
            {
                logger.WriteInfo($"Department {existingDept.Id}, {existingDept.DeptName} updated successfully.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.DepartmentUpdatedSuccessfully);
                result.Add("Message", SuccessDescription.DepartmentUpdatedSuccessfully_2102);
            }
            else
            {
                logger.WriteInfo($"Department {existingDept.Id} could not be updated.");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.FailedToUpdateDepartment_1205, 500, ErrorNumber.FailedToUpdateDepartment);
            }

            return result;
        }

        public async Task<JObject> Delete(JObject departmentData)
        {
            var result = new JObject();
            var departmentId = departmentData["departmentId"]?.ToObject<int>();
            var modifiedById = departmentData["modifiedById"]?.ToObject<int>();

            // Validation
            if (departmentId == null || departmentId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.DepartmentIdIsRequired_1008, 400, ErrorNumber.DepartmentIdIsRequired);
            }

            // Fetch existing department
            var existingDepartment = await departmentRepository.Get(departmentId.Value);

            if (existingDepartment != null)
            {
                existingDepartment.IsActive = false;
                existingDepartment.ModifiedById = modifiedById;
                existingDepartment.ModifiedDate = DateTime.UtcNow;

                int updated = await departmentRepository.Update(existingDepartment);

                if (updated > 0)
                {
                    logger.WriteInfo($"Department {existingDepartment.Id} - {existingDepartment.DeptName} deleted successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.DepartmentDeletedSuccessfully);
                    result.Add("Message", SuccessDescription.DepartmentDeletedSuccessfully_2103);
                }
                else
                {
                    logger.WriteInfo($"Department {existingDepartment.Id} could not be updated.");
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToUpdateDepartment_1205, 500, ErrorNumber.FailedToUpdateDepartment);
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.DepartmentNotFound_1204, departmentId),
                    404,
                    ErrorNumber.DepartmentNotFound
                );
            }

            return result;
        }

        public async Task<List<MDepartment>> Get(JObject requestData)
        {
            int? id = requestData["DepartmentId"]?.ToObject<int>();
            string? departmentName = requestData["DepartmentName"]?.ToString();

            var departments = await departmentRepository.Get(id, departmentName);
            logger.WriteInfo($"Retrieved {departments.Count} departments from repository.");

            return departments;
        }

        public async Task<JObject> BulkAddOrUpdate(List<MDepartment> departments)
        {
            var result = new JObject();
            var skipped = new JArray();
            int addedCount = 0;
            int updatedCount = 0;

            if (departments == null || !departments.Any())
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoDepartmentRecordsProvided_1206,
                    400,
                    ErrorNumber.NoDepartmentRecordsProvided
                );

            var existingId = departments.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            var existingDepartments = await departmentRepository.Getlist(existingId);
            var newDepartments = new List<MDepartment>();

            foreach (var department in departments)
            {
                if (string.IsNullOrWhiteSpace(department.DeptName))
                {
                    skipped.Add(ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.DepartmentNameIsRequired_1201,
                        400,
                        ErrorNumber.DepartmentNameIsRequired
                    ));
                    continue;
                }

                department.DeptName = department.DeptName.Trim();

                if (department.Id > 0)
                {
                    // Update flow
                    if (department.ModifiedById == null || department.ModifiedById <= 0)
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(
                            ErrorDescription.ModifiedByIdIsRequired_1057,
                            400,
                            ErrorNumber.ModifiedByIdIsRequired
                        ));
                        continue;
                    }

                    if (existingDepartments.TryGetValue(department.Id, out var existing))
                    {
                        // ✅ Prevent update if same name exists in other department
                        bool nameConflict = await departmentRepository.DepartmentNameIsExists(department.DeptName);
                        if (nameConflict && !string.Equals(existing.DeptName, department.DeptName, StringComparison.OrdinalIgnoreCase))
                        {
                            string error = string.Format(ErrorDescription.DepartmentAlreadyExists_1202, department.DeptName);
                            skipped.Add(ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.DepartmentAlreadyExists));
                            continue;
                        }

                        existing.DeptName = department.DeptName;
                        existing.ModifiedById = department.ModifiedById;
                        existing.ModifiedDate = DateTime.UtcNow;

                        await departmentRepository.Update(existing);
                        updatedCount++;
                        logger.WriteInfo($"[BulkUpdate] Updated department: Id={existing.Id}");
                    }
                    else
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(
                            ErrorDescription.DepartmentNotFound_1204,
                            400,
                            ErrorNumber.DepartmentNotFound
                        ));
                    }
                }
                else
                {
                    // Add flow
                    if (department.CreatedById == null || department.CreatedById <= 0)
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(
                            ErrorDescription.CreatedByIdIsRequired_1009,
                            400,
                            ErrorNumber.CreatedByIdIsRequired
                        ));
                        continue;
                    }

                    bool departmentExists = await departmentRepository.DepartmentNameIsExists(department.DeptName);
                    if (departmentExists)
                    {
                        string error = string.Format(ErrorDescription.DepartmentAlreadyExists_1202, department.DeptName);
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.DepartmentAlreadyExists));
                        continue;
                    }

                    newDepartments.Add(new MDepartment
                    {
                        DeptName = department.DeptName,
                        CreatedById = department.CreatedById,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    });

                    addedCount++;
                }
            }

            if (newDepartments.Any())
            {
                await departmentRepository.BulkAddOrUpdate(newDepartments);
            }

            result.Add("Success", true);
            result.Add("SuccessNumber", SuccessNumber.DepartmentBulkAddOrUpdateSuccess);

            //string summaryMessage = $"Successfully added {addedCount} department(s), updated {updatedCount} department(s)";
            //if (skipped.Count > 0)
            //    summaryMessage += $", skipped {skipped.Count} department(s).";

            string summaryMessage = $"Bulk add/update completed. {addedCount} record(s) added, {updatedCount} record(s) updated, and {skipped.Count} duplicate or invalid record(s) skipped.";

            result.Add("Message", summaryMessage);
            result.Add("TotalAdded", addedCount);
            result.Add("TotalUpdated", updatedCount);
            result.Add("TotalSkipped", skipped.Count());

            if (skipped.Count > 0)
            {
                result.Add("SkippedDetails", skipped);
            }

            return result;
        }

        public async Task<JObject> BulkUpload(IFormFile file, int userId)
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

            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.InvalidFileFormat_1209,
                    400,
                    ErrorNumber.InvalidFileFormat
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

            var departments = ExcelHelper.ReadDepartmentFromExcel(file.OpenReadStream());

            if (departments == null || !departments.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.InvalidOrEmptyExcelData_1036,
                    400,
                    ErrorNumber.InvalidOrEmptyExcelData
                );
            }

            logger.WriteInfo($"Attempting to bulk import {departments.Count} departments from Excel for user {userId}.");

            var departmentsToImport = new List<MDepartment>();
            var skippedDepartmentNames = new List<string>();

            foreach (var dept in departments)
            {
                if (string.IsNullOrWhiteSpace(dept.DeptName))
                    continue;

                bool exists = await departmentRepository.DepartmentNameIsExists(dept.DeptName.Trim());

                if (!exists)
                {
                    dept.DeptName = dept.DeptName.Trim();
                    dept.CreatedById = userId;
                    dept.CreatedDate = DateTime.UtcNow;
                    dept.IsActive = true;
                    departmentsToImport.Add(dept);
                }
                else
                {
                    skippedDepartmentNames.Add(dept.DeptName.Trim());
                }
            }

            if (!departmentsToImport.Any())
            {
                string skippedMsg = skippedDepartmentNames.Any()
                    ? $" All departments already exist: {string.Join(", ", skippedDepartmentNames)}."
                    : " No valid departments found to import.";

                return ErrorResponseWrapper.CreateErrorResponse(
                    $"Bulk upload failed. {skippedMsg}",
                    409,
                    ErrorNumber.DepartmentBulkUploadFailed
                );
            }

            string dbMessage = await departmentRepository.BulkUpload(departmentsToImport, userId);

            if (!string.IsNullOrWhiteSpace(dbMessage) && dbMessage.Contains("success", StringComparison.OrdinalIgnoreCase))
            {
                //string successMsg = $"Successfully imported {departmentsToImport.Count} departments.";
                //if (skippedDepartmentNames.Any())
                //{
                //    successMsg += $" The following departments were skipped because they already exist: {string.Join(", ", skippedDepartmentNames)}.";
                //    logger.WriteInfo($"Skipped departments during bulk upload: {string.Join(", ", skippedDepartmentNames)}");
                //}

                int addedCount = departmentsToImport.Count;
                int duplicateCount = skippedDepartmentNames.Count;

                string successMsg = $"Successfully added {addedCount} new department(s). {duplicateCount} duplicate(s) were skipped.";

                if (duplicateCount > 0)
                {
                    logger.WriteInfo($"Skipped departments during bulk upload: {string.Join(", ", skippedDepartmentNames)}");
                }

                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.DepartmentBulkUploadSuccess);
                result.Add("Message", successMsg);
                result.Add("ImportedCount", departmentsToImport.Count);
            }
            else
            {
                logger.WriteInfo("Bulk upload failed at repository layer.");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.DepartmentBulkUploadFailed_1208,
                    500,
                    ErrorNumber.DepartmentBulkUploadFailed
                );
            }

            return result;
        }
    }
}