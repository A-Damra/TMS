using MailKit.Net.Smtp;
using MimeKit;


namespace TMS.Services
{
    public class EmailSender : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmail(string title, string email, string body)
        {
            var message = new MimeMessage();

            message.From.Add(MailboxAddress.Parse(_config["EmailSettings:username"]));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = title;

            var text = new BodyBuilder
            {
                HtmlBody = body
            };

            message.Body = text.ToMessageBody();


            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["EmailSettings:host"], int.Parse(_config["EmailSettings:port"]), true);
            await smtp.AuthenticateAsync(_config["EmailSettings:username"], _config["EmailSettings:password"]);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
