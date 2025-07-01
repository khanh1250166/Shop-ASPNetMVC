using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020102.DataLayers
{
    /// <summary>
    /// Định nghĩa  chức năng truy vấn dữ liệu đơn giãn
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISimpleQueryDAL<T> where T : class 
    {
        /// <summary>
        /// Truy vấn đơn giãn 1 dữ liệu T trong bảng
        /// </summary>
        /// <returns></returns>
        List<T> Listt();
    }
    
}
