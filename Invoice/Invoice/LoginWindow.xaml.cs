using InvoiceX.Classes;
using InvoiceX.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
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
            User user = getUserByUsername(txtUsername.Text);

            if (user.username != null)
            {
                bool isPasswordMatched = VerifyPassword(txtPassword.Password, user.hash, user.salt);

                if (isPasswordMatched)
                {
                    //Login Successfull
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    //Login Failed
                    MessageBox.Show("Password is invalid");
                }
            }
            else
            {
                MessageBox.Show("Username is invalid");
            }
        }

        private User getUserByUsername(string username)
        {
            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";
            User user = new User();  
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM User WHERE idUser = \"" + username + "\"", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count != 0)
                {
                    return new User()
                    {
                        username = dt.Rows[0].Field<string>("idUser"),
                        hash = dt.Rows[0].Field<string>("Hash"),
                        salt = dt.Rows[0].Field<string>("Salt"),
                        admin = dt.Rows[0].Field<bool>("AdminPrivileges"),
                    };
                }               

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }

            return user;
        }

        private bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            //login_button_Click(sender, e);
        }
    }
}
