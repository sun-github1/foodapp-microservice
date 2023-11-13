using Food.Services.ProductAPI.Dtos;
using Food.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private ResponseDto _responseDto;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductAPIController> _logger;
        public ProductAPIController(ILogger<ProductAPIController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            this._responseDto = new ResponseDto();
        }

        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
                var products = await _productRepository.GetProducts();
                _responseDto.Result = products;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Get Products", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ResponseDto> Get(int id)
        
        {
            try
            {
                var product = await _productRepository.GetProductById(id);
                _responseDto.Result = product;
                _responseDto.Message = product != null ? null : "No product found for given criteria";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Get Product for id {id}", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] ProductDto productDto)
        {
            try
            {
                var createdProduct = await _productRepository.CreateUpdateProduct(productDto);
                _responseDto.Message = createdProduct != null ? "Product is created successfully" : null;
                return CreatedAtAction(nameof(Get), new { id = createdProduct.ProductId }, _responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Post", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpPut]
        public async Task<ResponseDto> Put([FromBody] ProductDto productDto)
        {
            try
            {
                var updatedProduct = await _productRepository.CreateUpdateProduct(productDto);
                _responseDto.Message = updatedProduct != null ? "Product updated successfully" : null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Put", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                var success = await _productRepository.DeleteProduct(id);
                _responseDto.IsSuccess = success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Delete", ex);
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string> { ex.Message };
            }
            return _responseDto;
        }
    }
}
