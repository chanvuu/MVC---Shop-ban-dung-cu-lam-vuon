using Shop.DataAccess.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository.Respositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Product product)
        {
            var productDB = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (productDB != null)
            {
                productDB.ProductName = product.ProductName;
                productDB.Description = product.Description;
                productDB.Price = product.Price;
                productDB.Quatity = product.Quatity;
                productDB.CategoryId = product.CategoryId;
                if (productDB.ImageUrl != null)
                {
                    productDB.ImageUrl = product.ImageUrl;
                }

            }
        }
    }
}
