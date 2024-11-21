using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAB1.Data;
using LAB1.Models;
using Microsoft.AspNetCore.Identity;
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
    .Include(m => m.Comments) // Завантажуємо колекцію коментарів
    .Include(m => m.Comments.Select(c => c.UserName)) // Завантажуємо користувача для кожного коментаря
    .FirstOrDefaultAsync(m => m.Id == id);

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

        public async Task<IActionResult> MangaDetails(int id)
        {
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

            double averageRating = manga.Ratings.Any() ? manga.Ratings.Average(r => r.UserRating) : 5;
            manga.AverageRating = averageRating;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var model1 = new MangaDetailsViewModel
                {
                    Manga = manga,
                    IsBookmarked = false 
                };

                return View(model1); 
            }

            var existingBookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.MangaId == manga.Id && b.UserId == user.Id);

            var model = new MangaDetailsViewModel
            {
                Manga = manga,
                IsBookmarked = existingBookmark != null 
            };

            return View(model); 
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(int mangaId, string content)
        {
            // Перевірка на мінімальну та максимальну довжину
            if (string.IsNullOrWhiteSpace(content) || content.Length < 4 || content.Length > 2000)
            {
                TempData["Error"] = "Коментар має містити від 4 до 2000 символів.";
                return RedirectToAction("MangaDetails", "Mangas", new { id = mangaId });
            }

            // Перевірка на заборонені слова
            var bannedWords = new List<string> { "спам", "образа", "ненормативна лексика" };
            foreach (var word in bannedWords)
            {
                if (content.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    TempData["Error"] = "Ваш коментар містить заборонені слова. Будь ласка, виправте текст.";
                    return RedirectToAction("MangaDetails", "Mangas", new { id = mangaId });
                }
            }

            content = System.Net.WebUtility.HtmlEncode(content);

            var comment = new Comment
            {
                Content = content,
                UserName = User?.Identity?.Name,
                MangaId = mangaId,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Повертаємося на сторінку деталей манги після додавання коментаря
            return RedirectToAction("MangaDetails", "Mangas", new { id = mangaId });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteComment(int commentId, int mangaId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

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

            // Оновлюємо середній рейтинг у манги
            if (manga != null)
            {
                manga.AverageRating = averageRating;
                _context.Manga.Update(manga); // Оновлюємо мангу в базі даних
            }

            await _context.SaveChangesAsync(); // Зберігаємо зміни

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

        public async Task<IActionResult> ReadChapter(int chapterId, int page = 1)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Pages.OrderBy(p => p.PageNumber))
                .FirstOrDefaultAsync(c => c.Id == chapterId);

            if (chapter == null)
            {
                return NotFound();
            }

            chapter.CurrentPageNumber = page;

            return View(chapter);
        }

        [Authorize(Roles = "Translator, Administrator")]
        [HttpPost]
        public async Task<IActionResult> DeleteChapter(int chapterId)
        {
            // Знаходимо розділ за ID
            var chapter = await _context.Chapters
                .Include(c => c.Manga)
                .FirstOrDefaultAsync(c => c.Id == chapterId);

            if (chapter == null)
            {
                return NotFound();
            }

            // Форматуємо назву манги для використання у шляху
            var mangaTitle = RemoveInvalidChars(chapter.Manga.Title);
            var volumeNumber = chapter.VolumeNumber;
            var chapterNumber = chapter.ChapterNumber;

            // Визначаємо шлях до папки розділу
            var chapterFolderPath = Path.Combine("wwwroot/images/chapters", mangaTitle, $"Vol_{volumeNumber}", $"Ch_{chapterNumber}");

            // Перевіряємо, чи існує папка, і видаляємо її разом з усіма файлами
            if (Directory.Exists(chapterFolderPath))
            {
                try
                {
                    Directory.Delete(chapterFolderPath, true); // true - видалити папку з усіма файлами і підпапками
                }
                catch (Exception ex)
                {
                    // Логування помилки
                    Console.WriteLine($"Помилка при видаленні папки розділу: {ex.Message}");
                }
            }

            // Видаляємо розділ з бази даних
            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();

            // Перенаправляємо на сторінку манги після видалення
            return RedirectToAction("MangaDetails", new { id = chapter.MangaId });
        }

        public IActionResult NextChapter(int currentChapterId, int mangaId)
        {
            var currentChapter = _context.Chapters.FirstOrDefault(c => c.Id == currentChapterId);
            if (currentChapter == null) return NotFound();

            // Знаходимо наступний розділ
            var nextChapter = _context.Chapters
                .Where(c => c.MangaId == mangaId && c.ChapterNumber > currentChapter.ChapterNumber)
                .OrderBy(c => c.ChapterNumber)
                .FirstOrDefault();

            // Якщо наступного розділу немає, переходимо на сторінку манги
            if (nextChapter == null)
                return RedirectToAction("MangaDetails", "Mangas", new { id = mangaId });

            // Переходимо на наступний розділ
            return RedirectToAction("ReadChapter", new { chapterId = nextChapter.Id });
        }

        public IActionResult PreviousChapter(int currentChapterId, int mangaId)
        {
            var currentChapter = _context.Chapters.FirstOrDefault(c => c.Id == currentChapterId);
            if (currentChapter == null) return NotFound();

            // Знаходимо попередній розділ
            var previousChapter = _context.Chapters
                .Where(c => c.MangaId == mangaId && c.ChapterNumber < currentChapter.ChapterNumber)
                .OrderByDescending(c => c.ChapterNumber)
                .FirstOrDefault();

            // Якщо попереднього розділу немає, переходимо на сторінку манги
            if (previousChapter == null)
                return RedirectToAction("MangaDetails", "Mangas", new { id = mangaId });

            // Переходимо на попередній розділ
            return RedirectToAction("ReadChapter", new { chapterId = previousChapter.Id });
        }
    }
}
