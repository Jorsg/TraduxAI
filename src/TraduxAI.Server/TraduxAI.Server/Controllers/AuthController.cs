using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraduxAI.Shared.DTOs;
using TraduxAI.Translation.Core.Interfaces;

namespace TraduxAI.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly IRefreshTokenServices _refreshTokenServices;
		private readonly IConfiguration _config;
		public AuthController(IAuthService authService, IConfiguration config, IRefreshTokenServices refreshTokenServices)
		{
			_authService = authService;
			_config = config;
			_refreshTokenServices = refreshTokenServices;
		}

		[HttpPost("register")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Register(RegisterUserDto dto)
		{
			var result = await _authService.RegisterUserAsync(dto);

			if (!result.Success)
			{
				return BadRequest(new { message = result.Message });
			}

			return Ok(new { message = result.Message });
		}

		[HttpPost("login")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Login(LoginUserDto dto)
		{
			AuthResponseDto response = new AuthResponseDto();

			var result = await _authService.LoginUserAsync(dto);

			if (!result.Success)
			{
				return Unauthorized(new { message = result.Message });
			}
			//Generate refresh token
			response.RefreshToken = result.Token.RefreshToken.Token;
			_refreshTokenServices.InsertRefreshTokenAsync(result.Token.RefreshToken, dto.Email);
			_refreshTokenServices.DisableUserTokenByEmailAsync(dto.Email);

			//return token here
			return Ok(new
			{
				success = true,
				message = result.Message,
				token = result.Token.AccessToken,
				refreshToken = response.RefreshToken,
			});
		}
		[HttpGet("me")]
		[Authorize]
		public IActionResult GetCurrentUser()
		{
			var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
			var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
			return Ok(new { id, email, role });
		}

		[HttpPost("refresh")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<AuthResponseDto> RefreshToken()
		{
			AuthResponseDto response = new AuthResponseDto();
			var resfreshToken = Request.Cookies["refreshToken"];
			if (string.IsNullOrEmpty(resfreshToken))
				return BadRequest(new { message = "Refresh token is missing" });
			var isValid = _refreshTokenServices.IsRefreshTokenValid(resfreshToken).Result;
			if (!isValid)
				return BadRequest(new { message = "Refresh token is invalid" });
			var currentUser = _refreshTokenServices.FindUserByToken(resfreshToken).Result;
			if (currentUser == null)
				return BadRequest(new { message = "User not found" });
			//Generate new token
			var token = _authService.LoginUserAsync(new LoginUserDto { Email = currentUser.Email, Password = currentUser.PasswordHash }).Result;
			response.AccessToken = token.Token.AccessToken;
			response.RefreshToken = token.Token.RefreshToken.Token;

			//Set new refresh token in cookie
			_refreshTokenServices.DisableUserToken(resfreshToken);
			_refreshTokenServices.InsertRefreshTokenAsync(token.Token.RefreshToken, currentUser.Email);
			return Ok(response);
		}

		[HttpPost("logout")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult Logout()
		{
			var resfreshToken = Request.Cookies["refreshToken"];
			if (string.IsNullOrEmpty(resfreshToken))
				return BadRequest(new { message = "Refresh token is missing" });
			_refreshTokenServices.DisableUserToken(resfreshToken);
			return Ok(new { message = "Logout successful" });
		}
	}
}
