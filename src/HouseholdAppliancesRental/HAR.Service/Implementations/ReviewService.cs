using HAR.Common;
using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Contracts;

namespace HAR.Service.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly DataContext context;

        public ReviewService(DataContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds a review to database
        /// </summary>
        /// <param name="review">Review object</param>
        /// <returns>Custrom response object</returns>
        public async Task<Response> CreateReviewAsync(Review review)
        {
            try
            {
                this.context.Reviews.Add(review);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Отзив създаден успешно."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Отзивът не успя да се добави.",
                    Details = ex.Message
                };
            }
        }
    }
}
