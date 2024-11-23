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
    public class TeamsController : Controller
    {
        private readonly NewsBlogContext _context;

        public TeamsController(NewsBlogContext context)
        {
            _context = context;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var teamsWithLeagues = await _context.Teams
                .Include(t => t.LeagueTeams)
                .ThenInclude(lt => lt.League)
                .ToListAsync();

            return View(teamsWithLeagues);
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.LeagueTeams)
                .ThenInclude(lt => lt.League)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }


        // GET: Teams/Create
        public IActionResult Create()
        {
            // Завантажуємо всі ліги
            ViewBag.Leagues = _context.Leagues.ToList();
            return View();
        }


        // POST: Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Team team, int[] selectedLeagues)
        {
            // Перевіряємо, чи модель валідна
            if (ModelState.IsValid)
            {
                // Перевірка: Чи обрано хоча б одну лігу
                if (selectedLeagues == null || selectedLeagues.Length == 0)
                {
                    ModelState.AddModelError("", "Please select at least one league.");
                    // Знову передаємо ліги до ViewBag, щоб вони відобразилися на сторінці
                    ViewBag.Leagues = _context.Leagues.ToList();
                    return View(team);
                }

                // Додаємо команду до бази даних
                _context.Add(team);
                await _context.SaveChangesAsync();

                // Додаємо зв'язки між командою та обраними лігами
                foreach (var leagueId in selectedLeagues)
                {
                    _context.LeagueTeams.Add(new LeagueTeam
                    {
                        TeamId = team.Id,
                        LeagueId = leagueId
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Якщо модель не валідна, знову завантажуємо ліги
            ViewBag.Leagues = _context.Leagues.ToList();
            return View(team);
        }



        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.LeagueTeams)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
            {
                return NotFound();
            }

            ViewBag.Leagues = _context.Leagues.ToList();
            ViewBag.SelectedLeagues = team.LeagueTeams.Select(lt => lt.LeagueId).ToList();
            return View(team);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Team team, int[] selectedLeagues)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();

                    // Оновлення зв'язків
                    var existingLeagueTeams = _context.LeagueTeams.Where(lt => lt.TeamId == team.Id);
                    _context.LeagueTeams.RemoveRange(existingLeagueTeams);

                    foreach (var leagueId in selectedLeagues)
                    {
                        _context.LeagueTeams.Add(new LeagueTeam
                        {
                            TeamId = team.Id,
                            LeagueId = leagueId
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
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

            ViewBag.Leagues = _context.Leagues.ToList();
            return View(team);
        }


        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.LeagueTeams)
                .ThenInclude(lt => lt.League)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }


        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}
