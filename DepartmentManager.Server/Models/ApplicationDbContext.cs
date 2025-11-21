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
        public DbSet<MemberAffiliation> MemberAffiliations { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Member - Manager self-referencing relationship
            modelBuilder.Entity<Member>()
                .HasOne(m => m.Manager)
                .WithMany()
                .HasForeignKey(m => m.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // MemberAffiliation relationships
            modelBuilder.Entity<MemberAffiliation>()
                .HasOne(ma => ma.Member)
                .WithMany(m => m.MemberAffiliations)
                .HasForeignKey(ma => ma.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MemberAffiliation>()
                .HasOne(ma => ma.City)
                .WithMany()
                .HasForeignKey(ma => ma.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MemberAffiliation>()
                .HasOne(ma => ma.Affiliation)
                .WithMany()
                .HasForeignKey(ma => ma.AffiliationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MemberAffiliation>()
                .HasOne(ma => ma.Role)
                .WithMany()
                .HasForeignKey(ma => ma.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MemberAffiliation>()
                .HasOne(ma => ma.Approver)
                .WithMany(m => m.ApprovalsAsApprover)
                .HasForeignKey(ma => ma.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MemberAffiliation>()
                .HasOne(ma => ma.ApprovedByApprover)
                .WithMany()
                .HasForeignKey(ma => ma.ApprovedByApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MemberAffiliation>()
                .HasOne(ma => ma.ApprovedByManager)
                .WithMany(m => m.ApprovalsAsManager)
                .HasForeignKey(ma => ma.ApprovedByManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApprovalRequest relationships
            modelBuilder.Entity<ApprovalRequest>()
                .HasOne(ar => ar.MemberAffiliation)
                .WithMany()
                .HasForeignKey(ar => ar.MemberAffiliationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApprovalRequest>()
                .HasOne(ar => ar.RequestedToMember)
                .WithMany()
                .HasForeignKey(ar => ar.RequestedToMemberId)
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
