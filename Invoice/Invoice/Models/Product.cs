using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class Product
    {
        public int idProduct { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        public Double Cost { get; set; }
        public Double SellPrice { get; set; }
        public float Vat { get; set; }
        private bool _LowStock;
        public bool LowStock
        {
            get
            {
                if (Stock >= MinStock)
                    return false;
                else
                    return true;
            }
            set { _LowStock = value; }
        }
        public string Category { get; set; }


        //these are used only in the new invoice grid
        public Double Total { get; set; }
        public int Quantity { get; set; }

    }
}
