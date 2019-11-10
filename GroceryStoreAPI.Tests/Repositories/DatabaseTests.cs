using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace GroceryStoreAPI.Repositories.Tests
{
    public class DatabaseTests
    {
        readonly string path;

        public DatabaseTests()
        {
            path = Path.GetTempFileName();
        }

        ~DatabaseTests()
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        [Fact]
        public void GetTable_MissingTable_CreatesTable()
        {
            var database = new Database(path);

            // Act
            var table = database.GetTable("orders");

            // Assert
            Assert.NotNull(table);
        }

        [Fact]
        public async Task LoadAsync()
        {
            File.WriteAllText(path, "{\"orders\":[{\"id\":123}]}");

            var database = new Database(path);

            await database.LoadAsync();

            // Act
            var table = database.GetTable("orders");

            // Assert
            Assert.Collection(table, (t) => Assert.Equal(123, t["id"]));
        }

        [Fact]
        public async Task SaveAsync()
        {
            var database = new Database(path);

            dynamic product = new
            {
                Id = 123,
                Description = "test"
            };

            database.GetTable("orders").Add(JObject.FromObject(product, RepositorySerializer.Instance));

            // Act
            await database.SaveAsync();

            // Assert
            var saved = File.ReadAllText(path);

            Assert.Equal("{\r\n  \"orders\": [\r\n    {\r\n      \"id\": 123,\r\n      \"description\": \"test\"\r\n    }\r\n  ]\r\n}", saved);
        }
    }
}
