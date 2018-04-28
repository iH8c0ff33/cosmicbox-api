using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmicBox.Auth;
using CosmicBox.Hubs;
using CosmicBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CosmicBox.Controllers {
    [Route("api/[controller]"), Produces("application/json")]
    [ApiController]
    public class TraceController : ControllerBase {
        private readonly CosmicContext _context;
        private readonly IHubContext<EventHub> _hubContext;

        public TraceController(CosmicContext context, IHubContext<EventHub> hubContext) {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Trace>), 200)]
        public async Task<ActionResult<List<Trace>>> GetAll() => await _context.Traces.ToListAsync();

        [HttpPost, Authorize]
        [ProducesResponseType(typeof(Trace), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Run), 409)]
        public async Task<ActionResult<Trace>> Create(Trace trace) {
            var run = await _context.Runs.Include(r => r.Box.Grants).SingleOrDefaultAsync(r => r.Id == trace.RunId);
            if (run == null) { return NotFound(); }
            if (!run.Box.HasWriteAccess(User.GetIdentifier())) {
                return Forbid();
            }
            if (!trace.BelongsTo(run)) {
                return Conflict(run);
            }
            trace.Run = run;


            _context.Traces.Add(trace);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(run.Id.ToString()).SendAsync("Trace", trace);

            return trace;
        }

        [HttpPost, Authorize]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 403)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 409)]
        public async Task<ActionResult> CreateBatch(List<Trace> traces) {
            var runIds = traces.Select(t => t.RunId).Distinct();
            var runs = runIds.Select(i => _context.Runs
                .Include(r => r.Box.Grants)
                .SingleOrDefault(r => r.Id == i));
            if (runs.Any(r => r == null)) {
                return NotFound();
            }
            if (runs.Any(r => !r.Box.HasWriteAccess(User.GetIdentifier()))) {
                return Forbid();
            }
            if (traces.Any(t => !t.BelongsTo(_context.Runs.Single(r => r.Id == t.RunId)))) {
                return Conflict();
            }

            _context.Traces.AddRange(traces);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}