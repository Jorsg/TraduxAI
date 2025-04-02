using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TraduxAI.Shared.Enumerations;

namespace TraduxAI.Shared.Errors
{
	public class ErrorMessages
	{

		static Dictionary<int, string> _messages;
		static Dictionary<int, HttpStatusCode> _statusCodes;
		static Dictionary<ErrorCodes, string> _logMessages;
		static ErrorMessages()
		{

			_messages = new Dictionary<int, string>();
			_statusCodes = new Dictionary<int, HttpStatusCode>();
			_statusCodes[(int)ErrorCodes.NoError] = HttpStatusCode.OK;

			//# Error InternalServerError
			AddMessage(0001, HttpStatusCode.InternalServerError, "For unexperted error that have occured within present endpoin excluding errors from the respository calls");
			AddMessage(0002, HttpStatusCode.InternalServerError, "Generic exception that occurred within the repository calls");
			AddMessage(0002, HttpStatusCode.InternalServerError, "SQL exception that occurred within the repository calls");
			AddMessage(0003, HttpStatusCode.InternalServerError, "First name is requered");
			AddMessage(0004, HttpStatusCode.InternalServerError, "Last name is requered");
			AddMessage(0005, HttpStatusCode.InternalServerError, "Address line 1 is requered");
			AddMessage(0006, HttpStatusCode.InternalServerError, "City is requered");
			AddMessage(0007, HttpStatusCode.InternalServerError, "Postal code is requered");
			AddMessage(4001, HttpStatusCode.InternalServerError, "GTP is failling");


			//# Error Authorization
			AddMessage(1001, HttpStatusCode.Forbidden, "Request authorization failed validation");
			AddMessage(1002, HttpStatusCode.Unauthorized, "Request authorization failed validation");

			AddMessage(2001, HttpStatusCode.BadRequest, "Ivalid Product Identifier");
			AddMessage(2002, HttpStatusCode.BadRequest, "Customer Identifier from route is null");
			AddMessage(2003, HttpStatusCode.BadRequest, "Customer Identifier from route is not numeric value");
			AddMessage(2004, HttpStatusCode.BadRequest, "Customer Identifier from route is a numeric value less than or equal 0");
			AddMessage(2005, HttpStatusCode.BadRequest, "Email is null or empity");
			AddMessage(2006, HttpStatusCode.BadRequest, "Customer is invalid as not able to load of a customer record from database off it");
			AddMessage(2007, HttpStatusCode.BadRequest, "No addresses found for Csustomer");


		}

		private static void AddMessage(int errorCode, HttpStatusCode statusCode, string errorMessage)
		{
			_messages[errorCode] = errorMessage;
			_statusCodes[errorCode] = statusCode;
		}
		public static int StatusCode(int code)
		{
			if (_statusCodes.TryGetValue((int)code, out HttpStatusCode statusCode)) return (int)statusCode;
			return (int)HttpStatusCode.InternalServerError;

		}

		public static int StatusCode(ErrorCodes code)
		{
			if (_statusCodes.TryGetValue((int)code, out HttpStatusCode statusCode)) return (int)statusCode;
			return (int)HttpStatusCode.InternalServerError;

		}
		public static string Message(ErrorCodes code)
		{

			if (_messages.TryGetValue((int)code, out string message)) return message;
			return "Unexpected Server Error";
		}

		public static string Message(int code)
		{

			if (_messages.TryGetValue(code, out string message)) return message;
			return "Unexpected Server Error";
		}

		public static string LogMessage(ErrorCodes code)
		{

			if (_logMessages.TryGetValue(code, out string message)) return message;
			return "Unexpected Server Error";
		}


	}
}
