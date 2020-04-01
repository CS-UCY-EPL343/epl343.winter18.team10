using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InvoiceX.Classes
{
    public sealed class DBConnection
    {
        private static DBConnection instance = null;
        public static DBConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBConnection();
                }
                return instance;
            }
        }

        private readonly string myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                                    "pwd=CCfHC5PWLjsSJi8G;database=invoice";
        public MySqlConnection Connection;

        private DBConnection()
        {
            try
            {
                Connection = new MySqlConnection(myConnectionString);
                Connection.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nCould not connect to the Database");
            }
        }

        ~DBConnection()  // finalizer
        {
            Connection.Close();
        }


    }
}
