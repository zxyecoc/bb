using Microsoft.AspNetCore.Identity;
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

    // Профіль користувача
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Користувача не знайдено");
        }

        var favoriteTeams = await _context.UserFavoriteTeams
            .Where(uf => uf.UserId == user.Id)
            .Select(uf => uf.Team)
            .ToListAsync();

        var model = new UserProfileViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            FavoriteTeams = favoriteTeams
        };

        return View(model);
    }


    // Додавання улюбленої команди
    [HttpGet]
    public async Task<IActionResult> AddFavoriteTeam()
    {
        var teams = await _context.Teams.ToListAsync(); // Завантажуємо список команд
        var model = new FavoriteTeamViewModel
        {
            Teams = teams, // Ініціалізація списку команд
            SelectedTeamIds = new List<int>() // Ініціалізуємо пустий список вибраних команд
        };

        return View(model); // Передаємо модель в представлення
    }

    // Обробка вибору улюбленої команди
    [HttpPost]
    public async Task<IActionResult> AddFavoriteTeam(FavoriteTeamViewModel model)
    {
        if (model.SelectedTeamIds == null || !model.SelectedTeamIds.Any())
        {
            ModelState.AddModelError("", "Ви повинні обрати хоча б одну команду.");
            model.Teams = await _context.Teams.ToListAsync(); // Повторно завантажуємо список команд
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User); // Отримуємо поточного користувача
        if (user == null)
        {
            return NotFound("Користувача не знайдено");
        }

        // Видалення старих улюблених команд
        var existingFavorites = _context.UserFavoriteTeams.Where(uf => uf.UserId == user.Id).ToList();
        _context.UserFavoriteTeams.RemoveRange(existingFavorites);

        // Додавання нових улюблених команд
        foreach (var teamId in model.SelectedTeamIds)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team != null)
            {
                var userFavoriteTeam = new UserFavoriteTeam
                {
                    UserId = user.Id,
                    TeamId = team.Id
                };
                _context.UserFavoriteTeams.Add(userFavoriteTeam);
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Profile");
    }
}
