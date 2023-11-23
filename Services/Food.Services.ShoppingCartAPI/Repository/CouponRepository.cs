using AutoMapper;
using Food.Services.ShoppingCartAPI.Data;
using Food.Services.ShoppingCartAPI.Dtos;
using Newtonsoft.Json;

namespace Food.Services.ShoppingCartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _httpClient;
        public CouponRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CouponDto> GetCoupon(string couponName)
        {
            var response = await _httpClient.GetAsync($"/api/coupon/{couponName}");
            var apicontent= await response.Content.ReadAsStringAsync();
            var resp= JsonConvert.DeserializeObject<ResponseDto>(apicontent);
            if(resp?.IsSuccess==true)
            {
                return JsonConvert.DeserializeObject<CouponDto>(resp.Result.ToString());
            }
            return new CouponDto();
        }
    }
}
