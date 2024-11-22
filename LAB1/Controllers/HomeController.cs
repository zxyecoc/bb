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

        public async Task<IActionResult> Index(int? authorId, string sortOrder, List<int> selectedTags, double? minRating)
        {
            var newses = _context.News
                .Include(m => m.Author)
                .Include(m => m.Tags)
                .AsQueryable();

            // Фільтрація за автором
            if (authorId.HasValue)
            {
                newses = newses.Where(m => m.AuthorId == authorId.Value);
            }

            // Фільтрація за тегами
            if (selectedTags != null && selectedTags.Any())
            {
                newses = newses.Where(m => m.Tags.Any(t => selectedTags.Contains(t.Id)));
            }

            //// Фільтрація за рейтингом
            //if (minRating.HasValue)
            //{
            //    mangas = mangas.Where(m => m.AverageRating >= minRating.Value);
            //}

            // Сортування
            newses = sortOrder switch
            {
                "title" => newses.OrderBy(m => m.Title),
                "year" => newses.OrderByDescending(m => m.CreatedAt),
                //"rating" => mangas.OrderByDescending(m => m.AverageRating),
                _ => newses.OrderBy(m => m.Title),
            };

            // Передаємо списки тегів, авторів та ілюстраторів у ViewBag
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.Authors = await _context.Authors.ToListAsync();

            return View(await newses.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }     
    }
}
