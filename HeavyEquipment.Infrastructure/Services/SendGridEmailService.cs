using HeavyEquipment.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace HeavyEquipment.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(
            IConfiguration config,
            ILogger<SendGridEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }
        public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var apiKey = _config["SendGrid:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("SendGrid API key is missing — skipping email to {Email}", toEmail);
                return;
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(
                _config["SendGrid:FromEmail"] ?? "noreply@heavyhub.eg",
                _config["SendGrid:FromName"] ?? "HeavyHub");
            var to = new EmailAddress(toEmail, toName);
            var message = MailHelper.CreateSingleEmail(from, to, subject, null, htmlBody);

            var response = await client.SendEmailAsync(message);

            if ((int)response.StatusCode >= 400)
                _logger.LogError("SendGrid error {Status} sending to {Email}", response.StatusCode, toEmail);
            else
                _logger.LogInformation("Email sent to {Email} — Subject: {Subject}", toEmail, subject);
        }
    }
}
