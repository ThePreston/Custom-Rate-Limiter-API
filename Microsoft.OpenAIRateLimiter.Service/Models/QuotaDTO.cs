namespace Microsoft.OpenAIRateLimiter.Service.Models
{
    public class QuotaDTO
    {
        public string Key { get; set; } = "";

        public int Value { get; set; }

        public string Product { get; set; } = "";
    }
}