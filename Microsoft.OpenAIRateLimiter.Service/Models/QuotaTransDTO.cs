namespace Microsoft.OpenAIRateLimiter.Service.Models
{
    public  class QuotaTransDTO
    {
        public string Key { get; set; } = "";

        public int Value { get; set; }

        public string Model { get; set; } = "";
    }
}
