using System.ComponentModel.DataAnnotations;

namespace HAR.Web.Models.User
{
    public class EditNameViewModel
    {
        [Required(ErrorMessage = "Име е задължително.")]
        [StringLength(32, ErrorMessage = "Името не може да е по-дължо от 32 знака.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия е задължителна.")]
        [StringLength(32, ErrorMessage = "Фамилията не може да е по-дълга от 32 знака.")]
        public string LastName { get; set; }
    }
}
