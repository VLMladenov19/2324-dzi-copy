using HAR.Common;
using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Contracts;

namespace HAR.Service.Implementations
{
    public class ProductImageService : IProductImageService
    {
        private readonly DataContext context;

        public ProductImageService(DataContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds an image to the database
        /// </summary>
        /// <param name="productImage">ProductImage object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> AddImage(ProductImage productImage)
        {
            try
            {
                this.context.Add(productImage);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Снимката бе успешно добавена."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Снимката не успя да се добави.",
                    Details = ex.Message
                };
            }
        }
    }
}
