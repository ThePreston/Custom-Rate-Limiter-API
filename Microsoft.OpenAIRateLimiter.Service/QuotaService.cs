
using Azure.Data.Tables;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OpenAIRateLimiter.Service.Models;
using System.Text;

namespace Microsoft.OpenAIRateLimiter.Service
{
    public class QuotaService : IQuotaService
    {

        private readonly IDistributedCache _cache;

        private readonly TableClient _client;

        public QuotaService(IDistributedCache cache, TableClient client)
        {
            _cache = cache;
            _client = client;
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

                var currentAmount = await GetById(quota.Key) ?? 0;

                var newQuota = new QuotaDTO()
                {
                    Key = quota.Key,
                    Value = (currentAmount - quota.Value) > 0 ? currentAmount - quota.Value : 0
                };

                await PersisttoCache(newQuota);

                await PersisttoTable(new QuotaEntity()
                {
                    PartitionKey = newQuota.Key,
                    RowKey = Guid.NewGuid().ToString(),
                    Operation = "Update",
                    Model = quota.Model,
                    Amount = newQuota.Value
                });

            return true;
        }

        public async Task<int?> GetById(string key)
        {
            return Convert.ToInt32(await _cache.GetStringAsync(key));
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
                                                                                                   Value = GetById(x.key).Result ?? 0 })); });

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