using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class Invoice
    {
        public DateTime m_date { get; set; }
        public int m_idInvoice { get; set; }
        public string m_customer { get; set; }
        public double m_cost { get; set; }
        public double m_VAT { get; set; }
        public double m_totalCost { get; set; }
        public List<InvoiceProduct> m_products { get; set; }
    }
}
