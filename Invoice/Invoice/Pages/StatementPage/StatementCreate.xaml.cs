﻿using InvoiceX.Models;
using InvoiceX.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public StatementCreate()
        {
            InitializeComponent();
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
            DateTime from = (DateTime)fromDate.SelectedDate;
            DateTime to = (DateTime)toDate.SelectedDate;

            List<StatementItem> statement = new List<StatementItem>();
            statement.AddRange(InvoiceViewModel.getInvoicesForStatement(customerID, from, to));
            statement.AddRange(CreditNoteViewModel.getCreditNotesForStatement(customerID, from, to));
            statement.AddRange(ReceiptViewModel.getReceiptsForStatement(customerID, from, to));

            statementDataGrid.ItemsSource = statement;
            var firstCol = statementDataGrid.Columns.First();
            firstCol.SortDirection = ListSortDirection.Ascending;
            statementDataGrid.Items.SortDescriptions.Add(new SortDescription(firstCol.SortMemberPath, ListSortDirection.Ascending));
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

        private void previewPdf_click(object sender, RoutedEventArgs e)
        {

        }

        private void printPdf_click(object sender, RoutedEventArgs e)
        {

        }

        private void savePdf_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}