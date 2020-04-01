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
    /// Interaction logic for ReceiptCreate.xaml
    /// </summary>
    public partial class ReceiptCreate : Page
    {
        CustomerViewModel customerView;
        ReceiptMain receiptMain;
        bool Refresh_DB_data = true;

        public ReceiptCreate(ReceiptMain receiptMain)
        {
            InitializeComponent();
            this.receiptMain = receiptMain;
            TotalAmount_TextBlock.Text = (0).ToString("C");
        }

        public void load()
        {
            if (Refresh_DB_data)
            {                
                //productView = new ProductViewModel();               
                customerView = new CustomerViewModel();               
                comboBox_customer.ItemsSource = customerView.customersList;
                //comboBox_PaymentMethod.ItemsSource = productView.ProductList;
                textBox_ReceiptNumber.Text = (ReceiptViewModel.returnLatestReceiptID()+1).ToString();
                ReceiptDate.SelectedDate = DateTime.Today;//set curent date 
                PaymentDate.SelectedDate = DateTime.Today;//set curent date 
            }
            Refresh_DB_data = false;
        }        


        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            if (comboBox_customer.SelectedIndex > -1)
            {
                textBox_Customer.Text = ((Customer)comboBox_customer.SelectedItem).CustomerName;
                textBox_Address.Text = ((Customer)comboBox_customer.SelectedItem).Address + ", " +
                 ((Customer)comboBox_customer.SelectedItem).City + ", " + ((Customer)comboBox_customer.SelectedItem).Country;
                textBox_Contact_Details.Text = ((Customer)comboBox_customer.SelectedItem).PhoneNumber.ToString();
                textBox_Email_Address.Text = ((Customer)comboBox_customer.SelectedItem).Email;
            }
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
                textBox_paymentNum.Text = "No number needed";

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
            if (Check_AddPayment_CompletedValues())
            {
                Enum.TryParse(comboBox_PaymentMethod.Text, out PaymentMethod paymentenum);
                ReceiptDataGrid.Items.Add(new Payment
                {                   
                    //idReceipt = Int32.Parse(textBox_ReceiptNumber.Text),
                    amount = float.Parse(textBox_amount.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat),
                    paymentNumber =(comboBox_PaymentMethod.SelectedIndex == 0 ? "": textBox_paymentNum.Text) ,
                    paymentDate = PaymentDate.SelectedDate.Value.Date,
                    paymentMethod = paymentenum

                });

                double total_amount = 0;
                total_amount = Double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
                total_amount = total_amount + Double.Parse(textBox_amount.Text.Replace(',','.'), CultureInfo.InvariantCulture);
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
            myReceipt.createdDate = ReceiptDate.SelectedDate.Value.Date;
            //myReceipt.status=
            myReceipt.idReceipt = Convert.ToInt32(textBox_ReceiptNumber.Text);
            myReceipt.customerName = ((Customer)comboBox_customer.SelectedItem).CustomerName;
            myReceipt.customer = ((Customer)comboBox_customer.SelectedItem);
            myReceipt.issuedBy = issuedBy.Text;
            myReceipt.totalAmount = float.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
            myReceipt.payments = ReceiptDataGrid.Items.OfType<Payment>().ToList();
            return myReceipt;
        }             

        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            bool ALL_VALUES_OK = true;
            if (!Check_CustomerForm()) ALL_VALUES_OK = false;
            if (!Check_DetailsForm()) ALL_VALUES_OK = false;
            if (!Has_Items_Selected()) ALL_VALUES_OK = false;
            if (ALL_VALUES_OK)
            {
                Receipt rec = createReceiptObject();
                ReceiptViewModel.insertReceipt(rec);
                MessageBox.Show("Receipt with ID " + rec.idReceipt + " was created.");
                receiptMain.viewReceipt(rec.idReceipt);
                Btn_clearAll_Click(null, null);
            }
        }

        private void Clear_Customer()
        {
            comboBox_customer.SelectedIndex = -1;
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        private void Clear_Details()
        {
            issuedBy.Text = "";
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }

        private void Clear_ProductGrid()
        {
            ReceiptDataGrid.Items.Clear();            
            TotalAmount_TextBlock.Text = (0).ToString("C");
        }

        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
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
