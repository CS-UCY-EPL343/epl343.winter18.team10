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

namespace InvoiceX.Pages.CreditNotePage
{
    /// <summary>
    /// Interaction logic for CreditNoteView.xaml
    /// </summary>
    public partial class CreditNoteView : Page
    {
        private CreditNote creditNote;
        CreditNoteMain mainPage;
        public CreditNoteView(CreditNoteMain creditNoteMain)
        {
            this.mainPage = creditNoteMain;

            InitializeComponent();
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_creditNoteNumber.Focus();
        }      
       
        private void Btn_LoadCreditNote_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_creditNoteNumber.Text, out int creditNoteID);
            if (creditNoteID > 0)
            {
                loadCreditNote(creditNoteID);
            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for credit note ID.");
            }
        }

        public void loadCreditNote(int creditNoteID)
        {
            creditNote = CreditNoteViewModel.getCreditNote(creditNoteID);
            if (creditNote != null)
            {
                // Customer details
                textBox_Customer.Text = creditNote.customer.CustomerName;
                textBox_Contact_Details.Text = creditNote.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = creditNote.customer.Email;
                textBox_Address.Text = creditNote.customer.Address + ", " + creditNote.customer.City + ", " + creditNote.customer.Country;

                // Credit Note details
                txtBox_creditNoteNumber.Text = creditNote.idCreditNote.ToString();
                txtBox_creditNoteNumber.IsReadOnly = true;
                txtBox_createdDate.Text = creditNote.createdDate.ToString("d");
                txtBox_issuedBy.Text = creditNote.issuedBy;
                NetTotal_TextBlock.Text = creditNote.cost.ToString("C");
                Vat_TextBlock.Text = creditNote.VAT.ToString("C");
                TotalAmount_TextBlock.Text = creditNote.totalCost.ToString("C");

                // Credit Note products           
                creditNoteProductsGrid.ItemsSource = creditNote.products;
            }
            else
            {
                MessageBox.Show("Credit Note with ID = " + creditNoteID + ", does not exist");
            }
        }

        private void txtBox_creditNoteNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Btn_LoadCreditNote_Click(null, null);
            }
        }

        private void Btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            this.creditNote = null;
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
            creditNoteProductsGrid.ItemsSource = null;
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_creditNoteNumber.IsReadOnly = false;
            txtBox_creditNoteNumber.Focus();
        }

        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_creditNoteNumber.Text, out int creditNoteID);
            if (txtBox_creditNoteNumber.IsReadOnly)
            {
                string msgtext = "You are about to delete the credit note with ID = " + creditNoteID + ". Are you sure?";
                string txt = "Delete Credit Note";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        CreditNoteViewModel.deleteCreditNote(creditNoteID);
                        Btn_clearView_Click(null, null);
                        MessageBox.Show("Deleted Credit Note with ID = " + creditNoteID);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                MessageBox.Show("No credit note is loaded");
            }
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (creditNote != null)
            {
                mainPage.editCreditNote(creditNote.idCreditNote);
            }
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
            string filename = "creditNote.pdf";
            filename = "creditNote.pdf";
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
            string filename = "CreditNote.pdf";
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
            if (File.Exists("creditNote_temp.pdf"))
            {
                File.Delete("creditNote_temp.pdf");
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
            string filename = "creditNote_temp.pdf";
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
            CreditNote creditNote = CreditNoteViewModel.getCreditNote(int.Parse(txtBox_creditNoteNumber.Text));
            Customer customer = creditNote.customer;
            string[] customerDetails = new string[6];
            customerDetails[0] = customer.CustomerName;
            customerDetails[1] = customer.Address + ", " +
            customer.City + ", " + customer.Country;
            customerDetails[2] = customer.PhoneNumber.ToString();
            customerDetails[3] = customer.Email;
            customerDetails[4] = customer.Balance.ToString();
            customerDetails[5] = customer.idCustomer.ToString();

            string[] creditNoteDetails = new string[6];
            creditNoteDetails[0] = txtBox_creditNoteNumber.Text;
            Console.WriteLine(txtBox_creditNoteNumber.Text);
            creditNoteDetails[1] = txtBox_createdDate.Text;
            creditNoteDetails[2] = txtBox_issuedBy.Text;
            creditNoteDetails[3] = NetTotal_TextBlock.Text;
            creditNoteDetails[4] = Vat_TextBlock.Text;
            creditNoteDetails[5] = TotalAmount_TextBlock.Text;

            List<Product> products = creditNoteProductsGrid.Items.OfType<Product>().ToList();


            Forms.CreditNoteForm creditnote2 = new Forms.CreditNoteForm("../../Forms/creditNote.xml", customerDetails, creditNoteDetails, products);
            MigraDoc.DocumentObjectModel.Document document = creditnote2.CreateDocument();
            return document;
        }
        #endregion
    }
}
