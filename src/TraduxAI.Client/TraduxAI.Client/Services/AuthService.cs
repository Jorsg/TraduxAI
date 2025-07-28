using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;
using TraduxAI.Client.Interfaces;
using TraduxAI.Client.Models;

namespace TraduxAI.Client.Services
{
	public interface IAuthService
	{
		Task<AuthResponse> LoginAsync(LoginRequest request);
		Task LogoutAsync();		
		Task<UserResponse> CreateUserAsync(UserRequest request);

	}
	public class AuthService : IAuthService, ITokenService
    {
		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _jsonOptions;	
		NavigationManager nav;
		private readonly AccesTokenService _accesTokenService;
		private readonly RefreshTokenService _refreshTokenService;

        public AuthService(
			IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonOptions, 
			IJSRuntime jSRuntime, AccesTokenService accesTokenService,
			NavigationManager navigationManager, RefreshTokenService refreshTokenService)
        {
			_httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = jsonOptions ?? new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            _accesTokenService = accesTokenService ?? throw new ArgumentNullException(nameof(accesTokenService));
            nav = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
            _refreshTokenService = refreshTokenService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
		{
			var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
			if (response.IsSuccessStatusCode)
			{
				var token = await response.Content.ReadAsStringAsync();
                var loginResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions)
								   ?? throw new Exception("Error deserializing login response");
				await _accesTokenService.DeleteAccessToken();
				await _accesTokenService.SetAccessToken(loginResponse.Token);
				await _refreshTokenService.Set(loginResponse.RefreshToken);
                return loginResponse;
			}
			else
			{
				var error = await response.Content.ReadAsStringAsync();
				throw new Exception($"Error during login: {response.StatusCode} - {error}");
			}
		}

		public async Task<bool> RefreshTokenAsync()
		{
			var refereshToken = await _refreshTokenService.Get();
			_httpClient.DefaultRequestHeaders.Add("Cookie", $"refreshtoken={refereshToken}");
            var response = await _httpClient.PostAsync("api/auth/refresh", null);
			if (response.IsSuccessStatusCode)
			{
                var token = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(token))
				{
                    var loginResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions)
                                   ?? throw new Exception("Error deserializing login response");
                    await _accesTokenService.SetAccessToken(loginResponse.Token);
                    await _refreshTokenService.Set(loginResponse.RefreshToken);
                    return true;
                }                   
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error during refresh token: {response.StatusCode} - {error}");
            }
                return false;
		}

		public async Task LogoutAsync()
		{
			var refereshToken = await _refreshTokenService.Get();
			_httpClient.DefaultRequestHeaders.Add("Cookie", $"refreshtoken={refereshToken}");
			var response = await _httpClient.PostAsync("api/auth/logout", null);
            if (response.IsSuccessStatusCode)
            {
				await _refreshTokenService.Remove();
                await _accesTokenService.DeleteAccessToken();          
                nav.NavigateTo("/login", forceLoad: true);

            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error during logout: {response.StatusCode} - {error}");
            }
        }
		
		public async Task<UserResponse> CreateUserAsync(UserRequest request)
		{
			UserResponse userResponse = new();
			var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
			if (response.IsSuccessStatusCode)
			{
				var registerResponse = await response.Content.ReadFromJsonAsync<UserResponse>(_jsonOptions)
								   ?? throw new Exception("Error deserializing login response");
				userResponse.Success = true;
				return userResponse;
			}
			else
			{
				var error = await response.Content.ReadAsStringAsync();
				throw new Exception($"Error during register: {response.StatusCode} - {userResponse.ErrorMessage = error}");
			}
		}
	}
}
