using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;
using TraduxAI.Client.Models;

namespace TraduxAI.Client.Services
{
	public interface IAuthService
	{
		Task<LoginResponse> LoginAsync(LoginRequest request);
		Task LogoutAsync();
		Task<string?> GetTokenAsync(); // Para consultar el token almacenado
		Task SetTokenAsync(string token); // Para establecer el token

	}
	public class AuthService : IAuthService
	{
		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _jsonOptions;
		private readonly IJSRuntime _jsRuntime;
		private string? _token;

		public AuthService(HttpClient httpClient, JsonSerializerOptions jsonOptions, IJSRuntime jSRuntime)
		{
			_httpClient = httpClient;
			_jsonOptions = jsonOptions ?? new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			_jsRuntime = jSRuntime ?? throw new ArgumentNullException(nameof(jSRuntime));
		}
		public async Task<string?> GetTokenAsync()
		{
			//// Check if the app is prerendering
			//if (_jsRuntime.GetType().FullName == "Microsoft.AspNetCore.Components.Server.Circuits.RemoteJSRuntime")
			//{
			//	// Skip JavaScript interop during prerendering
			//	return null;
			//}
			//return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt_token");
			try
			{
				return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt_token");
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine($"[AuthService] JSInterop aún no está disponible (prerender): {ex.Message}");
				return null;
			}
		}

		public async Task<LoginResponse> LoginAsync(LoginRequest request)
		{
			var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
			if (response.IsSuccessStatusCode)
			{
				var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)
								   ?? throw new Exception("Error deserializing login response");
				return loginResponse;
			}
			else
			{
				var error = await response.Content.ReadAsStringAsync();
				throw new Exception($"Error during login: {response.StatusCode} - {error}");
			}

		}

		public async Task LogoutAsync()
		{
			_token = null;
			await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "jwt_token");
		}

		public async Task SetTokenAsync(string token)
		{
			_token = token;
			await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwt_token", token);
		}
	}
}
