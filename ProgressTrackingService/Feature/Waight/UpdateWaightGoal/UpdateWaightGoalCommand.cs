using MediatR;
using ProgressTrackingService.Feature.Waight.UpdateWaightGoal.DTOs;

namespace ProgressTrackingService.Feature.Waight.UpdateWaightGoal
{
    public record UpdateWaightGoalCommand (int userId, double newGoalWeight) : IRequest<UpdateGoalWaightDtoResponse>;




}
