using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegTeachApi.Data;
using RegTeachApi.Data.Models;
using RegTeachApi.DTOs;
using RegTeachApi.Helpers;
using RegTeachApi.Services;

namespace RegTeachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly APPDBC _db;
        private readonly JwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthController(
            APPDBC db,
            JwtService jwtService,
            IEmailService emailService)
        {
            _db = db;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterDto dto)
        {
            if (!PasswordValidator.IsValid(dto.Password))
            {
                return BadRequest(
                    "Password policy failed.");
            }

            if (await _db.RegTeachUsers.AnyAsync(
                x => x.Username == dto.Username))
            {
                return BadRequest(
                    "Username already exists.");
            }

            if (await _db.RegTeachUsers.AnyAsync(
                x => x.Email == dto.Email))
            {
                return BadRequest(
                    "Email already exists.");
            }

            var code = Random.Shared
                .Next(100000, 999999)
                .ToString();

            var user = new RegTeachUsers
            {
                Username = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,

                Password =
                    BCrypt.Net.BCrypt
                    .HashPassword(dto.Password),

                EmailVerificationCode = code,

                VerificationCodeExpiresAt =
                    DateTime.UtcNow.AddMinutes(10)
            };

            _db.RegTeachUsers.Add(user);

            await _db.SaveChangesAsync();

            await _emailService
                .SendVerificationCodeAsync(
                    user.Email,
                    code);

            return Ok(
                "Verification code sent.");
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(
            VerifyEmailDto dto)
        {
            var user = await _db.RegTeachUsers
                .FirstOrDefaultAsync(
                    x => x.Email == dto.Email);

            if (user == null)
                return NotFound();

            if (user.EmailVerificationCode
                != dto.Code)
            {
                return BadRequest(
                    "Invalid code.");
            }

            if (user.VerificationCodeExpiresAt
                < DateTime.UtcNow)
            {
                return BadRequest(
                    "Code expired.");
            }

            user.IsEmailVerified = true;
            user.EmailVerificationCode = null;
            user.VerificationCodeExpiresAt = null;

            await _db.SaveChangesAsync();

            return Ok("Email verified.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginDto dto)
        {
            var user = await _db.RegTeachUsers
                .FirstOrDefaultAsync(
                    x => x.Email == dto.Email);

            if (user == null)
                return Unauthorized();

            if (user.LockoutEnd.HasValue &&
                user.LockoutEnd > DateTime.UtcNow)
            {
                return BadRequest(
                    "Account locked.");
            }

            if (!BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.Password))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd =
                        DateTime.UtcNow
                        .AddMinutes(15);
                }

                await _db.SaveChangesAsync();

                return Unauthorized();
            }

            if (!user.IsEmailVerified)
            {
                return BadRequest(
                    "Verify email first.");
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            var token =
                _jwtService.GenerateToken(user);

            var refreshToken =
                _jwtService.GenerateRefreshToken();

            user.RefreshToken =
                refreshToken;

            user.RefreshTokenExpiryTime =
                DateTime.UtcNow.AddDays(7);

            await _db.SaveChangesAsync();

            return Ok(new
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult>
            RefreshToken(
            RefreshTokenDto dto)
        {
            var user = await _db.RegTeachUsers
                .FirstOrDefaultAsync(
                    x => x.RefreshToken ==
                         dto.RefreshToken);

            if (user == null)
                return Unauthorized();

            if (user.RefreshTokenExpiryTime
                < DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var token =
                _jwtService.GenerateToken(user);

            var newRefreshToken =
                _jwtService.GenerateRefreshToken();

            user.RefreshToken =
                newRefreshToken;

            user.RefreshTokenExpiryTime =
                DateTime.UtcNow.AddDays(7);

            await _db.SaveChangesAsync();

            return Ok(new
            {
                Token = token,
                RefreshToken =
                    newRefreshToken
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult>
            ForgotPassword(
            ForgotPasswordDto dto)
        {
            var user = await _db.RegTeachUsers
                .FirstOrDefaultAsync(
                    x => x.Email == dto.Email);

            if (user == null)
            {
                return Ok(
                    "If account exists, code sent.");
            }

            var code = Random.Shared
                .Next(100000, 999999)
                .ToString();

            user.PasswordResetCode = code;

            user.PasswordResetCodeExpiresAt =
                DateTime.UtcNow.AddMinutes(10);

            await _db.SaveChangesAsync();

            await _emailService
                .SendPasswordResetCodeAsync(
                    user.Email,
                    code);

            return Ok(
                "Reset code sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult>
            ResetPassword(
            ResetPasswordDto dto)
        {
            var user = await _db.RegTeachUsers
                .FirstOrDefaultAsync(
                    x => x.Email == dto.Email);

            if (user == null)
                return BadRequest();

            if (user.PasswordResetCode
                != dto.Code)
            {
                return BadRequest(
                    "Invalid code.");
            }

            if (user.PasswordResetCodeExpiresAt
                < DateTime.UtcNow)
            {
                return BadRequest(
                    "Code expired.");
            }

            if (dto.NewPassword
                != dto.ConfirmPassword)
            {
                return BadRequest(
                    "Passwords do not match.");
            }

            user.Password =
                BCrypt.Net.BCrypt
                .HashPassword(
                    dto.NewPassword);

            user.PasswordResetCode = null;
            user.PasswordResetCodeExpiresAt = null;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _db.SaveChangesAsync();

            return Ok(
                "Password reset successful.");
        }
    }
}
