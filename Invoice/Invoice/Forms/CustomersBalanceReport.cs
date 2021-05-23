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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using InvoiceX.Models;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using InvoiceX.ViewModels;
namespace InvoiceX.Forms
{
    /// <summary>
    ///     Creates the invoice form.
    /// </summary>
    public class CustomersBalanceReport
    {
        /// <summary>
        ///     The MigraDoc document that represents the invoice.
        /// </summary>
        private Document document;

        /// <summary>
        ///     An XML invoice based on a sample created with Microsoft InfoPath.
        /// </summary>
        private readonly XmlDocument invoice;

        /// <summary>
        ///     The root navigator for the XML document.
        /// </summary>
        private readonly XPathNavigator navigator;

        /// <summary>
        ///     The text frame of the MigraDoc document that contains the address.
        /// </summary>
        private TextFrame addressFrame;

        private TextFrame invoiceDetailsFrame;
        private TextFrame customerDetails;
        private TextFrame footerFrameLeft;
        private TextFrame footerFrameRight;

        /// <summary>
        ///     The table of the MigraDoc document that contains the invoice items.
        /// </summary>
        private Table table;

        /// <summary>
        ///     Initializes a new instance of the class BillFrom and opens the specified XML document.
        /// </summary>
        private readonly List<Customer> customers = new List<Customer>();
        private readonly float totalBalanceDue;
        public CustomersBalanceReport(string filename, List<Customer> cd,float total)
        {
            invoice = new XmlDocument();
            invoice.Load(filename);
            navigator = invoice.CreateNavigator();
            customers = cd;
            totalBalanceDue = total;
        }

        /// <summary>
        ///     Creates the invoice document.
        /// </summary>
        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            document = new Document();
            document.Info.Title = "Customers Balance Report";
            document.Info.Author = "Eco-Bright";

            DefineStyles();

            CreatePage();

            FillContent();

            return document;
        }

