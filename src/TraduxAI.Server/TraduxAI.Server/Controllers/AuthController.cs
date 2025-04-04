using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraduxAI.Shared.DTOs;
using TraduxAI.Shared.Models;
using TraduxAI.Translation.Core.Interfaces;

namespace TraduxAI.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService)
		{
			_authService = authService;
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
			var result = await _authService.LoginUserAsync(dto);

			if (!result.Success)
			{
				return Unauthorized(new { message = result.Message });
			}

			// Later: return token here
			return Ok(new
			{
				message = result.Message,
				token = result.Token
			});
		}
		[HttpGet("me")]
		[Authorize]
		public IActionResult GetCurrentUser()
		{
			var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
			var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
			return Ok(new {id, email, role });
		}

	}
}
