using Serilog;
using Play.Catalog.Entities;
using Play.Common.MongoDB;
using Play.Common.MassTransit;

//Creating logger
Log.Logger = new LoggerConfiguration()
     .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ResponseStatusCode} {RequestMethod} {RequestPath}{NewLine}{Message}{NewLine}{Exception}")
     .CreateLogger();
     

Log.Information("Application is starting");

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigin");

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();
builder.Services.AddMongo()
                .AddMongoRepository<Item>("items")
                .AddMassTransitRabbitMQ();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(allowedOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();

Log.Fatal("Application is shutting down");