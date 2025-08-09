using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using core8_nextjs_postgre.Entities;

namespace core8_nextjs_postgre.Helpers
{    
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
/*
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        }
*/

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);
        //     modelBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        //     modelBuilder.Entity<User>().ToTable("users");
        //     modelBuilder.Entity<Product>().ToTable("products");
        // }

        // public ApplicationDbContext CreateDbContext(string[] args)
        // {
        //     var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //     optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        //     return new ApplicationDbContext(optionsBuilder.Options);
        // }
        public DbSet<User> Users { get; set; } 
        public DbSet<Product> Products { get; set; } 
    }
    
}
