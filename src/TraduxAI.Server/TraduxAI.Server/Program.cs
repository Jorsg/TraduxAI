using TraduxAI.Shared.Errors;
using TraduxAI.Translation.Core.Interfaces;
using TraduxAI.Translation.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Register HTTP client
builder.Services.AddHttpClient<IOpenAIService, OpenAIService>();

// Register services
builder.Services.AddTransient<IOpenAIService, OpenAIService>();
builder.Services.AddTransient<IDocumentProcessor, DocumentProcessorService>();

// maxim request body size
builder.WebHost.ConfigureKestrel(serverOptions =>
{
	serverOptions.Limits.MaxRequestBodySize = 104857600; // 100 MB
});


//binding Json
var configureBuild = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsttings.json");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    configureBuild.AddJsonFile($"appsetting.{builder.Environment.EnvironmentName}.json");
    configureBuild.AddUserSecrets<Program>();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Global error handler
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.ContentType = "application/json";

var exceptionHandlerFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

if (exceptionHandlerFeature != null)
{
	var exception = exceptionHandlerFeature.Error;

	var statusCode = exception is ApiException apiException
		? apiException.StatusCode
		: StatusCodes.Status500InternalServerError;

	var errorCode = exception is ApiException apiEx
		? apiEx.Code
		: "internal_server_error";

	context.Response.StatusCode = statusCode;

	var apiError = new ApiError
	{
		Code = errorCode,
		Message = exception.Message,
		Details = app.Environment.IsDevelopment() ? exception.StackTrace ?? string.Empty : string.Empty
	};

	await context.Response.WriteAsJsonAsync(apiError);
}
    });
});


app.MapControllers();


app.Run();

