using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TraduxAI.Client.Services;

namespace TraduxAI.Client.Providers
{
	public class JwtAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly IAuthService _authService;
		private string? _token;
		private ClaimsPrincipal? _cachedUser;

		public JwtAuthenticationStateProvider(IAuthService authService)
		{
			_authService = authService;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			if(_cachedUser is not null)
			{
				return new AuthenticationState(_cachedUser);
			}

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
			
			return new AuthenticationState(new ClaimsPrincipal(identity));
		}

		public async Task NotifyUserAuthentication(string token)
		{
			_token = token;
			await _authService.SetTokenAsync(token);
			var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
			var user = new ClaimsPrincipal(identity);
			// Notifica al sistema que el estado de autenticación ha cambiado.
			_cachedUser = user;

			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		// Método auxiliar para extraer claims desde el JWT
		private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		{
			var handler = new JwtSecurityTokenHandler();
			var token = handler.ReadJwtToken(jwt);
			return token.Claims;
		}

		public async Task NotifyUserLogout()
		{
			_token = null;
			await _authService.LogoutAsync();			
			_cachedUser = null;
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
		}

		public async Task ForceAuthStateRefresh()
		{
			var state = await GetAuthenticationStateAsync();
			NotifyAuthenticationStateChanged(Task.FromResult(state));
		}

	}
}
