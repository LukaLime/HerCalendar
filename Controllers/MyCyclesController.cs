using HerCalendar.Data;
using HerCalendar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HerCalendar.Controllers
{
    // MyCyclesController is responsible for managing the user's menstrual cycle data.
    [Authorize]
    public class MyCyclesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<MyCyclesController> _logger;

        public MyCyclesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<MyCyclesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        private async Task<T?> RetryDbCallAsync<T>(Func<Task<T>> dbOperation, int retries = 3, int delayMs = 1000)
        {
            for (int attempt = 1; attempt <= retries; attempt++)
            {
                try
                {
                    _logger.LogInformation("DB call attempt {Attempt} of {Retries}", attempt, retries);
                    return await dbOperation();
                }
                catch (SqlException ex) when (attempt < retries)
                {
                    _logger.LogWarning(ex, "DB call failed on attempt {Attempt} with SQL error {ErrorNumber}. Retrying...", attempt, ex.Number);
                    await Task.Delay(delayMs);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error during DB call on attempt {Attempt}", attempt);
                    throw; // Rethrow unexpected exceptions
                }
            }
            _logger.LogError("DB call failed after {Retries} attempts", retries);

            // If all retries fail, return default value (null for reference types, 0 for int, etc.)
            return default;
        }


        private async Task<(List<CycleTracker> Cycles, int Avg, DateTime? EstDate, int? DaysUntil)> GetCycleDataAsync()
        {
            var userId = _userManager.GetUserId(User);

            var existingCycle = await _context.CycleTracker
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.LastPeriodStartDate)
                .ToListAsync();

            int averageCycleLength = (int)(existingCycle.Any() ? existingCycle.Average(c => c.CycleLength) : 0);
            var lastEntry = existingCycle.FirstOrDefault();

            DateTime? estimatedNextPeriod = null;
            int? daysUntilNext = null;

            if (lastEntry != null && averageCycleLength > 0)
            {
                DateTime nextPeriod = lastEntry.NextPeriodStartDate;
                while (nextPeriod <= DateTime.Today)
                {
                    nextPeriod = nextPeriod.AddDays(averageCycleLength - 1);
                }

                estimatedNextPeriod = nextPeriod;
                daysUntilNext = (estimatedNextPeriod.Value - DateTime.Today).Days;
            }

            return (existingCycle, averageCycleLength, estimatedNextPeriod, daysUntilNext);
        }


        // GET: MyCycles
        public async Task<IActionResult> Index()
        {
            //await Task.Delay(10000); // for testing only
            var (cycles, avg, estDate, daysUntil) = await GetCycleDataAsync();

            ViewData["AverageCycleLength"] = avg;
            ViewBag.EstimatedNextPeriod = estDate;
            ViewBag.DaysUntilNextPeriod = daysUntil;

            return View(cycles);
        }

        [HttpGet]
        public async Task<IActionResult> IndexPartial()
        {
            _logger.LogInformation("IndexPartial called");
            //await Task.Delay(10000); // simulate delay
            var result = await RetryDbCallAsync(async () =>
            {
                var (cycles, avg, estDate, daysUntil) = await GetCycleDataAsync();

                ViewData["AverageCycleLength"] = avg;
                ViewBag.EstimatedNextPeriod = estDate;
                ViewBag.DaysUntilNextPeriod = daysUntil;

                _logger.LogInformation("DB call successful");
                return PartialView("Index", cycles);
            });

            if (result == null)
            {
                _logger.LogWarning("Returning 503 due to repeated DB failures");
                return StatusCode(503, "Database unavailable after multiple attempts.");
            }

            return result;
            //return StatusCode(503); // For testing purposes, return a 503 Service Unavailable status code
        }


        // GET: MyCycles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var existingCycle = await _context.CycleTracker
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (existingCycle == null)
            {
                return NotFound();
            }

            return View(existingCycle);
        }

        // GET: MyCycles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MyCycles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastPeriodStartDate,NextPeriodStartDate")] CycleTracker myCycle)
        {
            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                myCycle.UserId = userId;

                // Calculate the cycle length (inclusive of the start date)
                myCycle.CycleLength = (myCycle.NextPeriodStartDate - myCycle.LastPeriodStartDate).Days + 1;

                _context.Add(myCycle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(myCycle);
        }

        // GET: MyCycles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var existingCycle = await _context.CycleTracker.FindAsync(id);
            if (existingCycle == null)
            {
                return NotFound();
            }
            return View(existingCycle);
        }

        // POST: MyCycles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastPeriodStartDate,NextPeriodStartDate")] CycleTracker myCycle)
        {
            var userId = _userManager.GetUserId(User);

            var existingCycle = await _context.CycleTracker
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (existingCycle == null)
            {
                return NotFound();
            }

            if (myCycle.NextPeriodStartDate <= myCycle.LastPeriodStartDate)
            {
                ModelState.AddModelError(string.Empty, "Next period date must be after last period start date.");
                return View(myCycle);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Recalculate the NextPeriodStartDate to match the logic in Create
                    // Apply updates to the existing tracked entity
                    existingCycle.LastPeriodStartDate = myCycle.LastPeriodStartDate;
                    existingCycle.NextPeriodStartDate = myCycle.NextPeriodStartDate;
                    existingCycle.CycleLength = (myCycle.NextPeriodStartDate - myCycle.LastPeriodStartDate).Days + 1;

                    //_context.Update(myCycle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyCycleExists(myCycle.Id))
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
            return View(myCycle);
        }

        // avoid exceptions or weird behavior if the record was deleted or modified by someone else between the time
        // the form was loaded and the time the user submitted changes.
        private bool CycleExists(int id)
        {
            return _context.CycleTracker.Any(e => e.Id == id);
        }


        // GET: MyCycles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var existingCycle = await _context.CycleTracker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (existingCycle == null)
            {
                return NotFound();
            }

            return View(existingCycle);
        }

        // POST: MyCycles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);

            var existingCycle = await _context.CycleTracker
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (existingCycle == null)
            {
                return NotFound(); // Either not found or not owned by current user
            }

            _context.CycleTracker.Remove(existingCycle);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyCycleExists(int id)
        {
            return _context.CycleTracker.Any(e => e.Id == id);
        }
    }
}
