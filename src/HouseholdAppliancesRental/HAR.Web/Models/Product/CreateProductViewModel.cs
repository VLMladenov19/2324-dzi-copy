using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HAR.Web.Models.Product
{
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Името на продукта е задължително.")]
        public string Name { get; set; }

        public string? Desc { get; set; }

        [Required(ErrorMessage = "Цена на продукта е задължителна.")]
        [Precision(9, 2)]
        public decimal Price { get; set; }

        public string? CategoryName { get; set; }

        public List<HAR.Data.Models.Category>? Categories { get; set; }

        [FromForm(Name = "imageUploads")]
        public List<IFormFile>? Images { get; set; }
    }
}
