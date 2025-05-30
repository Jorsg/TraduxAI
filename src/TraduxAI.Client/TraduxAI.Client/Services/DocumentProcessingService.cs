using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TraduxAI.Client.Models;



namespace TraduxAI.Client.Services
{
	public interface IDocumentProcessingService
	{
		Task<DocumentProcessResult> ProcessImageToTextAsync(byte[] imageData, string token, string promtp);
		Task<DocumentProcessResult> ProcessPdfToTextAsync(byte[] pdfData, string token);
		Task<DocumentProcessResult> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage, string token);
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

		public async Task<DocumentProcessResult> ProcessImageToTextAsync(byte[] imageData, string token, string promtp)
		{
			var base64Image = Convert.ToBase64String(imageData);

			var body = new
			{
				promtp = promtp,
                Base64Content = base64Image
			};

			var request = new HttpRequestMessage(HttpMethod.Post, "api/documentprocessing/ocr/image")
			{
				Content = new StringContent(JsonSerializer.Serialize(body, _jsonOptions), Encoding.UTF8, "application/json")
			};

			// ✅ add token to header Authorization
			if (!string.IsNullOrEmpty(token))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}

			var response = await _httpClient.SendAsync(request);
			return await HandleResponseAsync<DocumentProcessResult>(response);
		}

		public async Task<DocumentProcessResult> ProcessPdfToTextAsync(byte[] pdfData, string token)
		{
			var base65Pdf = Convert.ToBase64String(pdfData);
			var payload = JsonSerializer.Serialize(new {base64Content = base65Pdf }, _jsonOptions);
			
			var request = new HttpRequestMessage(HttpMethod.Post, "api/documentprocessing/ocr/pdf")
			{
				Content = new StringContent(payload, Encoding.UTF8, "application/json")
			};

			//✅ add token to header Authorization
			if (!string.IsNullOrEmpty(token))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
			var response = await _httpClient.SendAsync(request);
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

		public async Task<DocumentProcessResult> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage, string token)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "api/documentprocessing/translate")
			{
				Content = new StringContent(JsonSerializer.Serialize(new { text, sourceLanguage, targetLanguage }, _jsonOptions), Encoding.UTF8, "application/json")
			};

			// ✅ add token to header Authorization
			if (!string.IsNullOrEmpty(token))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}

			var response = await _httpClient.SendAsync(request);
			return await HandleResponseAsync<DocumentProcessResult>(response);
		}
	}
}
