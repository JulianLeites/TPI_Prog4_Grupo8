using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public enum EnumState
        {
            EnPreparacion,
            Listo
        }
        public decimal Total { get; set; }

        public int Table_id { get; set; }

        public EnumState State { get; set; }

        public Table Table { get; set; } = null!;
    }
}