using Food.Web.Models;
using Food.Web.Services.Interfaces;

namespace Food.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory): base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;   
        }
        public async Task<ResponseDto> CreateProductsAsync(ProductDto productDto)
        {
            return await this.SendAsync(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.POST,
                Data = productDto,
                Url=StartingDetails.ProductAPIbase + "/api/product",
                ContentType = StartingDetails.ContentType.MultipartFormData,
                AccessToken=""
            });
        }

        public async Task<ResponseDto> DeleteProductsAsync(int id)
        {
            return await this.SendAsync(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.DELETE,
                Url = StartingDetails.ProductAPIbase + "/api/product/"+id,
                AccessToken = ""
            });
        }

        public async Task<ResponseDto> GetAllProductsAsync()
        {
            return await this.SendAsync(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.GET,
                Url = StartingDetails.ProductAPIbase + "/api/product",
                AccessToken = ""
            });
        }

        public async Task<ResponseDto> GetProductByIdAsync(int id)
        {
            return await this.SendAsync(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.GET,
                Url = StartingDetails.ProductAPIbase + "/api/product/" + id,
                AccessToken = ""
            });
        }

        public async Task<ResponseDto> UpdateProductsAsync(ProductDto productDto)
        {
            return await this.SendAsync(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.PUT,
                Data = productDto,
                Url = StartingDetails.ProductAPIbase + "/api/product",
                ContentType = StartingDetails.ContentType.MultipartFormData,
                AccessToken = ""
            });
        }
    }
}
