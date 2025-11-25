using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

using VexaDriveAPI.Models;
 
namespace VexaDriveAPI.Context

{

    public class VexaDriveDbContext : IdentityDbContext

    {

        public VexaDriveDbContext(DbContextOptions<VexaDriveDbContext> options)

            : base(options)

        {

        }
 
        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Owner> Owners { get; set; }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {

            base.OnModelCreating(modelBuilder);
 
            // Seed Owners

            modelBuilder.Entity<Owner>().HasData(

                new Owner { OwnerId = 1, FirstName = "John", LastName = "Doe", ContactNumber = "9876543210", Email = "john.doe@example.com" },

                new Owner { OwnerId = 2, FirstName = "Emma", LastName = "Smith", ContactNumber = "9753108642", Email = "emma.smith@example.com" },

                new Owner { OwnerId = 3, FirstName = "Raj", LastName = "Kumar", ContactNumber = "9898989898", Email = "raj.kumar@example.com" }

            );
 
            // Seed Vehicles

            modelBuilder.Entity<Vehicle>().HasData(

                new Vehicle { VehicleId = 1, Model = "Honda City", NumberPlate = "TN10AB1234", Type = "Car", OwnerId = 1 },

                new Vehicle { VehicleId = 2, Model = "Yamaha FZ", NumberPlate = "TN22CD5678", Type = "Bike", OwnerId = 2 },

                new Vehicle { VehicleId = 3, Model = "Hyundai i20", NumberPlate = "TN05EF9898", Type = "Car", OwnerId = 3 }

            );

        }

    }

}
