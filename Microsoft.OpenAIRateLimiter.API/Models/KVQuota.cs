using System.Text.Json.Serialization;

namespace Microsoft.OpenAIRateLimiter.API.Models
{
    public class KVQuota
    {
        [JsonPropertyName("subscriptionKey")]
        public string SubscriptionKey { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }   

    }
}