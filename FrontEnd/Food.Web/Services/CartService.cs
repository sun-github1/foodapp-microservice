﻿using Food.Web.Models;
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
                Url = StartingDetails.ShoppingCartAPIAPIbase + "/api/cart/getcart/"+userId,
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
        public async Task<T> ApplyCoupon<T>(CartDto cartDto, string token = null)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.POST,
                Data = cartDto,
                Url = StartingDetails.ShoppingCartAPIAPIbase + "/api/cart/applycoupon",
                ContentType = StartingDetails.ContentType.MultipartFormData,
                AccessToken = token
            });
        }
        public async Task<T> RemoveCoupon<T>(string userId, string token = null)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.POST,
                Data = userId,
                Url = StartingDetails.ShoppingCartAPIAPIbase + "/api/cart/RemoveCoupon",
                AccessToken = token
            });
        }

        public async Task<T> Checkout<T>(CartHeaderDto cartHeader, string token = null)
        {
            return await this.SendAsync<T>(new RequestDto()
            {
                ApiType = StartingDetails.ApiType.POST,
                Data = cartHeader,
                Url = StartingDetails.ShoppingCartAPIAPIbase + "/api/cart/checkout",
                ContentType = StartingDetails.ContentType.MultipartFormData,
                AccessToken = token
            });
        }
    }
}
