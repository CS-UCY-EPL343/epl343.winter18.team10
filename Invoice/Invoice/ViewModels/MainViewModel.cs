using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invoice.Models;

namespace Invoice.ViewModels
{
    public class MainViewModel
    {
        public List<Customers> CustomersList { get; set; }
        public MainViewModel() 
        {
            CustomersList = new List<Customers>
            {
                new Customers
                {
                FirstName = "chrysis",
                LastName = "mixail",
                Address ="kapou sto tseri, 13 , Nicosia",
                Telephone="99999999",
                Email="chrysis@hotmail.com",
                },
                new Customers
                {
                 FirstName = "foivos ",
                LastName = "panagi",
                Address = "athens",
                Telephone = "99999999",
                Email = "fnp@hotmail.com",
                },
                 new Customers
                {
                FirstName = "giannis ",
                LastName = "panteli",
                Address = "skala",
                Telephone = "99999999",
                Email = "atetokoumpos@hotmail.com",
                }

            };
    
        }

    }
}
