using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.Models
{
    public class User
    {
        public string username { get; set; }
        public string hash { get; set; }
        public string salt { get; set; }
        public bool admin { get; set; }
    }
}
