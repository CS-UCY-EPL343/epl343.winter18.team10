using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class CreditNote
    {
        public DateTime createdDate { get; set; }
        public int idCreditNote { get; set; }
        public string customerName { get; set; }
        public double cost { get; set; }
        public double VAT { get; set; }
        public double totalCost { get; set; }
        public List<Product> products { get; set; }
        public Customer customer { get; set; }
        public string issuedBy { get; set; }

    }
}
