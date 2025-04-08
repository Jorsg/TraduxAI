using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json;
using TraduxAI.Client.Providers;
using TraduxAI.Client.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<IDocumentProcessingService, DocumentProcessingService>();

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddSingleton(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

builder.Services.AddAuthorizationCore();

//Configure httpClient for API Call
builder.Services.AddHttpClient<DocumentProcessingService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/");
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
