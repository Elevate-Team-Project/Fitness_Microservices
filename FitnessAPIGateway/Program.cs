using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Net; // Required for SSL options
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Load Ocelot Configuration
// This loads the routes from ocelot.json. 'optional: false' means app will crash if file is missing.
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// ---------------------------------------------------------
// ✅ 1. Register HttpClient with SSL Bypass (Development Only)
// ---------------------------------------------------------
// We create a named client "InsecureClient" that ignores SSL certificate errors.
// This is necessary because Docker containers use self-signed certificates that are not trusted by default.
builder.Services.AddHttpClient("InsecureClient")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ClientCertificateOptions = ClientCertificateOption.Manual,
        ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) => true // ⚠️ Accept ANY certificate (Development only)
    });

// 2. Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Log a warning if keys are missing (Prevents silent failures)
if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    Console.WriteLine("⚠️ Warning: JWT settings are missing in appsettings.json. Auth might fail.");
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
            // Use the key from config, or a dummy key to prevent startup crash if config is missing
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? "temp_key_for_build_success_only"))
        };
    });

// 3. Add Ocelot Services
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// 4. Configure Middleware Pipeline
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ---------------------------------------------------------
// ✅ 2. Connectivity Test Zone (Minimal APIs)
// ---------------------------------------------------------

// Helper function to test connection to other microservices
// It uses the "InsecureClient" to bypass SSL errors inside Docker
async Task<IResult> TestServiceConnection(IHttpClientFactory factory, string serviceName, string url)
{
    try
    {
        var client = factory.CreateClient("InsecureClient");
        client.Timeout = TimeSpan.FromSeconds(5); // Short timeout to avoid long waits

        // We try to reach the Swagger UI page as a "Heartbeat" check
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

// 👉 Test Endpoint: Workout Service
// Connects via HTTPS on internal port 8081
app.MapGet("/test/workout", async (IHttpClientFactory factory) =>
{
    return await TestServiceConnection(factory, "Workout Service", "https://workoutservice:8081/swagger/index.html");
});

// 👉 Test Endpoint: Auth Service
// Connects via HTTPS on internal port 8081
app.MapGet("/test/auth", async (IHttpClientFactory factory) =>
{
    return await TestServiceConnection(factory, "Auth Service", "https://authenticationservice:8081/swagger/index.html");
});

// 👉 Test Endpoint: Nutrition Service
// Connects via HTTPS on internal port 8081
app.MapGet("/test/nutrition", async (IHttpClientFactory factory) =>
{
    return await TestServiceConnection(factory, "Nutrition Service", "https://nutritionservice:8081/swagger/index.html");
});

// ---------------------------------------------------------

// 5. Enable Ocelot (Must be the last middleware)
await app.UseOcelot();

app.Run();