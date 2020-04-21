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

namespace InvoiceX.Pages.SettingsPage
{
    /// <summary>
    /// Interaction logic for SettingsDatabase.xaml
    /// </summary>
    public partial class SettingsDatabase : Page
    {
        public SettingsDatabase()
        {
            InitializeComponent();
        }

        private void Btn_exportPath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "database", // Default file name
                DefaultExt = ".sql", // Default file extension
                Filter = "All Files (*.*)|*.*" // Filter files by extension
            };

            Nullable<bool> result = dlg.ShowDialog();
            string filename = "";

            if (result == true)
            {
                filename = dlg.FileName;
            }

            txtBlock_exportPath.Text = filename;
        }

        private void Btn_export_Click(object sender, RoutedEventArgs e)
        {
            string filename = txtBlock_exportPath.Text;
            if (DatabaseViewModel.exportDatabase(filename))
            {
                MessageBox.Show("Database successfully exported to : " + filename);
            }
            else
            {
                MessageBox.Show("Database failed to export.");
            }
        }

        private void Btn_importPath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "database", // Default file name
                DefaultExt = ".sql", // Default file extension
                Filter = "All Files (*.*)|*.*" // Filter files by extension
            };

            Nullable<bool> result = dlg.ShowDialog();
            string filename = "";
            if (result == true)
            {                
                 filename = dlg.FileName;
            }

            txtBlock_importPath.Text = filename;
        }

        private void Btn_import_Click(object sender, RoutedEventArgs e)
        {
            string msgtext = "You are about to import into to the database. Are you sure?";
            string txt = "Import Database";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);
            string filename = txtBlock_importPath.Text;

            switch (result)
            {
                case MessageBoxResult.Yes:
                    if (DatabaseViewModel.importDatabase(filename))
                    {
                        MessageBox.Show("Database successfully imported");
                    }
                    else
                    {
                        MessageBox.Show("Database failed to import file : " + filename);
                    }
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void Btn_exportPathCSV_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "databaseCSV", // Default file name
                DefaultExt = ".txt", // Default file extension
                Filter = "All Files (*.*)|*.*" // Filter files by extension
            };

            Nullable<bool> result = dlg.ShowDialog();
            string filename = "";

            if (result == true)
            {
                filename = dlg.FileName;
            }

            txtBlock_exportPathCSV.Text = filename;
        }

        private void Btn_exportCSV_Click(object sender, RoutedEventArgs e)
        {
            string filename = txtBlock_exportPathCSV.Text;
            if (DatabaseViewModel.exportDatabaseAsCSV(filename))
            {
                MessageBox.Show("Database successfully exported as CSV to : " + filename);
            }
            else
            {
                MessageBox.Show("Database failed to export.");
            }
        }
    }
}
