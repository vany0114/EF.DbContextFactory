using System;
using System.ComponentModel.DataAnnotations;

namespace EF.DbContextFactory.Examples.Data.Entity
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        public virtual Order Order { get; set; }
    }
}