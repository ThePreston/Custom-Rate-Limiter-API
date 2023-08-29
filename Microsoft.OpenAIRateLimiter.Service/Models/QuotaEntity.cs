using Azure;
using Azure.Data.Tables;

namespace Microsoft.OpenAIRateLimiter.Service.Models
{
    internal class QuotaEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        
        public string RowKey { get; set; } = default!;
        
        public string ProductName { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;

        public string Model { get; set; } = "";

        public int TotalTokens { get; set; } 

        public int PromptTokens { get; set; } 

        public string Operation { get; set; } = "";

        public decimal Amount { get; set; }

        public ETag ETag { get; set; } = default!;
    }
}
