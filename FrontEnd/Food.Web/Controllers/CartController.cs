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
        private readonly ICouponService _couponService;

        public CartController(ILogger<CartController> logger,
            ICartService cartService,
            IProductService productService,
            ICouponService couponService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        [Authorize]
        [HttpGet]
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
                if(!string.IsNullOrEmpty(cartDto.Header.CouponCode))
                {
                    var couponResult = await _couponService.GetCoupon<ResponseDto>(cartDto.Header.CouponCode, 
                        accessToken);
                    if (couponResult?.IsSuccess == true)
                    {
                        var coupon = JsonConvert.DeserializeObject<CouponDto>(couponResult.Result?.ToString());
                        cartDto.Header.DiscountTotal = coupon.DiscountAmount;
                        //cartDto.Header.CouponCode = coupon.CouponCode;
                    }
                }

                foreach(var item in cartDto.CartDetails)
                {
                    cartDto.Header.OrderTotal += (item.Product.Price * item.Count);
                }
                cartDto.Header.OrderTotal = cartDto.Header.OrderTotal - cartDto.Header.DiscountTotal;
            }

            return cartDto;
        }
        [HttpGet("Remove")]
        public async Task<IActionResult> Remove(int cartDetailsId)
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

        [HttpPost]
        //[ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon([FromForm] CartDto cartDto)
        {
            //string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, accessToken);
            if (response?.IsSuccess == true)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();

        }
        [HttpPost("RemoveCoupon")]
        //[ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon([FromForm]CartDto cartDto)
        {
            //string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto.Header.UserId, accessToken);
            if (response?.IsSuccess == true)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();

        }

        [Authorize]
        [HttpGet("CheckOut")]
        public async Task<ActionResult> CheckOut()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromForm]CartDto cartDto)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.Checkout<ResponseDto>(cartDto.Header, accessToken);
                if (response==null && !response.IsSuccess)
                {
                    TempData["Error"] = response.Message;
                    return RedirectToAction(nameof(Checkout));
                }
                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception ex)
            {
                return View(cartDto);
            }
        }
        [HttpGet("Confirmation")]
        public async Task<ActionResult> Confirmation()
        {
            return View();
        }
    }
}
