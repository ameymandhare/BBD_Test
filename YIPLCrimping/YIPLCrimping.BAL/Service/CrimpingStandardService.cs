using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YIPLCrimping.DAL;
using YIPLCrimping.DAL.Models;
using YIPLCrimping.DAL.Repository;
using YIPLCrimping.Helper;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.BAL.Service
{
    public class CrimpingStandardService
    {
        private readonly CrimpingStandardRepository _crimpingStandardRepository;
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public CrimpingStandardService(CrimpingStandardRepository crimpingStandardRepository)
        {
            this._crimpingStandardRepository = crimpingStandardRepository;
        }

        //public async Task<JObject> BulkAddUpdateTerminals(List<CrimpingStandardRequestVM> bulkData)
        //{
        //    var stopwatch = Stopwatch.StartNew();
        //    var result = new JObject();
        //    var successCount = 0;
        //    var errorCount = 0;
        //    var errorDetails = new List<JObject>();

        //    if (bulkData == null || bulkData.Count == 0)
        //    {
        //        return ErrorResponseWrapper.CreateErrorResponse(
        //            ErrorDescription.BulkDataIsRequired_1613,
        //            400,
        //            ErrorNumber.BulkDataIsRequired);
        //    }

        //    // Process in batches for better performance
        //    const int batchSize = 50;
        //    var batches = bulkData
        //        .Select((x, i) => new { Index = i, Value = x })
        //        .GroupBy(x => x.Index / batchSize)
        //        .Select(g => g.Select(x => x.Value).ToList());

        //    foreach (var batch in batches)
        //    {
        //        var batchTasks = batch.Select(async terminalData =>
        //        {
        //            try
        //            {
        //                // Validate required fields
        //                if (string.IsNullOrWhiteSpace(terminalData.Customer) ||
        //                    string.IsNullOrWhiteSpace(terminalData.TerminalNo))
        //                {
        //                    var error = CreateBulkErrorRecord(terminalData,
        //                        string.IsNullOrWhiteSpace(terminalData.Customer)
        //                            ? ErrorDescription.CustomerIsRequired_1601
        //                            : ErrorDescription.TerminalNoIsRequired_1604,
        //                        string.IsNullOrWhiteSpace(terminalData.Customer)
        //                            ? ErrorNumber.CustomerIsRequired
        //                            : ErrorNumber.TerminalNoIsRequired);

        //                    lock (errorDetails) { errorDetails.Add(error); }
        //                    Interlocked.Increment(ref errorCount);
        //                    return;
        //                }

        //                bool success;
        //                if (terminalData.Id == 0)
        //                {
        //                    // Use existing SaveTerminalData for new records
        //                    var terminalId = await SaveTerminalData(terminalData);
        //                    success = terminalId > 0;
        //                }
        //                else
        //                {
        //                    // Use existing UpdateTerminalData for updates
        //                    success = await UpdateTerminalData(terminalData);
        //                }

        //                if (success)
        //                {
        //                    Interlocked.Increment(ref successCount);
        //                    logger.WriteInfo($"[BulkAddUpdate] Processed terminal: Id={terminalData.Id}, TerminalNo={terminalData.TerminalNo}");
        //                }
        //                else
        //                {
        //                    var error = CreateBulkErrorRecord(terminalData,
        //                        ErrorDescription.FailedToProcessTerminal_1614,
        //                        ErrorNumber.FailedToProcessTerminal);

        //                    lock (errorDetails) { errorDetails.Add(error); }
        //                    Interlocked.Increment(ref errorCount);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.WriteError($"[BulkAddUpdate] Error processing terminal {terminalData.Id}: {ex.Message}");
        //                var error = CreateBulkErrorRecord(terminalData,
        //                    ex.Message,
        //                    ErrorNumber.FailedToProcessTerminal);

        //                lock (errorDetails) { errorDetails.Add(error); }
        //                Interlocked.Increment(ref errorCount);
        //            }
        //        });

        //        await Task.WhenAll(batchTasks);
        //    }

        //    // Prepare result
        //    result.Add("Success", errorCount == 0);
        //    result.Add("TotalRecords", bulkData.Count);
        //    result.Add("SuccessCount", successCount);
        //    result.Add("ErrorCount", errorCount);
        //    result.Add("ProcessingTimeMs", stopwatch.ElapsedMilliseconds);

        //    if (errorCount > 0)
        //    {
        //        result.Add("ErrorDetails", JArray.FromObject(errorDetails));
        //    }

        //    return result;
        //}
        //public async Task<JObject> BulkAddUpdateTerminals(List<CrimpingStandardRequestVM> bulkData)
        //{
        //    var stopwatch = Stopwatch.StartNew();
        //    var result = new JObject();
        //    var successCount = 0;
        //    var errorCount = 0;
        //    var errorDetails = new List<JObject>();

        //    if (bulkData == null || bulkData.Count == 0)
        //    {
        //        return ErrorResponseWrapper.CreateErrorResponse(
        //            ErrorDescription.BulkDataIsRequired_1613,
        //            400,
        //            ErrorNumber.BulkDataIsRequired);
        //    }

        //    const int batchSize = 50;
        //    var batches = bulkData
        //        .Select((x, i) => new { Index = i, Value = x })
        //        .GroupBy(x => x.Index / batchSize)
        //        .Select(g => g.Select(x => x.Value).ToList());

        //    foreach (var batch in batches)
        //    {
        //        foreach (var terminalData in batch)
        //        {
        //            try
        //            {
        //                if (string.IsNullOrWhiteSpace(terminalData.Customer) ||
        //                    string.IsNullOrWhiteSpace(terminalData.TerminalNo))
        //                {
        //                    var error = CreateBulkErrorRecord(terminalData,
        //                        string.IsNullOrWhiteSpace(terminalData.Customer)
        //                            ? ErrorDescription.CustomerIsRequired_1601
        //                            : ErrorDescription.TerminalNoIsRequired_1604,
        //                        string.IsNullOrWhiteSpace(terminalData.Customer)
        //                            ? ErrorNumber.CustomerIsRequired
        //                            : ErrorNumber.TerminalNoIsRequired);

        //                    errorDetails.Add(error);
        //                    errorCount++;
        //                    continue;
        //                }

        //                bool success;
        //                if (terminalData.Id == 0)
        //                {
        //                    var terminalId = await SaveTerminalData(terminalData);
        //                    success = terminalId > 0;
        //                }
        //                else
        //                {
        //                    success = await UpdateTerminalData(terminalData);
        //                }

        //                if (success)
        //                {
        //                    successCount++;
        //                    logger.WriteInfo($"[BulkAddUpdate] Processed terminal: Id={terminalData.Id}, TerminalNo={terminalData.TerminalNo}");
        //                }
        //                else
        //                {
        //                    var error = CreateBulkErrorRecord(terminalData,
        //                        ErrorDescription.FailedToProcessTerminal_1614,
        //                        ErrorNumber.FailedToProcessTerminal);

        //                    errorDetails.Add(error);
        //                    errorCount++;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.WriteError($"[BulkAddUpdate] Error processing terminal {terminalData.Id}: {ex.Message}");
        //                var error = CreateBulkErrorRecord(terminalData,
        //                    ex.Message,
        //                    ErrorNumber.FailedToProcessTerminal);

        //                errorDetails.Add(error);
        //                errorCount++;
        //            }
        //        }
        //    }

        //    result.Add("Success", errorCount == 0);
        //    result.Add("TotalRecords", bulkData.Count);
        //    result.Add("SuccessCount", successCount);
        //    result.Add("ErrorCount", errorCount);
        //    result.Add("ProcessingTimeMs", stopwatch.ElapsedMilliseconds);

        //    if (errorCount > 0)
        //    {
        //        result.Add("ErrorDetails", JArray.FromObject(errorDetails));
        //    }

        //    return result;
        //}
        public async Task<JObject> BulkAddUpdateTerminals(List<CrimpingStandardRequestVM> bulkData)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new JObject();
            var successCount = 0;
            var errorCount = 0;
            var addedRecordCount = 0;
            var updatedRecordCount = 0;
            var errorDetails = new List<JObject>();
            var skipped = new List<CrimpingStandardRequestVM>(); // optional: for duplicates

            if (bulkData == null || bulkData.Count == 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.BulkDataIsRequired_1613,
                    400,
                    ErrorNumber.BulkDataIsRequired);
            }

            const int batchSize = 50;
            var batches = bulkData
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / batchSize)
                .Select(g => g.Select(x => x.Value).ToList());

            foreach (var batch in batches)
            {
                foreach (var terminalData in batch)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(terminalData.Customer) ||
                            string.IsNullOrWhiteSpace(terminalData.TerminalNo))
                        {
                            var error = CreateBulkErrorRecord(terminalData,
                                string.IsNullOrWhiteSpace(terminalData.Customer)
                                    ? ErrorDescription.CustomerIsRequired_1601
                                    : ErrorDescription.TerminalNoIsRequired_1604,
                                string.IsNullOrWhiteSpace(terminalData.Customer)
                                    ? ErrorNumber.CustomerIsRequired
                                    : ErrorNumber.TerminalNoIsRequired);

                            errorDetails.Add(error);
                            errorCount++;
                            continue;
                        }

                        bool success;
                        if (terminalData.Id == 0)
                        {
                            if (terminalData.CreatedById == null || terminalData.CreatedById == 0)
                            {
                                var error = CreateBulkErrorRecord(terminalData,
                                    "CreatedById is required for new record.",
                                    1001); // custom error code

                                errorDetails.Add(error);
                                errorCount++;
                                continue;
                            }
                            var terminalId = await SaveTerminalData(terminalData);
                            success = terminalId > 0;

                            if (success) addedRecordCount++;
                        }
                        else
                        {
                            if (terminalData.ModifiedById == null || terminalData.ModifiedById == 0)
                            {
                                var error = CreateBulkErrorRecord(terminalData,
                                    "ModifiedById is required for update.",
                                    1002); // custom error code

                                errorDetails.Add(error);
                                errorCount++;
                                continue;
                            }
                            success = await UpdateTerminalData(terminalData);

                            if (success) updatedRecordCount++;
                        }

                        if (success)
                        {
                            successCount++;
                            logger.WriteInfo($"[BulkAddUpdate] Processed terminal: Id={terminalData.Id}, TerminalNo={terminalData.TerminalNo}");
                        }
                        else
                        {
                            var error = CreateBulkErrorRecord(terminalData,
                                ErrorDescription.FailedToProcessTerminal_1614,
                                ErrorNumber.FailedToProcessTerminal);

                            errorDetails.Add(error);
                            errorCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.WriteError($"[BulkAddUpdate] Error processing terminal {terminalData.Id}: {ex.Message}");
                        var error = CreateBulkErrorRecord(terminalData,
                            ex.Message,
                            ErrorNumber.FailedToProcessTerminal);

                        errorDetails.Add(error);
                        errorCount++;
                    }
                }
            }

            result.Add("Success", errorCount == 0);
            result.Add("TotalRecords", bulkData.Count);
            result.Add("SuccessCount", successCount);
            result.Add("ErrorCount", errorCount);
            result.Add("ProcessingTimeMs", stopwatch.ElapsedMilliseconds);

            if (errorCount > 0)
            {
                result.Add("ErrorDetails", JArray.FromObject(errorDetails));
            }

            // ✅ Add success message
            string addMessage = $"{addedRecordCount} record{(addedRecordCount == 1 ? "" : "s")} added";
            string updateMessage = $"{updatedRecordCount} record{(updatedRecordCount == 1 ? "" : "s")} updated";
           // string skipMessage = $"{skipped.Count} duplicate record{(skipped.Count == 1 ? "" : "s")} skipped";

            result.Add("Message", $"Bulk add/update completed. {addMessage}, {updateMessage}.");

            return result;
        }

        private JObject CreateBulkErrorRecord(CrimpingStandardRequestVM data, string errorMessage, int errorNumber)
        {
            return new JObject
            {
                ["TerminalId"] = data.Id,
                ["TerminalNo"] = data.TerminalNo,
                ["ErrorMessage"] = errorMessage,
                ["ErrorNumber"] = errorNumber
            };
        }

     


        public async Task<JObject> DeleteTerminal(int terminalId, int modifiedById)
        {
            var result = new JObject();

            if (terminalId <= 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse("TerminalId is required", 400, 1001);
            }

            try
            {
                var success = await _crimpingStandardRepository.DeleteTerminal(terminalId, modifiedById);

                if (success)
                {
                    logger.WriteInfo($"[DeleteTerminal] Terminal and related data marked inactive: Id={terminalId}");

                    result.Add("Success", true);
                    result.Add("Message", "Terminal and related data marked as inactive.");
                    result.Add("TerminalId", terminalId);
                }
                else
                {
                    return ErrorResponseWrapper.CreateErrorResponse("Terminal not found", 404, 1002);
                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"[DeleteTerminal] Error: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse("Failed to delete terminal", 500, 1003);
            }

            return result;
        }


        public async Task<JObject> UpdateTerminal(CrimpingStandardRequestVM updateData)
        {
            var result = new JObject();

            // Validate required fields
            if (updateData.Id <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.TerminalIdIsRequired_1611,
                    400,
                    ErrorNumber.TerminalIdIsRequired);

            if (updateData.ModifiedById <= 0)
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.ModifiedByIdIsRequired_1057,
                    400,
                    ErrorNumber.ModifiedByIdIsRequired);

            if (string.IsNullOrWhiteSpace(updateData.Customer))
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.CustomerIsRequired_1601,
                    400,
                    ErrorNumber.CustomerIsRequired);

            if (string.IsNullOrWhiteSpace(updateData.TerminalNo))
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.TerminalNoIsRequired_1604,
                    400,
                    ErrorNumber.TerminalNoIsRequired);

            try
            {
                // Update terminal data
                var success = await UpdateTerminalData(updateData);

                if (success)
                {
                    logger.WriteInfo($"[UpdateTerminalData] Terminal updated: Id={updateData.Id}, TerminalNo={updateData.TerminalNo}");

                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.TerminalUpdatedSuccessfully);
                    result.Add("Message", SuccessDescription.TerminalUpdatedSuccessfully_2602);
                    result.Add("TerminalId", updateData.Id);
                }
                else
                {
                    logger.WriteInfo($"[UpdateTerminalData] Failed to update terminal: Id={updateData.Id}");
                    return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToUpdateTerminal_1612, 500, ErrorNumber.FailedToUpdateTerminal);

                }
            }
            catch (Exception ex)
            {
                logger.WriteError($"[UpdateTerminalData] Error updating terminal: {ex.Message}");
                return ErrorResponseWrapper.CreateErrorResponse(ErrorDescription.FailedToUpdateTerminal_1612, 500,ErrorNumber.FailedToUpdateTerminal);
            }

            return result;
        }

        private async Task<bool> UpdateTerminalData(CrimpingStandardRequestVM data)
        {
            using var transaction = await _crimpingStandardRepository.BeginTransactionAsync();

            try
            {
                // 1. Update main terminal record
                var terminal = await _crimpingStandardRepository.GetTerminalById(data.Id);
                if (terminal == null)
                {
                    logger.WriteInfo($"[UpdateTerminalData] Terminal not found: Id={data.Id}");
                    return false;
                }

                terminal.Customer = data.Customer;
                terminal.Flag = data.Flag;
                terminal.PlantCode = ParsePlantCode(data.Plant);
                terminal.RegistrationNo = data.RegistrationNo;
                terminal.ManufacturinCrimpNo = data.ManufacturinCrimpNo;
                terminal.TerminalNo = data.TerminalNo;
                terminal.CommonTerminalNo = data.CommonTerminalNo;
                terminal.TerminalName = data.TerminalName;
                terminal.TerminalThickness = data.TerminalThickness;
                terminal.ModifiedDate = DateTime.Now;
                terminal.ModifiedById= data.ModifiedById;

                await _crimpingStandardRepository.UpdateTerminal(terminal);

                // 2. Update or Add Accessories
                if (data.Accessories != null)
                {
                    var accessory = await _crimpingStandardRepository.GetAccessoryByTerminalId(data.Id);
                    if (accessory != null)
                    {
                        accessory.ShieldNo = data.Accessories.ShieldNo;
                        accessory.RubberSealPosition = data.Accessories.RubberSealPosition;
                        accessory.ModifiedDate = DateTime.Now;
                        accessory.ModifiedById = data.ModifiedById;
                        await _crimpingStandardRepository.UpdateAccessory(accessory);
                    }
                    else
                    {
                        var newAccessory = new T1C1Accessory
                        {
                            TerminalId = data.Id,
                            TerminalNo = data.TerminalNo,
                            ShieldNo = data.Accessories.ShieldNo,
                            RubberSealPosition = data.Accessories.RubberSealPosition,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatedById= data.CreatedById,
                        };
                        await _crimpingStandardRepository.AddAccessory(newAccessory);
                    }
                }

                // 3. Update or Add Applicator Details
                if (data.ApplicatorDetails != null)
                {
                    var applicator = await _crimpingStandardRepository.GetApplicatorDetailByTerminalId(data.Id);
                    if (applicator != null)
                    {
                        applicator.Machine = data.ApplicatorDetails.Machine;
                        applicator.Feed = data.ApplicatorDetails.Feed;
                        applicator.ModifiedDate = DateTime.Now;
                        applicator.ModifiedById= data.ModifiedById;
                        await _crimpingStandardRepository.UpdateApplicatorDetail(applicator);
                    }
                    else
                    {
                        var newApplicator = new T1C2ApplicatorDetail
                        {
                            TerminalId = data.Id,
                            TerminalNo = data.TerminalNo,
                            Machine = data.ApplicatorDetails.Machine,
                            Feed = data.ApplicatorDetails.Feed,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatedById = data.CreatedById,
                        };
                        await _crimpingStandardRepository.AddApplicatorDetail(newApplicator);
                    }
                }

                // 4. Update Wire Combinations (delete all and recreate)
                if (data.WireCombinations != null)
                {
                    await _crimpingStandardRepository.DeleteWireCombinationsByTerminalId(data.Id);

                    foreach (var wire in data.WireCombinations.Where(w => !string.IsNullOrWhiteSpace(w.WireCode)))
                    {
                        var combination = new T1C4CombinationDetail
                        {
                            TerminalId = data.Id,
                            TerminalNo = data.TerminalNo,
                            WireCode = wire.WireCode,
                            WireType = wire.WireType,
                            WireSizeCode = wire.WireSizeCode,
                            WireSize = wire.WireSize,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatedById= data.CreatedById,
                        };
                        await _crimpingStandardRepository.AddCombinationDetail(combination);
                    }
                }

                // 5. Update or Add Crimping Standard Details
                if (data.CrimpingStandardDetails != null)
                {
                    var wireSizeRange = ParseRange(data.CrimpingStandardDetails.ApplicableWireSize);
                    var ccHeight = ParseMeasurementWithTolerance(data.CrimpingStandardDetails.Ccheight);
                    var icHeight = ParseMeasurementWithTolerance(data.CrimpingStandardDetails.Icheight);
                    var ccWidth = ParseRange(data.CrimpingStandardDetails.Ccwidth);
                    var icWidth = ParseRange(data.CrimpingStandardDetails.Icwidth);

                    var crimpingStandard = await _crimpingStandardRepository.GetCrimpingStandardDetailByTerminalId(data.Id);
                    if (crimpingStandard != null)
                    {
                        crimpingStandard.ApplicableWireSize = data.CrimpingStandardDetails.ApplicableWireSize;
                        crimpingStandard.WireMin = wireSizeRange.Min;
                        crimpingStandard.WireMax = wireSizeRange.Max;
                        crimpingStandard.InsulationCrimpShape = await GetCrimpingShapeId(data.CrimpingStandardDetails.InsulationCrimpShape);
                        crimpingStandard.Ccheight = data.CrimpingStandardDetails.Ccheight;
                        crimpingStandard.CcheightStd = ccHeight.Value;
                        crimpingStandard.CcheightVariance = ccHeight.Tolerance;
                        crimpingStandard.Icheight = data.CrimpingStandardDetails.Icheight;
                        crimpingStandard.IcheightStd = icHeight.Value;
                        crimpingStandard.IcheightVariance = icHeight.Tolerance;
                        crimpingStandard.Ccwidth = data.CrimpingStandardDetails.Ccwidth;
                        crimpingStandard.CcwidthMin = ccWidth.Min;
                        crimpingStandard.CcwidthMax = ccWidth.Max;
                        crimpingStandard.Icwidth = data.CrimpingStandardDetails.Icwidth;
                        crimpingStandard.IcwidthMin = icWidth.Min;
                        crimpingStandard.IcwidthMax = icWidth.Max;
                        crimpingStandard.TensileForceKgf = data.CrimpingStandardDetails.TensileForceKgf;
                        crimpingStandard.TensileForceN = data.CrimpingStandardDetails.TensileForceN;
                        crimpingStandard.PillShape = data.CrimpingStandardDetails.PillShape;
                        crimpingStandard.Soldering = data.CrimpingStandardDetails.Soldering;
                        crimpingStandard.ModifiedDate = DateTime.Now;
                        crimpingStandard.ModifiedById = data.ModifiedById;
                        await _crimpingStandardRepository.UpdateCrimpingStandardDetail(crimpingStandard);
                    }
                    else
                    {
                        var newCrimpingStandard = new T1C5CrimpingStandardDetail
                        {
                            TerminalId = data.Id,
                            TerminalNo = data.TerminalNo,
                            ApplicableWireSize = data.CrimpingStandardDetails.ApplicableWireSize,
                            WireMin = wireSizeRange.Min,
                            WireMax = wireSizeRange.Max,
                            InsulationCrimpShape = await GetCrimpingShapeId(data.CrimpingStandardDetails.InsulationCrimpShape),
                            Ccheight = data.CrimpingStandardDetails.Ccheight,
                            CcheightStd = ccHeight.Value,
                            CcheightVariance = ccHeight.Tolerance,
                            Icheight = data.CrimpingStandardDetails.Icheight,
                            IcheightStd = icHeight.Value,
                            IcheightVariance = icHeight.Tolerance,
                            Ccwidth = data.CrimpingStandardDetails.Ccwidth,
                            CcwidthMin = ccWidth.Min,
                            CcwidthMax = ccWidth.Max,
                            Icwidth = data.CrimpingStandardDetails.Icwidth,
                            IcwidthMin = icWidth.Min,
                            IcwidthMax = icWidth.Max,
                            TensileForceKgf = data.CrimpingStandardDetails.TensileForceKgf,
                            TensileForceN = data.CrimpingStandardDetails.TensileForceN,
                            PillShape = data.CrimpingStandardDetails.PillShape,
                            Soldering = data.CrimpingStandardDetails.Soldering,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatedById=data.CreatedById,
                        };
                        await _crimpingStandardRepository.AddCrimpingStandardDetail(newCrimpingStandard);
                    }
                }

                // 6. Update or Add Striping Details
                if (data.StripingDetails != null)
                {
                    var striping = await _crimpingStandardRepository.GetStripingDetailByTerminalId(data.Id);
                    if (striping != null)
                    {
                        striping.MiddelStriping = data.StripingDetails.MiddelStriping;
                        striping.MiddelStrippingUpperLimit = data.StripingDetails.MiddelStrippingUpperLimit;
                        striping.MiddelStrippingLowerLimit = data.StripingDetails.MiddelStrippingLowerLimit;
                        striping.EndStriping = data.StripingDetails.EndStriping;
                        striping.EndStripingUpperLimit = data.StripingDetails.EndStripingUpperLimit;
                        striping.EndStripingLowerLimit = data.StripingDetails.EndStripingLowerLimit;
                        striping.ModifiedDate = DateTime.Now;
                        striping.ModifiedById=data.ModifiedById;
                        await _crimpingStandardRepository.UpdateStripingDetail(striping);
                    }
                    else
                    {
                        var newStriping = new T1C3StripingDetail
                        {
                            TerminalId = data.Id,
                            TerminalNo = data.TerminalNo,
                            MiddelStriping = data.StripingDetails.MiddelStriping,
                            MiddelStrippingUpperLimit = data.StripingDetails.MiddelStrippingUpperLimit,
                            MiddelStrippingLowerLimit = data.StripingDetails.MiddelStrippingLowerLimit,
                            EndStriping = data.StripingDetails.EndStriping,
                            EndStripingUpperLimit = data.StripingDetails.EndStripingUpperLimit,
                            EndStripingLowerLimit = data.StripingDetails.EndStripingLowerLimit,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatedById=data.CreatedById,
                        };
                        await _crimpingStandardRepository.AddStripingDetail(newStriping);
                    }
                }

                // 7. Update or Add Crimping Other Parameters
                if (data.CrimpingOtherParameters != null)
                {
                    var frontCuttingCarry = ParseRange(data.CrimpingOtherParameters.FrontCuttingCarry);
                    var rearCuttingCarry = ParseRange(data.CrimpingOtherParameters.RearCuttingCarry);
                    var brushLength = ParseRange(data.CrimpingOtherParameters.BrushLength);
                    var frontBellMouth = ParseRange(data.CrimpingOtherParameters.FrontBellMouth);
                    var rearBellMouth = ParseRange(data.CrimpingOtherParameters.RearBellMouth);

                    var otherParams = await _crimpingStandardRepository.GetCrimpingOtherParameterByTerminalId(data.Id);
                    if (otherParams != null)
                    {
                        otherParams.FrontCuttingCarry = data.CrimpingOtherParameters.FrontCuttingCarry;
                        otherParams.FronCuttingCarryMin = frontCuttingCarry.Min;
                        otherParams.FrontCuttingCarryMax = frontCuttingCarry.Max;
                        otherParams.RearCuttingCarry = data.CrimpingOtherParameters.RearCuttingCarry;
                        otherParams.RearCuttingCarryMin = rearCuttingCarry.Min;
                        otherParams.RearCuttingCarryMax = rearCuttingCarry.Max;
                        otherParams.BrushLength = data.CrimpingOtherParameters.BrushLength;
                        otherParams.BrushLengthMin = brushLength.Min;
                        otherParams.BrushLengthMax = brushLength.Max;
                        otherParams.FrontBellMouth = data.CrimpingOtherParameters.FrontBellMouth;
                        otherParams.FrontBellMouthMin = frontBellMouth.Min;
                        otherParams.FrontBellMouthMax = frontBellMouth.Max;
                        otherParams.RearBellMouth = data.CrimpingOtherParameters.RearBellMouth;
                        otherParams.RearBellMouthMin = rearBellMouth.Min;
                        otherParams.RearBellMouthMax = rearBellMouth.Max;
                        otherParams.BendUp = data.CrimpingOtherParameters.BendUp;
                        otherParams.BendUpUnit = data.CrimpingOtherParameters.BendUpUnit;
                        otherParams.BendDown = data.CrimpingOtherParameters.BendDown;
                        otherParams.BendDownUnit = data.CrimpingOtherParameters.BendDownUnit;
                        otherParams.Rolling = data.CrimpingOtherParameters.Rolling;
                        otherParams.RollingUnit = data.CrimpingOtherParameters.RollingUnit;
                        otherParams.Twist = data.CrimpingOtherParameters.Twist;
                        otherParams.TwistUnit = data.CrimpingOtherParameters.TwistUnit;
                        otherParams.ModifiedDate = DateTime.Now;
                        otherParams.ModifiedById = data.ModifiedById;
                        await _crimpingStandardRepository.UpdateCrimpingOtherParameter(otherParams);
                    }
                    else
                    {
                        var newOtherParams = new T1C6CrimpingOtherParameter
                        {
                            TerminalId = data.Id,
                            TerminalNo = data.TerminalNo,
                            FrontCuttingCarry = data.CrimpingOtherParameters.FrontCuttingCarry,
                            FronCuttingCarryMin = frontCuttingCarry.Min,
                            FrontCuttingCarryMax = frontCuttingCarry.Max,
                            RearCuttingCarry = data.CrimpingOtherParameters.RearCuttingCarry,
                            RearCuttingCarryMin = rearCuttingCarry.Min,
                            RearCuttingCarryMax = rearCuttingCarry.Max,
                            BrushLength = data.CrimpingOtherParameters.BrushLength,
                            BrushLengthMin = brushLength.Min,
                            BrushLengthMax = brushLength.Max,
                            FrontBellMouth = data.CrimpingOtherParameters.FrontBellMouth,
                            FrontBellMouthMin = frontBellMouth.Min,
                            FrontBellMouthMax = frontBellMouth.Max,
                            RearBellMouth = data.CrimpingOtherParameters.RearBellMouth,
                            RearBellMouthMin = rearBellMouth.Min,
                            RearBellMouthMax = rearBellMouth.Max,
                            BendUp = data.CrimpingOtherParameters.BendUp,
                            BendUpUnit = data.CrimpingOtherParameters.BendUpUnit,
                            BendDown = data.CrimpingOtherParameters.BendDown,
                            BendDownUnit = data.CrimpingOtherParameters.BendDownUnit,
                            Rolling = data.CrimpingOtherParameters.Rolling,
                            RollingUnit = data.CrimpingOtherParameters.RollingUnit,
                            Twist = data.CrimpingOtherParameters.Twist,
                            TwistUnit = data.CrimpingOtherParameters.TwistUnit,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatedById=data.CreatedById,
                        };
                        await _crimpingStandardRepository.AddCrimpingOtherParameter(newOtherParams);
                    }
                }

                // 8. Update or Add Crimping Dies Details
                if (data.CrimpingDiesDetails != null)
                {
                    var diesDetails = await _crimpingStandardRepository.GetCrimpingDiesDetailByTerminalId(data.Id);
                    if (diesDetails != null)
                    {
                        diesDetails.CrimpingDieNo_AnvilA = data.CrimpingDiesDetails.CrimpingDieNoAnvilA;
                        diesDetails.CrimpingDieNo_WireCrimperW = data.CrimpingDiesDetails.CrimpingDieNoWireCrimperW;
                        diesDetails.CrimpingDieNo_InsulationCrimperI = data.CrimpingDiesDetails.CrimpingDieNoInsulationCrimperI;
                        diesDetails.CrimpingDieNo_StabilizerCrimperQ = data.CrimpingDiesDetails.CrimpingDieNoStabilizerCrimperQ;
                        diesDetails.DiesCrimpingWidth_ConductorAnvilA = data.CrimpingDiesDetails.DiesCrimpingWidthConductorAnvilA;
                        diesDetails.DiesCrimpingWidth_InsulationAnvilA = data.CrimpingDiesDetails.DiesCrimpingWidthInsulationAnvilA;
                        diesDetails.DiesCrimpingWidth_WireCrimperW = data.CrimpingDiesDetails.DiesCrimpingWidthWireCrimperW;
                        diesDetails.DiesCrimpingWidth_InsulationCrimperI = data.CrimpingDiesDetails.DiesCrimpingWidthInsulationCrimperI;
                        diesDetails.ConductorDieThickness = data.CrimpingDiesDetails.ConductorDieThickness;
                        diesDetails.InsulationDieThickness = data.CrimpingDiesDetails.InsulationDieThickness;
                        diesDetails.ModifiedDate = DateTime.Now;
                        diesDetails.ModifiedById = data.ModifiedById;
                        await _crimpingStandardRepository.UpdateCrimpingDiesDetail(diesDetails);
                    }
                    else
                    {
                        var newDiesDetails = new T1C7CrimpingDiesDetail
                        {
                            TerminalId = data.Id,
                            TerminalNo = data.TerminalNo,
                            CrimpingDieNo_AnvilA = data.CrimpingDiesDetails.CrimpingDieNoAnvilA,
                            CrimpingDieNo_WireCrimperW = data.CrimpingDiesDetails.CrimpingDieNoWireCrimperW,
                            CrimpingDieNo_InsulationCrimperI = data.CrimpingDiesDetails.CrimpingDieNoInsulationCrimperI,
                            CrimpingDieNo_StabilizerCrimperQ = data.CrimpingDiesDetails.CrimpingDieNoStabilizerCrimperQ,
                            DiesCrimpingWidth_ConductorAnvilA = data.CrimpingDiesDetails.DiesCrimpingWidthConductorAnvilA,
                            DiesCrimpingWidth_InsulationAnvilA = data.CrimpingDiesDetails.DiesCrimpingWidthInsulationAnvilA,
                            DiesCrimpingWidth_WireCrimperW = data.CrimpingDiesDetails.DiesCrimpingWidthWireCrimperW,
                            DiesCrimpingWidth_InsulationCrimperI = data.CrimpingDiesDetails.DiesCrimpingWidthInsulationCrimperI,
                            ConductorDieThickness = data.CrimpingDiesDetails.ConductorDieThickness,
                            InsulationDieThickness = data.CrimpingDiesDetails.InsulationDieThickness,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                           CreatedById=data.CreatedById,
                        };
                        await _crimpingStandardRepository.AddCrimpingDiesDetail(newDiesDetails);
                    }
                }

                // 9. Update or Add Approval Details
                if (data.ApprovalDetails != null)
                {
                    var approval = await _crimpingStandardRepository.GetApprovalDetailByTerminalId(data.Id);
                    if (approval != null)
                    {
                        approval.RevisionNo = data.ApprovalDetails.RevisionNo;
                        approval.RevisionDate = data.ApprovalDetails.RevisionDate;
                        approval.RevisionDetails = data.ApprovalDetails.RevisionDetails;
                        approval.MadeBy = data.ApprovalDetails.MadeBy;
                        approval.CheckedBy = data.ApprovalDetails.CheckedBy;
                        approval.ApprovedBy = data.ApprovalDetails.ApprovedBy;
                        approval.ModifiedDate = DateTime.Now;
                        approval.ModifiedById=data.ModifiedById;
                        await _crimpingStandardRepository.UpdateApprovalDetail(approval);
                    }
                    else
                    {
                        var newApproval = new T1C8ApprovalDetail
                        {
                            TerminalId = data.Id,
                            TerminalNo = data.TerminalNo,
                            RevisionNo = data.ApprovalDetails.RevisionNo,
                            RevisionDate = data.ApprovalDetails.RevisionDate,
                            RevisionDetails = data.ApprovalDetails.RevisionDetails,
                            MadeBy = data.ApprovalDetails.MadeBy,
                            CheckedBy = data.ApprovalDetails.CheckedBy,
                            ApprovedBy = data.ApprovalDetails.ApprovedBy,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatedById=data.CreatedById,
                        };
                        await _crimpingStandardRepository.AddApprovalDetail(newApproval);
                    }
                }

                // 10. Update or Add Terminal Supplier Crimping Spec
                if (data.TerminalSupplierCrimpingSpec != null)
                {
                    var supplierCcHeight = ParseMeasurementWithTolerance(data.TerminalSupplierCrimpingSpec.Ccheight);
                    var supplierIcHeight = ParseMeasurementWithTolerance(data.TerminalSupplierCrimpingSpec.Icheight);
                    var supplierCcWidth = ParseRange(data.TerminalSupplierCrimpingSpec.Ccwidth);
                    var supplierIcWidth = ParseRange(data.TerminalSupplierCrimpingSpec.Icwidth);
                    var wireSizeRange = ParseRange(data.TerminalSupplierCrimpingSpec.ApplicableWireSize);

                    var supplierSpec = await _crimpingStandardRepository.GetTerminalSupplierCrimpingSpecByTerminalId(data.Id);
                    if (supplierSpec != null)
                    {
                        supplierSpec.TerminalSupplierName = data.TerminalSupplierCrimpingSpec.TerminalSupplierName;
                        supplierSpec.TerminalSupplierNumber = data.TerminalSupplierCrimpingSpec.TerminalSupplierNumber;
                        supplierSpec.ApplicableWireSize = data.TerminalSupplierCrimpingSpec.ApplicableWireSize;
                        supplierSpec.ApplicableWireSizeMin = wireSizeRange.Min;
                        supplierSpec.ApplicableWireSizeMax = wireSizeRange.Max;
                       // supplierSpec.InsulationCrimpShape = data.TerminalSupplierCrimpingSpec.InsulationCrimpShape;
                        supplierSpec.InsulationCrimpShape = await GetCrimpingShapeId(data.CrimpingStandardDetails.InsulationCrimpShape);

                        supplierSpec.CC_Height = data.TerminalSupplierCrimpingSpec.Ccheight;
                        supplierSpec.CC_HeightStd = supplierCcHeight.Value;
                        supplierSpec.CC_HeightTolerance = supplierCcHeight.Tolerance;
                        supplierSpec.IC_Height = data.TerminalSupplierCrimpingSpec.Icheight;
                        supplierSpec.IC_HeightStd = supplierIcHeight.Value;
                        supplierSpec.IC_HeightTolerance = supplierIcHeight.Tolerance;
                        supplierSpec.CC_Width = data.TerminalSupplierCrimpingSpec.Ccwidth;
                        supplierSpec.CC_WidthMIn = supplierCcWidth.Min;
                        supplierSpec.CC_WidthMax = supplierCcWidth.Max;
                        supplierSpec.IC_Width = data.TerminalSupplierCrimpingSpec.Icwidth;
                        supplierSpec.IC_WidthMin = supplierIcWidth.Min;
                        supplierSpec.IC_WidthMax = supplierIcWidth.Max;
                        supplierSpec.TensileForce_KGF = data.TerminalSupplierCrimpingSpec.TensileForceKgf;
                        supplierSpec.TensileForce_N = data.TerminalSupplierCrimpingSpec.TensileForceN;
                        supplierSpec.StandardAsperSupplier = data.TerminalSupplierCrimpingSpec.StandardAsperSupplier;
                        supplierSpec.ModifiedDate = DateTime.Now;
                        supplierSpec.ModifiedById=data.ModifiedById;
                        await _crimpingStandardRepository.UpdateTerminalSupplierCrimpingSpec(supplierSpec);
                    }
                    else
                    {
                        var newSupplierSpec = new T1C9TerminalSupplierCrimpingSpec
                        {
                            TerminalId = data.Id,
                            TerminalNo = data.TerminalNo,
                            TerminalSupplierName = data.TerminalSupplierCrimpingSpec.TerminalSupplierName,
                            TerminalSupplierNumber = data.TerminalSupplierCrimpingSpec.TerminalSupplierNumber,
                            ApplicableWireSize = data.TerminalSupplierCrimpingSpec.ApplicableWireSize,
                            ApplicableWireSizeMin = wireSizeRange.Min,
                            ApplicableWireSizeMax = wireSizeRange.Max,
                            //InsulationCrimpShape = data.TerminalSupplierCrimpingSpec.InsulationCrimpShape,
                            InsulationCrimpShape = await GetCrimpingShapeId(data.CrimpingStandardDetails.InsulationCrimpShape),
                            CC_Height = data.TerminalSupplierCrimpingSpec.Ccheight,
                            CC_HeightStd = supplierCcHeight.Value,
                            CC_HeightTolerance = supplierCcHeight.Tolerance,
                            IC_Height = data.TerminalSupplierCrimpingSpec.Icheight,
                            IC_HeightStd = supplierIcHeight.Value,
                            IC_HeightTolerance = supplierIcHeight.Tolerance,
                            CC_Width = data.TerminalSupplierCrimpingSpec.Ccwidth,
                            CC_WidthMIn = supplierCcWidth.Min,
                            CC_WidthMax = supplierCcWidth.Max,
                            IC_Width = data.TerminalSupplierCrimpingSpec.Icwidth,
                            IC_WidthMin = supplierIcWidth.Min,
                            IC_WidthMax = supplierIcWidth.Max,
                            TensileForce_KGF = data.TerminalSupplierCrimpingSpec.TensileForceKgf,
                            TensileForce_N = data.TerminalSupplierCrimpingSpec.TensileForceN,
                            StandardAsperSupplier = data.TerminalSupplierCrimpingSpec.StandardAsperSupplier,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            CreatedById=data.CreatedById,
                        };
                        await _crimpingStandardRepository.AddTerminalSupplierCrimpingSpec(newSupplierSpec);
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.WriteError($"[UpdateTerminalData] Transaction failed: {ex.Message}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<CrimpingStandardRequestVM>> GetAllTerminalsWithDetails(JObject filters)
        {
            try
            {
                logger.WriteInfo("Fetching all terminals with details");

                var terminals = await _crimpingStandardRepository.GetAllActiveTerminalsWithDetails(filters);

                if (terminals == null || !terminals.Any())
                {
                    logger.WriteInfo("No active terminals found");
                    return new List<CrimpingStandardRequestVM>();
                }

                var result = terminals.Select(t => new CrimpingStandardRequestVM
                {
                    Id = t.Id,
                    Customer = t.Customer,
                    Flag = t.Flag,
                    Plant = t.PlantCode.ToString(),
                    RegistrationNo = t.RegistrationNo,
                    ManufacturinCrimpNo = t.ManufacturinCrimpNo,
                    TerminalNo = t.TerminalNo,
                    CommonTerminalNo = t.CommonTerminalNo,
                    TerminalName = t.TerminalName,
                    TerminalThickness = t.TerminalThickness,
                    CreatedById=t.CreatedById,  
                    CreatedDate=t.CreatedDate,

                    Accessories = t.Accessories.Select(a => new AccessoryDto
                    {
                        Id = a.Id,
                        ShieldNo = a.ShieldNo,
                        RubberSealPosition = a.RubberSealPosition
                    }).FirstOrDefault(),

                    ApplicatorDetails = t.ApplicatorDetails.Select(a => new ApplicatorDetailDto
                    {
                        Id = a.Id,
                        Machine = a.Machine,
                        Feed = a.Feed
                    }).FirstOrDefault(),

                    WireCombinations = t.WireCombinations.Select(w => new WireCombinationDto
                    {
                        Id = w.Id,
                        WireCode = w.WireCode,
                        WireType = w.WireType,
                        WireSizeCode = w.WireSizeCode,
                        WireSize = w.WireSize
                    }).ToList(),

                    CrimpingStandardDetails = t.CrimpingStandardDetails.Select(c => new CrimpingStandardDetailDto
                    {
                        Id = c.Id,
                        ApplicableWireSize = c.ApplicableWireSize,
                        InsulationCrimpShape = c.InsulationCrimpShapeNavigation?.Name,
                        Ccheight = c.Ccheight,
                        Icheight = c.Icheight,
                        Ccwidth = c.Ccwidth,
                        Icwidth = c.Icwidth,
                        TensileForceKgf = c.TensileForceKgf,
                        TensileForceN = c.TensileForceN,
                        PillShape = c.PillShape,
                        Soldering = c.Soldering
                    }).FirstOrDefault(),

                    StripingDetails = t.StripingDetails.Select(s => new StripingDetailDto
                    {
                        Id = s.Id,
                        MiddelStriping = s.MiddelStriping,
                        MiddelStrippingUpperLimit = s.MiddelStrippingUpperLimit,
                        MiddelStrippingLowerLimit = s.MiddelStrippingLowerLimit,
                        EndStriping = s.EndStriping,
                        EndStripingUpperLimit = s.EndStripingUpperLimit,
                        EndStripingLowerLimit = s.EndStripingLowerLimit
                    }).FirstOrDefault(),

                    CrimpingOtherParameters = t.CrimpingOtherParameters.Select(c => new CrimpingOtherParameterDto
                    {
                        Id = c.Id,
                        FrontCuttingCarry = c.FrontCuttingCarry,
                        RearCuttingCarry = c.RearCuttingCarry,
                        BrushLength = c.BrushLength,
                        FrontBellMouth = c.FrontBellMouth,
                        RearBellMouth = c.RearBellMouth,
                        BendUp = c.BendUp,
                        BendUpUnit = c.BendUpUnit,
                        BendDown = c.BendDown,
                        BendDownUnit = c.BendDownUnit,
                        Rolling = c.Rolling,
                        RollingUnit = c.RollingUnit,
                        Twist = c.Twist,
                        TwistUnit = c.TwistUnit
                    }).FirstOrDefault(),

                    CrimpingDiesDetails = t.CrimpingDiesDetails.Select(c => new CrimpingDiesDetailDto
                    {
                        Id = c.Id,
                        CrimpingDieNoAnvilA = c.CrimpingDieNo_AnvilA,
                        CrimpingDieNoWireCrimperW = c.CrimpingDieNo_WireCrimperW,
                        CrimpingDieNoInsulationCrimperI = c.CrimpingDieNo_InsulationCrimperI,
                        CrimpingDieNoStabilizerCrimperQ = c.CrimpingDieNo_StabilizerCrimperQ,
                        DiesCrimpingWidthConductorAnvilA = c.DiesCrimpingWidth_ConductorAnvilA,
                        DiesCrimpingWidthInsulationAnvilA = c.DiesCrimpingWidth_InsulationAnvilA,
                        DiesCrimpingWidthWireCrimperW = c.DiesCrimpingWidth_WireCrimperW,
                        DiesCrimpingWidthInsulationCrimperI = c.DiesCrimpingWidth_InsulationCrimperI,
                        ConductorDieThickness = c.ConductorDieThickness,
                        InsulationDieThickness = c.InsulationDieThickness
                    }).FirstOrDefault(),

                    ApprovalDetails = t.ApprovalDetails.Select(a => new ApprovalDetailDto
                    {
                        Id = a.Id,
                        RevisionNo = a.RevisionNo,
                        RevisionDate = a.RevisionDate,
                        RevisionDetails = a.RevisionDetails,
                        MadeBy = a.MadeBy,
                        CheckedBy = a.CheckedBy,
                        ApprovedBy = a.ApprovedBy
                    }).FirstOrDefault(),

                    TerminalSupplierCrimpingSpec = t.TerminalSupplierCrimpingSpec.Select(t => new TerminalSupplierCrimpingSpecDto
                    {
                        Id = t.Id,
                        TerminalSupplierName = t.TerminalSupplierName,
                        TerminalSupplierNumber = t.TerminalSupplierNumber,
                        ApplicableWireSize = t.ApplicableWireSize,
                        InsulationCrimpShape = t.InsulationCrimpShapeNavigation?.Name,
                        Ccheight = t.CC_Height,
                        Icheight = t.IC_Height,
                        Ccwidth = t.CC_Width,
                        Icwidth = t.IC_Width,
                        TensileForceKgf = t.TensileForce_KGF,
                        TensileForceN = t.TensileForce_N,
                        StandardAsperSupplier = t.StandardAsperSupplier
                    }).FirstOrDefault()
                }).ToList();

                logger.WriteInfo($"Successfully retrieved {result.Count} terminals with details");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in GetAllTerminalsWithDetails: {ex.Message}");
                throw;
            }
        }
        public async Task<List<CrimpingStandardRequestVM>> GetAllTerminalsWithDetails_SP(JObject filters)
        {
            try
            {
                logger.WriteInfo("Fetching all terminals with details");

                var terminals = await _crimpingStandardRepository.GetAllActiveTerminalsWithDetails_SP(filters);

                if (terminals == null || !terminals.Any())
                {
                    logger.WriteInfo("No active terminals found");
                    return new List<CrimpingStandardRequestVM>();
                }

                var result = terminals.Select(t => new CrimpingStandardRequestVM
                {
                    Id = t.Id,
                    Customer = t.Customer,
                    Flag = t.Flag,
                    Plant = t.PlantCode.ToString(),
                    RegistrationNo = t.RegistrationNo,
                    ManufacturinCrimpNo = t.ManufacturinCrimpNo,
                    TerminalNo = t.TerminalNo,
                    CommonTerminalNo = t.CommonTerminalNo,
                    TerminalName = t.TerminalName,
                    TerminalThickness = t.TerminalThickness,
                    CreatedById = t.CreatedById,
                    CreatedDate = t.CreatedDate,

                    Accessories = t.Accessories.Select(a => new AccessoryDto
                    {
                        Id = a.Id,
                        ShieldNo = a.ShieldNo,
                        RubberSealPosition = a.RubberSealPosition
                    }).FirstOrDefault(),

                    ApplicatorDetails = t.ApplicatorDetails.Select(a => new ApplicatorDetailDto
                    {
                        Id = a.Id,
                        Machine = a.Machine,
                        Feed = a.Feed
                    }).FirstOrDefault(),

                    WireCombinations = t.WireCombinations.Select(w => new WireCombinationDto
                    {
                        Id = w.Id,
                        WireCode = w.WireCode,
                        WireType = w.WireType,
                        WireSizeCode = w.WireSizeCode,
                        WireSize = w.WireSize
                    }).ToList(),

                    CrimpingStandardDetails = t.CrimpingStandardDetails.Select(c => new CrimpingStandardDetailDto
                    {
                        Id = c.Id,
                        ApplicableWireSize = c.ApplicableWireSize,
                        InsulationCrimpShape = c.InsulationCrimpShapeNavigation?.Name,
                        Ccheight = c.Ccheight,
                        Icheight = c.Icheight,
                        Ccwidth = c.Ccwidth,
                        Icwidth = c.Icwidth,
                        TensileForceKgf = c.TensileForceKgf,
                        TensileForceN = c.TensileForceN,
                        PillShape = c.PillShape,
                        Soldering = c.Soldering
                    }).FirstOrDefault(),

                    StripingDetails = t.StripingDetails.Select(s => new StripingDetailDto
                    {
                        Id = s.Id,
                        MiddelStriping = s.MiddelStriping,
                        MiddelStrippingUpperLimit = s.MiddelStrippingUpperLimit,
                        MiddelStrippingLowerLimit = s.MiddelStrippingLowerLimit,
                        EndStriping = s.EndStriping,
                        EndStripingUpperLimit = s.EndStripingUpperLimit,
                        EndStripingLowerLimit = s.EndStripingLowerLimit
                    }).FirstOrDefault(),

                    CrimpingOtherParameters = t.CrimpingOtherParameters.Select(c => new CrimpingOtherParameterDto
                    {
                        Id = c.Id,
                        FrontCuttingCarry = c.FrontCuttingCarry,
                        RearCuttingCarry = c.RearCuttingCarry,
                        BrushLength = c.BrushLength,
                        FrontBellMouth = c.FrontBellMouth,
                        RearBellMouth = c.RearBellMouth,
                        BendUp = c.BendUp,
                        BendUpUnit = c.BendUpUnit,
                        BendDown = c.BendDown,
                        BendDownUnit = c.BendDownUnit,
                        Rolling = c.Rolling,
                        RollingUnit = c.RollingUnit,
                        Twist = c.Twist,
                        TwistUnit = c.TwistUnit
                    }).FirstOrDefault(),

                    CrimpingDiesDetails = t.CrimpingDiesDetails.Select(c => new CrimpingDiesDetailDto
                    {
                        Id = c.Id,
                        CrimpingDieNoAnvilA = c.CrimpingDieNo_AnvilA,
                        CrimpingDieNoWireCrimperW = c.CrimpingDieNo_WireCrimperW,
                        CrimpingDieNoInsulationCrimperI = c.CrimpingDieNo_InsulationCrimperI,
                        CrimpingDieNoStabilizerCrimperQ = c.CrimpingDieNo_StabilizerCrimperQ,
                        DiesCrimpingWidthConductorAnvilA = c.DiesCrimpingWidth_ConductorAnvilA,
                        DiesCrimpingWidthInsulationAnvilA = c.DiesCrimpingWidth_InsulationAnvilA,
                        DiesCrimpingWidthWireCrimperW = c.DiesCrimpingWidth_WireCrimperW,
                        DiesCrimpingWidthInsulationCrimperI = c.DiesCrimpingWidth_InsulationCrimperI,
                        ConductorDieThickness = c.ConductorDieThickness,
                        InsulationDieThickness = c.InsulationDieThickness
                    }).FirstOrDefault(),

                    ApprovalDetails = t.ApprovalDetails.Select(a => new ApprovalDetailDto
                    {
                        Id = a.Id,
                        RevisionNo = a.RevisionNo,
                        RevisionDate = a.RevisionDate,
                        RevisionDetails = a.RevisionDetails,
                        MadeBy = a.MadeBy,
                        CheckedBy = a.CheckedBy,
                        ApprovedBy = a.ApprovedBy
                    }).FirstOrDefault(),

                    TerminalSupplierCrimpingSpec = t.TerminalSupplierCrimpingSpec.Select(t => new TerminalSupplierCrimpingSpecDto
                    {
                        Id = t.Id,
                        TerminalSupplierName = t.TerminalSupplierName,
                        TerminalSupplierNumber = t.TerminalSupplierNumber,
                        ApplicableWireSize = t.ApplicableWireSize,
                        InsulationCrimpShape = t.InsulationCrimpShapeNavigation?.Name,
                        Ccheight = t.CC_Height,
                        Icheight = t.IC_Height,
                        Ccwidth = t.CC_Width,
                        Icwidth = t.IC_Width,
                        TensileForceKgf = t.TensileForce_KGF,
                        TensileForceN = t.TensileForce_N,
                        StandardAsperSupplier = t.StandardAsperSupplier
                    }).FirstOrDefault()
                }).ToList();

                logger.WriteInfo($"Successfully retrieved {result.Count} terminals with details");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in GetAllTerminalsWithDetails: {ex.Message}");
                throw;
            }
        }

        public async Task<JObject> ImportTerminalData(CrimpingStandardRequestVM importData)
        {
            var result = new JObject();

            // Validate required fields
            if (string.IsNullOrWhiteSpace(importData.Customer))
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.CustomerIsRequired_1601,
                    400,
                    ErrorNumber.CustomerIsRequired);

            if (importData.CreatedById == null || importData.CreatedById == 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.CreatedByIdIsRequired_1009,
                    400,
                    ErrorNumber.CreatedByIdIsRequired);
            }


            if (string.IsNullOrWhiteSpace(importData.Plant))
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.PlantIsRequired_1007,
                    400,
                    ErrorNumber.PlantIsRequired);

            if (string.IsNullOrWhiteSpace(importData.RegistrationNo))
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.RegistrationNoIsRequired_1603,
                    400,
                    ErrorNumber.RegistrationNoIsRequired);

            if (string.IsNullOrWhiteSpace(importData.TerminalNo))
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.TerminalNoIsRequired_1604,
                    400,
                    ErrorNumber.TerminalNoIsRequired);

            // Check if terminal already exists
            //var existingTerminal = await _crimpingStandardRepository.GetByTerminalNo(importData.TerminalNo);
            //if (existingTerminal != null)
            //{
            //    string error = string.Format(ErrorDescription.TerminalAlreadyExists_1605, importData.TerminalNo);
            //    return ErrorResponseWrapper.CreateErrorResponse(error, 409, ErrorNumber.TerminalAlreadyExists);
            //}


            var customerExists = await _crimpingStandardRepository.CustomerExists(importData.Customer);
            if (!customerExists)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    $"Customer '{importData.Customer}' does not exist in master data",
                    400,
                    ErrorNumber.CustomerNotFound);
            }

            // Check if plant exists in master
            var plantCode = ParsePlantCode(importData.Plant); // Your existing plant code parsing
            var plantExists = await _crimpingStandardRepository.PlantExists(plantCode.ToString());
            if (!plantExists)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    $"Plant '{importData.Plant}' (code: {plantCode}) does not exist in master data",
                    400,
                    ErrorNumber.PlantNotFound);
            }


            var existingTerminal = await _crimpingStandardRepository.GetTeminalCombination(
         importData.TerminalNo,
         importData.CrimpingStandardDetails.InsulationCrimpShape,
         importData.Accessories.ShieldNo);

            if (existingTerminal != null)
            {
                string error = $"Terminal combination already exists (TerminalNo: {importData.TerminalNo}, " +
                              $"InsulationShape: {importData.CrimpingStandardDetails.InsulationCrimpShape}, " +
                              $"AccessoryNo: {importData.Accessories.ShieldNo})";

                return ErrorResponseWrapper.CreateErrorResponse(
                    error,
                    409, // Conflict status code
                    ErrorNumber.TerminalAlreadyExists);
            }



            // Create and save terminal entity
            var terminalId = await SaveTerminalData(importData);

                if (terminalId > 0)
                {
                    logger.WriteInfo($"[ImportTerminalData] Terminal added: Id={terminalId}, TerminalNo={importData.TerminalNo}");

                    result.Add("Success", true);
                    result.Add("SuccessNumber", SuccessNumber.TerminalAddedSuccessfully);
                    result.Add("Message", SuccessDescription.TerminalAddedSuccessfully_2601);
                    result.Add("TerminalId", terminalId);
                }
                else
                {
                    logger.WriteInfo($"[ImportTerminalData] Failed to add terminal: TerminalNo={importData.TerminalNo}");
                    return ErrorResponseWrapper.CreateErrorResponse(
                        ErrorDescription.FailedToAddTerminal_1606,
                        500,
                        ErrorNumber.FailedToAddTerminal);
                }
            
        

            return result;
        }

        private async Task<int> SaveTerminalData(CrimpingStandardRequestVM data)
        {
            using var transaction = await _crimpingStandardRepository.BeginTransactionAsync();

            try
            {
                // 1. Save main terminal record
                var terminal = new T1Terminal
                {
                    Customer = data.Customer,
                    Flag = data.Flag,
                    PlantCode = ParsePlantCode(data.Plant),
                    RegistrationNo = data.RegistrationNo,
                    ManufacturinCrimpNo = data.ManufacturinCrimpNo,
                    TerminalNo = data.TerminalNo,
                    CommonTerminalNo = data.CommonTerminalNo,
                    TerminalName = data.TerminalName,
                    TerminalThickness = data.TerminalThickness,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedById= data.CreatedById,
                };

                var terminalId = await _crimpingStandardRepository.AddTerminal(terminal);

                // 2. Save Accessories
                if (data.Accessories != null)
                {
                    var accessory = new T1C1Accessory
                    {
                        TerminalId = terminalId,
                        TerminalNo = data.TerminalNo,
                        ShieldNo = data.Accessories.ShieldNo,
                        RubberSealPosition = data.Accessories.RubberSealPosition,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedById=data.CreatedById,
                    };
                    await _crimpingStandardRepository.AddAccessory(accessory);
                }

                // 3. Save Applicator Details
                if (data.ApplicatorDetails != null)
                {
                    var applicator = new T1C2ApplicatorDetail
                    {
                        TerminalId = terminalId,
                        TerminalNo = data.TerminalNo,
                        Machine = data.ApplicatorDetails.Machine,
                        Feed = data.ApplicatorDetails.Feed,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedById=data.CreatedById,
                    };
                    await _crimpingStandardRepository.AddApplicatorDetail(applicator);
                }

                // 4. Save Combination Details (Wire Types)
                if (data.WireCombinations != null && data.WireCombinations.Any())
                {
                    foreach (var wire in data.WireCombinations)
                    {
                        if (!string.IsNullOrWhiteSpace(wire.WireCode))
                        {
                            var combination = new T1C4CombinationDetail
                            {
                                TerminalId = terminalId,
                                TerminalNo = data.TerminalNo,
                                WireCode = wire.WireCode,
                                WireType = wire.WireType,
                                WireSizeCode = wire.WireSizeCode,
                                WireSize = wire.WireSize,
                                IsActive = true,
                                CreatedDate = DateTime.Now,
                                CreatedById=data.CreatedById,
                            };
                            await _crimpingStandardRepository.AddCombinationDetail(combination);
                        }
                    }
                }

                // 5. Save Crimping Standard Details
                if (data.CrimpingStandardDetails != null)
                {
                    // Parse applicable wire size range
                    var wireSizeRange = ParseRange(data.CrimpingStandardDetails.ApplicableWireSize);

                    // Parse height and width measurements
                    var ccHeight = ParseMeasurementWithTolerance(data.CrimpingStandardDetails.Ccheight);
                    var icHeight = ParseMeasurementWithTolerance(data.CrimpingStandardDetails.Icheight);
                    var ccWidth = ParseRange(data.CrimpingStandardDetails.Ccwidth);
                    var icWidth = ParseRange(data.CrimpingStandardDetails.Icwidth);

                    var crimpingStandard = new T1C5CrimpingStandardDetail
                    {
                        TerminalId = terminalId,
                        TerminalNo = data.TerminalNo,
                        ApplicableWireSize = data.CrimpingStandardDetails.ApplicableWireSize,
                        WireMin = wireSizeRange.Min,
                        WireMax = wireSizeRange.Max,
                        InsulationCrimpShape = await GetCrimpingShapeId(data.CrimpingStandardDetails.InsulationCrimpShape),
                        Ccheight = data.CrimpingStandardDetails.Ccheight,
                        CcheightStd = ccHeight.Value,
                        CcheightVariance = ccHeight.Tolerance,
                        Icheight = data.CrimpingStandardDetails.Icheight,
                        IcheightStd = icHeight.Value,
                        IcheightVariance = icHeight.Tolerance,
                        Ccwidth = data.CrimpingStandardDetails.Ccwidth,
                        CcwidthMin = ccWidth.Min,
                        CcwidthMax = ccWidth.Max,
                        Icwidth = data.CrimpingStandardDetails.Icwidth,
                        IcwidthMin = icWidth.Min,
                        IcwidthMax = icWidth.Max,
                        TensileForceKgf = data.CrimpingStandardDetails.TensileForceKgf,
                        TensileForceN = data.CrimpingStandardDetails.TensileForceN,
                        PillShape = data.CrimpingStandardDetails.PillShape,
                        Soldering = data.CrimpingStandardDetails.Soldering,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedById=data.CreatedById,
                    };
                    await _crimpingStandardRepository.AddCrimpingStandardDetail(crimpingStandard);
                }

                // 6. Save Striping Details
                if (data.StripingDetails != null)
                {
                    var striping = new T1C3StripingDetail
                    {
                        TerminalId = terminalId,
                        TerminalNo = data.TerminalNo,
                        MiddelStriping = data.StripingDetails.MiddelStriping,
                        MiddelStrippingUpperLimit = data.StripingDetails.MiddelStrippingUpperLimit,
                        MiddelStrippingLowerLimit = data.StripingDetails.MiddelStrippingLowerLimit,
                        EndStriping = data.StripingDetails.EndStriping,
                        EndStripingUpperLimit = data.StripingDetails.EndStripingUpperLimit,
                        EndStripingLowerLimit = data.StripingDetails.EndStripingLowerLimit,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedById=data.CreatedById,
                        
                    };
                    await _crimpingStandardRepository.AddStripingDetail(striping);
                }

                // 7. Save Crimping Other Parameters
                if (data.CrimpingOtherParameters != null)
                {
                    // Parse all range values
                    var frontCuttingCarry = ParseRange(data.CrimpingOtherParameters.FrontCuttingCarry);
                    var rearCuttingCarry = ParseRange(data.CrimpingOtherParameters.RearCuttingCarry);
                    var brushLength = ParseRange(data.CrimpingOtherParameters.BrushLength);
                    var frontBellMouth = ParseRange(data.CrimpingOtherParameters.FrontBellMouth);
                    var rearBellMouth = ParseRange(data.CrimpingOtherParameters.RearBellMouth);

                    var otherParams = new T1C6CrimpingOtherParameter
                    {
                        TerminalId = terminalId,
                        TerminalNo = data.TerminalNo,
                        FrontCuttingCarry = data.CrimpingOtherParameters.FrontCuttingCarry,
                        FronCuttingCarryMin = frontCuttingCarry.Min,
                        FrontCuttingCarryMax = frontCuttingCarry.Max,
                        RearCuttingCarry = data.CrimpingOtherParameters.RearCuttingCarry,
                        RearCuttingCarryMin = rearCuttingCarry.Min,
                        RearCuttingCarryMax = rearCuttingCarry.Max,
                        BrushLength = data.CrimpingOtherParameters.BrushLength,
                        BrushLengthMin = brushLength.Min,
                        BrushLengthMax = brushLength.Max,
                        FrontBellMouth = data.CrimpingOtherParameters.FrontBellMouth,
                        FrontBellMouthMin = frontBellMouth.Min,
                        FrontBellMouthMax = frontBellMouth.Max,
                        RearBellMouth = data.CrimpingOtherParameters.RearBellMouth,
                        RearBellMouthMin = rearBellMouth.Min,
                        RearBellMouthMax = rearBellMouth.Max,
                        BendUp = data.CrimpingOtherParameters.BendUp,
                        BendUpUnit = data.CrimpingOtherParameters.BendUpUnit,
                        BendDown = data.CrimpingOtherParameters.BendDown,
                        BendDownUnit = data.CrimpingOtherParameters.BendDownUnit,
                        Rolling = data.CrimpingOtherParameters.Rolling,
                        RollingUnit = data.CrimpingOtherParameters.RollingUnit,
                        Twist = data.CrimpingOtherParameters.Twist,
                        TwistUnit = data.CrimpingOtherParameters.TwistUnit,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedById=data.CreatedById,
                    };
                    await _crimpingStandardRepository.AddCrimpingOtherParameter(otherParams);
                }

                // 8. Save Crimping Dies Details
                if (data.CrimpingDiesDetails != null)
                {
                    var diesDetails = new T1C7CrimpingDiesDetail
                    {
                        TerminalId = terminalId,
                        TerminalNo = data.TerminalNo,
                        CrimpingDieNo_AnvilA = data.CrimpingDiesDetails.CrimpingDieNoAnvilA,
                        CrimpingDieNo_WireCrimperW = data.CrimpingDiesDetails.CrimpingDieNoWireCrimperW,
                        CrimpingDieNo_InsulationCrimperI = data.CrimpingDiesDetails.CrimpingDieNoInsulationCrimperI,
                        CrimpingDieNo_StabilizerCrimperQ = data.CrimpingDiesDetails.CrimpingDieNoStabilizerCrimperQ,
                        DiesCrimpingWidth_ConductorAnvilA = data.CrimpingDiesDetails.DiesCrimpingWidthConductorAnvilA,
                        DiesCrimpingWidth_InsulationAnvilA = data.CrimpingDiesDetails.DiesCrimpingWidthInsulationAnvilA,
                        DiesCrimpingWidth_WireCrimperW = data.CrimpingDiesDetails.DiesCrimpingWidthWireCrimperW,
                        DiesCrimpingWidth_InsulationCrimperI = data.CrimpingDiesDetails.DiesCrimpingWidthInsulationCrimperI,
                        ConductorDieThickness = data.CrimpingDiesDetails.ConductorDieThickness,
                        InsulationDieThickness = data.CrimpingDiesDetails.InsulationDieThickness,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedById=data.CreatedById,
                    };
                    await _crimpingStandardRepository.AddCrimpingDiesDetail(diesDetails);
                }

                // 9. Save Approval Details
                if (data.ApprovalDetails != null)
                {
                    var approval = new T1C8ApprovalDetail
                    {
                        TerminalId = terminalId,
                        TerminalNo = data.TerminalNo,
                        RevisionNo = data.ApprovalDetails.RevisionNo,
                        RevisionDate = data.ApprovalDetails.RevisionDate,
                        RevisionDetails = data.ApprovalDetails.RevisionDetails,
                        MadeBy = data.ApprovalDetails.MadeBy,
                        CheckedBy = data.ApprovalDetails.CheckedBy,
                        ApprovedBy = data.ApprovalDetails.ApprovedBy,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedById=data.CreatedById,
                    };
                    await _crimpingStandardRepository.AddApprovalDetail(approval);
                }

                // 10. Save Terminal Supplier Crimping Spec
                if (data.TerminalSupplierCrimpingSpec != null)
                {
                    // Parse supplier measurements
                    var supplierCcHeight = ParseMeasurementWithTolerance(data.TerminalSupplierCrimpingSpec.Ccheight);
                    var supplierIcHeight = ParseMeasurementWithTolerance(data.TerminalSupplierCrimpingSpec.Icheight);
                    var supplierCcWidth = ParseRange(data.TerminalSupplierCrimpingSpec.Ccwidth);
                    var supplierIcWidth = ParseRange(data.TerminalSupplierCrimpingSpec.Icwidth);
                    var wireSizeRange = ParseRange(data.TerminalSupplierCrimpingSpec.ApplicableWireSize);

                    var supplierSpec = new T1C9TerminalSupplierCrimpingSpec
                    {
                        TerminalId = terminalId,
                        TerminalNo = data.TerminalNo,
                        TerminalSupplierName = data.TerminalSupplierCrimpingSpec.TerminalSupplierName,
                        TerminalSupplierNumber = data.TerminalSupplierCrimpingSpec.TerminalSupplierNumber,
                        ApplicableWireSize = data.TerminalSupplierCrimpingSpec.ApplicableWireSize,
                        ApplicableWireSizeMin = wireSizeRange.Min,
                        ApplicableWireSizeMax = wireSizeRange.Max,
                        // InsulationCrimpShape = data.TerminalSupplierCrimpingSpec.InsulationCrimpShape,
                        InsulationCrimpShape = await GetCrimpingShapeId(data.CrimpingStandardDetails.InsulationCrimpShape),
                        CC_Height = data.TerminalSupplierCrimpingSpec.Ccheight,
                        CC_HeightStd = supplierCcHeight.Value,
                        CC_HeightTolerance = supplierCcHeight.Tolerance,
                        IC_Height = data.TerminalSupplierCrimpingSpec.Icheight,
                        IC_HeightStd = supplierIcHeight.Value,
                        IC_HeightTolerance = supplierIcHeight.Tolerance,
                        CC_Width = data.TerminalSupplierCrimpingSpec.Ccwidth,
                        CC_WidthMIn = supplierCcWidth.Min,
                        CC_WidthMax = supplierCcWidth.Max,
                        IC_Width = data.TerminalSupplierCrimpingSpec.Icwidth,
                        IC_WidthMin = supplierIcWidth.Min,
                        IC_WidthMax = supplierIcWidth.Max,
                        TensileForce_KGF = data.TerminalSupplierCrimpingSpec.TensileForceKgf,
                        TensileForce_N = data.TerminalSupplierCrimpingSpec.TensileForceN,
                        StandardAsperSupplier = data.TerminalSupplierCrimpingSpec.StandardAsperSupplier,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedById=data.CreatedById,
                    };
                    await _crimpingStandardRepository.AddTerminalSupplierCrimpingSpec(supplierSpec);
                }

                await transaction.CommitAsync();
                return terminalId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Helper classes for parsing measurements
        private class MeasurementWithTolerance
        {
            public decimal Value { get; set; }
            public decimal Tolerance { get; set; }
        }

        private class RangeMeasurement
        {
            public decimal Min { get; set; }
            public decimal Max { get; set; }
            public decimal Avg => (Min + Max) / 2;
        }

        // Helper methods for parsing different measurement formats
        private MeasurementWithTolerance ParseMeasurementWithTolerance(string measurement)
        {
            if (string.IsNullOrWhiteSpace(measurement))
                return new MeasurementWithTolerance();

            var parts = measurement.Split('±');
            if (parts.Length == 2 && decimal.TryParse(parts[0].Trim(), out var value) &&
                decimal.TryParse(parts[1].Trim(), out var tolerance))
            {
                return new MeasurementWithTolerance { Value = value, Tolerance = tolerance };
            }

            if (decimal.TryParse(measurement, out value))
            {
                return new MeasurementWithTolerance { Value = value };
            }

            return new MeasurementWithTolerance();
        }

        private RangeMeasurement ParseRange(string range)
        {
            if (string.IsNullOrWhiteSpace(range))
                return new RangeMeasurement();

            // Handle formats like "1.70 ~ 1.90" or "0.1-0.3"
            var rangeParts = range.Split(new[] { '~', '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (rangeParts.Length == 2 &&
                decimal.TryParse(rangeParts[0].Trim(), out var min) &&
                decimal.TryParse(rangeParts[1].Trim(), out var max))
            {
                return new RangeMeasurement { Min = min, Max = max };
            }

            // Handle single value
            if (decimal.TryParse(range, out var value))
            {
                return new RangeMeasurement { Min = value, Max = value };
            }

            return new RangeMeasurement();
        }

        /// <summary>
        /// Parses plant code from the plant string (e.g., "2025 -KAB" → 2025)
        /// </summary>
        /// <param name="plant">The plant string to parse</param>
        /// <returns>Parsed plant code or null if invalid</returns>
        private int? ParsePlantCode(string plant)
        {
            if (string.IsNullOrWhiteSpace(plant))
                return null;

            // Extract numeric part from formats like "2025 -KAB" or "2025"
            var numericPart = plant.Split('-')[0].Trim();

            if (int.TryParse(numericPart, out int plantCode))
            {
                return plantCode;
            }

            return null;
        }

    

        /// <summary>
        /// Gets the crimping shape ID from the shape name by using repository method
        /// </summary>
        /// <param name="shapeName">The shape name to lookup</param>
        /// <returns>ID of the crimping shape or null if not found</returns>
        private async Task<int?> GetCrimpingShapeId(string shapeName)
        {
            return await _crimpingStandardRepository.GetCrimpingShapeIdByName(shapeName);
        }

        public async Task<JObject> BulkUpload(IFormFile file)
        {
            var result = new JObject();
            if (file == null || file.Length == 0)
            {
                return ErrorResponseWrapper.CreateErrorResponse(
                    ErrorDescription.NoFileUploaded_1035,
                    400,
                    ErrorNumber.NoFileUploaded);
            }
            //if (userId <= 0)
            //{
            //    return ErrorResponseWrapper.CreateErrorResponse(
            //        ErrorDescription.CreatedByIdIsRequired_1009,
            //        400,
            //        ErrorNumber.CreatedByIdIsRequired);
            //}
            using var stream = file.OpenReadStream();
            var payload = ExcelHelper.ReadCrimpingStandardsFromExcel(stream);
            if( payload != null)
            {
                result["data"] = JArray.FromObject(payload);
            }
            else
            {
                result["data"] = null;
                return result;
            }
            return result;
        }
    }
}
