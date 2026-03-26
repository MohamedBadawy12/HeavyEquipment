using HeavyEquipment.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace HeavyEquipment.Infrastructure.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TwilioSmsService> _logger;

        public TwilioSmsService(IConfiguration config, ILogger<TwilioSmsService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string toPhone, string message)
        {
            var accountSid = _config["Twilio:AccountSid"];
            var authToken = _config["Twilio:AuthToken"];
            var fromPhone = _config["Twilio:FromPhone"];

            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken))
            {
                _logger.LogWarning("Twilio credentials missing — skipping SMS to {Phone}", toPhone);
                return;
            }

            TwilioClient.Init(accountSid, authToken);

            var msg = await MessageResource.CreateAsync(
                to: new PhoneNumber(toPhone),
                from: new PhoneNumber(fromPhone),
                body: message);

            _logger.LogInformation("SMS sent to {Phone} — SID: {Sid}", toPhone, msg.Sid);
        }
    }
}
