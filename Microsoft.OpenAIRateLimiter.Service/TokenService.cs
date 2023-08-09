using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public double CalculateCost(int TotalTokens, string model)
        {

            var retVal = 0.000;

            switch (model.Trim().ToLower())
            {
                case "gpt-35-turbo":
                    retVal = (TotalTokens / 1000) * .002;
                    break;

                case "gpt-4":
                    retVal = (TotalTokens / 1000) * .03;
                    break;

                default:
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