using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LAB1.Data;
using LAB1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LAB1.Controllers
{
    public class CatalogController : Controller
    {
        private readonly LAB1Context _context;

        public CatalogController(LAB1Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? authorId, int? illustratorId, string sortOrder, List<int> selectedTags, double? minRating)
        {
            var mangas = _context.Manga
                .Include(m => m.Author)
                .Include(m => m.Illustrator)
                .Include(m => m.Tags)
                .AsQueryable();

            // Фільтрація за автором
            if (authorId.HasValue)
            {
                mangas = mangas.Where(m => m.AuthorId == authorId.Value);
            }

            // Фільтрація за ілюстратором
            if (illustratorId.HasValue)
            {
                mangas = mangas.Where(m => m.IllustratorId == illustratorId.Value);
            }

            // Фільтрація за тегами
            if (selectedTags != null && selectedTags.Any())
            {
                mangas = mangas.Where(m => m.Tags.Any(t => selectedTags.Contains(t.Id)));
            }

            // Фільтрація за рейтингом
            if (minRating.HasValue)
            {
                mangas = mangas.Where(m => m.AverageRating >= minRating.Value);
            }

            // Сортування
            mangas = sortOrder switch
            {
                "title" => mangas.OrderBy(m => m.Title),
                "year" => mangas.OrderByDescending(m => m.ReleaseYear),
                "rating" => mangas.OrderByDescending(m => m.AverageRating),
                "chapters" => mangas.OrderByDescending(m => m.Chapters),
                _ => mangas.OrderBy(m => m.Title),
            };

            // Передаємо списки тегів, авторів та ілюстраторів у ViewBag
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.Authors = await _context.Authors.ToListAsync();

            return View(await mangas.ToListAsync());
        }
    }
}
