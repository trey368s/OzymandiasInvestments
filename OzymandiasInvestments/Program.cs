using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzymandiasInvestments.Areas.Identity.Data;
using OzymandiasInvestments.Classes;
using OzymandiasInvestments.Models.AppSettingsModels;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityDBContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityDBContextConnection' not found.");

builder.Services.AddDbContext<IdentityDBContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<InvestmentDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<OzymandiasInvestmentsUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityDBContext>();

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", false, true)
    .Build();

var configSettings = configuration.Get<ConfigOptions>();
var ApiKey = configSettings.AlpacaApiSettings.ApiKey;
var ApiSecret = configSettings.AlpacaApiSettings.ApiSecret;
var alphaVantageKey = configSettings.alphaVantageKey;
var alphaVantageKey2 = configSettings.alphaVantageKey2;
var emailSettings = configSettings.EmailSettings;
var gptKey = configSettings.gptKey;

var dockerGptKey = Environment.GetEnvironmentVariable("GPT_API_KEY");

builder.Services.AddScoped(provider => new GetMarketData(ApiKey, ApiSecret, alphaVantageKey, alphaVantageKey2));
builder.Services.AddScoped(provider => new GetOrderData());
builder.Services.AddScoped(provider => new GetPositionData());
builder.Services.AddScoped(provider => new GetActivityData(ApiKey, ApiSecret));
builder.Services.AddScoped(provider => new SendEmail(emailSettings));
builder.Services.AddScoped(provider => new GetChatGPTResponse(dockerGptKey));
builder.Services.AddScoped(provider => new OpenAIService(dockerGptKey));

builder.Services.AddControllersWithViews().AddRazorPagesOptions(options =>
{
    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Stock}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
