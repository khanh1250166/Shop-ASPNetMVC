using SV21T1020102.DomainModels;

namespace SV21T1020102.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data {get;set;}
    }
}
