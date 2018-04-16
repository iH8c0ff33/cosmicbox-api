using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public IActionResult GetAll([FromQuery] int boxId) {
            var box = _context.Boxes.FirstOrDefault(b => b.Id == boxId);
            if (box == null) {
                return NotFound();
            }

            var runs = _context.Runs.Where(r => r.Box == box);
            return new ObjectResult(runs);
        }

        [HttpPost, Authorize]
        [ProducesResponseType(typeof(Run), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(IEnumerable<Run>), 409)]
        public IActionResult Create([FromBody] Run run) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var sub = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var box = _context.Boxes.Where(b => b.Id == run.BoxId).Include(b => b.Grants).FirstOrDefault();
            if (box == null) {
                return NotFound();
            }
            if (!box.Grants.Any(g => g.Sub == sub)) {
                return Forbid();
            }
            run.Box = box;

            var q =
                from r in _context.Runs
                where r.End >= run.Start && r.Box == box
                select r;

            if (q.Count() > 0) {
                return new ConflictObjectResult(q.ToList());
            }

            _context.Runs.Add(run);
            _context.SaveChanges();

            return CreatedAtRoute("GetRun", new { id = run.Id }, run);
        }

        [HttpGet("{id}", Name = "GetRun")]
        [ProducesResponseType(typeof(Run), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetById(int id) {
            var run = _context.Runs.FirstOrDefault(r => r.Id == id);
            if (run == null) {
                return NotFound();
            }

            return new ObjectResult(run);
        }

        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(int id) {
            var run = _context.Runs.Where(r => r.Id == id).Include(r => r.Box.Grants).FirstOrDefault();
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
            _context.SaveChanges();
            return NoContent();
        }
    }
}