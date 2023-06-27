using System.Text.Json.Serialization;

namespace Microsoft.OpenAIRateLimiter.API.Models
{
    public class QuotaEntry
    {

        [JsonPropertyName("subscriptionKey")]
        public string SubscriptionKey { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("prompTokens")]
        public string PrompTokens { get; set; }

        [JsonPropertyName("completionTokens")]
        public string CompletionTokens { get; set; }

        [JsonPropertyName("totalTokens")]
        public string TotalTokens { get; set; }

    }
}