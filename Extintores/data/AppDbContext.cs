using Extintores.Model;
using Microsoft.EntityFrameworkCore;

namespace Extintores.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PedidoProduto>()
                .HasKey(pp => new { pp.PedidoCodigo, pp.ProdutoCodigo });
        }


        //DBSet
        public DbSet<Categoria> Categoria { get; set; }
        
        public DbSet<Produto> Produto { get; set; }

        public DbSet<Cliente> Cliente { get; set; }

        public DbSet<Pedido> Pedido { get; set; }

        public DbSet<PedidoProduto> PedidoProduto { get; set; }
    }
}
