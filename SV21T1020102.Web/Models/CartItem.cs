﻿namespace SV21T1020102.Web.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string Photo { get; set; }= "";  
        public string Unit { get; set; } = "";
        public int Quantity { get; set; } = 0;
        public decimal SalePrice { get; set; } = 0;
        public decimal TotalPrice 
        {
            get
            {
                return Quantity * SalePrice;
            }
        }

    }
}
