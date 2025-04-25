using System.ComponentModel.DataAnnotations;

namespace TraduxAI.Client.Models
{
	public class UserRequest
	{

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
		public string? Password { get; set; }

		[Required(ErrorMessage = "Confirm Password is required.")]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public string? ConfirmPassword { get; set; }
	}
}
