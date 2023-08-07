using Microsoft.OpenAIRateLimiter.Service.Common;
using Microsoft.OpenAIRateLimiter.Service.Models;
using Newtonsoft.Json.Linq;

namespace Microsoft.OpenAIRateLimiter.Service
{
    public class ParseService : IParseService
    {

        public async Task<QuotaTransDTO> Parse(QuotaEntry entry)
        {

            var retVal = new QuotaTransDTO();

            if (entry.SubscriptionKey is null)
                throw new Exception("SubscriptionKey is Null");

            retVal.subscription = entry.SubscriptionKey;

            if (entry.ResponseBody.Contains("data: "))
            {
                var splitData = entry.ResponseBody.Split("data: ");

                retVal.TotalTokens = splitData.Length - 1;

                var objRes = await GetObject(splitData[0]);

                retVal.Model = objRes["model"]?.ToString() ?? "";

                //TODO get Prompt Tokens

            }
            else
            {

                var objRes = await GetObject(entry.ResponseBody);

                retVal.Model = objRes["model"]?.ToString() ?? "";
                if (!(objRes["usage"]is null))
                {
                    retVal.TotalTokens = Convert.ToInt32(objRes["usage"]["total_tokens"]);
                    retVal.PromptTokens = Convert.ToInt32(objRes["usage"]["prompt_tokens"]);
                }
            }

            //TODO need a CostService

            return retVal;
        }

        private async Task<JObject> GetObject(string value)
        {
            return await new TaskFactory().StartNew(() => JObject.Parse(value));
        }

    }
}
