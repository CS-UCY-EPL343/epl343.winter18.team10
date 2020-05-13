/*****************************************************************************
 * MIT License
 *
 * Copyright (c) 2020 InvoiceX
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    /// <summary>
    /// This class represents a Receipt with all its information
    /// </summary>
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

    /// <summary>
    /// The enumeration representing the 3 payment methods for a receipt
    /// </summary>
    public enum PaymentMethod
    {
        Cash, Bank, Cheque
    }

    /// <summary>
    /// This class represents a Payment with all its information
    /// </summary>
    public class Payment
    {
        public int idPayment { get; set; }
        public int idReceipt { get; set; }
        public float amount { get; set; }

        public float Vat { get; set; }
        public string paymentNumber { get; set; }

        public DateTime paymentDate { get; set; }
        public PaymentMethod paymentMethod { get; set; }
    }
}
