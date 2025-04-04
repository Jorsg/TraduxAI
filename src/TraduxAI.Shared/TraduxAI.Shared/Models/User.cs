using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TraduxAI.Shared.Enumerations;

namespace TraduxAI.Shared.Models
{
	public class User
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("email")]
		public string Email { get; set; }

		[BsonElement("passwordHash")]
		public string PasswordHash { get; set; }

		[BsonElement("role")]
		[BsonRepresentation(BsonType.String)]
		public UserRole Role { get; set; } = UserRole.User;

		[BsonElement("createAt")]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	}
}