        /// <summary>
        ///     Defines the styles used to format the MigraDoc document.
        /// </summary>
        private void DefineStyles()
        {
            // Get the predefined style Normal.
            var style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("6cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Arial";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        /// <summary>
        ///     Creates the static parts of the invoice.
        /// </summary>
        private void CreatePage()
        {
            // Each MigraDoc document needs at least one section.
            var section = document.AddSection();
            // Put a logo in the header
            var image = section.Headers.Primary.AddImage("../../Images/invoiceDesign.png");
            image.Width = "16cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;


            var par = section.Headers.Primary.AddParagraph();
            par.Format.Alignment = ParagraphAlignment.Right;
            par.Format.Font.Size = 24;
            par.Format.Font.Color = LogoBlue;
            par.Format.SpaceBefore = 5;
            section.PageSetup.TopMargin = 230;
            section.PageSetup.BottomMargin = 140;

            // Create the left footer frame
            footerFrameLeft = section.Footers.Primary.AddTextFrame();
            footerFrameLeft.Height = "3.0cm";
            footerFrameLeft.Width = "7.0cm";
            footerFrameLeft.Left = ShapePosition.Left;
            footerFrameLeft.RelativeHorizontal = RelativeHorizontal.Margin;
            footerFrameLeft.Top = "26cm";
            footerFrameLeft.RelativeVertical = RelativeVertical.Page;
            // Create the right footer frame
            footerFrameRight = section.Footers.Primary.AddTextFrame();
            footerFrameRight.Height = "3.0cm";
            footerFrameRight.Width = "7.0cm";
            footerFrameRight.Left = ShapePosition.Right;
            footerFrameRight.RelativeHorizontal = RelativeHorizontal.Margin;
            footerFrameRight.Top = "26cm";
            footerFrameRight.RelativeVertical = RelativeVertical.Page;

            // Create the text frame for the address
            addressFrame = section.Headers.Primary.AddTextFrame();
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "8.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "3cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            //create the invoice detail frame

            invoiceDetailsFrame = section.Headers.Primary.AddTextFrame();
            invoiceDetailsFrame.Height = "3.0cm";
            invoiceDetailsFrame.Width = "6.0cm";
            invoiceDetailsFrame.Left = ShapePosition.Right;
            invoiceDetailsFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            invoiceDetailsFrame.Top = "3cm";
            invoiceDetailsFrame.RelativeVertical = RelativeVertical.Page;

            //create the customer detail frame

            customerDetails = section.AddTextFrame();
            customerDetails.Height = "3.0cm";
            customerDetails.Width = "7.0cm";
            customerDetails.Left = ShapePosition.Left;
            customerDetails.RelativeHorizontal = RelativeHorizontal.Margin;
            customerDetails.Top = "3cm";
            customerDetails.RelativeVertical = RelativeVertical.Page;


            // Add the print date field
            var paragraph = section.AddParagraph();
                    // Create the item table
            table = section.AddTable();
            table.Style = "Table";
            table.Borders.Bottom.Width = 0.25;
            table.Borders.Bottom.Color = Colors.DarkSlateGray;
            table.Rows.LeftIndent = 0;
            paragraph.Format.SpaceAfter = "-0.05cm";
            table.Rows.Height = 25;


            // Before you can add a row, you must define the columns
            var column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;

             column = table.AddColumn("7cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;


            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Right;




        }

        /// <summary>
        ///     Creates the dynamic parts of the invoice.
        /// </summary>
        private void FillContent()
        {
            float previousBalanceTotal = 0;
            float currentBalanceTotal = 0;
            DateTime temp = DateTime.Now;
            temp.AddYears(-1);
            DateTime dateFrom = new DateTime(2019, 01, 01);
            DateTime dateTo = new DateTime(temp.Year, 01, 01);

            DateTime date = DateTime.Now;
            // Fill address in address text frame
            var item = SelectItem("/invoice/to");
            var paragraph = addressFrame.AddParagraph();
            paragraph.Format.SpaceBefore = 50;
            FormattedText ft = paragraph.AddFormattedText("Date",TextFormat.Bold);
            ft.Font.Size = 13;
            paragraph.AddLineBreak();
            ft=paragraph.AddFormattedText(date.ToString(), TextFormat.Bold);
            ft.Color = LogoBlue;
            ft.Font.Size = 13;



            paragraph = invoiceDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.SpaceBefore = 50;
            ft = paragraph.AddFormattedText("Previous Balance Date", TextFormat.Bold);
            ft.Font.Size = 13;


            paragraph.AddLineBreak();
            ft = paragraph.AddFormattedText(dateTo.ToString(), TextFormat.Bold);
            ft.Font.Size = 13;
            ft.Color = LogoBlue;


            // Create the header of the table

            var row = table.AddRow();
            row.HeadingFormat = true;

            row.Cells[4].AddParagraph().AddImage("../../Images/customersBalances-02.png").Width = "15.9cm";

            // Iterate the invoice items
            for (var i = 0; i < customers.Count; i++)
            {
                if (customers[i]!=null)
                {
                    // Each item fills two rows
                    var row1 = table.AddRow();
                    row1.Format.LineSpacing = 15;
                    if (i % 2 == 0)
                    {
                        row1.Shading.Color = Colors.AliceBlue;
                    }
                    row1.VerticalAlignment = VerticalAlignment.Center;
                    row1.TopPadding = 1.5;

                    row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;

                    row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;

                    row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                    row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;

                    row1.Cells[0].AddParagraph(customers[i].idCustomer.ToString());
                    paragraph = row1.Cells[1].AddParagraph();
                    paragraph.AddFormattedText(customers[i].CustomerName, TextFormat.Bold);

                    dateFrom = new DateTime(2019, 01, 01);
                    dateTo = DateTime.Now;

                    float balance = 0;
                    float.TryParse(CustomerViewModel.calculateCustomerBalanceDates(customers[i].idCustomer, dateFrom, dateTo), out balance);
                    currentBalanceTotal += balance;
                    dateFrom = new DateTime(2019, 01, 01);
                    dateTo = new DateTime(temp.Year, 01, 01);

                    float previousBalance = 0;
                    float.TryParse(CustomerViewModel.calculateCustomerBalanceDates(customers[i].idCustomer, dateFrom, dateTo),out previousBalance);
                    previousBalanceTotal += previousBalance;
                    row1.Cells[2].AddParagraph(previousBalance.ToString("c"));
                    row1.Cells[3].AddParagraph(balance.ToString("c"));

                    if (balance > previousBalance)
                    {
                        row1.Cells[4].AddParagraph("+");

                    }
                    else if (balance < previousBalance)

                    {
                        row1.Cells[4].AddParagraph("-");

                    }
                    else
                    {
                        row1.Cells[4].AddParagraph(" ");

                    }
                    row1.Cells[4].Format.Alignment = ParagraphAlignment.Center;

                    row1.Cells[3].VerticalAlignment = VerticalAlignment.Center;

                    row1.Cells[1].VerticalAlignment = VerticalAlignment.Center;

                }
            }



            row = table.AddRow();
            row.Height = 10;
            row.Borders.Visible = false;

            // Add the total price row
            row = table.AddRow();
            row.Height = 20;
            row.Borders.Visible = false;
            row.Cells[0].AddParagraph("Total Balance Due");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].AddParagraph(currentBalanceTotal.ToString("c"));
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;

            row = table.AddRow();
            row.Height = 20;
            row.Borders.Visible = false;
            row.Cells[0].AddParagraph("Previous Total Balance Due");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].AddParagraph(previousBalanceTotal.ToString("c"));
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;




        }

        /// <summary>
        ///     Selects a subtree in the XML data.
        /// </summary>
        private XPathNavigator SelectItem(string path)
        {
            var iter = navigator.Select(path);
            iter.MoveNext();
            return iter.Current;
        }

        /// <summary>
        ///     Gets an element value from the XML data.
        /// </summary>
        private static string GetValue(XPathNavigator nav, string name)
        {
            //nav = nav.Clone();
            var iter = nav.Select(name);
            iter.MoveNext();
            return iter.Current.Value;
        }

        /// <summary>
        ///     Gets an element value as double from the XML data.
        /// </summary>
        private static double GetValueAsDouble(XPathNavigator nav, string name)
        {
            try
            {
                var value = GetValue(nav, name);
                if (value.Length == 0)
                    return 0;
                return double.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return 0;
        }

        // Some pre-defined colors
#if true
        // RGB colors
        private static readonly Color TableBorder = new Color(81, 125, 192);
        private static readonly Color TableBlue = new Color(235, 240, 249);
        private static readonly Color TableGray = new Color(242, 242, 242);
        private static readonly Color LogoBlue = new Color(38, 34, 98);
#else
    // CMYK colors
    readonly static Color tableBorder = Color.FromCmyk(100, 50, 0, 30);
    readonly static Color tableBlue = Color.FromCmyk(0, 80, 50, 30);
    readonly static Color tableGray = Color.FromCmyk(30, 0, 0, 0, 100);
#endif
    }
}