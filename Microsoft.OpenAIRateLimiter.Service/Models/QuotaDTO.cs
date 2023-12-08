namespace Microsoft.OpenAIRateLimiter.Service.Models
{
    public class QuotaDTO
    {
        public string Key { get; set; } = "";

        public decimal Value { get; set; }

        public string Product { get; set; } = "";

        public bool RateLimitOnCost { get; set; } = true;
    }
}