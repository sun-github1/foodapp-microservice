using Food.Web.Models;
using Food.Web.Services.Interfaces;

namespace Food.Web.Services
{
    public class CartService : BaseService, ICartService
    {
        public CartService(IHttpClientFactory httpClientFactory): base(httpClientFactory)
        { }

        public async Task<T> AddToCartAsync<T>(CartDto cartDto, string token = null)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.POST,
                Data = cartDto,
                Url = StartingDetails.ShoppingCartAPIAPIbase + "/api/cart/addcart",
                ContentType = StartingDetails.ContentType.MultipartFormData,
                AccessToken = token
            });
        }

        public async Task<T> GetCartbyUserId<T>(string userId, string token = null)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.GET,
                Data = userId,
                Url = StartingDetails.ShoppingCartAPIAPIbase + "/api/cart/GetCart",
                ContentType = StartingDetails.ContentType.MultipartFormData,
                AccessToken = token
            });
        }

        public async Task<T> RemoveFromCartAsync<T>(int cartId, string token = null)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.POST,
                Data = cartId,
                Url = StartingDetails.ShoppingCartAPIAPIbase + "/api/cart/RemoveCart",
                ContentType = StartingDetails.ContentType.MultipartFormData,
                AccessToken = token
            });
        }

        public async Task<T> UpdateCartAsync<T>(CartDto cartDto, string token = null)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.POST,
                Data = cartDto,
                Url = StartingDetails.ShoppingCartAPIAPIbase + "/api/cart/UpdateCart",
                ContentType = StartingDetails.ContentType.MultipartFormData,
                AccessToken = token
            });
        }
    }
}
