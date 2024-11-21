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
        private readonly LAB1Context _context;
        private readonly IStringLocalizer<HomeController> _localizer;

        // Об'єднаний конструктор
        public HomeController(ILogger<HomeController> logger, LAB1Context context, IStringLocalizer<HomeController> localizer)
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

        public async Task<IActionResult> Index()
        {

            // Отримуємо 5 манг з останніми розділами
            var recentMangas = await _context.Chapters
                .Include(c => c.Manga) // Включаємо мангу
                .OrderByDescending(c => c.UpdatedAt) // Сортуємо по часу оновлення
                .GroupBy(c => c.MangaId) // Групуємо по MangaId, щоб отримати останні манги
                .Take(5) // Лімітуємо до 5 манг
                .Select(g => g.OrderByDescending(c => c.UpdatedAt).FirstOrDefault()) // Для кожної манги беремо останній розділ
                .ToListAsync();

            // Отримуємо 5 манг з найбільшими середніми рейтингами
            var topRatedMangas = await _context.Manga
                .Where(m => m.AverageRating.HasValue) // Перевіряємо, що рейтинг існує
                .OrderByDescending(m => m.AverageRating) // Сортуємо за рейтингом від найбільшого
                .Take(5) // Лімітуємо до 5 манг
                .ToListAsync();

            // Якщо немає манг з новими розділами, створюємо порожній список
            if (recentMangas == null || !recentMangas.Any())
            {
                recentMangas = new List<Chapter>();
            }

            // Якщо немає манг з рейтингами, створюємо порожній список
            if (topRatedMangas == null || !topRatedMangas.Any())
            {
                topRatedMangas = new List<Manga>();
            }

            ViewData["RecentChapters"] = recentMangas;
            ViewData["TopRatedMangas"] = topRatedMangas;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }     
    }
}
