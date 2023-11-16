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
        public async Task<T> CreateProductsAsync<T>(ProductDto productDto, string token)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.POST,
                Data = productDto,
                Url=StartingDetails.ProductAPIbase + "/api/product",
                ContentType = StartingDetails.ContentType.MultipartFormData,
                AccessToken= token
            });
        }

        public async Task<T> DeleteProductsAsync<T>(int id, string token)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.DELETE,
                Url = StartingDetails.ProductAPIbase + "/api/product/"+id,
                AccessToken = token
            });
        }

        public async Task<T> GetAllProductsAsync<T>(string token)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.GET,
                Url = StartingDetails.ProductAPIbase + "/api/product",
                AccessToken = token
            });
        }

        public async Task<T> GetProductByIdAsync<T>(int id, string token)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.GET,
                Url = StartingDetails.ProductAPIbase + "/api/product/" + id,
                AccessToken = token
            });
        }

        public async Task<T> UpdateProductsAsync<T>(ProductDto productDto, string token)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.PUT,
                Data = productDto,
                Url = StartingDetails.ProductAPIbase + "/api/product",
                ContentType = StartingDetails.ContentType.MultipartFormData,
                AccessToken = token
            });
        }
    }
}
