using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrize { get; set; }
        public decimal Subtotal { get; set; }

        //public int Product_id { get; set; }
        //public int Order_id { get; set; }
    }
}