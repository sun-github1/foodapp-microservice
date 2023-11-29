using Food.Services.Email.Data;
using Food.Services.Email.Messages;
using Food.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Food.Services.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;
        //protected IMapper _mapper;
        public EmailRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            //implement an email sender or call some other class library
            EmailLog emailLog = new EmailLog()
            {
                Email = message.Email,
                EmailSent = DateTime.Now,
                Log = $"Order - {message.OrderId} has been created successfully."
            };

            await using var _db = new ApplicationDbContext(_dbContext);
            _db.EmailLogs.Add(emailLog);
            await _db.SaveChangesAsync();
        }

    }
}
