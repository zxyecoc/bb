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
                          View(await _context.Manga.Include(a=>a.Author).ToListAsync()) :
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
         .Include(m => m.Comments)
         .ThenInclude(c => c.User) // Щоб відображати ім'я користувача
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
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name");
            return View();
        }

        // POST: Mangas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseYear,Genres,Rating,Description,AuthorId,Illustrator,Volumes,Chapters,CoverUrl,Status")] Manga manga)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manga);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name", manga.AuthorId);
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
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name", manga.AuthorId);

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
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name", manga.AuthorId);

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
            var manga = await _context.Manga
                .Include(m => m.Ratings)
                .Include(m => m.Comments) // Завантажуємо коментарі разом із мангою
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manga == null)
            {
                return NotFound();
            }

            // Обчислення середнього рейтингу
            double averageRating = manga.Ratings.Any() ? manga.Ratings.Average(r => r.UserRating) : 5;

            // Передаємо середній рейтинг до View, додаємо в модель
            manga.AverageRating = averageRating;

            return View(manga);
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

            // Повертаємося на сторінку деталей манги після додавання коментаря
            return RedirectToAction("MangaDetails", "Mangas", new { id = mangaId });
        }


        [HttpPost]
        public async Task<IActionResult> AddRating(int mangaId, int ratingValue)
        {
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(); // Якщо користувач не авторизований
            }

            // Перевірка, чи вже існує рейтинг для цієї манги від цього користувача
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.MangaId == mangaId && r.UserName == userName);

            if (existingRating != null)
            {
                // Оновлюємо рейтинг, якщо він уже є
                existingRating.UserRating = ratingValue;
                existingRating.CreatedAt = DateTime.Now;
                _context.Ratings.Update(existingRating); // Оновлюємо рейтинг у базі
            }
            else
            {
                // Якщо рейтингу ще немає, додаємо новий
                var newRating = new Rating
                {
                    MangaId = mangaId,
                    UserName = userName,
                    UserRating = ratingValue,
                    CreatedAt = DateTime.Now
                };
                _context.Ratings.Add(newRating); // Додаємо новий рейтинг
            }

            await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних

            // Обчислення середнього рейтингу
            var manga = await _context.Manga
                .Include(m => m.Ratings) // Завантажуємо всі рейтинги цієї манги
                .FirstOrDefaultAsync(m => m.Id == mangaId);

            double averageRating = 5; // За замовчуванням середній рейтинг = 5

            if (manga != null && manga.Ratings.Any())
            {
                averageRating = manga.Ratings.Average(r => r.UserRating); // Обчислення середнього рейтингу
            }

            ViewData["AverageRating"] = averageRating; // Зберігаємо середній рейтинг в ViewData

            return RedirectToAction("MangaDetails", new { id = mangaId }); // Перенаправляємо на сторінку з деталями манги
        }







        //        private async Task UpdateMangaAverageRating(int mangaId)
        //        {
        //             var ratings = await _context.Ratings
        //            .Where(r => r.MangaId == mangaId)
        //            .Select(r => r.UserRating)
        //            .ToListAsync();

        //             if (ratings.Count > 0)
        //                {
        //                    var averageRating = ratings.Average();
        //                    var manga = await _context.Manga.FindAsync(mangaId);
        //                     if (manga != null)
        //                     {
        //                        manga.Ratings = averageRating;
        //                        await _context.SaveChangesAsync();
        //                     }
        //                }
        //}



    }
}
