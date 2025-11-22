using MediatR;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Feature.UserStatisticsfiles.GetByUserIdQuery;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.GetByIdQuery
{
    public class GetByIdCommandHandler : IRequestHandler<GetUserStatisticsId_ByUserIdQuery, int>
    {
        private readonly IGenericRepository<Domain.Entity.UserStatistics> _repository;

        public GetByIdCommandHandler(IGenericRepository<Domain.Entity.UserStatistics>repository)
        {
            this._repository = repository;
        }
        public Task<int> Handle(GetUserStatisticsId_ByUserIdQuery request, CancellationToken cancellationToken)
        {
           var result =  _repository.GetByUserId(request.userId)
                                          .Select(x => x.Id)
                .FirstOrDefault();
            return Task.FromResult(result);
            

        }
    }
}
