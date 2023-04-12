using Plau.Inventory.Service.Clients;
using Play.Common.MongoDB;
using Play.Common.MassTransit;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMongo()
                .AddMongoRepository<InventoryItem>("inventoryitems")
                .AddMongoRepository<CatalogItem>("catalogitems")
                .AddMassTransitRabbitMQ();

builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5034");
})
.AddTransientHttpErrorPolicy(builder => {
    return builder.WaitAndRetryAsync(new[]
    {
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromMilliseconds(1000),
        TimeSpan.FromMilliseconds(2000)
    });
})
.AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(15), onBreak: (ex, ts) =>
        {   
            //change it to logger
            Console.WriteLine($"Circuit breaker opened - time {ts.TotalSeconds}");
        }, onReset: () =>
        {
            //change it to logger 
            Console.WriteLine("Circuit breaker reset");
        }))
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(1000)));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
