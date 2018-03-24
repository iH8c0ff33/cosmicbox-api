using Microsoft.EntityFrameworkCore;

namespace CosmicBox.Models {
    public class ApiContext : DbContext {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
    }
}