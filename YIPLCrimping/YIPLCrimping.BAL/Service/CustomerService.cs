using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using YIPLCrimping.DAL;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.BAL.Service
{
    /// <summary>
    /// Service layer for handling business logic related to customer operations.
    /// </summary>
    public class CustomerService
    {
        private readonly CustomerRepository customerRepository;

        // Singleton logger instance for consistent logging
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="customerRepository">Repository for customer data access.</param>
        public CustomerService(CustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        /// <summary>
        /// Retrieves customers filtered by optional ID and/or search text.
        /// </summary>
        /// <param name="requestData">JSON object with 'id' and 'searchText'.</param>
        /// <returns>List of matching customers.</returns>
        public async Task<List<MCustomer>> Get(JObject requestData)
        {
            try
            {
                int? id = requestData["id"]?.ToObject<int>();
                string? searchText = requestData["searchText"]?.ToString();

                var customers = await customerRepository.Get(id, searchText);
                logger.WriteInfo($"[Get] Retrieved {customers.Count} customer(s).");
                return customers;
            }
            catch (Exception ex)
            {
                logger.WriteError($"[Get] Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds a new customer after validation.
        /// </summary>
        /// <param name="customer">Customer data to add.</param>
        /// <returns>Success or error response as JObject.</returns>
        public async Task<JObject> Add(MCustomer customer)
        {
            var result = new JObject();

            if (string.IsNullOrWhiteSpace(customer.CustomerName))
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CustomerNameIsRequired_1050, 400, ErrorNumber.CustomerNameIsRequired);

            if (string.IsNullOrWhiteSpace(customer.CustomerCode))
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CustomerCodeIsRequired_1051, 400, ErrorNumber.CustomerCodeIsRequired);

            if (customer.CreatedById == null || customer.CreatedById <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);

            var isCustomerCodeExist = await customerRepository.GetByCode(customer.CustomerCode);
            if (isCustomerCodeExist != null)
            {
                string error = string.Format(ErrorDescription.CustomerCodeAlreadyExists_1060, customer.CustomerCode);
                return ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.CustomerCodeAlreadyExists);
            }
            customer.CreatedDate = DateTime.UtcNow;
            customer.IsActive = true;

            int addCustomer = await customerRepository.Add(customer);

            if (addCustomer > 0)
            {
                logger.WriteInfo($"[Add] Customer added: Id={customer.Id}, Name={customer.CustomerName}");
                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.CustomerAddedSuccessfully);
                result.Add("Message", SuccessDescription.CustomerAddedSuccessfully_2050);
            }
            else
            {
                logger.WriteInfo($"[Add] Failed to add customer: Name={customer.CustomerName}");
                result.Add("Success", false);
                result.Add("Message", ErrorDescription.FailedToAddCustomer_1053);
            }

            return result;
        }

        /// <summary>
        /// Updates an existing customer's data.
        /// </summary>
        /// <param name="customer">Customer data with updated fields.</param>
        /// <returns>Success or error response as JObject.</returns>
        public async Task<JObject> Update(MCustomer customer)
        {
            var result = new JObject();

            if (customer.Id <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CustomerIdIsRequired_1052, 400, ErrorNumber.CustomerIdIsRequired);

            if (customer.ModifiedById == null || customer.ModifiedById <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired);
            if (!string.IsNullOrWhiteSpace(customer.CustomerCode))
            {
                var isCustomerCodeExist = await customerRepository.GetByCode(customer.CustomerCode);
                if (isCustomerCodeExist != null && isCustomerCodeExist.Id != customer.Id)
                {
                    string error = string.Format(ErrorDescription.CustomerCodeAlreadyExists_1060, customer.CustomerCode);
                    return ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.CustomerCodeAlreadyExists);
                }
            }
            var existingCustomer = await customerRepository.Get(customer.Id);

            if (existingCustomer != null)
            {
                if (!string.IsNullOrWhiteSpace(customer.CustomerName))
                    existingCustomer.CustomerName = customer.CustomerName;

                if (!string.IsNullOrWhiteSpace(customer.CustomerCode))
                    existingCustomer.CustomerCode = customer.CustomerCode;

                existingCustomer.ModifiedById = customer.ModifiedById;
                existingCustomer.ModifiedDate = DateTime.UtcNow;

                int updated = await customerRepository.Update(existingCustomer);

                if (updated > 0)
                {
                    logger.WriteInfo($"[Update] Customer updated: Id={existingCustomer.Id}");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.CustomerUpdatedSuccessfully);
                    result.Add("Message", SuccessDescription.CustomerUpdatedSuccessfully_2051);
                }
                else
                {
                    logger.WriteInfo($"[Update] Failed to update customer: Id={existingCustomer.Id}");
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToUpdateCustomer_1054, 500, ErrorNumber.FailedToUpdateCustomer);
                }
            }
            else
            {
                logger.WriteInfo($"[Update] Customer not found: Id={customer.Id}");
                return ErrorResponseWrapper.CreateErrorResponse(string.Format(ErrorDescription.CustomerNotFound_1055, customer.Id), 404, ErrorNumber.CustomerNotFound);
            }

            return result;
        }

        /// <summary>
        /// Soft-deletes a customer by marking as inactive.
        /// </summary>
        /// <param name="customerData">JSON object with 'id' and 'modifiedById'.</param>
        /// <returns>Success or error response as JObject.</returns>
        public async Task<JObject> Delete(JObject customerData)
        {
            var result = new JObject();
            var customerId = Convert.ToInt32(customerData["id"]?.ToString());
            var modifiedById = Convert.ToInt32(customerData["modifiedById"]?.ToString());

            if (customerId <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CustomerIdIsRequired_1052, 400, ErrorNumber.CustomerIdIsRequired);

            if (modifiedById == null || modifiedById <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired);

            var existingCustomer = await customerRepository.Get(customerId);

            if (existingCustomer != null)
            {
                existingCustomer.IsActive = false;
                existingCustomer.ModifiedById = modifiedById;
                existingCustomer.ModifiedDate = DateTime.UtcNow;

                int updated = await customerRepository.Update(existingCustomer);

                if (updated > 0)
                {
                    logger.WriteInfo($"[Delete] Customer deleted (soft): Id={existingCustomer.Id}");
                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.CustomerDeletedSuccessfully);
                    result.Add("Message", SuccessDescription.CustomerDeletedSuccessfully_2052);
                }
                else
                {
                    logger.WriteInfo($"[Delete] Failed to delete customer: Id={existingCustomer.Id}");
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToDeleteCustomer_1056, 500, ErrorNumber.FailedToDeleteCustomer);
                }
            }
            else
            {
                logger.WriteInfo($"[Delete] Customer not found: Id={customerId}");
                return ErrorResponseWrapper.CreateErrorResponse(string.Format(ErrorDescription.CustomerNotFound_1055, customerId), 404, ErrorNumber.CustomerNotFound);
            }

            return result;
        }

        /// <summary>
        /// Adds or updates a batch of customers.
        /// </summary>
        /// <param name="customers">List of customers to process.</param>
        /// <returns>Result object with success and skipped records.</returns>
        public async Task<JObject> BulkAddOrUpdate(List<MCustomer> customers)
        {
            var result = new JObject();
            var skipped = new JArray();
            int processed = 0;
            int newAddedCount = 0;
            int updatedCount = 0;

            if (customers == null || !customers.Any())
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoCustomerRecordsProvided_1058,
                    400,
                    ErrorNumber.NoCustomerRecordsProvided
                );

            var existingId = customers.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            var existingCustomers = await customerRepository.Getlist(existingId);
            var newCustomers = new List<MCustomer>();

            foreach (var customer in customers)
            {
                bool isValid = true;

                if (string.IsNullOrWhiteSpace(customer.CustomerName))
                {
                    skipped.Add(ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.CustomerNameIsRequired_1050, 400, ErrorNumber.CustomerNameIsRequired));
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(customer.CustomerCode))
                {
                    skipped.Add(ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.CustomerCodeIsRequired_1051, 400, ErrorNumber.CustomerCodeIsRequired));
                    isValid = false;
                }

                if (!isValid) continue;

                if (customer.Id > 0)
                {
                    if (customer.ModifiedById == null || customer.ModifiedById <= 0)
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(
                            ErrorDescription.ModifiedByIdIsRequired_1057, 400, ErrorNumber.ModifiedByIdIsRequired));
                        continue;
                    }

                    var isCustomerCodeExist = await customerRepository.GetByCode(customer.CustomerCode);
                    if (isCustomerCodeExist != null && isCustomerCodeExist.Id != customer.Id)
                    {
                        string error = string.Format(ErrorDescription.CustomerCodeAlreadyExists_1060, customer.CustomerCode);
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.CustomerCodeAlreadyExists));
                        continue;
                    }

                    if (existingCustomers.TryGetValue(customer.Id, out var existing))
                    {
                        existing.CustomerName = customer.CustomerName.Trim();
                        existing.CustomerCode = customer.CustomerCode.Trim();
                        existing.ModifiedById = customer.ModifiedById;
                        existing.ModifiedDate = DateTime.UtcNow;

                        await customerRepository.Update(existing);
                        updatedCount++;
                        processed++;

                        logger.WriteInfo($"[BulkUpdate] Updated customer: Id={existing.Id}");
                    }
                    else
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(
                            ErrorDescription.CustomerNotFound_1055, 400, ErrorNumber.CustomerNotFound));
                    }
                }
                else
                {
                    if (customer.CreatedById == null || customer.CreatedById <= 0)
                    {
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(
                            ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired));
                        continue;
                    }

                    var isCustomerCodeExist = await customerRepository.GetByCode(customer.CustomerCode);
                    if (isCustomerCodeExist != null)
                    {
                        string error = string.Format(ErrorDescription.CustomerCodeAlreadyExists_1060, customer.CustomerCode);
                        skipped.Add(ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.CustomerCodeAlreadyExists));
                        continue;
                    }

                    newCustomers.Add(new MCustomer
                    {
                        CustomerName = customer.CustomerName.Trim(),
                        CustomerCode = customer.CustomerCode.Trim(),
                        CreatedById = customer.CreatedById,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    });

                    newAddedCount++;
                    processed++;
                }
            }

            if (newCustomers.Any())
            {
                await customerRepository.BulkAdd(newCustomers);
                logger.WriteInfo($"[BulkAdd] Inserted {newAddedCount} new customers.");
            }

            // Summary Message with singular/plural handling
            int skippedCount = skipped.Count;

            string summaryMessage = $"Bulk add/update completed. " +
                $"{newAddedCount} {(newAddedCount == 1 ? "record" : "records")} added, " +
                $"{updatedCount} {(updatedCount == 1 ? "record" : "records")} updated, and " +
                $"{skippedCount} {(skippedCount == 1 ? "record" : "records")} skipped.";

            result.Add("Success", true);
            result.Add("SuccessNumber", SuccessNumber.CustomerBulkAddOrUpdateSuccessfully);
            result.Add("Message", summaryMessage);
            result.Add("TotalProcessed", processed);
            result.Add("TotalSkipped", skippedCount);

            if (skippedCount > 0)
                result.Add("SkippedDetails", skipped);

            return result;
        }

        /// <summary>
        /// Uploads customers in bulk via Excel file.
        /// </summary>
        /// <param name="file">Excel file</param>
        /// <param name="userId">User performing the upload</param>
        /// <returns>Upload result response</returns>
        public async Task<JObject> BulkUpload(IFormFile file, int userId)
        {
            var result = new JObject();
            var skipped = new JArray();
            var validCustomers = new List<MCustomer>();

            if (file == null || file.Length == 0)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.NoFileUploaded_1035, 400, ErrorNumber.NoFileUploaded);

            if (userId <= 0 || userId == null)
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CreatedByIdIsRequired_1009, 400, ErrorNumber.CreatedByIdIsRequired);

            List<MCustomer> customers;
            try
            {
                customers = ExcelHelper.ReadCustomerFromExcel(file.OpenReadStream());
                logger.WriteInfo($"[BulkUpload] Read {customers.Count} customers from Excel.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"[BulkUpload] Error reading Excel: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.InvalidOrEmptyExcelData_1036, 400, ErrorNumber.InvalidOrEmptyExcelData);
            }

            if (customers == null || !customers.Any())
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.InvalidOrEmptyExcelData_1036, 400, ErrorNumber.InvalidOrEmptyExcelData);

            foreach (var customer in customers)
            {
                customer.CustomerName = customer.CustomerName?.Trim();
                customer.CustomerCode = customer.CustomerCode?.Trim();

                // Validate CustomerName
                if (string.IsNullOrWhiteSpace(customer.CustomerName))
                {
                    skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CustomerNameIsRequired_1050, 400, ErrorNumber.CustomerNameIsRequired));
                    continue;
                }

                // Validate CustomerCode
                if (string.IsNullOrWhiteSpace(customer.CustomerCode))
                {
                    skipped.Add(ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CustomerCodeIsRequired_1051, 400, ErrorNumber.CustomerCodeIsRequired));
                    continue;
                }

                // Check if customer code already exists
                var isCustomerCodeExist = await customerRepository.GetByCode(customer.CustomerCode);
                if (isCustomerCodeExist != null)
                {
                    string error = string.Format(ErrorDescription.CustomerCodeAlreadyExists_1060, customer.CustomerCode);
                    skipped.Add(ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.CustomerCodeAlreadyExists));
                    continue;
                }

                // Valid customer
                customer.CreatedById = userId;
                customer.CreatedDate = DateTime.UtcNow;
                customer.IsActive = true;

                validCustomers.Add(customer);
            }

            int addedCount = validCustomers.Count;
            int skippedCount = skipped.Count;

            // No valid records to insert
            if (addedCount == 0)
            {
                result.Add("Success", false);
                result.Add("Message", "No valid customers to import.");
                result.Add("TotalSkipped", skippedCount);
                result.Add("SkippedDetails", skipped);
                return result;
            }

            try
            {
                await customerRepository.BulkInsertCustomers(validCustomers);

                string message = $"Bulk upload completed. " +
                    $"{addedCount} {(addedCount == 1 ? "record" : "records")} added, " +
                    $"{skippedCount} {(skippedCount == 1 ? "record" : "records")} skipped.";

                result.Add("Success", true);
                result.Add("SuccessNumber", SuccessNumber.CustomerBulkUploadSuccess);
                result.Add("Message", message);
                result.Add("ImportedCount", addedCount);
                result.Add("TotalSkipped", skippedCount);

                if (skippedCount > 0)
                {
                    result.Add("SkippedDetails", skipped);
                }

                logger.WriteInfo($"[BulkUpload] Successfully uploaded {addedCount} customers by user {userId}.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"[BulkUpload] Upload failed: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.CustomerBulkUploadFailed_1059, 500, ErrorNumber.CustomerBulkUploadFailed);
            }

            return result;
        }
    }
}