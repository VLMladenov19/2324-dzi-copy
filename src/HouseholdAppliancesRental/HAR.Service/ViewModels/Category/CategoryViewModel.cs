using System.ComponentModel.DataAnnotations;

namespace HAR.Service.ViewModels.Category
{
    public class CategoryViewModel
    {
        [Required(ErrorMessage = "Името е задължително.")]
        [StringLength(32, ErrorMessage = "Името трябва да е между 2 и 32 знака.", MinimumLength = 2)]
        public string Name { get; set; }

        public string? ParentCategoryName { get; set; }
    }
}
