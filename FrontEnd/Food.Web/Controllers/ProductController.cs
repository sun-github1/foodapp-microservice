using Food.Web.Models;
using Food.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Food.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<ActionResult> ProductIndex()
        {
            List<ProductDto> products = new();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await _productService.GetAllProductsAsync<ResponseDto>(accessToken);
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

        public async Task<ActionResult> ProductEdit(int productId)
        {
            ProductDto product = new();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await _productService.GetProductByIdAsync<ResponseDto>
                (productId, accessToken);
            if (result?.IsSuccess == true)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(result.Result.ToString());
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProductEdit(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ProductDto product = new();
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var result = await _productService.UpdateProductsAsync<ResponseDto>
                    (productDto, accessToken);
                if (result?.IsSuccess == true)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = result.Message;
                }
            }
            return View(productDto);
        }
        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProductCreate(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ProductDto product = new();
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var result = await _productService.CreateProductsAsync
                    <ResponseDto>(productDto, accessToken);
                if (result?.IsSuccess == true)
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = result.Message;
                }
            }
            return View(productDto);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            ResponseDto? response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);

            if (response != null && response.IsSuccess)
            {
                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto productDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            ResponseDto? response = await _productService.DeleteProductsAsync<ResponseDto>(productDto.ProductId, accessToken);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDto);
        }
    }
}
