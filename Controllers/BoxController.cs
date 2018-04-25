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
    [Route("api/[controller]")]
    [ApiController]
    public class BoxController : ControllerBase {
        private readonly CosmicContext _context;

        public BoxController(CosmicContext context) => _context = context;

        [HttpGet]
        [ProducesResponseType(typeof(List<Box>), 200)]
        public async Task<ActionResult<List<Box>>> GetAll() => await _context.Boxes.ToListAsync();

        [HttpGet("{id}/runs")]
        [ProducesResponseType(typeof(List<Run>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<Run>>> GetRuns(int id) {
            var box = await _context.Boxes.Include(b => b.Runs).SingleOrDefaultAsync(b => b.Id == id);
            if (box == null) {
                return NotFound();
            }

            return box.Runs;
        }

        [HttpPost, Authorize("addBoxes")]
        [ProducesResponseType(typeof(Box), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Box>> Create() {
            var box = new Box();

            var grant = Grant.Owner(User.GetIdentifier(), box);

            _context.Boxes.Add(box);
            _context.Grants.Add(grant);
            await _context.SaveChangesAsync();

            return box;
        }

        [HttpDelete("{id}"), Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(int id) {
            var box = await _context.Boxes.Include(b => b.Grants).SingleOrDefaultAsync(b => b.Id == id);
            if (box == null) {
                return NotFound();
            }

            if (!box.HasWriteAccess(User.GetIdentifier()) && !User.CanDeleteBoxes()) {
                return Forbid();
            }

            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}