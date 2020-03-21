using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class Expense
    {
        public string companyName { get; set; }
        public string category { get; set; }
        public int contactDetails { get; set; }
        public string description { get; set; }

        public int idExpense { get; set; }
        public int invoiceNo { get; set; }
        public DateTime createdDate { get; set; }
        public float cost { get; set; }
        public float VAT { get; set; }
        public float totalCost { get; set; }
        public string issuedBy { get; set; }

        public bool isPaid { get; set; }      

        public List<Payment> payments { get; set; }
    }
}
