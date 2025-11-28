using MediatR;
using AuthenticationService.Contarcts;
using AuthenticationService.Dtos;

namespace AuthenticationService.Features.Auth.UpdateUserProfile
{
    public class UpdateUserProfileOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IImageHelper _imageHelper;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        public UpdateUserProfileOrchestrator(IMediator mediator, IImageHelper imageHelper, IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _mediator = mediator;
            _imageHelper = imageHelper;
        }

        public async Task<UpdateUserProfileResponse> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequest request, string? currentImageUrl = null)
        {
            string? imageUrl = currentImageUrl;

            if (request.ProfileImage is not null && request.ProfileImage.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(currentImageUrl))
                {
                    _imageHelper.DeleteImage(currentImageUrl);
                }

                // Save new image
                imageUrl = await _imageHelper.SaveImageAsync(request.ProfileImage, "Users");
            }

            var command = new UpdateUserProfileCommand(
    userId,
    request.FirstName,
    request.LastName,
    request.PhoneNumber,
    imageUrl,             // ProfileImage
    request.Goal,         // Goal
    request.activtyLevel, // ActivtyLevel
    request.Height,       // Height
    request.Weight

);

            var wgaUrl = _config["Services:FitnessCalculationService"];
            var httpClient = new HttpClient();

            var wga = new WgaDto
            {
                UserId = command.UserId,
                Goal = command.Goal,
                ActivtyLevel = command.activtyLevel, // ⚡ Use value from command
                Weight = command.Weight,
                Height = command.Height,

            };
            var response = await httpClient.PutAsJsonAsync($"{wgaUrl}/api/wga/{userId}", wga);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"WGA Service update failed: {error}");
            }
            return await _mediator.Send(command);




        }
    }
}
