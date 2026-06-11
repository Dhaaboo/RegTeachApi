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
    }
}
