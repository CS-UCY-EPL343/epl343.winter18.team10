using InvoiceX.Models;
using InvoiceX.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace InvoiceX.Pages.StatementPage
{
    /// <summary>
    /// Interaction logic for StatementCreate.xaml
    /// </summary>
    public partial class StatementCreate : Page
    {
        CustomerViewModel customerView;
        StatementMain statementMain;

        public StatementCreate(StatementMain statementMain)
        {
            InitializeComponent();
            this.statementMain = statementMain;
            txtBlock_Balance.Text = 0.ToString("C");
        }

        public void load()
        {
            customerView = new CustomerViewModel();
            comboBox_customer.ItemsSource = customerView.customersList;
        }

        private void btn_createStatement_Click(object sender, RoutedEventArgs e)
        {
            if (checkCustomerForm() && dateRangeSelected())            
            {
                loadStatementItems();
            }
            else
            {
                MessageBox.Show("Please filled in missing values!");
            }
        }

        private void loadStatementItems()
        {
            int customerID = ((Customer)comboBox_customer.SelectedItem).idCustomer;
            DateTime from = fromDate.SelectedDate.Value.Date;
            from += new TimeSpan(0, 0, 0);  // start from 00:00:00 of from date
            DateTime to = toDate.SelectedDate.Value.Date;
            to += new TimeSpan(23, 59, 59); // end on 23:59:59 of to date

            List<StatementItem> statement = new List<StatementItem>();
            statement.AddRange(InvoiceViewModel.getInvoicesForStatement(customerID, from, to));
            statement.AddRange(CreditNoteViewModel.getCreditNotesForStatement(customerID, from, to));
            statement.AddRange(ReceiptViewModel.getReceiptsForStatement(customerID, from, to));

            statementDataGrid.ItemsSource = statement;
            var firstCol = statementDataGrid.Columns.First();
            firstCol.SortDirection = ListSortDirection.Ascending;
            statementDataGrid.Items.SortDescriptions.Add(new SortDescription(firstCol.SortMemberPath, ListSortDirection.Ascending));

            StatementItem item = (StatementItem)statementDataGrid.Items.GetItemAt(statementDataGrid.Items.Count - 1);
            txtBlock_Balance.Text = item.balance.ToString("C");
        }

        private bool dateRangeSelected()
        {
            bool response = true;
            if (fromDate.SelectedDate == null)
            {
                fromDate.BorderBrush = Brushes.Red;
                response =  false;
            }
            if (toDate.SelectedDate == null)
            {
                toDate.BorderBrush = Brushes.Red;
                response = false;
            }
            return response;
        }

        private bool checkCustomerForm()
        {
            if (comboBox_customer.SelectedIndex <= -1)
            {
                comboBox_customer_border.BorderBrush = Brushes.Red;
                comboBox_customer_border.BorderThickness = new Thickness(1);
                return false;
            }
            return true;
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            foreach (var ctrl in grid_Customer.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            fromDate.SelectedDate = null;
            toDate.SelectedDate = null;
            fromDate.ClearValue(DatePicker.BorderBrushProperty);
            toDate.ClearValue(DatePicker.BorderBrushProperty);
            issuedBy.Text = null;
            statementDataGrid.ItemsSource = null;
            txtBlock_Balance.Text = 0.ToString("C");
            load();
        }

        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            if (comboBox_customer.SelectedIndex > -1)
            {
                Customer customer = ((Customer)comboBox_customer.SelectedItem);
                textBox_Customer.Text = customer.CustomerName;
                textBox_Address.Text = customer.Address + ", " + customer.City + ", " + customer.Country;
                textBox_Contact_Details.Text = customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = customer.Email;
            }
        }

        private void fromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            fromDate.ClearValue(DatePicker.BorderBrushProperty);
        }

        private void toDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            toDate.ClearValue(DatePicker.BorderBrushProperty);
        }

        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
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
            string filename = "Statement.pdf";
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
            string filename = "Statement.pdf";
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
            if (File.Exists("Statement_temp.pdf"))
            {
                File.Delete("Statement_temp.pdf");
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
            string filename = "Statement_temp.pdf";
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
            string[] customerDetails = new string[6];
            Customer customer = (Customer)comboBox_customer.SelectedItem;
            Console.WriteLine(customer.CustomerName);
            customerDetails[0] = textBox_Customer.Text;
            customerDetails[1] = textBox_Address.Text;
            customerDetails[2] = textBox_Contact_Details.Text;
            customerDetails[3] = textBox_Email_Address.Text;
            customerDetails[4] = customer.idCustomer.ToString();
            customerDetails[5] = customer.Balance.ToString();

            string[] statementDetails = new string[4];
            statementDetails[0] = fromDate.Text;
            statementDetails[1] = toDate.Text;
            statementDetails[2] = issuedBy.Text;


            List<StatementItem> items = statementDataGrid.Items.OfType<StatementItem>().ToList();


            Forms.StatementForm statement2 = new Forms.StatementForm("../../Forms/Receipt.xml", customerDetails, statementDetails, items);
            MigraDoc.DocumentObjectModel.Document document = statement2.CreateDocument();
            return document;
        }

        #endregion

        private void ViewItem_Click(object sender, RoutedEventArgs e)
        {
            statementMain.viewItem((StatementItem)statementDataGrid.SelectedItem);
        }
    }
}
