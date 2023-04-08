using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Microsoft.AspNetCore.Mvc;

namespace Play.Catalog.Service
{

    public class Setup
    {

        public IConfiguration configuration { get; set; }

        public Setup(IWebHostEnvironment env, IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
           
            

        }

    }

}