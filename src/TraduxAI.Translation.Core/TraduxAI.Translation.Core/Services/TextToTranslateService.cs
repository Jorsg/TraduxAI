using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Interfaces;
using TraduxAI.Shared.Models;

namespace TraduxAI.Shared.Services
{
	public class TextToTranslateService : ITextToTranslateService
	{
		ITexTranslateRepository _texTranslateRepository;

		public TextToTranslateService(ITexTranslateRepository texTranslateRepository)
		{
			_texTranslateRepository = texTranslateRepository;
		}
		public async Task<ServiceResponse<string>> TextToTranslate(ConvertFileRequest request)
		{
			var response = new ServiceResponse<string>();
			var result = new ServiceResponse<string>();

			string input = request.FilePath.Trim();
			char delimiter = '.';
			string[] substring = input.Split(delimiter);

			if (substring[3].Equals("pdf"))
			{
				var text = await _texTranslateRepository.GetConvertPdfToText(request);
				if (text.IsSucces)
				{
					result = await _texTranslateRepository.TextTranslate(text.Data, request);

					if (result.Data != null)
					{
						response.Data = result.Data;
						response.ErroCode = (int)result.ErroCode;
					}
					else
					{
						response.ErroCode = result.ErroCode;
						response.ErorMessage = result.ErorMessage;
					}
				}
			}
			else
			{
				var text = await _texTranslateRepository.ConvertImgToText(request);
				if (text.IsSucces)
				{
					result = await _texTranslateRepository.TextTranslate(text.Data, request);

					if (result.Data != null)
					{
						response.Data = result.Data;
						response.ErroCode = (int)result.ErroCode;
					}
					else
					{
						response.ErroCode = result.ErroCode;
						response.ErorMessage = result.ErorMessage;
					}
				}
			}


			return response;
		}
	}
}
