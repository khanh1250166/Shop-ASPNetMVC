namespace SV21T1020102.Shop.Models
{
    public class PaginationInput
    {
        /// <summary>
        /// Trang can hien thi
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// So dong can hien thi tren moi trang
        /// </summary>
        public int PageSize { get; set; }
    }
}
