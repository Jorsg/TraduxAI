using iText.StyledXmlParser.Jsoup.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.DTOs;
using TraduxAI.Translation.Core.Services;

namespace TraduxAI.Translation.Core.Interfaces
{
	public interface IAuthService
	{
		Task<(bool Success, string Message)> RegisterUserAsync(RegisterUserDto registerUserDto);
		Task<(bool Success, string Message, TokenAccess? Token)> LoginUserAsync(LoginUserDto dto);

	}
}
