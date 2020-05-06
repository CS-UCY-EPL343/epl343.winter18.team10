using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InvoiceX.Pages.OrderPage
{
    /// <summary>
    /// Interaction logic for OrderCreate.xaml
    /// </summary>
    public partial class OrderCreate : Page
    {
        ProductViewModel productView;        
        CustomerViewModel customerView;
        OrderMain orderMain;

        bool Refresh_DB_data = true;

        public OrderCreate(OrderMain orderMain)
        {
            InitializeComponent();
            this.orderMain = orderMain;
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
        }

        public void load()
        {
            if (Refresh_DB_data)
            {                
                productView = new ProductViewModel();               
                customerView = new CustomerViewModel();               
                comboBox_customer.ItemsSource = customerView.customersList;
                comboBox_Product.ItemsSource = productView.productList;
                textBox_orderNumber.Text = (OrderViewModel.returnLatestOrderID()+1).ToString();
                OrderDate.SelectedDate = DateTime.Today;//set curent date 
                dueDate.SelectedDate = DateTime.Today.AddDays(60); ;//set curent date +60
            }
            Refresh_DB_data = false;
        }        


        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            if (comboBox_customer.SelectedIndex > -1)
            {
                textBox_Customer.Text = ((Customer)comboBox_customer.SelectedItem).CustomerName;
                textBox_Address.Text = ((Customer)comboBox_customer.SelectedItem).Address + ", " +
                 ((Customer)comboBox_customer.SelectedItem).City + ", " + ((Customer)comboBox_customer.SelectedItem).Country;
                textBox_Contact_Details.Text = ((Customer)comboBox_customer.SelectedItem).PhoneNumber.ToString();
                textBox_Email_Address.Text = ((Customer)comboBox_customer.SelectedItem).Email;
            }
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
                textBox_ProductPrice.Text = ((Product)comboBox_Product.SelectedItem).SellPrice.ToString("n2");
                textBox_ProductVat.Text = (((Product)comboBox_Product.SelectedItem).Vat * 100).ToString();
            }
        }

        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out int quantity) && 
                float.TryParse(textBox_ProductPrice.Text.Replace('.',','), out float price) && (comboBox_Product.SelectedIndex > -1))
            {
                //textBox_ProductTotal.Text = (Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
            }
        }

        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out int quantity) &&
                float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out float price) && (comboBox_Product.SelectedIndex > -1))
            {
                //textBox_ProductTotal.Text = (Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
            }
        }
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
            else 
            {
                comboBox_Product_border.BorderThickness = new Thickness(0);
            }           
            if (!int.TryParse(textBox_ProductQuantity.Text, out n) || (n<0) )
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

        private void Btn_AddProduct(object sender, RoutedEventArgs e)
        {
            if (Check_AddProduct_CompletedValues())
            {
                ProductDataGrid.Items.Add(new Product
                {
                    idProduct = ((Product)comboBox_Product.SelectedItem).idProduct,
                    ProductName = textBox_Product.Text,
                    ProductDescription = textBox_ProductDescription.Text,
                    Stock = Convert.ToInt32(textBox_ProductStock.Text),
                    SellPrice = Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')),
                    Quantity = Convert.ToInt32(textBox_ProductQuantity.Text),
                    Total = Convert.ToDouble(textBox_ProductTotal.Text),
                    Vat = ((Product)comboBox_Product.SelectedItem).Vat
                });

                double NetTotal_TextBlock_var = 0;
                NetTotal_TextBlock_var = Double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
                NetTotal_TextBlock_var = NetTotal_TextBlock_var + Convert.ToDouble(textBox_ProductTotal.Text);
                NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString("C");
                double Vat_TextBlock_var = 0;
                Vat_TextBlock_var = Double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
                Vat_TextBlock_var = Vat_TextBlock_var + (Convert.ToDouble(textBox_ProductTotal.Text) * ((Product)comboBox_Product.SelectedItem).Vat);
                Vat_TextBlock.Text = (Vat_TextBlock_var).ToString("C");
                TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + Vat_TextBlock_var).ToString("C");
            }
        }

        private void Button_Click_CreateOrder_REMOVE(object sender, RoutedEventArgs e)
        {
            Product CurrentCell_Product = (Product)(ProductDataGrid.CurrentCell.Item);
            double netTotal = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
            netTotal = netTotal - Convert.ToDouble(CurrentCell_Product.Total);
            NetTotal_TextBlock.Text = netTotal.ToString("C");
            double vat = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
            vat = vat - (CurrentCell_Product.Total * CurrentCell_Product.Vat);
            Vat_TextBlock.Text = vat.ToString("C");
            TotalAmount_TextBlock.Text = (netTotal + vat).ToString("C");
            ProductDataGrid.Items.Remove(ProductDataGrid.CurrentCell.Item);
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
            textBox_ProductTotal.Text = "";
            comboBox_Product_border.BorderThickness = new Thickness(0);
            textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
            textBox_ProductQuantity.ClearValue(TextBox.BorderBrushProperty);
        }

        private bool Check_CustomerForm()
        {
            if (comboBox_customer.SelectedIndex <= -1)
            {
                comboBox_customer_border.BorderBrush = Brushes.Red;
                comboBox_customer_border.BorderThickness = new Thickness(1);
                return false;
            }
            return true;
        }

      

        private bool Check_DetailsForm()
        {
            bool all_ok = true;
            if (issuedBy.Text.Equals(""))
            {
                issuedBy.BorderBrush = Brushes.Red;
                all_ok = false;
            }
            if (OrderDate.SelectedDate.Value > dueDate.SelectedDate.Value)
            {
                dueDate.BorderBrush = Brushes.Red;
                OrderDate.BorderBrush = Brushes.Red;
                MessageBox.Show("Due date is earlier than created date");
                all_ok = false;
            }
            return all_ok;
        }

        private bool Has_Items_Selected()
        {
            if (ProductDataGrid.Items.Count == 0)//vale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxo
            {
                MessageBox.Show("You havent selectet any products");
                return false;
            }
            return true;
        }
        
        private Order createOrderObject()
        {
            Order my_order;
            my_order = new Order();
            my_order.customer = ((Customer)comboBox_customer.SelectedItem);           
            my_order.products = ProductDataGrid.Items.OfType<Product>().ToList();
            my_order.idOrder = Int32.Parse(textBox_orderNumber.Text);
            my_order.cost = Double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
            my_order.VAT = Double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
            my_order.totalCost = Double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
            my_order.createdDate = OrderDate.SelectedDate.Value.Date;
            my_order.shippingDate = OrderDate.SelectedDate.Value.Date;
            my_order.issuedBy = issuedBy.Text;
            Enum.TryParse("Pending", out OrderStatus stautsenum);
            my_order.status = stautsenum;
            return my_order;
        }             

        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            bool ALL_VALUES_OK = true;
            if (!Check_CustomerForm()) ALL_VALUES_OK = false;
            if (!Check_DetailsForm()) ALL_VALUES_OK = false;
            if (!Has_Items_Selected()) ALL_VALUES_OK = false;
            if (ALL_VALUES_OK) 
            {                 
                Order order = createOrderObject();
                OrderViewModel.insertOrder(order);
                MessageBox.Show("Order with ID " + order.idOrder + " was created.");
                orderMain.viewOrder(order.idOrder);
                Btn_clearAll_Click(null, null);
            }
        }

        private void Clear_Customer()
        {
            comboBox_customer.SelectedIndex = -1;
            comboBox_customer_border.BorderThickness = new Thickness(0);
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }
        

        private void Clear_Details()
        {
            issuedBy.Text = "";
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
            OrderDate.ClearValue(TextBox.BorderBrushProperty);
            dueDate.ClearValue(TextBox.BorderBrushProperty);
            OrderDate.SelectedDate = DateTime.Today;//set curent date 
            dueDate.SelectedDate = DateTime.Today.AddDays(60); ;//set curent date +60
        }

        private void Clear_ProductGrid()
        {
            ProductDataGrid.Items.Clear();
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
        }

        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            
            Btn_clearProduct_Click(new object(), new RoutedEventArgs());
            Clear_Customer();
            Clear_Details();
            Clear_ProductGrid();
            Refresh_DB_data = true;
            load();                       
        }

        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }


        private void orderDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            OrderDate.ClearValue(TextBox.BorderBrushProperty);
        }

        private void dueDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dueDate.ClearValue(TextBox.BorderBrushProperty);
        }

    }
}
