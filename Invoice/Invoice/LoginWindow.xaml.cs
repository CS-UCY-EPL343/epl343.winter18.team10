using InvoiceX.Classes;
using InvoiceX.Models;
using InvoiceX.ViewModels;
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

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {            
            User user = UserViewModel.getUserByUsername(txtUsername.Text);

            if (user.username != null)
            {
                bool isPasswordMatched = VerifyPassword(txtPassword.Password, user.hash, user.salt);

                if (isPasswordMatched)
                {
                    //Login Successfull
                    MainWindow mainWindow = new MainWindow(user);
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

        private bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }       

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn_login_Click(new object(), new RoutedEventArgs());
            }
        }
    }
}
