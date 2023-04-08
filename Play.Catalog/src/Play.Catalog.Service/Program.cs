using Play.Catalog.Service;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Serilog;
using Play.Catalog.Service.Settings;
using Play.Catalog.Service.Repositories;
using MongoDB.Driver;


//Creating logger
Log.Logger = new LoggerConfiguration()
     .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ResponseStatusCode} {RequestMethod} {RequestPath}{NewLine}{Message}{NewLine}{Exception}")
     .CreateLogger();
     

Log.Information("Application is starting");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
    BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
    options.SuppressAsyncSuffixInActionNames = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();
builder.Services.AddSingleton( serviceProvider =>{
    var settings = builder.Configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>();
    var mongoClient = new MongoClient(settings.ConnectionString);
    return mongoClient.GetDatabase(builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>().ServiceName);
});

builder.Services.AddSingleton<IItemRepository, ItemRepository>();

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

Log.Fatal("Application is shutting down");