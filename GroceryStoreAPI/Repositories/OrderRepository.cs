using GroceryStoreAPI.Entities;
using System;
using System.Collections.Generic;

namespace GroceryStoreAPI.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> LoadByCustomer(int customerId);
        IEnumerable<Order> LoadByDate(DateTime date);
    }

    public sealed class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(IDatabase database)
            : base(database, "orders")
        {
        }

        public IEnumerable<Order> LoadByCustomer(int customerId)
        {
            foreach (var order in LoadAll())
            {
                if (order.CustomerId == customerId)
                    yield return order;
            }
        }

        public IEnumerable<Order> LoadByDate(DateTime date)
        {
            foreach (var record in GetTable())
            {
                var order = Deserialize(record);

                if (order.DateCreated?.Date == date.Date)
                    yield return order;
            }
        }
    }
}
