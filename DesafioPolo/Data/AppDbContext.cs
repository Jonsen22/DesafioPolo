using DesafioPolo.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioPolo.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<IndicadorModelDB> Indicadores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=DesafioPolo;User Id=sa;Password=teste123@;TrustServerCertificate=True;");
        }
    }
}
