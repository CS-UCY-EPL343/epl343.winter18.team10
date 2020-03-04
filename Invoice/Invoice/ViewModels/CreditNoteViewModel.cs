using InvoiceX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.ViewModels
{
   
    public class CreditNoteViewModel
    {
        public List<CreditNote> creditNoteList { get; set; }
        static string myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                           "pwd=CCfHC5PWLjsSJi8G;database=invoice";

        public CreditNoteViewModel()
        {

        }

        public static void getCreditNoteById()
        {

        }

        public static void deleteCreditNoteById(int creditNoteID)
        {

        }

    }
}
