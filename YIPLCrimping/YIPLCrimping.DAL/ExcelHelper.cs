using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL
{
    public static class ExcelHelper
    {
        //public static List<MBuyer> ReadBuyersFromExcel(Stream fileStream)
        //{
        //    var buyers = new List<MBuyer>();

        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

        //    foreach (var row in rows)
        //    {
        //        var buyer = new MBuyer
        //        {
        //            Name = row.Cell(1).GetString().Trim(),
        //            Role = row.Cell(2).GetString().Trim(),
        //            EmpId = row.Cell(3).GetString().Trim(),
        //            PlantCode = int.TryParse(row.Cell(4).GetString(), out var plant) ? plant : (int?)null,
        //            BuyerEmailId = row.Cell(5).GetString().Trim(),
        //            BuyerGrade = row.Cell(6).GetString().Trim(),
        //            ReportingMgrId = row.Cell(7).GetString().Trim(),
        //            ReportingMgrEmailId = row.Cell(8).GetString().Trim(),
        //            ReportingMgrName = row.Cell(9).GetString().Trim(),
        //            IsActive = true,
        //            CreatedOn = DateTime.Now
        //        };

        //        buyers.Add(buyer);
        //    }

        //    return buyers;
        //}

        //public static List<MBusinessGroup> ReadBusinessGroupFromExcel(Stream fileStream)
        //{
        //    var datalist = new List<MBusinessGroup>();

        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

        //    foreach (var row in rows)
        //    {
        //        var data = new MBusinessGroup
        //        {
        //            BusinessGroup = row.Cell(1).GetString().Trim(),
        //            BusinessId = row.Cell(2).GetString().Trim(),
        //            Description = row.Cell(3).GetString().Trim(),
        //            CreatedOn = DateTime.Now,
        //            IsActive = true
        //        };

        //        datalist.Add(data);
        //    }

        //    return datalist;
        //}
        //public static List<BusinessCustomer> ReadBusinessCustomerFromExcel(Stream fileStream)
        //{
        //    var datalist = new List<BusinessCustomer>();

        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

        //    foreach (var row in rows)
        //    {
        //        string businessGroup = row.Cell(1).GetString().Trim();
        //        string businessIdText = row.Cell(2).GetString().Trim();
        //        string customerCode = row.Cell(3).GetString().Trim();
        //        string customerName = row.Cell(4).GetString().Trim();

        //        long? businessId = null;
        //        if (long.TryParse(businessIdText, out var parsedId))
        //        {
        //            businessId = parsedId;
        //        }

        //        var data = new BusinessCustomer
        //        {
        //            BusinessGroup = string.IsNullOrWhiteSpace(businessGroup) ? null : businessGroup,
        //            BussinessId = businessId,
        //            CustomerCode = string.IsNullOrWhiteSpace(customerCode) ? null : customerCode,
        //            CustomerName = string.IsNullOrWhiteSpace(customerName) ? null : customerName,
        //            CreatedOn = DateTime.Now,
        //            IsActive = true
        //        };

        //        datalist.Add(data);
        //    }

        //    return datalist;
        //}
        //public static List<MCustomer> ReadCustomerFromExcel(Stream fileStream)
        //{
        //    var customers = new List<MCustomer>();

        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

        //    foreach (var row in rows)
        //    {
        //        string customerName = row.Cell(1).GetString().Trim();
        //        string carlinerCode = row.Cell(2).GetString().Trim();
        //        string businessId = row.Cell(3).GetString().Trim();

        //        var customer = new MCustomer
        //        {
        //            CustomerName = string.IsNullOrWhiteSpace(customerName) ? null : customerName,
        //            CarlinerCode = string.IsNullOrWhiteSpace(carlinerCode) ? null : carlinerCode,
        //            BusinessId = string.IsNullOrWhiteSpace(businessId) ? null : businessId,
        //        };

        //        customers.Add(customer);
        //    }

        //    return customers;
        //}

        //public static List<MWireSize> ReadWireSizeFromExcel(Stream fileStream)
        //{
        //    var wireSizes = new List<MWireSize>();

        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

        //    foreach (var row in rows)
        //    {
        //        var wireSize = new MWireSize
        //        {
        //            WireSizeCode = row.Cell(1).GetString().Trim(),
        //            WireSize = row.Cell(2).GetValue<decimal>(),
        //            IsActive = true
        //        };
        //        wireSizes.Add(wireSize);
        //    }

        //    return wireSizes;
        //}

        public static List<MWireSize> ReadWireSizeFromExcel(Stream fileStream)
        {
            var wireSizes = new List<MWireSize>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var wireSizeCell = row.Cell(2);
                decimal wireSizeValue = 0;

                if (!string.IsNullOrWhiteSpace(wireSizeCell.GetString()))
                {
                    wireSizeValue = wireSizeCell.GetValue<decimal>();
                }

                var wireSize = new MWireSize
                {
                    WireSizeCode = row.Cell(1).GetString().Trim(),
                    WireSize = wireSizeValue,
                    IsActive = true
                };
                wireSizes.Add(wireSize);
            }

            return wireSizes;
        }


        /// <summary>
        /// Reads wire types from Excel stream.
        /// </summary>
        public static List<MWireType> ReadWireTypeFromExcel(Stream fileStream)
        {
            var wireTypes = new List<MWireType>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var wireTypeCodeStr = row.Cell(1).GetString().Trim();
                var wireTypeCode = string.IsNullOrWhiteSpace(wireTypeCodeStr) ? null : wireTypeCodeStr;

                var wireType = new MWireType
                {
                    WireTypeCode = wireTypeCode,
                    WireTypeName = row.Cell(2).GetString().Trim(),
                    IsActive = true // Default value
                };

                wireTypes.Add(wireType);
            }

            return wireTypes;
        }

        /// <summary>
        /// Reads suppliers from Excel stream.
        /// </summary>
        public static List<MSupplier> ReadSupplierFromExcel(Stream fileStream)
        {
            var suppliers = new List<MSupplier>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var supplierCodeStr = row.Cell(1).GetString().Trim();
                var supplierCode = string.IsNullOrWhiteSpace(supplierCodeStr) ? null : supplierCodeStr;

                var supplier = new MSupplier
                {
                    SupplierCode = supplierCode,
                    SupplierName = row.Cell(2).GetString().Trim(),
                };

                suppliers.Add(supplier);
            }

            return suppliers;
        }

        public static List<WireCombinationDetails> ReadWireCombinationFromExcel(Stream fileStream)
        {
            var wires = new List<WireCombinationDetails>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

            foreach (var row in rows)
            {
                var wireCode = row.Cell(1).GetString().Trim();
                var wireType = row.Cell(2).GetString().Trim();
                var wireSizeCode = row.Cell(3).GetString().Trim();
                var wireSizeStr = row.Cell(4).GetString().Trim();

                // Try parse wire size if it's a number
                decimal? wireSize = null;
                if (decimal.TryParse(wireSizeStr, out var parsedSize))
                    wireSize = parsedSize;

                var wire = new WireCombinationDetails
                {
                    WireCode = string.IsNullOrWhiteSpace(wireCode) ? null : wireCode,
                    WireType = string.IsNullOrWhiteSpace(wireType) ? null : wireType,
                    WireSizeCode = string.IsNullOrWhiteSpace(wireSizeCode) ? null : wireSizeCode,
                    WireSize = wireSize
                };

                wires.Add(wire);
            }

            return wires;
        }

        public static List<MPlant> ReadPlantFromExcel(Stream fileStream)
        {
            var plants = new List<MPlant>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var plantCodeStr = row.Cell(1).GetString().Trim();
                var plantCode = string.IsNullOrWhiteSpace(plantCodeStr) ? null : plantCodeStr;

                var plant = new MPlant
                {
                    PlantCode = plantCode,
                    PlantName = row.Cell(2).GetString().Trim(),
                    City = row.Cell(3).GetString().Trim()
                };

                plants.Add(plant);
            }

            return plants;
        }

        public static List<MCustomer> ReadCustomerFromExcel(Stream fileStream)
        {
            var customers = new List<MCustomer>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var customerCodeStr = row.Cell(1).GetString().Trim();
                var customerCode = string.IsNullOrWhiteSpace(customerCodeStr) ? null : customerCodeStr;

                var customer = new MCustomer
                {
                    CustomerCode = customerCode,
                    CustomerName = row.Cell(2).GetString().Trim(),
                };

                customers.Add(customer);
            }

            return customers;
        }

        public static List<MDepartment> ReadDepartmentFromExcel(Stream fileStream)
        {
            var departments = new List<MDepartment>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();

            var range = worksheet.RangeUsed();
            if (range == null)
            {
                return departments;
            }

            var rows = range.RowsUsed().Skip(1); // Skip header row

            foreach (var row in rows)
            {
                var deptName = row.Cell(1).GetString().Trim();

                if (!string.IsNullOrWhiteSpace(deptName))
                {
                    var department = new MDepartment
                    {
                        DeptName = deptName,
                        IsActive = true, // Default to active
                        CreatedDate = DateTime.Now
                    };

                    departments.Add(department);
                }
            }

            return departments;
        }

        public static List<MMachine> ReadMachineFromExcel(Stream fileStream)
        {
            var machines = new List<MMachine>();
            using var workbook = new XLWorkbook(fileStream);
            var ws = workbook.Worksheets.First();
            var rows = ws.RangeUsed().RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                machines.Add(new MMachine
                {
                    MachineName = row.Cell(1).GetString().Trim(),
                    MachineCost = decimal.TryParse(row.Cell(2).GetString(), out var cost) ? cost : 0,
                    PlantId = int.TryParse(row.Cell(3).GetString(), out var pid) ? pid : 0
                });
            }

            return machines;
        }

        //public static List<MSupplier> ReadSupplierFromExcel(Stream fileStream)
        //{
        //    var suppliers = new List<MSupplier>();

        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

        //    foreach (var row in rows)
        //    {
        //        // Safely parse SupplierCode
        //        int? supplierCode = null;
        //        var supplierCodeStr = row.Cell(2).GetString().Trim();
        //        if (int.TryParse(supplierCodeStr, out var code))
        //            supplierCode = code;

        //        // Safely parse PostalCode
        //        int? postalCode = int.TryParse(row.Cell(10).GetString().Trim(), out var pCode) ? pCode : null;

        //        var supplier = new MSupplier
        //        {
        //            SupplierName = row.Cell(1).GetString().Trim(),
        //            SupplierCode = supplierCode,
        //            ShortName = row.Cell(3).GetString().Trim(),
        //            City = row.Cell(4).GetString().Trim(),
        //            State = row.Cell(5).GetString().Trim(),
        //            Country = row.Cell(6).GetString().Trim(),
        //            Address = row.Cell(7).GetString().Trim(),
        //            Commodity = row.Cell(8).GetString().Trim(),
        //            SupplierType = row.Cell(9).GetString().Trim(),
        //            PostalCode = postalCode
        //        };

        //        suppliers.Add(supplier);
        //    }

        //    return suppliers;
        //}

        //public static List<MProgram> ReadProgramFromExcel(Stream fileStream)
        //{
        //    var programs = new List<MProgram>();

        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

        //    foreach (var row in rows)
        //    {
        //        var program = new MProgram
        //        {
        //            Name = row.Cell(1).GetString().Trim(),
        //            CurrentTerm = row.Cell(2).GetString().Trim(),
        //            ProjectRank = row.Cell(3).GetString().Trim(),
        //            BusinessGroupCode = row.Cell(4).GetString().Trim(),
        //            BusinessGroupName = row.Cell(5).GetString().Trim(),
        //            PlantId = TryGetNullableInt(row.Cell(6)),
        //            CustomerCode = row.Cell(7).GetString().Trim(),
        //            CustomerName = row.Cell(8).GetString().Trim(),
        //            VehicleType = row.Cell(9).GetString().Trim(),
        //            ProductType = row.Cell(10).GetString().Trim(),
        //            AnnualVolume = TryGetNullableInt(row.Cell(11)),
        //            ProjectLifeCycle = row.Cell(12).GetString().Trim(),
        //            ProjectStatus = row.Cell(13).GetString().Trim(),
        //            CustomerLocation = row.Cell(14).GetString().Trim(),
        //            YIPLLocation = row.Cell(15).GetString().Trim(),
        //            IOCode = TryGetNullableInt(row.Cell(16)),
        //            IOCodeBudget = TryGetNullableInt(row.Cell(17)),
        //            ProjectDate = TryGetDate(row.Cell(18)),
        //            YIPLSOP  = TryGetDate(row.Cell(19)),
        //            CustomerSOP = TryGetDate(row.Cell(20)),
        //            BusinessId = row.Cell(21).GetString().Trim(),
        //            ProjectType = row.Cell(22).GetString().Trim(),
        //            SubProjectType = row.Cell(23).GetString().Trim(),
        //            TotalParts = TryGetNullableInt(row.Cell(24)),
        //            EOPDate = TryGetDate(row.Cell(25)),
        //            AnnualBusinessValue_MINR = row.Cell(26).GetString().Trim(),
        //            ProgramCode = row.Cell(27).GetString().Trim(),
        //            BuyerId = TryGetNullableInt(row.Cell(28)),
        //            CustomerId = row.Cell(29).GetString().Trim(),
        //            Image = row.Cell(30).GetString().Trim(),
        //            Business = row.Cell(31).GetString().Trim()
        //        };

        //        programs.Add(program);
        //    }

        //    return programs;
        //}
        //public static List<MProgramPart> ReadProgramPartFromExcel(Stream fileStream)
        //{
        //    var parts = new List<MProgramPart>();
        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

        //    foreach (var row in rows)
        //    {
        //        var part = new MProgramPart
        //        {
        //            ProgramCode = row.Cell(1).GetValue<int?>(),
        //            ProgramName = row.Cell(2).GetString().Trim(),
        //            SevenPartNumber = row.Cell(3).GetValue<decimal?>(),
        //            XPartNumber = row.Cell(4).GetValue<decimal?>(),
        //            NewOrCommon = row.Cell(5).GetString().Trim(),
        //            PartRev = row.Cell(6).GetString().Trim(),
        //            PartName = row.Cell(7).GetString().Trim(),
        //            MaterialSpecification = row.Cell(8).GetString().Trim(),
        //            PartThickness = row.Cell(9).GetValue<decimal?>(),
        //            Weight = row.Cell(10).GetValue<decimal?>(),
        //            Color = row.Cell(11).GetString().Trim(),
        //            VersionDetails = row.Cell(12).GetString().Trim(),
        //            Commodity = row.Cell(13).GetString().Trim(),
        //            SubCommodity = row.Cell(14).GetString().Trim(),
        //            CavityTool = row.Cell(15).GetString().Trim(),
        //            PurchaseBuyerId = row.Cell(16).GetValue<int?>(),
        //            PurchaseBuyerName = row.Cell(17).GetString().Trim(),
        //            EnggReleaseRepId = row.Cell(18).GetValue<int?>(),
        //            EnggReleaseRepName = row.Cell(19).GetString().Trim(),
        //            EnggReleasePlanDate = TrySafeDate(row.Cell(20)),
        //            NominatedToolSupplierId = row.Cell(21).GetValue<int?>(),
        //            NominatedToolSupplierName = row.Cell(22).GetString().Trim(),
        //            ToolSupplierNomPlanDate = DateTime.UtcNow,
        //            ToolingRespId = row.Cell(24).GetValue<int?>(),
        //            ToolingRespName = row.Cell(25).GetString().Trim(),
        //            DFMPlanDate = DateTime.UtcNow,
        //            T0PlanDate = DateTime.UtcNow,
        //            DVPPlanDate = DateTime.UtcNow,
        //            T1PlanDate = DateTime.UtcNow,
        //            OTPlanDate = DateTime.UtcNow,
        //            ProductionSupplierCode = row.Cell(31).GetValue<int?>(),
        //            ProductionSupplierName = row.Cell(32).GetString().Trim(),
        //            ProductionSupplierNomineePlanDate = DateTime.UtcNow,
        //            ToolTransferETDPlanDate = DateTime.UtcNow,
        //            ToolTransferETAPlanDate = DateTime.UtcNow,
        //            ToolTransferProdSuppPlanDate = DateTime.UtcNow,
        //            OTOPPlanaDate = DateTime.UtcNow,
        //            PPAPRepId = row.Cell(38).GetValue<int?>(),
        //            PPAPRepName = row.Cell(39).GetString().Trim(),
        //            PPAPlan = DateTime.UtcNow,
        //            HandoverPlan = DateTime.UtcNow
        //        };

        //        parts.Add(part);
        //    }

        //    return parts;
        //}
        //public static List<ProgramOpenIssue> ReadProgramOpenIssuesFromExcel(Stream fileStream)
        //{
        //    var issues = new List<ProgramOpenIssue>();

        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

        //    foreach (var row in rows)
        //    {
        //        var issue = new ProgramOpenIssue
        //        {
        //            BusinessGroup = row.Cell(1).GetValue<int?>(),
        //            ProjectCode = row.Cell(2).GetString().Trim(),
        //            ProjectName = row.Cell(3).GetString().Trim(),
        //            CustomerCode = row.Cell(4).GetValue<int?>(),
        //            CustomerName = row.Cell(5).GetString().Trim(),
        //            EntryDate = TrySafeDate(row.Cell(6)),
        //            UpcomingEvent = row.Cell(7).GetString().Trim(),
        //            OpenIssueTitle = row.Cell(8).GetString().Trim(),
        //            IssueDescription = row.Cell(9).GetString().Trim(),
        //            IssueFunction = row.Cell(10).GetString().Trim(),
        //            OpenIssue = row.Cell(11).GetString().Trim(),
        //            IssueResolutionAction = row.Cell(12).GetString().Trim(),
        //            Owner = row.Cell(13).GetString().Trim(),
        //            OwnerId = row.Cell(14).GetString().Trim(),
        //            ActionDate = TrySafeDate(row.Cell(15)),
        //            Remarks = row.Cell(16).GetString().Trim(),
        //            ProgramId = row.Cell(17).GetValue<int?>(),
        //            FunctionId = row.Cell(18).GetValue<int?>(),
        //            Flag = row.Cell(19).GetString().Trim(),
        //            ClosureDate = TrySafeDate(row.Cell(20))
        //        };

        //        issues.Add(issue);
        //    }

        //    return issues;
        //}

        private static DateTime? TrySafeDate(IXLCell cell)
        {
            try
            {
                if (cell.DataType == XLDataType.DateTime)
                    return cell.GetDateTime();

                if (cell.DataType == XLDataType.Number)
                    return DateTime.FromOADate(cell.GetDouble());

                return DateTime.TryParse(cell.GetString().Trim(), out var dt) ? dt : null;
            }
            catch
            {
                return null;
            }
        }

        public static List<MCrimpingShape> ReadShapeFromExcel(Stream fileStream)
        {
            var shapes = new List<MCrimpingShape>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

            foreach (var row in rows)
            {
                var name = row.Cell(1).GetString().Trim();
                var imageUrl = row.Cell(2).GetString().Trim(); // Assuming ImageUrl is in column 2

                if (string.IsNullOrWhiteSpace(name))
                    continue; // Skip row if Shape Name is empty

                var shape = new MCrimpingShape
                {
                    Name = name,
                    ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl
                };

                shapes.Add(shape);
            }

            return shapes;
        }

        public static string GetFriendlyFileType(string extension)
        {
            extension = extension.ToLowerInvariant();

            return extension switch
            {
                "xlsx" or "xls" => "Excel",
                "csv" => "CSV",
                "pdf" => "Document",
                "docx" or "doc" => "Document",
                "jpg" or "jpeg" or "png" or "bmp" => "Picture",
                "txt" => "Text",
                _ => "Unknown"
            };
        }

        #region commonDataSanitizingMethod

        private static int? TryGetNullableInt(IXLCell cell)
        {
            var str = cell.GetString().Trim();
            return int.TryParse(str, out var val) ? val : null;
        }

        private static decimal? TryGetNullableDecimal(IXLCell cell)
        {
            var str = cell.GetString().Trim();
            return decimal.TryParse(str, out var val) ? val : null;
        }

        private static DateTime? TryGetDate(IXLCell cell)
        {
            try
            {
                if (cell.DataType == XLDataType.DateTime)
                    return cell.GetDateTime();

                if (cell.DataType == XLDataType.Number)
                    return DateTime.FromOADate(cell.GetDouble());

                return DateTime.TryParse(cell.GetString().Trim(), out var dt) ? dt : null;
            }
            catch
            {
                return null;
            }
        }

        //public static List<CrimpingStandardRequestVM> ReadCrimpingStandardsFromExcel(Stream fileStream)
        //{
        //    var result = new List<CrimpingStandardRequestVM>();

        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(2); // skip header
        //    var header = worksheet.Row(3);
        //    var colDict = GetColumnDictionary(header);

        //    int crimpingStandardDetailColumnIndex = -1;
        //    var crimpingStandardHeader = worksheet.Row(2);
        //    for (int i = 0; i < crimpingStandardHeader.CellCount(); i++)
        //    {
        //        Console.WriteLine();
        //        if (crimpingStandardHeader.Cell(i + 1).GetString().ToLower().Contains("crimping standard detail", StringComparison.OrdinalIgnoreCase))
        //        {
        //            crimpingStandardDetailColumnIndex = i + 1;
        //            break;
        //        }
        //    }
        //    var crimpingStandardDetailsModel = new CrimpingStandardRequestVM();
        //    // If "Crimping Standard Detail" column exists, extract data
        //    if (crimpingStandardDetailColumnIndex != -1)
        //    {
        //        foreach (var row in rows)
        //        {
        //            crimpingStandardDetailsModel = new CrimpingStandardRequestVM
        //            {
        //                // Other fields...
        //                CrimpingStandardDetails = new CrimpingStandardDetailDto
        //                {
        //                    ApplicableWireSize = GetCell(row, colDict, "Applicable wire size"),
        //                    InsulationCrimpShape = GetCell(row, colDict, "Insulation crimp Shape"),
        //                    Ccheight = GetCell(row, colDict, "CC/H"),
        //                    Icheight = GetCell(row, colDict, "IC/H"),
        //                    Ccwidth = GetCell(row, colDict, "CC/W"),
        //                    Icwidth = GetCell(row, colDict, "IC/W"),
        //                    TensileForceKgf = TryParseDecimal(GetCell(row, colDict, "Tensile force (KGF)")),
        //                    TensileForceN = TryParseDecimal(GetCell(row, colDict, "Tensile force (N)")),
        //                    PillShape = TryParseDecimal(GetCell(row, colDict, "Pill Shape (FIG)")),
        //                    Soldering = TryParseDecimal(GetCell(row, colDict, "Soldering"))
        //                },
        //            };

        //            //result.Add(model);
        //        }
        //    }
        //    foreach (var row in rows)
        //    {
        //        var model = new CrimpingStandardRequestVM
        //        {
        //            Customer = GetCell(row, colDict, "Customer"),
        //            Flag = GetCell(row, colDict, "Flag"),
        //            Plant = GetCell(row, colDict, "plant"),
        //            RegistrationNo = GetCell(row, colDict, "REGISTRATION NO. OF\nYazaki Product Engineering Standard "),
        //            ManufacturinCrimpNo = GetCell(row, colDict, "MANUFACTURING CRIMPING STANDARD Control Numbe"),
        //            TerminalNo = GetCell(row, colDict, "Terminal Number"),
        //            CommonTerminalNo = GetCell(row, colDict, "Common Terminal"),
        //            TerminalName = GetCell(row, colDict, "Termainal Name"),
        //            TerminalThickness = TryParseDecimal(GetCell(row, colDict, "Terminal Thickness / SHIELD RING THICKNESS")),

        //            Accessories = new AccessoryDto
        //            {
        //                ShieldNo = GetCell(row, colDict, "ACCESSORYPARTS /SHIEID RING NO./ Shrink Tube"),
        //                RubberSealPosition = TryParseDecimal(GetCell(row, colDict, "Rubber seal position"))
        //            },

        //            ApplicatorDetails = new ApplicatorDetailDto
        //            {
        //                Machine = GetCell(row, colDict, "Machine"),
        //                Feed = GetCell(row, colDict, "FEED")
        //            },

        //            WireCombinations = ExtractWireCombinations(row, colDict),

        //            //CrimpingStandardDetails = new CrimpingStandardDetailDto
        //            //{
        //            //    ApplicableWireSize = GetCell(row, colDict, "Applicable wire size"),
        //            //    InsulationCrimpShape = GetCell(row, colDict, "Insulation crimp Shape"),
        //            //    Ccheight = GetCell(row, colDict, "CC/H"),
        //            //    Icheight = GetCell(row, colDict, "IC/H"),
        //            //    Ccwidth = GetCell(row, colDict, "CC/W"),
        //            //    Icwidth = GetCell(row, colDict, "IC/W"),
        //            //    TensileForceKgf = TryParseDecimal(GetCell(row, colDict, "Tensile force (KGF)")),
        //            //    TensileForceN = TryParseDecimal(GetCell(row, colDict, "Tensile force (N)")),
        //            //    PillShape = TryParseDecimal(GetCell(row, colDict, "Pill Shape (FIG)")),
        //            //    Soldering = TryParseDecimal(GetCell(row, colDict, "Soldering"))
        //            //},

        //            CrimpingStandardDetails = crimpingStandardDetailsModel.CrimpingStandardDetails ?? new CrimpingStandardDetailDto(),

        //            StripingDetails = new StripingDetailDto
        //            {
        //                MiddelStriping = TryParseDecimal(GetCell(row, colDict, "Middel Striping")),
        //                MiddelStrippingUpperLimit = TryParseDecimal(GetCell(row, colDict, "Middel Striping Upper limit")),
        //                MiddelStrippingLowerLimit = TryParseDecimal(GetCell(row, colDict, "Middel Striping Lower limit")),
        //                EndStriping = TryParseDecimal(GetCell(row, colDict, "End Striping")),
        //                EndStripingUpperLimit = TryParseDecimal(GetCell(row, colDict, "End Striping Upper limit")),
        //                EndStripingLowerLimit = TryParseDecimal(GetCell(row, colDict, "End Striping Lower limit"))
        //            },

        //            CrimpingOtherParameters = new CrimpingOtherParameterDto
        //            {
        //                FrontCuttingCarry = GetCell(row, colDict, "Front Cutting Carry (mm)"),
        //                RearCuttingCarry = GetCell(row, colDict, "Rear Cutting Carry (mm)"),
        //                BrushLength = GetCell(row, colDict, "Brush Length /Wire (mm)"),
        //                FrontBellMouth = GetCell(row, colDict, "Front Bell mouth (mm)"),
        //                RearBellMouth = GetCell(row, colDict, "Rear Bell mouth  (mm)"),
        //                BendUp = TryParseDecimal(GetCell(row, colDict, "Bend Up (°/ mm )")),
        //                BendUpUnit = GetCell(row, colDict, "Unit"),
        //                BendDown = TryParseDecimal(GetCell(row, colDict, "Bend Down (°/ mm )")),
        //                BendDownUnit = GetCell(row, colDict, "Unit"),
        //                Rolling = TryParseDecimal(GetCell(row, colDict, "Rolling (° / mm)")),
        //                RollingUnit = GetCell(row, colDict, "Unit"),
        //                Twist = TryParseDecimal(GetCell(row, colDict, "Twist  (° / mm )")),
        //                TwistUnit = GetCell(row, colDict, "Unit")
        //            },

        //            CrimpingDiesDetails = new CrimpingDiesDetailDto
        //            {
        //                CrimpingDieNoAnvilA = GetCell(row, colDict, "CRIMPING DIE NO Anvil(A)"),
        //                CrimpingDieNoWireCrimperW = GetCell(row, colDict, "CRIMPING DIE NO               Wire crimper(W)"),
        //                CrimpingDieNoInsulationCrimperI = GetCell(row, colDict, "CRIMPING DIE NO Insulation crimper (I)"),
        //                CrimpingDieNoStabilizerCrimperQ = TryParseDecimal(GetCell(row, colDict, "CRIMPING DIE NO Stabilizer Crimper(Q) ")),
        //                DiesCrimpingWidthConductorAnvilA = TryParseDecimal(GetCell(row, colDict, "Dies Crimping Width Conductor Anvil(A)")),
        //                DiesCrimpingWidthInsulationAnvilA = TryParseDecimal(GetCell(row, colDict, "Dies Crimping Width Insulation  Anvil(A)")),
        //                DiesCrimpingWidthWireCrimperW = TryParseDecimal(GetCell(row, colDict, "Dies Crimping Width Wire crimper (W)")),
        //                DiesCrimpingWidthInsulationCrimperI = TryParseDecimal(GetCell(row, colDict, "Dies Crimping Width Insulation crimper (I)")),
        //                ConductorDieThickness = TryParseDecimal(GetCell(row, colDict, "Conductoer Die Thickness")),
        //                InsulationDieThickness = TryParseDecimal(GetCell(row, colDict, "Insulation Die Thickness"))
        //            },

        //            TerminalSupplierCrimpingSpec = new TerminalSupplierCrimpingSpecDto
        //            {
        //                TerminalSupplierName = GetCell(row, colDict, "Terminal Supplier Name"),
        //                TerminalSupplierNumber = GetCell(row, colDict, "Terminal Supplier No."),
        //                ApplicableWireSize = GetCell(row, colDict, "Applicable wire size"),
        //                InsulationCrimpShape = GetCell(row, colDict, "Insulation crimp Shape"),
        //                Ccheight = GetCell(row, colDict, "CC/H"),
        //                Icheight = GetCell(row, colDict, "IC/H"),
        //                Ccwidth = GetCell(row, colDict, "CC/W"),
        //                Icwidth = GetCell(row, colDict, "IC/W"),
        //                TensileForceKgf = TryParseDecimal(GetCell(row, colDict, "Tensile force (KGF)")),
        //                TensileForceN = TryParseDecimal(GetCell(row, colDict, "Tensile force (N)")),
        //                StandardAsperSupplier = GetCell(row, colDict, "STD As per Supplier Specification")
        //            }
        //        };

        //        result.Add(model);
        //    }

        //    return result;
        //}




        //private static Dictionary<string, int> GetColumnDictionary(IXLRow headerRow)
        //{
        //    var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        //    int colIndex = 1;

        //    foreach (var cell in headerRow.Cells())
        //    {
        //        var name = cell.GetString().Trim();
        //        if (!string.IsNullOrEmpty(name))
        //            dict[name] = colIndex;

        //        colIndex++;
        //    }

        //    return dict;
        //}

        //private static string GetCell(IXLRangeRow row, Dictionary<string, int> colDict, string columnName)
        //{
        //    return colDict.TryGetValue(columnName, out int col) ? row.Cell(col).GetString().Trim() : null;
        //}

        //private static decimal? TryParseDecimal(string input)
        //{
        //    if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        //        return result;
        //    return null;
        //}

        //private static List<WireCombinationDto> ExtractWireCombinations(IXLRangeRow row, Dictionary<string, int> colDict)
        //{
        //    var combinations = new List<WireCombinationDto>();

        //    for (int i = 1; i <= 13; i++)
        //    {
        //        var wireCode = GetCell(row, colDict, $"Wire Type Code {i}");
        //        var wireType = GetCell(row, colDict, $"Wire Type {i}");
        //        var wireSizeCode = GetCell(row, colDict, $"Wire Size Code {i}");
        //        var wireSize = GetCell(row, colDict, $"Wire Size {i}");

        //        if (!string.IsNullOrWhiteSpace(wireCode) ||
        //            !string.IsNullOrWhiteSpace(wireType) ||
        //            !string.IsNullOrWhiteSpace(wireSizeCode) ||
        //            !string.IsNullOrWhiteSpace(wireSize))
        //        {
        //            combinations.Add(new WireCombinationDto
        //            {
        //                WireCode = wireCode,
        //                WireType = wireType,
        //                WireSizeCode = wireSizeCode,
        //                WireSize = wireSize
        //            });
        //        }
        //    }

        //    return combinations;
        //}
        #region Last Code 2.40
        //public static List<CrimpingStandardRequestVM> ReadCrimpingStandardsFromExcel(Stream fileStream)
        //{
        //    var result = new List<CrimpingStandardRequestVM>();

        //    using var workbook = new XLWorkbook(fileStream);
        //    var worksheet = workbook.Worksheets.First();
        //    var rows = worksheet.RangeUsed().RowsUsed().Skip(2); // skip header
        //    var header = worksheet.Row(3);
        //    var colDict = GetColumnDictionary(header);

        //    // Locate the "Crimping Standard Detail" column
        //    int crimpingStandardDetailColumnIndex = -1;
        //    var crimpingStandardHeader = worksheet.Row(2);
        //    for (int i = 0; i < crimpingStandardHeader.CellCount(); i++)
        //    {
        //        if (crimpingStandardHeader.Cell(i + 1).GetString().ToLower().Contains("crimping standard detail", StringComparison.OrdinalIgnoreCase))
        //        {
        //            crimpingStandardDetailColumnIndex = i + 1;
        //            break;
        //        }
        //    }

        //    foreach (var row in rows)
        //    {
        //        var crimpingStandardDetailsModel = new CrimpingStandardDetailDto
        //        {
        //            ApplicableWireSize = GetCell(row, colDict, "Applicable wire size"),
        //            InsulationCrimpShape = GetCell(row, colDict, "Insulation crimp Shape"),
        //            Ccheight = GetCell(row, colDict, "CC/H"),
        //            Icheight = GetCell(row, colDict, "IC/H"),
        //            Ccwidth = GetCell(row, colDict, "CC/W"),
        //            Icwidth = GetCell(row, colDict, "IC/W"),
        //            TensileForceKgf = TryParseDecimal(GetCell(row, colDict, "Tensile force (KGF)")),
        //            TensileForceN = TryParseDecimal(GetCell(row, colDict, "Tensile force (N)")),
        //            PillShape = TryParseDecimal(GetCell(row, colDict, "Pill Shape (FIG)")),
        //            Soldering = TryParseDecimal(GetCell(row, colDict, "Soldering"))
        //        };

        //        var model = new CrimpingStandardRequestVM
        //        {
        //            Customer = GetCell(row, colDict, "Customer"),
        //            Flag = GetCell(row, colDict, "Flag"),
        //            Plant = GetCell(row, colDict, "plant"),
        //            RegistrationNo = GetCell(row, colDict, "REGISTRATION NO. OF\nYazaki Product Engineering Standard "),
        //            ManufacturinCrimpNo = GetCell(row, colDict, "MANUFACTURING CRIMPING STANDARD Control Numbe"),
        //            TerminalNo = GetCell(row, colDict, "Terminal Number"),
        //            CommonTerminalNo = GetCell(row, colDict, "Common Terminal"),
        //            TerminalName = GetCell(row, colDict, "Termainal Name"),
        //            TerminalThickness = TryParseDecimal(GetCell(row, colDict, "Terminal Thickness / SHIELD RING THICKNESS")),

        //            Accessories = new AccessoryDto
        //            {
        //                ShieldNo = GetCell(row, colDict, "ACCESSORYPARTS /SHIEID RING NO./ Shrink Tube"),
        //                RubberSealPosition = TryParseDecimal(GetCell(row, colDict, "Rubber seal position"))
        //            },

        //            ApplicatorDetails = new ApplicatorDetailDto
        //            {
        //                Machine = GetCell(row, colDict, "Machine"),
        //                Feed = GetCell(row, colDict, "FEED")
        //            },

        //            WireCombinations = ExtractWireCombinations(row, colDict),

        //            // Here, you assign the crimpingStandardDetailsModel directly
        //            CrimpingStandardDetails = crimpingStandardDetailsModel,

        //            StripingDetails = new StripingDetailDto
        //            {
        //                MiddelStriping = TryParseDecimal(GetCell(row, colDict, "Middel Striping")),
        //                MiddelStrippingUpperLimit = TryParseDecimal(GetCell(row, colDict, "Middel Striping Upper limit")),
        //                MiddelStrippingLowerLimit = TryParseDecimal(GetCell(row, colDict, "Middel Striping Lower limit")),
        //                EndStriping = TryParseDecimal(GetCell(row, colDict, "End Striping")),
        //                EndStripingUpperLimit = TryParseDecimal(GetCell(row, colDict, "End Striping Upper limit")),
        //                EndStripingLowerLimit = TryParseDecimal(GetCell(row, colDict, "End Striping Lower limit"))
        //            },

        //            CrimpingOtherParameters = new CrimpingOtherParameterDto
        //            {
        //                FrontCuttingCarry = GetCell(row, colDict, "Front Cutting Carry (mm)"),
        //                RearCuttingCarry = GetCell(row, colDict, "Rear Cutting Carry (mm)"),
        //                BrushLength = GetCell(row, colDict, "Brush Length /Wire (mm)"),
        //                FrontBellMouth = GetCell(row, colDict, "Front Bell mouth (mm)"),
        //                RearBellMouth = GetCell(row, colDict, "Rear Bell mouth  (mm)"),
        //                BendUp = TryParseDecimal(GetCell(row, colDict, "Bend Up (°/ mm )")),
        //                BendUpUnit = GetCell(row, colDict, "Unit"),
        //                BendDown = TryParseDecimal(GetCell(row, colDict, "Bend Down (°/ mm )")),
        //                BendDownUnit = GetCell(row, colDict, "Unit"),
        //                Rolling = TryParseDecimal(GetCell(row, colDict, "Rolling (° / mm)")),
        //                RollingUnit = GetCell(row, colDict, "Unit"),
        //                Twist = TryParseDecimal(GetCell(row, colDict, "Twist  (° / mm )")),
        //                TwistUnit = GetCell(row, colDict, "Unit")
        //            },

        //            CrimpingDiesDetails = new CrimpingDiesDetailDto
        //            {
        //                CrimpingDieNoAnvilA = GetCell(row, colDict, "CRIMPING DIE NO Anvil(A)"),
        //                CrimpingDieNoWireCrimperW = GetCell(row, colDict, "CRIMPING DIE NO               Wire crimper(W)"),
        //                CrimpingDieNoInsulationCrimperI = GetCell(row, colDict, "CRIMPING DIE NO Insulation crimper (I)"),
        //                CrimpingDieNoStabilizerCrimperQ = TryParseDecimal(GetCell(row, colDict, "CRIMPING DIE NO Stabilizer Crimper(Q) ")),
        //                DiesCrimpingWidthConductorAnvilA = TryParseDecimal(GetCell(row, colDict, "Dies Crimping Width Conductor Anvil(A)")),
        //                DiesCrimpingWidthInsulationAnvilA = TryParseDecimal(GetCell(row, colDict, "Dies Crimping Width Insulation  Anvil(A)")),
        //                DiesCrimpingWidthWireCrimperW = TryParseDecimal(GetCell(row, colDict, "Dies Crimping Width Wire crimper (W)")),
        //                DiesCrimpingWidthInsulationCrimperI = TryParseDecimal(GetCell(row, colDict, "Dies Crimping Width Insulation crimper (I)")),
        //                ConductorDieThickness = TryParseDecimal(GetCell(row, colDict, "Conductoer Die Thickness")),
        //                InsulationDieThickness = TryParseDecimal(GetCell(row, colDict, "Insulation Die Thickness"))
        //            },

        //            TerminalSupplierCrimpingSpec = new TerminalSupplierCrimpingSpecDto
        //            {
        //                TerminalSupplierName = GetCell(row, colDict, "Terminal Supplier Name"),
        //                TerminalSupplierNumber = GetCell(row, colDict, "Terminal Supplier No."),
        //                ApplicableWireSize = GetCell(row, colDict, "Applicable wire size"),
        //                InsulationCrimpShape = GetCell(row, colDict, "Insulation crimp Shape"),
        //                Ccheight = GetCell(row, colDict, "CC/H"),
        //                Icheight = GetCell(row, colDict, "IC/H"),
        //                Ccwidth = GetCell(row, colDict, "CC/W"),
        //                Icwidth = GetCell(row, colDict, "IC/W"),
        //                TensileForceKgf = TryParseDecimal(GetCell(row, colDict, "Tensile force (KGF)")),
        //                TensileForceN = TryParseDecimal(GetCell(row, colDict, "Tensile force (N)")),
        //                StandardAsperSupplier = GetCell(row, colDict, "STD As per Supplier Specification")
        //            }
        //        };

        //        result.Add(model);
        //    }

        //    return result;
        //}

        //private static Dictionary<string, int> GetColumnDictionary(IXLRow headerRow)
        //{
        //    var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        //    int colIndex = 1;

        //    foreach (var cell in headerRow.Cells())
        //    {
        //        var normalized = NormalizeHeader(cell.GetString());
        //        if (!string.IsNullOrEmpty(normalized))
        //            dict[normalized] = colIndex;

        //        colIndex++;
        //    }

        //    return dict;
        //}

        //private static string GetCell(IXLRangeRow row, Dictionary<string, int> colDict, string columnName)
        //{
        //    var normalizedColumnName = NormalizeHeader(columnName);
        //    return colDict.TryGetValue(normalizedColumnName, out int col) ? row.Cell(col).GetString().Trim() : null;
        //}

        //private static decimal? TryParseDecimal(string input)
        //{
        //    if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        //        return result;
        //    return null;
        //}

        //private static List<WireCombinationDto> ExtractWireCombinations(IXLRangeRow row, Dictionary<string, int> colDict)
        //{
        //    var combinations = new List<WireCombinationDto>();

        //    for (int i = 1; i <= 13; i++)
        //    {
        //        var wireCode = GetCell(row, colDict, $"Wire Type Code {i}");
        //        var wireType = GetCell(row, colDict, $"Wire Type {i}");
        //        var wireSizeCode = GetCell(row, colDict, $"Wire Size Code {i}");
        //        var wireSize = GetCell(row, colDict, $"Wire Size {i}");

        //        if (!string.IsNullOrWhiteSpace(wireCode) ||
        //            !string.IsNullOrWhiteSpace(wireType) ||
        //            !string.IsNullOrWhiteSpace(wireSizeCode) ||
        //            !string.IsNullOrWhiteSpace(wireSize))
        //        {
        //            combinations.Add(new WireCombinationDto
        //            {
        //                WireCode = wireCode,
        //                WireType = wireType,
        //                WireSizeCode = wireSizeCode,
        //                WireSize = wireSize
        //            });
        //        }
        //    }

        //    return combinations;
        //}

        //private static string NormalizeHeader(string header)
        //{
        //    if (string.IsNullOrWhiteSpace(header))
        //        return null;

        //    return string.Join(" ", header
        //        .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
        //        .Trim()
        //        .Replace("  ", " ")
        //        .ToLowerInvariant(); // optional: lowercase for case-insensitive matching
        //}
        #endregion Last Code 2.40
        public static List<CrimpingStandardRequestVM> ReadCrimpingStandardsFromExcel(Stream fileStream)
        {
            var result = new List<CrimpingStandardRequestVM>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();

            var headerRow = worksheet.Row(3);        // actual column names in Row 3
            var dataRows = worksheet.RangeUsed().RowsUsed().Skip(2); // data starts from Row 4

            var orderedHeaders = GetOrderedHeaders(headerRow);

            foreach (var row in dataRows)
            {
                var model = new CrimpingStandardRequestVM
                {
                    Customer = GetCell(row, IndexOfNth(orderedHeaders, "customer", 1)),
                    Flag = GetCell(row, IndexOfNth(orderedHeaders, "flag", 1)),
                    Plant = GetCell(row, IndexOfNth(orderedHeaders, "plant", 1)),
                    RegistrationNo = GetCell(row, IndexOfNth(orderedHeaders, "registration no. of yazaki product engineering standard", 1)),
                    ManufacturinCrimpNo = GetCell(row, IndexOfNth(orderedHeaders, "manufacturing crimping standard control numbe", 1)),
                    TerminalNo = GetCell(row, IndexOfNth(orderedHeaders, "terminal number", 1)),
                    CommonTerminalNo = GetCell(row, IndexOfNth(orderedHeaders, "common terminal", 1)),
                    TerminalName = GetCell(row, IndexOfNth(orderedHeaders, "termainal name", 1)),
                    TerminalThickness = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "terminal thickness / shield ring thickness", 1))),

                    Accessories = new AccessoryDto
                    {
                        ShieldNo = GetCell(row, IndexOfNth(orderedHeaders, "accessory parts /shieid ring no./ shrink tube", 1)),
                        RubberSealPosition = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "rubber seal position", 1)))
                    },

                    ApplicatorDetails = new ApplicatorDetailDto
                    {
                        Machine = GetCell(row, IndexOfNth(orderedHeaders, "machine", 1)),
                        Feed = GetCell(row, IndexOfNth(orderedHeaders, "feed", 1))
                    },

                    WireCombinations = ExtractWireCombinations(row, orderedHeaders),

                    CrimpingStandardDetails = new CrimpingStandardDetailDto
                    {
                        ApplicableWireSize = GetCell(row, IndexOfNth(orderedHeaders, "applicable wire size", 1)),
                        InsulationCrimpShape = GetCell(row, IndexOfNth(orderedHeaders, "insulation crimp shape", 1)),
                        Ccheight = GetCell(row, IndexOfNth(orderedHeaders, "cc/h", 1)),
                        Icheight = GetCell(row, IndexOfNth(orderedHeaders, "ic/h", 1)),
                        Ccwidth = GetCell(row, IndexOfNth(orderedHeaders, "cc/w", 1)),
                        Icwidth = GetCell(row, IndexOfNth(orderedHeaders, "ic/w", 1)),
                        TensileForceKgf = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "tensile force (kgf)", 1))),
                        TensileForceN = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "tensile force (n)", 1))),
                        PillShape = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "pill shape (fig)", 1))),
                        Soldering = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "soldering", 1)))
                    },

                    StripingDetails = new StripingDetailDto
                    {
                        MiddelStriping = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "middel striping", 1))),
                        MiddelStrippingUpperLimit = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "middel striping upper limit", 1))),
                        MiddelStrippingLowerLimit = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "middel striping lower limit", 1))),
                        EndStriping = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "end striping", 1))),
                        EndStripingUpperLimit = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "end striping upper limit", 1))),
                        EndStripingLowerLimit = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "end striping lower limit", 1)))
                    },

                    CrimpingOtherParameters = new CrimpingOtherParameterDto
                    {
                        FrontCuttingCarry = GetCell(row, IndexOfNth(orderedHeaders, "front cutting carry (mm)", 1)),
                        RearCuttingCarry = GetCell(row, IndexOfNth(orderedHeaders, "rear cutting carry (mm)", 1)),
                        BrushLength = GetCell(row, IndexOfNth(orderedHeaders, "brush length /wire (mm)", 1)),
                        FrontBellMouth = GetCell(row, IndexOfNth(orderedHeaders, "front bell mouth (mm)", 1)),
                        RearBellMouth = GetCell(row, IndexOfNth(orderedHeaders, "rear bell mouth (mm)", 1)),
                        BendUp = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "bend up (°/ mm )", 1))),
                        BendUpUnit = GetCell(row, IndexOfNth(orderedHeaders, "unit", 1)),
                        BendDown = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "bend down (°/ mm )", 1))),
                        BendDownUnit = GetCell(row, IndexOfNth(orderedHeaders, "unit", 2)),
                        Rolling = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "rolling (° / mm)", 1))),
                        RollingUnit = GetCell(row, IndexOfNth(orderedHeaders, "unit", 3)),
                        Twist = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "twist  (° / mm )", 1))),
                        TwistUnit = GetCell(row, IndexOfNth(orderedHeaders, "unit", 4))
                    },

                    CrimpingDiesDetails = new CrimpingDiesDetailDto
                    {
                        CrimpingDieNoAnvilA = GetCell(row, IndexOfNth(orderedHeaders, "crimping die no anvil(a)", 1)),
                        CrimpingDieNoWireCrimperW = GetCell(row, IndexOfNth(orderedHeaders, "crimping die no               wire crimper(w)", 1)),
                        CrimpingDieNoInsulationCrimperI = GetCell(row, IndexOfNth(orderedHeaders, "crimping die no insulation crimper (i)", 1)),
                        CrimpingDieNoStabilizerCrimperQ = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "crimping die no stabilizer crimper(q)", 1))),
                        DiesCrimpingWidthConductorAnvilA = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "dies crimping width   \r\nconductor anvil(a)", 1))),
                        DiesCrimpingWidthInsulationAnvilA = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "dies crimping width  \r\ninsulation  anvil(a)", 1))),
                        DiesCrimpingWidthWireCrimperW = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "dies crimping width wire crimper (w)", 1))),
                        DiesCrimpingWidthInsulationCrimperI = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "dies crimping width  \r\ninsulation crimper (i)", 1))),
                        ConductorDieThickness = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "conductoer die thickness", 1))),
                        InsulationDieThickness = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "insulation die thickness", 1)))
                    },
                    ApprovalDetails = new ApprovalDetailDto
                    {
                        RevisionNo = TryParseInt(GetCell(row, IndexOfNth(orderedHeaders, "rev. no.", 1))),
                        RevisionDate = TryParseDate(GetCell(row, IndexOfNth(orderedHeaders, "rev.date", 1))),
                        RevisionDetails = GetCell(row, IndexOfNth(orderedHeaders, "rev. details", 1)),
                        MadeBy = GetCell(row, IndexOfNth(orderedHeaders, "made by:", 1)),
                        CheckedBy = GetCell(row, IndexOfNth(orderedHeaders, "checked by:", 1)),
                        ApprovedBy = GetCell(row, IndexOfNth(orderedHeaders, "approved by:", 1))
                    },

                    TerminalSupplierCrimpingSpec = new TerminalSupplierCrimpingSpecDto
                    {
                        TerminalSupplierName = GetCell(row, IndexOfNth(orderedHeaders, "terminal supplier name", 1)),
                        TerminalSupplierNumber = GetCell(row, IndexOfNth(orderedHeaders, "terminal supplier no.", 1)),
                        ApplicableWireSize = GetCell(row, IndexOfNth(orderedHeaders, "applicable wire size", 2)),
                        InsulationCrimpShape = GetCell(row, IndexOfNth(orderedHeaders, "insulation crimp shape", 2)),
                        Ccheight = GetCell(row, IndexOfNth(orderedHeaders, "cc/h", 2)),
                        Icheight = GetCell(row, IndexOfNth(orderedHeaders, "ic/h", 2)),
                        Ccwidth = GetCell(row, IndexOfNth(orderedHeaders, "cc/w", 2)),
                        Icwidth = GetCell(row, IndexOfNth(orderedHeaders, "ic/w", 2)),
                        TensileForceKgf = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "tensile force (kgf)", 2))),
                        TensileForceN = TryParseDecimal(GetCell(row, IndexOfNth(orderedHeaders, "tensile force (n)", 2))),
                        StandardAsperSupplier = GetCell(row, IndexOfNth(orderedHeaders, "std as per supplier specification", 1))
                    }
                };

                result.Add(model);
            }

            return result;
        }

        private static List<WireCombinationDto> ExtractWireCombinations(IXLRangeRow row, List<(string Name, int Index)> orderedHeaders)
        {
            var combinations = new List<WireCombinationDto>();

            for (int i = 1; i <= 13; i++)
            {
                var wireCode = GetCell(row, IndexOfNth(orderedHeaders, $"wire type code {i}", 1));
                var wireType = GetCell(row, IndexOfNth(orderedHeaders, $"wire type {i}", 1));
                var wireSizeCode = GetCell(row, IndexOfNth(orderedHeaders, $"wire size code {i}", 1));
                var wireSize = GetCell(row, IndexOfNth(orderedHeaders, $"wire size {i}", 1));

                if (!string.IsNullOrWhiteSpace(wireCode) ||
                    !string.IsNullOrWhiteSpace(wireType) ||
                    !string.IsNullOrWhiteSpace(wireSizeCode) ||
                    !string.IsNullOrWhiteSpace(wireSize))
                {
                    combinations.Add(new WireCombinationDto
                    {
                        WireCode = wireCode,
                        WireType = wireType,
                        WireSizeCode = wireSizeCode,
                        WireSize = wireSize
                    });
                }
            }

            return combinations;
        }


        private static List<(string Name, int Index)> GetOrderedHeaders(IXLRow headerRow)
        {
            var headers = new List<(string Name, int Index)>();
            int colIndex = 1;

            foreach (var cell in headerRow.Cells())
            {
                var name = NormalizeHeader(cell.GetString());
                if (!string.IsNullOrWhiteSpace(name))
                    headers.Add((name, colIndex));
                colIndex++;
            }

            return headers;
        }

        private static int IndexOfNth(List<(string Name, int Index)> headers, string name, int occurrence)
        {
            name = NormalizeHeader(name);
            int count = 0;

            foreach (var header in headers)
            {
                if (header.Name == name)
                {
                    count++;
                    if (count == occurrence)
                        return header.Index;
                }
            }

            return -1;
        }

        private static string GetCell(IXLRangeRow row, int colIndex)
        {
            return colIndex > 0 ? row.Cell(colIndex).GetString().Trim() : null;
        }

        private static decimal? TryParseDecimal(string input)
        {
            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }

        private static string NormalizeHeader(string header)
        {
            return string.Join(" ", header.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                        .Trim()
                        .Replace("  ", " ")
                        .ToLowerInvariant();
        }

        private static int? TryParseInt(string input)
        {
            if (int.TryParse(input, out var result))
                return result;
            return null;
        }

        private static DateTime? TryParseDate(string input)
        {
            if (DateTime.TryParse(input, out var result))
                return result;
            return null;
        }

        #endregion commonDataSanitizingMethod
    }
}