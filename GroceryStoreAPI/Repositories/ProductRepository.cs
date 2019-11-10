using GroceryStoreAPI.Entities;

namespace GroceryStoreAPI.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
    }

    public sealed class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(IDatabase database)
            : base(database, "products")
        {
        }
    }
}
