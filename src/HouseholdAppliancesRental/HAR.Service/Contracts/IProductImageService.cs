using HAR.Common;
using HAR.Data.Models;

namespace HAR.Service.Contracts
{
    public interface IProductImageService
    {
        Task<Response> AddImage(ProductImage productImage);
    }
}
