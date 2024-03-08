using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Options;
using SecilStoreCase.Business;
using SecilStoreCase.Dal.Mongo.Extensions;
using SecilStoreCase.Entities.Entities;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMongoDBServices(builder.Configuration);
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDBConnection"));

builder.Services.AddSingleton<ConfigurationReader>(serviceProvider =>
{
    var mongoSettings = serviceProvider.GetService<IOptions<MongoDbSettings>>().Value;
    return new ConfigurationReader(
        applicationName: "Secil", 
        connectionString: mongoSettings.ConnectionString,
        refreshTimerIntervalInMs: 60000 
    );
});

builder.Services.AddHangfire(configuration => configuration.UseMongoStorage(
    builder.Configuration.GetConnectionString("HangfireConnection"), "hangfireDB",
    new MongoStorageOptions
    {
        MigrationOptions = new MongoMigrationOptions
        {
            MigrationStrategy = new MigrateMongoMigrationStrategy(),
            BackupStrategy = new NoneMongoBackupStrategy()
        }
    }));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

app.Services.GetRequiredService<IRecurringJobManager>().AddOrUpdate(
    "CheckConfigUpdates",
    () => app.Services.GetRequiredService<ConfigurationReader>().CheckForUpdatesAndNotify(),
    Cron.MinuteInterval(1));

app.Run();

