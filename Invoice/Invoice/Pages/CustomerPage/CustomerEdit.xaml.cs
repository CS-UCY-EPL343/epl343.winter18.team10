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
using InvoiceX.Models;
using InvoiceX.ViewModels;

namespace InvoiceX.Pages.CustomerPage
{
    /// <summary>
    ///     Interaction logic for CustomerEdit.xaml
    /// </summary>
    public partial class CustomerEdit : Page
    {
        public CustomerEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Checks that all values are completed
        /// </summary>
        /// <returns></returns>
        private bool validate_customer()
        {
            var ProductCreateOK = true;
            if (string.IsNullOrWhiteSpace(textBox_CustomerName.Text))
            {
                textBox_CustomerName.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (!int.TryParse(textBox_PhoneNumber.Text, out var a))
            {
                textBox_PhoneNumber.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (string.IsNullOrWhiteSpace(textBox_CustomerEmail.Text))
            {
                textBox_CustomerEmail.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (string.IsNullOrWhiteSpace(textBox_CustomerCountry.Text))
            {
                textBox_CustomerCountry.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (string.IsNullOrWhiteSpace(textBox_CustomerCity.Text))
            {
                textBox_CustomerCity.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (string.IsNullOrWhiteSpace(textBox_CustomerAddress.Text))
            {
                textBox_CustomerAddress.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (!double.TryParse(textBox_CustomerBalance.Text, out var b))
            {
                textBox_CustomerBalance.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            return ProductCreateOK;
        }

        /// <summary>
        ///     Creates and returns the Customer based on the information on the page
        /// </summary>
        /// <returns></returns>
        private Customer createCustomerObject()
        {
            var customer = new Customer();
            customer.CustomerName = textBox_CustomerName.Text;
            customer.PhoneNumber = int.Parse(textBox_PhoneNumber.Text);
            customer.Email = textBox_CustomerEmail.Text;
            customer.Country = textBox_CustomerCountry.Text;
            customer.City = textBox_CustomerCity.Text;
            customer.Address = textBox_CustomerAddress.Text;
            customer.Balance = float.Parse(textBox_CustomerBalance.Text);
            customer.idCustomer = int.Parse(txtBox_Customerid.Text);
            return customer;
        }

        /// <summary>
        ///     After validating updates the customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_updateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtBox_Customerid.Text, out var customerid))
            {
                var latestcustomerid = CustomerViewModel.returnLatestCustomerID();
                if (customerid <= latestcustomerid && customerid > -1)
                {
                    if (validate_customer())
                    {
                        var cust = createCustomerObject();
                        CustomerViewModel.updateCustomer(cust);
                        MessageBox.Show("Customer with ID " + cust.idCustomer + " was updated");
                        Btn_clearAll_Click(null, null);
                    }
                }
                else
                {
                    MessageBox.Show("Customer ID doesn't exist");
                    txtBox_Customerid.BorderBrush = Brushes.Red;
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid customer ID");
                txtBox_Customerid.BorderBrush = Brushes.Red;
            }
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Customer Name.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_CustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerName.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Customer Phone Number.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_CustomePhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_PhoneNumber.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Customer Email.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_CustomerEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerEmail.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Customer Country.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBlock_CustomerCountry_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerCountry.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Customer City.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBlock_CustomerCity_Stock_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerCity.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Customer Address.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBlock_CustomerAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerAddress.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Customer Balance.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBlock_CustomerBalance_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerBalance.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Customer ID.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBox_Customerid_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBox_Customerid.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Clear all inputs from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            textBox_CustomerName.ClearValue(Control.BorderBrushProperty);
            textBox_PhoneNumber.ClearValue(Control.BorderBrushProperty);
            textBox_CustomerEmail.ClearValue(Control.BorderBrushProperty);
            textBox_CustomerCountry.ClearValue(Control.BorderBrushProperty);
            textBox_CustomerCity.ClearValue(Control.BorderBrushProperty);
            textBox_CustomerAddress.ClearValue(Control.BorderBrushProperty);
            textBox_CustomerBalance.ClearValue(Control.BorderBrushProperty);
            txtBox_Customerid.ClearValue(Control.BorderBrushProperty);

            textBox_CustomerName.Clear();
            textBox_PhoneNumber.Clear();
            textBox_CustomerEmail.Clear();
            textBox_CustomerCountry.Clear();
            textBox_CustomerCity.Clear();
            textBox_CustomerAddress.Clear();
            textBox_CustomerBalance.Clear();
            txtBox_Customerid.Clear();
        }

        /// <summary>
        ///     After validating the ID loads the customer on the page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LoadCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customerid = -1;
            if (int.TryParse(txtBox_Customerid.Text, out var n))
            {
                customerid = int.Parse(txtBox_Customerid.Text);
                var latestcustomerid = CustomerViewModel.returnLatestCustomerID();
                if (customerid <= latestcustomerid && customerid > -1)
                {
                    var customer = CustomerViewModel.getCustomer(customerid);
                    textBox_CustomerName.Text = customer.CustomerName;
                    textBox_PhoneNumber.Text = customer.PhoneNumber.ToString();
                    textBox_CustomerEmail.Text = customer.Email;
                    textBox_CustomerCountry.Text = customer.Country;
                    textBox_CustomerCity.Text = customer.City;
                    textBox_CustomerAddress.Text = customer.Address;
                    textBox_CustomerBalance.Text = customer.Balance.ToString();
                }
                else
                {
                    MessageBox.Show("Customer ID doesn't exist");
                    txtBox_Customerid.BorderBrush = Brushes.Red;
                }
            }
            else
            {
                MessageBox.Show("Customer ID doesn't exist");
                txtBox_Customerid.BorderBrush = Brushes.Red;
            }
        }

        /// <summary>
        ///     Given the customer ID proceeds to load the customer
        /// </summary>
        /// <param name="custID"></param>
        public void loadCustomer(int custID)
        {
            txtBox_Customerid.Text = custID.ToString();
            Btn_LoadCustomer_Click(null, null);
        }
    }
}