using LAB1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LAB1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LAB1.Controllers
{
    [Authorize(Roles = "Administrator")] // Доступ тільки для адміністратора
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Перевірка існування ролей та їх створення
        public async Task<IActionResult> EnsureRolesExist()
        {
            var roles = new[] { "Administrator", "Translator" };

            foreach (var roleName in roles)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole(roleName);
                    await _roleManager.CreateAsync(role);
                }
            }

            return Ok("Ролі перевірені та створені, якщо потрібно.");
        }

        // Додавання ролі користувачеві
        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                return NotFound("Роль не існує");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Користувача не знайдено");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return RedirectToAction("Users"); // Перенаправляємо на список користувачів
            }

            return BadRequest("Не вдалося додати роль користувачеві");
        }

        // Видалення ролі у користувача
        [HttpPost]
        public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Користувача не знайдено");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return RedirectToAction("Users"); // Перенаправляємо на список користувачів
            }

            return BadRequest("Не вдалося забрати роль у користувача");
        }

        // Список користувачів з можливістю призначення ролей
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var userRoleViewModel = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoleViewModel.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return View(userRoleViewModel); // Повертає список користувачів
        }
    }
}
