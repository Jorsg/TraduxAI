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
		private readonly AccesTokenService _accesTokenService;

        public JwtAuthenticationStateProvider(IAuthService authService, AccesTokenService accesTokenService)
		{
			_authService = authService;
            _accesTokenService = accesTokenService;
        }

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{

			try
			{
				var token = await _accesTokenService.GetAccessToken();
                if (string.IsNullOrEmpty(token))
                {
                    // No hay token, el usuario no está autenticado
                   return await MarkAsUnauthorize();
                }

				var readJwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
				var identity = new ClaimsIdentity(readJwtToken.Claims, "jwt");
				var principal = new ClaimsPrincipal(identity);

				return await Task.FromResult(new AuthenticationState(principal));
            }
			catch (Exception)
			{
				return await MarkAsUnauthorize();
            }
		}

		public async Task<AuthenticationState> MarkAsUnauthorize()
        {
			try
			{
				var state = new  AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }
			catch (Exception)
			{

				return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

    }
}
