using MediatR;
using Microsoft.AspNetCore.Identity;
using AuthenticationService.Contarcts;
using AuthenticationService.Models;
using System.Net.Http;
using AuthenticationService.Dtos;
using Azure;

namespace AuthenticationService.Features.Auth.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public RegisterHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ITokenService tokenService,
            IConfiguration config,
            HttpClient httpClient)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var dto = request.RegisterDto;

            // 🔍 Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new ApplicationException("Email already registered.");

            // 🧠 Create new ApplicationUser
            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                FullName = $"{dto.FirstName} {dto.LastName}",
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                EmailConfirmed = true,
                ProfileImageUrl = "/images/users/default-user.png",
                Height = dto.Height,
                Weight = dto.Weight,
                Age = dto.Age,
                Gender= dto.Gender,
                ActivtyLevel = dto.ActivtyLevel,
                Goal = dto.Goal,
                CreatedAt = DateTime.UtcNow
            };

            // 🔑 Create user in Identity
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ApplicationException($"User creation failed: {errors}");
            }

            // 👤 Assign default role
            if (await _roleManager.RoleExistsAsync("User"))
                await _userManager.AddToRoleAsync(user, "User");

            // 🎭 Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // ✅ Generate Access + Refresh Tokens (RememberMe optional from DTO if available)
            bool rememberMe = dto.RememberMe; // Add this property in RegisterDto if not already
            var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user, rememberMe);


            /////////////////////ADDING CALL TO WGA FOR USER///////////////////////
            ///

            var wgaUrl = _config["Services:FitnessCalculationService"];
    
            var wga = new WgaDto
            {
                UserId = user.Id,
                Goal = user.Goal,
                ActivtyLevel = user.ActivtyLevel,
                Weight = user.Weight,
                Height = user.Height,
                Age = user.Age,
                Gender = user.Gender,
                
            };

           var response= await _httpClient.PostAsJsonAsync($"{wgaUrl}/api/wga", wga);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"WGA Service failed: {error}");
            }


            // 🧾 Return final response
            return new RegisterResponse(
                Success: true,
                Message: "Registration successful",
                UserId: user.Id,
                UserName: user.UserName,
                FirstName: user.FirstName,
                LastName: user.LastName,
                FullName: user.FullName,
                Email: user.Email,
                Gender: user.Gender,
                ActivtyLevel: user.ActivtyLevel,
                Goal: user.Goal,
                Height: user.Height,
                Weight: user.Weight,
                Age: user.Age,
                PhoneNumber: user.PhoneNumber,
                ProfileImageUrl: user.ProfileImageUrl,
                Roles: roles,
                Token: accessToken,
                RefreshToken: refreshToken
            );
           
        }
    }
}
