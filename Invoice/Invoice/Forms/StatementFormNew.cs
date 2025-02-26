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
    public class StatementFormNew
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

        private TextFrame statementDetailsFrame;
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
        private readonly string[] customer_details = new string[7];

        private readonly string[] statement_details = new string[4];
        private readonly List<StatementItem> items = new List<StatementItem>();
        private readonly List<StatementItem> allItems = new List<StatementItem>();

        public StatementFormNew(string filename, string[] cd, string[] id, List<StatementItem> p, List<StatementItem> allItems)
        {
            invoice = new XmlDocument();
            invoice.Load(filename);
            navigator = invoice.CreateNavigator();
            customer_details = cd;
            statement_details = id;
            items = p;
            this.allItems=allItems;
        }

        /// <summary>
        ///     Creates the invoice document.
        /// </summary>
        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            document = new Document();
            document.Info.Title = "Statement";
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
            var image = section.Headers.Primary.AddImage("../../Images/statementDesign.png");
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

            section.PageSetup.TopMargin = 255;
            section.PageSetup.BottomMargin = 100;


            // Create the left footer frame
            footerFrameLeft = section.Footers.Primary.AddTextFrame();
            footerFrameLeft.Height = "3.0cm";
            footerFrameLeft.Width = "14cm";
            footerFrameLeft.Left = ShapePosition.Left;
            footerFrameLeft.RelativeHorizontal = RelativeHorizontal.Margin;
            footerFrameLeft.Top = "26cm";
            footerFrameLeft.RelativeVertical = RelativeVertical.Page;

            // Create the right footer frame

            // Create the text frame for the address
            addressFrame = section.Headers.Primary.AddTextFrame();
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "8.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "3cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            //create the invoice detail frame

            statementDetailsFrame = section.Headers.Primary.AddTextFrame();
            statementDetailsFrame.Height = "3.0cm";
            statementDetailsFrame.Width = "6.0cm";
            statementDetailsFrame.Left = ShapePosition.Right;
            statementDetailsFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            statementDetailsFrame.Top = "3cm";
            statementDetailsFrame.RelativeVertical = RelativeVertical.Page;

            //create the customer detail frame

            customerDetails = section.Headers.Primary.AddTextFrame();
            customerDetails.Height = "3.0cm";
            customerDetails.Width = "7.0cm";
            customerDetails.Left = ShapePosition.Left;
            customerDetails.RelativeHorizontal = RelativeHorizontal.Margin;
            customerDetails.Top = "3cm";
            customerDetails.RelativeVertical = RelativeVertical.Page;


            // Add the print date field
            var paragraph = section.AddParagraph();
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
            var column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("6cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;


            // Create the header of the table
            image = paragraph.AddImage("../../Images/statementDesign-02.png");
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
            paragraph.Format.SpaceBefore = 60;
            FormattedText ft = paragraph.AddFormattedText("Customer Info.", TextFormat.Bold);
            ft.Font.Size = 13;
            paragraph.AddLineBreak();
            ft=paragraph.AddFormattedText(customer_details[0], TextFormat.Bold);
            ft.Font.Size = 12;
            ft.Color = LogoBlue;

            paragraph = addressFrame.AddParagraph();
            paragraph.AddFormattedText("A. ", TextFormat.Bold);
            ft=paragraph.AddFormattedText(customer_details[1], TextFormat.NotItalic);
            ft.Font.Color = Colors.DarkSlateGray;
            paragraph.Format.SpaceBefore = 5;

            paragraph.AddLineBreak();
            paragraph.AddFormattedText("T. ", TextFormat.Bold);
            ft = paragraph.AddFormattedText(customer_details[2], TextFormat.NotItalic); paragraph.AddLineBreak();
            ft.Font.Color = Colors.DarkSlateGray;

            paragraph.AddFormattedText("E. ", TextFormat.Bold);
            ft = paragraph.AddFormattedText(customer_details[3], TextFormat.NotItalic);
            ft.Font.Color = Colors.DarkSlateGray;

            paragraph = statementDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.SpaceBefore = 60;
            ft=paragraph.AddFormattedText("Account Due", TextFormat.Bold);
            ft.Font.Size = 13;


            paragraph.AddLineBreak();
            ft = paragraph.AddFormattedText("€ "+customer_details[6], TextFormat.NotBold);
            ft.Font.Size = 14;
            ft.Color = LogoBlue;

            paragraph = statementDetailsFrame.AddParagraph();
            paragraph.AddLineBreak();
            paragraph.AddText("Account No.");
            paragraph.AddTab();
            paragraph.AddFormattedText(customer_details[4], TextFormat.Bold);

            paragraph = statementDetailsFrame.AddParagraph();
            paragraph.AddText("Date From.");
            paragraph.AddTab();
            paragraph.AddFormattedText(statement_details[0], TextFormat.Bold);
            paragraph.Format.SpaceBefore = 5;

            paragraph = statementDetailsFrame.AddParagraph();
            paragraph.AddText("Date To.");
            paragraph.AddTab();
            paragraph.AddFormattedText(statement_details[1], TextFormat.Bold);
            paragraph.Format.SpaceBefore = 5;

            //Create the previous balance row

            var row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Center;
            paragraph = row.Cells[1].AddParagraph();
            paragraph.AddFormattedText("Brought Forward", TextFormat.Bold);
            row.Cells[4].AddParagraph(customer_details[5]);
            float amount = float.Parse(customer_details[5]);
            // Iterate the invoice items
            float sumCharges = 0;
            float sumCredits = 0;
            float sum30 = 0;
            float sum60 = 0;
            float sum90 = 0;
            float sumOver90 = 0;
            float temp = 0;
            bool flag = false;

            for (var i = 0; i < allItems.Count; i++)
            {
                sumCharges+=allItems[i].charges;
                sumCredits+=allItems[i].credits;
            }
            for (var i = 0; i < allItems.Count; i++) { 

                sumCredits=sumCredits-allItems[i].charges;
            if (i+1<allItems.Count-1 && sumCredits-allItems[i+1].charges<0 && !flag)
            {
                flag=true;
                temp=sumCredits;
                }
                if (sumCredits<0)
            {
                int dayDifference = (DateTime.Now-allItems[i].createdDate).Days;
                if (dayDifference<=30)
                {
                    sum30+=allItems[i].charges;
                        Debug.WriteLine("0-30: "+sum30);
                }
                else if (dayDifference<=60)
                {
                    sum60+=allItems[i].charges;
                        Debug.WriteLine("30-60: "+sum60);

                    }
                    else if (dayDifference<=90)
                {
                    sum90+=allItems[i].charges;
                        Debug.WriteLine("60-90: "+sum90);

                    }
                    else
                {
                    sumOver90+=allItems[i].charges;
                        Debug.WriteLine("90+: "+sumOver90);

                    }
                }
        }
            if (sumOver90>0)
            {
                sumOver90-=temp;
            }
            else if (sum90>0)
            {
                sum90-=temp;
            }
            else if (sum60>0)
            {
                sum60-=temp;
            }
            else if (sum30>0)
            {
                sum30-=temp;
            }
                for (var i = 0; i < items.Count; i++)
            {
               
                // Each item fills two rows
                var row1 = table.AddRow();
                row1.VerticalAlignment = VerticalAlignment.Center;

                if (i % 2 == 0)
                {
                    row1.Shading.Color = Colors.AliceBlue;
                }
                row1.Cells[0].AddParagraph(items[i].createdDate.ToString("dd/MM/yyyy"));

                paragraph = row1.Cells[1].AddParagraph();
                paragraph.AddFormattedText(items[i].description, TextFormat.Bold);
                row1.Cells[2].AddParagraph(items[i].charges.ToString());
                row1.Cells[3].AddParagraph(items[i].credits.ToString());
                amount = amount + items[i].charges-items[i].credits;

                
                if (i == items.Count - 1)
                {
                    paragraph = row1.Cells[4].AddParagraph();
                    paragraph.AddFormattedText(amount.ToString("c"), TextFormat.Bold);
                }else { row1.Cells[4].AddParagraph(amount.ToString("c")); }
            }
                //Fill totally 5 rows
                if (items.Count < 5){
                for (int i = items.Count; i < 5; i++)
                {
                    var row1 = table.AddRow();
                    row1.Format.LineSpacing = 15;
                    if (i % 2 == 0)
                    {
                        row1.Shading.Color = Colors.AliceBlue;
                    }

                }
            }

            paragraph = footerFrameLeft.AddParagraph();
            // Add the print date field

            // Create the item table
            table = footerFrameLeft.AddTable();
            table.Style = "Table";
            table.Borders.Bottom.Width = 0.25;
            table.Borders.Bottom.Color = Colors.DarkSlateGray;
            table.Rows.LeftIndent = 0;
            paragraph.Format.SpaceAfter = "-0.05cm";
            table.Rows.Height = 25;
            // Before you can add a row, you must define the columns
            var column = table.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            //ageing analysis
            var row2 = table.AddRow();
            row2.VerticalAlignment = VerticalAlignment.Center;

            paragraph = row2.Cells[0].AddParagraph();
            paragraph.AddFormattedText("Ageing Analysis", TextFormat.Bold);

            row2.Cells[1].AddParagraph("0-30");
            row2.Cells[2].AddParagraph("31-60");
            row2.Cells[3].AddParagraph("61-90");
            row2.Cells[4].AddParagraph("90+");

            paragraph = row2.Cells[5].AddParagraph();
            paragraph.AddFormattedText("Balance", TextFormat.Bold);

            row2 = table.AddRow();
            row2.VerticalAlignment = VerticalAlignment.Center;
            row2.Cells[1].AddParagraph(sum30.ToString("c"));
            row2.Cells[2].AddParagraph(sum60.ToString("c"));
            row2.Cells[3].AddParagraph(sum90.ToString("c"));
            row2.Cells[4].AddParagraph(sumOver90.ToString("c"));
            paragraph = row2.Cells[5].AddParagraph();
            paragraph.AddFormattedText((sum30+sum60+sum90+sumOver90).ToString("c"), TextFormat.Bold);

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