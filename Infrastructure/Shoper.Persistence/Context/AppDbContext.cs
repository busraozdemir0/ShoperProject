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
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-EIOVHUN; Database=Shoper; IntegratedSecurity=True; TrustServerCertificate=True;");
        }

        public DbSet<Category> Categories { get; set; }
    }
}
