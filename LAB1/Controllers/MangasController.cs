using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAB1.Data;
using LAB1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using Microsoft.AspNetCore.Authorization;

namespace LAB1.Controllers
{
    public class MangasController : Controller
    {
        private readonly LAB1Context _context;
        private readonly UserManager<User> _userManager;

        public MangasController(LAB1Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Mangas
        public async Task<IActionResult> Index()
        {
              return _context.Manga != null ? 
                          View(await _context.Manga.Include(t=>t.Tags).Include(a=>a.Author).Include(i=>i.Illustrator).ToListAsync()) :
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
                .Include(m => m.Comments)            // Завантажуємо коментарі манги
                .ThenInclude(c => c.User)            // Завантажуємо користувачів, що залишили коментарі
                .FirstOrDefaultAsync(m => m.Id == id);  // Знаходимо мангу за ID

            if (manga == null)
            {
                return NotFound();
            }

            return View(manga);  // Передаємо мангу до виду
        }


        // GET: Mangas/Create
        public IActionResult Create()
        {
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name");
            ViewBag.Illustrators = new SelectList(_context.Authors, "Id", "Name");
            ViewBag.Tags = _context.Tags.ToList(); // Передаємо список тегів
            return View();
        }

        // POST: Mangas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseYear,Genres,Rating,Description,AuthorId,IllustratorId,Volumes,Chapters,CoverUrl,Status")] Manga manga, int[] selectedTags)
        {
            if (ModelState.IsValid)
            {
                if (selectedTags != null)
                {
                    foreach (var tagId in selectedTags)
                    {
                        var tag = await _context.Tags.FindAsync(tagId);
                        if (tag != null)
                        {
                            manga.Tags.Add(tag);
                        }
                    }
                }
                _context.Add(manga);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name", manga.AuthorId);
            ViewBag.Illustrators = new SelectList(_context.Authors, "Id", "Name", manga.IllustratorId);
            ViewBag.Tags = _context.Tags.ToList();
            return View(manga);
        }

        // GET: Mangas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Manga == null)
            {
                return NotFound();
            }

            var manga = await _context.Manga
                .Include(m => m.Tags) // Завантажуємо пов'язані теги
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manga == null)
            {
                return NotFound();
            }

            // Завантажуємо список авторів та ілюстраторів
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name", manga.AuthorId);
            ViewData["Illustrators"] = new SelectList(_context.Authors, "Id", "Name", manga.IllustratorId);

            // Завантажуємо список всіх тегів
            var allTags = await _context.Tags.ToListAsync();
            ViewBag.Tags = allTags;

            // Вибрані теги
            ViewBag.SelectedTags = manga.Tags.Select(t => t.Id).ToList();

            return View(manga);
        }


