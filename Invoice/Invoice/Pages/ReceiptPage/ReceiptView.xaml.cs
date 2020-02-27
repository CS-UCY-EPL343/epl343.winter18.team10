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

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    /// Interaction logic for ReceiptView.xaml
    /// </summary>
    public partial class ReceiptView : Page
    {
        private Receipt receipt;

        public ReceiptView()
        {
            InitializeComponent();
            InitializeComponent();
           
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_receiptNumber.Focus();
        }

        private void Btn_LoadReceipt_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_receiptNumber.Text, out int receiptID);
            if (receiptID > 0)
            {
                loadReceipt(receiptID);
            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for receipt ID.");
            }
        }

        public void loadReceipt(int receiptID)
        {
            receipt = ReceiptViewModel.getReceiptByID(receiptID);
            if (receipt != null)
            {
                // Customer details
                textBox_Customer.Text = receipt.customer.CustomerName;
                textBox_Contact_Details.Text = receipt.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = receipt.customer.Email;
                textBox_Address.Text = receipt.customer.Address + ", " + receipt.customer.City + ", " + receipt.customer.Country;

                // Receipt details
                txtBox_receiptNumber.Text = receipt.idReceipt.ToString();
                txtBox_receiptNumber.IsReadOnly = true;
                txtBox_receiptDate.Text = receipt.createdDate.ToString("d");
                txtBox_issuedBy.Text = receipt.issuedBy;               
                TotalAmount_TextBlock.Text = receipt.totalAmount.ToString("C");

                // Receipt payments           
                receiptPaymentsGrid.ItemsSource = receipt.payments;
            }
            else
            {
                MessageBox.Show("Receipt with ID = " + receiptID + ", does not exist");
            }
        }

        private void txtBox_receiptNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Btn_LoadReceipt_Click(null, null);
            }
        }

        private void Btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            this.receipt = null;
            foreach (var ctrl in grid_Customer.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            foreach (var ctrl in grid_Invoice.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            receiptPaymentsGrid.ItemsSource = null;            
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_receiptNumber.IsReadOnly = false;
            txtBox_receiptNumber.Focus();
        }

        #region PDF
        private void previewPdf_click(object sender, RoutedEventArgs e)
        {

        }

        private void printPdf_click(object sender, RoutedEventArgs e)
        {

        }

        private void savePdf_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_receiptNumber.Text, out int receiptID);
            if (txtBox_receiptNumber.IsReadOnly)
            {
                string msgtext = "You are about to delete the receipt with ID = " + receiptID + ". Are you sure?";
                string txt = "Delete Receipt";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        ReceiptViewModel.deleteReceiptByID(receiptID);
                        Btn_clearView_Click(null, null);
                        MessageBox.Show("Deleted Receipt with ID = " + receiptID);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                MessageBox.Show("No receipt is loaded");
            }
        }       

    }
}
