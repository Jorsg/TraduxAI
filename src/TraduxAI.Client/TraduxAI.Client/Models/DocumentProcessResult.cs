namespace TraduxAI.Client.Models
{
	public class DocumentProcessResult
	{
		public bool Success { get; set; }
		public string ProcessedContent { get; set; } = string.Empty;
		public string ErrorMessage { get; set; } = string.Empty;
		public string OperationType { get; set; } = string.Empty;
		public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
	}
}
