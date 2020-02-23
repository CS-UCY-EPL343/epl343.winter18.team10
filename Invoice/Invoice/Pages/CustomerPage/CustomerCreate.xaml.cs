using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
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

namespace InvoiceX.Pages.CustomerPage
{
    /// <summary>
    /// Interaction logic for CustomerCreate.xaml
    /// </summary>
    public partial class CustomerCreate : Page
    {
        public CustomerCreate()
        {
            InitializeComponent();
        }
       

        private bool validate_customer()
        {
            bool ProductCreateOK = true;
            if (string.IsNullOrWhiteSpace(textBox_CustomerName.Text)) { textBox_CustomerName.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!int.TryParse(textBox_PhoneNumber.Text, out int a)) { textBox_PhoneNumber.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_CustomerEmail.Text)) { textBox_CustomerEmail.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_CustomerCountry.Text)) { textBox_CustomerCountry.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_CustomerCity.Text)) { textBox_CustomerCity.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_CustomerAddress.Text)) { textBox_CustomerAddress.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!double.TryParse(textBox_CustomerBalance.Text, out double b)) { textBox_CustomerBalance.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            return ProductCreateOK;
        }

        private Customer create_Object_customer()
        {
            Customer customer = new Customer();
            customer.CustomerName= textBox_CustomerName.Text;
            customer.PhoneNumber=Int32.Parse(textBox_PhoneNumber.Text);
            customer.Email=textBox_CustomerEmail.Text;
            customer.Country= textBox_CustomerCountry.Text;
            customer.City=textBox_CustomerCity.Text;
            customer.Address=textBox_CustomerAddress.Text;
            customer.Balance=float.Parse(textBox_CustomerBalance.Text);
            return customer;
        }
        private void Btn_createCustomer_Click(object sender, RoutedEventArgs e)
        {  
            if (validate_customer()) CustomerViewModel.SendCustomerToDB(create_Object_customer());
        }

        private void TextBox_CustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerName.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TextBox_CustomePhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_PhoneNumber.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TextBox_CustomerEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerEmail.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_CustomerCountry_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerCountry.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_CustomerCity_Stock_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerCity.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_CustomerAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerAddress.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_CustomerBalance_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerBalance.ClearValue(TextBox.BorderBrushProperty);
        }

      

        private void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            textBox_CustomerName.ClearValue(TextBox.BorderBrushProperty);
            textBox_PhoneNumber.ClearValue(TextBox.BorderBrushProperty);
            textBox_CustomerEmail.ClearValue(TextBox.BorderBrushProperty);
            textBox_CustomerCountry.ClearValue(TextBox.BorderBrushProperty);
            textBox_CustomerCity.ClearValue(TextBox.BorderBrushProperty);
            textBox_CustomerAddress.ClearValue(TextBox.BorderBrushProperty);
            textBox_CustomerBalance.ClearValue(TextBox.BorderBrushProperty);


            textBox_CustomerName.Clear();
            textBox_PhoneNumber.Clear();
            textBox_CustomerEmail.Clear();
            textBox_CustomerCountry.Clear();
            textBox_CustomerCity.Clear();
            textBox_CustomerAddress.Clear();
            textBox_CustomerBalance.Clear();
            
        }
    }
}
