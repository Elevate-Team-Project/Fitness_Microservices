using Fitness.Data.Enums;
using Fitness.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Fitness.Api.Infrastructure.Persistence;
using Fitness.Infrastructure.Services;

public record CalculateUserFitnessCommand(Guid UserId) : IRequest<Guid>;

public class CalculateUserFitnessCommandHandler : IRequestHandler<CalculateUserFitnessCommand, Guid>
{
    private readonly ApplicationDbContext _context;
    private readonly IRepository<UserFitnessStatdb> _repo;
    private readonly IConfiguration _configuration;
    public CalculateUserFitnessCommandHandler(ApplicationDbContext context, IRepository<UserFitnessStatdb> repo, IConfiguration configuration)
    {
        _context = context;
        _repo = repo;
        _configuration = configuration;
    }

    public async Task<Guid> Handle(CalculateUserFitnessCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.WeightGoalActivity
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (profile == null)
            throw new Exception("User profile not found.");

        double bmr = profile.Gender.ToUpper() switch
        {
            "M" => 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age + 5,
            "F" => 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age - 161,
            _ => throw new Exception("Invalid gender value.")
        };

        double activityFactor = profile.ActivityLevel switch
        {
            ActivityLevel.Rookie => 1.2,
            ActivityLevel.Beginner => 1.375,
            ActivityLevel.Intermediate => 1.55,
            ActivityLevel.Advance => 1.725,
            ActivityLevel.TrueBeast => 1.9,
            _ => 1.2
        };

        double tdee = bmr * activityFactor;

        double calorieTarget = profile.Goal switch
        {
            Goal.LoseWeight => tdee - 500,
            Goal.GetFitter => tdee,
            Goal.GainWeight => tdee + 300,
            Goal.GainMoreflexible => tdee + 150,
            Goal.LearntheBasic => tdee,
            _ => tdee
        };

        string status = calorieTarget switch
        {
            <= 1800 => "Weak",
            <= 2500 => "Normal",
            _ => "Hard"
        };

        var fitnessStat = new UserFitnessStatdb
        {
            UserId = profile.Id,
            Bmr = (decimal)bmr,
            Tdee = (decimal)tdee,
            CalorieTarget = (decimal)calorieTarget,
            Status = status,
            InsertDate = DateTime.Now
        };

        await _repo.AddAsync(fitnessStat);
        await _repo.SaveChanges();

        return fitnessStat.Id;
    }
}
