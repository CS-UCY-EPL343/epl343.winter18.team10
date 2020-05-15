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
    ///     Creates the credit note form.
    /// </summary>
    public class CreditNoteForm
    {
        /// <summary>
        ///     The MigraDoc document that represents the credit note.
        /// </summary>
        private Document document;

        /// <summary>
        ///     An XML credit note based on a sample created with Microsoft InfoPath.
        /// </summary>
        private readonly XmlDocument creditNote;

        /// <summary>
        ///     The root navigator for the XML document.
        /// </summary>
        private readonly XPathNavigator navigator;

        /// <summary>
        ///     The text frame of the MigraDoc document that contains the address.
        /// </summary>
        private TextFrame addressFrame;

        private TextFrame creditNoteDetailsFrame;
        private TextFrame customerDetails;

        /// <summary>
        ///     The table of the MigraDoc document that contains the credit note items.
        /// </summary>
        private Table table;

        /// <summary>
        ///     Initializes a new instance of the class BillFrom and opens the specified XML document.
        /// </summary>
        private readonly string[] customer_details = new string[6];

        private readonly string[] creditNote_details = new string[6];
        private readonly List<Product> products = new List<Product>();

        public CreditNoteForm(string filename, string[] cd, string[] id, List<Product> p)
        {
            creditNote = new XmlDocument();
            creditNote.Load(filename);
            navigator = creditNote.CreateNavigator();
            customer_details = cd;
            creditNote_details = id;
            products = p;
        }

        /// <summary>
        ///     Creates the credit note document.
        /// </summary>
        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            document = new Document();
            document.Info.Title = "Credit Note";
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
        ///     Creates the static parts of the credit note.
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
            par.AddFormattedText("CREDIT NOTE", TextFormat.Bold);

            // Create footer
            var paragraph = section.Footers.Primary.AddParagraph();
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

            //create the credit note detail frame

            creditNoteDetailsFrame = section.AddTextFrame();
            creditNoteDetailsFrame.Height = "3.0cm";
            creditNoteDetailsFrame.Width = "7.0cm";
            creditNoteDetailsFrame.Left = ShapePosition.Right;
            creditNoteDetailsFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            creditNoteDetailsFrame.Top = "3cm";
            creditNoteDetailsFrame.RelativeVertical = RelativeVertical.Page;

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
            paragraph.Format.SpaceBefore = "6cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("CREDIT NOTE", TextFormat.Bold);

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

            column = table.AddColumn("8.5cm");
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

            row.Cells[0].AddParagraph("QTY");
            row.Cells[0].Format.Font.Bold = true;
            row.Format.LineSpacing = 10;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph("DESCRIPTION");
            row.Cells[1].Format.Font.Bold = true;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[2].AddParagraph("UNIT PRICE");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[3].AddParagraph("AMOUNT");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            table.SetEdge(0, 0, 3, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        /// <summary>
        ///     Creates the dynamic parts of the credit note.
        /// </summary>
        private void FillContent()
        {
            // Fill address in address text frame
            var item = SelectItem("/creditNote/to");
            var paragraph = addressFrame.AddParagraph();
            paragraph.AddText(GetValue(item, "name/singleName"));
            paragraph.AddLineBreak();
            paragraph.AddText(GetValue(item, "address/line1") + " " + GetValue(item, "address/postalCode") + " " +
                              GetValue(item, "address/city"));
            paragraph.AddLineBreak();
            paragraph.AddText(GetValue(item, "telephoneNumberHome") + ", " + GetValue(item, "telephoneNumberCell"));
            paragraph.AddLineBreak();
            paragraph.AddText(GetValue(item, "emailAddressPrimary"));
            paragraph.AddLineBreak();
            paragraph.AddText(GetValue(item, "webSite"));

            paragraph = creditNoteDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.SpaceBefore = 48;
            paragraph.Format.Shading.Color = LogoBlue;
            paragraph.Format.Font.Color = new Color(255, 255, 255);
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Size = 11;
            paragraph.AddText("CREDIT NOTE #");
            paragraph.AddSpace(7);
            paragraph.AddText("DATE");

            paragraph = creditNoteDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            var creditNoteNumber = creditNote_details[0];
            var date = creditNote_details[1];
            paragraph.AddText(creditNoteNumber);
            paragraph.AddSpace(24);
            paragraph.AddText(date);

            paragraph = addressFrame.AddParagraph();
            paragraph.AddLineBreak();

            paragraph = customerDetails.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.SpaceBefore = 80;
            paragraph.Format.Shading.Color = LogoBlue;
            paragraph.Format.Font.Color = new Color(255, 255, 255);
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Size = 11;
            paragraph.AddText("BILL TO");

            paragraph = creditNoteDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.SpaceBefore = 7;
            paragraph.Format.Shading.Color = LogoBlue;
            paragraph.Format.Font.Color = new Color(255, 255, 255);
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Size = 11;
            paragraph.AddText("CUSTOMER ID");
            paragraph.AddTab();
            paragraph.AddText("BALANCE");

            paragraph = creditNoteDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            var customerId = customer_details[5];
            var balance = customer_details[4];
            paragraph.AddText(customerId);
            paragraph.AddSpace(20);
            paragraph.AddText(balance);


            paragraph = addressFrame.AddParagraph();
            paragraph.Format.SpaceBefore = 20;
            paragraph.AddText(customer_details[0]);
            paragraph.AddLineBreak();
            paragraph.AddText(customer_details[1]);
            paragraph.AddLineBreak();
            paragraph.AddText(customer_details[2]);
            paragraph.AddLineBreak();
            paragraph.AddText(customer_details[3]);

            // Iterate the credit note items

            for (var i = 0; i < products.Count; i++)
            {
                double quantity = products[i].Quantity;
                var price = GetValueAsDouble(item, "price");
                var discount = GetValueAsDouble(item, "discount");

                // Each item fills two rows
                var row1 = table.AddRow();
                row1.VerticalAlignment = VerticalAlignment.Center;
                row1.TopPadding = 1.5;
                row1.Cells[0].Shading.Color = TableGray;
                row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[0].AddParagraph(products[i].Quantity.ToString());
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;

                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[1].VerticalAlignment = VerticalAlignment.Center;

                row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[2].AddParagraph(products[i].SellPrice.ToString("c"));

                row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[3].Shading.Color = TableGray;
                row1.Cells[3].AddParagraph(products[i].Total.ToString("c"));
                row1.Cells[3].VerticalAlignment = VerticalAlignment.Center;

                paragraph = row1.Cells[1].AddParagraph();
                paragraph.AddFormattedText(products[i].ProductName, TextFormat.Bold);

                table.SetEdge(0, table.Rows.Count - 2, 4, 2, Edge.Box, BorderStyle.Single, 0.75);
            }

            // Add an invisible row as a space line to the table
            var row = table.AddRow();
            row.Borders.Visible = false;

            // Add the total price row
            row = table.AddRow();
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].AddParagraph("Total Price");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 2;
            row.Cells[3].AddParagraph(creditNote_details[3]);

            // Add the VAT row
            row = table.AddRow();
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].AddParagraph("VAT (19%)");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 2;
            row.Cells[3].AddParagraph(creditNote_details[4]);


            // Add the total due row
            row = table.AddRow();
            row.Cells[0].AddParagraph("Total Due");
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 2;
            row.Cells[3].AddParagraph(creditNote_details[5]);

            // Set the borders of the specified cell range
            table.SetEdge(3, table.Rows.Count - 4, 1, 4, Edge.Box, BorderStyle.Single, 0.75);
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