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
        SettingsUsers usersPage;
        SettingsDatabase databasePage;

        public SettingsMain()
        {
            InitializeComponent();
            usersPage = new SettingsUsers();
            databasePage = new SettingsDatabase();
            btnUsers_Click(null, null);
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnUsers.Style = FindResource("ButtonStyleSelected") as Style;
            settingsPage.Content = usersPage;
            usersPage.load();
        }

        private void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnDatabase.Style = FindResource("ButtonStyleSelected") as Style;
            settingsPage.Content = databasePage;
            
        }

        private void resetAllBtnStyles()
        {           
            btnUsers.Style = btnDatabase.Style = FindResource("ButtonStyle") as Style;
        }
    }
}
