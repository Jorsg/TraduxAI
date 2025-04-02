using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Models
{
	public class ConvertToJson
	{
		public string model { get; set; }
		public List<MessageRequest> messages { get; set; }
	}
}
