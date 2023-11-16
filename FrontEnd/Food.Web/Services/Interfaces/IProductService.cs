using Food.Web.Models;
using System.Threading.Tasks;

namespace Food.Web.Services.Interfaces
{
    public interface IProductService
    {
        Task<T> GetAllProductsAsync<T>(string token);
        Task<T> GetProductByIdAsync<T>(int id, string token);
        Task<T> CreateProductsAsync<T>(ProductDto productDto, string token);
        Task<T> UpdateProductsAsync<T>(ProductDto productDto, string token);
        Task<T> DeleteProductsAsync<T>(int id, string token);
    }
}
