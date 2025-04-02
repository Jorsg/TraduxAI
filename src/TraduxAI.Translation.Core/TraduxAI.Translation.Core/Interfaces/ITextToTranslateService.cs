using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Models;

namespace TraduxAI.Shared.Interfaces
{
	public interface ITextToTranslateService
	{

		Task<ServiceResponse<string>> TextToTranslate(ConvertFileRequest request);
	}
}
