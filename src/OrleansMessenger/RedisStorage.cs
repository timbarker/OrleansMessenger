using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using StackExchange.Redis;

namespace OrleansMessenger
{
    public class RedisStorage : IStorageProvider
    {
        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _redisDatabase;

        private string _serviceId;
        private JsonSerializer _serialiser;

        public string Name { get; private set; }

        public Logger Log { get; private set; }

        public async Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            Name = name;
            _serviceId = providerRuntime.ServiceId.ToString();

            if (!config.Properties.ContainsKey("RedisConnectionString") ||
                string.IsNullOrWhiteSpace(config.Properties["RedisConnectionString"]))
            {
                throw new ArgumentException("RedisConnectionString is not set.");
            }
            var connectionString = config.Properties["RedisConnectionString"];

            _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

            if (!config.Properties.ContainsKey("DatabaseNumber") ||
                string.IsNullOrWhiteSpace(config.Properties["DatabaseNumber"]))
            {
                _redisDatabase = _connectionMultiplexer.GetDatabase();
            }
            else
            {
                var databaseNumber = Convert.ToInt16(config.Properties["DatabaseNumber"]);
                _redisDatabase = _connectionMultiplexer.GetDatabase(databaseNumber);
            }

            Log = providerRuntime.GetLogger("StorageProvider.RedisStorage." + _serviceId);

            _serialiser = JsonSerializer.Create(new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.All});
        }

        public Task Close()
        {
            _connectionMultiplexer.Dispose();
            return TaskDone.Done;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, GrainState grainState)
        {
            var primaryKey = grainReference.ToKeyString();

            var value = await _redisDatabase.StringGetAsync(primaryKey);
            var data = new Dictionary<string, object>();
            if (value.HasValue)
            {
                data = _serialiser.Deserialize<Dictionary<string, object>>(new JsonTextReader(new StringReader(value)));
            }

            grainState.SetAll(data);

            grainState.Etag = Guid.NewGuid().ToString();
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, GrainState grainState)
        {
            var primaryKey = grainReference.ToKeyString();
            var data = grainState.AsDictionary();
            var json = new StringBuilder();
            _serialiser.Serialize(new StringWriter(json), data);
            await _redisDatabase.StringSetAsync(primaryKey, json.ToString());
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, GrainState grainState)
        {
            var primaryKey = grainReference.ToKeyString();
            _redisDatabase.KeyDelete(primaryKey);
            return TaskDone.Done;
        }
    }
}