using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public int Diners { get; set; }
        public int UserId { get; set; }
        public ReservationState State { get; set; } = ReservationState.Confirmed;
        public User User { get; set; } = null!;
        public ICollection<Table> Tables { get; set; } = new List<Table>();
        //enum
    }
}