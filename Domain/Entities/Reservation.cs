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

        //public int User_id { get; set; }
        //public int Table_id { get; set; }
        //public int ReservationState_id { get; set; }
    }
}