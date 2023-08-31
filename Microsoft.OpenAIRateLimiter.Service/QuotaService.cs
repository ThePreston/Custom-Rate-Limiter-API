using Azure.Data.Tables;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.OpenAIRateLimiter.Service.Models;
using System.Text;

namespace Microsoft.OpenAIRateLimiter.Service
{
    public class QuotaService : IQuotaService
    {

        private readonly IDistributedCache _cache;

        private readonly TableClient _client;

        private readonly ILogger _logger;

        public QuotaService(IDistributedCache cache, TableClient client, ILogger<QuotaService> logger)
        {
            _cache = cache;
            _client = client;
            _logger = logger;
        }

        public async Task<bool> Create(QuotaDTO quota)
        {

            await PersisttoCache(quota);

            await PersisttoTable(new QuotaEntity() { PartitionKey = quota.Key,
                                                     RowKey = Guid.NewGuid().ToString(), 
                                                     ProductName = quota.Product,
                                                     Operation = "Create",
                                                     Amount = quota.Value });

            return true; //await new TaskFactory().StartNew<bool>(() => { return true; });
            
        }

        public async Task<bool> BudgetUpdate(QuotaDTO quota)
        {

            quota.Key = _client.Query<QuotaEntity>(x => x.ProductName == quota.Product)
                               .Select(z => z.PartitionKey).FirstOrDefault() ?? "";

            if (string.IsNullOrEmpty(quota.Key))
                throw new Exception($"PartitionKey not found for product = {quota.Product} ");

            await PersisttoCache(quota); 

            await PersisttoTable(new QuotaEntity() { PartitionKey = quota.Key,
                                                     RowKey = Guid.NewGuid().ToString(),
                                                     ProductName = quota.Product,
                                                     Operation = "BudgetStop",
                                                     Amount = quota.Value });

            return true;

        }

        public async Task<bool> Update(QuotaTransDTO quota)
        {
            _logger.LogInformation($"Update Value = {quota.Value}; Update Total Tokens = {quota.TotalTokens}");
            var currentAmount = Convert.ToDecimal(await GetById(quota.subscription) ?? 0M);

            _logger.LogInformation($"currentAmount = {currentAmount}");

            var newQuota = new QuotaDTO()
            {
                Key = quota.subscription,
                Value = (currentAmount - quota.Value) > 0M ? currentAmount - quota.Value : 0M
            };

            _logger.LogInformation($"newQuota Value = {newQuota.Value}");

            await PersisttoCache(newQuota);

            _logger.LogInformation ($"PersisttoCache completed");

            await PersisttoTable(new QuotaEntity()
            {
                PartitionKey = newQuota.Key,
                RowKey = Guid.NewGuid().ToString(),
                Operation = "Update",
                PromptTokens = quota.PromptTokens,
                TotalTokens = quota.TotalTokens,
                Model = quota.Model,
                Amount = newQuota.Value
            });

            _logger.LogInformation($"PersisttoTable completed");

            return true;
        }

        public async Task<decimal?> GetById(string key)
        {
            return Convert.ToDecimal(await _cache.GetStringAsync(key));
        }

        public async Task<IList<QuotaDTO>> GetAll()
        {
            var result = new List<QuotaDTO>();

            var keys = _client.Query<QuotaEntity>(x => x.PartitionKey != "")
                              .Where(w => w.ProductName != null) 
                              .Select(z => new { key = z.PartitionKey, product = z.ProductName })
                              .Distinct().ToList();

            await new TaskFactory().StartNew(() => { keys.ForEach(x => result.Add(new QuotaDTO() { Key = x.key, 
                                                                                                   Product = x.product ?? "",
                                                                                                   Value = GetById(x.key).Result ?? 0M })); });

            return result; 

        }

        #region Private Methods

        private async Task PersisttoCache(QuotaDTO quota)
        {
            await _cache.SetAsync(quota.Key, Encoding.UTF8.GetBytes(quota.Value.ToString()));
        }

        private async Task PersisttoTable(QuotaEntity entity)
        {
            await _client.AddEntityAsync(entity);
        }

        #endregion

    }
}