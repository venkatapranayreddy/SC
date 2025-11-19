using Microsoft.EntityFrameworkCore;

namespace DepartmentManager.Server.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Affiliation> Affiliations { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Member>()
    .HasOne(m => m.City)
    .WithMany()
    .HasForeignKey(m => m.CityId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Member>()
                .HasOne(m => m.Affiliation)
                .WithMany()
                .HasForeignKey(m => m.AffiliationId)
                .OnDelete(DeleteBehavior.Restrict);

            //// Seed Cities
            //modelBuilder.Entity<City>().HasData(
            //    new City { CityId = 1, CityName = "New York" },
            //    new City { CityId = 2, CityName = "Los Angeles" }
            //);

            //// Seed Affiliations
            //modelBuilder.Entity<Affiliation>().HasData(
            //    new Affiliation { AffiliationId = 1, Name = "Affiliation A", CityId = 1 },
            //    new Affiliation { AffiliationId = 2, Name = "Affiliation B", CityId = 2 }
            //);

            //// Seed Roles
            //modelBuilder.Entity<Role>().HasData(
            //    new Role { RoleId = 1, Name = "Admin", AffiliationId = 1, Level = 1 },
            //    new Role { RoleId = 2, Name = "User", AffiliationId = 1, Level = 2 }
            //);
        }
    }
}
