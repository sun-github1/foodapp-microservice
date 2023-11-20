using System.ComponentModel.DataAnnotations;

namespace Food.Services.ShoppingCartAPI.Dtos
{
    public class CartHeaderDto
    {
        public CartHeaderDto()
        {
            CouponCode = "Dummy";
            CartHeaderId = 0;
        }
        public int CartHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
    }
}
