using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Models
{

	public class OpenAIRequest
	{
		public string Model { get; set; } = "gpt-4o";
		public List<Message> Messages { get; set; }
		public double Temperature { get; set; } = 0.7;
		//public int MaxTokens { get; set; } = 256;
	}



	public class Message
	{
		public string Role { get; set; } = "user";
		public List<MessageContent>? Content { get; set; }
		public string? ContentText { get; set; }
	}
}
