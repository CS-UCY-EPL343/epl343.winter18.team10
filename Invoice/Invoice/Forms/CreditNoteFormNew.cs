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

namespace InvoiceX.Forms
{
    /// <summary>
    ///     Creates the invoice form.
    /// </summary>
    public class CreditNoteFormNew
    {
        /// <summary>
        ///     The MigraDoc document that represents the invoice.
        /// </summary>
        private Document document;

        /// <summary>
        ///     An XML invoice based on a sample created with Microsoft InfoPath.
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
        private TextFrame footerFrameLeft;
        private TextFrame footerFrameRight;

        /// <summary>
        ///     The table of the MigraDoc document that contains the invoice items.
        /// </summary>
        private Table table;

        /// <summary>
        ///     Initializes a new instance of the class BillFrom and opens the specified XML document.
        /// </summary>
        private readonly string[] customer_details = new string[6];

        private readonly string[] creditNote_details = new string[6];
        private readonly List<Product> products = new List<Product>();

        public CreditNoteFormNew(string filename, string[] cd, string[] id, List<Product> p)
        {
            creditNote = new XmlDocument();
            creditNote.Load(filename);
            navigator = creditNote.CreateNavigator();
            customer_details = cd;
            creditNote_details = id;
            products = p;
        }

        /// <summary>
        ///     Creates the invoice document.
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
            var image = section.Headers.Primary.AddImage("../../Images/CreditNoteDesign.png");
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
            addressFrame = section.AddTextFrame();
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "8.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "3cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            //create the invoice detail frame

            creditNoteDetailsFrame = section.AddTextFrame();
            creditNoteDetailsFrame.Height = "3.0cm";
            creditNoteDetailsFrame.Width = "6.0cm";
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
            var paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "6cm";
            paragraph.Style = "Reference";


            // Create the item table
            table = section.AddTable();
            table.Style = "Table";
            table.Borders.Bottom.Width = 0.25;
            table.Borders.Bottom.Color = Colors.DarkSlateGray;
            table.Rows.LeftIndent = 0;
            paragraph.Format.SpaceAfter = "-0.05cm";
            table.Rows.Height = 25;


            // Before you can add a row, you must define the columns

            var column = table.AddColumn("9cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;


            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Create the header of the table
            paragraph.AddLineBreak();
            image = paragraph.AddImage("../../Images/invoiceDesign2.png");
            image.Width = "16cm";
            image.LockAspectRatio = true;



        }

