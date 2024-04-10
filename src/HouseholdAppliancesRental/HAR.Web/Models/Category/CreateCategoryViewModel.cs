using System.ComponentModel.DataAnnotations;

namespace HAR.Web.Models.Category
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Име за категорията е задължително.")]
        [StringLength(32, ErrorMessage = "Името на категорията трябва да е между 2 и 32 знака.", MinimumLength = 2)]
        public string Name { get; set; }

        public string? ParentCategoryName { get; set; }

        public List<HAR.Data.Models.Category>? Categories { get; set; }
    }
}
