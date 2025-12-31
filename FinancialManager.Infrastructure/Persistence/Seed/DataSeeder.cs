using FinancialManager.Domain.Entities;
using FinancialManager.Domain.Enums;
using FinancialManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager, CancellationToken cancellationToken = default)
    {
        // Ensure database is created and migrations are applied
        await db.Database.MigrateAsync(cancellationToken);

        var adminEmail = "admin@admin.com";
        var adminPassword = "123456";
 
         var admin = await userManager.FindByEmailAsync(adminEmail);
         if (admin == null)
         {
             admin = new ApplicationUser
             {
                 UserName = adminEmail,
                 Email = adminEmail,
                 EmailConfirmed = true
             };

             await userManager.CreateAsync(admin, adminPassword);
         }
         else if (!await userManager.CheckPasswordAsync(admin, adminPassword))
         {
             await userManager.RemovePasswordAsync(admin);
             await userManager.AddPasswordAsync(admin, adminPassword);
         }
     }
 }

