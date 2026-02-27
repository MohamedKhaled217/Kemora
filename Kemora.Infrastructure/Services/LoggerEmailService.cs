using Kemora.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Services
{
    public class LoggerEmailService : IEmailService
    {
        private readonly ILogger<LoggerEmailService> _logger;

        public LoggerEmailService(ILogger<LoggerEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation("Sending email to {To} with subject {Subject}. Body: {Body}", to, subject, body);
            return Task.CompletedTask;
        }
    }
}
