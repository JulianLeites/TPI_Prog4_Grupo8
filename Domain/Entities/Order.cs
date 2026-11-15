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
        public OrderState State { get; set; } = OrderState.Active;
        public Table Table { get; set; } = null!;
        public ICollection<OrderGroup> OrderGroups { get; set; } = new List<OrderGroup>();

        public decimal CalculateTotal()
        {
            Total = OrderGroups
                .SelectMany(group => group.Details)
                .Sum(detail => detail.Subtotal);

            return Total;
        }

    }
}