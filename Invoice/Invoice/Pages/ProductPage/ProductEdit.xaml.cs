﻿using InvoiceX.Models;
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
    /// Interaction logic for ProductEdit.xaml
    /// </summary>
    public partial class ProductEdit : Page
    {

        public ProductEdit()
        {
            InitializeComponent();
        } 

        
        private Product createObjectProduct()
        {
            Product product = new Product();
            product.ProductName = textBox_ProductName.Text;
            product.Category = textBox_ProductCategory.Text;
            product.ProductDescription = textBox_ProductDescription.Text;
            product.Stock = Int32.Parse(txtBlock_ProductCurrentStock.Text);
            product.MinStock = Int32.Parse(txtBlock_ProductMinimun_Stock.Text);
            product.SellPrice = double.Parse(txtBlock_ProductSellPrice.Text);
            product.Cost = double.Parse(txtBlock_ProductCost.Text);
            product.SellPrice = double.Parse(txtBlock_ProductSellPrice.Text);
            product.Vat = float.Parse(txtBlock_ProductVat.Text)/100;
            product.idProduct= Int32.Parse(txtbox_productId.Text);
            return product;
        }

        private bool validate_product()
        {
            bool ProductCreateOK = true;
            if (string.IsNullOrWhiteSpace(textBox_ProductName.Text)) { textBox_ProductName.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_ProductCategory.Text)) { textBox_ProductCategory.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (string.IsNullOrWhiteSpace(textBox_ProductDescription.Text)) { textBox_ProductDescription.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!int.TryParse(txtBlock_ProductCurrentStock.Text, out int a)) { txtBlock_ProductCurrentStock.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!int.TryParse(txtBlock_ProductMinimun_Stock.Text, out a)) { txtBlock_ProductMinimun_Stock.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!double.TryParse(txtBlock_ProductSellPrice.Text, out double b)) { txtBlock_ProductSellPrice.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!double.TryParse(txtBlock_ProductCost.Text, out b)) { txtBlock_ProductCost.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!float.TryParse(txtBlock_ProductVat.Text, out float g)) { txtBlock_ProductVat.BorderBrush = Brushes.Red; ProductCreateOK = false; }
            if (!int.TryParse(txtbox_productId.Text, out int gg)) { txtbox_productId.BorderBrush = Brushes.Red; ProductCreateOK = false; }

            if (int.TryParse(txtbox_productId.Text, out int ggg))
            {
                int productid = Int32.Parse(txtbox_productId.Text);
                if (productid < 0 || productid > ProductViewModel.ReturnLatestProductID())
                {
                    MessageBox.Show("Product ID doesn't exist");
                    txtbox_productId.BorderBrush = Brushes.Red;
                    ProductCreateOK = false;
                }
            }

            if (float.TryParse(txtBlock_ProductVat.Text, out float k))
            {
                float vat = float.Parse(txtBlock_ProductVat.Text);
                if (vat < 0 || vat > 100)
                {

                    MessageBox.Show("VAT is not in range 0-100");
                    txtBlock_ProductVat.BorderBrush = Brushes.Red; ProductCreateOK = false;
                    ProductCreateOK = false;
                }
            }
            return ProductCreateOK;
        }
        
        private void Btn_UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            int productid = -1;
            if (int.TryParse(txtbox_productId.Text, out int n))
            {
                productid = int.Parse(txtbox_productId.Text);
                int latestproductid = ProductViewModel.ReturnLatestProductID();
                if ((productid <= latestproductid) && (productid > -1))
                {
                    if (validate_product())
                    {                        
                        ProductViewModel.UpdateProductToDB(createObjectProduct());
                    }                 

                }
                else
                {
                    MessageBox.Show("Product ID doesn't exist");
                    txtbox_productId.BorderBrush = Brushes.Red;
                }
            }
            else
            {
                MessageBox.Show("Product ID doesn't exist");
                txtbox_productId.BorderBrush = Brushes.Red;
            }
           

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

        private void Txtbox_productId_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtbox_productId.ClearValue(TextBox.BorderBrushProperty);
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
            txtbox_productId.ClearValue(TextBox.BorderBrushProperty);

            textBox_ProductName.Clear();
            textBox_ProductCategory.Clear();
            textBox_ProductDescription.Clear();
            txtBlock_ProductCurrentStock.Clear();
            txtBlock_ProductMinimun_Stock.Clear();
            txtBlock_ProductSellPrice.Clear();
            txtBlock_ProductCost.Clear();
            txtBlock_ProductVat.Clear();
            txtbox_productId.Clear();
        }

        private void Btn_loadProduct_Click(object sender, RoutedEventArgs e)
        {
            int productid=-1;
            if (int.TryParse(txtbox_productId.Text, out int n))
            {
                productid = int.Parse(txtbox_productId.Text);
                int latestproductid = ProductViewModel.ReturnLatestProductID();
                if ((productid <= latestproductid) && (productid > -1))
                {
                    Product product=ProductViewModel.ReturnProductByid( productid);
                     textBox_ProductName.Text = product.ProductName;
                     textBox_ProductCategory.Text = product.Category;
                     textBox_ProductDescription.Text = product.ProductDescription;
                     txtBlock_ProductCurrentStock.Text =product.Stock.ToString();
                     txtBlock_ProductMinimun_Stock.Text = product.MinStock.ToString();
                     txtBlock_ProductSellPrice.Text = product.SellPrice.ToString("n2");
                     txtBlock_ProductCost.Text = product.Cost.ToString("n2");
                     txtBlock_ProductVat.Text = product.Vat.ToString();
                }
                 else
                {
                     MessageBox.Show("Product ID doesn't exist");
                     txtbox_productId.BorderBrush = Brushes.Red;
                 }   
            }
            else
            {
                MessageBox.Show("Product ID doesn't exist");
                txtbox_productId.BorderBrush = Brushes.Red;
            }
            
        }

       
    }
}