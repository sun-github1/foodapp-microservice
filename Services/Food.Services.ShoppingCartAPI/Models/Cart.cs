namespace Food.Services.ShoppingCartAPI.Models
{
    public class Cart
    {
        public CartHeader Header { get; set; }
        public IEnumerable<CartDetail> CartDetails { get; set; }
    }
}
