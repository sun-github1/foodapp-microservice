using Food.Services.OrderAPI.Data;
using Food.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Food.Services.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;
        //protected IMapper _mapper;
        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<OrderHeader> AddOrder(OrderHeader orderHeader)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            var neworder = await _db.OrderHeaders.AddAsync(orderHeader);
            await _db.SaveChangesAsync();
            return neworder.Entity;
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            var existingorder = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.OrderHeaderId == orderHeaderId);
            if (existingorder != null)
            {
                existingorder.PaymentStatus = paid;
                await _db.SaveChangesAsync();
                //return true;
            }
            //return false;

        }
    }
}
