using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB{

    public static class Extensions{
        public static IServiceCollection AddMongo(this IServiceCollection service){
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            service.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var settings = configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>();
                var mongoClient = new MongoClient(settings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            return service;

        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection service, string collectionName) where T : IEntity
        {
            service.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetRequiredService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });

            return service;

        }
    }
}