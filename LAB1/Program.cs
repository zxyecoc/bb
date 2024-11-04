using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LAB1.Data;
using Microsoft.AspNetCore.Identity;
using LAB1.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LAB1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LAB1Context") ?? throw new InvalidOperationException("Connection string 'LAB1Context' not found.")));

// Додаємо службу Identity для керування користувачами та ролями
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<LAB1Context>()
    .AddDefaultTokenProviders();

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

app.UseAuthentication(); // Додаємо аутентифікацію
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ініціалізуємо ролі під час запуску програми
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedRoles(roleManager);
}

app.Run();

// Метод для створення ролей, якщо вони ще не існують
async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    if (!await roleManager.RoleExistsAsync("Administrator"))
    {
        await roleManager.CreateAsync(new IdentityRole("Administrator"));
    }

    if (!await roleManager.RoleExistsAsync("Translator"))
    {
        await roleManager.CreateAsync(new IdentityRole("Translator"));
    }
}

