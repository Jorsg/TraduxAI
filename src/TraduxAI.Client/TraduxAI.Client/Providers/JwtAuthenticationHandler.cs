using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace TraduxAI.Client.Providers
{
    public class JwtAuthenticationHandler : AuthenticationHandler<CustomerOption>
    {
        public JwtAuthenticationHandler(IOptionsMonitor<CustomerOption> options,
            ILoggerFactory logger,
            UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var token = Request.Cookies["access_token"];
                if (string.IsNullOrEmpty(token))
                    return AuthenticateResult.NoResult();

                var jwtHandler = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var identity = new ClaimsIdentity(jwtHandler.Claims, "jwt");
                var principal = new ClaimsPrincipal(identity);

               var tickets = new AuthenticationTicket(principal, Scheme.Name);
                var result = AuthenticateResult.Success(tickets);
                return await Task.FromResult(result);


            }
            catch (Exception)
            {

                return AuthenticateResult.NoResult();
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Redirect("/login");
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.Redirect("/accessdenied");
            return Task.CompletedTask;
        }

    }

    public class CustomerOption : AuthenticationSchemeOptions
    {
        public string? Token { get; set; }
    }



}
