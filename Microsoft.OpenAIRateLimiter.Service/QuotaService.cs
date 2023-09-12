using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.OpenAIRateLimiter.Service.Models;
using System.Diagnostics.CodeAnalysis;
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
                                                     Operation = await Exists(quota.Key) ? "Deposit" : "Create",
                                                     Amount = Convert.ToDouble(quota.Value) });

            return true; 
            
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
                                                     Amount = Convert.ToDouble(quota.Value) });

            return true;

        }

        public async Task<bool> Update(QuotaTransDTO quota)
        {
            _logger.LogInformation($"Update Value = {quota.Value}; Update Total Tokens = {quota.TotalTokens}");
            var currentAmount = Convert.ToDecimal(await GetById(quota.subscription) ?? 0M);

            var newQuota = new QuotaDTO()
            {
                Key = quota.subscription,
                Value = (currentAmount - quota.Value) > 0M ? currentAmount - quota.Value : 0M
            };

            await PersisttoCache(newQuota);

            await PersisttoTable(new QuotaEntity()
            {
                PartitionKey = newQuota.Key,
                RowKey = Guid.NewGuid().ToString(),
                Operation = "Update",
                PromptTokens = quota.PromptTokens,
                TotalTokens = quota.TotalTokens,
                Model = quota.Model,
                TransCost = quota.Value.ToString(),
                Balance = newQuota.Value.ToString()
            });

            return true;
        }

        public async Task<decimal?> GetById(string key)
        {
            return Convert.ToDecimal(await _cache.GetStringAsync(key));
        }

        public async Task<IList<QuotaEntity>> GetAll()
        {
            var result = new List<QuotaEntity>();

            var keys = _client.Query<QuotaEntity>(x => x.PartitionKey != "" && x.Operation == "Create").ToList();

            await new TaskFactory().StartNew(() =>  {
                keys.ForEach( x =>
                {
                    x.Balance = GetById(x.PartitionKey).Result.ToString() ?? "0";
                    x.TotalTokens = CalculateTokenUsage(x.PartitionKey);
                }); });

            return keys; 

        }

        public async Task<IList<QuotaEntity>> GetHistoryById(string key)
        {
            return await GetallRecords(key);
        }

        #region Private Methods

        private int CalculateTokenUsage(string subscription)
        {
            var amount = _client.Query<QuotaEntity>(x => x.PartitionKey == subscription).Sum(s => s.TotalTokens);

            return amount;

        }

        private async Task PersisttoCache(QuotaDTO quota)
        {
            await _cache.SetAsync(quota.Key, Encoding.UTF8.GetBytes(quota.Value.ToString()));
        }

        private async Task PersisttoTable(QuotaEntity entity)
        {
            await _client.AddEntityAsync(entity);
        }

        private async Task<List<QuotaEntity>> GetallRecords(string subscriptionKey)
        {

            return  await new TaskFactory().StartNew(() => { return _client.Query<QuotaEntity>(x => x.PartitionKey == subscriptionKey).ToList(); });

        }

        private async Task<bool> Exists(string subscriptionKey)
        {

            var Retval = false;

            await foreach (var q in _client.QueryAsync<QuotaEntity>(x => x.PartitionKey == subscriptionKey, 1))
            {
                Retval = true;
                break;
            }

            return Retval;

        }


        #endregion

    }
}