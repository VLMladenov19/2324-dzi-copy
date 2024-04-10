using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAR.Data.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; }

        public string? Desc { get; set; }

        [Required(ErrorMessage = "Product price is required.")]
        [Precision(9, 2)]
        public decimal Price { get; set; }

        public string? CategoryName { get; set; }
        public Category? Category { get; set; }

        public ICollection<ProductImage> Images { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<CartProduct> CartProducts { get; set; }

        public ICollection<RentProduct> RentProducts { get; set; }
    }
}
