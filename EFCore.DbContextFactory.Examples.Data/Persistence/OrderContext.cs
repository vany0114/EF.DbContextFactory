using EFCore.DbContextFactory.Examples.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace EFCore.DbContextFactory.Examples.Data.Persistence
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
                .HasOne(t => t.Order)
                .WithMany(t => t.OrderItems)
                .IsRequired();
        }
    }
}