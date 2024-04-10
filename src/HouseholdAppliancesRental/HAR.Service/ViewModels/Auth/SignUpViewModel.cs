using HAR.Common.DataAnnotation;
using System.ComponentModel.DataAnnotations;

namespace HAR.Service.ViewModels.Auth
{
    public class SignUpViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Имейлът е задължителен.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Първото име е задължително.")]
        [RegularExpression("^(?=.*[A-ZА-Яа-яa-z])([A-ZА-Я])([a-zа-я]{2,29})+(?<![_.])$")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилията е задължителна.")]
        [RegularExpression("^(?=.*[A-ZА-Яа-яa-z])([A-ZА-Я])([a-zа-я]{2,29})+(?<![_.])$")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Ролята е задължителна")]
        [AllowedRoles(ErrorMessage = "Невалидна роля, валидните роли са 'Admin' и 'User'.")]
        public string Role { get; set; }
    }
}
