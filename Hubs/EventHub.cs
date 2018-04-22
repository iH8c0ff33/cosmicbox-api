using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CosmicBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CosmicBox.Hubs {
    [Authorize]
    public class EventHub : Hub {
        private readonly CosmicContext _context;

        public EventHub(CosmicContext context) => _context = context;

        public async Task<IEnumerable<Run>> ListRuns(int boxId) =>
            await _context.Runs.Where(r => r.BoxId == boxId).ToListAsync();

        public async Task JoinRun(int id) => await Groups.AddAsync(Context.ConnectionId, id.ToString());

        public async Task AddTrace(Models.Trace trace) {
            var run = _context.Runs.Where(r => r.Id == trace.RunId).Include(r => r.Box.Grants).FirstOrDefault();
            if (run == null || !run.Box.Grants.Any(g => g.Sub == Context.UserIdentifier && (
                g.Type == Grant.Types.Owner ||
                g.Type == Grant.Types.Write
            ))) {
                await Clients.Caller.SendAsync("pirla");
                return;
            }
            if (run.End <= DateTime.Now) {
                await Clients.Caller.SendAsync("pirla");
                return;
            }

            _context.Traces.Add(trace);
            _context.SaveChanges();

            await Clients.Group(run.Id.ToString()).SendAsync("Trace", trace);
        }
    }
}