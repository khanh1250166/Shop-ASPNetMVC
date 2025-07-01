using SV21T1020102.DomainModels;

namespace SV21T1020102.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}
