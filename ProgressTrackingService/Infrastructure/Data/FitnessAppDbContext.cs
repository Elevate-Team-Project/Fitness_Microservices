using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Domain.Entity;

namespace ProgressTrackingService.Infrastructure.Data
{
    public class FitnessAppDbContext : DbContext
    {
        public FitnessAppDbContext(DbContextOptions<FitnessAppDbContext> options) : base(options)
        {
        }
        public DbSet<WorkoutLog> workoutLogs { get; set; }
        public DbSet<Achievement> achievements { get; set; }
        public DbSet<UserAchievement> userAchievements { get; set; }
        public DbSet<WeightHistory> weightHistories { get; set; }

        public DbSet<UserStatistics> userStatistics { get; set; }

        




        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    OnBeforeSaving();
        //    return base.SaveChangesAsync(cancellationToken);
        //}




        public void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries) 
            {
              if(entry.Entity is BaseEntity baseEntity)
                {
                    switch(entry.Entity)
                    {
                        case EntityState.Added:
                            baseEntity.CreatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            baseEntity.UpdatedAt= DateTime.UtcNow;
                            break;
                        case EntityState.Deleted:
                            baseEntity.IsDeleted= true;
                            baseEntity.DeletedAt=DateTime.UtcNow;
                            entry.State = EntityState.Modified;//soft delete
                            break;
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitnessAppDbContext).Assembly);
            modelBuilder.Entity<WorkoutLog>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Achievement>().HasQueryFilter(e =>! e.IsDeleted);
            modelBuilder.Entity<UserAchievement>().HasQueryFilter(e=>!e.IsDeleted);
            modelBuilder.Entity<WeightHistory>().HasQueryFilter(e=>!e.IsDeleted);
            modelBuilder.Entity<UserStatistics>().HasQueryFilter(e=>!e.IsDeleted);
        }

        

    }
}
