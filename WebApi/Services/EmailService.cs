using Core.Configurations;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace WebApi.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string body, string username, string email, string subject);
    }

    public class EmailService : IEmailService
    {
        private readonly Mail _mailOptions;

        public EmailService(IOptions<Mail> mailOptions)
        {
            _mailOptions = mailOptions.Value;
        }

        public async Task<bool> SendEmail(
            string body,
            string username,
            string email,
            string subject
        )
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_mailOptions.DisplayName, _mailOptions.Email));
                message.To.Add(new MailboxAddress(username, email));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_mailOptions.Host, _mailOptions.Port, false);
                    await client.AuthenticateAsync(_mailOptions.Email, _mailOptions.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
