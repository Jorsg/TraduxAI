using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Errors;
using TraduxAI.Shared.Models;
using TraduxAI.Translation.Core.Interfaces;

namespace TraduxAI.Translation.Core.Services
{
	public class DocumentProcessorService : IDocumentProcessor
	{
		private readonly IOpenAIService _openAIService;

		public DocumentProcessorService(IOpenAIService openAIService)
		{
			_openAIService = openAIService;
		}

		public async Task<DocumentProcessResult> ProcessDocumentAsync(DocumentProcessRequest request)
		{
			try
			{
				DocumentProcessResult result = new();

				switch (request.DocumentType.ToLower())
				{
					case "image" when request.Operation.ToLower() == "ocr":
						result = await ExtractTextFromImageAsync(request.Content, request.Promtp);
						break;
					case "pdf" when request.Operation.ToLower() == "ocr":
						result = await ExtractTextFromPdfAsync(request.Content);
						break;
					case "text" when request.Operation.ToLower() == "translate":
						result = await TranslateTextAsync(request.Content, request.SourceLanguage, request.TargetLanguage);
						break;
					default:
						throw new ApiException("Unsupported document type or operation", "unsupported_operation", 400);
				}

				return result;
			}
			catch (ApiException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ApiException($"Error processing document: {ex.Message}", ex, "document_processing_error");
			}
		}

		public async Task<DocumentProcessResult> ExtractTextFromImageAsync(string base64Image, string promtp)
		{
			var extractedText = await _openAIService.ImageToTextAsync(base64Image, promtp);

			return new DocumentProcessResult
			{
				Success = !string.IsNullOrEmpty(extractedText),
				ProcessedContent = extractedText,
				OperationType = "ocr_image",
				ProcessedAt = DateTime.UtcNow
			};
		}

		public async Task<DocumentProcessResult> ExtractTextFromPdfAsync(string base64Pdf)
		{
			var extractedText = await _openAIService.PdfToTextAsync(base64Pdf);

			return new DocumentProcessResult
			{
				Success = !string.IsNullOrEmpty(extractedText),
				ProcessedContent = extractedText,
				OperationType = "ocr_pdf",
				ProcessedAt = DateTime.UtcNow
			};
		}

		public async Task<DocumentProcessResult> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage)
		{
			var translatedText = await _openAIService.TranslateTextAsync(text, sourceLanguage, targetLanguage);

			return new DocumentProcessResult
			{
				Success = !string.IsNullOrEmpty(translatedText),
				ProcessedContent = translatedText,
				OperationType = "translate",
				ProcessedAt = DateTime.UtcNow
			};
		}
	}
}
