using HAR.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HAR.Service.ViewModels.Product
{
    public class ProductViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        public string Name { get; set; }

        public string? Desc { get; set; }

        [Required(ErrorMessage = "Цената е задължителна.")]
        [Precision(9, 2)]
        public decimal Price { get; set; }

        public string? CategoryName { get; set; }

        public List<ProductImage> Images { get; set; }

        public List<HAR.Data.Models.Review> Reviews { get; set; }
    }
}
