using HAR.Common.DataAnnotation;
using System.ComponentModel.DataAnnotations;

namespace HAR.Service.ViewModels.User
{
    public class UserViewModel
    {
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "Име е задължително.")]
        [StringLength(32, ErrorMessage = "Името не може да е по-дължо от 32 знака.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия е задължителна.")]
        [StringLength(32, ErrorMessage = "Фамилията не може да е по-дълга от 32 знака.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Имейлът е задължителен.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ролята е задължителна.")]
        [AllowedRoles(ErrorMessage = "Невалидна роля, валидните роли са 'Admin' и 'User'.")]
        public string Role { get; set; }
    }
}
