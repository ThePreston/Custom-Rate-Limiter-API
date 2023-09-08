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

        [JsonPropertyName("tokenAmount")]
        public int TokenAmount { get; set; } = default!;

        [JsonPropertyName("monthlyAmount")]
        public int MonthlyAmount { get; set; } = default!;

        [JsonPropertyName("createdTime")]
        public string CreatedTime { get; set; } = default!;
    }
}