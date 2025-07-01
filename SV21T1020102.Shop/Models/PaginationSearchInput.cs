namespace SV21T1020102.Shop.Models
{   
    /// <summary>
    /// Luu tru thong tin dau vao su dung cho chuc nang tim kiem va hien thi du lieu duoi dang phan trang
    /// </summary>
    public class PaginationSearchInput
    {
        /// <summary>
        /// Trang can hien thi
        /// </summary>
        public int Page { get; set; } = 1;  
        /// <summary>
        /// So dong can hien thi tren moi trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Chuoi gia tri can tim kiem
        /// </summary>
        public string SearchValue { get; set; } = "";

    }
}
