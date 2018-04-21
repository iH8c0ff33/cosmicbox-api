using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CosmicBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmicBox.Controllers {
    [Route("api/[controller]"), Produces("application/json")]
    public class RunController : Controller {
        private readonly CosmicContext _context;

        public RunController(CosmicContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int boxId) {
            var box = await _context.Boxes.SingleOrDefaultAsync(b => b.Id == boxId);
            if (box == null) {
                return NotFound();
            }

            var runs = _context.Runs.Where(r => r.Box == box);
            return new ObjectResult(await runs.ToListAsync());
        }

        [HttpPost, Authorize]
        [ProducesResponseType(typeof(Run), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(IEnumerable<Run>), 409)]
        public async Task<IActionResult> Create([FromBody] Run run) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var sub = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var box = await _context.Boxes.Where(b => b.Id == run.BoxId).Include(b => b.Grants).SingleOrDefaultAsync();
            if (box == null) {
                return NotFound();
            }
            if (!box.Grants.Any(g => g.Sub == sub)) {
                return Forbid();
            }
            run.Box = box;

            var q =
                from r in _context.Runs
                where r.End > run.Start && r.Box == box
                select r;

            if (await q.CountAsync() > 0) {
                return new ConflictObjectResult(await q.ToListAsync());
            }

            _context.Runs.Add(run);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetRun", new { id = run.Id }, run);
        }

        [HttpGet("{id}", Name = "GetRun")]
        [ProducesResponseType(typeof(Run), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetById(int id) {
            var run = await _context.Runs.SingleOrDefaultAsync(r => r.Id == id);
            if (run == null) {
                return NotFound();
            }

            return new ObjectResult(run);
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(int id) {
            var run = await _context.Runs.Where(r => r.Id == id).Include(r => r.Box.Grants).SingleOrDefaultAsync();
            if (run == null) {
                return NotFound();
            }

            var sub = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!run.Box.Grants.Any(g => g.Sub == sub && (
                    run.End < DateTime.Now ?
                    true :
                    g.Type == Grant.Types.Owner))
                ) {
                return Forbid();
            }

            _context.Runs.Remove(run);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}