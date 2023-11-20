using AutoMapper;
using Food.Services.ShoppingCartAPI.Data;
using Food.Services.ShoppingCartAPI.Dtos;
using Food.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Food.Services.ShoppingCartAPI.Repository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;
        public ShoppingCartRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

      
        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(
               u => u.UserId == userId);

            if (cartHeaderFromDb != null)
            {
                var cartDeatilsFromDb = _db.CartDetails.Where(
                u => u.CartHeaderId==cartHeaderFromDb.CartHeaderId);
                    _db.RemoveRange(cartDeatilsFromDb);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            Cart cart = _mapper.Map<CartDto, Cart>(cartDto);
            //check if product exists in DB, if not create it
            var productInDb = _db.Products.AsNoTracking().FirstOrDefault(p => p.ProductId == 
                cart.CartDetails.FirstOrDefault().ProductId);
            if (productInDb == null)
            {
                await _db.Products.AddAsync(cart.CartDetails.FirstOrDefault().Product);
                await _db.SaveChangesAsync();
            }

            //if header is null create a new header
            var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(
                u => u.UserId ==  cart.Header.UserId);
            if (cartHeaderFromDb==null)
            {
                await _db.CartHeaders.AddAsync(cart.Header);
                await _db.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.Header.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _db.SaveChangesAsync();
            }
            else
            {
                //if header is not null
                //check if details has same product
                 var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                    u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if (cartDetailsFromDb == null)
                {
                    //create details
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //update the count / cart details
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                    cart.CartDetails.FirstOrDefault().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                    _db.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
            }
            return _mapper.Map<Cart, CartDto>(cart);
        }

        public async Task<CartDto> GetCartbyUserId(string userId)
        {
            Cart cart = new Cart() { 
                Header= await _db.CartHeaders.FirstOrDefaultAsync(a=>a.UserId==userId)
            };

            cart.CartDetails = _db.CartDetails
                .Where(c=>c.CartHeaderId==cart.Header.CartHeaderId).Include(u=>u.Product);

            return _mapper.Map<Cart, CartDto>(cart);
        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                CartDetail cartDetail = await _db.CartDetails.FirstOrDefaultAsync
                    (c => c.CartDetailsId == cartDetailsId);

                int totalCountOfCartItems = _db.CartDetails.Where
                    (c => c.CartDetailsId == cartDetailsId).Count();

                _db.CartDetails.Remove(cartDetail);
                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                        .FirstOrDefaultAsync(h => h.CartHeaderId == cartDetail.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            cartFromDb.CouponCode = couponCode;
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            cartFromDb.CouponCode = "";
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
