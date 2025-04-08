namespace TraduxAI.Client.Models
{
	public class LoginRequest
	{
		public string Email { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
