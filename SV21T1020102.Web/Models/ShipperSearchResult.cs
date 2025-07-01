using SV21T1020102.DomainModels;

namespace SV21T1020102.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    } 
}
