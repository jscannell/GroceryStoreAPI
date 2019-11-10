using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Repositories
{
    public interface IDatabase
    {
        JArray GetTable(string name);
        Task LoadAsync();
        Task SaveAsync();
    }

    public class Database : IDatabase
    {
        readonly string _path;
        private JObject _data = new JObject();

        public Database(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            _path = path;
        }

        public async Task LoadAsync()
        {
            using (var file = File.OpenText(_path))
            {
                using (var reader = new JsonTextReader(file))
                    _data = (JObject)await JToken.ReadFromAsync(reader);
            }
        }

        readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1);

        public async Task SaveAsync()
        {
            await _writeLock.WaitAsync();

            try
            {
                using (var file = File.CreateText(_path))
                {
                    using (var writer = new JsonTextWriter(file) { Formatting = Formatting.Indented })
                        await _data.WriteToAsync(writer);
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public JArray GetTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var table = (JArray)_data[name];

            if (table == null)
            {
                table = new JArray();

                _data.Add(new JProperty(name, table));
            }

            return table;
        }
    }
}
