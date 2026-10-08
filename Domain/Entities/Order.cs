using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public enum State
        {
            EnPreparacion,
            Listo
        }
        public decimal Total { get; set; }

        //public int Table_id { get; set; }
    }
}