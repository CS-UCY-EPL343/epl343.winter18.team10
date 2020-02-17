using InvoiceX.Classes;
using InvoiceX.Models;
using InvoiceX.ViewModels;
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

namespace InvoiceX.Pages.SettingsPage
{
    /// <summary>
    /// Interaction logic for SettingsMain.xaml
    /// </summary>
    public partial class SettingsMain : Page
    {
        private UserViewModel userView;
        
        public SettingsMain()
        {
            InitializeComponent();
        }

        private void btn_addUser_Click(object sender, RoutedEventArgs e)
        {
            string username = textBox_username.Text;
            string password = textBox_password.Text;
            bool admin_privileges = (bool)checkBox_admin.IsChecked;
            User user = UserViewModel.getUserByUsername(username);

            if (user.username == null)
            {
                HashSalt hashSalt = HashSalt.GenerateSaltedHash(password);
                UserViewModel.addUserToDB(username, hashSalt.Hash, hashSalt.Salt, admin_privileges);
                btn_loadUsers_Click();
            }
            else
            {
                MessageBox.Show("Username \"" + username + "\" is taken.");
            }
            
        }

        private void btn_loadUsers_Click(object sender = null, RoutedEventArgs e = null)
        {
            userView = new UserViewModel();
            dataGrid_Users.ItemsSource = userView.UsersList;
        }

        private void btn_removeUser_Click(object sender, RoutedEventArgs e)
        {
            User user = (User)dataGrid_Users.CurrentCell.Item;
            string msgtext = "You are about to remove user \"" + user.username + "\". Are you sure?";
            string txt = "Remove User";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    UserViewModel.removeUser(user.username);
                    btn_loadUsers_Click();
                    break;
                case MessageBoxResult.No:
                    break;
            }
            
        }
    }
}
