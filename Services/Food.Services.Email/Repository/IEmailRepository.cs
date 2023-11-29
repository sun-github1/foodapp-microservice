using Food.Services.Email.Messages;
using Food.Services.Email.Models;

namespace Food.Services.Email.Repository
{
    public interface IEmailRepository
    {
        Task SendAndLogEmail(UpdatePaymentResultMessage message);
    }
}
