using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class Expense
    {
        public DateTime createdDate { get; set; }
        public string status { get; set; }
        public int idExpense { get; set; }
        public string customerName { get; set; }
        public Customer customer { get; set; }
        public string issuedBy { get; set; }
        public float totalAmount { get; set; }
        public List<Payment> payments { get; set; }
    }
}
