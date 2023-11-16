using Food.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Food.Services.ShoppingCartAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Product> Products { get; set; }

    }
}
