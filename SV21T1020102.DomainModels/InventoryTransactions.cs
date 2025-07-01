using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020102.DomainModels
{
    public class InventoryTransactions
    {
        public string TransactionID { get; set; } = "";
        public int ProductID { get; set; }
        public int QuantityChange { get; set; }
        public string TransactionType { get; set; } = "";
        public DateTime TransactionDate { get; set; }
        public int RelatedOrderID { get; set; }
        public string Note { get; set; } = "";
    }
}
