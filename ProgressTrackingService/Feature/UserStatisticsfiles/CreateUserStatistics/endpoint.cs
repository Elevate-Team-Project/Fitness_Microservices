using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackingService.Feature.UserStatistics.CreateUserStatistics;

namespace ProgressTrackingService.Feature.UserStatisticsfiles.CreateUserStatistics
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserStatisticsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserStatisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUserStatistics([FromBody] CreateUserStatisticsCommand request)
        {
            var command = new CreateUserStatisticsCommand(request.userId,request.currentWeight,request.goalWeight);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
