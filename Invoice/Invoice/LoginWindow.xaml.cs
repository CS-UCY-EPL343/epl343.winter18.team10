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

namespace Invoice
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
            login_button.Click += myButton_Click;

        }
        void myButton_Click(object sender, RoutedEventArgs e)
        {
            if (password.Password == pass && username.Text == user)
            {
                MainWindow wnd = new MainWindow();
                wnd.Show();

                login_button.Content = "Succeed";
                this.Close();
            }
        }


    }
}
