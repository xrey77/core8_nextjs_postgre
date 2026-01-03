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
        
        public DbSet<User> Users { get; set; } 
        public DbSet<Product> Products { get; set; } 
        public DbSet<Role> Roles {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    j => j.HasOne<Role>().WithMany().HasForeignKey("roles_id"), // FK to Roles table
                    j => j.HasOne<User>().WithMany().HasForeignKey("user_id"),  // FK to Users table
                    j =>
                    {
                        j.ToTable("user_roles"); // Join table name in Postgres
                    });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedAt); // EF will now track the value you set in SaveChanges
                entity.Property(e => e.UpdatedAt);
            });
            // modelBuilder.Entity<User>(entity =>
            // {
            //     entity.Property(e => e.CreatedAt)
            //         .HasDefaultValueSql("now()")
            //         .ValueGeneratedOnAdd(); 
                    
            //     entity.Property(e => e.UpdatedAt)
            //         .HasDefaultValueSql("now()")
            //          .ValueGeneratedOnAddOrUpdate(); 
            // });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()");
                    
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("now()");
            });
        }
            public override int SaveChanges(bool acceptAllChangesOnSuccess)
            {
                var entries = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

                foreach (var entityEntry in entries)
                {
                    entityEntry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;

                    if (entityEntry.State == EntityState.Added)
                    {
                        entityEntry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                    }
                }

                return base.SaveChanges(acceptAllChangesOnSuccess);
            }

            public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
            {
                return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
        

    }
    
}
