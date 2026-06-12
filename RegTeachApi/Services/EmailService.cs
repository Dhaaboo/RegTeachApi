using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace RegTeachApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(
            IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendVerificationCodeAsync(
            string email,
            string code)
        {
            var message = new MimeMessage();

            message.From.Add(
                MailboxAddress.Parse(
                    _settings.SenderEmail));

            message.To.Add(
                MailboxAddress.Parse(email));

            message.Subject =
                "Email Verification";

            message.Body =
                new TextPart("html")
                {
                    Text =
                    $"""
                <h2>Email Verification</h2>
                <p>Your verification code:</p>
                <h1>{code}</h1>
                <p>Expires in 10 minutes.</p>
                """
                };

            await SendEmailAsync(message);
        }

        public async Task SendPasswordResetCodeAsync(
            string email,
            string code)
        {
            var message = new MimeMessage();

            message.From.Add(
                MailboxAddress.Parse(
                    _settings.SenderEmail));

            message.To.Add(
                MailboxAddress.Parse(email));

            message.Subject =
                "Password Reset";

            message.Body =
                new TextPart("html")
                {
                    Text =
                    $"""
                <h2>Password Reset</h2>
                <p>Your reset code:</p>
                <h1>{code}</h1>
                <p>Expires in 10 minutes.</p>
                """
                };

            await SendEmailAsync(message);
        }

        private async Task SendEmailAsync(
            MimeMessage message)
        {
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.SmtpServer,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _settings.SenderEmail,
                _settings.SenderPassword);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}
