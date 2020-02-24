using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class Order
    {
        public DateTime createdDate { get; set; }
        public DateTime shippingDate { get; set; }
        public string status { get; set; }
        public int idOrder { get; set; }
        public string customerName { get; set; }
        public double cost { get; set; }
        public double VAT { get; set; }
        public double totalCost { get; set; }
        public List<Product> products { get; set; }
        public Customer customer { get; set; }
        public string issuedBy { get; set; }

    }
}
