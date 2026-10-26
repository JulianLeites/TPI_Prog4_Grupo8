using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Table
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Capacity { get; set; }
        public TableState State { get; set; } = TableState.Unoccupied;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}