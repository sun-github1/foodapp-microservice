using Food.Services.ShoppingCartAPI.Dtos;

namespace Food.Services.ShoppingCartAPI.Repository
{
    public interface IShoppingCartRepository
    {
        Task<CartDto> GetCartbyUserId(string userId);
        Task<CartDto> CreateUpdateCart(CartDto cartDto);
        Task<bool> RemoveFromCart(int cartDetailsId);
        Task<bool> ClearCart(string userId);
    }
}
