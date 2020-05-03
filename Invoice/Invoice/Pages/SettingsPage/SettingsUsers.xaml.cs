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
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class SettingsUsers : Page
    {
        private UserViewModel userView;

        public SettingsUsers()
        {
            InitializeComponent();
            if (MainWindow.user.admin == false)
            {
                btn_addUser.IsEnabled = false;
                btn_addUser.Background = Brushes.Gray;
                colRemove.Visibility = Visibility.Collapsed;
            }
        }

        public void load()
        {
            userView = new UserViewModel();
            dataGrid_Users.ItemsSource = userView.UsersList;
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
                UserViewModel.insertUser(username, hashSalt.Hash, hashSalt.Salt, admin_privileges);
                load();
                clear();
            }
            else
            {
                MessageBox.Show("Username \"" + username + "\" is taken.");
            }

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
                    UserViewModel.deleteUser(user.username);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }

        }

        public void clear()
        {
            textBox_username.Clear();
            textBox_password.Clear();
            checkBox_admin.IsChecked = false;
        }
    }
}
