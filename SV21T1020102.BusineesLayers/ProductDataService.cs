using SV21T1020102.DataLayers;
using SV21T1020102.DataLayers.SQLServer;
using SV21T1020102.DomainModels;

namespace SV21T1020102.BusineesLayers
{
    public class ProductDataService
    {
        private static readonly IProductDAL productDB;
    static ProductDataService()
    {
        productDB = new ProductDAL(Configuration.ConnectionString);
    }
    #region Product
    public static List<Product> ListOfProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "",
        int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0)
    {
        rowCount = productDB.Count(searchValue, categoryId, supplierId, minPrice, maxPrice);
        return productDB.List(page, pageSize, searchValue, categoryId, supplierId, minPrice, maxPrice).ToList();
    }
    public static List<Product> ListOfProducts(string searchValue = "")
    {
        return productDB.List(1, 0, searchValue).ToList();
    }
    public static Product? GetProduct(int productId)
    {
        return productDB.Get(productId);
    }
    public static int AddProduct(Product data)
    {
        return productDB.AddProduct(data);
    }
    public static bool UpdateProduct(Product data)
    {
        return productDB.Update(data);
    }
    public static bool DeleteProduct(int productId)
    {
        return productDB.Delete(productId);
    }
    public static bool InUsedProduct(int productId)
    {
        return productDB.IsUsed(productId);
    }
    #endregion

    #region Product Photo

    public static List<ProductPhoto> ListProductPhotos(int productId)
    {
        return productDB.ListPhoto(productId).ToList();
    }
    public static ProductPhoto? GetProductPhoto(long productPhotoId)
    {
        return productDB.GetPhoto(productPhotoId);
    }
    public static long AddProductPhoto(ProductPhoto data)
    {
        return productDB.AddProductPhoto(data);
    }

    public static bool DeleteProductPhoto(long productPhotoId)
    {
        return productDB.DeletePhoto(productPhotoId);
    }

    public static bool UpdateProductPhoto(ProductPhoto data)
    {
        return productDB.UpdatePhoto(data);
    }


    #endregion

    #region Attribute
    public static List<ProductAttribute> ListProductAttributes(int productID)
    {
        return productDB.ListAttribute(productID).ToList();
    }
    public static ProductAttribute? GetAttribute(long productAttributeId)
    {
        return productDB.GetAttribute(productAttributeId);
    }
    public static long AddAttribute(ProductAttribute data)
    {
        return productDB.AddProductAttribute(data);
    }
    public static bool UpdateAttribute(ProductAttribute data)
    {
        return productDB.UpdateAttribute(data);
    }
    public static bool DeleteAttribute(long productAttributeId)
    {
        return productDB.DeleteAttribute(productAttributeId);
    }
    #endregion
}
}
