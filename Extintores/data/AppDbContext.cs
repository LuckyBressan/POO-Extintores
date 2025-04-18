using Extintores.Model;
using Microsoft.EntityFrameworkCore;

namespace Extintores.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //DBSet
        public DbSet<Produto> Produto { get; set; }

        public DbSet<Categoria> Categoria { get; set; }
    }
}
