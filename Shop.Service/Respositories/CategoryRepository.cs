using Microsoft.EntityFrameworkCore;
using Shop.DataAccess.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository.Respositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Category category)
        {
            var categoryDB = _context.Categories.FirstOrDefault(x => x.Id == category.Id);
            if (categoryDB != null)
            {
                categoryDB.CategoryName = category.CategoryName;                
            }
        }
    }
}
