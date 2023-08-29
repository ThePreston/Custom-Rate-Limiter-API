namespace Microsoft.OpenAIRateLimiter.Service.Models
{
    public  class QuotaTransDTO
    {
        public string subscription { get; set; } = "";

        public decimal Value { get; set; }

        public string Model { get; set; } = "";

        public int PromptTokens { get; set; }

        public int TotalTokens { get; set; }
    }
}
