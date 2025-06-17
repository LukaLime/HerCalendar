using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HerCalendar.Data;
using HerCalendar.Models;

namespace HerCalendar.Controllers
{
    public class MyCyclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyCyclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MyCycles
        public async Task<IActionResult> Index()
        {
            var cycles = await _context.CycleTracker.OrderByDescending(c => c.LastPeriodStartDate).ToListAsync();
            // Average cycle length calculation
            int averageCycleLength = (int)(cycles.Any() ? cycles.Average(c => c.CycleLength) : 0);

            // Pass the last entry to the view
            ViewData["AverageCycleLength"] = averageCycleLength;

            // Get the most recent entry (if it exists)
            var lastEntry = cycles.FirstOrDefault();
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

            return View(cycles);
        }

        // GET: MyCycles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var CycleTracker = await _context.CycleTracker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (CycleTracker == null)
            {
                return NotFound();
            }

            return View(CycleTracker);
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
            if (ModelState.IsValid)
            {
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

            var myCycle = await _context.CycleTracker.FindAsync(id);
            if (myCycle == null)
            {
                return NotFound();
            }
            return View(myCycle);
        }

        // POST: MyCycles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastPeriodStartDate,NextPeriodStartDate")] CycleTracker myCycle)
        {
            if (id != myCycle.Id)
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
                    myCycle.CycleLength = (myCycle.NextPeriodStartDate - myCycle.LastPeriodStartDate).Days + 1;

                    _context.Update(myCycle);
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

            var myCycle = await _context.CycleTracker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myCycle == null)
            {
                return NotFound();
            }

            return View(myCycle);
        }

        // POST: MyCycles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var myCycle = await _context.CycleTracker.FindAsync(id);
            if (myCycle != null)
            {
                _context.CycleTracker.Remove(myCycle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyCycleExists(int id)
        {
            return _context.CycleTracker.Any(e => e.Id == id);
        }
    }
}
