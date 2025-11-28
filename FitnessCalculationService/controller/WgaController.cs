using Fitness.Features.Dtos;
using Fitness.Features.WeightGoalActivity.Comands;
using FitnessCalculationService.Features.WeightGoalActivity.Comands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCalculationService.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class WgaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WgaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddWga([FromBody] AddWGA dto)
        {
            var id = await _mediator.Send(new WeightGoalActivityAddComand(dto));
            return Ok(new { Success = true, UserId = id });


        }

        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateWga(Guid userId, [FromBody] AddWGA dto)
        {
            if (userId != dto.UserId)
                return BadRequest("UserId mismatch");

            var id = await _mediator.Send(new WeightGoalActivityUpdateComand(dto));
            return Ok(new { Success = true, UserId = id });
        }
    }
}
