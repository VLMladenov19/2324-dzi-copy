using System.ComponentModel.DataAnnotations;

namespace HAR.Service.ViewModels.Auth
{
    public class SignInViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Имейл е задължителен.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна.")]
        public string Password { get; set; }
    }
}
