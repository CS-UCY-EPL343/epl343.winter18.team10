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

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InvoiceX.Models;
using InvoiceX.ViewModels;

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    ///     Interaction logic for ReceiptEdit.xaml
    /// </summary>
    public partial class ReceiptEdit : Page
    {
        private CustomerViewModel customerView;
        private Receipt old_receipt;
        private bool receipt_loaded;
        private readonly ReceiptMain receiptMain;

        private bool Refresh_DB_data = true;

        public ReceiptEdit(ReceiptMain receiptMain)
        {
            InitializeComponent();
            this.receiptMain = receiptMain;
            TotalAmount_TextBlock.Text = 0.ToString("C");
        }

        public void load()
        {
            if (Refresh_DB_data)
            {
                customerView = new CustomerViewModel();
                PaymentDate.SelectedDate = DateTime.Today; //set curent date 
            }

            Refresh_DB_data = false;
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox paymentMethod.
        ///     Clears the red border and adds text based on selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_PaymentMethod.SelectedIndex > -1)
                comboBox_paymentMethod_border.BorderThickness = new Thickness(0);
            if (comboBox_PaymentMethod.SelectedIndex == 0)
            {
                textBox_paymentNum.IsReadOnly = true;
                textBox_paymentNum.Text = "No number needed ";
            }
            else
            {
                textBox_paymentNum.IsReadOnly = false;
                textBox_paymentNum.Clear();
            }
        }

        /// <summary>
        ///     Checks if a payment has already been selected
        /// </summary>
        /// <returns></returns>
        private bool paymenttype_already_selected()
        {
            var gridPayments = ReceiptDataGrid.Items.OfType<Payment>().ToList();
            Enum.TryParse(comboBox_PaymentMethod.Text, out PaymentMethod paymentenum);
            foreach (var p in gridPayments)
                if (p.paymentMethod == paymentenum)
                {
                    MessageBox.Show("Payment method already selected");
                    return true;
                }

            return false;
        }

        /// <summary>
        ///     Checks if the payment details are all completed and valid
        /// </summary>
        /// <returns></returns>
        private bool Check_AddPayment_CompletedValues()
        {
            var all_completed = true;
            if (comboBox_PaymentMethod.SelectedIndex <= -1) //|| paymenttype_already_selected()
            {
                all_completed = false;
                comboBox_paymentMethod_border.BorderBrush = Brushes.Red;
                comboBox_paymentMethod_border.BorderThickness = new Thickness(1);
            }

            if (string.IsNullOrWhiteSpace(textBox_paymentNum.Text))
            {
                all_completed = false;
                textBox_paymentNum.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_paymentNum.ClearValue(Control.BorderBrushProperty);
            }

            if (!float.TryParse(textBox_amount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var f) ||
                f <= 0)
            {
                all_completed = false;
                textBox_amount.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_amount.ClearValue(Control.BorderBrushProperty);
            }

            if (PaymentDate.SelectedDate == null) PaymentDate.SelectedDate = DateTime.Today; //set curent date 

            return all_completed;
        }

        /// <summary>
        ///     Adds the payment selected to the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddPayment(object sender, RoutedEventArgs e)
        {
            if (Check_AddPayment_CompletedValues() && receipt_loaded)
            {
                Enum.TryParse(comboBox_PaymentMethod.Text, out PaymentMethod paymentenum);
                ReceiptDataGrid.Items.Add(new Payment
                {
                    //idReceipt = Int32.Parse(textBox_ReceiptNumber.Text),
                    amount = float.Parse(textBox_amount.Text.Replace(',', '.'),
                        CultureInfo.InvariantCulture.NumberFormat),
                    paymentNumber = comboBox_PaymentMethod.SelectedIndex == 0 ? "" : textBox_paymentNum.Text,
                    paymentDate = PaymentDate.SelectedDate.Value.Date,
                    paymentMethod = paymentenum
                });

                double total_amount = 0;
                total_amount = double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
                total_amount = total_amount +
                               double.Parse(textBox_amount.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
                TotalAmount_TextBlock.Text = total_amount.ToString("C");
            }
        }

        /// <summary>
        ///     Removes a payment from the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_CreateReceipt_REMOVE(object sender, RoutedEventArgs e)
        {
            var CurrentCell_Product = (Payment) ReceiptDataGrid.CurrentCell.Item;
            var total_amount = double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
            total_amount = total_amount - Convert.ToDouble(CurrentCell_Product.amount);
            TotalAmount_TextBlock.Text = total_amount.ToString("C");
            ReceiptDataGrid.Items.Remove(ReceiptDataGrid.CurrentCell.Item);
        }

        /// <summary>
        ///     Clears the payment's information from all the textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            comboBox_PaymentMethod.SelectedIndex = -1;
            textBox_paymentNum.Text = "";
            textBox_amount.Text = "";
            comboBox_paymentMethod_border.BorderThickness = new Thickness(0);
            textBox_paymentNum.ClearValue(Control.BorderBrushProperty);
            textBox_amount.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Checks that the receipt details are completed
        /// </summary>
        /// <returns></returns>
        private bool Check_DetailsForm()
        {
            if (issuedBy.Text.Equals(""))
            {
                issuedBy.BorderBrush = Brushes.Red;
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Checks if the grid has payments in it
        /// </summary>
        /// <returns></returns>
        private bool Has_Items_Selected()
        {
            if (ReceiptDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selected any payments");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Creates and returns the Receipt based on the information on the page
        /// </summary>
        /// <returns></returns>
        private Receipt createReceiptObject()
        {
            Receipt myReceipt;
            myReceipt = new Receipt();
            myReceipt.createdDate = old_receipt.createdDate;
            myReceipt.idReceipt = Convert.ToInt32(textBox_ReceiptNumber.Text);
            myReceipt.customerName = old_receipt.customerName;
            myReceipt.customer = old_receipt.customer;
            myReceipt.issuedBy = issuedBy.Text;
            myReceipt.totalAmount = float.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
            myReceipt.payments = ReceiptDataGrid.Items.OfType<Payment>().ToList();
            return myReceipt;
        }

        /// <summary>
        ///     After validating updates the receipt and switches to viewing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            var ALL_VALUES_OK = true;
            //if (!Check_CustomerForm()) ALL_VALUES_OK = false;
            if (!Check_DetailsForm()) ALL_VALUES_OK = false;
            if (!Has_Items_Selected()) ALL_VALUES_OK = false;
            if (ALL_VALUES_OK)
            {
                ReceiptViewModel.updateReceipt(createReceiptObject(), old_receipt);
                receiptMain.viewReceipt(old_receipt.idReceipt);
                Btn_clearAll_Click(null, null);
            }
        }

        /// <summary>
        ///     Clears all the customet information
        /// </summary>
        private void Clear_Customer()
        {
            //comboBox_customer.SelectedIndex = -1;
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        /// <summary>
        ///     Clears all the receipt details
        /// </summary>
        private void Clear_Details()
        {
            issuedBy.Text = "";
            textBox_ReceiptNumber.Clear();
            issuedBy.ClearValue(Control.BorderBrushProperty);
            txtbox_ReceiptDate.Clear();
        }

        /// <summary>
        ///     Clears the grid
        /// </summary>
        private void Clear_ProductGrid()
        {
            ReceiptDataGrid.Items.Clear();
            TotalAmount_TextBlock.Text = 0.ToString("C");
        }

        /// <summary>
        ///     Clears all inputs from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            receipt_loaded = false;
            Btn_clearProduct_Click(new object(), new RoutedEventArgs());
            Clear_Customer();
            Clear_Details();
            Clear_ProductGrid();
            Refresh_DB_data = true;
            load();
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox IssuedBy.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     After validating the receipt ID calls loadReceipt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LoadReceipt_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(textBox_ReceiptNumber.Text, out var receiptID);
            if (ReceiptViewModel.receiptExists(receiptID))
            {
                Btn_clearAll_Click(null, null);
                loadReceipt(receiptID);
                receipt_loaded = true;
            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for receipt ID.");
            }
        }

        /// <summary>
        ///     Loads the receipt information on the page
        /// </summary>
        /// <param name="receiptID"></param>
        public void loadReceipt(int receiptID)
        {
            old_receipt = ReceiptViewModel.getReceipt(receiptID);
            if (old_receipt != null)
            {
                // Customer details
                textBox_Customer.Text = old_receipt.customer.CustomerName;
                textBox_Contact_Details.Text = old_receipt.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = old_receipt.customer.Email;
                textBox_Address.Text = old_receipt.customer.Address + ", " + old_receipt.customer.City + ", " +
                                       old_receipt.customer.Country;
                // Receipt details
                textBox_ReceiptNumber.Text = old_receipt.idReceipt.ToString();
                txtbox_ReceiptDate.Text = old_receipt.createdDate.ToString("d");
                issuedBy.Text = old_receipt.issuedBy;
                TotalAmount_TextBlock.Text = old_receipt.totalAmount.ToString("C");

                // Receipt payments           
                foreach (var p in old_receipt.payments) ReceiptDataGrid.Items.Add(p);
            }
            else
            {
                MessageBox.Show("Receipt with ID = " + receiptID + ", does not exist");
            }
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox payment num.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_paymentNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_paymentNum.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox amount.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_amount.ClearValue(Control.BorderBrushProperty);
        }
    }
}