using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Data;
using TraduxAI.Shared.Models;
using TraduxAI.Translation.Core.Interfaces;

namespace TraduxAI.Translation.Core.Repositories
{
	public class UserRepository : IUserRepository		
	{
		private readonly IMongoCollection<User> _users;

		public UserRepository(MongoDbContext context)
		{
			_users = context.Users;

		}

		public async Task CreateUserAsync(User user)
		{
			await _users.InsertOneAsync(user);
		}

		public async Task<User?> GetUserByEmailAsync(string email)
		{
			return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
		}
	}
}
