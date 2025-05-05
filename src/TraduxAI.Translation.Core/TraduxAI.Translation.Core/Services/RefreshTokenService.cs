using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Models;
using TraduxAI.Translation.Core.Interfaces;
using TraduxAI.Translation.Core.Repositories;

namespace TraduxAI.Translation.Core.Services
{
	public class RefreshTokenService : IRefreshTokenServices
	{
		private readonly RefreshTokenRepository _refreshTokenRepository;

		public RefreshTokenService(RefreshTokenRepository refreshTokenRepository)
		{
			_refreshTokenRepository = refreshTokenRepository;
		}
		public async Task DisableUserToken(string token)
		{
			await _refreshTokenRepository.DisableUserToken(token);
		}

		public async Task DisableUserTokenByEmailAsync(string email)
		{
			await _refreshTokenRepository.DisableUserTokenByEmailAsync(email);
		}

		public async Task<User?> FindUserByToken(string toke)
		{
			  return await _refreshTokenRepository.FindUserByToken(toke);
		}

		public async Task InsertRefreshTokenAsync(RefreshToken refreshToken, string email)
		{
			await _refreshTokenRepository.InsertRefreshTokenAsync(refreshToken, email);
		}

		public async Task<bool> IsRefreshTokenValid(string token)
		{
			return await _refreshTokenRepository.IsRefreshTokenValid(token);
		}
	}
}
