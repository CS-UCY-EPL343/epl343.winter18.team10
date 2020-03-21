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
        public double Cost { get; set; }
        public double SellPrice { get; set; }
        public float Vat { get; set; }
        public string Category { get; set; }
        public bool LowStock
        {
            get
            {
                if (Stock >= MinStock)
                    return false;
                else
                    return true;
            }
        }


        //these are used only in the new invoice product grid
        public double Total { get; set; }
        public int Quantity { get; set; }

        public double OfferPrice { get; set; }

    }
    
}
