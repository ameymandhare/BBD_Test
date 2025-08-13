using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using YIPLCrimping.DAL;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper;
using YIPLCrimpingAPI.Models;
using Path = System.IO.Path;

namespace YIPLCrimping.BAL.Service
{
    public class ShapeService
    {
        private readonly ShapeRepository shapeRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public ShapeService(ShapeRepository shapeRepository, IWebHostEnvironment hostEnvironment)
        {
            this.shapeRepository = shapeRepository;
            _hostingEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Adds a new crimping shape to the repository and optionally uploads an associated image file.
        /// </summary>
        /// <remarks>The method performs the following operations: <list type="bullet"> <item>Validates
        /// that the shape's name is provided and does not already exist in the repository.</item> <item>If an image
        /// file is provided, it is saved to the server, and the shape's <see cref="MCrimpingShape.ImageUrl"/> is
        /// updated with the file's relative path.</item> <item>Sets audit fields such as <see
        /// cref="MCrimpingShape.CreatedDate"/> and <see cref="MCrimpingShape.IsActive"/> before saving the
        /// shape.</item> </list></remarks>
        /// <param name="shape">The crimping shape to add. The <see cref="MCrimpingShape.Name"/> property must not be null, empty, or
        /// whitespace.</param>
        /// <param name="imageFile">An optional image file to associate with the shape. If provided, the file will be saved to the server.</param>
        /// <returns>A <see cref="JObject"/> containing the result of the operation.  If successful, the object includes details
        /// of the added shape, a success flag, and a success message.  If unsuccessful, it includes an error message,
        /// error number, and a failure flag.</returns>
        public async Task<JObject> Add(MCrimpingShape shape, IFormFile? imageFile)
        {
            var result = new JObject();

            if (string.IsNullOrWhiteSpace(shape.Name))
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ShapeNameIsRequired_1151, 400, ErrorNumber.ShapeNameIsRequired);
            }

            // ✅ Validate CreatedById (must be > 0)
            if (!shape.CreatedById.HasValue || shape.CreatedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);
            }

            bool exists = shapeRepository.NameExists(shape.Name);

            if (exists)
            {
                return ErrorResponseWrapper.CreateErrorResponse(string.Format(ErrorDescription.ShapeNameAlreadyExists_1152, shape.Name), 409, ErrorNumber.ShapeNameAlreadyExists);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var folderName = "images";
                var wwwRootPath = _hostingEnvironment.WebRootPath;
                var fullPath = Path.Combine(wwwRootPath, folderName);

                // Ensure the directory exists
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                // Use original filename
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(fullPath, fileName);

                // Optionally handle duplicate filenames by appending a counter
                int counter = 1;
                string baseFileName = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);

