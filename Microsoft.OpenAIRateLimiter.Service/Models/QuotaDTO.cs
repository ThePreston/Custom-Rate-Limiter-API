namespace Microsoft.OpenAIRateLimiter.Service.Models
{
    public class QuotaDTO
    {
        public string Key { get; set; } = "";

        public double Value { get; set; }

        public string Product { get; set; } = "";
    }
}