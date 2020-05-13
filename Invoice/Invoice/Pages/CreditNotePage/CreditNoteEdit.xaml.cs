/*****************************************************************************
 * MIT License
 *
 * Copyright (c) 2020 InvoiceX
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 *****************************************************************************/

using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace InvoiceX.Pages.CreditNotePage
{
    /// <summary>
    /// Interaction logic for CreditNoteEdit.xaml
    /// </summary>
    public partial class CreditNoteEdit : Page
    {
       
        bool creditNote_loaded = false;
        CreditNote oldCreditNote;
        CreditNoteMain creditNoteMain;

        public CreditNoteEdit(CreditNoteMain creditNoteMain)
        {
            InitializeComponent();
            this.creditNoteMain = creditNoteMain;
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");           
        }

        /// <summary>
        /// The method that handles the event Selection Changed on the combobox containing the Products.
        /// Loads the product's information in their appropriate text boxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                textBox_ProductPrice.Text = ((Product)comboBox_Product.SelectedItem).SellPrice.ToString("n2");
                textBox_ProductVat.Text = (((Product)comboBox_Product.SelectedItem).Vat * 100).ToString();                
            }
        }

        /// <summary>
        /// The method that handles the event Selection Changed on the combobox containing the Invoice IDs.
        /// Loads the invoice's products in the combobox_Product.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_invoiceID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_invoiceID.SelectedIndex > -1)
            {
                comboBox_incoiceID_border.BorderThickness = new Thickness(0);                
                comboBox_Product.ItemsSource = InvoiceViewModel.getInvoice(int.Parse(comboBox_invoiceID.SelectedItem.ToString())).products;
            }
        }

        /// <summary>
        /// The method that handles the event Text Changed on the textbox containing the Product quantity.
        /// If the quantity is valid the product Total textbox is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out int quantity) &&
                 float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out float price) && (comboBox_Product.SelectedIndex > -1))
            {
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
            }
        }

        /// <summary>
        /// The method that handles the event Text Changed on the textbox containing the Product price.
        /// If the price is valid the product Total textbox is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out int quantity) &&
                float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out float price) && (comboBox_Product.SelectedIndex > -1))
            {
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
            }
        }

        /// <summary>
        /// Checks if the product selected in the combobox already exists in the product grid.
        /// </summary>
        /// <returns></returns>
        bool product_already_selected()
        {
            List<Product> gridProducts = ProductDataGrid.Items.OfType<Product>().ToList();
            foreach (Product p in gridProducts)
            {
                if (p.idProduct == ((Product)comboBox_Product.SelectedItem).idProduct)
                {
                    MessageBox.Show("Product already selected");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the product details are all completed and valid before its added to the grid.
        /// </summary>
        /// <returns></returns>
        private bool Check_AddProduct_CompletedValues()
        {
            bool all_completed = true;
            int n;
            if ((comboBox_Product.SelectedIndex <= -1) || product_already_selected())
            {
                all_completed = false;
                comboBox_Product_border.BorderBrush = Brushes.Red;
                comboBox_Product_border.BorderThickness = new Thickness(1);
            }
            if ((comboBox_invoiceID.SelectedIndex <= -1) )
            {
                all_completed = false;
                comboBox_incoiceID_border.BorderBrush = Brushes.Red;
                comboBox_incoiceID_border.BorderThickness = new Thickness(1);
            }
            if (!int.TryParse(textBox_ProductQuantity.Text, out n) || (n < 0))
            {
                all_completed = false;
                textBox_ProductQuantity.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_ProductQuantity.ClearValue(TextBox.BorderBrushProperty);
            }
            if (!float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out float f) || (f < 0))
            {
                all_completed = false;
                textBox_ProductPrice.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
            }

            return all_completed;
        }

        /// <summary>
        /// Adds the product selected to the product grid and updates the total and Vat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddProduct(object sender, RoutedEventArgs e)
        {
            if (Check_AddProduct_CompletedValues() && creditNote_loaded)
            {
                ProductDataGrid.Items.Add(new Product
                {
                    idProduct = ((Product)comboBox_Product.SelectedItem).idProduct,
                    ProductName = textBox_Product.Text,
                    ProductDescription = textBox_ProductDescription.Text,
                    SellPrice = Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')),
                    Quantity = Convert.ToInt32(textBox_ProductQuantity.Text),
                    Total = Convert.ToDouble(textBox_ProductTotal.Text),
                    Vat = ((Product)comboBox_Product.SelectedItem).Vat,
                    productInvoiceID = Convert.ToInt32(comboBox_invoiceID.SelectedItem)
                });

                double NetTotal_TextBlock_var = Double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
                NetTotal_TextBlock_var += Convert.ToDouble(textBox_ProductTotal.Text);
                NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString("C");

                double Vat_TextBlock_var = Double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
                Vat_TextBlock_var += (Convert.ToDouble(textBox_ProductTotal.Text) * ((Product)comboBox_Product.SelectedItem).Vat);
                Vat_TextBlock.Text = (Vat_TextBlock_var).ToString("C");

                TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + Vat_TextBlock_var).ToString("C");
            }
        }

        /// <summary>
        /// Removes a product from the product grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_CreateInvoice_REMOVE(object sender, RoutedEventArgs e)
        {
            Product CurrentCell_Product = (Product)(ProductDataGrid.CurrentCell.Item);
            double netTotal = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
            netTotal -= Convert.ToDouble(CurrentCell_Product.Total);
            NetTotal_TextBlock.Text = netTotal.ToString("C");

            double vat = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
            vat -= (CurrentCell_Product.Total * CurrentCell_Product.Vat);
            Vat_TextBlock.Text = vat.ToString("C");

            TotalAmount_TextBlock.Text = (netTotal + vat).ToString("C");
            ProductDataGrid.Items.Remove(ProductDataGrid.CurrentCell.Item);
        }

        /// <summary>
        /// Clears text when textbox gets focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        /// <summary>
        /// Clears the product's information from all the textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            comboBox_Product.SelectedIndex = -1;
            textBox_Product.Text = "";
            textBox_ProductDescription.Text = "";
            textBox_ProductQuantity.Text = "";
            textBox_ProductPrice.Text = "";
            textBox_ProductVat.Text = "";
            textBox_ProductTotal.Text = "";
            comboBox_Product_border.BorderThickness = new Thickness(0);
            textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
            textBox_ProductQuantity.ClearValue(TextBox.BorderBrushProperty);
        }

        /// <summary>
        /// Checks if the grid has products in it
        /// </summary>
        /// <returns></returns>
        private bool Has_Items_Selected()
        {
            if (ProductDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selectet any products");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates and returns the Credit Note based on the information on the page
        /// </summary>
        /// <returns></returns>
        private CreditNote createCreditNoteObject()
        {
            CreditNote creditNote = new CreditNote();          
            creditNote.customer = oldCreditNote.customer;
            creditNote.products = ProductDataGrid.Items.OfType<Product>().ToList();
            creditNote.idCreditNote = Int32.Parse(textBox_invoiceNumber.Text);
            creditNote.cost = Double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
            creditNote.VAT = Double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
            creditNote.totalCost = Double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
            creditNote.createdDate = oldCreditNote.createdDate;
            creditNote.issuedBy = issuedBy.Text;
            return creditNote;           
        }

        /// <summary>
        /// After validating updates the credit note and switches to viewing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            if (Has_Items_Selected())
            {
                if (int.TryParse(textBox_invoiceNumber.Text, out int creditNoteID))
                {
                    CreditNoteViewModel.updateCreditNote(createCreditNoteObject(), oldCreditNote);
                    creditNoteMain.viewCreditNote(creditNoteID);
                    Btn_clearAll_Click(null, null);
                }
            }
        }

        /// <summary>
        /// Clears all the customer's information
        /// </summary>
        private void Clear_Customer()
        {
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        /// <summary>
        /// Clears the issuedBy textbox
        /// </summary>
        private void Clear_Details()
        {
            issuedBy.Text = "";
            textBox_invoiceNumber.Clear();
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
            txtbox_invoiceDate.Clear();
        }

        /// <summary>
        /// Clears all products from the product grid
        /// </summary>
        private void Clear_ProductGrid()
        {
            ProductDataGrid.Items.Clear();
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
        }

        /// <summary>
        /// Clear all inputs from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            creditNote_loaded = false;
            comboBox_invoiceID.ItemsSource = null;
            comboBox_Product.ItemsSource = null;
            Btn_clearProduct_Click(null, null);
            Clear_Customer();
            Clear_Details();
            Clear_ProductGrid();            
        }

        /// <summary>
        /// The method that handles the event Text Changed on the textbox IssuedBy.
        /// Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)//mono meta to refresh whritable
        {
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }

        /// <summary>
        /// After validating that the given credit note ID exists loads the credit note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Load_CreditNote(object sender, RoutedEventArgs e)
        {
            int.TryParse(textBox_invoiceNumber.Text, out int creditNoteid);
            if ((CreditNoteViewModel.CreditNoteExists(creditNoteid)))
            {
                Btn_clearAll_Click(null, null);
                loadCreditNote(creditNoteid);
                creditNote_loaded = true;
            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for credit note ID.");
            }
        }

        /// <summary>
        /// Loads the credit note information on the page
        /// </summary>
        /// <param name="creditNoteId"></param>
        public void loadCreditNote(int creditNoteId)
        {
            oldCreditNote = CreditNoteViewModel.getCreditNote(creditNoteId);
            if (oldCreditNote != null)
            {
                // Customer details
                textBox_Customer.Text = oldCreditNote.customer.CustomerName;
                textBox_Contact_Details.Text = oldCreditNote.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = oldCreditNote.customer.Email;
                textBox_Address.Text = oldCreditNote.customer.Address + ", " + oldCreditNote.customer.City + ", " + oldCreditNote.customer.Country;

                // Invoice details
                textBox_invoiceNumber.Text = oldCreditNote.idCreditNote.ToString();
                txtbox_invoiceDate.Text = oldCreditNote.createdDate.ToString("d"); ;
                issuedBy.Text = oldCreditNote.issuedBy;
                NetTotal_TextBlock.Text = oldCreditNote.cost.ToString("C");
                Vat_TextBlock.Text = oldCreditNote.VAT.ToString("C");
                TotalAmount_TextBlock.Text = oldCreditNote.totalCost.ToString("C");
                              
                foreach (Product p in oldCreditNote.products)
                {

                    ProductDataGrid.Items.Add(p);
                }
                
                comboBox_invoiceID.ItemsSource = InvoiceViewModel.getCustomerInvoices(oldCreditNote.customer.idCustomer);
            }
            else
            {
                MessageBox.Show("Credit Note id doesnt't exist");
            }
        }        
    }
}
