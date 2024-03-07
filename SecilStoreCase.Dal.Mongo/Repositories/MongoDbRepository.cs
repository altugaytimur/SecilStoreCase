using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SecilStoreCase.Dal.Interfaces.Repositories;
using SecilStoreCase.Entities.Entities;

namespace SecilStoreCase.Dal.Mongo.Repositories
{
    public class MongoDbRepository : IConfigurationItemRepository
    {
        private readonly IMongoCollection<ConfigurationItemModel> _configurations;

        public MongoDbRepository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _configurations = database.GetCollection<ConfigurationItemModel>(settings.Value.CollectionName);
        }

        public async Task CreateConfigurationAsync(ConfigurationItemModel configurationItemModel)
        {
            configurationItemModel.Id = ObjectId.GenerateNewId().ToString();
            await _configurations.InsertOneAsync(configurationItemModel);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _configurations.DeleteOneAsync(configuration => configuration.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<IEnumerable<ConfigurationItemModel>> GetAllConfigurationsAsync(string applicationName)
        {
            return await _configurations
                .Find(configuration => configuration.ApplicationName == applicationName)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConfigurationItemModel>> GetAllConfigurationsAsync()
        {
            var result = await _configurations.Find(_ => true).ToListAsync();
            return result;
        }

        public async Task<ConfigurationItemModel> GetConfigurationByIdAsync(string id)
        {
            return await _configurations
                .Find(configuration => configuration.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateConfigurationAsync(ConfigurationItemModel configurationItemModel)
        {
            var result = await _configurations.ReplaceOneAsync(configuration => configuration.Id == configurationItemModel.Id, configurationItemModel);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
