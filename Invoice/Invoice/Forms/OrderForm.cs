using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using System.Diagnostics;
using System.Collections.Generic;

namespace InvoiceX.Forms
{
    /// <summary>
    /// Creates the quote form.
    /// </summary>
    public class OrderForm
    {
        /// <summary>
        /// The MigraDoc document that represents the quote.
        /// </summary>
        Document document;

        /// <summary>
        /// An XML quote based on a sample created with Microsoft InfoPath.
        /// </summary>
        readonly XmlDocument order;

        /// <summary>
        /// The root navigator for the XML document.
        /// </summary>
        readonly XPathNavigator navigator;

        /// <summary>
        /// The text frame of the MigraDoc document that contains the address.
        /// </summary>
        TextFrame addressFrame;
        TextFrame orderDetailsFrame;
        TextFrame customerDetails;
        /// <summary>
        /// The table of the MigraDoc document that contains the quote items.
        /// </summary>
        Table table;

        /// <summary>
        /// Initializes a new instance of the class BillFrom and opens the specified XML document.
        /// </summary>
        string[] customer_details = new string[2];
        string[] order_details = new string[6];
        List<Models.Product> products = new List<Models.Product>();

        public OrderForm(string filename, string[] cd, string[] id, List<Models.Product> p)
        {
            this.order = new XmlDocument();
            this.order.Load(filename);
            this.navigator = this.order.CreateNavigator();
            this.customer_details = cd;
            this.order_details = id;
            this.products = p;
        }

        /// <summary>
        /// Creates the quote document.
        /// </summary>
        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "Order";
            this.document.Info.Author = "Eco-Bright";

            DefineStyles();

            CreatePage();

            FillContent();

            return this.document;
        }

        /// <summary>
        /// Defines the styles used to format the MigraDoc document.
        /// </summary>
    
