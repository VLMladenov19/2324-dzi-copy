using HAR.Common;
using HAR.Service.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HAR.Web.Models.Rent
{
    public class RentFilterQuery : IFilterQuery
    {
        [FromQuery(Name = "p")]
        public int Page { get; set; }

        [FromQuery(Name = "ps")]
        public int PageSize { get; set; }

        [FromQuery(Name = "q")]
        public string SearchQuery { get; set; }

        public void Normalize()
        {
            if (this.Page < 1)
            {
                this.Page = 1;
            }

            if (this.PageSize < 1)
            {
                this.PageSize = CollectionConstants.RentPageSize;
            }

            if (!string.IsNullOrWhiteSpace(this.SearchQuery))
            {
                this.SearchQuery = this.SearchQuery.Trim();
            }
        }
    }
}
