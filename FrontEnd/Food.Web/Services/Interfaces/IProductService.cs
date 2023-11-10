using Food.Web.Models;
using System.Threading.Tasks;

namespace Food.Web.Services.Interfaces
{
    public interface IProductService
    {
        Task<ResponseDto> GetAllProductsAsync();
        Task<ResponseDto> GetProductByIdAsync(int id);
        Task<ResponseDto> CreateProductsAsync(ProductDto productDto);
        Task<ResponseDto> UpdateProductsAsync(ProductDto productDto);
        Task<ResponseDto> DeleteProductsAsync(int id);
    }
}
