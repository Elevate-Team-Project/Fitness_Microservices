using AuthenticationService.Contarcts;
using AuthenticationService.Data.Seed;
using AuthenticationService.Models;
using AuthenticationService.Repositories;
using AuthenticationService.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Online_Exam_System.Data;
using Online_Exam_System.Repositories;
using System.Reflection;
using System.Text;
using System.Text.Json;
using AuthenticationService.Features.Auth.Register;
using AuthenticationService.Features.Auth.Login; // Adjust namespaces as needed

namespace AuthenticationService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- Services ---
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Swagger Configuration
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<SuperFitnessAppAuthContext>(options =>
                options.UseSqlServer(connectionString));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<SuperFitnessAppAuthContext>()
            .AddDefaultTokenProviders();

            // JWT Configuration
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                };
            });

            builder.Services.AddAuthorization();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials());
            });

            // DI Registrations
            builder.Services.AddScoped<IImageHelper, ImageHelper>();
            builder.Services.AddScoped<ITokenService, JwtService>();
            builder.Services.AddScoped<IMailKitEmailService, MailKitEmailService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
            // Caching
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            // --- Migration & Seeding ---
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    Console.WriteLine("📊 [Auth] Starting database migration...");
                    var context = services.GetRequiredService<SuperFitnessAppAuthContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                    await context.Database.MigrateAsync();
                    await IdentitySeeder.SeedIdentityAsync(roleManager, userManager);
                    Console.WriteLine("✅ [Auth] Database migration & seeding completed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ [Auth] Error seeding data: {ex.Message}");
                }
            }

            // --- Pipeline ---
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            app.UseCors("AllowAll"); // CORS first
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}