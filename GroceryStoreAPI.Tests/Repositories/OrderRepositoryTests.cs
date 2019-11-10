using GroceryStoreAPI.Entities;
using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace GroceryStoreAPI.Repositories.Tests
{
    public class OrderRepositoryTests
    {
        private Database BuildDatabase(params Order[] entities)
        {
            var database = new Database("test");
            var table = database.GetTable("orders");

            foreach (var entity in entities)
                table.Add(JToken.FromObject(entity));

            return database;
        }

        [Fact]
        public void LoadByCustomer()
        {
            var expectedOrderId = 123;
            var expectedCustomerId = 456;

            var db = BuildDatabase(
                new Order { Id = expectedOrderId, CustomerId = expectedCustomerId },
                new Order { Id = expectedOrderId + 1, CustomerId = expectedCustomerId + 1 },
                new Order { Id = expectedOrderId + 2, CustomerId = expectedCustomerId });

            var repository = new OrderRepository(db);

            // Act
            var loaded = repository.LoadByCustomer(expectedCustomerId);

            // Assert
            Assert.Collection(loaded,
                (e) => { Assert.Equal(expectedOrderId, e.Id); Assert.Equal(expectedCustomerId, e.CustomerId); },
                (e) => { Assert.Equal(expectedOrderId + 2, e.Id); Assert.Equal(expectedCustomerId, e.CustomerId); });
        }

        [Fact]
        public void LoadByDate()
        {
            var expectedOrderId = 123;
            var expectedDate = new DateTime(2019, 11, 9, 2, 3, 4, DateTimeKind.Utc);

            var db = BuildDatabase(
                new Order { Id = expectedOrderId, DateCreated = expectedDate },
                new Order { Id = expectedOrderId + 1, DateCreated = expectedDate.AddDays(-1) },
                new Order { Id = expectedOrderId + 2, DateCreated = expectedDate });

            var repository = new OrderRepository(db);

            // Act
            var loaded = repository.LoadByDate(expectedDate.AddMinutes(6));

            // Assert
            Assert.Collection(loaded,
                (e) => { Assert.Equal(expectedOrderId, e.Id); Assert.Equal(expectedDate, e.DateCreated); },
                (e) => { Assert.Equal(expectedOrderId + 2, e.Id); Assert.Equal(expectedDate, e.DateCreated); });
        }
    }
}
