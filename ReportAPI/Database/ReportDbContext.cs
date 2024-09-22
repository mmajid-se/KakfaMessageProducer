using KafkaConsumer.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaConsumer.Database
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }
        public DbSet<Message> Message { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Inventory> inventories { get; set; }
    }
}
