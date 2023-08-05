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
        private readonly IQuotaService _svc;

        public QuotaRequest(IQuotaService quotaService)
        {
            _svc = quotaService;
        }

        [FunctionName("Create")]
        [OpenApiOperation(operationId: "Create")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(KVQuota), Required = true, Description = "The minimum required parameters")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(Exception), Description = "Exception")]
        public async Task<HttpResponseMessage> CreateQuota(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Quota/")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Entered CreateQuota");

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                log.LogInformation($"Request Body = {requestBody}");

                var data = JsonConvert.DeserializeObject<KVQuota>(requestBody);

                if (data?.SubscriptionKey is null)
                    return HttpUtilities.RESTResponse(data?.SubscriptionKey);

                if (data?.ProductName is null)
                    return HttpUtilities.RESTResponse(data?.ProductName);

                if (data?.Amount is null)
                    return HttpUtilities.RESTResponse(data?.Amount);

                return HttpUtilities.RESTResponse(await _svc.Create(new QuotaDTO() { Key = data.SubscriptionKey, 
                                                                                     Product = data.ProductName,
                                                                                     Value = Convert.ToInt32(data.Amount) }));

            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Quota/Update")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Entered UpdateQuota");

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var quotaObj = JsonConvert.DeserializeObject<QuotaGTO>(requestBody);

                log.LogInformation($"Request Body = {requestBody}");

                if (quotaObj.responseBody.Contains("data: "))
                {
                    var splitData = quotaObj.responseBody.Split("data: ");

                    log.LogInformation($"number of records = {splitData.Length - 1}");

                    //TODO send to a cost Calculator
                    return HttpUtilities.RESTResponse("true");

                }
                else
                {

                    var data = JsonConvert.DeserializeObject<QuotaEntry>(quotaObj.responseBody);

                    if (data?.SubscriptionKey is null)
                        return HttpUtilities.RESTResponse(data?.SubscriptionKey);

                    if (data?.Model is null)
                        return HttpUtilities.RESTResponse(data?.Model);

                    //TODO send to a cost Calculator then to the service
                    return HttpUtilities.RESTResponse(await _svc.Update(new QuotaTransDTO() { Key = data.SubscriptionKey,
                                                                                              Value = CalculateAmount(data),
                                                                                              Model = data.Model }));
                
                }

            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                return HttpUtilities.RESTResponse(ex);
            }

        }

        private static int CalculateAmount(QuotaEntry entry)
        {
            /* 
                Formula:
                    TotalTokens / TokenNumberThreshold * Model Cost

                Formula for gpt-3.5-turbo
                    entry.TotalTokens / 1000 X .002
            
            return entry?.TotalTokens / 1000 * .002;
            */

            return (entry?.TotalTokens == null ? 0 : Convert.ToInt32(entry.TotalTokens));

        }

        [FunctionName("BudgetAlertEndpoint")]
        [OpenApiOperation(operationId: "Budget")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(string), Required = true, Description = "The minimum required parameters")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(Exception), Description = "Exception")]
        public async Task<HttpResponseMessage> BudgetAlertEndpoint(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Quota/Budget")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Entered BudgetAlertEndpoint");

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                log.LogInformation($"Request Body = {requestBody}");

                var alert = JsonConvert.DeserializeObject<BudgetAlert>(requestBody);

                if (alert?.data?.alertContext?.AlertData?.BudgetName is null) {
                    log.LogError($"Missing Budget Name = {requestBody}");
                    return HttpUtilities.RESTResponse(alert?.data?.alertContext?.AlertData?.BudgetName);
                }

                return HttpUtilities.RESTResponse(await _svc.BudgetUpdate(new QuotaDTO() { Product = alert.data.alertContext.AlertData.BudgetName, Value = 0 }));

            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                return HttpUtilities.RESTResponse(ex);
            }

        }

        [FunctionName("GetQuotaByKey")]
        [OpenApiOperation(operationId: "GetQuotaByKey")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "keyId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The subscription id of the Quota key")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(KVQuota), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(Exception), Description = "Exception")]
        public async Task<HttpResponseMessage> GetQuotaByKey(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Quota/{keyId}")] HttpRequest req, string keyId, ILogger log)
        {
            log.LogInformation($"Entered GetQuotaByKey Key = {keyId}");

            try
            {
                req.ToString();

                if (string.IsNullOrEmpty(keyId))
                    return HttpUtilities.RESTResponse(keyId);

                var retVal = await _svc.GetById(keyId);

                log.LogInformation($"returned valye from _svc.GetById = {retVal}");

                return HttpUtilities.RESTResponse(new KVQuota() { SubscriptionKey = keyId, Amount = retVal.ToString() });

            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                return HttpUtilities.RESTResponse(ex);

            }

        }

        [FunctionName("GetAll")]
        [OpenApiOperation(operationId: "GetAll")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<KVQuota>), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(Exception), Description = "Exception")]
        public async Task<HttpResponseMessage> GetAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Quota/")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Entered GetAll");

            try
            {
                req.ToString();

                var allQuotas = await _svc.GetAll();

                var convertedQuotas = allQuotas.ToList().Select(x => new KVQuota() { SubscriptionKey = x.Key, 
                                                                                     ProductName = x.Product, 
                                                                                     Amount = x.Value.ToString() });

                return HttpUtilities.RESTResponse(convertedQuotas);

            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                return HttpUtilities.RESTResponse(ex);

            }

        }
    }
}