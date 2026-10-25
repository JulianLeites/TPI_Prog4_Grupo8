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
        public OrderState State { get; set; }
        public Table Table { get; set; } = null!;
    }
}