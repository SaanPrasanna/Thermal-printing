using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CustomJson
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private static RootObject ReportData;
        private static int height;
        private static int itemIndex = 0;
        private static int pageNum = 1;
        private static bool printPage = false;
        private static List<PageSizeSettings> PageSizeSettingsList;

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            List<Item> totalItems = new List<Item>();
            //List<Item> totalItems = new List<Item> { new Item { No = 1, ItemAmt = "350.00", ItemName = "තේ කොල 1Kg", Qty = 1, Rate = "" }, new Item { No = 2, ItemAmt = "200.00", ItemName = "මැගී නූඩ්ල්ස් චිකන්", Qty = 4, Rate = "" } };
            try
            {
                String sql = "SELECT * FROM orders";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adupter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                table.Load(cmd.ExecuteReader());

                //Inserting data to DataTable as array
                var rows = table.AsEnumerable().ToArray();
                adupter.Fill(table);
                
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var item = rows[i];
                    //Console.WriteLine(item["itemName"]);
                    totalItems.Add( new Item { No = (int)item["id"], ItemAmt = item["itemAmount"].ToString(), ItemName = item["itemName"].ToString(), Qty = (int)item["qty"], Rate = item["rate"].ToString() });
                }
                Console.WriteLine(totalItems.Count);

                RootObject json = new RootObject
                {
                    Total = "1150.00",
                    GrandTotal = "1150.00",
                    CustomerInfo = new CustomerInfo { Address1 = "Address 1", Address2 = "Address 2", Phone = "01234456789" },
                    BillSummary = new List<BillSummary> { new BillSummary { key = "Shopping", value = "10.00" } },
                    Header = new Header { MainName = "Demo Name", Phone = "0123456789", Address = "මීගමු පාර‍,කුරුණැගල‍", City = "කුරුණෑගල", BillNo = "INVO0001", DateOfBill = "2020-02-20", FssaiNo = "12345678901234", GSTNo = "12345678901234", CustomerRemarksLine1 = "", OrderNote = "" },
                    Items = totalItems,
                    //Canon TS200 series
                    Settings = new Settings { PrinterName = "Microsoft XPS Document Writer", PrinterType = "Default", ItemLength = 40, PrintLogo = true, ThankYouNote = "Thank you for choosing to order from us", EIDRMK = "Thank you", PrintType = "Cash" }
                };

                Console.Read();
                String jsonString = JsonConvert.SerializeObject(json, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented });
                System.IO.File.WriteAllText(@"temp.txt", jsonString);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex, "Error Title", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            Console.Read();

            //items
            
            //foreach(var item in )
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(Environment.ExpandEnvironmentVariables("%allusersprofile%") + "/DefaultPrinter/PageConfiguration.config"))
                {
                    PageSizeSettingsList = new List<PageSizeSettings>();
                    PageSizeSettingsList.Add(new PageSizeSettings());
                    Console.WriteLine("info" + Environment.ExpandEnvironmentVariables("%allusersprofile%"));
                    Console.Read();
                    File.WriteAllText(Environment.ExpandEnvironmentVariables("%allusersprofile%") + "/DefaultPrinter/PageConfiguration.config", JsonConvert.SerializeObject(PageSizeSettingsList));
                    FileSecurity(Environment.ExpandEnvironmentVariables("%allusersprofile%") + "/DefaultPrinter/PageConfiguration.config");
                }
                else
                {
                    PageSizeSettingsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PageSizeSettings>>(File.ReadAllText(Environment.ExpandEnvironmentVariables("%allusersprofile%") + "/DefaultPrinter/PageConfiguration.config"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.Read();
            }
        }
        public void FileSecurity(string FileName)
        {
            FileSecurity fSecurity = File.GetAccessControl(FileName);
            fSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            File.SetAccessControl(FileName, fSecurity);
        }

        private void CashReceipt()
        {
            itemIndex = 0;
            pageNum = 1;
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrinterSettings.PrinterName = ReportData.Settings.PrinterName;
            printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(CreateReceipt);

            printDocument.Print();
        }

        public static void CreateReceipt(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //this prints the reciept
            var pageSetting = PageSizeSettingsList.Where(i => i.PageSize == ReportData.Settings.PageSize).FirstOrDefault();

            Graphics graphic = e.Graphics;

            Font font = new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize); //must use a mono spaced font as the spaces need to line up

            float fontHeight = font.GetHeight();
            height = e.PageSettings.PaperSize.Height - 12;

            int startX = pageSetting.PageSetting.XPoint;
            int startY = pageSetting.PageSetting.YPoint;
            int offset = pageSetting.PageSetting.Offset;
            startY = 10;
            Rectangle rect;
                Console.WriteLine("Image Info" + Environment.ExpandEnvironmentVariables("%allusersprofile%") + "/DefaultPrinter/logo.png");
            if (File.Exists(Environment.ExpandEnvironmentVariables("%allusersprofile%") + "/DefaultPrinter/logo.png"))
            {
                //rect = new Rectangle(5, startY, pageSetting.PageSetting.LogoWidth, pageSetting.PageSetting.LogoHeight);
                var image = Image.FromFile(Environment.ExpandEnvironmentVariables("%allusersprofile%") + "/DefaultPrinter/logo.png");
                graphic.DrawImage(image, pageSetting.PageSetting.LogoXPoint, startY, pageSetting.PageSetting.LogoWidth, pageSetting.PageSetting.LogoHeight);

                offset = offset + pageSetting.PageSetting.LogoHeight;

            }

            rect = new Rectangle(5, startY + offset, pageSetting.PageSetting.RecWidth, 20);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;

            if (pageNum == 1)
            {
                //Rectangle rect = new Rectangle(5, startY,   POS_PrintUsingPrintDocument.Properties.Settings.Default.ItemWidth, 20);
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                graphic.DrawString(ReportData.Header.MainName, new Font(pageSetting.PageSetting.HeaderCashFontName, pageSetting.PageSetting.HeaderCashFontSize, FontStyle.Bold), Brushes.Black, rect, sf);
                offset = offset + (int)fontHeight + 5;
                if (ReportData.Header.Address != null)
                {
                    rect.Y = startY + offset;
                    graphic.DrawString(ReportData.Header.Address, new Font(pageSetting.PageSetting.SubHeaderFontName, pageSetting.PageSetting.SubHeaderFontSize), new SolidBrush(Color.Black), rect, format);
                    offset = offset + (int)fontHeight + 5;
                }
                if (ReportData.Header.Address1 != null)
                {
                    rect.Y = startY + offset;
                    graphic.DrawString(ReportData.Header.Address1, new Font(pageSetting.PageSetting.SubHeaderFontName, pageSetting.PageSetting.SubHeaderFontSize), new SolidBrush(Color.Black), rect, format);
                    offset = offset + (int)fontHeight + 5;
                }
                if (ReportData.Header.Address2 != null)
                {
                    rect.Y = startY + offset;
                    graphic.DrawString(ReportData.Header.Address2, new Font(pageSetting.PageSetting.SubHeaderFontName, pageSetting.PageSetting.SubHeaderFontSize), new SolidBrush(Color.Black), rect, format);
                    offset = offset + (int)fontHeight + 5;
                }
                if (ReportData.Header.Address3 != null)
                {
                    rect.Y = startY + offset;
                    graphic.DrawString(ReportData.Header.Address2, new Font(pageSetting.PageSetting.SubHeaderFontName, pageSetting.PageSetting.SubHeaderFontSize), new SolidBrush(Color.Black), rect, format);
                    offset = offset + (int)fontHeight + 5;
                }
                if (ReportData.Header.City != null)
                {
                    rect.Y = startY + offset;
                    graphic.DrawString(ReportData.Header.City, new Font(pageSetting.PageSetting.SubHeaderFontName, pageSetting.PageSetting.SubHeaderFontSize), new SolidBrush(Color.Black), rect, format);
                    offset = offset + (int)fontHeight + 5;
                }
                if (ReportData.Header.Phone != null)
                {
                    rect.Y = startY + offset;
                    graphic.DrawString(ReportData.Header.Phone, new Font(pageSetting.PageSetting.SubHeaderFontName, pageSetting.PageSetting.SubHeaderFontSize), new SolidBrush(Color.Black), rect, format);
                    offset = offset + (int)fontHeight + 5;
                }
                if (ReportData.Header.GSTNo != null)
                {
                    rect.Y = startY + offset;
                    graphic.DrawString(ReportData.Header.GSTNo, new Font(pageSetting.PageSetting.SubHeaderFontName, pageSetting.PageSetting.SubHeaderFontSize), new SolidBrush(Color.Black), rect, format);
                    offset = offset + (int)fontHeight + 5;
                }
                if (ReportData.Header.FssaiNo != null)
                {
                    rect.Y = startY + offset;
                    graphic.DrawString(ReportData.Header.FssaiNo, new Font(pageSetting.PageSetting.SubHeaderFontName, pageSetting.PageSetting.SubHeaderFontSize), new SolidBrush(Color.Black), rect, format);
                    offset = offset + (int)fontHeight + 5;
                }
                graphic.DrawString("-------------------------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                Font fontBoldnBig = new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.BillNoRowFontSize, FontStyle.Bold); //must use a mono spaced font as the spaces need to line up

                if (ReportData.CustomerInfo != null)
                {
                    graphic.DrawString(string.Format("{0,-" + (ReportData.Settings.ItemLength + 6 + 9) + "}", "Customer:"), new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize), new SolidBrush(Color.Black), startX, startY + offset);
                    graphic.DrawString(string.Format("{0,-" + "26".ToString() + "}", "Bill No: " + ReportData.Header.BillNo), new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize), new SolidBrush(Color.Black), startX + 180, startY + offset);
                    offset = offset + (int)fontHeight + 3;
                }
                else
                {

                    rect = new Rectangle(startX + pageSetting.PageSetting.QuantityPoint - 10, startY + offset, 140, 20);
                    StringFormat sfright = new StringFormat();
                    sfright.LineAlignment = StringAlignment.Far;
                    sfright.Alignment = StringAlignment.Far;

                    graphic.DrawString(string.Format("{0,-" + (ReportData.Settings.ItemLength + 6 + 9) + "}\n", "Bill No: " + ReportData.Header.BillNo), fontBoldnBig, new SolidBrush(Color.Black), startX, startY + offset);
                    graphic.DrawString(string.Format("{0,9:N2}\n", "Date: " + ReportData.Header.DateOfBill), fontBoldnBig, new SolidBrush(Color.Black), rect, sfright);
                    offset = offset + (int)fontHeight + 5;
                }



                if (ReportData.CustomerInfo != null)
                {
                    graphic.DrawString(string.Format("{0,-" + (ReportData.Settings.ItemLength + 6 + 9) + "}", ReportData.CustomerInfo.Address1), new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize), new SolidBrush(Color.Black), startX, startY + offset);
                    graphic.DrawString(string.Format("{0,-" + "26".ToString() + "}", "Date: " + ReportData.Header.DateOfBill), new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize), new SolidBrush(Color.Black), startX + 180, startY + offset);
                    offset = offset + (int)fontHeight + 3;
                }



                if (ReportData.CustomerInfo != null)
                {
                    graphic.DrawString(string.Format("{0,-" + (ReportData.Settings.ItemLength + 6 + 9) + "}", ReportData.CustomerInfo.Address2), new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize), new SolidBrush(Color.Black), startX, startY + offset);
                    graphic.DrawString(string.Format("{0,-" + "26".ToString() + "}", "Time: " + ReportData.Header.TimeOfBill), new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize), new SolidBrush(Color.Black), startX + 180, startY + offset);
                    offset = offset + (int)fontHeight + 3;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ReportData.Header.TimeOfBill))
                    {
                        graphic.DrawString(string.Format("{0,-" + (ReportData.Settings.ItemLength + 6 + 9) + "}", "Time: " + ReportData.Header.TimeOfBill), new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize), new SolidBrush(Color.Black), startX, startY + offset);
                        offset = offset + (int)fontHeight + 3;
                    }
                }

                if (ReportData.CustomerInfo != null)
                {
                    graphic.DrawString(string.Format("{0,-" + (ReportData.Settings.ItemLength + 6 + 9) + "}", ReportData.CustomerInfo.Address3), new Font("Segoe UI", (float)8.5), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 3;
                }

                if (ReportData.CustomerInfo != null)
                {
                    graphic.DrawString(string.Format("{0,-" + (ReportData.Settings.ItemLength + 6 + 9) + "}", "Phone: " + ReportData.CustomerInfo.Phone), new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 3;
                }

                graphic.DrawString("-------------------------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                rect = new Rectangle(pageSetting.PageSetting.XPoint, startY + offset, pageSetting.PageSetting.RecWidth, 20);
                sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Near;
                startY = 10;

                Font fontBold = new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize, FontStyle.Bold); //must use a mono spaced font as the spaces need to line up

                graphic.DrawString(string.Format("{0,-" + "26".ToString() + "}", "Menu Item"), fontBold, Brushes.Black, rect, sf);
                sf.Alignment = StringAlignment.Far;
                rect = new Rectangle(startX + pageSetting.PageSetting.QuantityPoint, startY + offset, 35, 20);
                graphic.DrawString(string.Format("{0,6}", "Qty"), fontBold, Brushes.Black, rect, sf);
                rect = new Rectangle(startX + pageSetting.PageSetting.PricePoint, startY + offset, 60, 20);
                graphic.DrawString(string.Format("{0,9}", "Rate"), fontBold, Brushes.Black, rect, sf);
                rect = new Rectangle(startX + pageSetting.PageSetting.AmountPoint, startY + offset, 90, 20);
                graphic.DrawString(string.Format("{0,9}", "Amount"), fontBold, Brushes.Black, rect, sf);

                offset = offset + (int)fontHeight + 5;
                pageNum = pageNum + 1;
            }
            graphic.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            while (itemIndex < ReportData.Items.Count && startY + offset <= height)
            {
                Item item = ReportData.Items[itemIndex];
                offset = ItemFormat(item, graphic, font, fontHeight, startX, startY, offset, pageSetting);
                itemIndex = itemIndex + 1;
            }

            if (itemIndex == ReportData.Items.Count && startY + offset <= height)
            {
                graphic.DrawString("-------------------------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                Font fontBoldnBig = new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.BillNoRowFontSize, FontStyle.Bold); //must use a mono spaced font as the spaces need to line up

                rect = new Rectangle(startX + pageSetting.PageSetting.TotalPoint, startY + offset, pageSetting.PageSetting.WidthSummary, 20);
                StringFormat sfright = new StringFormat();
                sfright.LineAlignment = StringAlignment.Far;
                sfright.Alignment = StringAlignment.Far;

                graphic.DrawString(string.Format("{0,-" + (ReportData.Settings.ItemLength + 6 + 9) + "}\n", "Total"), fontBoldnBig, new SolidBrush(Color.Black), startX, startY + offset);
                graphic.DrawString(string.Format("{0,9:N2}\n", ReportData.Total), fontBoldnBig, new SolidBrush(Color.Black), rect, sfright);
                offset = offset + (int)fontHeight + 5;

                graphic.DrawString("-------------------------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                Font fontBold = new Font(pageSetting.PageSetting.CashItemFontName, pageSetting.PageSetting.CashItemFontSize, FontStyle.Bold); //must use a mono spaced font as the spaces need to line up

                var sfSummary = new StringFormat();
                sfSummary.LineAlignment = StringAlignment.Center;
                sfSummary.Alignment = StringAlignment.Near;
                sfSummary.Alignment = StringAlignment.Far;
                foreach (var summary in ReportData.BillSummary)
                {
                    graphic.DrawString(string.Format("{0,-" + "26".ToString() + "}", summary.key), fontBold, new SolidBrush(Color.Black), startX, startY + offset);
                    rect = new Rectangle(startX + pageSetting.PageSetting.AmountPoint, startY + offset, 90, 20);
                    graphic.DrawString(string.Format("{0,9:N2}", summary.value), fontBold, new SolidBrush(Color.Black), rect, sfSummary);
                    offset = offset + (int)fontHeight + 5;
                }

                if (ReportData.BillSummary != null && ReportData.BillSummary.Count > 0)
                {
                    graphic.DrawString("-------------------------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;
                }

                rect = new Rectangle(startX + pageSetting.PageSetting.TotalPoint, startY + offset, pageSetting.PageSetting.WidthSummary, 20);

                graphic.DrawString(string.Format("{0,-" + (ReportData.Settings.ItemLength + 6 + 9) + "}\n", "Grand Total"), fontBoldnBig, new SolidBrush(Color.Black), startX, startY + offset);
                graphic.DrawString(string.Format("{0,9:N2}\n", ReportData.GrandTotal), fontBoldnBig, new SolidBrush(Color.Black), rect, sfright);
                offset = offset + (int)fontHeight + 5;

                graphic.DrawString("-------------------------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                rect.X = startX;
                rect.Width = pageSetting.PageSetting.AmountPoint + 90;
                rect.Y = startY + offset;

                graphic.DrawString(ReportData.Settings.ThankYouNote, new Font(pageSetting.PageSetting.CashItemFontName, 10), new SolidBrush(Color.Black), rect, format);
                offset = offset + (int)fontHeight + 5;
                if (!string.IsNullOrEmpty(ReportData.Settings.ThankYouNote2))
                {
                    rect.Y = startY + offset;
                    graphic.DrawString(ReportData.Settings.ThankYouNote2, new Font(pageSetting.PageSetting.CashItemFontName, 10), new SolidBrush(Color.Black), rect, format);
                    offset = offset + (int)fontHeight + 5;
                }
                rect.Height = 35;
                rect.Y = startY + offset;
                offset = offset + (int)fontHeight + 5;
                graphic.DrawString(ReportData.Settings.ThankYouNote, new Font("Bauhaus 93", 14), new SolidBrush(Color.Black), rect, format);
                rect.Y = startY + offset;
                offset = offset + (int)fontHeight + 5;
                graphic.DrawString(ReportData.Settings.ThankYouNote, new Font("Bodoni MT Poster Compressed", 14), new SolidBrush(Color.Black), rect, format);
                rect.Y = startY + offset;
                offset = offset + (int)fontHeight + 5;
                graphic.DrawString(ReportData.Settings.ThankYouNote, new Font("Brush Script MT", 14), new SolidBrush(Color.Black), rect, format);
                rect.Y = startY + offset;
                offset = offset + (int)fontHeight + 5;
                rect.Height = 50;
                graphic.DrawString(ReportData.Settings.ThankYouNote, new Font("test", 14), new SolidBrush(Color.Black), rect, format);
                offset = offset + (int)fontHeight + 5;
                printPage = true;
            }

            if (startY + offset < height || printPage == true)
            {
                e.HasMorePages = false;
            }
            else
            {
                e.HasMorePages = true;
            }
        }
        public static int ItemFormat(Item item, Graphics graphic, Font font, float fontHeight, int startX, int startY, int offset, PageSizeSettings pageSetting)
        {
            StringFormat sFormat = new StringFormat(StringFormat.GenericTypographic);
            sFormat.Trimming = StringTrimming.None;
            sFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
            var outStr = getStrSplit(item.ItemName, pageSetting.PageSetting.ItemLength);
            var outlst = outStr.Split('\n').ToList();

            Rectangle rect = new Rectangle(pageSetting.PageSetting.XPoint, startY + offset, pageSetting.PageSetting.RecWidth, 20);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Near;
            startY = 10;
            graphic.DrawString(string.Format("{0,-" + pageSetting.PageSetting.ItemLength.ToString() + "}", outlst[0]), font, Brushes.Black, rect, sf);
            sf.Alignment = StringAlignment.Far;
            rect = new Rectangle(startX + pageSetting.PageSetting.QuantityPoint, startY + offset, 30, 20);
            graphic.DrawString(string.Format("{0,6}", item.Qty), font, Brushes.Black, rect, sf);
            rect = new Rectangle(startX + pageSetting.PageSetting.PricePoint, startY + offset, 60, 20);
            graphic.DrawString(string.Format("{0,9}", item.Rate), font, Brushes.Black, rect, sf);
            rect = new Rectangle(startX + pageSetting.PageSetting.AmountPoint, startY + offset, 90, 20);
            graphic.DrawString(string.Format("{0,9}", item.ItemAmt), font, Brushes.Black, rect, sf);

            offset = offset + (int)fontHeight + 5;
            sf.Alignment = StringAlignment.Near;
            if (outlst.Count > 1)
            {
                int skip = 1;
                foreach (var i in outlst.Skip(skip))
                {
                    rect = new Rectangle(pageSetting.PageSetting.XPoint, startY + offset, pageSetting.PageSetting.RecWidth, 20);

                    graphic.DrawString(string.Format("{0,-" + "50".ToString() + "}", i), font, Brushes.Black, rect, sf);
                    offset = offset + (int)fontHeight + 5;
                    skip = skip + 1;
                }
            }

            if (!string.IsNullOrEmpty(item.CustRmks))
            {
                var outStrRmk = getStrSplit("Customer Remark: " + item.CustRmks, pageSetting.PageSetting.KitchenItemLength);
                var outlstRmk = outStrRmk.Split('\n').ToList();

                rect = new Rectangle(pageSetting.PageSetting.XPoint, startY + offset, pageSetting.PageSetting.RecWidth, 20);
                graphic.DrawString(string.Format("{0,-" + pageSetting.PageSetting.KitchenItemLength.ToString() + "}", outlstRmk[0]), font, Brushes.Black, rect, sf);
                offset = offset + (int)fontHeight + 5;


                if (outlstRmk.Count > 1)
                {
                    int skip = 1;

                    foreach (var i in outlstRmk.Skip(skip))
                    {
                        rect = new Rectangle(pageSetting.PageSetting.XPoint, startY + offset, pageSetting.PageSetting.RecWidth, 20);

                        graphic.DrawString(string.Format("{0,-" + pageSetting.PageSetting.KitchenItemLength.ToString() + "}", i), font, Brushes.Black, rect, sf);
                        offset = offset + (int)fontHeight + 5;
                        skip = skip + 1;
                    }
                }
            }


            return offset;
        }


        //This function split item name into multiple lines depending upon length parameter
        public static string getStrSplit(string x, int length)
        {
            string result = string.Empty;
            if (x.Length <= length)
            {
                return x;
            }
            else
            {
                var sub = x.Substring(0, length);
                result += sub + "\n" + getStrSplit(x.Replace(sub, ""), length);
            }

            return result;
        }

        private void CmdMysql_Click(object sender, EventArgs e)
        {
            try
            {
                string json = File.ReadAllText("temp.json");
                ReportData = JsonConvert.DeserializeObject<RootObject>(json);
                CashReceipt();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : "+ ex.ToString());
            }
            finally
            {
                Console.Read();
            }
        }
    }

}
