using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.Models;

namespace Shop.N8.Models
{
    public class ProductVM
    {
        public Product Product { get; set; } = new Product();
        [ValidateNever]
        public IEnumerable<Product> Products { get; set;} = new List<Product>();
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
