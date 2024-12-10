using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using EShopApp.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Infrastructure.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public DataSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task MigrateAsync()
    {
        await _context.Database.MigrateAsync();
    }

    public async Task SetUpRoles()
    {
        var roleExists = await _roleManager.RoleExistsAsync("Admin");
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));
        }
    }

    public async Task CreateAdminAsync()
    {
        var admin = new User("Admin", "Admin", "Admin@gmail.com", new Address("Street 123", "City 123", "Country 123"));
        if (await _userManager.FindByEmailAsync("admin@gmail.com") == null)
        {
            var adminUser = ApplicationUser.FromUser(admin);
            await _userManager.CreateAsync(adminUser, "Admin123!");
            await _userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}