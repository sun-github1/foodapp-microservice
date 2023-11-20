using Food.Web.Models;
using Food.Web.Services;
using Food.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Food.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger , 
            ICartService cartService, 
            IProductService productService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> products = new();

            var result = await _productService.GetAllProductsAsync<ResponseDto>("");
            if (result?.IsSuccess == true)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>
                    (result.Result.ToString());
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto product = new();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);
            if (result?.IsSuccess == true)
            {
                product = JsonConvert.DeserializeObject<ProductDto>
                    (result.Result.ToString());
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return View(product);
        }

        [Authorize]
        [ActionName("Details")]
        [HttpPost]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            CartDto cartDto = new()
            {
                Header = new CartHeaderDto
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetailDto cartDetails = new CartDetailDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId,
                CartHeaderId=cartDto.Header.CartHeaderId,
                //CartHeader=cartDto.Header
            };
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var resp = await _productService.GetProductByIdAsync<ResponseDto>(productDto.ProductId, accessToken);
            if (resp != null && resp.IsSuccess)
            {
                cartDetails.Product = JsonConvert.DeserializeObject<ProductDto>
                    (Convert.ToString(resp.Result));
            }

            List<CartDetailDto> cartDetailDtos = new();
            cartDetailDtos.Add(cartDetails);
            cartDto.CartDetails = cartDetailDtos;

            var addToCartResp = await _cartService.AddToCartAsync<ResponseDto>(cartDto, accessToken);
            if (addToCartResp != null && addToCartResp.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(productDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? 
                HttpContext.TraceIdentifier });
        }
        [Authorize]
        public async Task<IActionResult> Login()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
