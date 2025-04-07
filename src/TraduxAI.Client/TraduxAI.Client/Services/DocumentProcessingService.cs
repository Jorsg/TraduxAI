using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TraduxAI.Shared.Models;



namespace TraduxAI.Client.Services
{
	public interface IDocumentProcessingService
	{
		Task<DocumentProcessResult> ProcessImageToTextAsync(byte[] imageData);
		Task<DocumentProcessResult> ProcessPdfToTextAsync(byte[] pdfData);
		Task<DocumentProcessResult> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage);
	}
	public class DocumentProcessingService : IDocumentProcessingService
	{


		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _jsonOptions;
		
		public DocumentProcessingService(HttpClient httpClient)
		{
			_httpClient = httpClient;
			_jsonOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			};
		}

        public async Task<DocumentProcessResult> ProcessImageToTextAsync(byte[] imageData)
		{
			var base64Image = Convert.ToBase64String(imageData);
			var payload = JsonSerializer.Serialize(new { base64Image }, _jsonOptions);
			var content = new StringContent(payload, Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsJsonAsync("api/documentprocessing/ocr/image",content);
			return await HandleResponseAsync<DocumentProcessResult>(response);

		}

		public async Task<DocumentProcessResult> ProcessPdfToTextAsync(byte[] pdfData)
		{
			var base65Pdf = Convert.ToBase64String(pdfData);
			var payload = JsonSerializer.Serialize(new { base65Pdf }, _jsonOptions);
			var content = new StringContent(payload, Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsJsonAsync("api/documentprocessing/ocr/pdf", content);
			return await HandleResponseAsync<DocumentProcessResult>(response);
		}

		private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
				return result ?? throw new Exception("Failed to deserialize response");
			}
			var errorContent = await response.Content.ReadAsStringAsync();
			throw new Exception($"API Error: {response.StatusCode}, Content: {errorContent}"); // Handle error response
		}

		public async Task<DocumentProcessResult> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage)
		{
			var request = new
			{
				Text = text,
				SourceLanguage = sourceLanguage,
				TargetLanguage = targetLanguage
			};

			var payload = JsonSerializer.Serialize(request, _jsonOptions);
			var content = new StringContent(payload, Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsJsonAsync("api/documentprocessing/translate", content);
			return await HandleResponseAsync<DocumentProcessResult>(response);
		}




	}
}
