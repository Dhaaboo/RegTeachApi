using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegTeachApi.Data;
using RegTeachApi.DTOs;
using RegTeachApi.Helpers;
using System.Security.Claims;

namespace RegTeachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly APPDBC _db;

        public UsersController(APPDBC db)
        {
            _db = db;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            var user = await _db.RegTeachUsers.FindAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Role,
                user.CreatedAt,
                user.UpdatedAt,
                HasProfilePicture =
                    user.ProfilePicture != null
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult>
            UpdateProfile(
            UpdateProfileDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value);

            var user = await _db.RegTeachUsers
                .FindAsync(userId);

            if (user == null)
                return NotFound();

            var usernameExists =
                await _db.RegTeachUsers.AnyAsync(
                    x => x.Username ==
                         dto.Username &&
                         x.Id != userId);

            if (usernameExists)
            {
                return BadRequest(
                    "Username already exists.");
            }

            var emailExists =
                await _db.RegTeachUsers.AnyAsync(
                    x => x.Email ==
                         dto.Email &&
                         x.Id != userId);

            if (emailExists)
            {
                return BadRequest(
                    "Email already exists.");
            }

            user.Username = dto.Username;
            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(
                "Profile updated.");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult>
            ChangePassword(
            ChangePasswordDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value);

            var user = await _db.RegTeachUsers
                .FindAsync(userId);

            if (user == null)
                return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(
                dto.CurrentPassword,
                user.Password))
            {
                return BadRequest(
                    "Current password incorrect.");
            }

            if (dto.NewPassword !=
                dto.ConfirmPassword)
            {
                return BadRequest(
                    "Passwords do not match.");
            }

            if (!PasswordValidator.IsValid(
                dto.NewPassword))
            {
                return BadRequest(
                    "Password policy failed.");
            }

            if (BCrypt.Net.BCrypt.Verify(
                dto.NewPassword,
                user.Password))
            {
                return BadRequest(
                    "New password cannot be the same as current password.");
            }

            user.Password =
                BCrypt.Net.BCrypt
                .HashPassword(
                    dto.NewPassword);

            user.UpdatedAt =
                DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(
                "Password changed successfully.");
        }

        [HttpPut("profile-picture")]
        public async Task<IActionResult>
            UpdateProfilePicture(
            [FromForm]
        UpdateProfilePictureDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value);

            var user = await _db.RegTeachUsers
                .FindAsync(userId);

            if (user == null)
                return NotFound();

            if (dto.ProfilePicture == null)
            {
                return BadRequest(
                    "Image required.");
            }

            if (dto.ProfilePicture.Length >
                FileSettings.MaxProfilePictureSize)
            {
                return BadRequest(
                    "Image cannot exceed 3 MB.");
            }

            var extension =
                Path.GetExtension(
                    dto.ProfilePicture.FileName)
                .ToLowerInvariant();

            var allowedExtensions =
                new[]
                {
                ".jpg",
                ".jpeg",
                ".png"
                };

            if (!allowedExtensions
                .Contains(extension))
            {
                return BadRequest(
                    "Only JPG and PNG allowed.");
            }

            var allowedTypes =
                new[]
                {
                "image/jpeg",
                "image/png"
                };

            if (!allowedTypes.Contains(
                dto.ProfilePicture.ContentType))
            {
                return BadRequest(
                    "Invalid image format.");
            }

            using var ms =
                new MemoryStream();

            await dto.ProfilePicture
                .CopyToAsync(ms);

            user.ProfilePicture =
                ms.ToArray();

            user.UpdatedAt =
                DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(
                "Profile picture updated.");
        }

        [HttpGet("profile-picture")]
        public async Task<IActionResult>
            GetProfilePicture()
        {
            var userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value);

            var user = await _db.RegTeachUsers
                .FindAsync(userId);

            if (user == null)
                return NotFound();

            if (user.ProfilePicture != null)
            {
                return File(
                    user.ProfilePicture,
                    "image/jpeg");
            }

            return NotFound(
                "No profile picture.");
        }

        [HttpDelete("profile-picture")]
        public async Task<IActionResult>
            DeleteProfilePicture()
        {
            var userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value);

            var user = await _db.RegTeachUsers
                .FindAsync(userId);

            if (user == null)
                return NotFound();

            user.ProfilePicture = null;
            user.UpdatedAt =
                DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(
                "Profile picture deleted.");
        }
    }
}
