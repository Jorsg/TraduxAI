using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.DTOs
{
	public class AuthResponseDto
	{
		public string  AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}
