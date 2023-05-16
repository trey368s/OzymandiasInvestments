using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzymandiasInvestments.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityDBContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityDBContextConnection' not found.");

builder.Services.AddDbContext<IdentityDBContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<InvestmentDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<OzymandiasInvestmentsUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityDBContext>();

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
