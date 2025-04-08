using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using TraduxAI.Client.Services;

namespace TraduxAI.Client.Providers
{
	public class JwtAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly IAuthService _authService;

		public JwtAuthenticationStateProvider(IAuthService authService)
		{
			_authService = authService;
		}

		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = _authService.GetToken();
			ClaimsIdentity identity = new ClaimsIdentity();
			if(!string.IsNullOrEmpty(token))
			{
				var jwtHandler = new JwtSecurityTokenHandler();
				var jwtToken = jwtHandler.ReadJwtToken(token);
				var claims = jwtToken.Claims.ToList();
				identity = new ClaimsIdentity(claims, "jwt");
			}
			else
			{
				identity = new ClaimsIdentity();
			}
			var user = new ClaimsPrincipal(identity);
			return Task.FromResult(new AuthenticationState(user));
		}

		public void NotifyUserAuthentication(string token)
		{
			var identity = !string.IsNullOrEmpty(token)
				? new ClaimsIdentity(new JwtSecurityTokenHandler().ReadJwtToken(token).Claims, "jwt")
				: new ClaimsIdentity();

			var user = new ClaimsPrincipal(identity);
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}
	}
}
