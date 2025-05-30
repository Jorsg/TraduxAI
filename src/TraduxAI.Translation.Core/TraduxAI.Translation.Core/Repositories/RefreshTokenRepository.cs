using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Data;
using TraduxAI.Shared.Models;

namespace TraduxAI.Translation.Core.Repositories
{
	public class RefreshTokenRepository
	{
		private readonly MongoDbContext _context;
		public RefreshTokenRepository(MongoDbContext context)
		{
			_context = context;
		}
		//DisableUserTokenByEmailAsync
		public async Task DisableUserTokenByEmailAsync(string email)
		{
			var filter = Builders<RefreshToken>.Filter.Eq(x => x.Email, email);
			var update = Builders<RefreshToken>.Update.Set(x => x.IsActive, false);
			await _context.RefreshToken.UpdateManyAsync(filter, update);
		}
		//insertRefreshTokenAsync(RefreshToken, email)
		public async Task InsertRefreshTokenAsync(RefreshToken refreshToken, string email)
		{
			var filter = Builders<RefreshToken>.Filter.Eq(x => x.Email, email);
			var update = Builders<RefreshToken>.Update.Set(x => x.Token, refreshToken.Token)
				.Set(x => x.CreateAt, refreshToken.CreateAt)
				.Set(x => x.ExpiresAt, refreshToken.ExpiresAt)
				.Set(x => x.IsActive, refreshToken.IsActive)
				.Set(x => x.Email, email);
			await _context.RefreshToken.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });			
		}

		//DisableUserToken(string token)
		public async Task DisableUserToken(string token)
		{
			var filter = Builders<RefreshToken>.Filter.Eq(x => x.Token, token);
			var update = Builders<RefreshToken>.Update.Set(x => x.IsActive, false);
			await _context.RefreshToken.UpdateOneAsync(filter, update);
		}
		//IsRefreshTokenValid(string token)
		public async Task<bool> IsRefreshTokenValid(string token)
		{
			var filter = Builders<RefreshToken>.Filter.Eq(x => x.Token, token);
			var refreshToken = await _context.RefreshToken.Find(filter).FirstOrDefaultAsync();
			if (refreshToken == null || refreshToken.IsExpired)
			{
				return false;
			}
			return true;
		}

		//FindUserByToken(string token)
		//crea un join en mongodb para las entidades RefreshToken y User,
		//donde el token de RefreshToken es igual al token que se pasa como parametro y el email de User es igual al email de RefreshToken
		public async Task<User?> FindUserByToken(string token)
		{
			var filter = Builders<RefreshToken>.Filter.Eq(x => x.Token, token);
			var refreshToken = await _context.RefreshToken.Find(filter).FirstOrDefaultAsync();
			if (refreshToken == null || refreshToken.IsExpired)
			{
				return null;
			}
			var userFilter = Builders<User>.Filter.Eq(x => x.Email, refreshToken.Email);
			var user = await _context.Users.Find(userFilter).FirstOrDefaultAsync();
			return user;
		}
	}
}
