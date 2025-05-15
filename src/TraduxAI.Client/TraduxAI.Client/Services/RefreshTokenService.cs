using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace TraduxAI.Client.Services
{
    public class RefreshTokenService
    {
        private readonly ProtectedLocalStorage _protectLocalStorage;
        private readonly string key = "refreshToken";
        public RefreshTokenService(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectLocalStorage = protectedLocalStorage;
        }

        public async Task Set(string value)
        {
            await _protectLocalStorage.SetAsync(key, value);
        }

        public async Task<string> Get()
        {
            var result = await _protectLocalStorage.GetAsync<string>(key);
            if (result.Success) return result.Value;
            else           
                return string.Empty;
        }

        internal async Task Remove()
        {
            await _protectLocalStorage.RemoveAsync(key);
        }
    }
}
