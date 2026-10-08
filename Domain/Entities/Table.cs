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
        public enum State
        {
            Libre,
            Ocupada
        }
    }
}