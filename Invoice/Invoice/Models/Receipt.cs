using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class Receipt
    {
        public DateTime createdDate { get; set; }
        public string status { get; set; }
        public int idReceipt { get; set; }
        public string customerName { get; set; }
        public Customer customer { get; set; }
        public string issuedBy { get; set; }
        public float totalAmount { get; set; }
        public List<Payment> payments { get; set; }
    }

    public enum PaymentMethod
    {
        CASH, BANK, CHEQUE
    }

    public class Payment
    {
        public int idPayment { get; set; }
        public int idReceipt { get; set; }
        public float amount { get; set; }
        public int paymentNumber { get; set; }
        public DateTime paymentDate { get; set; }
        public PaymentMethod paymentMethod { get; set; }
    }
}
