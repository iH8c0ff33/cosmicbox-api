using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CosmicBox.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace CosmicBox.Controllers {
    [Produces("application/json")]
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
        /// <summary>
        /// Get all the events on the DB
        /// </summary>
        /// 
        /// <response code="200">List of all events</response>
        [HttpGet]
        public IEnumerable<Event> GetAll() => _context.Events.ToList();

        /// <summary>
        /// Create an Event
        /// </summary>
        /// <param name="ev"></param>
        /// 
        /// <response code="201">Event created</response>
        /// <response code="400">Supplied event is not valid</response>
        [HttpPost]
        [Authorize("write:events")]
        [ProducesResponseType(typeof(Event), 201)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult Create([FromBody] Event ev) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            _context.Events.Add(ev);
            _context.SaveChanges();

            return CreatedAtRoute("GetEvent", new { id = ev.Id }, ev);
        }

        /// <summary>
        /// Get an event by ID
        /// </summary>
        /// <param name="id">event ID</param>
        /// 
        /// <response code="200">Event found</response>
        /// <response code="404">Event not found</response>
        [ProducesResponseType(typeof(Event), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [HttpGet("{id}", Name = "GetEvent")]
        public IActionResult GetById(long id) {
            var item = _context.Events.FirstOrDefault(e => e.Id == id);
            if (item == null) {
                return NotFound();
            }

            return new ObjectResult(item);
        }

        /// <summary>
        /// Update an Event
        /// </summary>
        /// <param name="id">event ID</param>
        /// <param name="ev">new event</param>
        /// 
        /// <response code="204">Event updated</response>
        /// <response code="400">Supplied event is not valid</response>
        /// <response code="404">Event not found</response>
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
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

        /// <summary>
        /// Delete an Event by ID
        /// </summary>
        /// <param name="id">event ID</param>
        /// 
        /// <response code="204">Event deleted</response>
        /// <response code="404">Event not found</response>
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 404)]
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