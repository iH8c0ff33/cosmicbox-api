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
    [ApiController]
    public class RunController : ControllerBase {
        private readonly CosmicContext _context;

        public RunController(CosmicContext context) => _context = context;

        [HttpGet]
        [ProducesResponseType(typeof(List<Run>), 200)]
        public async Task<ActionResult<List<Run>>> GetAll() => await _context.Runs.ToListAsync();

        [HttpPost, Authorize]
        [ProducesResponseType(typeof(Run), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(List<Run>), 409)]
        public async Task<ActionResult<Run>> Create(Run run) {
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
        public async Task<ActionResult<Run>> GetById(int id) {
            var run = await _context.Runs.SingleOrDefaultAsync(r => r.Id == id);
            if (run == null) {
                return NotFound();
            }

            return run;
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult> Delete(int id) {
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