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

namespace InvoiceX.Forms
{
    /// <summary>
    ///     Creates the quote form.
    /// </summary>
    public class ReceiptForm
    {
        /// <summary>
        ///     The MigraDoc document that represents the quote.
        /// </summary>
        private Document document;

        /// <summary>
        ///     An XML quote based on a sample created with Microsoft InfoPath.
        /// </summary>
        private readonly XmlDocument receipt;

        /// <summary>
        ///     The root navigator for the XML document.
        /// </summary>
        private readonly XPathNavigator navigator;

        /// <summary>
        ///     The text frame of the MigraDoc document that contains the address.
        /// </summary>
        private TextFrame addressFrame;

        private TextFrame receiptDetailsFrame;
        private TextFrame customerDetails;

        /// <summary>
        ///     The table of the MigraDoc document that contains the quote items.
        /// </summary>
        private Table table;

        /// <summary>
        ///     Initializes a new instance of the class BillFrom and opens the specified XML document.
        /// </summary>
        private readonly string[] customer_details = new string[6];

        private readonly string[] receipt_details = new string[4];
        private readonly List<Payment> payments = new List<Payment>();

        public ReceiptForm(string filename, string[] cd, string[] id, List<Payment> p)
        {
            receipt = new XmlDocument();
            receipt.Load(filename);
            navigator = receipt.CreateNavigator();
            customer_details = cd;
            receipt_details = id;
            payments = p;
        }

        /// <summary>
        ///     Creates the quote document.
        /// </summary>
        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            document = new Document();
            document.Info.Title = "Receipt";
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
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        /// <summary>
        ///     Creates the static parts of the quote.
        /// </summary>
        private void CreatePage()
        {
            // Each MigraDoc document needs at least one section.
            var section = document.AddSection();

            // Put a logo in the header
            var image = section.Headers.Primary.AddImage("../../Images/companyLogo.png");
            image.Height = "1.5cm";
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
            par.AddFormattedText("Receipt", TextFormat.Bold);

            // Create footer

            var paragraph = section.Footers.Primary.AddParagraph();
            paragraph.Format.SpaceBefore = 5;
            paragraph.AddText(
                "If you have any questions concerning this quotation, please contact us. Thank you for your business!");
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.AddLineBreak();

            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("PowerBooks Inc · Sample Street 42 · 56789 Cologne · Germany");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            // Create the text frame for the address
            addressFrame = section.AddTextFrame();
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "7.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "3cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            //create the quote detail frame

            receiptDetailsFrame = section.AddTextFrame();
            receiptDetailsFrame.Height = "3.0cm";
            receiptDetailsFrame.Width = "7.0cm";
            receiptDetailsFrame.Left = ShapePosition.Right;
            receiptDetailsFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            receiptDetailsFrame.Top = "3cm";
            receiptDetailsFrame.RelativeVertical = RelativeVertical.Page;

            //create the customer detail frame

            customerDetails = section.AddTextFrame();
            customerDetails.Height = "3.0cm";
            customerDetails.Width = "7.0cm";
            customerDetails.Left = ShapePosition.Left;
            customerDetails.RelativeHorizontal = RelativeHorizontal.Margin;
            customerDetails.Top = "3cm";
            customerDetails.RelativeVertical = RelativeVertical.Page;


            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "4cm";
            paragraph.Style = "Reference";
            // Create the item table
            table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = TableBorder;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;


            // Before you can add a row, you must define the columns
            var column = table.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("5.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;


            // Create the header of the table
            var row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = LogoBlue;
            row.Format.Font.Color = new Color(255, 255, 255);
            row.TopPadding = 3;
            row.BottomPadding = 3;
            row.Format.Font.Size = 10;

            row.Cells[0].AddParagraph("NO");
            row.Cells[0].Format.Font.Bold = true;
            row.Format.LineSpacing = 10;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph("PAYMENT METHOD");
            row.Cells[1].Format.Font.Bold = true;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[2].AddParagraph("PAYMENT NUMBER");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[3].AddParagraph("DATE");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[4].AddParagraph("AMOUNT");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Center;

            table.SetEdge(0, 0, 3, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        /// <summary>
        ///     Creates the dynamic parts of the quote.
        /// </summary>
        private void FillContent()
        {
            // Fill address in address text frame
            var item = SelectItem("/receipt/to");
            var paragraph = addressFrame.AddParagraph();
            paragraph.AddText("Andreas Panteli Trading LTD");
            paragraph.AddLineBreak();
            paragraph.AddText("Neas Ionias 18, Aradippou, Larnaca");
            paragraph.AddLineBreak();
            paragraph.AddText("+357 24433172, +357 99560755");
            paragraph.AddLineBreak();
            paragraph.AddText("info@ecobrightcy.com");
            paragraph.AddLineBreak();
            paragraph.AddText("www.ecobrightcy.com");


            paragraph = receiptDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.SpaceBefore = 48;
            paragraph.Format.Shading.Color = LogoBlue;
            paragraph.Format.Font.Color = new Color(255, 255, 255);
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Size = 11;
            paragraph.AddText("RECEIPT #");
            paragraph.AddSpace(16);
            paragraph.AddText("DATE");

            paragraph = receiptDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            var quoteNumber = receipt_details[0];
            var date = receipt_details[1];
            paragraph.AddText(quoteNumber);
            paragraph.AddSpace(24);
            paragraph.AddText(date);

            paragraph.AddLineBreak();
            paragraph = addressFrame.AddParagraph();
            paragraph.AddLineBreak();
            paragraph.AddText("Received from: ");
            paragraph.AddFormattedText(customer_details[0], TextFormat.Bold);

            paragraph.AddLineBreak();
            paragraph.AddText("the amount: €");
            paragraph.AddFormattedText(receipt_details[3], TextFormat.Bold);
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();

            paragraph.AddText("Payment Methods: ");


            // Iterate the invoice items

            for (var i = 0; i < payments.Count; i++)
            {
                // Each item fills two rows
                var row1 = table.AddRow();
                row1.TopPadding = 1.5;
                row1.Cells[0].Shading.Color = TableGray;
                row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[3].Shading.Color = TableGray;

                row1.Cells[0].AddParagraph((i + 1).ToString());
                paragraph = row1.Cells[1].AddParagraph();
                paragraph.AddFormattedText(payments[i].paymentMethod.ToString(), TextFormat.Bold);
                row1.Cells[2].AddParagraph(payments[i].paymentNumber);
                row1.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[3].AddParagraph(payments[i].paymentDate.ToString("dd/MM/yyyy"));
                row1.Cells[3].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[4].AddParagraph(payments[i].amount.ToString());
                row1.Cells[4].VerticalAlignment = VerticalAlignment.Center;


                table.SetEdge(0, table.Rows.Count - 2, 5, 2, Edge.Box, BorderStyle.Single, 0.75);
            }

            // Add an invisible row as a space line to the table
            var row = table.AddRow();
            row.Borders.Visible = false;
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