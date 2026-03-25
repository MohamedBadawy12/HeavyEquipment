using HeavyEquipment.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace HeavyEquipment.Infrastructure.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly IConfiguration _config;

        public TwilioSmsService(IConfiguration config) => _config = config;

        public async Task<bool> SendSmsAsync(string phoneNumber, string message, CancellationToken ct = default)
        {
            var sid = _config["Twilio:AccountSid"];
            var token = _config["Twilio:AuthToken"];
            var from = _config["Twilio:FromNumber"];

            try
            {
                TwilioClient.Init(sid, token);
                var response = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(from),
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );

                return response.Status != MessageResource.StatusEnum.Failed;
            }
            catch
            {
                return false;
            }
        }
    }
}
