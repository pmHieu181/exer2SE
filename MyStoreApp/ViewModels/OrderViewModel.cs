using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyStoreApp.ViewModels
{
    public class OrderViewModel
    {
        [Required]
        [Display(Name = "Đại lý")]
        public int AgentId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
        public OrderViewModel()
        {
            OrderDetails = new List<OrderDetailViewModel>();
            OrderDate = DateTime.Now;
        }
    }
}