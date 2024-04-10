using System.ComponentModel.DataAnnotations;

namespace HAR.Web.Models.User
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Текущата парола е задължителна.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Новата парола е задължителна.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Повтарящата парола е задължителна.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Паролите не са еднакви.")]
        public string ConfirmPassword { get; set; }
    }
}
