using System.Threading.Tasks;
using CosmicBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CosmicBox.Hubs {
    public class EventHub : Hub {
        [Authorize("write:events")]
        public Task SendEvent(Event ev) {

            return Clients.All.SendAsync("Event", ev);
        }
    }
}