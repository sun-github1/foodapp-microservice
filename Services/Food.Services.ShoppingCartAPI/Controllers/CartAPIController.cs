using Food.Services.ShoppingCartAPI.Dtos;
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

    }
}
