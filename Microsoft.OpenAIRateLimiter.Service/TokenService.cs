using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text;

namespace Microsoft.OpenAIRateLimiter.Service
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient _client;

        public TokenService(IHttpClientFactory httpClientFactory)
        {

            _client = httpClientFactory.CreateClient("Tokenizer");
        }

        public async Task<int> GetTokenCount(string prompt, string model)
        {

            var payload = new { input = prompt, model = GetModel(model) };

            HttpContent c = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var resp = await _client.PostAsync("/api/tokenize", c);

            var body = await resp.Content.ReadAsStringAsync();

            return Convert.ToInt32(JObject.Parse(body)["num_tokens"]);

        }

        public decimal CalculateCost(int completionTokens, int promptTokens, string model)
        {

            decimal retVal = 0M;

            //var dt = new DataTable();
            //var v = dt.Compute($"({totalToken} / 1000) * .002", "");

            //TODO change cost calculation to include Prompt tokens and Completion tokens

            switch (model.Trim().ToLower())
            {
                case "gpt-35-turbo":
                    retVal = Convert.ToDecimal(promptTokens) / 1000M * .0015M;
                    retVal += Convert.ToDecimal(completionTokens) / 1000M * .002M; 
                    break;

                case "gpt-4":
                    retVal = Convert.ToDecimal(promptTokens) / 1000M * .03M;
                    retVal += Convert.ToDecimal(completionTokens) / 1000M * .06M;
                    break;

                case "gpt-4-32k":
                    retVal = Convert.ToDecimal(promptTokens) / 1000M * .06M;
                    retVal += Convert.ToDecimal(completionTokens) / 1000M * .12M;
                    break;

                default:
                    retVal = Convert.ToDecimal(promptTokens) / 1000M * .003M;
                    retVal += Convert.ToDecimal(completionTokens) / 1000M * .004M;
                    break;
            }

            return retVal;
        }

        #region Private

        private string GetModel(string model)
        {
            var retVal = "";

            switch (model.Trim().ToLower())
            {
                case "gpt-35-turbo":
                    retVal = "gpt-3.5-turbo";
                    break;

                case "gpt-4":
                    retVal = "gpt-4";
                    break;

                default:
                    break;
            }

            return retVal;
        }

        #endregion
    }
}