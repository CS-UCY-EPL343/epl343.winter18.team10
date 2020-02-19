using MySql.Data.MySqlClient;
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

namespace InvoiceX.Pages.CustomerPage
{
    /// <summary>
    /// Interaction logic for CustomerCreate.xaml
    /// </summary>
    public partial class CustomerCreate : Page
    {
        public CustomerCreate()
        {
            InitializeComponent();
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
                string query = "INSERT INTO Customer (CustomerName,PhoneNumber,Email,Country,City,Address,Balance) Values (@CustomerName,@PhoneNumber,@Email,@Country,@City,@Address,@Balance)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@CustomerName", textBox_CustomerName.Text);
                    cmd.Parameters.AddWithValue("@PhoneNumber", textBox_PhoneNumber.Text);
                    cmd.Parameters.AddWithValue("@Email", textBox_CustomerEmail.Text);
                    cmd.Parameters.AddWithValue("@Country", textBox_CustomerCountry.Text);
                    cmd.Parameters.AddWithValue("@City", textBox_CustomerCity.Text);
                    cmd.Parameters.AddWithValue("@Address", textBox_CustomerAddress.Text);
                    cmd.Parameters.AddWithValue("@Balance", double.Parse( textBox_CustomerBalance.Text));                   
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
                MessageBox.Show("Customer addet to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        private void Btn_createCustomer_Click(object sender, RoutedEventArgs e)
        {
            bool ProductCreateOK = true;
            if (string.IsNullOrWhiteSpace(textBox_CustomerName.Text)) { textBox_CustomerName.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!int.TryParse(textBox_PhoneNumber.Text, out int a)) { textBox_PhoneNumber.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_CustomerEmail.Text)) { textBox_CustomerEmail.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_CustomerCountry.Text)) { textBox_CustomerCountry.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_CustomerCity.Text)) { textBox_CustomerCity.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_CustomerAddress.Text)) { textBox_CustomerAddress.BorderBrush = Brushes.Red; ProductCreateOK = false; }     
            if (!double.TryParse(textBox_CustomerBalance.Text, out double b)) { textBox_CustomerBalance.BorderBrush = Brushes.Red; ProductCreateOK = false; }

            if (ProductCreateOK) SendProductToDB();


        }

        private void TextBox_CustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerName.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TextBox_CustomePhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_PhoneNumber.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TextBox_CustomerEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerEmail.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_CustomerCountry_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerCountry.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_CustomerCity_Stock_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerCity.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_CustomerAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerAddress.ClearValue(TextBox.BorderBrushProperty);
        }

        private void TxtBlock_CustomerBalance_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_CustomerBalance.ClearValue(TextBox.BorderBrushProperty);
        }

      

        private void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            textBox_CustomerName.ClearValue(TextBox.BorderBrushProperty);
            textBox_PhoneNumber.ClearValue(TextBox.BorderBrushProperty);
            textBox_CustomerEmail.ClearValue(TextBox.BorderBrushProperty);
            textBox_CustomerCountry.ClearValue(TextBox.BorderBrushProperty);
            textBox_CustomerCity.ClearValue(TextBox.BorderBrushProperty);
            textBox_CustomerAddress.ClearValue(TextBox.BorderBrushProperty);
            textBox_CustomerBalance.ClearValue(TextBox.BorderBrushProperty);


            textBox_CustomerName.Clear();
            textBox_PhoneNumber.Clear();
            textBox_CustomerEmail.Clear();
            textBox_CustomerCountry.Clear();
            textBox_CustomerCity.Clear();
            textBox_CustomerAddress.Clear();
            textBox_CustomerBalance.Clear();
            
        }
    }
}
