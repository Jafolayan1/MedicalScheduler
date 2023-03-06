using MedicalSchedular.Models;

namespace MedicalScheduler.Service
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest, string body);
    }
}
