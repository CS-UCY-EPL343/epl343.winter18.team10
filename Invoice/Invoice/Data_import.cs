using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice
{
    class Data_import
    {
        private ObservableCollection<Customer> customer = new ObservableCollection<Customer>();

        //Data from DataBase here (i used fake static data :-) )

        public void CreateCustomerData() 
        {
            customer.Add(new Customer()
            {
                Name = "chrysis mixail",
                Address="kapou sto tseri, 13 , Nicosia",
                Telephone="99999999",
                Email="chrysis@hotmail.com"
            }) ;
            customer.Add(new Customer()
            {
                Name = "nektarios ",
                Address = "athens",
                Telephone = "99999999",
                Email = "fnp@hotmail.com"
            });
            customer.Add(new Customer()
            {
                Name = "Giannis Panteli - The Profit",
                Address = "skala",
                Telephone = "99999999",
                Email = "atetokoumpos@hotmail.com"
            });

        }

       
        public ObservableCollection<Customer> GetCustomers() 
        {
            customer.Clear();
            CreateCustomerData();
            return customer;
        }



    }
}
