using SV21T1020102.DomainModels;

namespace SV21T1020102.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}
