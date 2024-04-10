using HAR.Common;
using Microsoft.AspNetCore.Mvc;

namespace HAR.Web.Models.Product
{
    public class ProductFilterQuery : IProductFilterQuery
    {
        [FromQuery(Name = "p")]
        public int Page { get; set; }

        [FromQuery(Name = "ps")]
        public int PageSize { get; set; }

        [FromQuery(Name = "q")]
        public string SearchQuery { get; set; }

        [FromQuery(Name = "c")]
        public string Category { get; set; }

        public void Normalize()
        {
            if (this.Page < 1)
            {
                this.Page = 1;
            }

            if (this.PageSize < 1)
            {
                this.PageSize = CollectionConstants.ProductPageSize;
            }

            if (!string.IsNullOrWhiteSpace(this.SearchQuery))
            {
                this.SearchQuery = this.SearchQuery.Trim();
            }
        }
    }
}
