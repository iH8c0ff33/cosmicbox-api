using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace CosmicBox.Models {
    public class ApiContext : DbContext {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);

        public DbSet<Event> Events { get; set; }

        public static readonly LoggerFactory MyLoggerFactory =
            new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });
    }
}