using Dapper;
using System.Data;
using SV21T1020102.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace SV21T1020102.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connecttionString) : base(connecttionString)
        {
        }
    #region Thêm Mới

        public int AddProduct(Product Data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Products(ProductName, ProductDescription,SupplierID,CategoryID,Unit,Price,Photo,IsSelling)
                                        values(@ProductName, @ProductDescription,@SupplierID,@CategoryID,@Unit,@Price,@Photo,@IsSelling);
                                        select @@IDENTITY ";
                var parameters = new
                {
                    ProductName = Data.ProductName,
                    ProductDescription = Data.ProductDescription,
                    SupplierID=Data.SupplierId,
                    CategoryID=Data.CategoryId,
                    Unit = Data.Unit,
                    Price = Data.Price,
                    Photo = Data.Photo,
                    IsSelling = Data.IsSelling
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);

                connection.Close();
            }
            return id;

        }

        public long AddProductAttribute(ProductAttribute Data)
        {
            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductAttributes where DisplayOrder = @DisplayOrder and ProductID=@ProductID)
                                select -1
                            else
                                begin
                                insert into ProductAttributes(ProductID,AttributeName, AttributeValue,DisplayOrder)
                                values(@ProductID,@AttributeName,@AttributeValue,@DisplayOrder);
                                select @@IDENTITY
                            end";
                var parameters = new
                {
                    ProductID=Data.ProductId,
                    AttributeName = Data.AttributeName,
                    AttributeValue = Data.AttributeValue,
                    DisplayOrder = Data.DisplayOrder

                };
                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: CommandType.Text);

                connection.Close();
            }
            return id;
        }

        public long AddProductPhoto(ProductPhoto Data)
        {
            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductPhotos where DisplayOrder = @DisplayOrder and ProductID=@ProductID)
                                select -1
                            else
                                begin
                                    insert into ProductPhotos(ProductID,Photo, Description,DisplayOrder,IsHidden)
                                    values(@ProductID,@Photo, @Description,@DisplayOrder,@IsHidden);
                                    select @@IDENTITY 
                                end";
                var parameters = new
                {
                    ProductID=Data.ProductId,
                    Photo = Data.Photo,
                    Description = Data.Description,
                    DisplayOrder = Data.DisplayOrder,
                    IsHidden = Data.IsHidden,
                };
                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }
        #endregion      
    #region Xoa 
        public bool Delete(int productId)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = "delete from Products where ProductID = @ProductID;";
                int count = conn.Execute(sql, param: new { ProductID = productId }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public bool DeleteAttribute(long  attributeId)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = "delete from ProductAttributes where AttributeID = @AttributeID;";
                int count = conn.Execute(sql, param: new { AttributeID = attributeId }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public bool DeletePhoto(long photoId)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = "delete from ProductPhotos where PhotoID = @PhotoID;";
                int count = conn.Execute(sql, param: new { PhotoID = photoId }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }
        #region tim kiem theo id
        public Product? Get(int productID)
        {
            var product = new Product();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from Products where ProductID = @productID";
                product = conn.QueryFirstOrDefault<Product>(sql, new { productID = productID }, commandType: CommandType.Text);
            }
            return product;
        }

        public ProductAttribute? GetAttribute(long attributeId)
        {
            var attribute = new ProductAttribute();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from ProductAttributes where AttributeID = @attributeID";
                attribute = conn.QueryFirstOrDefault<ProductAttribute>(sql, new { attributeID = attributeId }, commandType: CommandType.Text);
            }
            return attribute;
        }

        public ProductPhoto? GetPhoto(long photoId)
        {
            var photo = new ProductPhoto();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from ProductPhotos where PhotoID = @photoID";
                photo = conn.QueryFirstOrDefault<ProductPhoto>(sql, new { photoID = photoId }, commandType: CommandType.Text);
            }
            return photo;
        }
        #endregion
        #endregion 
    #region Lay danh sach du lieu
        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            var listProducts = new List<Product>();
            using (var conn = OpenConnection())
            {
                string sql = @"select * 
                                from( select *, 
                                ROW_NUMBER() over (order by ProductName) as RowNumber
                                from Products
                                where (@searchValue = N'' or ProductName like @searchValue)
                                and (@categoryId = 0 or CategoryId = @categoryId)
                                and (@supplierId = 0 or SupplierId = @supplierId)
                                and (Price >= @minPrice)
                                and (@maxPrice <= 0 or Price <= @maxPrice)
                                ) as t
                            where ((@pageSize = 0) or RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)";

                var paramaters = new
                {
                    searchValue = $"%{searchValue}%",
                    page = page,
                    pageSize = pageSize,
                    categoryId = categoryID,
                    supplierId = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };
                listProducts = conn.Query<Product>(sql, param: paramaters, commandType: CommandType.Text).ToList();
            }
            return listProducts;
        }

        public IList<ProductAttribute> ListAttribute(int productID)
        {
            var listAttributes = new List<ProductAttribute>();
            using (var conn = OpenConnection())
            {
                string sql = @"
                                    select * 
                                    from ProductAttributes 
                                    where ProductID = @productID 
                                    order by DisplayOrder 
                                ";
                listAttributes = conn.Query<ProductAttribute>(sql, new { productID = productID }, commandType: CommandType.Text).ToList();
            }
            return listAttributes;
        }

        public IList<ProductPhoto> ListPhoto(int productID)
        {
            var listPhotos = new List<ProductPhoto>();
            using (var conn = OpenConnection())
            {
                string sql = @"
                                 select * from ProductPhotos where ProductID = @productID  order by DisplayOrder
                                ";
                listPhotos = conn.Query<ProductPhoto>(sql, new { productID = productID }, commandType: CommandType.Text).ToList();
            }
            return listPhotos;
        }
        #endregion
    #region Update
        public bool Update(Product Data)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"update Products set 
                                        ProductName = @ProductName, 
                                        ProductDescription = @ProductDescription, 
                                        SupplierID = @SupplierID, 
                                        CategoryID = @CategoryID, 
                                        Unit = @Unit, 
                                        Price = @Price,
                                        Photo = @Photo, 
                                        IsSelling = @IsSelling
                                        where ProductID = @ProductID";
                var paramaters = new
                {
                    ProductID = Data.ProductId,
                    ProductName = Data.ProductName,
                    ProductDescription = Data.ProductDescription,
                    SupplierID = Data.SupplierId,
                    CategoryID = Data.CategoryId,
                    Unit = Data.Unit,
                    Price = Data.Price,
                    Photo = Data.Photo,
                    IsSelling = Data.IsSelling
                };
                int count = conn.Execute(sql: sql, param: paramaters, commandType: CommandType.Text);
                result = count > 0;
                conn.Close();
                
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute Data)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"if not exists(Select* from ProductAttributes where AttributeID <> @AttributeID and DisplayOrder= @DisplayOrder)
                                begin
                                        update ProductAttributes set 
                                        AttributeName = @AttributeName, 
                                        AttributeValue = @AttributeValue,
                                        DisplayOrder = @DisplayOrder
                                        where ProductID = @ProductID and AttributeID = @AttributeID
                                end";
                var paramaters = new
                {
                    ProductID = Data.ProductId,
                    AttributeName = Data.AttributeName,
                    AttributeValue = Data.AttributeValue,
                    DisplayOrder = Data.DisplayOrder,
                    AttributeID = Data.AttributeId
                };
                int count = conn.Execute(sql: sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto Data)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"if not exists(Select* from ProductPhotos where PhotoID <> @PhotoID and DisplayOrder= @DisplayOrder)
                                begin
                                        update ProductPhotos set 
                                        Photo = @Photo,
                                        Description = @Description,
                                        DisplayOrder = @DisplayOrder,
                                        IsHidden = @IsHidden
                                        where ProductID = @ProductID and PhotoID = @PhotoID
                                end";
                var paramaters = new
                {
                    Photo = Data.Photo,
                    PhotoID = Data.PhotoId,
                    Description = Data.Description,
                    DisplayOrder = Data.DisplayOrder,
                    IsHidden = Data.IsHidden,
                    ProductID = Data.ProductId
                };
                int count = conn.Execute(sql: sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }
        #endregion
        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            var count = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"select count(*) from Products where (@searchValue = N'' or ProductName like @searchValue)
                                and (@categoryId = 0 or CategoryId = @categoryId)
                                and (@supplierId = 0 or SupplierId = @supplierId)
                                and (Price >= @minPrice)
                                and (@maxPrice <= 0 or Price <= @maxPrice)";
                count = conn.ExecuteScalar<int>(sql, param: new
                {
                    searchValue = $"%{searchValue}%",
                    categoryId = categoryID,
                    supplierId = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                }, commandType: CommandType.Text);
            }
            return count;
        }
        public bool IsUsed(int productID)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"IF EXISTS (
                                        SELECT * 
                                        FROM OrderDetails 
                                        WHERE ProductID = @ProductID
                                    ) OR EXISTS (
                                        SELECT * 
                                        FROM ProductPhotos 
                                        WHERE ProductID = @ProductID
                                    ) OR EXISTS (
                                        SELECT * 
                                        FROM ProductAttributes 
                                        WHERE ProductID = @ProductID
                                    )
                                        SELECT 1
                                    ELSE
                                        SELECT 0;";
                var param = new { ProductID = productID };
                int count = conn.ExecuteScalar<int>(sql, param, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

       
    }
}
