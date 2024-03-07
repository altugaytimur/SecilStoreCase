using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SecilStoreCase.Dal.Interfaces.Repositories;
using SecilStoreCase.Dal.Mongo.Repositories;
using SecilStoreCase.Entities.Entities;

namespace SecilStoreCase.Dal.Mongo.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMongoDBServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbConnection"));
            services.AddSingleton<IMongoDatabase>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var client=new MongoClient(settings.ConnectionString);
                return client.GetDatabase(settings.DatabaseName);
            });
            services.AddScoped<IConfigurationItemRepository, MongoDbRepository>();
            return services;
            
        }
    }
}
