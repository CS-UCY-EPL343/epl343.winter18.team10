using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class Customers
    {
        public int idCustomer { get; set; }
        public string CustomerName { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public float Balance { get; set; }


    }
}
