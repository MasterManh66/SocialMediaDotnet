using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Models;

namespace SocialMedia.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            public DbSet<User> users { get; set; }
            public DbSet<Role> role { get; set; }
        }
    }
}
