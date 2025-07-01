using SV21T1020102.DomainModels;

namespace SV21T1020102.Shop.Models
{
    public class OrderResult:PaginationResult
    {
        public required List<Order> Data { get; set; }
    }
}
