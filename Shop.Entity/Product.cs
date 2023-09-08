using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }

        [Required]
        public string ProductName { get; set; }

        public string Description { get; set; }

        [Required]
        public int Quatity { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Price { get; set; }

        [ValidateNever]
        [Display(Name = "Photo")]
        public string? ImageUrl { get; set; }

        // Navigation property to related Category entity
        [ValidateNever]
        public Category Category { get; set; }

    }
}