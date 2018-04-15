using System.Collections.Generic;
using System.Linq;
using CosmicBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CosmicBox.Controllers {
    [Route("api/[controller]"), Produces("application/json")]
    public class RunController : Controller {
        private readonly CosmicContext _context;

        public RunController(CosmicContext context) => _context = context;

        [HttpGet]
        public IEnumerable<Run> GetAll() => _context.Runs.ToList();

        [HttpPost, Authorize]
        [ProducesResponseType(typeof(Run), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(IEnumerable<Run>), 409)]
        public IActionResult Create([FromBody] Run run) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var box = _context.Boxes.FirstOrDefault(b => b.Uuid == run.BoxUuid);
            if (box == null) {
                return NotFound();
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

        [HttpGet("{id}", Name="GetRun")]
        [ProducesResponseType(typeof(Run), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetById(int id) {
            var run = _context.Boxes.FirstOrDefault(r => r.Id == id);
            if (run == null) {
                return NotFound();
            }

            return new ObjectResult(run);
        }
    }
}