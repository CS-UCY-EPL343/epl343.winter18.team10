using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class Product
    {
        public int idProducts { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        public float Cost { get; set; }
        public float SellPrice { get; set; }
      
    }
}
