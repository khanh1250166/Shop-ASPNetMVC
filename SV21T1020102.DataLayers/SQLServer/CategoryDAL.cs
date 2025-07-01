using System.Data;
using Dapper;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers.SQLServer
{
    public class CategoryDAL : BaseDAL, ICommonDAL<Category>, ISimpleQueryDAL<Category>
    {
        public CategoryDAL(string connecttionString) : base(connecttionString)
        {
        }

        public int Add(Category data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Categories where CategoryName = @CategoryName)
                                select -1
                            else
                            begin
                                insert into Categories(CategoryName, Description)
                                values(@CategoryName, @Description);
                                select @@IDENTITY
                            end";
                            
                var parameters = new
                {
                   CategoryName=data.CategoryName,
                   Description =data.Description,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);

                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var conn = OpenConnection()) 
            {
                var sql = @"select count(*)
                            from Categories
                            Where (CategoryName like @searchValue)";
                var parameter = new 
                { 
                    searchValue
                };
                count = conn.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Categories where CategoryID = @CategoryID"; 
                var parameters = new
                {
                    CategoryID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)>0;
                connection.Close();
            }
            return result;
        }

        public Category? Get(int id)
        {
            Category? data = null;
            using (var conn = OpenConnection())
            {
                var sql = @"Select * From Categories where CategoryID=@CategoryID";
                var parameter = new
                {
                    CategoryID = id,
                };
                data = conn.QueryFirstOrDefault<Category>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                var sql = @" if exists(Select * from  Products where CategoryID=@CategoryID)
	                            select 1
                            else 
	                            select 0";
                var parameter = new
                {
                    CategoryID = id,
                };
                result = conn.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return result;
        }

        public List<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
           List<Category> list = new List<Category>();
            searchValue = $"%{searchValue}%";
            using (var conn = OpenConnection()) 
            {
                var sql = @"select * 
                            from (
		                            select * ,
				                            ROW_NUMBER()over (order by CategoryName) as RowNumber
		                            from Categories
		                            where (CategoryName like @searchValue)
	                            )as t
                            where (@pageSize=0)
                            or	(t.RowNumber between (@page-1)*@pageSize+1 and @page *@pageSize)
                            order by RowNumber";
                var parameter = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                list = conn.Query<Category>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            }
            return list;
        }

        public bool Update(Category data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(Select* from Categories where CategoryID <> @CategoryID and CategoryName= @CategoryName)
                                begin
                                update Categories 
                                set CategoryName = @CategoryName,
                                    Description = @Description                                  
                                where CategoryID = @CategoryID
                                end";
                var parameters = new
                {
                    CategoryID = data.CategoryID,
                    CategoryName = data.CategoryName,
                    Description = data.Description,
                   
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();

            }
            return result;
        }
        List<Category> ISimpleQueryDAL<Category>.Listt()
        {
            List<Category> data = new List<Category>();
            using (var conn = OpenConnection())
            {
                var sql = @"select CategoryName,CategoryID from Categories";
                data = conn.Query<Category>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            };
            return data;
        }
    }
}
