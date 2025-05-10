using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyStoreApp.ViewModels
{
    public class OrderDetailViewModel
    {
        [Required]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải ít nhất là 1")]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}