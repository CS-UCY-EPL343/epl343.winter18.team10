using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class Invoice
    {
        public DateTime m_createdDate { get; set; }
        public DateTime m_dueDate { get; set; }
        public int m_idInvoice { get; set; }
        public string m_customerName { get; set; }
        public double m_cost { get; set; }
        public double m_VAT { get; set; }
        public double m_totalCost { get; set; }
        public List<Product> m_products { get; set; }
        public Customer m_customer { get; set; }
        public string m_issuedBy { get; set; }

        
    }
}
