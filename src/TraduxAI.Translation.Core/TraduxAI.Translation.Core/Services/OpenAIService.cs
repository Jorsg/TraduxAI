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

		public async Task<string> GetCompletionAsync(string prompt)
		{
			var request = new OpenAIRequest
			{
				Model = "gpt-4o",
				Messages = new List<Message>
				{
					new Message
					{
						Role = "user",
						ContentText = prompt
					}
				},
				Temperature = 0.7,
				//MaxTokens = maxTokens
			};

			var response = await SendRequestAsync<OpenAIResponse>(request, "v1/chat/completions");
			return response.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
		}

		public async Task<string> ImageToTextAsync(string base64Image)
		{
			// Construct a prompt asking GPT-4 Vision to describe the image
			var request = new OpenAIRequest
			{
				Model = "gpt-4o",
				//MaxTokens = 300,

				Messages = new List<Message>
				{
					new Message
					{
						Role = "user",
						Content = new List<MessageContent>
						{
							new MessageContent
							{
								Type = "text",
								Text = "Please extract and transcribe all text from this image. If there's no text, just describe the image."
							},
							new MessageContent
							{
								Type = "image_url",
								Image_url = new ImageUrl
								{
									Url = $"data:image/png;base64,{base64Image}",
								}
							}
						},
					}
				}
			};

			var response = await SendRequestAsync<OpenAIResponse>(request, "v1/chat/completions");
			return response.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
		}

		public async Task<string> PdfToTextAsync(string base64Pdf)
		{
			// In a real implementation, you would use a PDF library to extract text
			// and then possibly use OpenAI to clean up or enhance the extraction

			// This is a placeholder implementation using GPT-4
			var promptText = "This is a PDF document. Please extract all text content from it and format it properly.";

			return await GetCompletionAsync(promptText);
		}

		public async Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage)
		{
			var prompt = $"Translate the following text from {sourceLanguage} to {targetLanguage}:\n\n{text}";

			return await GetCompletionAsync(prompt);
		}

		private async Task<T> SendRequestAsync<T>(object requestData, string endpoint)
		{
			var jsonContent = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
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
