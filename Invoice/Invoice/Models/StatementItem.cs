﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
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
        public float balance { get; set; }
        public int idItem { get; set; }
        public ItemType itemType { get; set; }

    }

    public enum ItemType
    {
        Invoice, Receipt, CreditNote
    }
}