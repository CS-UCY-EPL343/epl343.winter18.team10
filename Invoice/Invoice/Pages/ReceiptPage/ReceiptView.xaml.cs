using InvoiceX.Models;
using InvoiceX.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        ReceiptMain receiptMain;

        public ReceiptView(ReceiptMain receiptMain)
        {
            InitializeComponent();
            this.receiptMain = receiptMain;
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
            receipt = ReceiptViewModel.getReceipt(receiptID);
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
        void savePdf_Click(object sender, RoutedEventArgs e)
        {
            MigraDoc.DocumentObjectModel.Document document = createPdf();
            document.UseCmykColor = true;
            // Create a renderer for PDF that uses Unicode font encoding
            MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save the PDF document...
            string filename = "Receipt" + txtBox_receiptNumber.Text + ".pdf";
            pdfRenderer.Save(filename);
            System.Diagnostics.Process.Start(filename);

        }

        void printPdf_click(object sender, RoutedEventArgs e)
        {
            //Create and save the pdf
            MigraDoc.DocumentObjectModel.Document document = createPdf();
            document.UseCmykColor = true;
            // Create a renderer for PDF that uses Unicode font encoding
            MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save the PDF document...
            string filename = "Receipt.pdf";
            pdfRenderer.Save(filename);
            //open adobe acrobat
            Process proc = new Process();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.Verb = "print";

            //Define location of adobe reader/command line
            //switches to launch adobe in "print" mode
            proc.StartInfo.FileName =
              @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
            proc.StartInfo.Arguments = String.Format(@"/p {0}", filename);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (proc.HasExited == false)
            {
                proc.WaitForExit(10000);
            }

            proc.EnableRaisingEvents = true;

            proc.Close();

        }

        private void previewPdf_click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Receipt_temp.pdf"))
            {
                File.Delete("Receipt_temp.pdf");
            }
            MigraDoc.DocumentObjectModel.Document document = createPdf();
            document.UseCmykColor = true;
            // Create a renderer for PDF that uses Unicode font encoding
            MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save the PDF document...
            string filename = "Receipt_temp.pdf";
            pdfRenderer.Save(filename);

            //open adobe acrobat
            Process proc = new Process();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.Verb = "print";

            //Define location of adobe reader/command line
            //switches to launch adobe in "print" mode
            proc.StartInfo.FileName =
              @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
            proc.StartInfo.Arguments = String.Format(@" {0}", filename);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (proc.HasExited == false)
            {
                proc.WaitForExit(10000);
            }

            proc.EnableRaisingEvents = true;

            proc.Close();
        }

        private MigraDoc.DocumentObjectModel.Document createPdf()
        {
            Receipt receipt = ReceiptViewModel.getReceipt(int.Parse(txtBox_receiptNumber.Text));
            Customer customer = receipt.customer;
            string[] customerDetails = new string[6];
            customerDetails[0] = customer.CustomerName;
            customerDetails[1] = customer.Address + ", " +
            customer.City + ", " + customer.Country;
            customerDetails[2] = customer.PhoneNumber.ToString();
            customerDetails[3] = customer.Email;
            customerDetails[4] = customer.Balance.ToString();
            customerDetails[5] = customer.idCustomer.ToString();
            MessageBox.Show(customer.idCustomer.ToString());

            string[] receiptDetails = new string[4];
            receiptDetails[0] = txtBox_receiptNumber.Text;
            receiptDetails[1] = txtBox_receiptDate.Text;
            receiptDetails[2] = txtBox_issuedBy.Text;
            receiptDetails[3] = receipt.totalAmount.ToString();


            List<Payment> payments = receiptPaymentsGrid.Items.OfType<Payment>().ToList();


            Forms.ReceiptForm receipt2 = new Forms.ReceiptForm("../../Forms/Receipt.xml", customerDetails, receiptDetails, payments);
            MigraDoc.DocumentObjectModel.Document document = receipt2.CreateDocument();
            return document;
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
                        ReceiptViewModel.deleteReceipt(receiptID);
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

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (receipt != null)
            {
                receiptMain.editReceipt(receipt.idReceipt);
            }
        }

        private void receiptPaymentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
