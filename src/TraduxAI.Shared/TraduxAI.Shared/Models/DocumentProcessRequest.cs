using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Models
{
	public class DocumentProcessRequest
	{
		public string DocumentType { get; set; } = string.Empty; // "image", "pdf", "text"
		public string Content { get; set; } = string.Empty; // Base64 string for images/PDFs, raw text for text
		public string Operation { get; set; } = string.Empty; // "ocr", "translate", "summarize"
		public string TargetLanguage { get; set; } = "en";
		public string SourceLanguage { get; set; } = "auto";
		public string Promtp { get; set; }
	}
}
