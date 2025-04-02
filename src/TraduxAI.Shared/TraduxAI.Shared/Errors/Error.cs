using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TraduxAI.Shared.Errors
{
	public class Error
	{
		public int Code { get; set; }
		[JsonIgnore]
		public int StatusCode { get; set; }
		[JsonIgnore]
		public System.Net.HttpStatusCode httpStatusCode { get; set; }
		public string ErrorMessage { get; set; }
		public string Detail { get; set; }
		public string Source { get; set; }
		[JsonIgnore]
		public bool HasError { get; set; }

		public Error()
		{
			HasError = false;
		}

		public Error(int code)
		{
			code = code;
			ErrorMessage = ErrorMessages.Message(code);
			StatusCode = ErrorMessages.StatusCode(code);
			httpStatusCode = (System.Net.HttpStatusCode)ErrorMessages.StatusCode(code);
			HasError = true;
		}

		public Error(int code, string details) : this(code)
		{
			Detail = details;
		}

		public Error(string errMsg, int code) : this(code)
		{
			Code = code;
			ErrorMessage = errMsg;
		}
	}
}
