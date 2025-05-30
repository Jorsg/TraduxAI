namespace TraduxAI.Client.Services
{
    public class AccesTokenService
    {
        private readonly CookieService _cookieService;
        private readonly string _tokenKey = "access_token";

        public AccesTokenService(CookieService cookieService)
        {
            _cookieService = cookieService;
        }

        public async Task SetAccessToken(string accessToken)
        {
            await _cookieService.SetCookie(_tokenKey, accessToken, 1);
        }
        public async Task<string> GetAccessToken()
        {
            return await _cookieService.GetCookie(_tokenKey);
        }
        public async Task DeleteAccessToken()
        {
            await _cookieService.DeleteCookie(_tokenKey);
        }
    }
}
