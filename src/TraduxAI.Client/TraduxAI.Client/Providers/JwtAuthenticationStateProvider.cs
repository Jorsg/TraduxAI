using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TraduxAI.Client.Services;

namespace TraduxAI.Client.Providers
{
	public class JwtAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly IAuthService _authService;
		private string? _token;

		public JwtAuthenticationStateProvider(IAuthService authService)
		{
			_authService = authService;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await _authService.GetTokenAsync();
			ClaimsIdentity identity = new ClaimsIdentity();
			if (!string.IsNullOrEmpty(token))
			{
				var jwtHandler = new JwtSecurityTokenHandler();
				try
				{
					var jwtToken = jwtHandler.ReadJwtToken(token);
					if(jwtToken.ValidTo < DateTime.UtcNow)
					{
						await _authService.LogoutAsync();
						return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
					}
					var claims = jwtToken.Claims.ToList();
					identity = new ClaimsIdentity(claims, "jwt");
				}
				catch (Exception)
				{

					await _authService.LogoutAsync(); // Previene loops
					return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
				}
			}			
			var user = new ClaimsPrincipal(identity);
			return new AuthenticationState(user);
		}

		public async Task NotifyUserAuthentication(string token)
		{
			_token = token;
			await _authService.SetTokenAsync(token);
			var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
			var user = new ClaimsPrincipal(identity);
			// Notifica al sistema que el estado de autenticación ha cambiado.

			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		// Método auxiliar para extraer claims desde el JWT
		private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		{
			var handler = new JwtSecurityTokenHandler();
			var token = handler.ReadJwtToken(jwt);
			return token.Claims;
		}
	}
}
