using Azure;
using Food.Services.ShoppingCartAPI.Dtos;
using Food.Services.ShoppingCartAPI.Messages;
using Food.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        public readonly IShoppingCartRepository _cartRepository;
        private ResponseDto _responseDto;
        private readonly ILogger<CartAPIController> _logger;
        public CartAPIController(ILogger<CartAPIController> logger,
            IShoppingCartRepository cartRepository)
        {
            _logger = logger;
            _cartRepository= cartRepository;
            _responseDto=new ResponseDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                var cartDto = await _cartRepository.GetCartbyUserId(userId);
                _responseDto.Result=cartDto;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Get Cart by user", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpPost("AddCart")]
        public async Task<ResponseDto> AddCart(CartDto cartDto)
        {
            return await  AddUpdateCart(cartDto);
        }

        [HttpPost("UpdateCart")]
        public async Task<ResponseDto> UpdateCart(CartDto cartDto)
        {
            return await AddUpdateCart(cartDto);
        }

        private async Task<ResponseDto> AddUpdateCart(CartDto cartDto)
        {
            try
            {
                var result = await _cartRepository.CreateUpdateCart(cartDto);
                _responseDto.Result = result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Add/update Cart by user", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartId)
        {
            try
            {
                var success = await _cartRepository.RemoveFromCart(cartId);
                _responseDto.Result = success;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Add RemoveCart", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var success = await _cartRepository.ApplyCoupon(cartDto.Header.UserId, 
                    cartDto.Header.CouponCode);
                _responseDto.Result = success;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Add ApplyCoupon", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDto> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                var success = await _cartRepository.RemoveCoupon(userId);
                _responseDto.Result = success;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in RemoveCoupon", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout([FromBody]CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartbyUserId(checkoutHeader.UserId);
                if (cartDto == null)
                {
                    return BadRequest();
                }

                //if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
                //{
                //    CouponDto coupon = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                //    if (checkoutHeader.DiscountTotal != coupon.DiscountAmount)
                //    {
                //        _response.IsSuccess = false;
                //        _response.ErrorMessages = new List<string>() { "Coupon Price has changed, please confirm" };
                //        _response.DisplayMessage = "Coupon Price has changed, please confirm";
                //        return _response;
                //    }
                //}

                checkoutHeader.CartDetails = cartDto.CartDetails;
                //logic to add message to process order.
                //await _messageBus.PublishMessage(checkoutHeader, "checkoutqueue");

                ////rabbitMQ
                //_rabbitMQCartMessageSender.SendMessage(checkoutHeader, "checkoutqueue");
                await _cartRepository.ClearCart(checkoutHeader.UserId);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }
    }
}
