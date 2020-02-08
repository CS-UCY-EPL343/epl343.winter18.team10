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
        public Double Cost { get; set; }
        public Double SellPrice { get; set; }

        public double Vat { get; set; }


        //those are used only in the new invoice grid
        public Double Total { get; set; }
        public int Quanity { get; set; }

    }
}
