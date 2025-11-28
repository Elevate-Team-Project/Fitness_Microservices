//using Fitness.Api.Infrastructure.Persistence;
//using Fitness.Data;
//using Fitness.Infrastructure.Services;
//using MediatR;
//using Microsoft.EntityFrameworkCore;

//namespace Fitness.Features.AssignFitnessPlanCommand
//{
//    public record AssignFitnessPlanCommand(Guid userid):IRequest<Guid>;
  

//    public class AssignFitnessPlanCommandHandler : IRequestHandler<AssignFitnessPlanCommand,Guid>
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IRepository<FitnessPlanConfigdb> _repo;

//        public AssignFitnessPlanCommandHandler(ApplicationDbContext context, IRepository<FitnessPlanConfigdb> repo)
//        {
//           _context = context;
//            _repo = repo;

//        }

//        public async Task<Guid> Handle (AssignFitnessPlanCommand request, CancellationToken cancellationToken)
//        {
           
            
            
//            var stat = await _context.UserFitnessStat
//           .Include(u => u.weightGoalActivity)
//           .OrderByDescending(x => x.InsertDate)
//           .FirstOrDefaultAsync(u => u.UserId == request.userid, cancellationToken);


//            if (stat == null)
//                throw new Exception("User fitness stats not found.");

//            var goal = stat.weightGoalActivity.Goal.ToString();
//            var status = stat.Status;
//            var calorieTarget = stat.CalorieTarget;

//            var config = await _context.FitnessPlanConfig
//                       .FirstOrDefaultAsync(f =>
//                           f.Goal == goal &&
//                           f.Status == status &&
//                           calorieTarget >= f.MinCalorie &&
//                           calorieTarget <= f.MaxCalorie,
//                           cancellationToken);

//            if (config == null)
//                throw new Exception("No matching fitness plan configuration found.");



//            return config.WorkoutPlan.PlanId;

//        }

//    }
//}