        // POST: Mangas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseYear,Genres,Rating,Description,AuthorId,IllustratorId,Volumes,Chapters,CoverUrl,Status")] Manga manga, int[] selectedTags)
        {
            if (id != manga.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Оновлюємо мангу
                    _context.Update(manga);

                    // Завантажуємо поточну мангу з тегами
                    var existingManga = await _context.Manga
                        .Include(m => m.Tags)
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (existingManga != null)
                    {
                        // Видаляємо старі теги
                        existingManga.Tags.Clear();

                        // Додаємо нові вибрані теги
                        foreach (var tagId in selectedTags)
                        {
                            var tag = await _context.Tags.FindAsync(tagId);
                            if (tag != null)
                            {
                                existingManga.Tags.Add(tag);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
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

            // Повторно завантажуємо списки у разі помилки
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name", manga.AuthorId);
            ViewData["Illustrators"] = new SelectList(_context.Authors, "Id", "Name", manga.IllustratorId);
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.SelectedTags = selectedTags;

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
            // Завантажуємо мангу за її id
            var manga = await _context.Manga
                .Include(a => a.Author)
                .Include(i => i.Illustrator)
                .Include(t => t.Tags)
                .Include(m => m.Ratings)
                .Include(m => m.Comments)
                .Include(m => m.Chapter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manga == null)
            {
                return NotFound();
            }

            // Обчислення середнього рейтингу
            double averageRating = manga.Ratings.Any() ? manga.Ratings.Average(r => r.UserRating) : 5;
            manga.AverageRating = averageRating;

            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Якщо користувач не автентифікований, передаємо лише мангу без інформації про закладки
                var model1 = new MangaDetailsViewModel
                {
                    Manga = manga,
                    IsBookmarked = false // Якщо користувач не автентифікований, закладка не існує
                };

                return View(model1); // Повертаємо ViewModel
            }

            // Перевірка на існування закладки для користувача
            var existingBookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.MangaId == manga.Id && b.UserId == user.Id);

            // Створюємо ViewModel і передаємо необхідні дані
            var model = new MangaDetailsViewModel
            {
                Manga = manga,
                IsBookmarked = existingBookmark != null // Перевіряємо, чи є закладка
            };

            return View(model); // Повертаємо ViewModel
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

        [HttpPost]
        public async Task<IActionResult> AddBookmark(int mangaId)
        {
            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);

            // Перевірка на існування манги
            var manga = await _context.Manga.FirstOrDefaultAsync(m => m.Id == mangaId);
            if (manga == null)
            {
                return NotFound();
            }

            // Перевірка, чи вже є закладка для цього користувача
            var existingBookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == user.Id && b.MangaId == mangaId);

            if (existingBookmark == null)
            {
                // Створюємо нову закладку
                var bookmark = new Bookmark
                {
                    UserId = user.Id,
                    MangaId = mangaId
                };

                _context.Bookmarks.Add(bookmark);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Profile", "User"); // Перенаправлення на сторінку користувача
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBookmark(int mangaId)
        {
            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Якщо користувач не авторизований
            }

            // Знаходимо закладку для видалення
            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.MangaId == mangaId && b.UserId == user.Id);

            if (bookmark != null)
            {
                // Видаляємо закладку
                _context.Bookmarks.Remove(bookmark);
                await _context.SaveChangesAsync();
            }

            // Перенаправляємо користувача назад на сторінку манги
            return RedirectToAction("MangaDetails", new { id = mangaId });
        }
        public async Task<IActionResult> Search(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return RedirectToAction("Index", "Catalog");
            }

            var searchResults = await _context.Manga
                .Where(m => m.Title.Contains(searchQuery))
                .ToListAsync();

            // Заповнюємо ViewBag.Tags для уникнення NullReferenceException
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.SearchQuery = searchQuery; // Зберігаємо пошуковий запит у ViewBag

            return View("/Views/Catalog/Index.cshtml", searchResults);
        }
        public IActionResult CreateChapter(int mangaId)
        {
            var model = new AddChapterViewModel
            {
                MangaId = mangaId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadChapter(int mangaId, int volumeNumber, int chapterNumber, IFormFileCollection folderPath)
        {
            // Отримуємо мангу з бази даних
            var manga = await _context.Manga.FirstOrDefaultAsync(m => m.Id == mangaId);
            if (manga == null)
            {
                return NotFound();
            }

            // Форматуємо назву манги для використання у шляху
            var mangaTitle = RemoveInvalidChars(manga.Title);

            // Створюємо новий розділ
            var chapter = new Chapter
            {
                MangaId = mangaId,
                VolumeNumber = volumeNumber,
                ChapterNumber = chapterNumber
            };

            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();

            // Створюємо папку для зображень розділу
            var chapterFolderPath = Path.Combine("wwwroot/images/chapters", mangaTitle, $"Vol_{volumeNumber}", $"Ch_{chapterNumber}");
            if (!Directory.Exists(chapterFolderPath))
            {
                Directory.CreateDirectory(chapterFolderPath);
            }

            // Завантажуємо зображення
            int pageNumber = 1;
            foreach (var file in folderPath.OrderBy(f => f.FileName))
            {
                var fileName = $"{pageNumber++}.png";
                var filePath = Path.Combine(chapterFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Додаємо сторінку до розділу
                var page = new Page
                {
                    ChapterId = chapter.Id,
                    ImagePath = $"/images/chapters/{mangaTitle}/Vol_{volumeNumber}/Ch_{chapterNumber}/{fileName}",
                    PageNumber = pageNumber - 1
                };

                _context.Pages.Add(page);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("MangaDetails", new { id = mangaId });
        }

        // Метод для видалення недійсних символів з назви
        private string RemoveInvalidChars(string input)
        {
            // Заміна пробілів на підкреслення
            string result = input.Replace(" ", "_");

            // Видалення недійсних символів
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                result = result.Replace(c.ToString(), "");
            }

            return result;
        }


        public async Task<IActionResult> ReadChapter(int chapterId)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Pages.OrderBy(p => p.PageNumber))
                .FirstOrDefaultAsync(c => c.Id == chapterId);

            if (chapter == null)
            {
                return NotFound();
            }

            return View(chapter);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChapter(int chapterId)
        {
            // Знаходимо розділ за ID
            var chapter = await _context.Chapters.FindAsync(chapterId);
            if (chapter == null)
            {
                return NotFound();
            }

            // Видаляємо розділ з бази даних
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();

            // Після видалення перенаправляємо на сторінку манги
            return RedirectToAction("MangaDetails", new { id = chapter.MangaId });
        }


    }
}
