using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Yu.Data.MongoDB
{
    public static class MongoServiceCollectionExtension
    {
        public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoOption>(configuration.GetSection("MongoOption"));

            services.AddSingleton<MongoClientHelper>();
            services.AddSingleton<MongoHelper>();
        }
    }
}
