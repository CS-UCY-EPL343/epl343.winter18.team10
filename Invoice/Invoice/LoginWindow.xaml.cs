using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InvoiceX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private static String pass = "pass";
        private static String user = "admin";

        public LoginWindow()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void login_button_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

            try
            {
                conn = new MySqlConnection(myConnectionString);
                //conn.Open();

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }

            if (txtPassword.Password == pass && txtUsername.Text == user)
            {
                MainWindow wnd = new MainWindow();
                wnd.Show();

                login_button.Content = "Succeed";
                this.Close();
            }
        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            login_button_Click(sender, e);
        }
    }
}
