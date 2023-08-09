namespace Microsoft.OpenAIRateLimiter.Service
{
    public interface ITokenService
    {
        double CalculateCost(int TotalTokens, string model);
        Task<int> GetTokenCount(string prompt, string model);
    }
}