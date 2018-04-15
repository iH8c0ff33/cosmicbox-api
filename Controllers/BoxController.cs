using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public IEnumerable<Box> GetAll() => _context.Boxes.ToList();

        [HttpPost, Authorize("addBoxes")]
        [ProducesResponseType(typeof(Box), 201)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Create() {
            var box = new Box();

            var grant = new Grant {
                Type = Grant.Types.Owner,
                Sub = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Box = box
            };

            _context.Boxes.Add(box);
            _context.Grants.Add(grant);
            _context.SaveChanges();

            return new ObjectResult(box);
        }

        [HttpDelete("{id}"), Authorize]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Delete(int id) {
            var box = _context.Boxes.Where(b => b.Id == id).Include(b => b.Grants).FirstOrDefault();
            if (box == null) {
                return NotFound();
            }

            var sub = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (
                !box.Grants.Any(g => g.Type == Grant.Types.Owner && g.Sub == sub) &&
                !User.Claims.FirstOrDefault(c => c.Type == "scope").Value.Contains("delete:boxes")
            ) {
                return Forbid();
            }

            _context.Boxes.Remove(box);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}