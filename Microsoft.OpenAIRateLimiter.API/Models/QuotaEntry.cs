using System.Text.Json.Serialization;

namespace Microsoft.OpenAIRateLimiter.API.Models
{
    public class QuotaEntry
    {

        [JsonPropertyName("subscriptionKey")]
        public string subscriptionKey { get; set; }

        [JsonPropertyName("prompt")]
        public string prompt { get; set; }

        [JsonPropertyName("responseBody")]
        public string responseBody { get; set; }

    }
}