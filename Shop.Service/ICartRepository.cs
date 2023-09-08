using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository
{
    public interface ICartRepository : IRepository<Cart>
    {
        void IncrementCartItem(Cart cartItem, int count);
        void DecrementCartItem(Cart cartItem, int count);
        void Update(Cart cart);


    }
}
