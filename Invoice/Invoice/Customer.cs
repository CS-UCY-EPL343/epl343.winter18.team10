using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice
{
    public class Customer
    {
        private string name = String.Empty;
        private string address = String.Empty;
        private string telephone = String.Empty;
        private string email = String.Empty;

        public string Name
        {
            get { return name; }

            set 
            {
                if (name != value) 
                {
                    name = value;
                }

            
            }
        }
        public string Address
        {
            get { return address; }

            set
            {
                if (address != value)
                {
                    address = value;
                }


            }
        }

        public string Telephone
        {
            get { return telephone; }

            set
            {
                if (telephone != value)
                {
                    telephone = value;
                }


            }
        }
        public string Email
        {
            get { return email; }

            set
            {
                if (email != value)
                {
                    email = value;
                }


            }
        }

    }
}
