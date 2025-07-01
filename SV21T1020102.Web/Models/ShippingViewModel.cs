namespace SV21T1020102.Web.Models
{
    public class ShippingViewModel
    {
        public int OrderID { get; set; }
        public int ShipperID { get; set; }
        public string Message { get; set; } // Thông báo lỗi hoặc trạng thái
    }
}