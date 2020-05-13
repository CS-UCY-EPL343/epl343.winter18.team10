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
    /// This class represents a Statement Item with all its information
    /// </summary>
    public class StatementItem
    {
        public DateTime createdDate { get; set; }
        public string description 
        {
            get 
            {
                return itemType.ToString() + ", ID: " + idItem;
            }
            set { }
        }
        public float charges { get; set; }
        public float credits { get; set; }
        private float prevBalance;
        public float balance
        {
            get
            {
                if (itemType == ItemType.Invoice)
                    return prevBalance + charges;
                else
                    return prevBalance - credits;
            }
            set
            {
                prevBalance = value;
            }
        }
        public int idItem { get; set; }
        public ItemType itemType { get; set; }

    }

    /// <summary>
    /// The enumeration representing the 3 types of Statement Item
    /// </summary>
    public enum ItemType
    {
        Invoice, Receipt, CreditNote
    }
}
