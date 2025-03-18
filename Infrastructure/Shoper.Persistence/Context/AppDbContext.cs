using Microsoft.EntityFrameworkCore;
using Shoper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoper.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-EIOVHUN; Database=Shoper; Integrated Security=True; TrustServerCertificate=True;");
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Category>()
        //        .HasMany(c => c.Products)
        //        .WithOne(p => p.Category)
        //        .HasForeignKey(p => p.CategoryId);

        //    modelBuilder.Entity<Product>()
        //       .HasOne(c => c.Category)
        //       .WithMany(p => p.Products)
        //       .HasForeignKey(p => p.CategoryId);

        //    modelBuilder.Entity<Customer>()
        //       .HasMany(c => c.Orders)
        //       .WithOne(p => p.Customer)
        //       .HasForeignKey(p => p.CustomerId);

        //    modelBuilder.Entity<Order>()
        //      .HasOne(c => c.Customer)
        //      .WithMany(p => p.Orders)
        //      .HasForeignKey(p => p.CustomerId);

        //    modelBuilder.Entity<Order>()
        //      .HasMany(c => c.OrderItems)
        //      .WithOne(p => p.Order)
        //      .HasForeignKey(p => p.OrderId);

        //    modelBuilder.Entity<OrderItem>()
        //     .HasOne(c => c.Order)
        //     .WithMany(p => p.OrderItems)
        //     .HasForeignKey(p => p.OrderId);
        //}
    }
}
