using Fitness.Data;
using Fitness.Features.Dtos;
using FitnessCalculationService.Domain.Enums;
using FitnessCalculationService.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitnessCalculationService.Features.WeightGoalActivity.Comands
{
    public record WeightGoalActivityUpdateComand(AddWGA updWGA) : IRequest<Guid>;



    public class WeightGoalActivityUpdateComandHandler : IRequestHandler<WeightGoalActivityUpdateComand, Guid>
    {
        private readonly IRepository<WeightGoalActivitydb> _wgarepository;
        private readonly IRepository<UserFitnessStatdb> _fitnessRepository;


        public WeightGoalActivityUpdateComandHandler(IRepository<WeightGoalActivitydb> wgarepository, IRepository<UserFitnessStatdb> fitnessRepository)
        {

            _wgarepository = wgarepository;
            _fitnessRepository = fitnessRepository;
        }

        public async Task<Guid> Handle(WeightGoalActivityUpdateComand request, CancellationToken cancellationToken)
        {

            var dto = request.updWGA;


            var existingwga = await _wgarepository.FirstOrDefaultAsync(wga => wga.UserId == dto.UserId);
            if (existingwga == null)
            {
                throw new Exception("WeightGoalActivity record not found for the specified UserId.");
            }
            existingwga.Weight = dto.Weight;
            existingwga.Height = dto.Height;
            existingwga.Age = dto.Age;
            existingwga.Gender = dto.Gender;
            existingwga.ActivityLevel = dto.ActivityLevel;
            existingwga.Goal = dto.Goal;

            _wgarepository.SaveInclude(existingwga);


            await _wgarepository.SaveChanges();

            double bmr = existingwga.Gender.ToUpper() switch
            {
                "M" => 10 * existingwga.Weight + 6.25 * existingwga.Height - 5 * existingwga.Age + 5,
                "F" => 10 * existingwga.Weight + 6.25 * existingwga.Height - 5 * existingwga.Age - 161,
                _ => throw new Exception("Invalid gender")
            };

            double activityFactor = existingwga.ActivityLevel switch
            {
                ActivityLevel.Rookie => 1.2,
                ActivityLevel.Beginner => 1.375,
                ActivityLevel.Intermediate => 1.55,
                ActivityLevel.Advance => 1.725,
                ActivityLevel.TrueBeast => 1.9,
                _ => 1.2
            };
            double tdee = bmr * activityFactor;

            double calorieTarget = existingwga.Goal switch
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

            var fitnessStat = await _fitnessRepository.Query()
                .FirstOrDefaultAsync(f => f.Id == existingwga.Id, cancellationToken);

            if (fitnessStat == null)
            {
                fitnessStat = new UserFitnessStatdb
                {
                    Id = Guid.NewGuid(),
                    weightGoalActivityId = existingwga.Id
                };
                await _fitnessRepository.AddAsync(fitnessStat);
            }

            fitnessStat.Bmr = (decimal)bmr;
            fitnessStat.Tdee = (decimal)tdee;
            fitnessStat.CalorieTarget = (decimal)calorieTarget;
            fitnessStat.Status = status;
            fitnessStat.InsertDate = DateTime.UtcNow;

            _fitnessRepository.SaveInclude(fitnessStat);


            await _fitnessRepository.SaveChanges();



            return existingwga.Id;

        }



    }
}
