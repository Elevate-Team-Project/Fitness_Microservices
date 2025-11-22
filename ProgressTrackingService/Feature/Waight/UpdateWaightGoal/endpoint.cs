using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProgressTrackingService.Feature.Waight.UpdateWaightGoal
{
    [ApiController]
    [Route("api/[controller]")]
    public class WaightController : ControllerBase 
    {
        private readonly IMediator _mediator;
        public WaightController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPut("UpdateWaightGoal")]
        public async Task<IActionResult> UpdateWaightGoal([FromBody] UpdateWaightGoalCommand request)
        {
            var command = new UpdateWaightGoalCommand(request.userId, request.newGoalWeight);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
