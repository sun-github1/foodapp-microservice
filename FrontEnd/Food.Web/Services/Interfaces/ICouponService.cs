using Food.Web.Models;

namespace Food.Web.Services.Interfaces
{
    public interface ICouponService
    {
        Task<T> GetCoupon<T>(string couponCode, string token = null);
    }
}
