using MediatR;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Feature.UserStatisticsfiles.GetByIdQuery;
using ProgressTrackingService.Feature.UserStatisticsfiles.GetByUserIdQuery;
using ProgressTrackingService.Feature.Waight.UpdateWaightGoal.DTOs;

namespace ProgressTrackingService.Feature.Waight.UpdateWaightGoal
{
    public class UpdateWaightGoalHandler : IRequestHandler<UpdateWaightGoalCommand, DTOs.UpdateGoalWaightDtoResponse>
    {
        private readonly IGenericRepository<Domain.Entity.UserStatistics> _repository;
        private readonly IMediator _mediator;
        private readonly IUniteOfWork _UOW;

        public UpdateWaightGoalHandler(IGenericRepository<Domain.Entity.UserStatistics>repository,
            IMediator mediator,
            IUniteOfWork UOW)
        {
            this._repository = repository;
            this._mediator = mediator;
            _UOW = UOW;
        }
        public Task<UpdateGoalWaightDtoResponse> Handle(UpdateWaightGoalCommand request, CancellationToken cancellationToken)
        {
            var userStatisticsId = _mediator.Send(new GetUserStatisticsId_ByUserIdQuery  (request.userId)).Result;

            var userStatistics = new Domain.Entity.UserStatistics
            {
                Id = userStatisticsId,
                GoalWeight = request.newGoalWeight
            };
            _repository.SaveInclude(userStatistics, [nameof(userStatistics.GoalWeight)]);
            _UOW.SaveChangesAsync();
            var response = new UpdateGoalWaightDtoResponse
            {
                UserId = request.userId,
                NewGoalWeight = request.newGoalWeight
            };
            return Task.FromResult(response);

        }
    }
}
