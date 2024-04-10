using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAR.Data.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Total price is required.")]
        [Precision(9, 2)]
        public decimal MonthPrice { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<CartProduct> CartProducts { get; set; }
    }
}
