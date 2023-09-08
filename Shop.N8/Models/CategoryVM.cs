using Shop.Models;

namespace Shop.N8.Models
{
    public class CategoryVM
    {
        public Category Category { get; set; } = new Category();
        public IEnumerable<Category> categories { get; set; } = new List<Category>();
    }
}
