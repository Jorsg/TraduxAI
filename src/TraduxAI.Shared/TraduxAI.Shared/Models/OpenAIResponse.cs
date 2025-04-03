using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Models
{
	public class OpenAIResponse
	{
		public string Id { get; set; } = string.Empty;
		public string Object { get; set; } = string.Empty;
		public long Created { get; set; }
		public string Model { get; set; } = string.Empty;
		public List<Choice> Choices { get; set; }
		public Usage Usage { get; set; } = new Usage();
	}

	public class Choice
	{
		public int Index { get; set; }
		public MessageResponse Message { get; set; }
		public string FinishReason { get; set; } = string.Empty;
	}

	public class Usage
	{
		public int PromptTokens { get; set; }
		public int CompletionTokens { get; set; }
		public int TotalTokens { get; set; }
	}

	public class MessageResponse
	{
		public string Role { get; set; }
		public string Content { get; set; } 
	}
}
