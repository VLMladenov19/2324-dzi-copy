using HAR.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HAR.Web.Models.Rent
{
    public class ReviewRentViewModel
    {
        [Required(ErrorMessage = "Пълното име е задължително.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Адресът е задължителен.")]
        public Guid AddressId { get; set; }

        public List<HAR.Data.Models.Address>? Addresses { get; set; }

        public List<CartProduct>? CartProducts { get; set; }

        [Required(ErrorMessage = "Обща цена е задължителна.")]
        [Precision(9, 2)]
        public decimal TotalPrice { get; set; }
    }
}
