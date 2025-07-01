﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020102.DomainModels
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductDescription { get; set; } = "";
        public int SupplierId { get; set; }
        public int CategoryId { get; set; }
        public string Unit { get; set; } = "";
        public decimal Price { get; set; }
        public string Photo { get; set; } = "";
        public int CurrentStock { get; set; }
        public bool IsSelling { get; set; }


    }
}
