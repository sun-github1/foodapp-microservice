using Food.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Food.Services.Email.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<EmailLog> EmailLogs { get; set; }
    }
}
