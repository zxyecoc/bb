using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LAB1.Data;
using Microsoft.AspNetCore.Identity;
using LAB1.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LAB1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LAB1Context") ?? throw new InvalidOperationException("Connection string 'LAB1Context' not found."))
     .EnableSensitiveDataLogging()
    );

// Додаємо службу Identity для керування користувачами та ролями
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;            // Вимога наявності цифр
    options.Password.RequiredLength = 6;             // Мінімальна довжина пароля
    options.Password.RequireNonAlphanumeric = true;  // Вимога спеціальних символів
    options.Password.RequireUppercase = true;        // Вимога великої літери
    options.Password.RequireLowercase = true;        // Вимога малої літери
})
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
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedRoles(roleManager);
    await SeedAdminUser(userManager, roleManager);
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

async Task SeedAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
{
    // Перевіряємо, чи існує користувач з електронною поштою admin@gmail.com
    var existingUser = await userManager.FindByEmailAsync("admin@gmail.com");

    if (existingUser == null)
    {
        // Створюємо нового користувача
        var adminUser = new User
        {
            UserName = "admin@gmail.com",
            Email = "admin@gmail.com",
            EmailConfirmed = true // Ви можете встановити EmailConfirmed у true, щоб не вимагати підтвердження
        };

        // Створюємо користувача з паролем
        var createUserResult = await userManager.CreateAsync(adminUser, "Admin_12345678");

        if (createUserResult.Succeeded)
        {
            // Перевіряємо, чи існує роль Administrator і додаємо її користувачу
            if (await roleManager.RoleExistsAsync("Administrator"))
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }
        else
        {
            // Логування помилок, якщо створення користувача не вдалося
            foreach (var error in createUserResult.Errors)
            {
                Console.WriteLine($"Помилка створення користувача: {error.Description}");
            }
        }
    }
}

