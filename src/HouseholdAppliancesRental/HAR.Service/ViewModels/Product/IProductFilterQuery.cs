using HAR.Service.ViewModels;

namespace HAR.Web.Models.Product
{
    public interface IProductFilterQuery : IFilterQuery
    {
        string Category { get; set; }
    }
}
