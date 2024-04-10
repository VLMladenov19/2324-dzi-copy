using System.ComponentModel.DataAnnotations;

namespace HAR.Data.Models
{
    public class Category
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(32, ErrorMessage = "Category name must be between 2 and 32 characters", MinimumLength = 2)]
        public string Name { get; set; }

        public string? ParentCategoryName { get; set; }
        public Category? ParentCategory { get; set; }

        public ICollection<Category> ChildCategories { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
