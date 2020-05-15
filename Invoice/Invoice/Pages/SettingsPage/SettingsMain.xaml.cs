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

namespace InvoiceX.Pages.SettingsPage
{
    /// <summary>
    ///     Interaction logic for SettingsMain.xaml
    /// </summary>
    public partial class SettingsMain : Page
    {
        private readonly SettingsDatabase databasePage;
        private readonly SettingsUsers usersPage;

        public SettingsMain()
        {
            InitializeComponent();
            usersPage = new SettingsUsers();
            databasePage = new SettingsDatabase();
            if (MainWindow.user.admin == false) btnDatabase.IsEnabled = false;
            btnUsers_Click(null, null);
        }

        /// <summary>
        ///     Switches to Users page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnUsers.Style = FindResource("ButtonStyleSelected") as Style;
            settingsPage.Content = usersPage;
            usersPage.load();
        }

        /// <summary>
        ///     Switches to Database page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnDatabase.Style = FindResource("ButtonStyleSelected") as Style;
            settingsPage.Content = databasePage;
        }

        /// <summary>
        ///     Resets all the button styles
        /// </summary>
        private void resetAllBtnStyles()
        {
            btnUsers.Style = btnDatabase.Style = FindResource("ButtonStyle") as Style;
        }
    }
}