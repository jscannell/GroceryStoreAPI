using GroceryStoreAPI.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Repositories
{
    public interface IRepository<T> where T : Entity, new()
    {
        T LoadById(int id);

        IEnumerable<T> LoadAll();

        Task SaveAsync(T entity);
    }

    public class RepositorySerializer : JsonSerializer
    {
        public RepositorySerializer()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
        }

        public static RepositorySerializer Instance => new RepositorySerializer();
    }

    public class Repository<T> : IRepository<T> where T : Entity, new()
    {
        readonly IDatabase _database;
        readonly string _tableName;

        public Repository(IDatabase database, string tableName)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            _database = database;
            _tableName = tableName;
        }

        protected JArray GetTable()
        {
            return _database.GetTable(_tableName);
        }

        protected T Deserialize(JToken record)
        {
            return record.ToObject<T>(RepositorySerializer.Instance);
        }

        protected JToken Serialize(T entity)
        {
            return JToken.FromObject(entity, RepositorySerializer.Instance);
        }

        public T LoadById(int id)
        {
            foreach (var record in GetTable())
            {
                if (id == (int)record["id"])
                    return Deserialize(record);
            }

            return null;
        }

        public IEnumerable<T> LoadAll()
        {
            foreach (var record in GetTable())
                yield return Deserialize(record);
        }

        private void SaveRecord(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var token = Serialize(entity);
            var table = _database.GetTable(_tableName);

            foreach (var record in table)
            {
                var id = (int)record["id"];

                if (id == entity.Id)
                {
                    record.Replace(token);
                    return;
                }
            }

            table.Add(token);
        }

        public Task SaveAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            SaveRecord(entity);

            return _database.SaveAsync();
        }
    }
}
