using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProgressTrackingService.Feature.LogWorkout.PlaceWorkoutOrchestrator
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutController : ControllerBase 
    {
       
        private readonly IMediator _mediator;

        public WorkoutController(IMediator mediator) 
        {
            
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> PlaceWorkout([FromBody] WorkoutOrchestrator command) 
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
