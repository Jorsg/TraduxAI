using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Models;

namespace TraduxAI.Translation.Core.Interfaces
{
	public interface IRefreshTokenServices
	{
		Task DisableUserTokenByEmailAsync(string email);
		Task InsertRefreshTokenAsync(RefreshToken refreshToken, string email);
		Task DisableUserToken(string token);
		Task<bool> IsRefreshTokenValid(string token);
		Task<User?> FindUserByToken(string toke);
	}
}
