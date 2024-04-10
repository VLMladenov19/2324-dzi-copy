using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HAR.Data.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(32, ErrorMessage = "First name cannot be longer than 32 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(32, ErrorMessage = "Last name cannot be longer than 32 characters.")]
        public string LastName { get; set; }

        public Cart Cart { get; set; }

        public ICollection<Address> Addresses { get; set; }

        public ICollection<Rent> Rents { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
