// /*****************************************************************************
//  * MIT License
//  *
//  * Copyright (c) 2020 InvoiceX
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.
//  *
//  *****************************************************************************/

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InvoiceX.Models;
using InvoiceX.ViewModels;

namespace InvoiceX.Pages.CreditNotePage
{
    /// <summary>
    ///     Interaction logic for InvoiceCreate.xaml
    /// </summary>
    public partial class CreditNoteCreate : Page
    {
        private readonly CreditNoteMain creditNoteMain;
        private CustomerViewModel customerView;
        private bool refreshDataDB = true;

        public CreditNoteCreate(CreditNoteMain creditNoteMain)
        {
            InitializeComponent();
            this.creditNoteMain = creditNoteMain;
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
        }

        /// <summary>
        ///     Loads the customers in the combobox as well as the new credit note ID
        /// </summary>
        public void load()
        {
            if (refreshDataDB)
            {
                customerView = new CustomerViewModel();
                comboBox_customer.ItemsSource = customerView.customersList;
                textBox_invoiceNumber.Text = (CreditNoteViewModel.returnLatestCreditNoteID() + 1).ToString();
                invoiceDate.SelectedDate = DateTime.Today; //set curent date 
            }

            refreshDataDB = false;
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the customers.
        ///     Loads the customer's information in the appropriate text boxes, as well as their invoice IDs
        ///     in the combobox_invoiceID.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            if (comboBox_customer.SelectedIndex > -1)
            {
                var customer = (Customer) comboBox_customer.SelectedItem;
                textBox_Customer.Text = customer.CustomerName;
                textBox_Address.Text = customer.Address + ", " + customer.City + ", " + customer.Country;
                textBox_Contact_Details.Text = customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = customer.Email;

                comboBox_invoiceID.ItemsSource = InvoiceViewModel.getCustomerInvoices(customer.idCustomer);
            }
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the Invoice IDs.
        ///     Loads the invoice's products in the combobox_Product.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_invoiceID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_invoiceID.SelectedIndex > -1)
            {
                comboBox_incoiceID_border.BorderThickness = new Thickness(0);
                comboBox_Product.ItemsSource = InvoiceViewModel
                    .getInvoice(int.Parse(comboBox_invoiceID.SelectedItem.ToString())).products;
            }
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the Products.
        ///     Loads the product's information in their appropriate text boxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_Product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_Product.SelectedIndex > -1)
            {
                var product = (Product) comboBox_Product.SelectedItem;
                comboBox_Product_border.BorderThickness = new Thickness(0);
                textBox_ProductPrice.ClearValue(Control.BorderBrushProperty);
                textBox_ProductQuantity.ClearValue(Control.BorderBrushProperty);
                textBox_Product.Text = product.ProductName;
                textBox_ProductQuantity.Text = product.Quantity.ToString();
                textBox_ProductDescription.Text = product.ProductDescription;
                textBox_ProductPrice.Text = product.SellPrice.ToString("n2");
                textBox_ProductVat.Text = (product.Vat * 100).ToString();
            }
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product quantity.
        ///     If the quantity is valid the product Total textbox is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out var quantity) &&
                float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out var price) &&
                comboBox_Product.SelectedIndex > -1)
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product price.
        ///     If the price is valid the product Total textbox is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out var quantity) &&
                float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out var price) &&
                comboBox_Product.SelectedIndex > -1)
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
        }

        /// <summary>
        ///     Checks if the product selected in the combobox already exists in the product grid.
        /// </summary>
        /// <returns></returns>
        private bool productAlreadySelected()
        {
            var gridProducts = ProductDataGrid.Items.OfType<Product>().ToList();
            foreach (var p in gridProducts)
                if (p.idProduct == ((Product) comboBox_Product.SelectedItem).idProduct)
                {
                    MessageBox.Show("Product '" + p.ProductName + "' already selected");
                    return true;
                }

            return false;
        }

        /// <summary>
        ///     Checks if the product details are all completed and valid before its added to the grid.
        /// </summary>
        /// <returns></returns>
        private bool checkAddProductCompletedValues()
        {
            var all_completed = true;
            int n;
            if (comboBox_Product.SelectedIndex <= -1 || productAlreadySelected())
            {
                all_completed = false;
                comboBox_Product_border.BorderBrush = Brushes.Red;
                comboBox_Product_border.BorderThickness = new Thickness(1);
            }

            if (comboBox_invoiceID.SelectedIndex <= -1)
            {
                all_completed = false;
                comboBox_incoiceID_border.BorderBrush = Brushes.Red;
                comboBox_incoiceID_border.BorderThickness = new Thickness(1);
            }

            if (!int.TryParse(textBox_ProductQuantity.Text, out n) || n < 0)
            {
                all_completed = false;
                textBox_ProductQuantity.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_ProductQuantity.ClearValue(Control.BorderBrushProperty);
            }

            if (!float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out var f) || f < 0)
            {
                all_completed = false;
                textBox_ProductPrice.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_ProductPrice.ClearValue(Control.BorderBrushProperty);
            }

            return all_completed;
        }

        /// <summary>
        ///     Adds the product selected to the product grid and updates the total and Vat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddProduct(object sender, RoutedEventArgs e)
        {
            if (checkAddProductCompletedValues())
            {
                ProductDataGrid.Items.Add(new Product
                {
                    idProduct = ((Product) comboBox_Product.SelectedItem).idProduct,
                    ProductName = textBox_Product.Text,
                    ProductDescription = textBox_ProductDescription.Text,
                    SellPrice = Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')),
                    Quantity = Convert.ToInt32(textBox_ProductQuantity.Text),
                    Total = Convert.ToDouble(textBox_ProductTotal.Text),
                    Vat = ((Product) comboBox_Product.SelectedItem).Vat,
                    productInvoiceID = Convert.ToInt32(comboBox_invoiceID.SelectedItem)
                });
                var netTotal = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
                netTotal += Convert.ToDouble(textBox_ProductTotal.Text);
                NetTotal_TextBlock.Text = netTotal.ToString("C");

                var VAT = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
                VAT += Convert.ToDouble(textBox_ProductTotal.Text) * ((Product) comboBox_Product.SelectedItem).Vat;
                Vat_TextBlock.Text = VAT.ToString("C");

                TotalAmount_TextBlock.Text = (netTotal + VAT).ToString("C");
            }
        }

        /// <summary>
        ///     Removes a product from the product grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_CreateInvoice_REMOVE(object sender, RoutedEventArgs e)
        {
            var CurrentCell_Product = (Product) ProductDataGrid.CurrentCell.Item;

            var netTotal = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
            netTotal -= Convert.ToDouble(CurrentCell_Product.Total);
            NetTotal_TextBlock.Text = netTotal.ToString("C");

            var VAT = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
            VAT -= CurrentCell_Product.Total * CurrentCell_Product.Vat;
            Vat_TextBlock.Text = VAT.ToString("C");

            TotalAmount_TextBlock.Text = (netTotal + VAT).ToString("C");
            ProductDataGrid.Items.Remove(ProductDataGrid.CurrentCell.Item);
        }

        /// <summary>
        ///     Clears text when textbox gets focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox) sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        /// <summary>
        ///     Clears the product's information from all the textboxes
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
            textBox_ProductPrice.ClearValue(Control.BorderBrushProperty);
            textBox_ProductQuantity.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Checks if a customer is selected
        /// </summary>
        /// <returns>True if a customer is selected, False otherwise</returns>
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

        /// <summary>
        ///     Checks if the textbox IssuedBy is completed
        /// </summary>
        /// <returns>True if it is completed, False otherwise</returns>
        private bool checkDetailsForm()
        {
            if (issuedBy.Text.Equals(""))
            {
                issuedBy.BorderBrush = Brushes.Red;
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Checks if the grid has products in it
        /// </summary>
        /// <returns>True if it is not empty, False otherwise</returns>
        private bool hasItemsSelected()
        {
            if (ProductDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selected any products");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Creates and returns the Credit Note based on the information on the page
        /// </summary>
        /// <returns>The Credit Note created</returns>
        private CreditNote createCreditNoteObject()
        {
            return new CreditNote
            {
                customer = (Customer) comboBox_customer.SelectedItem,
                products = ProductDataGrid.Items.OfType<Product>().ToList(),
                idCreditNote = int.Parse(textBox_invoiceNumber.Text),
                cost = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency),
                VAT = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency),
                totalCost = double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency),
                createdDate = invoiceDate.SelectedDate.Value.Date,
                issuedBy = issuedBy.Text
            };
        }

        /// <summary>
        ///     After validating creates the credit note and switches to viewing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            if (checkCustomerForm() && checkDetailsForm() && hasItemsSelected())
            {
                var creditNote = createCreditNoteObject();
                creditNote.createdDate += DateTime.Now.TimeOfDay;
                CreditNoteViewModel.insertCreditNote(creditNote);
                MessageBox.Show("Credit Note with ID " + creditNote.idCreditNote + " was created.");
                creditNoteMain.viewCreditNote(creditNote.idCreditNote);
                Btn_clearAll_Click(null, null);
            }
        }

        /// <summary>
        ///     Clears all the customer's information
        /// </summary>
        private void clearCustomer()
        {
            comboBox_customer.SelectedIndex = -1;
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        /// <summary>
        ///     Clears the issuedBy textbox
        /// </summary>
        private void clearDetails()
        {
            issuedBy.Text = "";
            issuedBy.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Clears all products from the product grid
        /// </summary>
        private void clearProductGrid()
        {
            ProductDataGrid.Items.Clear();
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
        }

        /// <summary>
        ///     Clears all inputs from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            comboBox_invoiceID.ItemsSource = null;
            comboBox_Product.ItemsSource = null;
            Btn_clearProduct_Click(null, null);
            clearCustomer();
            clearDetails();
            clearProductGrid();
            refreshDataDB = true;
            load();
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox IssuedBy.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(Control.BorderBrushProperty);
        }
    }
}