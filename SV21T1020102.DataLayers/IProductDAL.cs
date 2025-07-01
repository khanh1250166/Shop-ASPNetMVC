using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers
{
    public interface IProductDAL
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang 
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang (0 nếu không phân trang)</param>
        /// <param name="searchValue">Tên mặt hàng cần tìm (chuổi rỗng nếu không tìm kiếm)</param>
        /// <param name="categoryID">Mã loại hàng cần tìm (0 nếu không tìm theo mã loại hàng)</param>
        /// <param name="supplierID">Mã nhà cung cấp cần tìm (0 nếu không tìm theoi mã nhà cung cấp)</param>
        /// <param name="minPrice">Mức giá nhỏ nhất trong khoảng cần tìm</param>
        /// <param name="maxPrice">Mức giá lớn nhất trong khoảng cần tìm (0 nếu hạn chế mức giá lớn nhất)</param>
        /// <returns></returns>
        List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0,int supplierID=0,decimal minPrice=0,decimal maxPrice=0);
        /// <summary>
        /// Đếm số lượng mặt hàng đã tìm kiếm được
        /// </summary>
        /// <param name="searchValue">Tên mặt hàng cần tìm (chuổi rỗng nếu không tìm kiếm)</param>
        /// <param name="categoryID">Mã loại hàng cần tìm (0 nếu không tìm theo mã loại hàng)</param>
        /// <param name="supplierID">Mã nhà cung cấp cần tìm (0 nếu không tìm theoi mã nhà cung cấp)</param>
        /// <param name="minPrice">Mức giá nhỏ nhất trong khoảng cần tìm</param>
        /// <param name="maxPrice">Mức giá lớn nhất trong khoảng cần tìm (0 nếu hạn chế mức giá lớn nhất)</param>
        /// <returns></returns>
        int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
        /// <summary>
        /// Lấy thông tin theo mã sản phẩm
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        Product? Get(int productID);
        /// <summary>
        /// Thêm mới sản phẩm (hàm trả về mã sản phẩm mặt hàng được bổ sung)
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        int AddProduct(Product Data);
        /// <summary>
        /// Cập nhật cho thông tin mặt hàng
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        bool Update(Product Data);
        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        bool Delete(int productId);
        /// <summary>
        /// Kiêm tra xem mặt hàng có đơn hàng liên quan hay không ?
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        bool IsUsed(int productID);
        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng sắp xếp theo DisplayOrder
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        IList<ProductPhoto> ListPhoto(int productID);
        /// <summary>
        /// Lấy thông tin 1 ảnh dựa vào ID 
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        ProductPhoto? GetPhoto(long photoId);
        /// <summary>
        /// Thêm mới thông tin của 1 ảnh (hàm trả về mã ảnh nếu được bổ sung)
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        long AddProductPhoto(ProductPhoto Data);
        /// <summary>
        /// Cập nhật ảnh của matự hàng 
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        bool UpdatePhoto(ProductPhoto Data);
        /// <summary>
        /// Xóa 1 ảnh mặt hàng
        /// </summary>
        /// <param name="photoId"></param>
        /// <returns></returns>
        bool DeletePhoto(long photoId);

        /// <summary>
        /// Lấy danh sách thuộc tính các mặt hàng, sắp xếp thứ tự theo DisplayOrder 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        IList<ProductAttribute> ListAttribute(int productID);
        /// <summary>
        /// Lấy thông tin của thuộc tính theo mã loại hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        ProductAttribute? GetAttribute(long attributeId);
        /// <summary>
        /// Bổ sung thuộc tính cho mặt hàng
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        long AddProductAttribute(ProductAttribute Data);
        /// <summary>
        /// Cập nhật thuộc tính của mặt hàng
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        bool UpdateAttribute(ProductAttribute Data);
        /// <summary>
        /// Xóa thuộc tính của mặt hàng 
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        bool DeleteAttribute(long attributeId);

    }
}
