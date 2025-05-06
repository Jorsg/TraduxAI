using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.Design;
using System.Text;
using TraduxAI.Shared.Data;
using TraduxAI.Shared.Errors;
using TraduxAI.Shared.Models;
using TraduxAI.Translation.Core.Interfaces;
using TraduxAI.Translation.Core.Repositories;
using TraduxAI.Translation.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

//Bind JwtSettings
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key_Api = builder.Configuration["Jwt:key"];
var key = Encoding.ASCII.GetBytes(key_Api);


// Register services
builder.Services.AddTransient<IOpenAIService, OpenAIService>();
builder.Services.AddTransient<IDocumentProcessor, DocumentProcessorService>();
builder.Services.Configure<MongoDbSettings>(
	builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddTransient<IUserRepository,UserRepository>();
builder.Services.AddTransient<MongoDbContext>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IRefreshTokenServices, RefreshTokenService>();
builder.Services.AddTransient<RefreshTokenRepository>();
builder.Services.AddTransient<IJwtService, JwtService>();


// Register HTTP client
builder.Services.AddHttpClient<IOpenAIService, OpenAIService>();



// maxim request body size
builder.WebHost.ConfigureKestrel(serverOptions =>
{
	serverOptions.Limits.MaxRequestBodySize = 104857600; // 100 MB
});


//binding Json
var configureBuild = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsttings.json");

//Authentication register
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = true;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidIssuer = jwtSettings.Issuer,
		ValidAudience = jwtSettings.Audience,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ClockSkew = TimeSpan.Zero
	};
	options.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
			if (context.Request.Cookies.ContainsKey("jwt_token"))
			{
				context.Token = context.Request.Cookies["jwt_token"];
			}
			return Task.CompletedTask;
		},
	};
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.Cookie.Name = "jwt_token";
		options.Cookie.HttpOnly = true;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
		options.ExpireTimeSpan = TimeSpan.FromDays(30);
		options.SlidingExpiration = true;
		options.LoginPath = "/login";
	});

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();

