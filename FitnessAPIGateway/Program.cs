using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// ---------------------------------------------------------
// ✅ 1. Register HttpClient (Required to send requests to other services)
// ---------------------------------------------------------
builder.Services.AddHttpClient();

// 2. Add Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Check if JWT settings exist (Optional warning for debugging)
if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    Console.WriteLine("⚠️ Warning: JWT settings are missing, Auth might fail.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            // Use key from config or a fallback for build safety
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? "temp_key_for_build_success"))
        };
    });

// Add Ocelot services
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ---------------------------------------------------------
// ✅ 2. Connectivity Test Zone (Minimal APIs)
// ---------------------------------------------------------

// Helper function to test connection
async Task<IResult> TestServiceConnection(IHttpClientFactory factory, string serviceName, string url)
{
    try
    {
        var client = factory.CreateClient();
        // We try to reach the Swagger UI page as a "Heartbeat" check
        // (Since we might not know the exact API endpoints yet)
        var response = await client.GetAsync(url);

        return Results.Ok(new
        {
            TargetService = serviceName,
            TargetUrl = url,
            StatusCode = response.StatusCode,
            Message = "✅ Success! I can see the service."
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            TargetService = serviceName,
            TargetUrl = url,
            Error = ex.Message,
            Message = "❌ Failed! I cannot reach the service."
        }, statusCode: 500);
    }
}

// 👉 Test Workout Service
app.MapGet("/test/workout", async (IHttpClientFactory factory) =>
{
    // Note: We use the Docker Service Name (workoutservice) and internal port (8080)
    return await TestServiceConnection(factory, "Workout Service", "http://workoutservice:8080/swagger/index.html");
});

// 👉 Test Auth Service
app.MapGet("/test/auth", async (IHttpClientFactory factory) =>
{
    return await TestServiceConnection(factory, "Auth Service", "http://authenticationservice:8080/swagger/index.html");
});

// 👉 Test Nutrition Service
app.MapGet("/test/nutrition", async (IHttpClientFactory factory) =>
{
    return await TestServiceConnection(factory, "Nutrition Service", "http://nutritionservice:8080/swagger/index.html");
});

// ---------------------------------------------------------

// Use Ocelot (Must be the last middleware)
await app.UseOcelot();

app.Run();