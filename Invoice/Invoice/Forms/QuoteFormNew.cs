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
    public class QuoteFormNew
    {
        /// <summary>
        ///     The MigraDoc document that represents the invoice.
        /// </summary>
        private Document document;

        /// <summary>
        ///     An XML invoice based on a sample created with Microsoft InfoPath.
        /// </summary>
        private readonly XmlDocument quote;

        /// <summary>
        ///     The root navigator for the XML document.
        /// </summary>
        private readonly XPathNavigator navigator;

        /// <summary>
        ///     The text frame of the MigraDoc document that contains the address.
        /// </summary>
        private TextFrame addressFrame;

        private TextFrame quoteDetailsFrame;
        private TextFrame customerDetails;
        private TextFrame footerFrameLeft;
        private TextFrame footerFrameRight;
        private TextFrame tableHeader;

        /// <summary>
        ///     The table of the MigraDoc document that contains the invoice items.
        /// </summary>
        private Table table;

        /// <summary>
        ///     Initializes a new instance of the class BillFrom and opens the specified XML document.
        /// </summary>
        private readonly string[] customer_details = new string[6];

        private readonly string[] quote_details = new string[3];
        private readonly List<Product> products = new List<Product>();

        public QuoteFormNew(string filename, string[] cd, string[] id, List<Product> p)
        {
            quote = new XmlDocument();
            quote.Load(filename);
            navigator = quote.CreateNavigator();
            customer_details = cd;
            quote_details = id;
            products = p;
        }

        /// <summary>
        ///     Creates the invoice document.
        /// </summary>
        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            document = new Document();
            document.Info.Title = "Quote";
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
            var image = section.Headers.Primary.AddImage("../../Images/quoteDesign.png");
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
            section.PageSetup.TopMargin = 250;
            section.PageSetup.BottomMargin = 100;


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

            quoteDetailsFrame = section.Headers.Primary.AddTextFrame();
            quoteDetailsFrame.Height = "3.0cm";
            quoteDetailsFrame.Width = "6.0cm";
            quoteDetailsFrame.Left = ShapePosition.Right;
            quoteDetailsFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            quoteDetailsFrame.Top = "3cm";
            quoteDetailsFrame.RelativeVertical = RelativeVertical.Page;

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
            var column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("4.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("6.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

           
        }

        /// <summary>
        ///     Creates the dynamic parts of the invoice.
        /// </summary>
        private void FillContent()
        {
            // Fill address in address text frame
            var item = SelectItem("/invoice/to");
            var paragraph = addressFrame.AddParagraph();
            paragraph.Format.SpaceBefore = 55;
            FormattedText ft = paragraph.AddFormattedText("Customer Info.",TextFormat.Bold);
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

            paragraph = quoteDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.SpaceBefore = 20;
            var invoiceNumber = quote_details[0];
            var date = quote_details[1];

            paragraph = quoteDetailsFrame.AddParagraph();
            paragraph.AddLineBreak();
            paragraph.AddText("Quote No.");
            paragraph.AddTab();
            paragraph.AddFormattedText(invoiceNumber,TextFormat.Bold);

            paragraph = quoteDetailsFrame.AddParagraph();
            paragraph.AddText("Account No.");
            paragraph.AddTab();
            paragraph.AddFormattedText(customer_details[5], TextFormat.Bold);
            paragraph.Format.SpaceBefore = 2;

            paragraph = quoteDetailsFrame.AddParagraph();
            paragraph.AddText("Date.");
            paragraph.AddTab();
            paragraph.AddTab();
            paragraph.AddFormattedText(date, TextFormat.Bold);
            paragraph.Format.SpaceBefore = 2;

            paragraph = quoteDetailsFrame.AddParagraph();
            paragraph.AddText("Valid Until.");
            paragraph.AddTab();
            paragraph.AddFormattedText("31/12/"+ DateTime.Now.Year.ToString(), TextFormat.Bold);
            paragraph.Format.SpaceBefore = 2;


            //Salesperson information
            var salesperson = "Yiannis Panteli";
            var telephone = "+357 96361198";
            var email = "yiannisp@ecobrightcy.com";

            if (quote_details[2] == "V.A")
            {
                salesperson = "Vasos Anayiotos";
                telephone = "+357 99676298";
                email = "vasos59@hotmail.com";
            }

            paragraph = quoteDetailsFrame.AddParagraph();
            paragraph.Format.SpaceBefore = 5;
            ft = paragraph.AddFormattedText("Salesperson Info.", TextFormat.Bold);
            ft.Font.Size = 12;
            paragraph.AddLineBreak();
            ft = paragraph.AddFormattedText(salesperson, TextFormat.Bold);
            ft.Font.Size = 11;
            ft.Color = LogoBlue;

            paragraph = quoteDetailsFrame.AddParagraph();
            paragraph.AddFormattedText("T. ", TextFormat.Bold);
            ft = paragraph.AddFormattedText(telephone, TextFormat.NotItalic); paragraph.AddLineBreak();
            ft.Font.Color = Colors.DarkSlateGray;
            paragraph.Format.SpaceBefore = 2;

            paragraph.AddFormattedText("E. ", TextFormat.Bold);
            ft = paragraph.AddFormattedText(email, TextFormat.NotItalic);
            ft.Font.Color = Colors.DarkSlateGray;

            var row = table.AddRow();
            row.HeadingFormat = true;

            row.Cells[3].AddParagraph().AddImage("../../Images/quoteDesign-02.png").Width="15.9cm";

            // Iterate the invoice items
            for (var i = 0; i < products.Count; i++)
            {
                double quantity = products[i].Quantity;
                var price = GetValueAsDouble(item, "price");
                var discount = GetValueAsDouble(item, "discount");

                // Each item fills two rows
                var row1 = table.AddRow();
                row1.VerticalAlignment = VerticalAlignment.Center;

                row1.Format.LineSpacing = 15;
                if (i % 2 == 0)
                {
                    row1.Shading.Color = Colors.AliceBlue;
                }
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;

                row1.Cells[0].AddParagraph(products[i].idProduct.ToString());
                paragraph = row1.Cells[1].AddParagraph();
                paragraph.AddFormattedText(products[i].ProductName, TextFormat.Bold);
                row1.Cells[2].AddParagraph(products[i].ProductDescription);
                row1.Cells[3].AddParagraph(products[i].OfferPrice.ToString("c"));

               

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

            paragraph = table.Section.AddParagraph();
            paragraph.Format.RightIndent = 350;
            paragraph.Format.SpaceBefore = 20;
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
            ft = paragraph.AddFormattedText(" The above prices do not include VAT.", TextFormat.NotItalic);
            ft.Font.Size = 8;
            ft.Font.Color = Colors.DarkSlateGray;
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            ft = paragraph.AddFormattedText("Thank you for your business!", TextFormat.Bold);
            ft.Font.Size = 8;
            ft.Color = Colors.DarkGray;

            //add number of pages
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            ft=paragraph.AddFormattedText("Page ", TextFormat.NotItalic);
            ft.Font.Size = 8;
            paragraph.AddPageField();
            ft=paragraph.AddFormattedText(" of ", TextFormat.NotItalic);
            ft.Font.Size = 8;
            paragraph.AddNumPagesField();

            paragraph = footerFrameRight.AddParagraph();
            paragraph.Format.Borders.Bottom = new Border() { Width = "1pt", Color = Colors.DarkGray };
            paragraph = footerFrameRight.AddParagraph();
            paragraph.Format.SpaceBefore = 5;
            ft = paragraph.AddFormattedText("Yiannis Panteli", TextFormat.Bold);
            paragraph.AddLineBreak();

            paragraph = footerFrameRight.AddParagraph();
            paragraph.Format.SpaceBefore = 5;
            ft = paragraph.AddFormattedText("If you have any questions concerning this quotation, please contact: ", TextFormat.NotItalic);
            paragraph.AddLineBreak();
            ft.Font.Size = 8;
            ft.Font.Color = Colors.DarkSlateGray;
            ft = paragraph.AddFormattedText("Email: info@ecobrightcy.com", TextFormat.NotItalic);
            ft.Font.Size = 8;
            ft.Font.Color = Colors.DarkSlateGray;

            paragraph.AddLineBreak();
            ft = paragraph.AddFormattedText("Phone: +357 96361198", TextFormat.NotItalic);
            paragraph.AddLineBreak();
        
            ft.Font.Size = 8;
            ft.Font.Color = Colors.DarkSlateGray;


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