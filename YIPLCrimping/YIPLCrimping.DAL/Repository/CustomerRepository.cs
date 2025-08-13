using Microsoft.EntityFrameworkCore;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL.Repository
{
    /// <summary>
    /// Repository class for managing Customer-related database operations.
    /// Handles CRUD operations, bulk operations, and advanced filtering.
    /// </summary>
    public class CustomerRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly CommonDbContext commonDbContext;

        // Singleton logger instance for logging repository actions and errors.
        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
        /// </summary>
        public CustomerRepository(AppDbContext appDbContext, CommonDbContext commonDbContext)
        {
            this.appDbContext = appDbContext;
            this.commonDbContext = commonDbContext;
        }

        /// <summary>
        /// Retrieves a list of active customers filtered by optional ID or search text.
        /// </summary>
        public async Task<List<MCustomer>> Get(int? id, string? searchText)
        {
            try
            {
                logger.WriteInfo($"Fetching customers. Filter: Id={id}, SearchText='{searchText}'");

                var query = appDbContext.MCustomer
                    .Where(x => x.IsActive)
                    .AsQueryable();

                if (id > 0)
                {
                    query = query.Where(x => x.Id == id);
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    string lowerSearch = searchText.ToLower();
                    query = query.Where(x =>
                        x.CustomerName.ToLower().Contains(lowerSearch) ||
                        x.CustomerCode.ToLower().Contains(lowerSearch));
                }

                var customers = await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
                logger.WriteInfo($"Found {customers.Count} customer(s).");
                return customers;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in CustomerRepository.Get: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific customer by ID including related terminal mappings.
        /// </summary>
        public async Task<MCustomer> Get(int id)
        {
            try
            {
                logger.WriteInfo($"Fetching customer by Id: {id}");
                var customer = await appDbContext.MCustomer
                    //.Include(x => x.CrimpingStandardCustomerTerminals)
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

                if (customer != null)
                {
                    logger.WriteInfo($"Customer found. Id: {customer.Id}, Name: {customer.CustomerName}");
                }
                else
                {
                    logger.WriteInfo($"Customer with Id {id} not found.");
                }

                return customer;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in CustomerRepository.Get (by Id): {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds a new customer to the database.
        /// </summary>
        public async Task<int> Add(MCustomer customer)
        {
            try
            {
                logger.WriteInfo($"Adding new customer: Name={customer.CustomerName}, Code={customer.CustomerCode}");
                await appDbContext.MCustomer.AddAsync(customer);
                int result = await appDbContext.SaveChangesAsync();
                logger.WriteInfo($"Customer added successfully. Id: {customer.Id}");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in CustomerRepository.Add: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing customer in the database.
        /// </summary>
        public async Task<int> Update(MCustomer customer)
        {
            try
            {
                logger.WriteInfo($"Updating customer: Id={customer.Id}, Name={customer.CustomerName}");
                appDbContext.MCustomer.Update(customer);
                int result = await appDbContext.SaveChangesAsync();
                logger.WriteInfo($"Customer updated successfully. Id={customer.Id}");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in CustomerRepository.Update: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a dictionary of active customers by a list of IDs.
        /// </summary>
        public async Task<Dictionary<int, MCustomer>> Getlist(List<int> ids)
        {
            try
            {
                logger.WriteInfo($"Fetching customers by Ids: {string.Join(",", ids)}");
                var result = await appDbContext.MCustomer
                    .Where(x => ids.Contains(x.Id) && x.IsActive)
                    .ToDictionaryAsync(x => x.Id);
                logger.WriteInfo($"Fetched {result.Count} customers.");
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in CustomerRepository.Getlist: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds a list of new customers to the database in bulk.
        /// </summary>
        public async Task BulkAdd(List<MCustomer> customers)
        {
            try
            {
                logger.WriteInfo($"Bulk adding {customers.Count} new customers.");
                await appDbContext.MCustomer.AddRangeAsync(customers);
                await appDbContext.SaveChangesAsync();
                logger.WriteInfo("Bulk add completed successfully.");
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in CustomerRepository.BulkAdd: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Inserts a list of customers in bulk and logs the result.
        /// </summary>
        /// <returns>A success message with count, or logs and throws exception on failure.</returns>
        public async Task<string> BulkInsertCustomers(List<MCustomer> customers)
        {
            try
            {
                logger.WriteInfo($"Starting bulk insert for {customers.Count} customers.");
                await appDbContext.MCustomer.AddRangeAsync(customers);
                await appDbContext.SaveChangesAsync();
                logger.WriteInfo("Bulk insert completed successfully.");
                return $"Imported {customers.Count} customers successfully.";
            }
            catch (Exception ex)
            {
                logger.WriteError($"Error in CustomerRepository.BulkInsertCustomers: {ex.Message}");
                throw;
            }
        }

        public async Task<MCustomer?> GetByCode(string customerCode)
        {
            return await appDbContext.MCustomer
                .FirstOrDefaultAsync(p => p.CustomerCode == customerCode && p.IsActive);
        }
    }
}