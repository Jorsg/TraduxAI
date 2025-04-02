using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Errors
{
	public class ApiException : Exception
	{
		public string Code { get; }
		public int StatusCode { get; }

		public ApiException(string message, string code = "unknown_error", int statusCode = 500)
			: base(message)
		{
			Code = code;
			StatusCode = statusCode;
		}

		public ApiException(string message, Exception innerException, string code = "unknown_error", int statusCode = 500)
			: base(message, innerException)
		{
			Code = code;
			StatusCode = statusCode;
		}
	}
}
