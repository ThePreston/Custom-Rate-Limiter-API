using Microsoft.OpenAIRateLimiter.Service.Models;

namespace Microsoft.OpenAIRateLimiter.Service
{
    public interface IQuotaService
    {
        Task<bool> Create(QuotaDTO quota);

        Task<bool> BudgetUpdate(QuotaDTO quota);

        Task<int?> GetById(string key);

        Task<bool> Update(QuotaTransDTO quota);

        Task<IList<QuotaDTO>> GetAll();
    }
}