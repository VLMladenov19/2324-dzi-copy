using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HAR.Data.Models
{
    public class ProductImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "File content is required.")]
        public string Base64Content { get; set; }

        [Required(ErrorMessage = "File name is required.")]
        public string FileName { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
