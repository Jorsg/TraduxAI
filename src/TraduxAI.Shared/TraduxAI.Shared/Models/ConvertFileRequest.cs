using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Models
{
	public class ConvertFileRequest
	{
		public string FilePath { get; set; }
		public string SourceLang { get; set; }
		public string TargetLang { get; set; }
	}
}
