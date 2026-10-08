using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Prize { get; set; }
        public string Description { get; set; }

        //public int ProductCategory_id { get; set; }
    }
}