        /// <summary>
        ///     Creates the dynamic parts of the invoice.
        /// </summary>
        private void FillContent()
        {
            // Fill address in address text frame
            var item = SelectItem("/invoice/to");
            var paragraph = addressFrame.AddParagraph();
            paragraph.Format.SpaceBefore = 50;
            FormattedText ft = paragraph.AddFormattedText("Bill To.",TextFormat.Bold);
            ft.Font.Size = 13;
            paragraph.AddLineBreak();
            ft=paragraph.AddFormattedText(customer_details[0], TextFormat.Bold);
            ft.Font.Size = 12;
            ft.Color = LogoBlue;

            paragraph = addressFrame.AddParagraph();
            paragraph.AddFormattedText("A. ", TextFormat.Bold);
            ft=paragraph.AddFormattedText(customer_details[1],TextFormat.NotItalic);
            ft.Font.Color = Colors.DarkSlateGray;
            paragraph.Format.SpaceBefore = 5;

            paragraph.AddLineBreak();
            paragraph.AddFormattedText("T. ", TextFormat.Bold);
            ft = paragraph.AddFormattedText(customer_details[2], TextFormat.NotItalic); paragraph.AddLineBreak();
            ft.Font.Color = Colors.DarkSlateGray;

            paragraph.AddFormattedText("E. ", TextFormat.Bold);
            ft = paragraph.AddFormattedText(customer_details[3], TextFormat.NotItalic);
            ft.Font.Color = Colors.DarkSlateGray;

            paragraph = creditNoteDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.SpaceBefore = 50;
            var invoiceNumber = creditNote_details[0];
            var date = creditNote_details[1];
            ft=paragraph.AddFormattedText("Account Due",TextFormat.Bold);
            ft.Font.Size = 13;

       
            paragraph.AddLineBreak();
            ft = paragraph.AddFormattedText("€ "+customer_details[4], TextFormat.NotBold);
            ft.Font.Size = 14;
            ft.Color = LogoBlue;

            paragraph = creditNoteDetailsFrame.AddParagraph();
            paragraph.AddLineBreak();
            paragraph.AddText("Doc. No.");
            paragraph.AddTab();
            paragraph.AddFormattedText(invoiceNumber,TextFormat.Bold);

            paragraph = creditNoteDetailsFrame.AddParagraph();
            paragraph.AddText("Account No.");
            paragraph.AddTab();
            paragraph.AddFormattedText(customer_details[5], TextFormat.Bold);
            paragraph.Format.SpaceBefore = 5;

            paragraph = creditNoteDetailsFrame.AddParagraph();
            paragraph.AddText("Date.");
            paragraph.AddTab();
            paragraph.AddTab();
            paragraph.AddFormattedText(date, TextFormat.Bold);
            paragraph.Format.SpaceBefore = 5;

            // Iterate the invoice items
            for (var i = 0; i < products.Count; i++)
            {
                double quantity = products[i].Quantity;
                var price = GetValueAsDouble(item, "price");
                var discount = GetValueAsDouble(item, "discount");

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
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;

                row1.Cells[2].AddParagraph(products[i].Quantity.ToString());
                paragraph = row1.Cells[0].AddParagraph();
                paragraph.AddFormattedText(products[i].ProductName, TextFormat.Bold);
                row1.Cells[1].AddParagraph(products[i].SellPrice.ToString("c"));
                row1.Cells[3].AddParagraph(products[i].Total.ToString("c"));
                row1.Cells[3].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[1].VerticalAlignment = VerticalAlignment.Center;

            }
            //Fill totally 5 rows
            if (products.Count < 5){
                for (int i = products.Count; i < 5; i++)
                {
                    var row1 = table.AddRow();
                    row1.Format.LineSpacing = 15;
                    if (i % 2 == 0)
                    {
                        row1.Shading.Color = Colors.AliceBlue;
                    }

                }
            }


            // Add an invisible row as a space line to the table
            var row = table.AddRow();
            row.Height = 10;
            row.Borders.Visible = false;

            // Add the total price row
            row = table.AddRow();
            row.Height = 20;

            row.Borders.Visible = false;
            row.Cells[0].AddParagraph("Sub Total");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 2;
            row.Cells[3].AddParagraph(creditNote_details[3]);
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            // Add the VAT row
            row = table.AddRow();
            row.Height = 20;

            row.Borders.Visible = false;
            row.Cells[0].AddParagraph("VAT (19%)");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 2;
            row.Cells[3].AddParagraph(creditNote_details[4]);
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;


            // Add the total due row
            row = table.AddRow();
            row.Height = 20;

            paragraph = row.Cells[0].AddParagraph("Grand Total");
            row.Borders.Visible = false;
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 2;
            row.Cells[3].AddParagraph(creditNote_details[5]);
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            paragraph = table.Section.AddParagraph();
            paragraph.Format.RightIndent = 350;
            paragraph.Format.SpaceBefore = -60;
            ft = paragraph.AddFormattedText("Company Info:", TextFormat.Bold);
            ft.Font.Size = 10;
           paragraph.Format.Borders.Bottom = new Border() { Width = "1pt", Color = Colors.DarkGray };

            paragraph = table.Section.AddParagraph();
            ft = paragraph.AddFormattedText("VAT Number: ", TextFormat.Bold);
            ft.Font.Size = 9; 
            ft.AddTab(); ft.AddTab();
            ft = paragraph.AddFormattedText("CY10138984C", TextFormat.NotBold);
            ft.Font.Size = 9;
            ft.Font.Color = Colors.DarkSlateGray;
            paragraph.Format.SpaceBefore = 5;

            paragraph = table.Section.AddParagraph();
            ft = paragraph.AddFormattedText("Cheque: ", TextFormat.Bold);
            ft.Font.Size = 9;
            ft.AddTab(); ft.AddTab();
            ft = paragraph.AddFormattedText("Andreas Panteli Trading LTD", TextFormat.NotBold);
            ft.Font.Size = 9;
            ft.Font.Color = Colors.DarkSlateGray;
            paragraph.Format.SpaceBefore = 5;

            paragraph = table.Section.AddParagraph();
            ft = paragraph.AddFormattedText("BANK: ", TextFormat.Bold);
            ft.Font.Size = 9;
            ft.AddTab(); ft.AddTab(); ft.AddTab();
            ft = paragraph.AddFormattedText("Hellenic Bank ", TextFormat.NotBold);
            ft.Font.Size = 9;
            ft.Font.Color = Colors.DarkSlateGray;
            paragraph.Format.SpaceBefore = 5;

            paragraph = table.Section.AddParagraph();
            ft = paragraph.AddFormattedText("IBAN: ", TextFormat.Bold);
            ft.Font.Size = 9;
            ft.AddTab(); ft.AddTab(); ft.AddTab();
            ft = paragraph.AddFormattedText("CY920050034500034501A6313801", TextFormat.NotBold);
            ft.Font.Size = 9;
            ft.Font.Color = Colors.DarkSlateGray;
            paragraph.Format.SpaceBefore = 5;
            paragraph = table.Section.AddParagraph();

            ft = paragraph.AddFormattedText("SWIFT: ", TextFormat.Bold);
            ft.Font.Size = 9;
            ft.AddTab(); ft.AddTab();
            ft = paragraph.AddFormattedText("HEBACY2N ", TextFormat.NotBold);
            ft.Font.Size = 9;
            ft.Font.Color = Colors.DarkSlateGray;
            paragraph.Format.SpaceBefore = 5;


            //Fill the footer frame
            paragraph = footerFrameLeft.AddParagraph();
            ft = paragraph.AddFormattedText("Terms & Conditions:", TextFormat.Bold);
            ft.Font.Size = 10;
            paragraph.Format.Borders.Bottom = new Border() { Width = "1pt", Color = Colors.DarkGray };

            paragraph = footerFrameLeft.AddParagraph();
            paragraph.Format.SpaceBefore = 5;
            ft = paragraph.AddFormattedText(" Καθυστερημένα τιμολόγια πέραν των 30 ημερών θα χρεώνονται με τόκο 5% μηνιαίως.",TextFormat.NotItalic);
            ft.Font.Size = 8;
            ft.Font.Color = Colors.DarkSlateGray;
            ft = paragraph.AddFormattedText(" Τα προϊόντα παραμένουν στην ιδιοκτησία της Εταιρείας μέχρι την εξώφληση τους",TextFormat.NotItalic);
            ft.Font.Size = 8;
            ft.Font.Color = Colors.DarkSlateGray;
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            ft = paragraph.AddFormattedText("Thank you for your business!", TextFormat.Bold);
            ft.Color = Colors.DarkGray;
            ft.Font.Size = 9;

            paragraph = footerFrameRight.AddParagraph();
            paragraph.Format.Borders.Bottom = new Border() { Width = "1pt", Color = Colors.DarkGray };
            paragraph = footerFrameRight.AddParagraph();
            paragraph.Format.SpaceBefore = 5;
            ft = paragraph.AddFormattedText("Yiannis Panteli", TextFormat.Bold);
            paragraph.AddLineBreak();

            paragraph = footerFrameRight.AddParagraph();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();

            paragraph.Format.Borders.Bottom = new Border() { Width = "1pt", Color = Colors.DarkGray };
            paragraph = footerFrameRight.AddParagraph();

            paragraph.Format.SpaceBefore = 5;
            ft = paragraph.AddFormattedText("Buyer", TextFormat.Bold);
            paragraph.AddLineBreak();

            ft.Font.Size = 10;

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