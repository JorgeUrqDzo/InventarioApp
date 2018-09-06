using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioApp.Models
{
    public class ApplicationDbContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=inventario.db");
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<IncomingProduct> IncomingProducts { get; set; }
        public DbSet<OutcomingProduct> OutcomingProducts { get; set; }
        public DbSet<Donations> Donations { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

    }
}
