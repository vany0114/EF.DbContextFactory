using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EF.DbContextFactory.Examples.Data.Entity;

namespace EF.DbContextFactory.Examples.NInject.Models
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public bool Selected { get; set; }
    }
}