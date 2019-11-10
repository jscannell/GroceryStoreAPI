using GroceryStoreAPI.Entities;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace GroceryStoreAPI.Repositories.Tests
{
    public class RepositoryTests
    {
        class TestEntity : Entity
        {
            public string Value { get; set; }
        }

        private Database BuildDatabase(params TestEntity[] entities)
        {
            var database = new Database("test");
            var table = database.GetTable("tests");

            foreach (var entity in entities)
                table.Add(JToken.FromObject(entity, RepositorySerializer.Instance));

            return database;
        }

        [Fact]
        public void LoadById()
        {
            var expectedId = 123;

            var db = BuildDatabase(new TestEntity { Id = expectedId });

            var repository = new Repository<TestEntity>(db, "tests");

            // Act
            var loaded = repository.LoadById(expectedId);

            // Assert
            Assert.Equal(expectedId, loaded.Id);
        }

        [Fact]
        public void LoadById_RecordNotFound_ReturnsNull()
        {
            var expectedId = 123;

            var db = BuildDatabase(new TestEntity { Id = expectedId });

            var repository = new Repository<TestEntity>(db, "tests");

            // Act
            var loaded = repository.LoadById(expectedId + 1);

            // Assert
            Assert.Null(loaded);
        }

        [Fact]
        public void LoadAll()
        {
            var db = BuildDatabase(
                new TestEntity { Id = 100 },
                new TestEntity { Id = 200 });

            var repository = new Repository<TestEntity>(db, "tests");

            // Act
            var loaded = repository.LoadAll();

            // Assert
            Assert.Collection(loaded,
                (e) => Assert.Equal(100, e.Id),
                (e) => Assert.Equal(200, e.Id));
        }

        [Fact]
        public async Task SaveAsync_SavesDatabase()
        {
            var tests = new JArray();

            var db = Substitute.For<IDatabase>();

            db.GetTable("tests").Returns(tests);

            var repository = new Repository<TestEntity>(db, "tests");

            var entity = new TestEntity { Id = 123 };

            // Act
            await repository.SaveAsync(entity);

            // Assert
            await db.Received().SaveAsync();
        }

        [Fact]
        public async Task SaveAsync_AddsRecord()
        {
            var tests = new JArray();

            var db = Substitute.For<IDatabase>();

            db.GetTable("tests").Returns(tests);

            var repository = new Repository<TestEntity>(db, "tests");

            var entity = new TestEntity { Id = 123 };

            // Act
            await repository.SaveAsync(entity);

            // Assert
            Assert.Collection(tests,
                (r) => Assert.Equal(123, (int)r["id"]));
        }

        [Fact]
        public async Task SaveAsync_UpdatesRecord()
        {
            var tests = new JArray();

            var db = Substitute.For<IDatabase>();

            db.GetTable("tests").Returns(tests);

            var repository = new Repository<TestEntity>(db, "tests");

            var entity = new TestEntity { Id = 123 };

            await repository.SaveAsync(entity);

            // Act
            entity.Value = "testvalue";

            await repository.SaveAsync(entity);

            // Assert
            Assert.Collection(tests,
                (r) => Assert.Equal(entity.Value, (string)r["value"]));
        }
    }
}
