using System.Text.Json;
using TraduxAI.Client.Models;

namespace TraduxAI.Client.Services
{
	public interface IAuthService
	{
		Task<LoginResponse> LoginAsync(LoginRequest request);
		Task LogoutAsync();
		string? GetToken(); // Para consultar el token almacenado
	}
	public class AuthService : IAuthService
	{
			private readonly HttpClient _httpClient;
			private readonly JsonSerializerOptions _jsonOptions;
			private string? _token;

			public AuthService(HttpClient httpClient, JsonSerializerOptions jsonOptions)
			{
				_httpClient = httpClient;
				_jsonOptions = jsonOptions ?? new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			}
			public string? GetToken() => _token;


			public async Task<LoginResponse> LoginAsync(LoginRequest request)
			{
				var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
				if(response.IsSuccessStatusCode)
				{
					var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)
						               ?? throw new Exception("Error deserializing login response");
				}

				var error = await response.Content.ReadAsStringAsync();
				throw new Exception($"Error during login: {response.StatusCode} - {error}");
			}

			public Task LogoutAsync()
			{
				_token = null;
				return Task.CompletedTask;
			}
	}
}
