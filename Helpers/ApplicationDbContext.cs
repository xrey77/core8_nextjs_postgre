using core8_nextjs_postgre.Entities;
using Microsoft.EntityFrameworkCore;

namespace core8_nextjs_postgre.Helpers
{    
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; } 
        public DbSet<Product> Products { get; set; } 
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Many-to-Many relationship between User and Role
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
                entity.Property(e => e.CreatedAt); 
                entity.Property(e => e.UpdatedAt);
            });
                                
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()");
                    
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("now()");
            });
        }

        // 1. Centralized method to handle timestamp updates
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                // Ensure the entity actually has these properties to prevent runtime errors
                if (entityEntry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
                {
                    entityEntry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }

                if (entityEntry.State == EntityState.Added && entityEntry.Properties.Any(p => p.Metadata.Name == "CreatedAt"))
                {
                    entityEntry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }

        // 2. Synchronous SaveChanges
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        // 3. Asynchronous SaveChanges (Crucial for REST APIs)
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
