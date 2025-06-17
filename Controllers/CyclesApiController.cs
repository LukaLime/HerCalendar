using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HerCalendar.Data;
using HerCalendar.Models;
using System.Threading.Tasks;

namespace HerCalendar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CyclesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CyclesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/cycles
        [HttpGet]
        public async Task<IActionResult> GetCycles()
        {
            var cycles = await _context.CycleTracker.ToListAsync();
            return Ok(cycles);
        }

        // GET: api/cycles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCycleById(int id)
        {
            var cycle = await _context.CycleTracker.FindAsync(id);

            if (cycle == null)
            {
                return NotFound();
            }

            return Ok(cycle);
        }


        // POST: api/cycles
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateCycle([FromBody] CycleTracker cycle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (cycle.NextPeriodStartDate <= cycle.LastPeriodStartDate)
            {
                return BadRequest("Next period start date must be after last period start date.");
            }

            cycle.CycleLength = (cycle.NextPeriodStartDate - cycle.LastPeriodStartDate).Days;



            _context.CycleTracker.Add(cycle);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCycleById), new { id = cycle.Id }, cycle);
        }

        // PUT: api/cycles/{id}
        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCycle(int id, [FromBody] CycleTracker updatedCycle)
        {
            if (id != updatedCycle.Id)
            {
                return BadRequest("Cycle ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (updatedCycle.NextPeriodStartDate <= updatedCycle.LastPeriodStartDate)
            {
                return BadRequest("Next period start date must be after last period start date.");
            }

            var existingCycle = await _context.CycleTracker.FindAsync(id);
            if (existingCycle == null)
            {
                return NotFound();
            }

            // Update fields
            existingCycle.LastPeriodStartDate = updatedCycle.LastPeriodStartDate;
            existingCycle.NextPeriodStartDate = updatedCycle.NextPeriodStartDate;
            existingCycle.CycleLength = (updatedCycle.NextPeriodStartDate - updatedCycle.LastPeriodStartDate).Days;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "A concurrency error occurred while updating the cycle.");
            }

            return NoContent();
        }


        // DELETE: api/cycles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCycle(int id)
        {
            var cycle = await _context.CycleTracker.FindAsync(id);
            if (cycle == null)
            {
                return NotFound();
            }

            _context.CycleTracker.Remove(cycle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

