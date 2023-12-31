﻿using Microsoft.OpenAIRateLimiter.Service.Models;
using Newtonsoft.Json.Linq;

namespace Microsoft.OpenAIRateLimiter.Service
{
    public class ParseService : IParseService
    {
        private readonly ITokenService _svc;

        public ParseService(ITokenService tokenService)
        {
            _svc = tokenService;
        }

        public async Task<QuotaTransDTO> Parse(QuotaEntry entry)
        {

            var retVal = new QuotaTransDTO();

            if (entry.SubscriptionKey is null)
                throw new Exception("SubscriptionKey is Null");

            retVal.subscription = entry.SubscriptionKey;

            if (entry.ResponseBody.Contains("data: "))
            {
                var splitData = entry.ResponseBody.Split("data: ", StringSplitOptions.RemoveEmptyEntries);

                retVal.TotalTokens = splitData.Length - 1;

                var objRes = GetObject(splitData[0]);

                retVal.Model = objRes["model"]?.ToString() ?? "";

                retVal.PromptTokens = await _svc.GetTokenCount(entry.Prompt, retVal.Model);

            }
            else
            {

                var objRes = GetObject(entry.ResponseBody);

                retVal.Model = objRes["model"]?.ToString() ?? "";
                if (!(objRes["usage"]is null))
                {
                    retVal.TotalTokens = Convert.ToInt32(objRes["usage"]["total_tokens"]); 
                    retVal.PromptTokens = Convert.ToInt32(objRes["usage"]["prompt_tokens"]);
                }
            }

            retVal.Value = _svc.CalculateCost(retVal.TotalTokens - retVal.PromptTokens, retVal.PromptTokens, retVal.Model);
            //retVal.Value = retVal.TotalTokens;

            return retVal;
        }

        private JObject GetObject(string value)
        {
            return JObject.Parse(value);
        }

    }
}