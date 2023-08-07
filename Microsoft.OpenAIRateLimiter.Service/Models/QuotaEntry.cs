using System.Text.Json.Serialization;

namespace Microsoft.OpenAIRateLimiter.Service.Models
{
    public class QuotaEntry
    {

        [JsonPropertyName("subscriptionKey")]
        public string SubscriptionKey { get; set; } = string.Empty;

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonPropertyName("responseBody")]
        public string ResponseBody { get; set; } = string.Empty;

    }
}