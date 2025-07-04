﻿using System.ComponentModel.DataAnnotations.Schema;

namespace SV21T1020102.DomainModels
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } =string.Empty;
        public bool IsLocked { get; set; }
        [NotMapped]  
        public string ConfirmPassword { get; set; }
    }
}
