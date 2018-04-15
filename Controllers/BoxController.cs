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
        public IActionResult Create([FromBody] Box box) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var grant = new Grant {
                Type = Grant.Types.Owner,
                Sub = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Box = box
            };

            _context.Boxes.Add(box);
            _context.Grants.Add(grant);
            try {
                _context.SaveChanges();
            } catch (DbUpdateException) {
                return BadRequest();
            }

            return CreatedAtRoute("GetBox", new { id = box.Id }, box);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult Delete(int id) {
            var box = _context.Boxes.FirstOrDefault(b => b.Id == id);
            if (box == null) {
                return NotFound();
            }

            if (!User.Claims.FirstOrDefault(c => c.Type == "scope").Value.Contains("delete:boxes")) {
                return Forbid();
            }

            _context.Boxes.Remove(box);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}