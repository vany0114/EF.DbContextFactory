using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.DbContextFactory.Examples.Data.Entity
{
    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        public Order Order { get; set; }
    }
}