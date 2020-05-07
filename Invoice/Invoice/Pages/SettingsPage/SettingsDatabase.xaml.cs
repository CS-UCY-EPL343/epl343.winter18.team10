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

        private void Btn_export_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "database", // Default file name
                DefaultExt = ".sql", // Default file extension
                Filter = "All Files (*.*)|*.*" // Filter files by extension
            };

            Nullable<bool> result = dlg.ShowDialog();            

            if (result == true)
            {
                if (DatabaseViewModel.exportDatabase(dlg.FileName))
                {
                    txtBlock_exportPath.Text = dlg.FileName;
                    MessageBox.Show("Database successfully exported to : " + dlg.FileName);
                }
                else
                {
                    MessageBox.Show("Database failed to export.");
                }
            }

        }

        private void Btn_import_Click(object sender, RoutedEventArgs e)
        {
            string msgtext = "You are about to import into to the database. Are you sure?";
            string txt = "Import Database";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult msgresult = MessageBox.Show(msgtext, txt, button);

            switch (msgresult)
            {
                case MessageBoxResult.Yes:
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
                    {
                        FileName = "database", // Default file name
                        DefaultExt = ".sql", // Default file extension
                        Filter = "All Files (*.*)|*.*" // Filter files by extension
                    };

                    Nullable<bool> result = dlg.ShowDialog();
                    
                    if (result == true)
                    {
                        txtBlock_importPath.Text = dlg.FileName;

                        if (DatabaseViewModel.importDatabase(dlg.FileName))
                        {
                            MessageBox.Show("Database successfully imported");
                        }
                        else
                        {
                            MessageBox.Show("Database failed to import file : " + dlg.FileName);
                        }
                    }                    
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }               

        private void Btn_exportCSV_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "databaseCSV", // Default file name
                DefaultExt = ".txt", // Default file extension
                Filter = "All Files (*.*)|*.*" // Filter files by extension
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                txtBlock_exportPathCSV.Text = dlg.FileName;

                if (DatabaseViewModel.exportDatabaseAsCSV(dlg.FileName))
                {
                    MessageBox.Show("Database successfully exported as CSV to : " + dlg.FileName);
                }
                else
                {
                    MessageBox.Show("Database failed to export.");
                }
            }            
        }

    }
}
