using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Models;

namespace TraduxAI.Shared.Interfaces
{
	public interface ITexTranslateRepository
	{
		Task<ServiceResponse<string>> TextTranslate(string text, ConvertFileRequest request);
		Task<ServiceResponse<string>> ConvertImgToText(ConvertFileRequest request);
		Task<ServiceResponse<string>> GetConvertPdfToText(ConvertFileRequest request);
	}
}
