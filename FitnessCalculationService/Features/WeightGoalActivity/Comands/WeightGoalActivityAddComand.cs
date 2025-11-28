using MediatR;
using Fitness.Features.Dtos;
using Fitness.Data;
using Fitness.Infrastructure.Services;
using System.Reflection.Metadata;
using Fitness.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace Fitness.Features.WeightGoalActivity.Comands
{
    public record WeightGoalActivityAddComand(AddWGA Addwga):IRequest<Guid>;

 


    public  class WeightGoalActivityAddCommandHandler:IRequestHandler<WeightGoalActivityAddComand,Guid>
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<WeightGoalActivitydb> _wgarepository;


        public WeightGoalActivityAddCommandHandler(ApplicationDbContext context,IRepository<WeightGoalActivitydb> wgarepository )
        {
            _context = context;   
            _wgarepository = wgarepository;
        }
        public async Task<Guid> Handle (WeightGoalActivityAddComand request,CancellationToken cancellationToken)
        {
            var dto = request.Addwga;



            var add = new Data.WeightGoalActivitydb
            {
                UserId = dto.UserId,
                Weight = dto.Weight,
                Height = dto.Height,
                Age = dto.Age,
                Gender = dto.Gender

            };
            await _wgarepository.AddAsync(add);
            await _wgarepository.SaveChanges();

            return add.Id;
        }




    }








}

