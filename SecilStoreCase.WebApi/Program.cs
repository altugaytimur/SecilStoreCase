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
        applicationName: "Secil", // Bu de�eri uygun �ekilde de�i�tirin
        connectionString: mongoSettings.ConnectionString,
        refreshTimerIntervalInMs: 60000 // �rne�in, 60 saniye
    );
});
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

app.Run();

