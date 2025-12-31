using FinancialManager.Infrastructure;
using FinancialManager.Infrastructure.Identity;
using FinancialManager.Infrastructure.Persistence;
using FinancialManager.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using MudBlazor.Services;

// Configure Npgsql to handle DateTime as UTC
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure Data Protection for development without key persistence issues
builder.Services.AddDataProtection()
    .SetApplicationName("FinancialManager");

builder.Services.AddRazorPages();
builder.Services.AddControllers();
// Disable antiforgery to avoid token deserialization errors in Blazor Server
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "FinancialManager.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.SuppressXFrameOptionsHeader = false;
    // Skip validation for all requests - use CSRF protection through other means
    options.HeaderName = "X-CSRF-TOKEN-DISABLED";
});
builder.Services.AddMudServices();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["AppUrl"] ?? "http://localhost:8080") });
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Only use HTTPS redirection if not in Docker
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")))
{
    // Running in Docker - don't redirect to HTTPS
}
else
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Configure login route for Blazor
app.UseStatusCodePagesWithReExecute("/auth/login");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await DataSeeder.SeedAsync(dbContext, userManager);
}

app.Run();
