using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CosmicBox.Models;
using System;
using System.Linq;

namespace CosmicBox.Controllers {
    [Route("api/[controller]")]
    public class EventController : Controller {
        private readonly ApiContext _context;

        public EventController(ApiContext context) {
            _context = context;

            if (_context.Events.Count() == 0) {
                _context.Events.Add(new Event {
                    Timestamp = DateTime.Now,
                    Pressure = 90.3
                });

                _context.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<Event> GetAll() {
            return _context.Events.ToList();
        }

        [HttpGet("{id}", Name = "GetEvent")]
        public IActionResult GetById(long id) {
            var item = _context.Events.FirstOrDefault(e => e.Id == id);
            if (item == null) {
                return NotFound();
            }

            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Event ev) {
            if (ev == null) {
                return BadRequest();
            }

            _context.Events.Add(ev);
            _context.SaveChanges();

            return CreatedAtRoute("GetEvent", new { id = ev.Id }, ev);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Event ev) {
            if (ev == null || ev.Id != id) {
                return BadRequest();
            }

            var evo = _context.Events.FirstOrDefault(e => e.Id == id);
            if (evo == null) {
                return NotFound();
            }

            evo.Timestamp = ev.Timestamp;
            evo.Pressure = ev.Pressure;

            _context.Events.Update(evo);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id) {
            var ev = _context.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null) {
                return NotFound();
            }

            _context.Events.Remove(ev);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}