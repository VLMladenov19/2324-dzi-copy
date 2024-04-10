using System.ComponentModel.DataAnnotations;

namespace HAR.Web.Models.Review
{
    public class CreateReviewViewModel
    {
        [Required(ErrorMessage = "Оценката е задължителна.")]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public Guid ProductId { get; set; }
    }
}
