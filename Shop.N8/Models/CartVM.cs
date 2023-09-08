using Shop.Models;

namespace Shop.N8.Models
{
    public class CartVM
    {
        public IEnumerable<Cart> ListOfCart { get; set; } = new List<Cart>();
        public OrderHeader OrderHeader { get; set; } = new OrderHeader();

    }
}
