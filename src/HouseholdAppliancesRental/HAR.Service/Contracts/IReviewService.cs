using HAR.Common;
using HAR.Data.Models;

namespace HAR.Service.Contracts
{
    public interface IReviewService
    {
        Task<Response> CreateReviewAsync(Review review);
    }
}
