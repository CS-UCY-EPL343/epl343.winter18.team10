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

        private void Btn_createProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_ProductName.Text)) textBox_ProductName.BorderBrush = Brushes.Red;
            if (string.IsNullOrWhiteSpace(textBox_ProductCategory.Text)) textBox_ProductCategory.BorderBrush = Brushes.Red;
            if (string.IsNullOrWhiteSpace(textBox_ProductDescription.Text)) textBox_ProductDescription.BorderBrush = Brushes.Red;
            if (!int.TryParse(txtBlock_ProductCurrentStock.Text, out int a)) txtBlock_ProductCurrentStock.BorderBrush = Brushes.Red;

            
        }
    }
}
