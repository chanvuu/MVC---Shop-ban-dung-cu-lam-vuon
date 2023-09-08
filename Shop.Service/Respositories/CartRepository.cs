using Shop.DataAccess.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository.Respositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _context;
        public CartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void DecrementCartItem(Cart cartItem, int count)
        {
            if (cartItem != null)
            {
                cartItem.Count -= count;
            }
        }

        public void IncrementCartItem(Cart cartItem, int count)
        {
            if (cartItem != null)
            {
                cartItem.Count += count;
            }
        }

        public void Update(Cart cart)
        {
            var cartDB = _context.Carts.FirstOrDefault(x => x.Id == cart.Id);
            if (cartDB != null)
            {
                cartDB.ProductId = cart.ProductId;
                cartDB.ApplicationUserId = cart.ApplicationUserId;
                cartDB.Count = cart.Count;
            }
        }
    }
}
