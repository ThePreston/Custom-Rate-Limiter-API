
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenAIRateLimiter.Service;

[assembly: FunctionsStartup(typeof(Microsoft.OpenAIRateLimiter.API.Startup))]
namespace Microsoft.OpenAIRateLimiter.API
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            builder.Services.AddLogging();

            builder.Services.AddTransient<IParseService, ParseService>();

            builder.Services.AddScoped<IQuotaService, QuotaService>();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("RedisConn");
                options.InstanceName = config["RedisInstance"];
            });

            builder.Services.AddScoped(Provider =>
            {
                return new TableServiceClient(config.GetConnectionString("StorageConn")).GetTableClient(config["TableName"]);
                
            });

        }
    }
}
