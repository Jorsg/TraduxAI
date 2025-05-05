using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TraduxAI.Shared.Models;

namespace TraduxAI.Translation.Core.Services
{
	public class JwtService
	{
		//private readonly JwtSettings _jwtSettings;
		private readonly IConfiguration _configuration;

		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public TokenAccess GenerateToken(User user)
		{
			var accessToken = GenerateAccessToken(user);
			var refreshToken = GetRefreshToken();			
			return new TokenAccess
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken
			};
		}

		public string GenerateAccessToken(User user)
		{
			var claim = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(ClaimTypes.Role, user.Role.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["JwtSettings:Issuer"],
				audience: _configuration["JwtSettings:Audience"],
				claims: claim,
				expires: DateTime.Now.AddHours(1),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);

		}

		private RefreshToken GetRefreshToken()
		{
			var refereshToken = new RefreshToken
			{
				Token = Guid.NewGuid().ToString(),
				CreateAt = DateTime.UtcNow,
				ExpiresAt = DateTime.UtcNow.AddMonths(1),
			};
			return refereshToken;
		}
	}

	public class TokenAccess
	{
		public string AccessToken { get; set; }
		public RefreshToken RefreshToken { get; set; }

	}
}