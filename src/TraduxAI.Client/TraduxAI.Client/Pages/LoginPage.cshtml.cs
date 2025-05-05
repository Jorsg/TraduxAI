using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TraduxAI.Client.Services;
using TraduxAI.Client.Models;

public class LoginPageModel : PageModel
{
	private readonly IAuthService _authService;
	public LoginRequest LoginModelRequest { get; set; } = new LoginRequest(); // Initialize LoginModel

	public LoginPageModel(IAuthService authService)
	{
		_authService = authService;
	}

	public string? ErrorMessage { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		
		if (LoginModelRequest.Email != "" && LoginModelRequest.Password != "")
		{
			var response = await _authService.LoginAsync(LoginModelRequest); // Use _authService instance
			var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, LoginModelRequest.Email),
					new Claim(ClaimTypes.Role, "User")
				};

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

			return Redirect("/");
		}

		ErrorMessage = "Usuario o contraseña incorrecta";
		return Page();
	}
}
