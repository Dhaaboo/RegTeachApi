namespace RegTeachApi.Data.Models
{
    public class RegTeachUsers
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public bool IsEmailVerified { get; set; }

        public string? EmailVerificationCode { get; set; }

        public DateTime? VerificationCodeExpiresAt { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public int FailedLoginAttempts { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public string? PasswordResetCode { get; set; }

        public DateTime? PasswordResetCodeExpiresAt { get; set; }
    }
}
