namespace RegTeachApi.DTOs
{
    using Microsoft.AspNetCore.Http;

    public class UpdateProfilePictureDto
    {
        public IFormFile ProfilePicture { get; set; } = default!;
    }
}
