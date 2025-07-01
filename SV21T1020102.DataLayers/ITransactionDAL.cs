using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV21T1020102.DomainModels;

namespace SV21T1020102.DataLayers
{
     public interface ITransactionDAL
    {
        List<InventoryTransactions> List();
        int AddInventoryTransactions(InventoryTransactions data);
    }
}
