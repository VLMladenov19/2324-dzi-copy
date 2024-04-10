using HAR.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HAR.Web.Models.Rent
{
    public class RentViewModel
    {
        public List<RentProduct> RentProducts { get; set; }

        [Required(ErrorMessage = "Общата цема е задължителна.")]
        [Precision(9, 2)]
        public decimal TotalPrice { get; set; }
    }
}
