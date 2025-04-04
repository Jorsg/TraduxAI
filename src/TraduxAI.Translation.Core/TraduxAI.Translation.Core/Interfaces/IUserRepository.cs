using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Models;

namespace TraduxAI.Translation.Core.Interfaces
{
	public interface IUserRepository
	{
		Task CreateUserAsync(User user);
		Task<User?> GetUserByEmailAsync(string email);
	}
}
