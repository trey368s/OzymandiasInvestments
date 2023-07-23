using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzymandiasInvestments.Areas.Identity.Data;
using OzymandiasInvestments.Classes;
using OzymandiasInvestments.Models.AppSettingsModels;

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

builder.Services.AddScoped<GetMarketData>(provider => new GetMarketData(ApiKey, ApiSecret));

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
