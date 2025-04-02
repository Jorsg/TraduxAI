using TraduxAI.Shared.Enumerations;
using TraduxAI.Shared.Errors;

namespace TraduxAI.Shared;

public class ServiceResponse<T> where T : class
{

	public T Data { get; set; }
	public int ErroCode { get; set; }
	public string ErorMessage { get; set; }
	public bool IsSucces { get { return ErroCode == (int)ErrorCodes.NoError; } }
	public Error error = new Error();
	public List<FieldError> errorsList { get; set; }
	public ServiceResponse() { }
	public ServiceResponse(T data) { Data = data; }

	public ServiceResponse<T> CatchError(int errorCode, params object?[] arg)
	{
		ServiceResponse<T> response = new()
		{
			ErroCode = errorCode,
			ErorMessage = String.Format(ErrorMessages.Message(errorCode), arg)
		};
		return response;
	}

}
