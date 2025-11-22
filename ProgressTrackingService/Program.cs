using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineExam.Infrastructure.UnitOfWork;
using ProgressTrackingService.Domain.Interfaces;
using ProgressTrackingService.Infrastructure;
using ProgressTrackingService.Infrastructure.Data;

namespace ProgressTrackingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
      
            builder.Services.AddScoped<IUniteOfWork,UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(BaseRepository<>));
            builder.Services.AddDbContext<FitnessAppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging(true);
                    options.EnableDetailedErrors(true);
                }
            });
            //builder.Services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "redis-18828.c16.us-east-1-3.ec2.redns.redis-cloud.com:18828,password=sKd8WfR2OctEuTiPXH5iqeQjS75xFlkl";
            //    options.InstanceName = "FitnessApp";
            //});
            builder.Services.AddMemoryCache();
            
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddMediatR(typeof(Program).Assembly);



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            

            app.MapControllers();

            app.Run();
        }
    }
}
