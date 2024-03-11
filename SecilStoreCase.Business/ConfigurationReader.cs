using MongoDB.Driver;
using Newtonsoft.Json;
using SecilStoreCase.Entities.Entities;
using SecilStoreCase.Entities.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SecilStoreCase.Business;

public class ConfigurationReader
{
    private readonly string _applicationName;
    private readonly IMongoCollection<ConfigurationItemModel> _configurations;
    private readonly ConcurrentDictionary<string, (string Value, DateTime LastUpdated)> _cache = new();
    private readonly TimeSpan _refreshInterval;

    public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
    {
        _applicationName = applicationName;
        _refreshInterval = TimeSpan.FromMilliseconds(refreshTimerIntervalInMs);

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("SecilStoreCase");
        _configurations = database.GetCollection<ConfigurationItemModel>("SecilStoreCaseDB");

        StartRefreshTask();
    }
    public async Task CheckForUpdatesAndNotify()
    {
        var latestUpdate = _cache.Values.Any() ? _cache.Values.OrderByDescending(v => v.LastUpdated).First().LastUpdated : DateTime.MinValue;


        var filter = Builders<ConfigurationItemModel>.Filter.Eq(c => c.ApplicationName, _applicationName) &
                     Builders<ConfigurationItemModel>.Filter.Gt(c => c.LastUpdated, latestUpdate);

        var updates = await _configurations.Find(filter).ToListAsync();

        foreach (var update in updates)
        {
            _cache.AddOrUpdate(update.Name, (update.Value, DateTime.UtcNow), (key, oldValue) => (update.Value, DateTime.UtcNow));

        }
    }

    private void StartRefreshTask()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    await RefreshConfigurations();
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine($"An error occurred during configuration refresh: {ex.Message}");
                }
                await Task.Delay(_refreshInterval);
            }
        });
    }

    private async Task RefreshConfigurations()
    {
        try
        {
            var filter = Builders<ConfigurationItemModel>.Filter.Eq(c => c.ApplicationName, _applicationName) &
                         Builders<ConfigurationItemModel>.Filter.Eq(c => c.IsActive, true);
            var configurations = await _configurations.Find(filter).ToListAsync();

            foreach (var config in configurations)
            {
                _cache.AddOrUpdate(config.Name, (config.Value, DateTime.UtcNow), (key, oldValue) => (config.Value, DateTime.UtcNow));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not refresh configurations from MongoDB: {ex.Message}. Continuing with cached values.");

        }
    }

    public T GetValue<T>(string key)
    {
        var configuration = _configurations.Find(Builders<ConfigurationItemModel>.Filter.Eq(c => c.Name, key) & Builders<ConfigurationItemModel>.Filter.Eq(c => c.IsActive, true)).FirstOrDefault();

        if (configuration == null)
        {
            throw new KeyNotFoundException($"Configuration key '{key}' not found for application '{_applicationName}'.");
        }

        object value = configuration.Value;

        try
        {
            switch (configuration.ConfigurationValueType)
            {
                case ConfigurationValueType.String:
                    if (typeof(T) == typeof(string))
                        return (T)value;
                    break;
                case ConfigurationValueType.Int:
                    if (typeof(T) == typeof(int))
                        value = int.Parse(configuration.Value);
                    break;
                case ConfigurationValueType.Bool:
                    if (typeof(T) == typeof(bool))
                        value = bool.Parse(configuration.Value);
                    break;
                case ConfigurationValueType.Double:
                    if (typeof(T) == typeof(double))
                        value = double.Parse(configuration.Value);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported configuration type '{configuration.ConfigurationValueType}' for key '{key}'.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing configuration value for key '{key}': {ex.Message}", ex);
        }

       
        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (InvalidCastException)
        {
            throw new InvalidOperationException($"Configuration value for key '{key}' cannot be converted to type '{typeof(T).Name}'.");
        }
    }
}
