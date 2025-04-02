using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TraduxAI.Shared.Errors;
using TraduxAI.Shared.Models;
using TraduxAI.Translation.Core.Interfaces;

namespace TraduxAI.Translation.Core.Services
{
	public class OpenAIService : IOpenAIService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;
		private readonly string? _apiKey;

		public OpenAIService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_configuration = configuration;
			_apiKey = _configuration["OpenAI:ApiKey"];

			if (string.IsNullOrEmpty(_apiKey))
			{
				throw new ApiException("OpenAI API key is not configured", "configuration_error", 500);
			}

			_httpClient.BaseAddress = new Uri("https://api.openai.com/");
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
		}

		public async Task<string> GetCompletionAsync(string prompt, int maxTokens = 256)
		{
			var request = new OpenAIRequest
			{
				Model = "gpt-4",
				Messages = new List<Message>
				{
					new Message
					{
						Role = "user",
						Content = prompt
					}
				},
				Temperature = 0.7,
				MaxTokens = maxTokens
			};

			var response = await SendRequestAsync<OpenAIResponse>(request, "v1/chat/completions");
			return response.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
		}

		public async Task<string> ImageToTextAsync(string base64Image)
		{
			// Construct a prompt asking GPT-4 Vision to describe the image
			var request = new OpenAIRequest
			{
				Model = "gpt-4-vision-preview",
				Messages = new List<Message>
				{
					new Message
					{
						Role = "user",
						Content = "Please extract and transcribe all text from this image accurately. If there's no text, just describe what you see."
					}
				},
				MaxTokens = 300
			};

			// This is simplified - in a real implementation, you'd need to properly format the base64 image
			// for the vision API according to OpenAI's documentation

			var response = await SendRequestAsync<OpenAIResponse>(request, "v1/chat/completions");
			return response.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
		}

		public async Task<string> PdfToTextAsync(string base64Pdf)
		{
			// In a real implementation, you would use a PDF library to extract text
			// and then possibly use OpenAI to clean up or enhance the extraction

			// This is a placeholder implementation using GPT-4
			var promptText = "This is a PDF document. Please extract all text content from it and format it properly.";

			return await GetCompletionAsync(promptText, 1000);
		}

		public async Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage)
		{
			var prompt = $"Translate the following text from {sourceLanguage} to {targetLanguage}:\n\n{text}";

			return await GetCompletionAsync(prompt, 1000);
		}

		private async Task<T> SendRequestAsync<T>(object requestData, string endpoint)
		{
			var jsonContent = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});

			var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync(endpoint, content);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				throw new ApiException(
					$"OpenAI API Error: {response.StatusCode} - {errorContent}",
					"openai_api_error",
					(int)response.StatusCode);
			}

			var responseContent = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			}) ?? throw new ApiException("Failed to deserialize OpenAI response", "deserialization_error");
		}
	}
}
