﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020102.DomainModels
{
    public class ProductPhoto
    {
        public long PhotoId { get; set; }
        public int ProductId { get; set; }
        public string Photo { get; set; } = "";
        public string Description { get; set; } = "";
        public int DisplayOrder { get; set; }
        public bool IsHidden { get; set; }
    }
}
