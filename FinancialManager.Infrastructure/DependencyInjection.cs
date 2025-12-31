using FinancialManager.Infrastructure.Identity;
using FinancialManager.Infrastructure.Persistence;
using FinancialManager.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinancialManager.Application.Services;

namespace FinancialManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") 
            ?? "Host=localhost;Database=financialmanager;Username=postgres;Password=postgres";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/login";
            options.AccessDeniedPath = "/login";
            options.Cookie.HttpOnly = true;
            options.SlidingExpiration = true;
        });

        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IIncomeService, IncomeService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IBankAccountService, BankAccountService>();
        services.AddScoped<ICreditCardService, CreditCardService>();

        return services;
    }
}
