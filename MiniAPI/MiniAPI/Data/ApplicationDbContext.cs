using Microsoft.EntityFrameworkCore;
using MiniAPI.Models;

namespace MiniAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Coupon> Coupon { get; set; }
        public DbSet<LocalUser> LocalUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupon>().HasData(
                               new Coupon { Id = 1, Name = "First Coupon", Percent = 10, IsActive = true },
                                    new Coupon { Id = 2, Name = "Second Coupon", Percent = 20, IsActive = true },
                                    new Coupon { Id = 3, Name = "Third Coupon", Percent = 30, IsActive = true }
         );

            base.OnModelCreating(modelBuilder);
        }
    }
}
