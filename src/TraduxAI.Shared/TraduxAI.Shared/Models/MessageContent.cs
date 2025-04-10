using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Models
{
	public class MessageContent
	{
		public string Type { get; set; } = "text";
		public string? Text { get; set; }
		public ImageUrl? Image_url { get; set; }
		public File_id? File_id { get; set; }
	}

	public class ImageUrl
	{
		public string Url { get; set; } = string.Empty;		
	}

	public class File_id
	{
		public string file_id { get; set; } = string.Empty;
	}
}
