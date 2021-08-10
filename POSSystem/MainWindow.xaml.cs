using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using System.Windows.Media.Effects;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Drawing.Printing;
using Color = System.Drawing.Color;
using System.Windows.Data;
using System.Configuration;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Reflection;
using System.Linq;
using System.Security.Permissions;

namespace POSSystem
{
    public partial class MainWindow : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        private PrintDocument PrintDocument;
        private Graphics graphics;
        string tenderCode = "";
        DataTable dt = new DataTable();
        DataTable dtdep = new DataTable();
        string username = App.Current.Properties["username"].ToString();
        string date = DateTime.Now.ToString("yyyy/MM/dd").Replace("-", "/");
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "MainWindow.cs";

        string txtGotFocusStr = string.Empty;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                lblDate.Content = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                lblusername.Content = username;
                TextBox tb = new TextBox();
                tb.KeyDown += new KeyEventHandler(OnKeyDownHandler);
                tb.KeyDown += new KeyEventHandler(TxtCashReceive_KeyDown);
                SqlConnection con = new SqlConnection(conString);
                string query = "select Scancode,description,unitretail,TaxRate from item where Scancode=@password ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@password", textBox1.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);

                //con.Open();
                sda.Fill(dt);
                dt.Columns.Add("quantity");
                dt.Columns.Add("Amount");
                dt.Columns.Add("Date");
                dt.Columns.Add("Time");
                dt.Columns.Add("TransactionId");
                dt.Columns.Add("CreateBy");
                dt.Columns.Add("CreateOn");
                dt.Columns.Add("PromotionName");
                //con.Close();
                textBox1.Focus();

                string queryS = "Select Department,TaxRate,FilePath from Department";
                SqlCommand cmd1 = new SqlCommand(queryS, con);
                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                sda1.Fill(dtdep);

                for (int i = 0; i < dtdep.Rows.Count; ++i)
                {
                    Button button = new Button();
                    button.Content = new TextBlock()
                    {
                        FontSize = 25,
                        Text = dtdep.Rows[i].ItemArray[0].ToString(),
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    };
                    if (dtdep.Rows[i].ItemArray[2].ToString() != "")
                    {
                        var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                        var path = dtdep.Rows[i].ItemArray[2].ToString();
                        var fullpath = Path + "\\Image\\" + path;
                        button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                    }
                    button.Foreground = new SolidColorBrush(Colors.White);
                    button.FontSize = 26;
                    button.FontWeight = FontWeights.Bold;
                    button.Margin = new Thickness(5, 5, 5, 5);
                    string abc = dtdep.Rows[i].ItemArray[1].ToString();
                    button.Click += (sender, e) => { button_Click(sender, e, abc); };
                    this.sp21.Children.Add(button);
                }

                //Customer Dropdown.
                string queryCustomer = "select Name from Account where Head='Customers'";
                SqlCommand cmdcustomer = new SqlCommand(queryCustomer, con);
                SqlDataAdapter sdacustomer = new SqlDataAdapter(cmdcustomer);
                DataTable dtAcc = new DataTable();
                sdacustomer.Fill(dtAcc);
                cbcustomer.ItemsSource = dtAcc.DefaultView;
                cbcustomer.DisplayMemberPath = "Name";

                loadtransactionId();

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void loadtransactionId()
        {
            using (SqlConnection conn = new SqlConnection(conString))
            {
                conn.Open();
                string query1 = "SELECT TOP 1 Tran_id FROM Transactions where DayClose is null ORDER BY Tran_id DESC";
                using (SqlCommand cmd2 = new SqlCommand(query1, conn))
                {
                    SqlDataReader data = cmd2.ExecuteReader();
                    if (data.Read())
                    {
                        lblTranid.Content = Convert.ToInt32(data.GetValue(0).ToString()) + 1;
                    }
                    else
                    {
                        lblTranid.Content = 1;
                    }
                }
                conn.Close();
            }
        }
        string taxrate = "";
        void button_Click(object sender, RoutedEventArgs e, string abc)
        {
            try
            {
                var btnContent = sender as Button;
                var tb = (TextBlock)btnContent.Content;
                taxrate = abc;
                lblDepartment.Content = tb.Text;
                TxtBxStackPanel2.Visibility = Visibility.Visible;
                sp21.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void Button_Click_Go_Back(object sender, RoutedEventArgs e)
        {
            try
            {
                sp21.Visibility = Visibility.Visible;
                TxtBxStackPanel2.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void Button_Click_Sale_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = lblDepartment.Content.ToString();
                dr[2] = Convert.ToDecimal(txtDeptAmt.Text).ToString("0.00");
                dr[3] = taxrate;
                dr[4] = 1;
                dr[5] = (decimal.Parse(txtDeptAmt.Text) * 1).ToString("0.00");
                dt.Rows.Add(dr);
                JRDGrid.ItemsSource = dt.DefaultView;
                JRDGrid.Items.Refresh();
                TotalEvent();
                txtDeptAmt.Text = "";
                sp21.Visibility = Visibility.Visible;
                TxtBxStackPanel2.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {

                SendErrorToText(ex, errorFileName);
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);

                if (e.Key == Key.Enter || e.Key == Key.Tab)
                {
                    string query = "select Scancode,Description,UnitRetail,@qty as quantity,UnitRetail as Amount,Department.TaxRate,UnitRetail as Oprice from Item inner join Department on item.Department=Department.Department where Scancode=@password ";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@password", textBox1.Text);
                    cmd.Parameters.AddWithValue("@qty", 1);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    con.Open();
                    sda.Fill(dt);
                    con.Close();

                    DataTable dt1 = new DataTable();
                    foreach (DataRow row in dt.AsEnumerable())
                    {

                        string _scancode = row["ScanCode"].ToString();
                        string query1 = "select promotiongroup.ScanCode, promotiongroup.PromotionName,promotion.NewPrice,promotion.Quantity,promotion.PriceReduce  from promotiongroup inner join promotion on promotiongroup.promotionname =  promotion.promotionname  where promotiongroup.ScanCode = @_scancode and @datetime between promotion.StartDate and promotion.EndDate";
                        SqlCommand cmd1 = new SqlCommand(query1, con);
                        cmd1.Parameters.AddWithValue("@_scancode", _scancode);
                        cmd1.Parameters.AddWithValue("@datetime", date);
                        SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                        con.Open();
                        sda1.Fill(dt1);
                        con.Close();
                    }
                    DataTable distrinctPromotionName = dt1.DefaultView.ToTable(true, "PromotionName");
                    DataTable distrinctScanCode = dt1.DefaultView.ToTable(true, "ScanCode", "PromotionName");
                    int distCount = distrinctPromotionName.AsEnumerable().Count();
                    foreach (DataRow distrinctRow in distrinctPromotionName.AsEnumerable())
                    {
                        int sumCount = 0;
                        for (int j = 0; j < distrinctScanCode.AsEnumerable().Count(); j++)
                        {
                            for (int i = 0; i < dt.AsEnumerable().Count(); i++)
                            {
                                if (distrinctScanCode.Rows[j]["PromotionName"].ToString() == distrinctRow["PromotionName"].ToString())
                                {
                                    if (distrinctScanCode.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                                    {
                                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                                    }
                                }
                            }
                        }

                        foreach (DataRow itemDT in dt.AsEnumerable())
                        {
                            foreach (DataRow itemDT1 in dt1.AsEnumerable())
                            {
                                if (itemDT1["PromotionName"].ToString() == distrinctRow["PromotionName"].ToString())
                                {
                                    if (itemDT["ScanCode"].ToString() == itemDT1["ScanCode"].ToString())
                                    {
                                        for (int i = 1; i <= sumCount; i++)
                                            if (sumCount == Convert.ToInt32(itemDT1["Quantity"]) * i)
                                            {
                                                string price = "";
                                                if (itemDT1["NewPrice"].ToString() != "")
                                                    price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Quantity"])).ToString();

                                                if (price == "")
                                                    price = (Convert.ToDecimal(itemDT["Oprice"]) - (Convert.ToDecimal(itemDT["Oprice"]) * Convert.ToDecimal(itemDT1["PriceReduce"]) / 100)).ToString();

                                                itemDT["PromotionName"] = itemDT1["PromotionName"];
                                                itemDT["UnitRetail"] = price;
                                                itemDT["Amount"] = Convert.ToDecimal(itemDT["UnitRetail"]) * Convert.ToDecimal(itemDT["Quantity"]);
                                            }
                                    }
                                }
                            }
                        }
                    }
                    JRDGrid.ItemsSource = dt.DefaultView;
                    JRDGrid.Items.Refresh();
                    TotalEvent();
                    textBox1.Text = "";
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Tender_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tenderCode = (sender as Button).Content.ToString();
                if (tenderCode == "Cash")
                {
                    cashTxtPanel.Visibility = Visibility.Visible;
                    sp02.Visibility = Visibility.Hidden;
                    customerTxtPanel.Visibility = Visibility.Hidden;
                    checkTxtPanel.Visibility = Visibility.Hidden;
                }
                if (tenderCode == "Customer")
                {
                    cashTxtPanel.Visibility = Visibility.Hidden;
                    sp02.Visibility = Visibility.Hidden;
                    customerTxtPanel.Visibility = Visibility.Visible;
                    checkTxtPanel.Visibility = Visibility.Hidden;
                }
                if (tenderCode == "Check")
                {
                    checkTxtPanel.Visibility = Visibility.Visible;
                    cashTxtPanel.Visibility = Visibility.Hidden;
                    sp02.Visibility = Visibility.Hidden;
                    customerTxtPanel.Visibility = Visibility.Hidden;
                }
                if (tenderCode == "Card")
                {
                    cashTxtPanel.Visibility = Visibility.Hidden;
                    sp02.Visibility = Visibility.Visible;
                    customerTxtPanel.Visibility = Visibility.Hidden;
                    checkTxtPanel.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void TotalEvent()
        {
            try
            {
                //  decimal sum = 0.00m;
                decimal sum = 0;
                decimal Qtysum = 0;
                decimal Taxsum = 0;
                decimal Total = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    string amounnt = dr.ItemArray[5].ToString();
                    string tax = dr.ItemArray[3].ToString();
                    if (amounnt != "" && tax != "")
                    {
                        sum += decimal.Parse(amounnt);
                        Taxsum += decimal.Parse(tax) * decimal.Parse(amounnt) / 100;
                    }
                    else
                    {
                        sum = 0;
                        Taxsum = 0;
                    }
                    Qtysum += decimal.Parse(dr.ItemArray[4].ToString());
                }
                Total = sum + Taxsum;
                txtTotal.Text = '$' + sum.ToString("0.00");
                //txtQty.Text = Qtysum.ToString();
                taxtTotal.Text = '$' + Taxsum.ToString("0.00");
                grandTotal.Text = '$' + Total.ToString("0.00");
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void TxtCashReceive_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Tab || e.Key == Key.Enter)
                {
                    TxtCashReturn.Text = decimal.Parse(Convert.ToDecimal(decimal.Parse(TxtCashReceive.Text) - decimal.Parse(grandTotal.Text.Replace("$", ""))).ToString("0.00")).ToString("0.00");
                    Button_Click_1();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_1()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string date = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
                string onlydate = date.Substring(0, 10);
                string onlytime = date.Substring(11);
                string totalAmt = txtTotal.Text.Replace("$", "");
                //string userName = lblusername.Content.ToString();
                string tax = taxtTotal.Text.Replace("$", "");
                string grandTotalAmt = grandTotal.Text.Replace("$", "");
                string cashRec = TxtCashReceive.Text;
                string cashReturn = TxtCashReturn.Text;
                string tranid = Convert.ToInt32(lblTranid.Content).ToString();

                string transaction = "insert into Transactions(Tran_id,EndDate,EndTime,GrossAmount,TaxAmount,GrandAmount,CreateBy,CreateOn)Values('" + tranid + "','" + onlydate + "','" + onlytime + "','" + totalAmt + "','" + tax + "','" + grandTotalAmt + "','" + username + "','" + date + "')";
                SqlCommand cmd = new SqlCommand(transaction, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                if (tenderCode == "Cash")
                {
                    string tender = "insert into Tender(EndDate,Endtime,TenderCode,Amount,Change,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + cashRec + "','" + cashReturn + "','" + tranid + "','" + username + "','" + date + "')";
                    SqlCommand cmdTender = new SqlCommand(tender, con);
                    con.Open();
                    cmdTender.ExecuteNonQuery();
                    con.Close();

                    //string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + "-" + cashReturn + "','" + tranid + "','" + username + "','" + date + "')";
                    //SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    //con.Open();
                    //cmdTender1.ExecuteNonQuery();
                    //con.Close();
                }
                else if (tenderCode == "Card")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + username + "','" + date + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                else if (tenderCode == "Customer")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,AccountName,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + cbcustomer.Text + "','" + username + "','" + date + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                else if (tenderCode == "Check")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CheckNo,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + TxtCheck.Text + "','" + username + "','" + date + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                foreach (DataRow dataRow in dt.Rows)
                {
                    dataRow[6] = onlydate;
                    dataRow[7] = onlytime;
                    dataRow[8] = tranid;
                    dataRow[9] = username;
                    dataRow[10] = date;
                }

                SqlBulkCopy objbulk = new SqlBulkCopy(con);
                objbulk.DestinationTableName = "SalesItem";
                objbulk.ColumnMappings.Add("scanCode", "ScanCode");
                objbulk.ColumnMappings.Add("description", "Descripation");
                objbulk.ColumnMappings.Add("quantity", "Quantity");
                objbulk.ColumnMappings.Add("unitretail", "Price");
                objbulk.ColumnMappings.Add("Amount", "Amount");
                objbulk.ColumnMappings.Add("Date", "EndDate");
                objbulk.ColumnMappings.Add("Time", "EndTime");
                objbulk.ColumnMappings.Add("PromotionName", "PromotionName");
                objbulk.ColumnMappings.Add("TransactionId", "TransactionId");
                objbulk.ColumnMappings.Add("CreateBy", "CreateBy");
                objbulk.ColumnMappings.Add("CreateOn", "CreateOn");
                con.Open();
                objbulk.WriteToServer(dt);
                con.Close();
                PrintDocument = new PrintDocument();
                PrintDocument.PrintPage += new PrintPageEventHandler(FormatPage);
                PrintDocument.Print();
                TxtCashReturn.Text = "";
                TxtCashReceive.Text = "";
                cbcustomer.Text = "";
                TxtCheck.Text = "";
                txtTotal.Text = "";
                //txtQty.Text = "";
                grandTotal.Text = "";
                taxtTotal.Text = "";
                lblDate.Content = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
                dt.Clear();
                JRDGrid.Items.Refresh();

                cashTxtPanel.Visibility = Visibility.Hidden;
                sp02.Visibility = Visibility.Visible;
                customerTxtPanel.Visibility = Visibility.Hidden;
                checkTxtPanel.Visibility = Visibility.Hidden;

                loadtransactionId();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void FormatPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string query = "select * from storedetails";
                SqlCommand cmdstore = new SqlCommand(query, con);
                SqlDataAdapter sdastore = new SqlDataAdapter(cmdstore);
                DataTable dtstr = new DataTable();
                sdastore.Fill(dtstr);
                //cbcustomer.ItemsSource = dtstr.DefaultView;
                //cbcustomer.DisplayMemberPath = "Name";

                graphics = e.Graphics;
                Font minifont = new Font("Arial", 7);
                Font itemfont = new Font("Arial", 8);
                Font smallfont = new Font("Arial", 10);
                Font mediumfont = new Font("Arial", 12);
                Font largefont = new Font("Arial", 14);
                Font headerfont = new Font("Arial", 16);
                int Offset = 10;
                int smallinc = 10, mediuminc = 12, largeinc = 15;

                //Image image = Resources.logo;
                //e.Graphics.DrawImage(image, startX + 50, startY + Offset, 100, 30);

                graphics.DrawString(dtstr.Rows[0]["StoreName"].ToString(), headerfont,
                new SolidBrush(Color.Black), 22 + 22, 22);

                //DrawAtStart(dtstr.Rows[0]["StoreName"].ToString(), Offset);

                Offset = Offset + largeinc + 10;

                String underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 0);

                Offset = Offset + mediuminc;
                DrawAtStart("Invoice Number:" + lblTranid.Content, Offset);

                //if (!String.Equals(order.Customer.Address, "N/A"))
                // {
                Offset = Offset + mediuminc;
                DrawAtStart(dtstr.Rows[0]["StoreAddress"].ToString(), Offset);
                //}

                //  if (!String.Equals(order.Customer.Phone, "N/A"))
                // {
                Offset = Offset + mediuminc;
                DrawAtStart(dtstr.Rows[0]["PhoneNumber"].ToString(), Offset);
                //}

                Offset = Offset + mediuminc;
                DrawAtStart("Date: " + DateTime.Now, Offset);

                Offset = Offset + smallinc;
                underLine = "-----------------------------------";
                DrawLine(underLine, largefont, Offset, 2);

                Offset = Offset + largeinc;

                InsertHeaderStyleItem("Name. ", "Qty", "Amount. ", Offset);

                Offset = Offset + largeinc;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    InsertItem(dt.Rows[i]["description"].ToString() + System.Environment.NewLine + dt.Rows[i]["Scancode"].ToString(), dt.Rows[i]["quantity"].ToString(), dt.Rows[i]["Amount"].ToString(), Offset);
                    Offset = Offset + largeinc;
                }

                underLine = "-----------------------------------";
                DrawLine(underLine, largefont, Offset, 2);

                Offset = Offset + largeinc;
                // InsertItem(" Net. Total: ", Offset);

                //if (!order.Cash.Discount.IsZero())
                //{
                //    Offset = Offset + smallinc;
                //    InsertItem(" Discount: ", order.Cash.Discount.CValue, Offset);
                //}

                Offset = Offset + smallinc;
                InsertHeaderStyleItem("Sub Total", "", txtTotal.Text, Offset);
                Offset = Offset + smallinc;
                InsertHeaderStyleItem("Tax", "", taxtTotal.Text, Offset);
                Offset = Offset + smallinc;
                InsertHeaderStyleItem("Amount Payble", "", grandTotal.Text, Offset);

                Offset = Offset + 7;
                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 0);

                Offset = Offset + mediuminc;
                String greetings = "Thanks for visiting us.";
                DrawSimpleString(greetings, mediumfont, Offset, 28);

                Offset = Offset + mediuminc;
                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 0);

                Offset = Offset + largeinc;
                string DrawnBy = "PSPCStore: 0312-0459491 - OR - 0321-6228321";
                DrawSimpleString(DrawnBy, itemfont, Offset, 15);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                App.Current.Properties["username"] = "";
                lblusername.Content = "";
                Login login = new Login();
                this.Close();
                login.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        void DrawAtStart(string text, int Offset)
        {
            try
            {
                int startX = 10;
                int startY = 5;
                Font minifont = new Font("Arial", 8);

                graphics.DrawString(text, minifont,
                         new SolidBrush(Color.Black), startX + 5, startY + Offset);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        void InsertItem(string key, string value, string value1, int Offset)
        {
            try
            {
                Font minifont = new Font("Arial", 8);
                int startX = 10;
                int startY = 5;

                graphics.DrawString(key, minifont,
                             new SolidBrush(Color.Black), startX + 5, startY + Offset);

                graphics.DrawString(value, minifont,
                         new SolidBrush(Color.Black), startX + 100, startY + Offset);
                graphics.DrawString(value1, minifont,
                        new SolidBrush(Color.Black), startX + 150, startY + Offset);
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
        }
        void InsertHeaderStyleItem(string key, string value, string value1, int Offset)
        {
            try
            {
                int startX = 10;
                int startY = 5;
                Font itemfont = new Font("Arial", 8, System.Drawing.FontStyle.Bold);

                graphics.DrawString(key, itemfont,
                             new SolidBrush(Color.Black), startX + 5, startY + Offset);

                graphics.DrawString(value, itemfont,
                         new SolidBrush(Color.Black), startX + 100, startY + Offset);
                graphics.DrawString(value1, itemfont,
                      new SolidBrush(Color.Black), startX + 150, startY + Offset);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }

        }
        void DrawLine(string text, Font font, int Offset, int xOffset)
        {
            try
            {
                int startX = 10;
                int startY = 5;
                graphics.DrawString(text, font,
                         new SolidBrush(Color.Black), startX + xOffset, startY + Offset);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        void DrawSimpleString(string text, Font font, int Offset, int xOffset)
        {
            try
            {
                int startX = 10;
                int startY = 5;
                graphics.DrawString(text, font,
                         new SolidBrush(Color.Black), startX + xOffset, startY + Offset);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void JRDGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                int rowInd = e.Row.GetIndex();
                int dtIdenx = dt.Rows.Count - 1;
                if (rowInd == dtIdenx)
                {
                    if (e.EditAction == DataGridEditAction.Commit)
                    {
                        var column = e.Column as DataGridBoundColumn;
                        if (column != null)
                        {
                            var bindingPath = (column.Binding as Binding).Path.Path;
                            if (bindingPath == "quantity")
                            {
                                int rowIndex = e.Row.GetIndex();
                                var el = e.EditingElement as TextBox;
                                DataRow dataRow = dt.Rows[rowIndex];
                                dt.Rows[rowIndex]["Quantity"] = el.Text;

                                DataTable dt1 = new DataTable();
                                //foreach (DataRow row in dt.AsEnumerable())
                                //{
                                SqlConnection con = new SqlConnection(conString);
                                string _scancode = dt.Rows[rowIndex]["ScanCode"].ToString();
                                string query1 = "select promotiongroup.ScanCode, promotiongroup.PromotionName,promotion.NewPrice,promotion.Quantity,promotion.PriceReduce  from promotiongroup inner join promotion on promotiongroup.promotionname =  promotion.promotionname  where promotiongroup.ScanCode = @_scancode and @datetime between promotion.StartDate and promotion.EndDate";
                                SqlCommand cmd1 = new SqlCommand(query1, con);
                                cmd1.Parameters.AddWithValue("@_scancode", _scancode);
                                cmd1.Parameters.AddWithValue("@datetime", date);
                                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                                con.Open();
                                sda1.Fill(dt1);
                                con.Close();

                                int qDT = Convert.ToInt32(dt.Rows[rowIndex]["Quantity"]);
                                int qDT1 = Convert.ToInt32(dt1.Rows[0]["Quantity"]);

                                if (qDT >= qDT1)
                                {
                                    int dtQunt = qDT;
                                    for (int i = 0; i < qDT; i++)
                                    {
                                        if (dtQunt >= qDT1)
                                            dtQunt = dtQunt - qDT1;
                                    }

                                    string quant = (qDT).ToString();
                                    if (dtQunt != 0)
                                    {
                                        quant = (qDT - dtQunt).ToString();
                                    }

                                    if (dt1.Rows[0]["NewPrice"].ToString() != "")
                                    {
                                        dt.Rows[rowIndex]["Quantity"] = quant;
                                        dt.Rows[rowIndex]["UnitRetail"] = Convert.ToDecimal(dt1.Rows[0]["NewPrice"]) / qDT1;
                                        dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                                    }
                                    else
                                    {
                                        dt.Rows[rowIndex]["Quantity"] = quant;
                                        dt.Rows[rowIndex]["UnitRetail"] = Convert.ToDecimal(dt.Rows[rowIndex]["OPrice"]) - (Convert.ToDecimal(dt.Rows[rowIndex]["OPrice"]) * Convert.ToDecimal(dt1.Rows[0]["PriceReduce"]) / 100);
                                        dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                                    }

                                    //dt.Rows[rowIndex]["Quantity"] = quant;
                                    //dt.Rows[rowIndex]["UnitRetail"] = Convert.ToDecimal(dt1.Rows[0]["NewPrice"])/ qDT1;
                                    //dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                                    dt.Rows[rowIndex]["PromotionName"] = dt1.Rows[0]["PromotionName"];
                                    if (dtQunt != 0)
                                    {
                                        for (int a = 0; a < dtQunt; a++)
                                        {
                                            DataRow newRow = dt.NewRow();
                                            newRow["ScanCode"] = dt.Rows[rowIndex]["ScanCode"];
                                            newRow["Description"] = dt.Rows[rowIndex]["Description"];
                                            newRow["Quantity"] = 1;
                                            newRow["UnitRetail"] = dt.Rows[rowIndex]["OPrice"];
                                            newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                                            newRow["OPrice"] = dt.Rows[rowIndex]["OPrice"];
                                            newRow["TaxRate"] = dt.Rows[rowIndex]["TaxRate"];
                                            dt.Rows.Add(newRow);
                                        }

                                    }

                                }
                                else
                                {
                                    dt.Rows[rowIndex]["Amount"] = Convert.ToInt32(dt.Rows[rowIndex]["Quantity"]) * Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]);
                                }
                                //}

                                //DataTable distrinctPromotionName = dt1.DefaultView.ToTable(true, "PromotionName");
                                //DataTable distrinctSC = dt1.DefaultView.ToTable(true, "ScanCode", "PromotionName", "Quantity", "NewPrice", "PriceReduce");

                                //var dtWhere = (from dtRow in dt.AsEnumerable()
                                //               where Convert.ToInt32(dtRow.Field<string>("Quantity")) > 1
                                //               select dtRow).ToList();

                                //int dtCount = dtWhere.Count();

                                //foreach (DataRow distrinctRow in distrinctPromotionName.AsEnumerable())
                                //{
                                //    int sumCount = 0;
                                //    for (int j = 0; j < distrinctSC.AsEnumerable().Count(); j++)
                                //    {
                                //        for (int i = 0; i < dt.AsEnumerable().Count(); i++)
                                //        {
                                //            if (distrinctSC.Rows[j]["PromotionName"].ToString() == distrinctRow["PromotionName"].ToString())
                                //            {
                                //                if (distrinctSC.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                                //                {
                                //                    sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                                //                }
                                //            }
                                //        }
                                //    }
                                //    foreach (DataRow itemDT1 in distrinctSC.AsEnumerable())
                                //    {
                                //        foreach (DataRow itemDT in dt.AsEnumerable())
                                //        {
                                //            if (itemDT1["PromotionName"].ToString() == distrinctRow["PromotionName"].ToString())
                                //            {
                                //                if (itemDT["ScanCode"].ToString() == itemDT1["ScanCode"].ToString())
                                //                {
                                //                    decimal price = 0;
                                //                    for (int i = 1; i <= dtCount; i++)
                                //                    {
                                //                        int z = sumCount / Convert.ToInt32(itemDT1["Quantity"]);
                                //                        if (Convert.ToInt32(itemDT["Quantity"]) > 1)
                                //                        {
                                //                            if (Convert.ToInt32(sumCount) >= Convert.ToInt32(itemDT1["Quantity"]) * z && z > 0)
                                //                            {
                                //                                //var dfg = itemDT1["NewPrice"].ToString();
                                //                                if (itemDT1["NewPrice"].ToString() != "")
                                //                                {
                                //                                    decimal price1 = z * Convert.ToDecimal(itemDT1["NewPrice"]);
                                //                                    decimal price2 = (Convert.ToInt32(sumCount) - Convert.ToInt32(itemDT1["Quantity"]) * z) * Convert.ToDecimal(itemDT["Oprice"]);
                                //                                    price = (price1 + price2) / Convert.ToInt32(sumCount);
                                //                                }
                                //                                else
                                //                                {
                                //                                    decimal price3 = z * Convert.ToInt32(itemDT1["Quantity"]) * (Convert.ToDecimal(itemDT["Oprice"]) - (Convert.ToDecimal(itemDT["Oprice"]) * Convert.ToDecimal(itemDT1["PriceReduce"]) / 100));
                                //                                    decimal price4 = (sumCount - Convert.ToInt32(itemDT1["Quantity"]) * z) * Convert.ToDecimal(itemDT["Oprice"]);
                                //                                    price = (price3 + price4) / sumCount;
                                //                                }
                                //                                itemDT["PromotionName"] = itemDT1["PromotionName"];
                                //                                itemDT["UnitRetail"] = Convert.ToDecimal(price).ToString("0.00");
                                //                                itemDT["Amount"] = Convert.ToDecimal(price * Convert.ToDecimal(itemDT["Quantity"])).ToString("0.00");
                                //                            }
                                //                            else if (dt.AsEnumerable().Count() >= i)
                                //                            {
                                //                                itemDT["UnitRetail"] = Convert.ToDecimal(itemDT["Oprice"]).ToString("0.00");
                                //                                itemDT["Amount"] = Convert.ToDecimal(Convert.ToDecimal(itemDT["UnitRetail"]) * Convert.ToDecimal(itemDT["Quantity"])).ToString("0.00");
                                //                            }
                                //                        }
                                //                    }
                                //                }
                                //            }
                                //        }
                                //    }
                                //}
                                //dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                                //if (dt.AsEnumerable().Count() > Convert.ToInt32(dtCount))

                                dt = ScanCodeFunction(dt, rowIndex);

                                JRDGrid.ItemsSource = dt.DefaultView;
                                TotalEvent();
                            }
                        }
                    }
                }
                else
                {
                    dt.Rows[rowInd]["Quantity"] = dt.Rows[rowInd]["Quantity"];
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void NumButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string number = (sender as Button).Content.ToString();

                if (txtGotFocusStr == "textBox1")
                {
                    string textBox1Str = textBox1.Text;
                    textBox1.Text = textBox1Str + number;
                }
                if (txtGotFocusStr == "TxtCashReceive")
                {
                    string textBox1Str = TxtCashReceive.Text;
                    TxtCashReceive.Text = textBox1Str + number;
                }
                if (txtGotFocusStr == "TxtCheck")
                {
                    string textBox1Str = TxtCheck.Text;
                    TxtCheck.Text = textBox1Str + number;
                }
                if (txtGotFocusStr == "txtDeptAmt")
                {
                    string textBox1Str = txtDeptAmt.Text;
                    txtDeptAmt.Text = textBox1Str + number;
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void SendErrorToText(Exception ex, string errorFileName)
        {
            var line = Environment.NewLine + Environment.NewLine;
            ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();

            ErrorLocation = ex.Message.ToString();
            try
            {
                string filepath = System.AppDomain.CurrentDomain.BaseDirectory;
                string errorpath = filepath + "\\ErrorFiles\\";

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                filepath = filepath + "log.txt";

                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt") + line + "File Name :" + errorFileName + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line + " Error Page Url:" + " " + exurl + line + "User Host IP:" + " " + hostIp + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt") + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();
                }
            }

            catch (Exception e)
            {
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Report rpt = new Report();
            rpt.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (txtGotFocusStr == "textBox1")
            {
                textBox1.Text = "";
            }
            if (txtGotFocusStr == "TxtCashReceive")
            {
                TxtCashReceive.Text = "";
            }
            if (txtGotFocusStr == "TxtCheck")
            {
                TxtCheck.Text = "";
            }
            if (txtGotFocusStr == "txtDeptAmt")
            {
                txtDeptAmt.Text = "";
            }
        }

        //Shift close
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string tenderQ = "Update tender set shiftClose=@username Where shiftClose is null";
                SqlCommand tenderCMD = new SqlCommand(tenderQ, con);
                tenderCMD.Parameters.AddWithValue("@username", username);
                string transQ = "Update Transactions set shiftClose=@username Where shiftClose is null";
                SqlCommand transCMD = new SqlCommand(transQ, con);
                transCMD.Parameters.AddWithValue("@username", username);
                string itemQ = "Update SalesItem set shiftClose=@username Where shiftClose is null";
                SqlCommand itemCMD = new SqlCommand(itemQ, con);
                itemCMD.Parameters.AddWithValue("@username", username);
                string expQ = "Update Expence set shiftClose=@username Where shiftClose is null";
                SqlCommand expCMD = new SqlCommand(expQ, con);
                expCMD.Parameters.AddWithValue("@username", username);
                con.Open();
                tenderCMD.ExecuteNonQuery();
                transCMD.ExecuteNonQuery();
                itemCMD.ExecuteNonQuery();
                expCMD.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        //page close
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                if (tb != null)
                {
                    txtGotFocusStr = tb.Name;
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Department_Button_Click(object sender, RoutedEventArgs e)
        {
            btnShortKey.Visibility = Visibility.Visible;
            btnDept.Visibility = Visibility.Hidden;
            sp21.Visibility = Visibility.Visible;
            sp22.Visibility = Visibility.Hidden;
            sp23.Visibility = Visibility.Hidden;
        }

        private void ShortcutKey_Button_Click(object sender, RoutedEventArgs e)
        {
            btnShortKey.Visibility = Visibility.Hidden;
            btnDept.Visibility = Visibility.Visible;
            sp21.Visibility = Visibility.Hidden;
            sp23.Visibility = Visibility.Hidden;
            TxtBxStackPanel2.Visibility = Visibility.Hidden;
            sp22.Visibility = Visibility.Visible;

            sp22.Children.Clear();
            SqlConnection con = new SqlConnection(conString);
            string queryS = "select category from addcategory";
            SqlCommand cmd1 = new SqlCommand(queryS, con);
            SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
            DataTable dtCat = new DataTable();
            sda1.Fill(dtCat);

            for (int i = 0; i < dtCat.Rows.Count; i++)
            {
                Button button = new Button();
                button.Content = new TextBlock()
                {
                    FontSize = 25,
                    Text = dtCat.Rows[i].ItemArray[0].ToString(),
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };
                if (dtCat.Rows[i].ItemArray[0].ToString() != "")
                {
                    //var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                    var path = dtCat.Rows[i].ItemArray[0].ToString();
                    //var fullpath = Path + "\\Image\\" + path;
                    //button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                }
                button.Foreground = new SolidColorBrush(Colors.White);
                button.FontSize = 26;
                button.FontWeight = FontWeights.Bold;
                button.Effect = new DropShadowEffect()
                { Color = Colors.BlueViolet };
                button.Margin = new Thickness(5, 5, 5, 5);
                string abc = dtCat.Rows[i].ItemArray[0].ToString();
                button.Click += new RoutedEventHandler(button_Click_Category);
                //button.Click += (sender, e) => { button_Click_CategoryDescription(sender, e); };
                this.sp22.Children.Add(button);
            }
            sp23.Children.Clear();
            dtCat = null;
        }

        private void JdGrid_delete_click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            // Remove Record.
            DataGrid newGrid = new DataGrid();
            DataRowView row = (DataRowView)JRDGrid.SelectedItem;
            dt.Rows.Remove(row.Row);
            dt.AcceptChanges();
            newGrid.ItemsSource = dt.DefaultView;
            dt = ((DataView)newGrid.ItemsSource).ToTable();

            if (dt.AsEnumerable().Count() != 0)
            {
                DataTable dt1 = new DataTable();
                foreach (DataRow row1 in dt.AsEnumerable())
                {
                    string _scancode = row1["ScanCode"].ToString();
                    string query1 = "select promotiongroup.ScanCode, promotiongroup.PromotionName,promotion.NewPrice,promotion.Quantity,promotion.PriceReduce  from promotiongroup inner join promotion on promotiongroup.promotionname =  promotion.promotionname  where promotiongroup.ScanCode = @_scancode and @datetime between promotion.StartDate and promotion.EndDate";
                    SqlCommand cmd1 = new SqlCommand(query1, con);
                    cmd1.Parameters.AddWithValue("@_scancode", _scancode);
                    cmd1.Parameters.AddWithValue("@datetime", date);
                    SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                    con.Open();
                    sda1.Fill(dt1);
                    con.Close();
                }


                DataTable distrinctPromotionName = dt1.DefaultView.ToTable(true, "PromotionName");

                var dtWhere = (from dtRow in dt.AsEnumerable()
                               where Convert.ToInt32(dtRow.Field<string>("Quantity")) > 1
                               select dtRow).ToList();

                int dtCount = dtWhere.Count();
                int distCount = distrinctPromotionName.AsEnumerable().Count();
                int qutCount = 0;
                foreach (var item in distrinctPromotionName.AsEnumerable())
                {
                    for (int j = 0; j < distCount; j++)
                    {
                        for (int i = 0; i < dt.AsEnumerable().Count(); i++)
                        {
                            if (dt1.Rows[j]["PromotionName"].ToString() == item["PromotionName"].ToString())
                            {
                                if (dt1.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                                {
                                    qutCount = Convert.ToInt32(qutCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                                }
                            }
                        }
                    }

                    foreach (DataRow itemDT in dt.AsEnumerable())
                    {
                        foreach (DataRow itemDT1 in dt1.AsEnumerable())
                        {
                            if (item["PromotionName"].ToString() == itemDT1["PromotionName"].ToString())
                            {
                                if (itemDT["ScanCode"].ToString() == itemDT1["ScanCode"].ToString())
                                {
                                    decimal price = 0;


                                    for (int i = 1; i <= qutCount; i++)
                                    {
                                        int z = Convert.ToInt32(itemDT["quantity"]) / Convert.ToInt32(itemDT1["Quantity"]);
                                        if (Convert.ToInt32(itemDT["Quantity"]) > 1)
                                        {
                                            int j = i - 1;
                                            if (Convert.ToInt32(qutCount) >= Convert.ToInt32(itemDT1["Quantity"]) * z && z > 0)
                                            {
                                                if (itemDT1["NewPrice"].ToString() != "")
                                                {
                                                    decimal price1 = z * Convert.ToDecimal(itemDT1["NewPrice"]);
                                                    decimal price2 = (Convert.ToInt32(itemDT["quantity"]) - Convert.ToInt32(itemDT1["Quantity"]) * z) * Convert.ToDecimal(itemDT["Oprice"]);
                                                    price = (price1 + price2) / Convert.ToInt32(itemDT["quantity"]);
                                                }
                                                else
                                                {
                                                    decimal price3 = z * Convert.ToInt32(itemDT1["Quantity"]) * (Convert.ToDecimal(itemDT["Oprice"]) - (Convert.ToDecimal(itemDT["Oprice"]) * Convert.ToDecimal(itemDT1["PriceReduce"]) / 100));
                                                    decimal price4 = (Convert.ToInt32(itemDT["quantity"]) - Convert.ToInt32(itemDT1["Quantity"]) * z) * Convert.ToDecimal(itemDT["Oprice"]);
                                                    price = (price3 + price4) / Convert.ToInt32(itemDT["quantity"]);
                                                }
                                                itemDT["UnitRetail"] = Convert.ToDecimal(price).ToString("0.00");
                                                itemDT["Amount"] = Convert.ToDecimal(Convert.ToDecimal(price) * Convert.ToDecimal(itemDT["Quantity"])).ToString("0.00");
                                            }
                                            else if (dt.AsEnumerable().Count() >= i)
                                            {
                                                itemDT["UnitRetail"] = Convert.ToDecimal(itemDT["Oprice"]).ToString("0.00");
                                                itemDT["Amount"] = Convert.ToDecimal(Convert.ToDecimal(itemDT["UnitRetail"]) * Convert.ToDecimal(itemDT["Quantity"])).ToString("0.00");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (dt.AsEnumerable().Count() > Convert.ToInt32(dtCount))
                {
                    int index = dt.AsEnumerable().Count();
                    int quCount = 0;

                    foreach (var item in dt.AsEnumerable())
                    {
                        if (item["Quantity"].ToString() == "1")
                            quCount++;
                    }

                    DataTable dt12 = new DataTable();
                    foreach (DataRow row1 in dt.AsEnumerable())
                    {
                        string _scancode = row1["ScanCode"].ToString();
                        string query1 = "select promotiongroup.ScanCode, promotiongroup.PromotionName,promotion.NewPrice,promotion.Quantity,promotion.PriceReduce  from promotiongroup inner join promotion on promotiongroup.promotionname =  promotion.promotionname  where promotiongroup.ScanCode = @_scancode and @datetime between promotion.StartDate and promotion.EndDate";
                        SqlCommand cmd1 = new SqlCommand(query1, con);
                        cmd1.Parameters.AddWithValue("@_scancode", _scancode);
                        cmd1.Parameters.AddWithValue("@datetime", date);
                        SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                        con.Open();
                        sda1.Fill(dt12);
                        con.Close();
                    }

                    for (int k = 0; k < quCount; k++)
                    {
                        int d = quCount;
                        for (int i = 0; i < quCount; i++)
                        {
                            if (d >= Convert.ToInt32(dt12.Rows[k]["Quantity"]))
                                d = d - Convert.ToInt32(dt12.Rows[k]["Quantity"]);
                        }

                        if (d > 0)
                        {
                            if (d == -1)
                                d = 1;

                            for (int i = 0; i < d; i++)
                            {
                                int j = i + 1;
                                int ind = index - j;
                                DataTable dt2 = new DataTable();
                                string query = "select UnitRetail,UnitRetail as Amount from Item inner join Department on item.Department=Department.Department where Scancode=@password ";
                                SqlCommand cmd = new SqlCommand(query, con);
                                cmd.Parameters.AddWithValue("@password", dt.Rows[ind]["ScanCode"].ToString());
                                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                                sda.Fill(dt2);
                                if (dt2.AsEnumerable().Count() != 0)
                                {
                                    dt.Rows[ind]["UnitRetail"] = dt2.Rows[0]["UnitRetail"];
                                    dt.Rows[ind]["Amount"] = dt2.Rows[0]["Amount"];
                                }
                            }
                        }
                    }
                }
                //if (dt.AsEnumerable().Count() > Convert.ToInt32(dtCount))
                //{
                //    int index = dt.AsEnumerable().Count();

                //    DataTable distrinctPromotionName1 = dt.DefaultView.ToTable(true, "PromotionName");
                //    int dtdCount;
                //    foreach (var item in distrinctPromotionName1.AsEnumerable())
                //    {
                //        dtdCount = 0;

                //        foreach (var item1 in dt.AsEnumerable())
                //        {
                //            if (item1["PromotionName"] == item["PromotionName"] && item1["Quantity"].ToString() == "1")
                //                dtdCount++;
                //        }

                //        DataTable distrinctScanCode = dt.DefaultView.ToTable(true, "ScanCode");

                //        DataTable dt12 = new DataTable();
                //        foreach (DataRow row1 in distrinctScanCode.AsEnumerable())
                //        {
                //            string _scancode = row1["ScanCode"].ToString();
                //            string query1 = "select promotiongroup.ScanCode, promotiongroup.PromotionName,promotion.NewPrice,promotion.Quantity,promotion.PriceReduce  from promotiongroup inner join promotion on promotiongroup.promotionname =  promotion.promotionname  where promotiongroup.ScanCode = @_scancode and @datetime between promotion.StartDate and promotion.EndDate";
                //            SqlCommand cmd1 = new SqlCommand(query1, con);
                //            cmd1.Parameters.AddWithValue("@_scancode", _scancode);
                //            cmd1.Parameters.AddWithValue("@datetime", date);
                //            SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                //            con.Open();
                //            sda1.Fill(dt12);
                //            con.Close();

                //            int d = dtdCount;
                //            for (int i = 0; i < dtdCount; i++)
                //            {
                //                if (d >= Convert.ToInt32(dt12.Rows[0]["Quantity"]))
                //                    d = d - Convert.ToInt32(dt12.Rows[0]["Quantity"]);
                //            }

                //            if (d == -1)
                //                d = 1;

                //            for (int i = 0; i < d; i++)
                //            {
                //                int j = i + 1;
                //                int ind = index - j;
                //                DataTable dt2 = new DataTable();
                //                string query = "select UnitRetail,UnitRetail as Amount from Item inner join Department on item.Department=Department.Department where Scancode=@password ";
                //                SqlCommand cmd = new SqlCommand(query, con);
                //                cmd.Parameters.AddWithValue("@password", dt.Rows[i]["ScanCode"].ToString());
                //                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                //                sda.Fill(dt2);
                //                if (dt2.AsEnumerable().Count() != 0)
                //                {
                //                    dt.Rows[ind]["UnitRetail"] = dt2.Rows[0]["UnitRetail"];
                //                    dt.Rows[ind]["Amount"] = dt2.Rows[0]["Amount"];
                //                }
                //            }
                //        }
                //    }
                //}
            }
            JRDGrid.ItemsSource = dt.DefaultView;
            TotalEvent();
        }

        private void TxtCheck_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Tab || e.Key == Key.Enter)
                {
                    Button_Click_1();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                btnConform.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void btnConform_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button_Click_1();
                btnConform.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        public DataTable ScanCodeFunction(DataTable datatable, int rowindex)
        {
            // datatable.DefaultView.Sort = "Quantity DESC,ScanCode";
            // datatable = datatable.DefaultView.ToTable();
            datatable.DefaultView.Sort = "ScanCode";
            datatable = datatable.DefaultView.ToTable();
            SqlConnection con = new SqlConnection(conString);

            for (int i = 0; i < datatable.AsEnumerable().Count(); i++)
            {
                if (Convert.ToInt32(datatable.Rows[i]["Quantity"]) == 1)
                {
                    DataTable dt2 = new DataTable();
                    var sc = datatable.Rows[i]["ScanCode"].ToString();
                    if (sc != "0")
                    {
                        string query = "select UnitRetail,UnitRetail as Amount from Item inner join Department on item.Department=Department.Department where Scancode=@password ";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@password", datatable.Rows[i]["ScanCode"].ToString());
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        sda.Fill(dt2);
                        if (dt2.AsEnumerable().Count() != 0)
                        {
                            datatable.Rows[i]["UnitRetail"] = dt2.Rows[0]["UnitRetail"];
                            datatable.Rows[i]["Amount"] = dt2.Rows[0]["Amount"];
                        }
                    }
                }
            }


            DataTable dt1 = new DataTable();
            foreach (DataRow row in datatable.AsEnumerable())
            {

                string _scancode = row["ScanCode"].ToString();
                string query1 = "select promotiongroup.ScanCode, promotiongroup.PromotionName,promotion.NewPrice,promotion.Quantity,promotion.PriceReduce  from promotiongroup inner join promotion on promotiongroup.promotionname =  promotion.promotionname  where promotiongroup.ScanCode = @_scancode and @datetime between promotion.StartDate and promotion.EndDate";
                SqlCommand cmd1 = new SqlCommand(query1, con);
                cmd1.Parameters.AddWithValue("@_scancode", _scancode);
                cmd1.Parameters.AddWithValue("@datetime", date);
                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                con.Open();
                sda1.Fill(dt1);
                con.Close();
            }
            DataTable distrinctPromotionName = dt1.DefaultView.ToTable(true, "PromotionName");
            DataTable distrinctSCANCODE = dt1.DefaultView.ToTable(true, "ScanCode", "PromotionName", "Quantity", "NewPrice", "PriceReduce");
            //int distCount = distrinctPromotionName.AsEnumerable().Count();
            foreach (DataRow distrinctRow in distrinctPromotionName.AsEnumerable())
            {

                //var res = from row in datatable.AsEnumerable()
                //          where row.Field<string>("PromotionName") == distrinctRow["PromotionName"].ToString()
                //          select row;

                //int qc = res.Sum(row => Convert.ToInt32(row.Field<string>("Quantity")));

                int sumCount = 0;
                for (int j = 0; j < distrinctSCANCODE.AsEnumerable().Count(); j++)
                {
                    for (int i = 0; i < datatable.Rows.Count; i++)
                    {
                        if (distrinctSCANCODE.Rows[j]["PromotionName"].ToString() == distrinctRow["PromotionName"].ToString())
                        {
                            if (distrinctSCANCODE.Rows[j]["ScanCode"].ToString() == datatable.Rows[i]["ScanCode"].ToString())
                            {
                                sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(datatable.Rows[i]["Quantity"]);
                                foreach (DataRow itemDT in datatable.AsEnumerable())
                                {
                                    foreach (DataRow itemDT1 in distrinctSCANCODE.AsEnumerable())
                                    {
                                        if (itemDT1["PromotionName"].ToString() == distrinctRow["PromotionName"].ToString())
                                        {
                                            if (datatable.Rows[i]["ScanCode"].ToString() == itemDT1["ScanCode"].ToString())
                                            {
                                                int Y = sumCount / Convert.ToInt32(itemDT1["Quantity"]);
                                                for (int x = 1; x <= Y; x++)
                                                {
                                                    if (sumCount == Convert.ToInt32(itemDT1["Quantity"]) * x)
                                                    {
                                                        for (int z = 0; z <= i; z++)
                                                        {
                                                            string price = "";
                                                            if (itemDT1["NewPrice"].ToString() != "")
                                                                price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Quantity"])).ToString();

                                                            if (price == "")
                                                                price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) * Convert.ToDecimal(itemDT1["PriceReduce"]) / 100)).ToString();

                                                            datatable.Rows[z]["PromotionName"] = itemDT1["PromotionName"];
                                                            datatable.Rows[z]["UnitRetail"] = price;
                                                            datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                                                        }
                                                        return datatable;
                                                    }
                                                    else if (sumCount > Convert.ToInt32(itemDT1["Quantity"]) * x)
                                                    {
                                                        int q1 = sumCount - Convert.ToInt32(datatable.Rows[i]["Quantity"]);
                                                        int oldqty = Convert.ToInt32(datatable.Rows[i]["Quantity"]);
                                                        int q2 = Convert.ToInt32(itemDT1["Quantity"]) - q1;
                                                        int finalqty = oldqty - q2;
                                                        if (datatable.Rows.Count == rowindex + 1)
                                                        {
                                                            for (int z = 0; z <= i; z++)
                                                            {
                                                                if (z == i)
                                                                {
                                                                    if (q2 != 0)
                                                                    {
                                                                        string price = "";
                                                                        if (itemDT1["NewPrice"].ToString() != "")
                                                                            price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Quantity"])).ToString();

                                                                        if (price == "")
                                                                            price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) * Convert.ToDecimal(itemDT1["PriceReduce"]) / 100)).ToString();
                                                                        datatable.Rows[z]["Quantity"] = q2;
                                                                        datatable.Rows[z]["PromotionName"] = itemDT1["PromotionName"];
                                                                        datatable.Rows[z]["UnitRetail"] = price;
                                                                        datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                                                                        if (finalqty != 0)
                                                                        {
                                                                            DataRow newRow = datatable.NewRow();
                                                                            newRow["ScanCode"] = datatable.Rows[z]["ScanCode"];
                                                                            newRow["Description"] = datatable.Rows[z]["Description"];
                                                                            newRow["Quantity"] = oldqty - q2;
                                                                            newRow["UnitRetail"] = datatable.Rows[z]["OPrice"];
                                                                            newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                                                                            newRow["OPrice"] = datatable.Rows[z]["OPrice"];
                                                                            newRow["TaxRate"] = datatable.Rows[z]["TaxRate"];
                                                                            datatable.Rows.Add(newRow);
                                                                        }
                                                                    }


                                                                    else
                                                                    {
                                                                        datatable.Rows[z]["UnitRetail"] = datatable.Rows[z]["OPrice"];
                                                                        datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                                                                    }
                                                                    return datatable;

                                                                }
                                                                else
                                                                {
                                                                    string price = "";
                                                                    if (itemDT1["NewPrice"].ToString() != "")
                                                                        price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Quantity"])).ToString();

                                                                    if (price == "")
                                                                        price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) * Convert.ToDecimal(itemDT1["PriceReduce"]) / 100)).ToString();

                                                                    datatable.Rows[z]["PromotionName"] = itemDT1["PromotionName"];
                                                                    datatable.Rows[z]["UnitRetail"] = price;
                                                                    datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }




                }

                //foreach (DataRow itemDT in dt.AsEnumerable())
                //{
                //    foreach (DataRow itemDT1 in dt1.AsEnumerable())
                //    {
                //        if (itemDT1["PromotionName"].ToString() == distrinctRow["PromotionName"].ToString())
                //        {
                //            if (itemDT["ScanCode"].ToString() == itemDT1["ScanCode"].ToString())
                //            {
                //                int Y = sumCount / Convert.ToInt32(itemDT1["Quantity"]);
                //                for (int i = 1; i <= sumCount; i++)
                //                {
                //                    if (sumCount > Convert.ToInt32(itemDT1["Quantity"]) * i)
                //                    {
                //                        int q1 = sumCount - Convert.ToInt32(datatable.Rows[i]["Quantity"]);
                //                        int oldqty = Convert.ToInt32(datatable.Rows[i]["Quantity"]);
                //                        int q2 = Convert.ToInt32(itemDT1["Quantity"]) - q1;
                //                        int finalqty = oldqty - q2;


                //                        string price = "";
                //                        if (itemDT1["NewPrice"].ToString() != "")
                //                            price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Quantity"])).ToString();

                //                        if (price == "")
                //                            price = (Convert.ToDecimal(itemDT["Oprice"]) - (Convert.ToDecimal(itemDT["Oprice"]) * Convert.ToDecimal(itemDT1["PriceReduce"]) / 100)).ToString();
                //                        itemDT["Quantity"] = q2;
                //                        itemDT["PromotionName"] = itemDT1["PromotionName"];
                //                        itemDT["UnitRetail"] = price;
                //                        itemDT["Amount"] = Convert.ToDecimal(itemDT["UnitRetail"]) * Convert.ToDecimal(itemDT["Quantity"]);

                //                        DataRow newRow = dt.NewRow();
                //                        newRow["ScanCode"] = dt.Rows[i]["ScanCode"];
                //                        newRow["Description"] = dt.Rows[i]["Description"];
                //                        newRow["Quantity"] = oldqty - q2;
                //                        newRow["UnitRetail"] = dt.Rows[i]["OPrice"];
                //                        newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                //                        newRow["OPrice"] = dt.Rows[i]["OPrice"];
                //                        newRow["TaxRate"] = dt.Rows[i]["TaxRate"];
                //                        dt.Rows.Add(newRow);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
            }

            return datatable;
        }

        void button_Click_Category(object sender, RoutedEventArgs e)
        {
            var btnContent = sender as Button;
            var tb = (TextBlock)btnContent.Content;

            btnShortKey.Visibility = Visibility.Hidden;
            btnDept.Visibility = Visibility.Visible;
            sp21.Visibility = Visibility.Hidden;
            sp22.Visibility = Visibility.Hidden;
            sp23.Visibility = Visibility.Visible;

            sp23.Children.Clear();
            SqlConnection con = new SqlConnection(conString);
            string queryS = "select Description from category where category = '" + tb.Text + "' ";
            SqlCommand cmd1 = new SqlCommand(queryS, con);
            SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
            DataTable dtCatDescr = new DataTable();
            sda1.Fill(dtCatDescr);

            for (int i = 0; i < dtCatDescr.Rows.Count; i++)
            {
                Button button = new Button();
                button.Content = new TextBlock()
                {
                    FontSize = 25,
                    Text = dtCatDescr.Rows[i].ItemArray[0].ToString(),
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };
                if (dtCatDescr.Rows[i].ItemArray[0].ToString() != "")
                {
                    //var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                    var path = dtCatDescr.Rows[i].ItemArray[0].ToString();
                    //var fullpath = Path + "\\Image\\" + path;
                    //button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                }
                button.Foreground = new SolidColorBrush(Colors.White);
                button.FontSize = 26;
                button.FontWeight = FontWeights.Bold;
                button.Effect = new DropShadowEffect()
                { Color = Colors.BlueViolet };
                button.Margin = new Thickness(5, 5, 5, 5);
                string abc = dtCatDescr.Rows[i].ItemArray[0].ToString();
                button.Click += new RoutedEventHandler(button_Click_Category_Description);
                this.sp23.Children.Add(button);
            }
        }

        void button_Click_Category_Description(object sender, RoutedEventArgs e)
        {
            var btnContent = sender as Button;
            var tb = (TextBlock)btnContent.Content;
            SqlConnection con = new SqlConnection(conString);
            string querya = "select CATEGORY  from Category  where Category = @Description";
            SqlCommand cmda = new SqlCommand(querya, con);
            cmda.Parameters.AddWithValue("@Description", tb.Text);
            cmda.Parameters.AddWithValue("@qty", 1);
            SqlDataAdapter sdaa = new SqlDataAdapter(cmda);
            DataTable dta = new DataTable();
            sdaa.Fill(dta);
            int A = dta.Rows.Count;
            if (A != 0)
                button_Click_Category(sender, e);
            else
            {
                string query = "select Category.ScanCode,Category.Description,item.UnitRetail,Department.TaxRate,@qty as Quantity,item.UnitRetail as Amount  from Category join Item on Category.scancode = Item.scancode join Department on Item.Department = Department.Department where Item.Description = @Description";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Description", tb.Text);
                cmd.Parameters.AddWithValue("@qty", 1);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                con.Open();
                sda.Fill(dt);
                con.Close();
                JRDGrid.ItemsSource = dt.DefaultView;
                sp23.Visibility = Visibility.Hidden;
                sp22.Visibility = Visibility.Visible;
                TotalEvent();
            }
        }
    }
}
