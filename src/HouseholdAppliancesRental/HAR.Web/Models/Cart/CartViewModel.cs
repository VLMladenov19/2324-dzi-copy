using HAR.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HAR.Web.Models.Cart
{
    public class CartViewModel
    {
        [Required(ErrorMessage = "Цена на месец е задължителна.")]
        [Precision(9, 2)]
        public decimal MonthPrice { get; set; }

        public List<CartProduct> CartProducts { get; set; }

        [Required(ErrorMessage = "Общ брой месеци са задължителни.")]
        public int TotalMonths { get; set; }

        [Required(ErrorMessage = "Общата цена е задължителна.")]
        [Precision(9, 2)]
        public decimal TotalPrice { get; set; }
    }
}
