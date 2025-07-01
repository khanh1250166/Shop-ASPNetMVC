namespace SV21T1020102.DataLayers
{
    /// <summary>
    /// Định nghĩa các phép xứ lý dự liệu thường dùng trên các bảng(Customer, Employees, Shipper,...)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tìm kiếm và lấy dnah sách dữ liệu kiểu là T dưới dạng phân trang
        /// </summary>
        /// <param name="page">Số trang cần hiển thị </param>
        /// <param name="pageSize">Số dòng được hiển thị trên mỗi trang (bằng 0 nếu k có phân trang )</param>
        /// <param name="searchValue">Giá trị cần tìm kiếm (chuổi rỗng nếu lấy toàn bộ dữ liệu ) </param>
        /// <returns></returns>
        List<T> List ( int page = 1, int pageSize = 0, string searchValue="" );
        /// <summary>
        /// Đếm số lượng dòng dữ liệu tìm kiếm được 
        /// </summary>
        /// <param name="searchValue">Giá trị cần toimf kiếm (chuổi rồng nếu đảm bảo trên toàn bộ dữu liệu)</param>
        /// <returns></returns>
        int Count(string searchValue="");
        /// <summary>
        /// Lấy 1 bảng ghi dữ liệu của kiểu T dựa và khóa chính của nó(trả về giá trị null nếu dữ liệu k tồn tại)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);
        /// <summary>
        /// Bổ sung một bảng ghi vào CSDL. Hàm trả về Id của dữ liệu  vừa bổ sung 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);
        /// <summary>
        /// Cập nhật 1 bảng ghi dữ liệu 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);
        /// <summary>
        /// Xóa 1 bảng ghi dữ liẹu dựa vào giá trị khóa chính 
        /// </summary>
        /// <param name="id"></param>
             /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Kiểm tra xem có dữ liệu đang tham chiếu ở bảng khác hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
    }
}
