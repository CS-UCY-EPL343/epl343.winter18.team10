using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice
{
    public class CustomerViewModel 
    {
        Data_import source = new Data_import();
        private ObservableCollection<Customer> customerlist = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> CustomerList 
        {
            get { return customerlist; }
        }

        public CustomerViewModel() 
        {

            foreach (var customer in source.GetCustomers()) 
            {

                customerlist.Add(customer);
            
            }
        
        
        }
    
    }
}
