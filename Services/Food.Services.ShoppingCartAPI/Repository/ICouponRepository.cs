using Food.Services.ShoppingCartAPI.Dtos;

namespace Food.Services.ShoppingCartAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string couponName);
    }
}
