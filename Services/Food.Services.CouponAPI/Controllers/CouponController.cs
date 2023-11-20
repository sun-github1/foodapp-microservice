using Food.Services.CouponAPI.Dtos;
using Food.Services.CouponAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        public readonly ICouponRepository _couponRepository;
        private ResponseDto _responseDto;
        private readonly ILogger<CouponController> _logger;
        public CouponController(ILogger<CouponController> logger,
            ICouponRepository couponRepository)
        {
            _logger = logger;
            _couponRepository = couponRepository;
            _responseDto = new ResponseDto();
        }

        [HttpGet("{code}")]
        public async Task<ResponseDto> GetCoupon(string code)
        {
            try
            {
                var cartDto = await _couponRepository.GetCouponByCode(code);
                _responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetCoupon by user", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }
    }
}
