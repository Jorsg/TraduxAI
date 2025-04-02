using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TraduxAI.Shared;
using TraduxAI.Shared.Errors;
using TraduxAI.Shared.Interfaces;
using TraduxAI.Shared.Models;

namespace TraduxAI.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FileTranslateController : ControllerBase
	{
		private readonly ITextToTranslateService _textToTranslate;
		public FileTranslateController(ITextToTranslateService textToTranslate)
		{
			_textToTranslate = textToTranslate;
		}

		[HttpPost("Translate")]
		public async Task<IActionResult> FileToTranslateAsync(ConvertFileRequest request)
		{
			ServiceResponse<string> response = new ServiceResponse<string>();
			response = await _textToTranslate.TextToTranslate(request);
			if(response.IsSucces) return StatusCode(StatusCodes.Status200OK, response.Data);
			else return StatusCode(ErrorMessages.StatusCode(response.ErroCode), response);
		}
	}
}
