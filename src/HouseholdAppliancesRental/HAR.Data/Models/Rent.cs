using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAR.Data.Models
{
    public class Rent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Общата цена е задължителна.")]
        [Precision(9, 2)]
        public decimal TotalPrice { get; set; }

        public Guid? AddressId { get; set; }
        public Address? Address { get; set; }

        public DateTime RentalDate { get; set; }

        public DateTime ReturnDate { get; set; }

        [Required(ErrorMessage = "Пълно име е задължително")]
        [StringLength(32, ErrorMessage = "Пълното име не може да е по дължо от 32 знака")]
        public string UserFullName { get; set; }

        public string UserEmail { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<RentProduct> RentProducts { get; set; }
    }
}
