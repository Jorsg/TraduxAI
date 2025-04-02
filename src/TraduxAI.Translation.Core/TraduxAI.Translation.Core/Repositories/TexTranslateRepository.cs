using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Text;
using Tesseract;
using TraduxAI.Shared.Enumerations;
using TraduxAI.Shared.Interfaces;
using TraduxAI.Shared.Models;

namespace TraduxAI.Shared.Repositories
{
	public class TexTranslateRepository : ITexTranslateRepository
	{
		private readonly IConfiguration _configuration;

		public TexTranslateRepository(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public async Task<ServiceResponse<string>> ConvertImgToText(ConvertFileRequest request)
		{
			var response = new ServiceResponse<string>();
			string imgFilepaht = request.FilePath;
			string tesstDataDirectory = _configuration["DataDirectory:DataLangagues"];

			using var engine = new TesseractEngine(tesstDataDirectory, "eng", EngineMode.Default);
			using Pix img = Pix.LoadFromFile(imgFilepaht);
			using Page page = engine.Process(img);

			response.Data = page.GetText();
			if (response.IsSucces)
				response.ErroCode = (int)ErrorCodes.NoError;

			return response;
		}

		public async Task<ServiceResponse<string>> GetConvertPdfToText(ConvertFileRequest request)
		{
			var response = new ServiceResponse<string>();
			string pdFFlePaht = request.FilePath;
			StringBuilder sb = new StringBuilder();
			try
			{
				using (PdfReader reader = new PdfReader(pdFFlePaht))
				{
					for (int i = 1; i <= reader.NumberOfPages; i++)
					{
						ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
						string text = PdfTextExtractor.GetTextFromPage(reader, i, strategy);
						text = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8,
							Encoding.Default.GetBytes(text)));
						sb.Append(text);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

			response.Data = sb.ToString();
			if (response.IsSucces)
				response.ErroCode = (int)ErrorCodes.NoError;

			return response;
		}

		public async Task<ServiceResponse<string>> TextTranslate(string text, ConvertFileRequest requestImg)
		{
			var response = new ServiceResponse<string>();
			var body = new ConvertToJson
			{
				model = "gpt-4o",
				messages = new List<MessageRequest>() { new MessageRequest() { role = "user", content = $"translate spanish to english {text}" } }
			};
			string json = JsonConvert.SerializeObject(body);
			string apikey = _configuration["ApikeyGpt:Apikey"];
			string url = _configuration["GptService:Url"];
			string athurization = _configuration["GptService:BearerAuthorization"];
			string translateText = string.Empty;
			var client = new RestClient(url);
			var request = new RestRequest();
			request.Method = Method.Post;
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Authorization", "Bearer " + apikey);

			request.AddJsonBody(json);

			var result = client.Execute(request);

			JObject jsonObject = JObject.Parse(result.Content);

			JToken contentToken = jsonObject.SelectToken("choices");
			if (contentToken == null)
				response.ErroCode = (int)ErrorCodes.ServerFailure;
			else
			{
				response.Data = contentToken.ToString();
				if (response.IsSucces)
					response.ErroCode = (int)ErrorCodes.NoError;
			}
			return response;
		}
	}
}
