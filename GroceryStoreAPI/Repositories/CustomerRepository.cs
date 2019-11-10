using GroceryStoreAPI.Entities;

namespace GroceryStoreAPI.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
    }

    public sealed class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IDatabase database)
            : base(database, "customers")
        {
        }
    }
}
