using LAB1.Data;
using LAB1.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace LAB1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NewsBlogContext _context;
        private readonly IStringLocalizer<HomeController> _localizer;

        // Об'єднаний конструктор
        public HomeController(ILogger<HomeController> logger, NewsBlogContext context, IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _context = context;
            _localizer = localizer;
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> Index(int? tagId, int? authorId, string sortOrder)
        {
            // Отримання всіх новин
            var newsQuery = _context.News
                .Include(n => n.Tags)
                .Include(n => n.Author)
                .AsQueryable();

            // Фільтрація за тегом
            if (tagId.HasValue)
            {
                newsQuery = newsQuery.Where(n => n.Tags.Any(t => t.Id == tagId));
            }

            // Фільтрація за автором
            if (authorId.HasValue)
            {
                newsQuery = newsQuery.Where(n => n.AuthorId == authorId);
            }

            // Сортування
            newsQuery = sortOrder switch
            {
                "title" => newsQuery.OrderBy(n => n.Title),
                "year" => newsQuery.OrderBy(n => n.CreatedAt.Year),
                "chapters" => newsQuery.OrderByDescending(n => n.Comments.Count), // наприклад, кількість коментарів
                _ => newsQuery.OrderByDescending(n => n.CreatedAt), // за замовчуванням сортування за датою створення
            };

            // Передача списку новин у представлення
            var newsList = await newsQuery.ToListAsync();

            // Передача даних для фільтрів
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.Authors = await _context.Authors.ToListAsync();

            return View(newsList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }     
    }
}
