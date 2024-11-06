using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAB1.Data;
using LAB1.Models;

namespace LAB1.Controllers
{
    public class MangasController : Controller
    {
        private readonly LAB1Context _context;

        public MangasController(LAB1Context context)
        {
            _context = context;
        }

        // GET: Mangas
        public async Task<IActionResult> Index()
        {
              return _context.Manga != null ? 
                          View(await _context.Manga.ToListAsync()) :
                          Problem("Entity set 'LAB1Context.Manga'  is null.");
        }

        // GET: Mangas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Manga == null)
            {
                return NotFound();
            }

            var manga = await _context.Manga
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manga == null)
            {
                return NotFound();
            }

            return View(manga);
        }

        // GET: Mangas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mangas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseYear,Genres,Rating,Description,Author,Illustrator,Volumes,Chapters,CoverUrl,Status")] Manga manga)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manga);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(manga);
        }

        // GET: Mangas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Manga == null)
            {
                return NotFound();
            }

            var manga = await _context.Manga.FindAsync(id);
            if (manga == null)
            {
                return NotFound();
            }
            return View(manga);
        }

        // POST: Mangas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseYear,Genres,Rating,Description,Author,Illustrator,Volumes,Chapters,CoverUrl,Status")] Manga manga)
        {
            if (id != manga.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manga);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MangaExists(manga.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(manga);
        }

        // GET: Mangas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Manga == null)
            {
                return NotFound();
            }

            var manga = await _context.Manga
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manga == null)
            {
                return NotFound();
            }

            return View(manga);
        }

        // POST: Mangas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Manga == null)
            {
                return Problem("Entity set 'LAB1Context.Manga'  is null.");
            }
            var manga = await _context.Manga.FindAsync(id);
            if (manga != null)
            {
                _context.Manga.Remove(manga);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MangaExists(int id)
        {
          return (_context.Manga?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // Метод MangaDetails для отримання конкретної манги по ID
        public async Task<IActionResult> MangaDetails(int id)
        {
            // Отримуємо мангу з бази даних за ID та завантажуємо пов'язані коментарі
            var manga = await _context.Manga
                .Include(m => m.Comments) // Завантаження коментарів
                .FirstOrDefaultAsync(m => m.Id == id);

            // Якщо манга не знайдена, повертаємо помилку 404
            if (manga == null)
            {
                return NotFound();
            }

            // Повертаємо представлення "MangaDetails" з моделлю манги
            return View("MangaDetails", manga);
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(int mangaId, string content)
        {
            var comment = new Comment
            {
                Content = content,
                User = User?.Identity?.Name ?? "Анонім",
                MangaId = mangaId,
                CreatedAt = DateTime.Now
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("MangaDetails", "Mangas", new { id = mangaId });
        }

    }
}
