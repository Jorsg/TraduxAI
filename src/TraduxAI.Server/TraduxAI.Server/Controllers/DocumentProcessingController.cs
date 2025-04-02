using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraduxAI.Shared.Models;
using TraduxAI.Translation.Core.Interfaces;

namespace TraduxAI.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentProcessingController : ControllerBase
	{
		private readonly IDocumentProcessor _documentProcessor;

		public DocumentProcessingController(IDocumentProcessor documentProcessor)
		{
			_documentProcessor = documentProcessor;
		}
		[HttpPost("process")]
		public async Task<ActionResult<DocumentProcessResult>> ProcessDocument([FromBody] DocumentProcessRequest request)
		{
			if (string.IsNullOrEmpty(request.Content))
				return BadRequest("Document content is required");

			var result = await _documentProcessor.ProcessDocumentAsync(request);
			return Ok(result);
		}

		[HttpPost("ocr/image")]
		public async Task<ActionResult<DocumentProcessResult>> ExtractTextFromImage([FromBody] string base64Image)
		{
			if (string.IsNullOrEmpty(base64Image))
				return BadRequest("Image content is required");

			var result = await _documentProcessor.ExtractTextFromImageAsync(base64Image);
			return Ok(result);
		}

		[HttpPost("ocr/pdf")]
		public async Task<ActionResult<DocumentProcessResult>> ExtractTextFromPdf([FromBody] string base64Pdf)
		{
			if (string.IsNullOrEmpty(base64Pdf))
				return BadRequest("PDF content is required");

			var result = await _documentProcessor.ExtractTextFromPdfAsync(base64Pdf);
			return Ok(result);
		}

		[HttpPost("translate")]
		public async Task<ActionResult<DocumentProcessResult>> TranslateText([FromBody] TranslateRequest request)
		{
			if (string.IsNullOrEmpty(request.Text))
				return BadRequest("Text content is required");

			var result = await _documentProcessor.TranslateTextAsync(
				request.Text,
				request.SourceLanguage ?? "auto",
				request.TargetLanguage ?? "en");
			return Ok(result);
		}
	}

	public class TranslateRequest
	{
		public string Text { get; set; } = string.Empty;
		public string? SourceLanguage { get; set; }
		public string? TargetLanguage { get; set; }
	}
}

