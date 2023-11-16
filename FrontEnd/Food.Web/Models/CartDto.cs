namespace Food.Web.Models
{
    public class CartDto
    {
        public CartHeaderDto Header { get; set; }
        public IEnumerable<CartDetailDto> CartDetails { get; set; }
    }
}
