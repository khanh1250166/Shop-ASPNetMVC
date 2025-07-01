
using Dapper;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers.SQLServer
{
    public class ProvinceDAL : BaseDAL, ISimpleQueryDAL<Province>
    {
        public ProvinceDAL(string connecttionString) : base(connecttionString)
        {
        }

        public List<Province> Listt()
        {
            List<Province> data = new List<Province>();
            using (var conn = OpenConnection()) 
            {
                var sql = @"select * from Provinces";
                data = conn.Query<Province>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            };
            return data;
        }
    }
}
