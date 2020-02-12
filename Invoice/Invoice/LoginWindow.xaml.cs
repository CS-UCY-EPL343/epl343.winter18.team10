using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

            string username = txtUsername.Text;
            string password = txtPassword.Password;
            bool response = false;
            bool adminPrivileges = false;
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "login_authentication";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userID", username);
                cmd.Parameters["@userID"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@pass", password);
                cmd.Parameters["@pass"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@response", MySqlDbType.Bool);
                cmd.Parameters["@response"].Direction = ParameterDirection.Output;

                cmd.Parameters.AddWithValue("@admin_priv", MySqlDbType.Bool);
                cmd.Parameters["@admin_priv"].Direction = ParameterDirection.Output;

                response = Convert.ToBoolean(cmd.Parameters["@response"].Value);
                adminPrivileges = Convert.ToBoolean(cmd.Parameters["@admin_priv"].Value);

                cmd.ExecuteNonQuery();
                conn.Close();
                if (response)
                {
                    MainWindow wnd = new MainWindow();
                    wnd.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid Username/Password!");
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            //login_button_Click(sender, e);
        }
    }
}
