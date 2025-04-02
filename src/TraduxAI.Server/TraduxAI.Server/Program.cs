using TraduxAI.Shared.Interfaces;
using TraduxAI.Shared.Repositories;
using TraduxAI.Shared.Services;
using TraduxAI.Translation.Core.Interfaces;
using TraduxAI.Translation.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<ITextToTranslateService, TextToTranslateService>();
builder.Services.AddTransient<ITexTranslateRepository, TexTranslateRepository>();
builder.Services.AddOpenApi();
// Register HTTP client
builder.Services.AddHttpClient<IOpenAIService, OpenAIService>();

// Register services
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<IDocumentProcessor, DocumentProcessorService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowBlazorClient", policy =>
	{
		policy.WithOrigins("https://localhost:7100") // Blazor client URL
			.AllowAnyMethod()
			.AllowAnyHeader();
	});
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
app.MapControllers();


app.Run();