                while (System.IO.File.Exists(filePath))
                {
                    fileName = $"{baseFileName}_{counter++}{extension}";
                    filePath = Path.Combine(fullPath, fileName);
                }

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Save the relative path to the database
                shape.ImageUrl = $"/{folderName}/{fileName}";
            }

            // Set audit fields
            shape.CreatedDate = DateTime.UtcNow;
            shape.IsActive = true;

            int added = await shapeRepository.Add(shape);
            if (added > 0)
            {
                logger.WriteInfo($"Shape {shape.Id}, '{shape.Name}' added successfully.");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.ShapeAddedSuccessfully_2051);
                result.Add("Message", SuccessDescription.ShapeAddedSuccessfully_2051);
                result.Add("Data", JObject.FromObject(shape));
            }
            else
            {
                logger.WriteError($"Failed to add shape '{shape.Name}'.");
                result.Add("Success", false);
                result.Add("Message", ErrorDescription.FailedToAddShape_1153);
                result.Add("ErrorNumber", ErrorNumber.FailedToAddShape);
            }

            return result;
        }

        /// <summary>
        /// Updates an existing crimping shape with the provided details and optionally uploads a new image.
        /// </summary>
        /// <remarks>This method performs the following actions: <list type="number">
        /// <item><description>Validates that the shape exists in the repository.</description></item>
        /// <item><description>Checks for duplicate shape names and ensures the name is not empty.</description></item>
        /// <item><description>Uploads the provided image file, if any, and updates the shape's image
        /// URL.</description></item> <item><description>Updates audit fields such as <see
        /// cref="MCrimpingShape.ModifiedDate"/> and <see cref="MCrimpingShape.ModifiedById"/>.</description></item>
        /// <item><description>Saves the changes to the repository and logs the operation.</description></item> </list>
        /// If the update fails, an appropriate error response is returned.</remarks>
        /// <param name="shape">The crimping shape object containing updated details. The <see cref="MCrimpingShape.Id"/> must correspond to
        /// an existing shape.</param>
        /// <param name="imageFile">An optional image file to associate with the shape. If provided, the image will be uploaded and its URL will
        /// be updated.</param>
        /// <returns>A <see cref="JObject"/> containing the result of the update operation. The object includes: <list
        /// type="bullet"> <item><description><c>Success</c>: A boolean indicating whether the update was
        /// successful.</description></item> <item><description><c>Message</c>: A message describing the outcome of the
        /// operation.</description></item> <item><description><c>Data</c>: The updated shape object, if the operation
        /// was successful.</description></item> <item><description><c>ErrorNumber</c>: An error code, if the operation
        /// failed.</description></item> </list></returns>
        public async Task<JObject> Update(MCrimpingShape shape, IFormFile? imageFile)
        {
            var result = new JObject();

            // Validate ID
            var existingShape = await shapeRepository.GetById(shape.Id);
            if (existingShape != null)
            {
                if (!string.IsNullOrWhiteSpace(shape.Name))
                {
                    var duplicate = shapeRepository.NameExists(shape.Name, shape.Id);

                    if (duplicate)
                    {
                        return ErrorResponseWrapper.CreateErrorResponse(
                            string.Format(ErrorDescription.ShapeNameAlreadyExists_1152, shape.Name),
                            409,
                            ErrorNumber.ShapeNameAlreadyExists);
                    }
                    existingShape.Name = shape.Name;
                }
                else
                {
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ShapeNameIsRequired_1151, 400, ErrorNumber.ShapeNameIsRequired);
                }

                // ✅ Validate ModifiedById
                if (shape.ModifiedById == null || shape.ModifiedById == 0)
                {
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.ModifiedByIdIsRequired_1057,
                        400,
                        ErrorNumber.ModifiedByIdIsRequired);
                }

                // Image Upload (if provided)
                if (imageFile != null && imageFile.Length > 0)
                {
                    var folderName = "images";
                    var wwwRootPath = _hostingEnvironment.WebRootPath;
                    var fullPath = Path.Combine(wwwRootPath, folderName);

                    if (!Directory.Exists(fullPath))
                        Directory.CreateDirectory(fullPath);

                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(fullPath, fileName);

                    int counter = 1;
                    var baseFileName = Path.GetFileNameWithoutExtension(fileName);
                    var extension = Path.GetExtension(fileName);

                    while (System.IO.File.Exists(filePath))
                    {
                        fileName = $"{baseFileName}_{counter++}{extension}";
                        filePath = Path.Combine(fullPath, fileName);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    existingShape.ImageUrl = $"/{folderName}/{fileName}";
                }

                // Set audit fields
                existingShape.ModifiedDate = DateTime.UtcNow;
                existingShape.ModifiedById = shape.ModifiedById;

                // Save changes
                var updated = await shapeRepository.Update(existingShape);

                if (updated > 0)
                {
                    logger.WriteInfo($"Shape {existingShape.Id}, '{existingShape.Name}' updated successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.ShapeUpdatedSuccessfully_2052);
                    result.Add("Message", SuccessDescription.ShapeUpdatedSuccessfully_2052);
                    result.Add("Data", JObject.FromObject(existingShape));
                }
                else
                {
                    logger.WriteError($"Failed to update shape {shape.Id}, '{shape.Name}'.");
                    result.Add("Success", false);
                    result.Add("Message", ErrorDescription.FailedToUpdateShape_1154);
                    result.Add("ErrorNumber", ErrorNumber.FailedToUpdateShape);
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ShapeNotFound_1156, 404, ErrorNumber.ShapeNotFound);
            }
            return result;
        }

        /// <summary>
        /// Retrieves a list of crimping shapes based on the specified search criteria.
        /// </summary>
        /// <param name="requestData">A <see cref="JObject"/> containing the search criteria. The following keys are supported: <list
        /// type="bullet"> <item> <description><c>name</c>: An optional string specifying the name of the crimping shape
        /// to filter by.</description> </item> <item> <description><c>id</c>: An optional integer specifying the unique
        /// identifier of the crimping shape to filter by.</description> </item> <item> <description><c>searchText</c>:
        /// An optional string for performing a general search across crimping shapes.</description> </item> </list></param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see
        /// cref="MCrimpingShape"/> objects matching the specified search criteria. If no shapes match, an empty list is
        /// returned.</returns>
        public async Task<List<MCrimpingShape>> Get(JObject requestData)
        {
            try
            {
                string? name = requestData["name"]?.ToString();
                int? id = requestData["id"]?.ToObject<int?>();
                string? searchText = requestData["searchText"]?.ToString();

                var shapes = await shapeRepository.Get(name, searchText, id);
                logger.WriteInfo($"Retrieved {shapes.Count} shapes from repository.");
                return shapes;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in ShapeService.Get: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes (deactivates) a shape identified by the provided data.
        /// </summary>
        /// <remarks>This method deactivates the shape rather than permanently deleting it. The shape's
        /// <c>IsActive</c> property is set to <see langword="false"/>,  and the <c>ModifiedById</c> and
        /// <c>ModifiedDate</c> properties are updated. If the shape does not exist, a 404 error response is
        /// returned.</remarks>
        /// <param name="shapeData">A <see cref="JObject"/> containing the shape data. Must include the following properties: <list
        /// type="bullet"> <item><description><c>id</c>: The unique identifier of the shape to delete. Must be a
        /// positive integer.</description></item> <item><description><c>modifiedById</c>: The identifier of the user
        /// performing the deletion. Must be a valid integer.</description></item> </list></param>
        /// <returns>A <see cref="JObject"/> containing the result of the operation. If successful, the object includes: <list
        /// type="bullet"> <item><description><c>Success</c>: <see langword="true"/> if the shape was successfully
        /// deleted.</description></item> <item><description><c>SuccessNumber</c>: A success code indicating the
        /// operation was completed.</description></item> <item><description><c>Message</c>: A message describing the
        /// success.</description></item> </list> If the operation fails, an error response is returned instead.</returns>
        public async Task<JObject> Delete(JObject shapeData)
        {
            var result = new JObject();
            var shapeId = shapeData["id"]?.ToObject<int>() ?? 0;
            var modifiedById = shapeData["modifiedById"]?.ToObject<int>() ?? 0;

            if (shapeId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.ShapeIdIsRequired_1157,
                    400,
                    ErrorNumber.ShapeIdIsRequired
                );
            }

            // Validate modifiedById
            if (modifiedById <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.ModifiedByIdIsRequired_1057,
                    400,
                    ErrorNumber.ModifiedByIdIsRequired
                );
            }

            var existingShape = await shapeRepository.GetById(shapeId);

            if (existingShape != null)
            {
                existingShape.IsActive = false;
                existingShape.ModifiedById = modifiedById;
                existingShape.ModifiedDate = DateTime.UtcNow;

                int updated = await shapeRepository.Update(existingShape);

                if (updated > 0)
                {
                    logger.WriteInfo($"Shape {shapeId} deleted (deactivated) successfully.");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.ShapeDeletedSuccessfully_2053);
                    result.Add("Message", SuccessDescription.ShapeDeletedSuccessfully_2053);
                }
                else
                {
                    logger.WriteError($"Failed to update shape {shapeId} during delete.");
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.FailedToDeleteShape_1155,
                        500,
                        ErrorNumber.FailedToDeleteShape
                    );
                }
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    string.Format(ErrorDescription.ShapeNotFound_1156, shapeId),
                    404,
                    ErrorNumber.ShapeNotFound
                );
            }

            return result;
        }

        /// <summary>
        /// Adds or updates a collection of crimping shapes in bulk.
        /// </summary>
        /// <remarks>This method processes the provided shapes in bulk, either adding new records or
        /// updating existing ones. If the operation fails, an error response is returned with details about the
        /// failure.</remarks>
        /// <param name="shapes">A list of <see cref="MCrimpingShape"/> objects to be added or updated. Each shape must have a valid name.</param>
        /// <returns>A <see cref="JObject"/> containing the result of the operation. The object includes the following keys:
        /// <list type="bullet"> <item> <description><c>Success</c>: A <see langword="true"/> value if the operation was
        /// successful; otherwise, an error response is returned.</description> </item> <item>
        /// <description><c>SuccessNumber</c>: A success code indicating the operation completed
        /// successfully.</description> </item> <item> <description><c>Message</c>: A message describing the outcome of
        /// the operation.</description> </item> <item> <description><c>ProcessedCount</c>: The number of shape records
        /// that were successfully processed.</description> </item> </list></returns>
        public async Task<JObject> BulkAddOrUpdate(List<MCrimpingShape> shapes)
        {
            var result = new JObject();

            if (shapes == null || !shapes.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoShapeRecordsProvided_1160,
                    400,
                    ErrorNumber.NoShapeRecordsProvided
                );
            }

            var duplicateNames = new List<string>();
            var validShapes = new List<MCrimpingShape>();

            int addCount = 0;
            int updateCount = 0;

            foreach (var shape in shapes)
            {
                if (string.IsNullOrWhiteSpace(shape.Name))
                {
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.ShapeNameIsRequired_1151,
                        400,
                        ErrorNumber.ShapeNameIsRequired
                    );
                }

                if (shape.Id > 0)
                {
                    if (shape.ModifiedById == null || shape.ModifiedById <= 0)
                    {
                        return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.ModifiedByIdIsRequired_1057,
                        400,
                        ErrorNumber.ModifiedByIdIsRequired
                        );
                    }
                }
                else
                {
                    if (shape.CreatedById == null || shape.CreatedById <= 0)
                    {
                        return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.CreatedByIdIsRequired_1009,
                        400,
                        ErrorNumber.CreatedByIdIsRequired
                        );
                    }
                }

                bool isDuplicate = shapeRepository.NameExists(shape.Name, shape.Id);
                if (isDuplicate)
                {
                    duplicateNames.Add(shape.Name);
                    continue; // skip adding this shape
                }

                if (shape.Id > 0)
                    updateCount++;
                else
                    addCount++;

                shape.ImageUrl = null;
                validShapes.Add(shape);
            }

            int duplicateCount = duplicateNames.Count;

            if (!validShapes.Any())
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    $"All {duplicateCount} shape(s) already exist. No new or updated shapes were processed.",
                    409,
                    ErrorNumber.ShapeNameAlreadyExists
                );
            }

            int processed = await shapeRepository.BulkAddOrUpdate(validShapes);

            if (processed > 0)
            {
                string message = $"Bulk add/update completed. {addCount} record(s) added, {updateCount} record(s) updated, and {duplicateCount} duplicate record(s) skipped.";

                logger.WriteInfo(message);

                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.ShapeBulkAddOrUpdateSuccess_2054);
                result.Add("Message", message);
                result.Add("AddedCount", addCount);
                result.Add("UpdatedCount", updateCount);
                result.Add("DuplicateCount", duplicateCount);
            }
            else
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.ShapeBulkAddOrUpdateFailed_1161,
                    500,
                    ErrorNumber.ShapeBulkAddOrUpdateFailed
                );
            }

            return result;
        }

        /// <summary>
        /// Processes a bulk upload of shapes from an Excel file and saves them to the database.
        /// </summary>
        /// <remarks>This method reads shape data from the provided Excel file, validates the data, and
        /// saves it to the database. Each shape is associated with the user performing the upload. If the file is
        /// invalid or the upload fails, an error response is returned.</remarks>
        /// <param name="file">The Excel file containing shape data. The file must not be null or empty.</param>
        /// <param name="userId">The ID of the user performing the upload. This is used to set audit fields for the shapes.</param>
        /// <returns>A <see cref="JObject"/> containing the result of the operation. The object includes the following keys:
        /// <list type="bullet"> <item><description><c>Success</c>: A <see langword="true"/> value if the upload was
        /// successful; otherwise, <see langword="false"/>.</description></item> <item><description><c>Message</c>: A
        /// message describing the outcome of the operation.</description></item>
        /// <item><description><c>ImportedCount</c>: The number of shapes successfully imported, if the operation was
        /// successful.</description></item> </list> If the operation fails, the returned object contains error details.</returns>
        public async Task<JObject> BulkUpload(IFormFile file, int userId)
        {
            var result = new JObject();

            // ✅ Validate userId
            if (userId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.CreatedByIdIsRequired_1009,
                    400,
                    ErrorNumber.CreatedByIdIsRequired
                );
            }

            if (file == null || file.Length == 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoFileUploadedForShape_1162,
                    400,
                    ErrorNumber.NoFileUploadedForShape
                );
            }

            List<MCrimpingShape> shapes;
            try
            {
                shapes = ExcelHelper.ReadShapeFromExcel(file.OpenReadStream());
                if (shapes == null || !shapes.Any())
                {
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.InvalidOrEmptyExcelDataForShape_1163,
                        400,
                        ErrorNumber.InvalidOrEmptyExcelDataForShape
                    );
                }

                // Set audit fields
                foreach (var shape in shapes)
                {
                    shape.ImageUrl = null; // Ignore image
                    shape.CreatedById = userId;
                    shape.CreatedDate = DateTime.UtcNow;
                    shape.IsActive = true;
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error reading Excel file: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.InvalidOrEmptyExcelDataForShape_1163,
                    400,
                    ErrorNumber.InvalidOrEmptyExcelDataForShape
                );
            }

            try
            {
                logger.WriteInfo($"Importing {shapes.Count} shapes from Excel for user {userId}.");

                string message = await shapeRepository.BulkUpload(shapes, userId);

                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.ShapeBulkUploadSuccess_2055);
                result.Add("Message", message);
                result.Add("ImportedCount", shapes.Count);
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in ShapeBulkUpload: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.ShapeBulkUploadFailed_1164,
                    500,
                    ErrorNumber.ShapeBulkUploadFailed
                );
            }
            return result;
        }
    }
}