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

namespace Invoice.Pages
{
    /// <summary>
    /// Interaction logic for InvoiceMain.xaml
    /// </summary>
    public partial class InvoiceMain : Page
    {
        public InvoiceMain()
        {
            InitializeComponent();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            viewTab.Visibility = Visibility.Visible;
            createTab.Visibility = Visibility.Hidden;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            viewTab.Visibility = Visibility.Hidden;
            createTab.Visibility = Visibility.Visible;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
