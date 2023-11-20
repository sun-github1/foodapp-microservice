using Food.Web.Models;
using Food.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Food.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger,
            ICartService cartService,
            IProductService productService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        [Authorize]
        public async Task<ActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.GetCartbyUserId<ResponseDto>(userId, accessToken);

            CartDto cartDto = new();
            if (response?.IsSuccess==true)
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(response.Result?.ToString());
            }

            if(cartDto.Header!=null)
            {
                foreach(var item in cartDto.CartDetails)
                {
                    cartDto.Header.OrderTotal += (item.Product.Price * item.Count);
                }
            }

            return cartDto;
        }
        [HttpGet("Remove")]
        public async Task<ActionResult> Remove(int cartDetailsId)
        {
            //string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailsId, accessToken);
            if (response?.IsSuccess == true)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();

        }

    }
}
