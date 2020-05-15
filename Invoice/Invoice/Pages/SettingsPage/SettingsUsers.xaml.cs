// /*****************************************************************************
//  * MIT License
//  *
//  * Copyright (c) 2020 InvoiceX
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.
//  *
//  *****************************************************************************/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InvoiceX.Classes;
using InvoiceX.Models;
using InvoiceX.ViewModels;

namespace InvoiceX.Pages.SettingsPage
{
    /// <summary>
    ///     Interaction logic for Users.xaml
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

        /// <summary>
        ///     Loads the users on the grid
        /// </summary>
        public void load()
        {
            userView = new UserViewModel();
            dataGrid_Users.ItemsSource = userView.UsersList;
        }

        /// <summary>
        ///     Adds a new user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_addUser_Click(object sender, RoutedEventArgs e)
        {
            var username = textBox_username.Text;
            var password = textBox_password.Text;
            var admin_privileges = (bool) checkBox_admin.IsChecked;
            var user = UserViewModel.getUserByUsername(username);

            if (user.username == null)
            {
                var hashSalt = HashSalt.GenerateSaltedHash(password);
                UserViewModel.insertUser(username, hashSalt.Hash, hashSalt.Salt, admin_privileges);
                load();
                clear();
            }
            else
            {
                MessageBox.Show("Username \"" + username + "\" is taken.");
            }
        }

        /// <summary>
        ///     Removes a specific user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_removeUser_Click(object sender, RoutedEventArgs e)
        {
            var user = (User) dataGrid_Users.CurrentCell.Item;
            var msgtext = "You are about to remove user \"" + user.username + "\". Are you sure?";
            var txt = "Remove User";
            var button = MessageBoxButton.YesNo;
            var result = MessageBox.Show(msgtext, txt, button);
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

        /// <summary>
        ///     Clears all inputs on the page
        /// </summary>
        public void clear()
        {
            textBox_username.Clear();
            textBox_password.Clear();
            checkBox_admin.IsChecked = false;
        }
    }
}