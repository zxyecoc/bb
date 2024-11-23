﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LAB1.Models;
using Microsoft.AspNetCore.Authorization; 
using LAB1.Resources;
using SQLitePCL; 
using LAB1.Data;
using LAB1.Models;
using Microsoft.EntityFrameworkCore;



public class UserController : Controller
{
    private readonly NewsBlogContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(NewsBlogContext context, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _context = context;  
        _userManager = userManager;
        _signInManager = signInManager;

    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (model == null)
        {
            ModelState.AddModelError("", "Model cannot be null");
            return View("Register", model);
        }

        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Автоматичний вхід після реєстрації
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Додаємо повідомлення про помилки до ModelState, якщо реєстрація не вдалася
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View("Register", model); // Повертаємо модель для відображення помилок
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            // Знаходимо користувача за Email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                // Виконуємо вхід, використовуючи UserName
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Невірна спроба входу.");
        }
        return View(model);
    }


    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Користувача не знайдено");
        }

        var roles = await _userManager.GetRolesAsync(user);

        // Якщо немає жодної ролі, встановлюємо "Читач"
        if (roles == null || roles.Count == 0)
        {
            roles = new List<string> { "Читач" };
        }
        var model = new UserProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            Roles = string.Join(", ", roles.Select(role => RoleTranslations.ResourceManager.GetString(role) ?? role)),
        };


        return View(model);
    }
}
