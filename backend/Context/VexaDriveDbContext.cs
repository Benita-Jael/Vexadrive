using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VexaDriveAPI.Models;

namespace VexaDriveAPI.Context
{
    public class VexaDriveDbContext : IdentityDbContext<IdentityUser>
    {
        public VexaDriveDbContext(DbContextOptions<VexaDriveDbContext> options)
            : base(options) { }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Bill> Bills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationships
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Vehicle)
                .WithMany(v => v.ServiceRequests)
                .HasForeignKey(sr => sr.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Bill)
                .WithOne(b => b.ServiceRequest)
                .HasForeignKey<Bill>(b => b.ServiceRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.ServiceRequest)
                .WithMany(sr => sr.Notifications)
                .HasForeignKey(n => n.ServiceRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.NumberPlate)
                .IsUnique();

            modelBuilder.Entity<ServiceRequest>()
                .HasIndex(sr => new { sr.Status, sr.CreatedAt });

            // NOTE: Identity role/user seeding is performed at runtime (startup) to ensure idempotency
            // and avoid embedding password hashes in migrations. See IdentitySeeder in Services/Seed.
        }
    }
}
