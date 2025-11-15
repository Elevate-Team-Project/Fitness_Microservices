using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Infrastructure.Data;

namespace WorkoutService.Infrastructure.Repositories
{
    public class WorkoutRepository : BaseRepository<Workout>, IWorkoutRepository
    {
        public WorkoutRepository(WorkoutContext context) : base(context)
        {
        }
    }
}
