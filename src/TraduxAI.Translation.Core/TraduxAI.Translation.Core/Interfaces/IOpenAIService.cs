using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Translation.Core.Interfaces
{
	public interface IOpenAIService
	{
		Task<string> GetCompletionAsync(string prompt);
		Task<string> ImageToTextAsync(string base64Image, string promtp);
		Task<string> PdfToTextAsync(string base64Pdf);
		Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage);
	}
}
