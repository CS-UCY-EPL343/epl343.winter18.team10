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

using System.Windows;
using System.Windows.Controls;
using InvoiceX.ViewModels;
using Microsoft.Win32;

namespace InvoiceX.Pages.SettingsPage
{
    /// <summary>
    ///     Interaction logic for SettingsDatabase.xaml
    /// </summary>
    public partial class SettingsDatabase : Page
    {
        public SettingsDatabase()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Exports the database as SQL script
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_export_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "database", // Default file name
                DefaultExt = ".sql", // Default file extension
                Filter = "All Files (*.*)|*.*" // Filter files by extension
            };

            var result = dlg.ShowDialog();

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

        /// <summary>
        ///     Imports the database from SQL script
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_import_Click(object sender, RoutedEventArgs e)
        {
            var msgtext = "You are about to import into to the database. Are you sure?";
            var txt = "Import Database";
            var button = MessageBoxButton.YesNo;
            var msgresult = MessageBox.Show(msgtext, txt, button);

            switch (msgresult)
            {
                case MessageBoxResult.Yes:
                    var dlg = new OpenFileDialog
                    {
                        FileName = "database", // Default file name
                        DefaultExt = ".sql", // Default file extension
                        Filter = "All Files (*.*)|*.*" // Filter files by extension
                    };

                    var result = dlg.ShowDialog();

                    if (result == true)
                    {
                        txtBlock_importPath.Text = dlg.FileName;

                        if (DatabaseViewModel.importDatabase(dlg.FileName))
                            MessageBox.Show("Database successfully imported");
                        else
                            MessageBox.Show("Database failed to import file : " + dlg.FileName);
                    }

                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        ///     Export the database as CSV file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_exportCSV_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                FileName = "databaseCSV", // Default file name
                DefaultExt = ".txt", // Default file extension
                Filter = "All Files (*.*)|*.*" // Filter files by extension
            };

            var result = dlg.ShowDialog();

            if (result == true)
            {
                txtBlock_exportPathCSV.Text = dlg.FileName;

                if (DatabaseViewModel.exportDatabaseAsCSV(dlg.FileName))
                    MessageBox.Show("Database successfully exported as CSV to : " + dlg.FileName);
                else
                    MessageBox.Show("Database failed to export.");
            }
        }
    }
}