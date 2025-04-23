using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Models
{
	public class JwtSettings
	{
		public string Secret { get; }
		public string Issuer { get; set; }
		public string Audience { get; set; }
	}
}
