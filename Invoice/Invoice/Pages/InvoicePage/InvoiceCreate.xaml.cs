using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    /// Interaction logic for InvoiceCreate.xaml
    /// </summary>
    public partial class InvoiceCreate : Page
    {
        ProductViewModel productView;
        UserViewModel userView;
        CustomerViewModel customerView;
       

        public InvoiceCreate()
        {
            InitializeComponent();
        }

        public void load()
        {
            Btn_clearProduct_Click(new object(),new RoutedEventArgs());
            productView = new ProductViewModel();
            userView = new UserViewModel();
            customerView = new CustomerViewModel();

            issuedBy.ItemsSource = userView.UsersList;
            comboBox_customer.ItemsSource = customerView.CustomersList;
            comboBox_Product.ItemsSource = productView.ProductList;
            textBox_invoiceNumber.Text=ReturnLatestInvoiceID();
        }

        string ReturnLatestInvoiceID() {

            int id_return = 0;
            MySqlConnection conn;
            MySqlCommand SQLCommand;
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

            try
            {
                string idInvoice;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idInvoice FROM Invoice ORDER BY idInvoice DESC LIMIT 1", conn);
                conn.Open();
                id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idInvoice = Convert.ToString(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idInvoice = "";

                conn.Close();
                return ((Convert.ToInt32(idInvoice)+1).ToString());

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return "0";
           
        }
        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_customer.SelectedIndex != -1)
            {
                textBox_Customer.Text = ((Customers)comboBox_customer.SelectedItem).CustomerName;
                textBox_Address.Text = ((Customers)comboBox_customer.SelectedItem).Address + ", " +
                 ((Customers)comboBox_customer.SelectedItem).City + ", " + ((Customers)comboBox_customer.SelectedItem).Country;
                textBox_Contact_Details.Text = ((Customers)comboBox_customer.SelectedItem).PhoneNumber.ToString();
                textBox_Email_Address.Text = ((Customers)comboBox_customer.SelectedItem).Email;
            }


        }
        private string filenamePath = null;
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
            string filename = "Invoice.pdf";
            filename = "Invoice.pdf";
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
            string filename = "Invoice.pdf";
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
            if (File.Exists("Invoice_temp.pdf"))
            {
                File.Delete("Invoice_temp.pdf");
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
            string filename = "Invoice_temp.pdf";
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

        MigraDoc.DocumentObjectModel.Document createPdf()
        {

            string[] customerDetails = new string[6];
            customerDetails[0] = ((Customers)comboBox_customer.SelectedItem).CustomerName;
            customerDetails[1] = ((Customers)comboBox_customer.SelectedItem).Address + ", " +
            ((Customers)comboBox_customer.SelectedItem).City + ", " + ((Customers)comboBox_customer.SelectedItem).Country;
            customerDetails[2] = ((Customers)comboBox_customer.SelectedItem).PhoneNumber.ToString();
            customerDetails[3] = ((Customers)comboBox_customer.SelectedItem).Email;
            customerDetails[4] = ((Customers)comboBox_customer.SelectedItem).Balance.ToString();
            customerDetails[5] = ((Customers)comboBox_customer.SelectedItem).idCustomer.ToString();

            string[] invoiceDetails = new string[6];
            invoiceDetails[0] = textBox_invoiceNumber.Text;
            Console.WriteLine(textBox_invoiceNumber.Text);
            invoiceDetails[1] = invoiceDate.SelectedDate.Value.ToString("dd/MM/yyyy");
            invoiceDetails[2] = issuedBy.Text;
            invoiceDetails[3] = NetTotal_TextBlock.Text;
            invoiceDetails[4] = Vat_TextBlock.Text;
            invoiceDetails[5] = TotalAmount_TextBlock.Text;

            List<Product> products = invoiceDataGrid2.Items.OfType<Product>().ToList();


            Forms.InvoiceForm invoice = new Forms.InvoiceForm("../../Forms/Invoice.xml", customerDetails, invoiceDetails, products);
            MigraDoc.DocumentObjectModel.Document document = invoice.CreateDocument();
            return document;

        }



        

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_Product.SelectedIndex > -1)
            {

                comboBox_Product_border.BorderThickness = new Thickness(0);
                textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
                textBox_ProductQuantity.ClearValue(TextBox.BorderBrushProperty);
                textBox_Product.Text = ((Product)comboBox_Product.SelectedItem).ProductName;
                textBox_ProductQuantity.Text = ((Product)comboBox_Product.SelectedItem).Quantity.ToString();
                textBox_ProductDescription.Text = ((Product)comboBox_Product.SelectedItem).ProductDescription;
                textBox_ProductStock.Text = ((Product)comboBox_Product.SelectedItem).Stock.ToString();
                textBox_ProductPrice.Text = ((Product)comboBox_Product.SelectedItem).SellPrice.ToString();
                textBox_ProductVat.Text = (((Product)comboBox_Product.SelectedItem).Vat * 100).ToString();
            }

        }

        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {

            int n;
            if (int.TryParse(textBox_ProductQuantity.Text, out n) &&  float.TryParse(textBox_ProductPrice.Text, out float f)&&(comboBox_Product.SelectedIndex > -1))
            {
                textBox_ProductAmount.Text = (Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
            }


        }

        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            int n;
            if (float.TryParse(textBox_ProductPrice.Text, out float f) && int.TryParse(textBox_ProductQuantity.Text, out n) && (comboBox_Product.SelectedIndex > -1))
            {
                textBox_ProductAmount.Text = (Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
            }
        }

        private bool Check_AddProduct_ComplitedValues() {
            bool all_completed = true;
            int n;
            if (comboBox_Product.SelectedIndex <= -1) {
                all_completed = false;
                comboBox_Product_border.BorderBrush = Brushes.Red;
                comboBox_Product_border.BorderThickness = new Thickness(1);
            }
            if (!int.TryParse(textBox_ProductQuantity.Text, out n)){
                all_completed = false;
                textBox_ProductQuantity.BorderBrush = Brushes.Red;
            }
            if (!int.TryParse(textBox_ProductPrice.Text, out n)) {
                all_completed = false;
                textBox_ProductPrice.BorderBrush = Brushes.Red;
            }


                return all_completed;
        }

        private void Btn_AddProduct(object sender, RoutedEventArgs e)
        {
            if (Check_AddProduct_ComplitedValues())
            {
                invoiceDataGrid2.Items.Add(new Product
                {
                    ProductName = textBox_Product.Text,
                    ProductDescription = textBox_ProductDescription.Text,
                    Stock = Convert.ToInt32(textBox_ProductQuantity.Text),
                    SellPrice = Convert.ToDouble(textBox_ProductPrice.Text),
                    Quantity = Convert.ToInt32(textBox_ProductQuantity.Text),
                    Total = Convert.ToDouble(textBox_ProductAmount.Text),
                    Vat = ((Product)comboBox_Product.SelectedItem).Vat
                });

                double NetTotal_TextBlock_var = 0;
                NetTotal_TextBlock_var = Convert.ToDouble(NetTotal_TextBlock.Text);
                NetTotal_TextBlock_var = NetTotal_TextBlock_var + Convert.ToDouble(textBox_ProductAmount.Text);
                NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString("n2");
                double Vat_TextBlock_var = 0;
                Vat_TextBlock_var = Convert.ToDouble(Vat_TextBlock.Text);
                Vat_TextBlock_var = Vat_TextBlock_var + (Convert.ToDouble(textBox_ProductAmount.Text) * ((Product)comboBox_Product.SelectedItem).Vat);
                Vat_TextBlock.Text = (Vat_TextBlock_var).ToString("n2");
                TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + Vat_TextBlock_var).ToString("n2");
            }
        }

        private void Button_Click_CreateInvoice_REMOVE(object sender, RoutedEventArgs e)
        {

            Product CurrentCell_Product = (Product)(invoiceDataGrid2.CurrentCell.Item);
            double NetTotal_TextBlock_var = 0;
            NetTotal_TextBlock_var = Convert.ToDouble(NetTotal_TextBlock.Text);
            NetTotal_TextBlock_var = NetTotal_TextBlock_var - Convert.ToDouble(CurrentCell_Product.Total);
            NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString("n2");
            Vat_TextBlock.Text = (NetTotal_TextBlock_var * (CurrentCell_Product.Vat)).ToString("n2");
            TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + (NetTotal_TextBlock_var * (CurrentCell_Product.Vat))).ToString("n2");
            invoiceDataGrid2.Items.Remove(invoiceDataGrid2.CurrentCell.Item);

        }
        /*remove txt from txtbox when clicked (Put GotFocus="TextBox_GotFocus" in txtBox)*/
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        public void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            comboBox_Product.SelectedIndex = -1;
            textBox_Product.Text = "";
            textBox_ProductDescription.Text = "";
            textBox_ProductStock.Text = "";
            textBox_ProductQuantity.Text = "";
            textBox_ProductPrice.Text = "";
            textBox_ProductVat.Text = "";
            textBox_ProductAmount.Text = "";
            comboBox_Product_border.BorderThickness = new Thickness(0);
            textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
            textBox_ProductQuantity.ClearValue(TextBox.BorderBrushProperty);

        }
    }
}
