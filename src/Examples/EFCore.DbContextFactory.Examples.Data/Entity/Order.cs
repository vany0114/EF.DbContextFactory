using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFCore.DbContextFactory.Examples.Data.Entity
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}