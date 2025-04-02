using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Models;

namespace TraduxAI.Translation.Core.Interfaces
{
	public interface IDocumentProcessor
	{
		Task<DocumentProcessResult> ProcessDocumentAsync(DocumentProcessRequest request);
		Task<DocumentProcessResult> ExtractTextFromImageAsync(string base64Image);
		Task<DocumentProcessResult> ExtractTextFromPdfAsync(string base64Pdf);
		Task<DocumentProcessResult> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage);
	}
}
