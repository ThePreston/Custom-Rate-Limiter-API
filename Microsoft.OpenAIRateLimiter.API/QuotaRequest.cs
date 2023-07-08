using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenAIRateLimiter.API.Models;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.OpenAIRateLimiter.Service;
using Microsoft.OpenAIRateLimiter.Service.Common;
using Microsoft.OpenAIRateLimiter.Service.Models;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenAIRateLimiter.API
{
    public class QuotaRequest
    {
        private readonly ILogger<QuotaRequest> _logger;

        private readonly IQuotaService _svc;

        public QuotaRequest(ILogger<QuotaRequest> log, IQuotaService quotaService)
        {
            _logger = log;
            _svc = quotaService;
        }

        [FunctionName("Create")]
        [OpenApiOperation(operationId: "Create")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(KVQuota), Required = true, Description = "The minimum required parameters")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(Exception), Description = "Exception")]
        public async Task<HttpResponseMessage> CreateQuota(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Quota/")] HttpRequest req)
        {
            _logger.LogInformation($"Entered CreateQuota");

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                _logger.LogInformation($"Request Body = {requestBody}");

                var data = JsonConvert.DeserializeObject<KVQuota>(requestBody);

                if (data?.SubscriptionKey is null)
                    return HttpUtilities.RESTResponse(data?.SubscriptionKey);

                if(data?.Amount is null)
                    return HttpUtilities.RESTResponse(data?.Amount);

                return HttpUtilities.RESTResponse(await _svc.Create(new QuotaDTO() { Key = data.SubscriptionKey, Value = Convert.ToInt32(data.Amount) }));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return HttpUtilities.RESTResponse(ex);
            }

        }

        [FunctionName("Update")]
        [OpenApiOperation(operationId: "Update")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(QuotaEntry), Required = true, Description = "The minimum required parameters")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(Exception), Description = "Exception")]
        public async Task<HttpResponseMessage> UpdateQuota(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Quota/Update")] HttpRequest req)
        {
            _logger.LogInformation($"Entered UpdateQuota");

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                _logger.LogInformation($"Request Body = {requestBody}");

                var data = JsonConvert.DeserializeObject<QuotaEntry>(requestBody);

                if (data?.SubscriptionKey is null)
                    return HttpUtilities.RESTResponse(data?.SubscriptionKey);

                return HttpUtilities.RESTResponse(await _svc.Update(new QuotaTransDTO() { Key = data.SubscriptionKey, 
                                                                                          Value = CalculateAmount(data),
                                                                                          Model = data.Model  }));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return HttpUtilities.RESTResponse(ex);
            }

        }

        private static int CalculateAmount(QuotaEntry entry)
        {
            return (entry?.TotalTokens == null ? 0 : Convert.ToInt32(entry.TotalTokens)) +
                   (entry?.PrompTokens == null ? 0 : Convert.ToInt32(entry.PrompTokens)) +
                   (entry?.CompletionTokens == null ? 0 : Convert.ToInt32(entry.CompletionTokens));

        }

        [FunctionName("GetQuotaByKey")]
        [OpenApiOperation(operationId: "GetQuotaByKey")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]        
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(int), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(Exception), Description = "Exception")]
        public async Task<HttpResponseMessage> GetQuotaByKey(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Quota/{keyId}")] HttpRequest req, string keyId)
        {
            _logger.LogInformation($"Entered GetQuotaByKey Key = {keyId}");

            try
            {
                req.ToString();

                if (string.IsNullOrEmpty(keyId))
                    return HttpUtilities.RESTResponse(keyId);

                var retVal = await _svc.GetById(keyId);

                _logger.LogInformation($"returned valye from _svc.GetById = {retVal}");

                return HttpUtilities.RESTResponse(retVal);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return HttpUtilities.RESTResponse(ex);

            }

        }

        [FunctionName("GetAll")]
        [OpenApiOperation(operationId: "GetAll")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<KVQuota>), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(Exception), Description = "Exception")]
        public async Task<HttpResponseMessage> GetAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Quota/")] HttpRequest req)
        {
            _logger.LogInformation("Entered GetAll");

            try
            {
                req.ToString();

                var allQuotas = await _svc.GetAll();

                var convertedQuotas = allQuotas.ToList().Select(x => new KVQuota() { SubscriptionKey = x.Key, Amount = x.Value.ToString() });

                return HttpUtilities.RESTResponse(convertedQuotas);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return HttpUtilities.RESTResponse(ex);

            }

        }
    }
}