        void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = this.document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("6cm", TabAlignment.Right);

            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        /// <summary>
        /// Creates the static parts of the quote.
        /// </summary>
        void CreatePage()
        {
            // Each MigraDoc document needs at least one section.
            Section section = this.document.AddSection();

            // Put a logo in the header
            Image image = section.Headers.Primary.AddImage("../../Images/logo-2.png");
            image.Height = "1.5cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;


            Paragraph par = section.Headers.Primary.AddParagraph();
            par.Format.Alignment = ParagraphAlignment.Right;
            par.Format.Font.Size = 24;
            par.Format.Font.Color = LogoBlue;
            par.Format.SpaceBefore = 5;
            par.AddFormattedText("Order", TextFormat.Bold);

            // Create footer

            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.Format.SpaceBefore = 5;
            paragraph.AddText("If you have any questions concerning this quotation, please contact us. Thank you for your business!");
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.AddLineBreak();

            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("PowerBooks Inc · Sample Street 42 · 56789 Cologne · Germany");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            // Create the text frame for the address
            this.addressFrame = section.AddTextFrame();
            this.addressFrame.Height = "3.0cm";
            this.addressFrame.Width = "7.0cm";
            this.addressFrame.Left = ShapePosition.Left;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.Top = "3cm";
            this.addressFrame.RelativeVertical = RelativeVertical.Page;

            //create the quote detail frame

            this.orderDetailsFrame = section.AddTextFrame();
            this.orderDetailsFrame.Height = "3.0cm";
            this.orderDetailsFrame.Width = "7.0cm";
            this.orderDetailsFrame.Left = ShapePosition.Right;
            this.orderDetailsFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.orderDetailsFrame.Top = "3cm";
            this.orderDetailsFrame.RelativeVertical = RelativeVertical.Page;
            
            //create the customer detail frame

            this.customerDetails = section.AddTextFrame();
            this.customerDetails.Height = "3.0cm";
            this.customerDetails.Width = "7.0cm";
            this.customerDetails.Left = ShapePosition.Left;
            this.customerDetails.RelativeHorizontal = RelativeHorizontal.Margin;
            this.customerDetails.Top = "3cm";
            this.customerDetails.RelativeVertical = RelativeVertical.Page;


            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "6cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("Comments or Special Instructions: ", TextFormat.Italic);
            paragraph.Format.Font.Color = LogoBlue;


            // Create the item table
            this.table = section.AddTable();
            this.table.Style = "Table";
            this.table.Borders.Color = TableBorder;
            this.table.Borders.Width = 0.25;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;



            // Before you can add a row, you must define the columns
            Column column = this.table.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("8.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = this.table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = this.table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;



            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = LogoBlue;
            row.Format.Font.Color = new Color(255, 255, 255);
            row.TopPadding = 3;
            row.BottomPadding = 3;
            row.Format.Font.Size = 10;

            row.Cells[0].AddParagraph("CODE");
            row.Cells[0].Format.Font.Bold = true;
            row.Format.LineSpacing = 10;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph("ITEM");
            row.Cells[1].Format.Font.Bold = true;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[2].AddParagraph("DESCRIPTION");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[3].AddParagraph("PRICE");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            this.table.SetEdge(0, 0, 3, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            

        }

        /// <summary>
        /// Creates the dynamic parts of the quote.
        /// </summary>
        void FillContent()
        {
            
            // Fill address in address text frame
            XPathNavigator item = SelectItem("/quote/to");
            Paragraph paragraph = this.addressFrame.AddParagraph();
            paragraph.AddText("Andreas Panteli Trading LTD");
            paragraph.AddLineBreak();
            paragraph.AddText("Neas Ionias 18, Aradippou, Larnaca");
            paragraph.AddLineBreak();
            paragraph.AddText("+357 24433172, +357 99560755");
            paragraph.AddLineBreak();
            paragraph.AddText("info@ecobrightcy.com");
            paragraph.AddLineBreak();
            paragraph.AddText("www.ecobrightcy.com");
            

            paragraph = this.orderDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.SpaceBefore = 48;
            paragraph.Format.Shading.Color = LogoBlue;
            paragraph.Format.Font.Color=new Color(255, 255, 255);
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Size = 11;
            paragraph.AddText("ORDER #");
            paragraph.AddSpace(16);
            paragraph.AddText("DATE");
            
            paragraph = this.orderDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            String quoteNumber = this.order_details[0];
            String date = this.order_details[1];
            paragraph.AddText(quoteNumber);
            paragraph.AddSpace(24);
            paragraph.AddText(date);

            paragraph = this.addressFrame.AddParagraph();
            paragraph.AddLineBreak();

            paragraph = this.customerDetails.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.SpaceBefore = 80;
            paragraph.Format.Shading.Color = LogoBlue;
            paragraph.Format.Font.Color = new Color(255, 255, 255);
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Size = 11;
            paragraph.AddText("CUSTOMER INFO");

            paragraph = this.orderDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.SpaceBefore = 7;
            paragraph.Format.Shading.Color = LogoBlue;
            paragraph.Format.Font.Color = new Color(255, 255, 255);
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Size = 11;
            paragraph.AddText("CUSTOMER ID");
            paragraph.AddSpace(2);
            paragraph.AddText("SHIPPING DATE");

            paragraph = this.orderDetailsFrame.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText(customer_details[0]);
            paragraph.AddTab(); 
            paragraph.AddTab();
            paragraph.AddText(order_details[2]);


            paragraph = this.addressFrame.AddParagraph();
            paragraph.Format.SpaceBefore = 20;
            paragraph.AddText(this.customer_details[1]);
            paragraph.AddLineBreak();

            // Iterate the invoice items
            
            for (int i=0; i<products.Count;i++)
            {
                double quantity = this.products[i].Quantity;
                double price = GetValueAsDouble(item, "price");
                double discount = GetValueAsDouble(item, "discount");

                // Each item fills two rows
                Row row1 = this.table.AddRow();
                row1.TopPadding = 1.5;
                row1.Cells[0].Shading.Color = TableGray;
                row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[3].Shading.Color = TableGray;

                row1.Cells[0].AddParagraph(this.products[i].Quantity.ToString());
                paragraph = row1.Cells[1].AddParagraph();
                paragraph.AddFormattedText(this.products[i].ProductName, TextFormat.Bold);
                row1.Cells[2].AddParagraph(this.products[i].SellPrice.ToString());
                row1.Cells[2].AddParagraph();
                row1.Cells[3].AddParagraph(this.products[i].Total.ToString());
                row1.Cells[3].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[2].VerticalAlignment = VerticalAlignment.Center;

                this.table.SetEdge(0, this.table.Rows.Count - 2, 4, 2, Edge.Box, BorderStyle.Single, 0.75);
            }
            
            // Add an invisible row as a space line to the table
            Row row = this.table.AddRow();
            row.Borders.Visible = false;


        }

        /// <summary>
        /// Selects a subtree in the XML data.
        /// </summary>
        XPathNavigator SelectItem(string path)
        {
            XPathNodeIterator iter = this.navigator.Select(path);
            iter.MoveNext();
            return iter.Current;
        }

        /// <summary>
        /// Gets an element value from the XML data.
        /// </summary>
        static string GetValue(XPathNavigator nav, string name)
        {
            //nav = nav.Clone();
            XPathNodeIterator iter = nav.Select(name);
            iter.MoveNext();
            return iter.Current.Value;
        }

        /// <summary>
        /// Gets an element value as double from the XML data.
        /// </summary>
        static double GetValueAsDouble(XPathNavigator nav, string name)
        {
            try
            {
                string value = GetValue(nav, name);
                if (value.Length == 0)
                    return 0;
                return Double.Parse(value, CultureInfo.InvariantCulture);
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
        readonly static Color TableBorder = new Color(81, 125, 192);
        readonly static Color TableBlue = new Color(235, 240, 249);
        readonly static Color TableGray = new Color(242, 242, 242);
        readonly static Color LogoBlue = new Color(38,34,98);
#else
    // CMYK colors
    readonly static Color tableBorder = Color.FromCmyk(100, 50, 0, 30);
    readonly static Color tableBlue = Color.FromCmyk(0, 80, 50, 30);
    readonly static Color tableGray = Color.FromCmyk(30, 0, 0, 0, 100);
#endif
    }
}
