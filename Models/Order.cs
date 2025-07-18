//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyStoreApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }
    
        public int OrderId { get; set; }
        public System.DateTime OrderDate { get; set; }
        public Nullable<int> AgentId { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string Status { get; set; }
    
        public virtual Agent Agent { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
