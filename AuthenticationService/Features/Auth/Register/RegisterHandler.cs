using MediatR;
using Microsoft.AspNetCore.Identity;
using AuthenticationService.Contarcts;
using AuthenticationService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AuthenticationService.Features.Auth.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IMemoryCache _cache;
        private readonly DbContext _db; // Inject your Identity DbContext

        public RegisterHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ITokenService tokenService,
            IMemoryCache cache,
            DbContext dbContext // pass your ApplicationDbContext
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _cache = cache;
            _db = dbContext;
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var dto = request.RegisterDto;

            // ========== 🔒 Start Transaction ==========
            using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            // Check existing user
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new ApplicationException("Email already registered.");

            // Create user object
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
                Gender = dto.Gender,
                ActivtyLevel = dto.ActivtyLevel,
                Goal = dto.Goal,
                CreatedAt = DateTime.UtcNow
            };

            // Insert user
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ApplicationException($"User creation failed: {errors}");
            }

            // =============== ⭐ Cached Role Check ===============
            string userRoleKey = "role_exists_user";

            bool roleExists = await _cache.GetOrCreateAsync(userRoleKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return await _roleManager.RoleExistsAsync("User");
            });

            if (roleExists)
                await _userManager.AddToRoleAsync(user, "User");

            // Commit transaction
            await transaction.CommitAsync();

            // Get roles from user (only one DB call but cached later in TokenService)
            var roles = await _userManager.GetRolesAsync(user);

            // Generate tokens
            var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user, dto.RememberMe);

            // Response
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
