using Microsoft.EntityFrameworkCore;
using Modique.Domain.Entities;
using Modique.Infrastructure.Data;
using BCrypt.Net;

namespace Modique.API.Extensions;

public static class DbSeeder
{
    public static async Task SeedDataAsync(ModiqueDbContext context)
    {
        await SeedCategoriesAsync(context);
        await SeedBrandsAsync(context);
        await SeedAdminUserAsync(context);
    }

    private static async Task SeedCategoriesAsync(ModiqueDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new[]
        {
            new Category { Name = "Muška odjeća", SubCategory = "Odekca", Description = "Muška moda i odjeća" },
            new Category { Name = "Ženska odjeća", SubCategory = "Odekca", Description = "Ženska moda i odjeća" },
            new Category { Name = "Dječija odjeća", SubCategory = "Odekca", Description = "Odjeća za djecu" },
            new Category { Name = "Obuća", SubCategory = "Obuća", Description = "Muška, ženska i dječija obuća" },
            new Category { Name = "Torbe", SubCategory = "Dodaci", Description = "Ručne torbe i novčanici" },
            new Category { Name = "Dodaci", SubCategory = "Dodaci", Description = "Modni dodaci" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBrandsAsync(ModiqueDbContext context)
    {
        if (await context.Brands.AnyAsync())
            return;

        var brands = new[]
        {
            new Brand { Name = "Longchamp", Country = "Francuska", LogoURL = "/assets/img/longchamp-logo.png" },
            new Brand { Name = "Adidas", Country = "Njemačka", LogoURL = "/assets/img/adidas-logo.png" },
            new Brand { Name = "Nike", Country = "SAD", LogoURL = "/assets/img/nike-logo.png" },
            new Brand { Name = "Puma", Country = "Njemačka", LogoURL = "/assets/img/puma-logo.png" },
            new Brand { Name = "Lacoste", Country = "Francuska", LogoURL = "/assets/img/lacoste-logo.png" },
            new Brand { Name = "New Balance", Country = "SAD", LogoURL = "/assets/img/newbalance-logo.png" },
            new Brand { Name = "Golden Goose", Country = "Italija", LogoURL = "/assets/img/goldengoose-logo.png" },
            new Brand { Name = "Tommy Hilfiger", Country = "SAD", LogoURL = "/assets/img/tommy-logo.png" }
        };

        await context.Brands.AddRangeAsync(brands);
        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(ModiqueDbContext context)
    {
        if (await context.Users.AnyAsync(u => u.Email == "admin@modique.com"))
            return;

        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Administrator");
        if (adminRole == null)
            return;

        string passwordSalt = BCrypt.Net.BCrypt.GenerateSalt();
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!", passwordSalt);

        var adminUser = new User
        {
            FirstName = "Admin",
            LastName = "Modique",
            Email = "admin@modique.com",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            RoleId = adminRole.RoleId,
            RegistrationDate = DateTime.UtcNow,
            Active = true
        };

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }
}
