using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Models
{
	public class RegisterRequest
	{
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
	}
}
