using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace TraduxAI.Client.Services
{
    public class APIService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly NavigationManager _navigationManager;
        private readonly AccesTokenService _accesTokenService;
        public APIService(IHttpClientFactory httpClientFactory, AccesTokenService accesTokenService,
            AuthService authService, NavigationManager navigationManager)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _authService = authService;
            _navigationManager = navigationManager;
            _accesTokenService = accesTokenService;
        }

        public async Task<HttpResponseMessage> GetAsync(string urlEndpoint)
        {
            var token = await _accesTokenService.GetAccessToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.GetAsync(urlEndpoint);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Handle unauthorized access, e.g., redirect to login page
                var refreshToken = await _authService.RefreshTokenAsync();
                if (!refreshToken)
                    await _authService.LogoutAsync();

                var newToken = await _accesTokenService.GetAccessToken();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

                var newResponse = await _httpClient.GetAsync(urlEndpoint);
                if (newResponse.IsSuccessStatusCode)
                    return newResponse;
                else
                    // Handle failure to refresh token
                    _navigationManager.NavigateTo("/login");
            }
            return response;
        }

        public async Task<HttpResponseMessage> PostDataAsync(string endpoint, object obj)
        {
            var token = await _accesTokenService.GetAccessToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.PostAsJsonAsync(endpoint, obj);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Handle unauthorized access, e.g., redirect to login page
                var refreshToken = await _authService.RefreshTokenAsync();
                if (!refreshToken)
                    await _authService.LogoutAsync();

                var newToken = await _accesTokenService.GetAccessToken();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                var newResponse = await _httpClient.PostAsJsonAsync(endpoint, obj);
                if (newResponse.IsSuccessStatusCode)
                    return newResponse;
                else
                    // Handle failure to refresh token
                    _navigationManager.NavigateTo("/login");
            }
            return response;
        }
    }
}
