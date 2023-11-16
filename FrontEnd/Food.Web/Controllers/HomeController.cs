using Food.Web.Models;
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
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger , IProductService productService)
        {
            _logger = logger;
            _productService = productService;
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
