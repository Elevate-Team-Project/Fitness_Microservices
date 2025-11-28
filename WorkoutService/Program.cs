using Mapster;
using MapsterMapper;
using MediatR;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Interfaces;
using WorkoutService.Features;
using WorkoutService.Infrastructure;
using WorkoutService.Infrastructure.Data;
using WorkoutService.Infrastructure.UnitOfWork;
using WorkoutService.MiddleWares;
using LinqKit;
using WorkoutService.Features.Consumers;
using WorkoutService.Infrastructure.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        // -------------------------------------------------------------------------------------
        // 1. Serilog Configuration
        // -------------------------------------------------------------------------------------
        // Configure structured logging to output logs to both Console and File.
        // We override minimum levels for system namespaces to reduce noise in the logs.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "WorkoutService")
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} | {CorrelationId} | {Message:lj}{NewLine}{Exception}", theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate)
            .WriteTo.File("logs/WorkoutService-.log", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext} | {CorrelationId} | {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Log.Information("Starting WorkoutService Application");

            var builder = WebApplication.CreateBuilder(args);

            // Replace the default logging provider with Serilog
            builder.Host.UseSerilog();

            var config = builder.Configuration;

            // -------------------------------------------------------------------------------------
            // 2. Service Registration (Dependency Injection)
            // -------------------------------------------------------------------------------------

            // Core Services
            builder.Services.AddMemoryCache(); // Used for caching query results
            builder.Services.AddHttpContextAccessor(); // Allows access to HTTP Context (User Identity)

            // Application Services & Middleware
            builder.Services.AddScoped<TransactionMiddleware>(); // Manages database transactions per request
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>(); // Abstraction for retrieving the current user ID

            // Database Context Configuration
            // We use AddDbContextPool to recycle context instances for better performance.
            builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // Optimization for read-heavy workloads
                       .WithExpressionExpanding(); // Enables LinqKit for dynamic predicate building

                // Enable detailed logging only in development environment for debugging
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging(true);
                    options.EnableDetailedErrors(true);
                }
            });

            // Unit of Work Registration
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Generic Repository Registration
            // Scans the assembly for all entities inheriting from BaseEntity and registers their repositories automatically.
            var entityTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseEntity)))
                .ToList();

            foreach (var entityType in entityTypes)
            {
                var interfaceType = typeof(IBaseRepository<>).MakeGenericType(entityType);
                var implementationType = typeof(BaseRepository<>).MakeGenericType(entityType);
                builder.Services.AddScoped(interfaceType, implementationType);
            }

            Log.Information("Registered {Count} generic repositories successfully", entityTypes.Count);

            // MediatR Configuration (CQRS Pattern)
            builder.Services.AddMediatR(typeof(Program).Assembly);

            // Mapster Configuration (Object Mapping)
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
            builder.Services.AddSingleton(typeAdapterConfig);

            // -------------------------------------------------------------------------------------
            // MassTransit & RabbitMQ Configuration
            // -------------------------------------------------------------------------------------
            builder.Services.AddMassTransit(x =>
            {
                // Register Consumers: These classes handle incoming messages from the message broker
                x.AddConsumer<WorkoutCreatedConsumer>();
                x.AddConsumer<WorkoutSessionStartedConsumer>();

                // Configure RabbitMQ Transport
                x.UsingRabbitMq((context, cfg) =>
                {
                    // Resolve Hostname (supports Docker service name or localhost)
                    var rabbitMqHost = config["RabbitMq:Host"] ?? "localhost";

                    cfg.Host(rabbitMqHost, "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Automatically create queues and bindings for registered consumers
                    cfg.ConfigureEndpoints(context);
                });
            });

            // -------------------------------------------------------------------------------------
            // API Security & Configuration
            // -------------------------------------------------------------------------------------

            // CORS Policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials());
            });

            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
                };
            });

            builder.Services.AddAuthorization();

            // Swagger / OpenAPI Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkoutService API", Version = "v1" });

                // Configure JWT Bearer Authentication for Swagger UI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field (e.g., 'Bearer {token}')",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            // -------------------------------------------------------------------------------------
            // 3. Database Migration & Seeding (Startup Scope)
            // -------------------------------------------------------------------------------------
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    // Apply pending migrations to the database
                    await context.Database.MigrateAsync();

                    // Seed initial data if the database is empty
                    await DatabaseSeeder.SeedAsync(services);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occurred during database migration or seeding");
                    if (app.Environment.IsDevelopment()) throw;
                }
            }

            // -------------------------------------------------------------------------------------
            // 4. HTTP Request Pipeline (Middleware Order)
            // -------------------------------------------------------------------------------------

            // Global Error Handling: Must be the first middleware to catch exceptions from all subsequent layers
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Enable Cross-Origin Resource Sharing
            app.UseCors("AllowAll");

            // Enable Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Transaction Middleware: Handles DB transactions for Command requests
            app.UseMiddleware<TransactionMiddleware>();

            // Map API Endpoints (Minimal APIs or Controllers)
            app.MapAllEndpoints();

            // Start the application
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application failed to start due to an unhandled exception");
        }
        finally
        {
            // Ensure logs are flushed before shutdown
            Log.CloseAndFlush();
        }
    }
}