using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shop.Models;

namespace Shop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [ValidateNever]
        public Product Product { get; set; }
        [ValidateNever]
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public int Count { get; set; }
        public int SelectedQuantity { get; set; }

    }
}
