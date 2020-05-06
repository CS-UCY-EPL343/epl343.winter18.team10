using InvoiceX.Models;
using InvoiceX.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    /// Interaction logic for ReceiptEdit.xaml
    /// </summary>
    public partial class ReceiptEdit : Page
    {
        CustomerViewModel customerView;
        ReceiptMain receiptMain;

        bool Refresh_DB_data = true;
        private Receipt old_receipt;
        bool receipt_loaded = false;

        public ReceiptEdit(ReceiptMain receiptMain)
        {
            InitializeComponent();
            this.receiptMain = receiptMain;
            TotalAmount_TextBlock.Text = (0).ToString("C");
        }

        public void load()
        {
            if (Refresh_DB_data)
            {                
                customerView = new CustomerViewModel();  
                PaymentDate.SelectedDate = DateTime.Today;//set curent date 

            }
            Refresh_DB_data = false;
        }             
        

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_PaymentMethod.SelectedIndex > -1)
            {
                comboBox_paymentMethod_border.BorderThickness = new Thickness(0);
               
            }
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

        bool paymenttype_already_selected()
        {
            List<Payment> gridPayments = ReceiptDataGrid.Items.OfType<Payment>().ToList();
            Enum.TryParse(comboBox_PaymentMethod.Text, out PaymentMethod paymentenum);
            foreach (Payment p in gridPayments)
            {
                if (p.paymentMethod== paymentenum)
                {
                    MessageBox.Show("Payment method already selected");
                    return true;
                }
            }
            return false;
        }

        private bool Check_AddPayment_CompletedValues()
        {
            bool all_completed = true;
            if ((comboBox_PaymentMethod.SelectedIndex <= -1))//|| paymenttype_already_selected()
            {
                all_completed = false;
                comboBox_paymentMethod_border.BorderBrush = Brushes.Red;
                comboBox_paymentMethod_border.BorderThickness = new Thickness(1);
            }
            if (string.IsNullOrWhiteSpace(textBox_paymentNum.Text.ToString())) 
            {
                all_completed = false;
                textBox_paymentNum.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_paymentNum.ClearValue(TextBox.BorderBrushProperty);
            }

            if ((!float.TryParse(textBox_amount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out float f))|| (f <= 0))
            {
                all_completed = false;
                textBox_amount.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_amount.ClearValue(TextBox.BorderBrushProperty);
            }
            if (PaymentDate.SelectedDate == null)
            {
                PaymentDate.SelectedDate = DateTime.Today;//set curent date 
            }

            return all_completed;
        }

        private void Btn_AddPayment(object sender, RoutedEventArgs e)
        {
            if (Check_AddPayment_CompletedValues() && receipt_loaded)
            {
                Enum.TryParse(comboBox_PaymentMethod.Text, out PaymentMethod paymentenum);
                ReceiptDataGrid.Items.Add(new Payment
                {                   
                    //idReceipt = Int32.Parse(textBox_ReceiptNumber.Text),
                    amount = float.Parse(textBox_amount.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat),
                    paymentNumber = (comboBox_PaymentMethod.SelectedIndex == 0 ? "" : textBox_paymentNum.Text),
                    paymentDate = PaymentDate.SelectedDate.Value.Date,
                    paymentMethod = paymentenum

                });

                double total_amount = 0;
                total_amount = Double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
                total_amount = total_amount + Double.Parse(textBox_amount.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
                TotalAmount_TextBlock.Text = (total_amount).ToString("C");
            }
        }

        private void Button_Click_CreateReceipt_REMOVE(object sender, RoutedEventArgs e)
        {

            Payment CurrentCell_Product = (Payment)(ReceiptDataGrid.CurrentCell.Item);
            double total_amount = double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
            total_amount = total_amount - Convert.ToDouble(CurrentCell_Product.amount);  
            TotalAmount_TextBlock.Text = (total_amount).ToString("C");
            ReceiptDataGrid.Items.Remove(ReceiptDataGrid.CurrentCell.Item);
        }             

        public void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            comboBox_PaymentMethod.SelectedIndex = -1;
            textBox_paymentNum.Text = "";
            textBox_amount.Text = "";           
            comboBox_paymentMethod_border.BorderThickness = new Thickness(0);
            textBox_paymentNum.ClearValue(TextBox.BorderBrushProperty);
            textBox_amount.ClearValue(TextBox.BorderBrushProperty);
        }
        /*
        private bool Check_CustomerForm()
        {
            if (comboBox_customer.SelectedIndex <= -1)
            {
                comboBox_customer_border.BorderBrush = Brushes.Red;
                comboBox_customer_border.BorderThickness = new Thickness(1);
                return false;
            }
            return true;
        }
        */
        private bool Check_DetailsForm()
        {
            if (issuedBy.Text.Equals(""))
            {
                issuedBy.BorderBrush = Brushes.Red;
                return false;
            }
            return true;
        }

        private bool Has_Items_Selected()
        {
            if (ReceiptDataGrid.Items.Count == 0)//vale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxo
            {
                MessageBox.Show("You havent selectet any products");
                return false;
            }
            return true;
        }
        
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

        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            bool ALL_VALUES_OK = true;
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

        private void Clear_Customer()
        {
            //comboBox_customer.SelectedIndex = -1;
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        private void Clear_Details()
        {
            issuedBy.Text = "";
            textBox_ReceiptNumber.Clear();
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
            txtbox_ReceiptDate.Clear();
        }

        private void Clear_ProductGrid()
        {
            ReceiptDataGrid.Items.Clear();            
            TotalAmount_TextBlock.Text = (0).ToString("C");
        }

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

        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }

        private void Btn_LoadReceipt_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(textBox_ReceiptNumber.Text, out int receiptID);
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

        public void loadReceipt(int receiptID)
        {
            old_receipt = ReceiptViewModel.getReceipt(receiptID);
            if (old_receipt != null)
            {
                // Customer details
                textBox_Customer.Text = old_receipt.customer.CustomerName;
                textBox_Contact_Details.Text = old_receipt.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = old_receipt.customer.Email;
                textBox_Address.Text = old_receipt.customer.Address + ", " + old_receipt.customer.City + ", " + old_receipt.customer.Country;
                // Receipt details
                textBox_ReceiptNumber.Text = old_receipt.idReceipt.ToString();
                txtbox_ReceiptDate.Text = old_receipt.createdDate.ToString("d");
                issuedBy.Text = old_receipt.issuedBy;
                TotalAmount_TextBlock.Text = old_receipt.totalAmount.ToString("C");

                // Receipt payments           
                foreach (Payment p in old_receipt.payments) 
                {
                    ReceiptDataGrid.Items.Add(p);
                }
            }
            else
            {
                MessageBox.Show("Receipt with ID = " + receiptID + ", does not exist");
            }
        }

        private void textBox_paymentNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_paymentNum.ClearValue(TextBox.BorderBrushProperty);

        }

        private void textBox_amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_amount.ClearValue(TextBox.BorderBrushProperty);
        }
    }
}
