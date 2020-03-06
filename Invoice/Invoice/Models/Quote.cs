using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class Quote
    {
        public DateTime createdDate { get; set; }
        public int idQuote { get; set; }
        public string customerName { get; set; }
        public List<Product> products { get; set; }
        public Customer customer { get; set; }
        public string issuedBy { get; set; }
    }
}
