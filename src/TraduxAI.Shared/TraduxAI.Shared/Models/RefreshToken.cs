using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Models
{
	public class RefreshToken
	{
		
		[BsonElement("token")]
		public string Token { get; set; }
		[BsonElement("creatAt")]
		public DateTime CreateAt { get; set; }
		[BsonElement("expiresAt")]
		public DateTime ExpiresAt { get; set; }
		public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
		[BsonElement("isActive")]
		public bool IsActive => !IsExpired;
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string UserId { get; set; } = null!;

		[BsonElement("email")]
		public string Email { get; set; }
	}
}
