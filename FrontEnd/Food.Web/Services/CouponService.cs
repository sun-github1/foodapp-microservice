using Food.Web.Models;
using Food.Web.Services.Interfaces;

namespace Food.Web.Services
{
    public class CouponService : BaseService, ICouponService
    {
        public CouponService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            
        }
        public async Task<T> GetCoupon<T>(string couponCode, string token = null)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.GET,
                Url = StartingDetails.CouponAPIAPIbase + "/api/coupon/" + couponCode,
                AccessToken = token
            });
        }
    }
}
