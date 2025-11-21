using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthenticationService.Models;

namespace Online_Exam_System.Data
{
    public class SuperFitnessAppAuthContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
   

        public SuperFitnessAppAuthContext(DbContextOptions<SuperFitnessAppAuthContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Always call base first to configure Identity properly
            base.OnModelCreating(modelBuilder);

            // ✅ Apply all IEntityTypeConfiguration from current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SuperFitnessAppAuthContext).Assembly);

            // ✅ Optional: Customize Identity table names if you want clean names
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
            });

            modelBuilder.Entity<IdentityRole<Guid>>(entity =>
            {
                entity.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }
    }
}
