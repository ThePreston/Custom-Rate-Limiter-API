using Microsoft.OpenAIRateLimiter.Service.Models;

namespace Microsoft.OpenAIRateLimiter.Service
{
    public interface IParseService
    {
        Task<QuotaTransDTO> Parse(QuotaEntry entry);
    }
}