using System.Data.Entity;
using EF.DbContextFactory.Examples.Data.Entity;

namespace EF.DbContextFactory.Examples.Data.Persistence
{
    public class OrderContext : DbContext
    {
        //public OrderContext() :
        //    base("Name=DefaultConnection")
        //{
        //    Database.SetInitializer<OrderContext>(null);
        //    Configuration.LazyLoadingEnabled = false;
        //    Configuration.ProxyCreationEnabled = false;
        //}

        //public OrderContext()
        //{
        //    Database.SetInitializer<OrderContext>(null);
        //    Configuration.LazyLoadingEnabled = false;
        //    Configuration.ProxyCreationEnabled = false;
        //}

        //public OrderContext(string connectionString) :
        //    base(connectionString)
        //{
        //    Database.SetInitializer<OrderContext>(null);
        //    Configuration.LazyLoadingEnabled = false;
        //    Configuration.ProxyCreationEnabled = false;
        //}

        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
                .HasRequired(t => t.Order)
                .WithMany(t => t.OrderItems)
                .HasForeignKey(d => d.OrderId);
        }
    }
}