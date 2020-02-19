using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace InvoiceX.Pages.ProductPage
{
    /// <summary>
    /// Interaction logic for ProductCreate.xaml
    /// </summary>
    public partial class ProductCreate : Page
    {
       
        public ProductCreate()
        {
            InitializeComponent();
        }

        internal void load()
        {
          
             
           
        }

        void SendProductToDB()
        {
            MySqlConnection conn;
            string myConnectionString;
            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                //insert Invoice 
                string query = "INSERT INTO Product (ProductName, Description, Stock, MinStock, Cost, SellPrice, VAT,Category) Values (@ProductName, @Description, @Stock, @MinStock, @Cost, @SellPrice, @VAT,@Category)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@ProductName", textBox_ProductName.Text);
                    cmd.Parameters.AddWithValue("@Description", textBox_ProductDescription.Text);
                    cmd.Parameters.AddWithValue("@Stock", txtBlock_ProductCurrentStock.Text);
                    cmd.Parameters.AddWithValue("@MinStock", txtBlock_ProductMinimun_Stock.Text);
                    cmd.Parameters.AddWithValue("@Cost", double.Parse(txtBlock_ProductCost.Text));
                    cmd.Parameters.AddWithValue("@VAT", double.Parse(txtBlock_ProductVat.Text));
                    cmd.Parameters.AddWithValue("@Category", textBox_ProductCategory.Text);
                    cmd.Parameters.AddWithValue("@SellPrice", double.Parse(txtBlock_ProductSellPrice.Text));
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
                MessageBox.Show("Product was send to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        private void Btn_createProduct_Click(object sender, RoutedEventArgs e)
        {
            bool ProductCreateOK = true; 
            if (string.IsNullOrWhiteSpace(textBox_ProductName.Text)) { textBox_ProductName.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_ProductCategory.Text)) { textBox_ProductCategory.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_ProductDescription.Text)) { textBox_ProductDescription.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!int.TryParse(txtBlock_ProductCurrentStock.Text, out int a)) { txtBlock_ProductCurrentStock.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!int.TryParse(txtBlock_ProductMinimun_Stock.Text, out a)) { txtBlock_ProductMinimun_Stock.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!double.TryParse(txtBlock_ProductSellPrice.Text, out double b)) { txtBlock_ProductSellPrice.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!double.TryParse(txtBlock_ProductCost.Text, out b)) { txtBlock_ProductCost.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!double.TryParse(txtBlock_ProductVat.Text, out b)) { txtBlock_ProductVat.BorderBrush = Brushes.Red; ProductCreateOK = false; }

            if (ProductCreateOK) SendProductToDB();


        }

        private void TextBox_ProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ProductName.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TextBox_ProductCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ProductCategory.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TextBox_ProductDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ProductDescription.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_ProductCurrentStock_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBlock_ProductCurrentStock.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_ProductMinimun_Stock_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBlock_ProductMinimun_Stock.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_ProductSellPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBlock_ProductSellPrice.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_ProductCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBlock_ProductCost.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_ProductVat_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBlock_ProductVat.ClearValue(TextBox.BorderBrushProperty);
        }

        private void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            textBox_ProductName.ClearValue(TextBox.BorderBrushProperty);
            textBox_ProductCategory.ClearValue(TextBox.BorderBrushProperty);
            textBox_ProductDescription.ClearValue(TextBox.BorderBrushProperty);
            txtBlock_ProductCurrentStock.ClearValue(TextBox.BorderBrushProperty);
            txtBlock_ProductMinimun_Stock.ClearValue(TextBox.BorderBrushProperty);
            txtBlock_ProductSellPrice.ClearValue(TextBox.BorderBrushProperty);
            txtBlock_ProductCost.ClearValue(TextBox.BorderBrushProperty);
            txtBlock_ProductVat.ClearValue(TextBox.BorderBrushProperty);

            textBox_ProductName.Clear();
            textBox_ProductCategory.Clear();
            textBox_ProductDescription.Clear();
            txtBlock_ProductCurrentStock.Clear();
            txtBlock_ProductMinimun_Stock.Clear();
            txtBlock_ProductSellPrice.Clear();
            txtBlock_ProductCost.Clear();
            txtBlock_ProductVat.Clear();
        }
    }
}
