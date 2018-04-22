using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CosmicBox.Auth;
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
        public IEnumerable<Run> GetAll() => _context.Runs;

        [HttpPost, Authorize]
        [ProducesResponseType(typeof(Run), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(IEnumerable<Run>), 409)]
        public async Task<IActionResult> Create([FromBody] Run run) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var box = await _context.Boxes.Include(b => b.Grants).SingleOrDefaultAsync(b => b.Id == run.BoxId);
            if (box == null) {
                return NotFound();
            }
            if (!box.HasWriteAccess(User.GetIdentifier())) {
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

            return Ok(run);
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(int id) {
            var run = await _context.Runs.Include(r => r.Box.Grants).SingleOrDefaultAsync(r => r.Id == id);
            if (run == null) {
                return NotFound();
            }

            var sub = User.GetIdentifier();
            if (DateTime.Now > run.Start ? run.Box.IsOwner(sub) : run.Box.HasWriteAccess(sub)) {
                return Forbid();
            }

            _context.Runs.Remove(run);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}