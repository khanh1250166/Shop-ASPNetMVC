using SV21T1020102.DomainModels;

namespace SV21T1020102.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
}
