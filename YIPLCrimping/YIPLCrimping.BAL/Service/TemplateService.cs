using Newtonsoft.Json.Linq;
using YIPLCrimping.DAL;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.BAL.Service
{
    public class TemplateService
    {
        private readonly TemplateRepository templateRepository;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public TemplateService(TemplateRepository templateRepository)
        {
            this.templateRepository = templateRepository;
        }

        public async Task<JObject> Add(MTemplateFileDto dto, string rootPath)
        {
            var result = new JObject();

            var allowedMasterTypes = new[] {
                                    "CrimpingShapes", "Customer", "Department", "Machine",
                                    "Plant", "Supplier", "WireSize", "WireType"
                                };

            if (string.IsNullOrWhiteSpace(dto.MasterName) || !allowedMasterTypes.Contains(dto.MasterName, StringComparer.OrdinalIgnoreCase))
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.MasterNameIsInvalid_1039, 400, ErrorNumber.MasterNameIsInvalid);
            }

            var isMasterExist = await templateRepository.GetFileByMasterName(dto.MasterName);
            if (isMasterExist == null)
            {
                // Validation
                if (dto.File == null || dto.File.Length == 0)
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FileIsRequired_1038, 400, ErrorNumber.FileIsRequired);

                if (dto.CreatedBy == null || dto.CreatedBy <= 0)
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIsRequired_1040, 400, ErrorNumber.CreatedByIsRequired);

                string rawExtension = Path.GetExtension(dto.File.FileName)?.TrimStart('.').ToLower() ?? "unknown";
                string fileType = ExcelHelper.GetFriendlyFileType(rawExtension);

                // Folder path
                string dateFolder = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");
                string relativePath = Path.Combine(dto.MasterName, dateFolder);
                string fullPath = Path.Combine(rootPath, relativePath);

                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                string fileName = Path.GetFileName(dto.File.FileName);
                string finalPath = Path.Combine(fullPath, fileName);

                using (var stream = new FileStream(finalPath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                var entity = new MTemplateFile
                {
                    MasterName = $"M_{dto.MasterName}",
                    FileType = fileType,
                    FilePath = Path.Combine(relativePath, fileName).Replace("\\", "/"),
                    CreatedBy = dto.CreatedBy,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true
                };

                int isAdded = await templateRepository.Add(entity);

                if (isAdded > 0)
                {
                    logger.WriteInfo($"Template {fileName} uploaded successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.TemplateAddedSuccessfully);
                    result.Add("Message", SuccessDescription.TemplateAddedSuccessfully_2014);
                }
                else
                {
                    result.Add("Success", false);
                    result.Add("Message", ErrorDescription.FailedToAddTemplate_1043);
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.TemplateAlreadyExists_1041, 400, ErrorNumber.TemplateAlreadyExists);
            }

            return result;
        }

        public async Task<JObject> Update(MTemplateFileDto dto, string rootPath)
        {
            var result = new JObject();

            if (dto.Id == null || dto.Id <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.IdIsRequiredForUpdate_1045, 400, ErrorNumber.IdIsRequiredForUpdate);
            if (dto.CreatedBy == null || dto.CreatedBy <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIsRequired_1040, 400, ErrorNumber.CreatedByIsRequired);
            var existing = await templateRepository.GetById(dto.Id.Value);
            if (existing == null)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.TemplateNotFound_1042, 404, ErrorNumber.TemplateNotFound);

            if (dto.File == null || dto.File.Length == 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FileIsRequired_1038, 400, ErrorNumber.FileIsRequired);

            string rawExtension = Path.GetExtension(dto.File.FileName)?.TrimStart('.').ToLower() ?? "unknown";
            string fileType = ExcelHelper.GetFriendlyFileType(rawExtension);
            string dateFolder = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");
            string relativePath = Path.Combine(dto.MasterName, dateFolder);
            string fullPath = Path.Combine(rootPath, relativePath);

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            string fileName = Path.GetFileName(dto.File.FileName);
            string finalPath = Path.Combine(fullPath, fileName);

            using (var stream = new FileStream(finalPath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            existing.MasterName = $"M_{dto.MasterName}";
            existing.FileType = fileType;
            existing.FilePath = Path.Combine(relativePath, fileName).Replace("\\", "/");
            existing.UpdatedBy = dto.CreatedBy;
            existing.UpdatedOn = DateTime.UtcNow;

            int updated = await templateRepository.Update(existing);

            if (updated > 0)
            {
                logger.WriteInfo($"Template {existing.Id} updated.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.TemplateUpdatedSuccessfully);
                result.Add("Message", SuccessDescription.TemplateUpdatedSuccessfully_2015);
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToUpdateTemplate_1044, 500, ErrorNumber.FailedToUpdateTemplate);
            }

            return result;
        }

        public async Task<JObject> Delete(int id, int modifiedBy)
        {
            var result = new JObject();
            if (id <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.IdIsRequiredForUpdate_1045, 400, ErrorNumber.IdIsRequiredForUpdate);
            }
            if (modifiedBy == null || modifiedBy <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired);
            }

            var existing = await templateRepository.GetById(id);
            if (existing != null)
            {
                existing.IsActive = false;
                existing.UpdatedBy = modifiedBy;
                existing.UpdatedOn = DateTime.UtcNow;
                int deleted = await templateRepository.Update(existing);
                if (deleted > 0)
                {
                    logger.WriteInfo($"Template {id} deleted successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.TemplateDeletedSuccessfully);
                    result.Add("Message", SuccessDescription.TemplateDeletedSuccessfully_2016);
                }
                else
                {
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToDeleteTemplate_1046, 500, ErrorNumber.FailedToDeleteTemplate);
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.TemplateNotFound_1042, 404, ErrorNumber.TemplateNotFound);
            }
            return result;
        }

        public async Task<JObject> Get(string masterName, string rootPath)
        {
            var result = new JObject();

            if (string.IsNullOrWhiteSpace(masterName))
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.MasterNameIsInvalid_1039, 400, ErrorNumber.MasterNameIsInvalid);
            }

            var entity = await templateRepository.GetFileByMasterName(masterName);
            if (entity == null)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.TemplateNotFound_1042, 404, ErrorNumber.TemplateNotFound);
            }

            string fullFilePath = Path.Combine(rootPath, entity.FilePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (!System.IO.File.Exists(fullFilePath))
            {
                return ErrorResponseWrapper.CreateErrorResponse("File not found on disk.", 404, 1047);
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(fullFilePath);
            string base64 = Convert.ToBase64String(fileBytes);
            string fileName = Path.GetFileName(fullFilePath);

            result.Add("Success", true);
            result.Add("FileName", fileName);
            result.Add("Base64", base64);

            return result;
        }
    }
}