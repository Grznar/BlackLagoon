using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Application.Services.Implemententaion;
using BlackLagoon.Application.Services.Interface;
using BlackLagoon.Common.Interfaces;
using BlackLagoon.Domain.Entities;
using BlackLagoon.Infrastructure.Data;
using BlackLagoon.Infrastructure.Migrations;
using BlackLagoon.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options=>
options.UseSqlServer
(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDashBoardService, DashboardService>();
builder.Services.AddScoped<IDbInitializor, DbInitializator>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = "/Account/AccessDenied";
    option.LoginPath = "/Account/Login";
    
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;

}
);
    
var app = builder.Build();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
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

app.UseAuthorization();
SeedDatabase();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbInitializator = services.GetRequiredService<IDbInitializor>();
        dbInitializator.Initializeta();
    }
    }