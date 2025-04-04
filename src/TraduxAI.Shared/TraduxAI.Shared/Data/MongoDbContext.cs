using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace TraduxAI.Shared.Data
{
	public class MongoDbContext
	{
		private readonly IMongoDatabase _database;

		public MongoDbContext(IOptions<MongoDbSettings> settings)
		{
			var client = new MongoClient(settings.Value.ConnectionString);
			_database = client.GetDatabase(settings.Value.DatabaseName);
		}

		public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
	}
}
