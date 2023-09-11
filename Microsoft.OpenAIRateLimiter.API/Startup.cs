
using Azure.Core;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenAIRateLimiter.Service;
using System;
using System.Net;

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

            builder.Services.AddHttpClient("Tokenizer", httpClient =>
            {
                httpClient.BaseAddress = new Uri(config["TokenizerURL"]);

                httpClient.DefaultRequestHeaders.Add("x-functions-key", config["TokenizerKey"]);

            });

            builder.Services.AddTransient<ITokenService, TokenService>();

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

            //builder.Services.AddTransient(Provider => {
            //    return new(
            //        accountEndpoint: Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")!,
            //        tokenCredential: new DefaultAzureCredential()
            //    );

            //});
        }
    }
}
