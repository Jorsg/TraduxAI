using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Enumerations
{
	public enum ErrorCodes
	{
		NoError = 0,
		UnexpectedError = 1,
		ServerFailure = 4001,
		FileNotFound = 2107,
		LocaleNotSupported = 2273
	}
}
