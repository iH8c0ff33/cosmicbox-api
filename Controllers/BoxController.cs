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
    public class BoxController : Controller {
        private readonly CosmicContext _context;

        public BoxController(CosmicContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Box>> GetAll() => await _context.Boxes.ToListAsync();

        [HttpGet("{id}/runs")]
        public async Task<IActionResult> GetRuns(int id) {
            var box = await _context.Boxes.Include(b => b.Runs).SingleOrDefaultAsync(b => b.Id == id);
            if (box == null) {
                return NotFound();
            }

            return Ok(box.Runs);
        }

        [HttpPost, Authorize("addBoxes")]
        [ProducesResponseType(typeof(Box), 201)]
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> Create() {
            var box = new Box();

            var grant = Grant.Owner(User.GetIdentifier(), box);

            _context.Boxes.Add(box);
            _context.Grants.Add(grant);
            await _context.SaveChangesAsync();

            return new ObjectResult(box);
        }

        [HttpDelete("{id}"), Authorize]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> Delete(int id) {
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