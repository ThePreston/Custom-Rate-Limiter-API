namespace Microsoft.OpenAIRateLimiter.Service
{
    public interface ITokenService
    {
        decimal CalculateCost(int completionTokens, int promptTokens, string model);
        Task<int> GetTokenCount(string prompt, string model);
    }
}