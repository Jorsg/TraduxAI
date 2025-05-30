namespace TraduxAI.Client.Interfaces
{
    public interface ITokenService
    {
        Task<bool> RefreshTokenAsync();
        Task LogoutAsync();
    }
}
