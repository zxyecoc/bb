using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAB1.Data;
using LAB1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace LAB1.Controllers
{
    public class NewsesController : Controller
    {
        private readonly NewsBlogContext _context;
        private readonly UserManager<User> _userManager;

        public NewsesController(NewsBlogContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Newses
        public async Task<IActionResult> Index()
        {
              return _context.News != null ? 
                          View(await _context.News.Include(t=>t.Tags).Include(a=>a.Author).ToListAsync()) :
                          Problem("Entity set 'NewsBlog.News'  is null.");
        }

        // GET: Newses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
    .Include(m => m.Comments) // Завантажуємо колекцію коментарів
    .Include(m => m.Comments.Select(c => c.UserName)) // Завантажуємо користувача для кожного коментаря
    .FirstOrDefaultAsync(m => m.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            return View(news);  // Передаємо мангу до виду
        }


        // GET: Newses/Create
        public IActionResult Create()
        {
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name");
            ViewBag.Illustrators = new SelectList(_context.Authors, "Id", "Name");
            ViewBag.Tags = _context.Tags.ToList(); // Передаємо список тегів
            return View();
        }

        // POST: Newses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Rating,NewsText,AuthorId,CoverUrl")] News news, int[] selectedTags)
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
                            news.Tags.Add(tag);
                        }
                    }
                }
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name", news.AuthorId);
            ViewBag.Tags = _context.Tags.ToList();
            return View(news);
        }

        // GET: Newses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(m => m.Tags) // Завантажуємо пов'язані теги
                .FirstOrDefaultAsync(m => m.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            // Завантажуємо список авторів та ілюстраторів
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name", news.AuthorId);
            // Завантажуємо список всіх тегів
            var allTags = await _context.Tags.ToListAsync();
            ViewBag.Tags = allTags;

            // Вибрані теги
            ViewBag.SelectedTags = news.Tags.Select(t => t.Id).ToList();

            return View(news);
        }


        // POST: Newses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Rating,NewsText,AuthorId,CoverUrl")] News news, int[] selectedTags)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Оновлюємо мангу
                    _context.Update(news);

                    // Завантажуємо поточну мангу з тегами
                    var existingNews = await _context.News
                        .Include(m => m.Tags)
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (existingNews != null)
                    {
                        // Видаляємо старі теги
                        existingNews.Tags.Clear();

                        // Додаємо нові вибрані теги
                        foreach (var tagId in selectedTags)
                        {
                            var tag = await _context.Tags.FindAsync(tagId);
                            if (tag != null)
                            {
                                existingNews.Tags.Add(tag);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
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
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name", news.AuthorId);
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.SelectedTags = selectedTags;

            return View(news);
        }


        // GET: Newses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: Newses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'NewsBlogContext.News'  is null.");
            }
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
          return (_context.News?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> NewsPage(int id)
        {
            var news = await _context.News
                .Include(a => a.Author)
                .Include(t => t.Tags)
                .Include(m => m.Ratings)
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            //double averageRating = manga.Ratings.Any() ? manga.Ratings.Average(r => r.UserRating) : 5;
            //manga.AverageRating = averageRating;

            return View(); 
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(int newsId, string content)
        {
            // Перевірка на мінімальну та максимальну довжину
            if (string.IsNullOrWhiteSpace(content) || content.Length < 4 || content.Length > 2000)
            {
                TempData["Error"] = "Коментар має містити від 4 до 2000 символів.";
                return RedirectToAction("Newspage", "Newses", new { id = newsId });
            }

            // Перевірка на заборонені слова
            var bannedWords = new List<string> { "спам", "образа", "ненормативна лексика" };
            foreach (var word in bannedWords)
            {
                if (content.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    TempData["Error"] = "Ваш коментар містить заборонені слова. Будь ласка, виправте текст.";
                    return RedirectToAction("NewsPage", "Newses", new { id = newsId });
                }
            }

            content = System.Net.WebUtility.HtmlEncode(content);

            var comment = new Comment
            {
                Content = content,
                UserName = User?.Identity?.Name,
                NewsId = newsId,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Повертаємося на сторінку деталей манги після додавання коментаря
            return RedirectToAction("NewsPage", "Newses", new { id = newsId });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteComment(int commentId, int newsId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("NewsPage", "Newses", new { id = newsId });
        }


        [HttpPost]
        public async Task<IActionResult> AddRating(int newsId, int ratingValue)
        {
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(); // Якщо користувач не авторизований
            }

            // Перевірка, чи вже існує рейтинг для цієї манги від цього користувача
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.NewsId == newsId && r.UserName == userName);

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
                    NewsId = newsId,
                    UserName = userName,
                    UserRating = ratingValue,
                    CreatedAt = DateTime.Now
                };
                _context.Ratings.Add(newRating); // Додаємо новий рейтинг
            }

            await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних

            // Обчислення середнього рейтингу
            var news = await _context.News
                .Include(m => m.Ratings) // Завантажуємо всі рейтинги цієї манги
                .FirstOrDefaultAsync(m => m.Id == newsId);

            double averageRating = 5; // За замовчуванням середній рейтинг = 5

            if (news != null && news.Ratings.Any())
            {
                averageRating = news.Ratings.Average(r => r.UserRating); // Обчислення середнього рейтингу
            }

            //// Оновлюємо середній рейтинг у манги
            //if (manga != null)
            //{
            //    manga.AverageRating = averageRating;
            //    _context.Manga.Update(manga); // Оновлюємо мангу в базі даних
            //}

            await _context.SaveChangesAsync(); // Зберігаємо зміни

            return RedirectToAction("NewsPage", new { id = newsId }); // Перенаправляємо на сторінку з деталями манги
        }

        [HttpPost]
        public async Task<IActionResult> AddBookmark(int newsId)
        {
            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);

            // Перевірка на існування манги
            var news = await _context.News.FirstOrDefaultAsync(m => m.Id == newsId);
            if (news == null)
            {
                return NotFound();
            }

            // Перевірка, чи вже є закладка для цього користувача
            var existingBookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == user.Id && b.NewsId == newsId);

            if (existingBookmark == null)
            {
                // Створюємо нову закладку
                var bookmark = new Bookmark
                {
                    UserId = user.Id,
                    NewsId = newsId
                };

                _context.Bookmarks.Add(bookmark);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Profile", "User"); // Перенаправлення на сторінку користувача
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBookmark(int newsId)
        {
            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Якщо користувач не авторизований
            }

            // Знаходимо закладку для видалення
            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.NewsId == newsId && b.UserId == user.Id);

            if (bookmark != null)
            {
                // Видаляємо закладку
                _context.Bookmarks.Remove(bookmark);
                await _context.SaveChangesAsync();
            }

            // Перенаправляємо користувача назад на сторінку манги
            return RedirectToAction("Newspage", new { id = newsId });
        }
        public async Task<IActionResult> Search(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return RedirectToAction("Index", "Home");
            }

            var searchResults = await _context.News
                .Where(m => m.Title.Contains(searchQuery))
                .ToListAsync();

            // Заповнюємо ViewBag.Tags для уникнення NullReferenceException
            ViewBag.Tags = await _context.Tags.ToListAsync();
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.SearchQuery = searchQuery; // Зберігаємо пошуковий запит у ViewBag

            return View("/Views/Home/Index.cshtml", searchResults);
        }
    }
}
