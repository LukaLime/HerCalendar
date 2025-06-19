using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HerCalendar.Data;
using HerCalendar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HerCalendar.Controllers
{
    // MyCyclesController is responsible for managing the user's menstrual cycle data.
    [Authorize]
    public class MyCyclesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyCyclesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MyCycles
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var existingCycle = await _context.CycleTracker
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.LastPeriodStartDate)
                .ToListAsync();

            // Average cycle length calculation
            int averageCycleLength = (int)(existingCycle.Any() ? existingCycle.Average(c => c.CycleLength) : 0);

            // Pass the last entry to the view
            ViewData["AverageCycleLength"] = averageCycleLength;

            // Get the most recent entry (if it exists)
            var lastEntry = existingCycle.FirstOrDefault();
            DateTime? estimatedNextPeriod = null;
            int? daysUntilNext = null;


            if (lastEntry != null && averageCycleLength > 0)
            {
                // Start from the last known period date
                DateTime nextPeriod = lastEntry.NextPeriodStartDate;

                // Keep adding the cycle length until it's in the future
                while (nextPeriod <= DateTime.Today)
                {
                    nextPeriod = nextPeriod.AddDays(averageCycleLength - 1);
                }
                estimatedNextPeriod = nextPeriod;
                daysUntilNext = (estimatedNextPeriod.Value - DateTime.Today).Days;
            }

            // Use ViewBag or a view model (simpler to start with ViewBag)
            ViewBag.EstimatedNextPeriod = estimatedNextPeriod;
            ViewBag.DaysUntilNextPeriod = daysUntilNext;

            return View(existingCycle);
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
