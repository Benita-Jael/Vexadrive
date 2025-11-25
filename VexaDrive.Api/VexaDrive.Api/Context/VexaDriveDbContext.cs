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

            // Seed Admin role + user (STATIC values only)
            var adminRoleId = "00000000-0000-0000-0000-000000000001";
            var customerRoleId = "00000000-0000-0000-0000-000000000002";
            var adminUserId = "00000000-0000-0000-0000-000000000003";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = customerRoleId, Name = "Customer", NormalizedName = "CUSTOMER" }
            );

            // Precomputed password hash for "Admin@123"
            // 👉 Generate once using PasswordHasher and paste here
            var adminPasswordHash = "AQAAAAIAAYagAAAAEB4x+LWMreo4yjc3W0Ur0bRgIgKWwutEO0yeNI6bE1I5uk1F+P3gIJtFcK6B2qocDA==";


            var adminUser = new IdentityUser
            {
                Id = adminUserId,
                UserName = "admin@vexadrive.com",
                NormalizedUserName = "ADMIN@VEXADRIVE.COM",
                Email = "admin@vexadrive.com",
                NormalizedEmail = "ADMIN@VEXADRIVE.COM",
                EmailConfirmed = true,
                SecurityStamp = "STATIC-SECURITY-STAMP",
                ConcurrencyStamp = "STATIC-CONCURRENCY-STAMP",
                PasswordHash = adminPasswordHash
            };

            modelBuilder.Entity<IdentityUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminUserId }
            );
        }
    }
}
