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

        public async Task<IActionResult> Index()
        {
            List<Manga> mangas = await _context.Manga.ToListAsync();
            return View(mangas); // Передаємо список манг у представлення
        }
    }
}
