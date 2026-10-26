using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public int Table_id { get; set; }
        public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
        public OrderState State { get; set; } = OrderState.EnPreparacion;
        public Table Table { get; set; } = null!;

        public decimal CalculateTotal()
        {
            Total = Details.Sum(detail => detail.Subtotal);
            return Total;
        }

    }
}