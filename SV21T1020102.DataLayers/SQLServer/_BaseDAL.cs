using Microsoft.Data.SqlClient;

namespace SV21T1020102.DataLayers.SQLServer
{
    /// <summary>
    /// Lớp cơ sở (lớp cha) của các lớp cài đặt các phép dữ liệu trên SQL Server
    /// </summary>
    public abstract class BaseDAL
    {
        /// <summary>
        /// Chuỗi tham số kết nối đến CSDL SQL Server
        /// </summary>
        protected string connecttionString = "";
        public BaseDAL(string connecttionString)
        {
            this.connecttionString = connecttionString;
        }
        /// <summary>
        /// Tạo và mở 1 kết nối CSDL (SQL Server)
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection conn = new SqlConnection(connecttionString);
            conn.Open();
            return conn;
        }
    }
}
