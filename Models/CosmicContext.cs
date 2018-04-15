using Microsoft.EntityFrameworkCore;

namespace CosmicBox.Models {
    class CosmicContext : DbContext {
        public DbSet<Box> Boxes { get; set; }
        public DbSet<Run> Runs { get; set; }
        public DbSet<Trace> Traces { get; set; }
        public DbSet<Grant> Grants { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder b) {
            if (!b.IsConfigured) {
                b.UseNpgsql("Host=localhost;Database=cosmic;Username=eee;Password=pw1.safe");
            }
        }
    }
}