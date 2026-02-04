namespace Infrastructure.Data;

using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.ToTable("Users");
            
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");
            
            entity.Property(e => e.Status)
                .HasConversion<int>()
                .IsRequired();
            
            entity.Property(e => e.LastImmunisationDate)
                .IsRequired(false);
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .IsRequired(false);
        });

        SeedData(modelBuilder);
    }

    // ✅ All dates are now STATIC - no more DateTime.UtcNow
    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = 1, 
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com", 
                Status = ImmunisationStatus.FullyImmunised,
                LastImmunisationDate = new DateTime(2025, 12, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User 
            { 
                Id = 2, 
                FirstName = "Jane", 
                LastName = "Smith", 
                Email = "jane.smith@example.com", 
                Status = ImmunisationStatus.PartiallyImmunised,
                LastImmunisationDate = new DateTime(2025, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User 
            { 
                Id = 3, 
                FirstName = "Bob", 
                LastName = "Johnson", 
                Email = "bob.johnson@example.com", 
                Status = ImmunisationStatus.NonImmunised,
                LastImmunisationDate = null,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User 
            { 
                Id = 4, 
                FirstName = "Alice", 
                LastName = "Williams", 
                Email = "alice.williams@example.com", 
                Status = ImmunisationStatus.Overdue,
                LastImmunisationDate = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User 
            { 
                Id = 5, 
                FirstName = "Charlie", 
                LastName = "Brown", 
                Email = "charlie.brown@example.com", 
                Status = ImmunisationStatus.FullyImmunised,
                LastImmunisationDate = new DateTime(2025, 11, 20, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}