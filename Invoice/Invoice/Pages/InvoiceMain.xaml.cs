using InvoiceX.Models;
using InvoiceX.ViewModels;
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

namespace InvoiceX.Pages
{
    /// <summary>
    /// Interaction logic for InvoiceMain.xaml
    /// </summary>
    public partial class InvoiceMain : Page
    {
        public InvoiceMain()
        {
            InitializeComponent();
            load();
        }    
        
        private void load()
        {
            InvoiceViewModel invVModel = new InvoiceViewModel();
            invoiceDataGrid.ItemsSource = invVModel.invoiceList;
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            viewTab.Visibility = Visibility.Visible;
            createTab.Visibility = Visibility.Hidden;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            viewTab.Visibility = Visibility.Hidden;
            createTab.Visibility = Visibility.Visible;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {

        }       

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            textBox_Address.Text = ((Customers)comboBox1.SelectedItem).Address+", " + 
                ((Customers)comboBox1.SelectedItem).City + ", " + ((Customers)comboBox1.SelectedItem).Country;
            textBox_Contact_Details.Text = ((Customers)comboBox1.SelectedItem).PhoneNumber.ToString();
            textBox_Email_Address.Text = ((Customers)comboBox1.SelectedItem).Email;
            

        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            load();
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCustomer.Text = null;
        }

        private void dtPickerFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtPickerTo.SelectedDate == null)
            {
                dtPickerTo.SelectedDate = dtPickerFrom.SelectedDate;
            }
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}
