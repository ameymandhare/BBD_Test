using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL.Repository
{
    public class CrimpingStandardRepository
    {
        private readonly AppDbContext _context;

        public CrimpingStandardRepository(AppDbContext context)
        {
            _context = context;
        }
        //public async Task<T1Terminal> GetFullTerminalDetails(string terminalNo)
        //{
        //    return await _context.T1Terminals
        //        .Include(t => t.Accessories)
        //        .Include(t => t.ApplicatorDetails)
        //        .Include(t => t.WireCombinations)
        //        .Include(t => t.CrimpingStandardDetails)
        //        .Include(t => t.StripingDetails)
        //        .Include(t => t.CrimpingOtherParameters)
        //        .Include(t => t.CrimpingDiesDetails)
        //        .Include(t => t.ApprovalDetails)
        //        .Include(t => t.TerminalSupplierCrimpingSpec)
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(t => t.TerminalNo == terminalNo && t.IsActive);
        //}


        //public async Task<List<T1Terminal>> GetAllActiveTerminalsWithDetails()
        //{
        //    try
        //    {

        //        var terminals = await _context.T1Terminals
        //            .Where(t => t.IsActive)
        //            .Include(t => t.Accessories)
        //            .Include(t => t.ApplicatorDetails)
        //            .Include(t => t.WireCombinations)
        //            .Include(t => t.CrimpingStandardDetails)
        //                .ThenInclude(c => c.InsulationCrimpShapeNavigation)
        //            .Include(t => t.StripingDetails)
        //            .Include(t => t.CrimpingOtherParameters)
        //            .Include(t => t.CrimpingDiesDetails)
        //            .Include(t => t.ApprovalDetails)
        //            .Include(t => t.TerminalSupplierCrimpingSpec)
        //            .AsNoTracking()
        //            .ToListAsync();

        //        return terminals;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        public async Task<bool> DeleteTerminal(int terminalId, int modifiedById)
        {
            var terminal = await _context.T1Terminals
                .Include(t => t.Accessories)
                .Include(t => t.ApplicatorDetails)
                .Include(t => t.WireCombinations)
                .Include(t => t.CrimpingStandardDetails)
                .Include(t => t.StripingDetails)
                .Include(t => t.CrimpingOtherParameters)
                .Include(t => t.CrimpingDiesDetails)
                .Include(t => t.ApprovalDetails)
                .Include(t => t.TerminalSupplierCrimpingSpec)
                .FirstOrDefaultAsync(t => t.Id == terminalId);

            if (terminal == null)
                return false;

            terminal.IsActive = false;

            foreach (var a in terminal.Accessories ?? new List<T1C1Accessory>())
            {
                a.IsActive = false;
                a.ModifiedById = modifiedById;
            }

            foreach (var a in terminal.ApplicatorDetails ?? new List<T1C2ApplicatorDetail>())
            {
                a.IsActive = false;
                a.ModifiedById = modifiedById;
            }

            foreach (var w in terminal.WireCombinations ?? new List<T1C4CombinationDetail>())
            {
                w.IsActive = false;
                w.ModifiedById = modifiedById;
            }

            foreach (var c in terminal.CrimpingStandardDetails ?? new List<T1C5CrimpingStandardDetail>())
            {
                c.IsActive = false;
                c.ModifiedById = modifiedById;
            }

            foreach (var s in terminal.StripingDetails ?? new List<T1C3StripingDetail>())
            {
                s.IsActive = false;
                s.ModifiedById = modifiedById;
            }

            foreach (var c in terminal.CrimpingOtherParameters ?? new List<T1C6CrimpingOtherParameter>())
            {
                c.IsActive = false;
                c.ModifiedById = modifiedById;
            }

            foreach (var c in terminal.CrimpingDiesDetails ?? new List<T1C7CrimpingDiesDetail>())
            {
                c.IsActive = false;
                c.ModifiedById = modifiedById;
            }

            foreach (var a in terminal.ApprovalDetails ?? new List<T1C8ApprovalDetail>())
            {
                a.IsActive = false;
                a.ModifiedById = modifiedById;
            }

            foreach (var t in terminal.TerminalSupplierCrimpingSpec ?? new List<T1C9TerminalSupplierCrimpingSpec>())
            {
                t.IsActive = false;
                t.ModifiedById = modifiedById;
            }





            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<T1Terminal>> GetAllActiveTerminalsWithDetails(JObject filters)
        {
            try
            {
                var query = _context.T1Terminals
                    .Include(t => t.Accessories)
                    .Include(t => t.ApplicatorDetails)
                    .Include(t => t.WireCombinations)
                    .Include(t => t.CrimpingStandardDetails)
                        .ThenInclude(c => c.InsulationCrimpShapeNavigation)
                    .Include(t => t.StripingDetails)
                    .Include(t => t.CrimpingOtherParameters)
                    .Include(t => t.CrimpingDiesDetails)
                    .Include(t => t.ApprovalDetails)
                    .Include(t => t.TerminalSupplierCrimpingSpec)
                       .ThenInclude(c => c.InsulationCrimpShapeNavigation)
                    .AsQueryable();

                // Apply filters from JObject (camelCase)

                if (filters["plantId"] != null && !string.IsNullOrWhiteSpace(filters["plantId"]!.ToString()))
                {
                    var plantId = Convert.ToInt32(filters["plantId"]);
                    query = query.Where(t => t.PlantCode == plantId);
                }
                if (filters["plantCode"] != null && !string.IsNullOrWhiteSpace(filters["plantCode"]!.ToString()))
                {
                    var plantCode = Convert.ToInt32(filters["plantCode"]);
                    query = query.Where(t => t.PlantCode == plantCode);
                }
                if (filters["terminalNo"] != null && !string.IsNullOrWhiteSpace(filters["terminalNo"]!.ToString()))
                {
                    var terminalNo = filters["terminalNo"]!.ToString();
                    query = query.Where(t => t.TerminalNo != null && t.TerminalNo.Contains(terminalNo));
                }

                if (filters["customer"] != null && !string.IsNullOrWhiteSpace(filters["customer"]!.ToString()))
                {
                    var customer = filters["customer"]!.ToString();
                    query = query.Where(t => t.Customer != null && t.Customer.Contains(customer));
                }

                if (filters["accessory"] != null && !string.IsNullOrWhiteSpace(filters["accessory"]!.ToString()))
                {
                    var accessory = filters["accessory"]!.ToString();
                    query = query.Where(t => t.Accessories.Any(a => a.ShieldNo != null && a.ShieldNo.Contains(accessory)));
                }

                //if (filters["insulation"] != null && !string.IsNullOrWhiteSpace(filters["insulation"]!.ToString()))
                //{
                //    var insulation = filters["insulation"]!.ToString();
                //    query = query.Where(t => t.InsulationCrimpShape != null && t.InsulationCrimpShape.Contains(insulation));
                //}
                if (filters["insulation"] != null && !string.IsNullOrWhiteSpace(filters["insulation"]!.ToString()))
                {
                    var insulation = filters["insulation"]!.ToString();
                    query = query.Where(t => t.CrimpingStandardDetails.FirstOrDefault().InsulationCrimpShapeNavigation.Name != null && t.CrimpingStandardDetails.FirstOrDefault().InsulationCrimpShapeNavigation.Name.Contains(insulation));
                }

                //if (filters["wireCombination"] != null)
                //{
                //    var wireComboListRaw = filters["wireCombination"]?.ToObject<List<JObject>>();

                //    var wireComboList = wireComboListRaw?
                //        .Where(x =>
                //            !string.IsNullOrWhiteSpace(x["wireKind"]?.ToString()) ||
                //            !string.IsNullOrWhiteSpace(x["wireSize"]?.ToString()))
                //        .Select(x => new
                //        {
                //            WireKind = x["wireKind"]?.ToString(),
                //            WireSize = x["wireSize"]?.ToString()
                //        })
                //        .ToList();

                //    if (wireComboList != null && wireComboList.Any())
                //    {
                //        var wireKindList = wireComboList
                //            .Where(x => !string.IsNullOrEmpty(x.WireKind))
                //            .Select(x => x.WireKind)
                //            .ToList();

                //        var wireSizeList = wireComboList
                //            .Where(x => !string.IsNullOrEmpty(x.WireSize))
                //            .Select(x => x.WireSize)
                //            .ToList();

                //        query = query.Where(t => t.WireCombinations.Any(w =>
                //            (wireKindList.Any() &&
                //                (wireKindList.Contains(w.WireType) || wireKindList.Contains(w.WireCode)))
                //            ||
                //            (wireSizeList.Any() &&
                //                (wireSizeList.Contains(w.WireSize) || wireSizeList.Contains(w.WireSizeCode)))
                //        ));
                //    }
                //}
                if (filters["wireCombination"] != null)
                {
                    var wireComboListRaw = filters["wireCombination"]?.ToObject<List<JObject>>();

                    var wireComboList = wireComboListRaw?
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(x["wireKind"]?.ToString()) ||
                            !string.IsNullOrWhiteSpace(x["wireSize"]?.ToString()))
                        .Select(x => new
                        {
                            WireKind = x["wireKind"]?.ToString().ToLower(),
                            WireSize = x["wireSize"]?.ToString().ToLower()
                        })
                        .ToList();

                    if (wireComboList != null && wireComboList.Any())
                    {
                        var wireKindList = wireComboList
                            .Where(x => !string.IsNullOrEmpty(x.WireKind))
                            .Select(x => x.WireKind)
                            .ToList();

                        var wireSizeList = wireComboList
                            .Where(x => !string.IsNullOrEmpty(x.WireSize))
                            .Select(x => x.WireSize)
                            .ToList();

                        query = query.Where(t => t.WireCombinations.Any(w =>
                            (wireKindList.Any() &&
                                wireKindList.Any(k =>
                                    (!string.IsNullOrEmpty(w.WireType) && w.WireType.ToLower().Contains(k)) ||
                                    (!string.IsNullOrEmpty(w.WireCode) && w.WireCode.ToLower().Contains(k))
                                ))
                            ||
                            (wireSizeList.Any() &&
                                wireSizeList.Any(s =>
                                    (!string.IsNullOrEmpty(w.WireSize) && w.WireSize.ToLower().Contains(s)) ||
                                    (!string.IsNullOrEmpty(w.WireSizeCode) && w.WireSizeCode.ToLower().Contains(s))
                                ))
                        ));
                    }
                }


                query = query.Where(t => t.IsActive);
                query = query.OrderByDescending(t => t.Id);
                return await query.AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<T1Terminal>> GetAllActiveTerminalsWithDetails_SP(JObject filters)
        {
            try
            {
                // Prepare parameters
                var parameters = new DynamicParameters();

                if (filters["plantId"] != null && !string.IsNullOrWhiteSpace(filters["plantId"]!.ToString()))
                {
                    parameters.Add("@PlantId", Convert.ToInt32(filters["plantId"]));
                }

                if (filters["terminalNo"] != null && !string.IsNullOrWhiteSpace(filters["terminalNo"]!.ToString()))
                {
                    parameters.Add("@TerminalNo", filters["terminalNo"]!.ToString());
                }

                if (filters["customer"] != null && !string.IsNullOrWhiteSpace(filters["customer"]!.ToString()))
                {
                    parameters.Add("@Customer", filters["customer"]!.ToString());
                }

                if (filters["accessory"] != null && !string.IsNullOrWhiteSpace(filters["accessory"]!.ToString()))
                {
                    parameters.Add("@Accessory", filters["accessory"]!.ToString());
                }

                if (filters["insulation"] != null && !string.IsNullOrWhiteSpace(filters["insulation"]!.ToString()))
                {
                    parameters.Add("@Insulation", filters["insulation"]!.ToString());
                }

                if (filters["wireCombination"] != null)
                {
                    parameters.Add("@WireCombinationJson", JsonConvert.SerializeObject(filters["wireCombination"]));
                }

                // Get the connection string from your DbContext
                var connectionString = _context.Database.GetDbConnection().ConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var multi = await connection.QueryMultipleAsync(
                        "sp_GetAllActiveTerminalsWithDetails",
                        parameters,
                        commandType: CommandType.StoredProcedure))
                    {
                        // Map the results
                        var terminals = (await multi.ReadAsync<T1Terminal>()).ToList();
                        var accessories = (await multi.ReadAsync<T1C1Accessory>()).ToList();
                        var applicatorDetails = (await multi.ReadAsync<T1C2ApplicatorDetail>()).ToList();
                        var wireCombinations = (await multi.ReadAsync<T1C4CombinationDetail>()).ToList();
                        var crimpingStandardDetails = (await multi.ReadAsync<T1C5CrimpingStandardDetail>()).ToList();
                        var stripingDetails = (await multi.ReadAsync<T1C3StripingDetail>()).ToList();
                        var crimpingOtherParameters = (await multi.ReadAsync<T1C6CrimpingOtherParameter>()).ToList();
                        var crimpingDiesDetails = (await multi.ReadAsync<T1C7CrimpingDiesDetail>()).ToList();
                        var approvalDetails = (await multi.ReadAsync<T1C8ApprovalDetail>()).ToList();
                        var terminalSupplierCrimpingSpec = (await multi.ReadAsync<T1C9TerminalSupplierCrimpingSpec>()).ToList();
                        var crimpingShapes = (await multi.ReadAsync<MCrimpingShape>()).ToList();

                        // Assign shape navigation property
                        foreach (var c in crimpingStandardDetails)
                        {
                            c.InsulationCrimpShapeNavigation = crimpingShapes.FirstOrDefault(s => s.Id == c.InsulationCrimpShape);
                        }

                        foreach (var t in terminalSupplierCrimpingSpec)
                        {
                            t.InsulationCrimpShapeNavigation = crimpingShapes.FirstOrDefault(s => s.Id == t.InsulationCrimpShape);
                        }

                        // Map related data to terminals
                        foreach (var terminal in terminals)
                        {
                            terminal.Accessories = accessories.Where(a => a.TerminalId == terminal.Id).ToList();
                            terminal.ApplicatorDetails = applicatorDetails.Where(a => a.TerminalId == terminal.Id).ToList();
                            terminal.WireCombinations = wireCombinations.Where(w => w.TerminalId == terminal.Id).ToList();
                            terminal.CrimpingStandardDetails = crimpingStandardDetails.Where(c => c.TerminalId == terminal.Id).ToList();
                            terminal.StripingDetails = stripingDetails.Where(s => s.TerminalId == terminal.Id).ToList();
                            terminal.CrimpingOtherParameters = crimpingOtherParameters.Where(c => c.TerminalId == terminal.Id).ToList();
                            terminal.CrimpingDiesDetails = crimpingDiesDetails.Where(c => c.TerminalId == terminal.Id).ToList();
                            terminal.ApprovalDetails = approvalDetails.Where(a => a.TerminalId == terminal.Id).ToList();
                            terminal.TerminalSupplierCrimpingSpec = terminalSupplierCrimpingSpec.Where(t => t.TerminalId == terminal.Id).ToList();
                        }

                        return terminals;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // In your MasterDataRepository
        public async Task<bool> CustomerExists(string customerName)
        {
            return await _context.MCustomer
                .AsNoTracking()
                .AnyAsync(c => c.CustomerName == customerName);
        }

        public async Task<bool> PlantExists(string plantCode)
        {
            return await _context.MPlants
                .AsNoTracking()
                .AnyAsync(p => p.PlantCode == plantCode);
        }
        public async Task<T1Terminal> GetTeminalCombination(
       string terminalNo,
       string insulationCrimpShape,
       string shieldNo)
        {
            if (string.IsNullOrWhiteSpace(terminalNo) ||
                string.IsNullOrWhiteSpace(insulationCrimpShape) ||
                string.IsNullOrWhiteSpace(shieldNo))
            {
                return null;
            }

            return await _context.T1Terminals
                .AsNoTracking()
                .Join(_context.T1C5CrimpingStandardDetails,
                    t => t.Id,
                    cs => cs.TerminalId,
                    (t, cs) => new { Terminal = t, CrimpingStandard = cs })
                .Join(_context.T1C1Accessories,
                    temp => temp.Terminal.Id,
                    a => a.TerminalId,
                    (temp, a) => new { temp.Terminal, temp.CrimpingStandard, Accessory = a })
                .Where(x => x.Terminal.TerminalNo == terminalNo &&
                           x.CrimpingStandard.InsulationCrimpShapeNavigation.Name == insulationCrimpShape &&
                           x.Accessory.ShieldNo == shieldNo)
                .Select(x => x.Terminal)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Begins a new database transaction
        /// </summary>
        /// <returns>A new database transaction</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Adds a new terminal record to the database
        /// </summary>
        /// <param name="terminal">The terminal entity to add</param>
        /// <returns>The ID of the newly created terminal</returns>
        public async Task<int> AddTerminal(T1Terminal terminal)
        {
            try
            {
                terminal.CreatedDate = DateTime.Now;
                terminal.IsActive = true;

                _context.T1Terminals.Add(terminal);
                await _context.SaveChangesAsync();
                return terminal.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Adds accessory information for a terminal
        /// </summary>
        /// <param name="accessory">The accessory entity to add</param>
        public async Task AddAccessory(T1C1Accessory accessory)
        {
            accessory.CreatedDate = DateTime.Now;
            accessory.IsActive = true;

            _context.T1C1Accessories.Add(accessory);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds applicator details for a terminal
        /// </summary>
        /// <param name="applicator">The applicator details entity to add</param>
        public async Task AddApplicatorDetail(T1C2ApplicatorDetail applicator)
        {
            applicator.CreatedDate = DateTime.Now;
            applicator.IsActive = true;

            _context.T1C2ApplicatorDetails.Add(applicator);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a wire combination for a terminal
        /// </summary>
        /// <param name="combination">The wire combination entity to add</param>
        public async Task AddCombinationDetail(T1C4CombinationDetail combination)
        {
            combination.CreatedDate = DateTime.Now;
            combination.IsActive = true;

            _context.T1C4CombinationDetails.Add(combination);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds crimping standard details for a terminal
        /// </summary>
        /// <param name="standard">The crimping standard details entity to add</param>
        public async Task AddCrimpingStandardDetail(T1C5CrimpingStandardDetail standard)
        {
            standard.CreatedDate = DateTime.Now;
            standard.IsActive = true;

            _context.T1C5CrimpingStandardDetails.Add(standard);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds striping details for a terminal
        /// </summary>
        /// <param name="striping">The striping details entity to add</param>
        public async Task AddStripingDetail(T1C3StripingDetail striping)
        {
            striping.CreatedDate = DateTime.Now;
            striping.IsActive = true;

            _context.T1C3StripingDetails.Add(striping);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds crimping other parameters for a terminal
        /// </summary>
        /// <param name="parameter">The crimping parameters entity to add</param>
        public async Task AddCrimpingOtherParameter(T1C6CrimpingOtherParameter parameter)
        {
            parameter.CreatedDate = DateTime.Now;
            parameter.IsActive = true;

            _context.T1C6CrimpingOtherParameters.Add(parameter);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds crimping dies details for a terminal
        /// </summary>
        /// <param name="diesDetail">The dies details entity to add</param>
        public async Task AddCrimpingDiesDetail(T1C7CrimpingDiesDetail diesDetail)
        {
            diesDetail.CreatedDate = DateTime.Now;
            diesDetail.IsActive = true;

            _context.T1C7CrimpingDiesDetails.Add(diesDetail);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds approval details for a terminal
        /// </summary>
        /// <param name="approval">The approval details entity to add</param>
        public async Task AddApprovalDetail(T1C8ApprovalDetail approval)
        {
            approval.CreatedDate = DateTime.Now;
            approval.IsActive = true;

            _context.T1C8ApprovalDetails.Add(approval);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds terminal supplier crimping specifications
        /// </summary>
        /// <param name="spec">The supplier specifications entity to add</param>
        public async Task AddTerminalSupplierCrimpingSpec(T1C9TerminalSupplierCrimpingSpec spec)
        {
            spec.CreatedDate = DateTime.Now;
            spec.IsActive = true;

            _context.T1C9TerminalSupplierCrimpingSpecs.Add(spec);
            await _context.SaveChangesAsync();
        }
        public async Task<int?> GetCrimpingShapeIdByName(string shapeName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(shapeName))
                    return null;

                var lowerName = shapeName.ToLower();
                var shape = await _context.MCrimpingShapes
                    .AsNoTracking()
                    .Where(s => s.IsActive && s.Name.ToLower() == lowerName)
                    .FirstOrDefaultAsync();

                return shape?.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Checks if a terminal with the given number already exists
        /// </summary>
        /// <param name="terminalNo">The terminal number to check</param>
        /// <returns>True if the terminal exists, false otherwise</returns>
        public async Task<bool> TerminalExists(string terminalNo)
        {
            return await _context.T1Terminals.AnyAsync(t => t.TerminalNo == terminalNo);
        }

        /// <summary>
        /// Checks if a wire combination already exists for a terminal
        /// </summary>
        /// <param name="terminalId">The terminal ID to check</param>
        /// <param name="wireCode">The wire code to check</param>
        /// <returns>True if the combination exists, false otherwise</returns>
        public async Task<bool> CombinationExists(int terminalId, string wireCode)
        {
            return await _context.T1C4CombinationDetails
                .AnyAsync(c => c.TerminalId == terminalId && c.WireCode == wireCode);
        }

        /// <summary>
        /// Gets terminals filtered by plant code
        /// </summary>
        /// <param name="plantCode">The plant code to filter by</param>
        /// <returns>List of terminals matching the plant code</returns>
        //public async Task<List<T1Terminal>> GetTerminalsByPlant(string plantCode)
        //{
        //    return await _context.T1Terminals
        //        .Where(t => t.PlantCode == plantCode)
        //        .ToListAsync();
        //}













        #region Terminal Operations

        /// <summary>
        /// Retrieves a terminal by its unique identifier
        /// </summary>
        /// <param name="id">Terminal ID to search for</param>
        /// <returns>The terminal entity if found, otherwise null</returns>
        public async Task<T1Terminal> GetTerminalById(int id)
        {
            return await _context.T1Terminals.FindAsync(id);
        }

        /// <summary>
        /// Updates an existing terminal record in the database
        /// </summary>
        /// <param name="terminal">The terminal entity with updated values</param>
        public async Task UpdateTerminal(T1Terminal terminal)
        {
            _context.T1Terminals.Update(terminal);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Accessory Operations

        /// <summary>
        /// Gets the accessory record associated with a specific terminal
        /// </summary>
        /// <param name="terminalId">Terminal ID to search for</param>
        /// <returns>Accessory entity if found, otherwise null</returns>
        public async Task<T1C1Accessory> GetAccessoryByTerminalId(int terminalId)
        {
            return await _context.T1C1Accessories
                .FirstOrDefaultAsync(a => a.TerminalId == terminalId);
        }

        /// <summary>
        /// Updates an existing accessory record
        /// </summary>
        /// <param name="accessory">Accessory entity with updated values</param>
        public async Task UpdateAccessory(T1C1Accessory accessory)
        {
            _context.T1C1Accessories.Update(accessory);
            await _context.SaveChangesAsync();
        }



        #endregion

        #region Applicator Detail Operations

        /// <summary>
        /// Gets the applicator details for a specific terminal
        /// </summary>
        /// <param name="terminalId">Terminal ID to search for</param>
        /// <returns>Applicator detail entity if found, otherwise null</returns>
        public async Task<T1C2ApplicatorDetail> GetApplicatorDetailByTerminalId(int terminalId)
        {
            return await _context.T1C2ApplicatorDetails
                .FirstOrDefaultAsync(a => a.TerminalId == terminalId);
        }

        /// <summary>
        /// Updates existing applicator details
        /// </summary>
        /// <param name="applicatorDetail">Applicator detail entity with updated values</param>
        public async Task UpdateApplicatorDetail(T1C2ApplicatorDetail applicatorDetail)
        {
            _context.T1C2ApplicatorDetails.Update(applicatorDetail);
            await _context.SaveChangesAsync();
        }



        #endregion

        #region Wire Combination Operations

        /// <summary>
        /// Deletes all wire combinations associated with a terminal
        /// (Used before recreating the list during updates)
        /// </summary>
        /// <param name="terminalId">Terminal ID to clear combinations for</param>
        public async Task DeleteWireCombinationsByTerminalId(int terminalId)
        {
            var combinations = await _context.T1C4CombinationDetails
                .Where(c => c.TerminalId == terminalId)
                .ToListAsync();

            _context.T1C4CombinationDetails.RemoveRange(combinations);
            await _context.SaveChangesAsync();
        }



        #endregion

        #region Crimping Standard Detail Operations

        /// <summary>
        /// Gets crimping standard details for a terminal
        /// </summary>
        /// <param name="terminalId">Terminal ID to search for</param>
        /// <returns>Crimping standard entity if found, otherwise null</returns>
        public async Task<T1C5CrimpingStandardDetail> GetCrimpingStandardDetailByTerminalId(int terminalId)
        {
            return await _context.T1C5CrimpingStandardDetails
                .FirstOrDefaultAsync(c => c.TerminalId == terminalId);
        }

        /// <summary>
        /// Updates existing crimping standard details
        /// </summary>
        /// <param name="detail">Crimping standard entity with updated values</param>
        public async Task UpdateCrimpingStandardDetail(T1C5CrimpingStandardDetail detail)
        {
            _context.T1C5CrimpingStandardDetails.Update(detail);
            await _context.SaveChangesAsync();
        }



        #endregion

        #region Striping Detail Operations

        /// <summary>
        /// Gets striping details for a terminal
        /// </summary>
        /// <param name="terminalId">Terminal ID to search for</param>
        /// <returns>Striping detail entity if found, otherwise null</returns>
        public async Task<T1C3StripingDetail> GetStripingDetailByTerminalId(int terminalId)
        {
            return await _context.T1C3StripingDetails
                .FirstOrDefaultAsync(s => s.TerminalId == terminalId);
        }

        /// <summary>
        /// Updates existing striping details
        /// </summary>
        /// <param name="detail">Striping detail entity with updated values</param>
        public async Task UpdateStripingDetail(T1C3StripingDetail detail)
        {
            _context.T1C3StripingDetails.Update(detail);
            await _context.SaveChangesAsync();
        }



        #endregion

        /// <summary>
        /// Gets approval details for a specific terminal
        /// </summary>
        /// <param name="terminalId">Terminal ID to search for</param>
        /// <returns>Approval detail record if found, otherwise null</returns>
        public async Task<T1C8ApprovalDetail> GetApprovalDetailByTerminalId(int terminalId)
        {
            return await _context.T1C8ApprovalDetails
                .FirstOrDefaultAsync(a => a.TerminalId == terminalId);
        }

        /// <summary>
        /// Updates existing approval details for a terminal
        /// </summary>
        /// <param name="detail">Approval detail entity with updated values</param>
        /// <returns>True if update succeeded, false if record not found</returns>
        public async Task<bool> UpdateApprovalDetail(T1C8ApprovalDetail detail)
        {
            var existing = await _context.T1C8ApprovalDetails
                .FirstOrDefaultAsync(a => a.Id == detail.Id);

            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(detail);
            await _context.SaveChangesAsync();
            return true;
        }





        /// <summary>
        /// Retrieves the terminal supplier crimping specifications for a given terminal ID
        /// </summary>
        /// <param name="terminalId">The ID of the terminal to search for</param>
        /// <returns>Supplier crimping specifications if found, otherwise null</returns>
        public async Task<T1C9TerminalSupplierCrimpingSpec> GetTerminalSupplierCrimpingSpecByTerminalId(int terminalId)
        {
            return await _context.T1C9TerminalSupplierCrimpingSpecs
                .AsNoTracking()  // Read-only operation doesn't need change tracking
                .FirstOrDefaultAsync(t => t.TerminalId == terminalId);
        }

        /// <summary>
        /// Updates existing terminal supplier crimping specifications
        /// </summary>
        /// <param name="spec">The updated supplier crimping specification entity</param>
        /// <exception cref="ArgumentNullException">Thrown if spec is null</exception>
        public async Task UpdateTerminalSupplierCrimpingSpec(T1C9TerminalSupplierCrimpingSpec spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            _context.T1C9TerminalSupplierCrimpingSpecs.Update(spec);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Retrieves crimping other parameters for a terminal
        /// </summary>
        /// <param name="terminalId">Terminal ID to search for</param>
        /// <returns>Crimping other parameters if found, otherwise null</returns>
        public async Task<T1C6CrimpingOtherParameter> GetCrimpingOtherParameterByTerminalId(int terminalId)
        {
            return await _context.T1C6CrimpingOtherParameters
                .FirstOrDefaultAsync(c => c.TerminalId == terminalId);
        }

        /// <summary>
        /// Updates crimping other parameters with concurrency check
        /// </summary>
        /// <param name="parameter">Updated parameter entity</param>
        /// <exception cref="DbUpdateConcurrencyException">Thrown if record was modified by another process</exception>
        public async Task UpdateCrimpingOtherParameter(T1C6CrimpingOtherParameter parameter)
        {
            try
            {
                _context.T1C6CrimpingOtherParameters.Update(parameter);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets crimping dies details for a specific terminal
        /// </summary>
        /// <param name="terminalId">Terminal ID to search for</param>
        /// <returns>Crimping dies details if found, otherwise null</returns>
        public async Task<T1C7CrimpingDiesDetail> GetCrimpingDiesDetailByTerminalId(int terminalId)
        {
            return await _context.T1C7CrimpingDiesDetails
                .Where(c => c.TerminalId == terminalId)
                .OrderByDescending(c => c.CreatedDate)  // Get most recent if multiple exist
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Updates crimping dies details with validation
        /// </summary>
        /// <param name="detail">Updated dies detail entity</param>
        /// <exception cref="InvalidOperationException">Thrown if entity state is invalid</exception>
        public async Task UpdateCrimpingDiesDetail(T1C7CrimpingDiesDetail detail)
        {
            if (detail.ConductorDieThickness <= 0 || detail.InsulationDieThickness <= 0)
            {
                throw new InvalidOperationException("Die thickness values must be positive");
            }

            _context.T1C7CrimpingDiesDetails.Update(detail);
            await _context.SaveChangesAsync();
        }
        #region Transaction Management



        #endregion
    }
}
