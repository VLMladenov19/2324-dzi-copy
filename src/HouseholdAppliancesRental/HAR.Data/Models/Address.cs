using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAR.Data.Models
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "City name is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Postal code is required.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Neighborhood is required.")]
        public string Neighborhood { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Street number is required.")]
        public string StreetNumber { get; set; }

        public string? BlockNumber { get; set; }

        public string? Entrance { get; set; }

        public int? Floor { get; set; }

        public string? Apartment { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<Rent> Rents { get; set; }
    }
}
