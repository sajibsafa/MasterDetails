using Microsoft.EntityFrameworkCore;
using System;

namespace MasterDetailsTest.Models
{
    public class MasterDBContext: DbContext
    {
        public MasterDBContext(DbContextOptions<MasterDBContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseProduct> PurchaseProducts { get; set; }
        public DbSet<Unit> Units { get; set; }

    }
}
