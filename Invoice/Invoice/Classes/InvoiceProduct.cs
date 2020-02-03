using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Classes
{
    public class InvoiceProduct
    {
        public int m_idInvoice { get; set; }
        public int m_idProduct { get; set; }
        public int m_quantity { get; set; }
        public double m_totalCost { get; set; }
    }
}
