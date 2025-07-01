using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020102.DomainModels
{
    public class ProductAttribute
    {
        public long AttributeId { get; set; }
        public int ProductId { get; set; }
        public string AttributeName { get; set; } = "";
        public string AttributeValue { get; set; } = "";
        public int DisplayOrder {  get; set; }  

    }
}
