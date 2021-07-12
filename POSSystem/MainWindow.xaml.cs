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

namespace POSSystem
{
    public partial class MainWindow : Window
    {
        private PrintDocument PrintDocument;
        private Graphics graphics;
        string tenderCode = "";
        DataTable dt = new DataTable();
        DataTable dtdep = new DataTable();
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        string username = App.Current.Properties["username"].ToString();

        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "MainWindow.cs";

        string txtGotFocusStr = string.Empty;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                lblDate.Content = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
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

                //con.Close();
                textBox1.Focus();

                string queryS = "Select Department,TaxRate,FilePath from Department";
                SqlCommand cmd1 = new SqlCommand(queryS, con);
                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                sda1.Fill(dtdep);
                con.Open();
                cmd1.ExecuteNonQuery();
                con.Close();

                //Shadow Effect Of Button
                //DropShadowEffect newDropShadowEffect = new DropShadowEffect();
                //newDropShadowEffect.BlurRadius = 5;
                //newDropShadowEffect.Direction = 100;
                //newDropShadowEffect.Opacity = 95;
                //newDropShadowEffect.ShadowDepth = 5;


                for (int i = 0; i < dtdep.Rows.Count; ++i)
                {
                    Button button = new Button()
                    {
                        Content = dtdep.Rows[i].ItemArray[0],
                        Tag = i
                    };
                    if (dtdep.Rows[i].ItemArray[2].ToString() != "")
                    {
                        var Path = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
                        var path = dtdep.Rows[i].ItemArray[2].ToString();
                        var fullpath = Path + "\\Image\\" + path;
                        button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                    }
                    button.Foreground = new SolidColorBrush(Colors.White);
                    button.FontSize = 30;
                    button.FontWeight = FontWeights.Bold;
                    //button.Effect = new DropShadowEffect()
                    //{ Color = Colors.BlueViolet };
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
            catch (Exception ex) { }
        }

        private void loadtransactionId()
        {
            using (SqlConnection conn = new SqlConnection(conString))
            {
                conn.Open();
                string query1 = "SELECT TOP 1 * FROM Transactions ORDER BY TransactionId DESC";
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
                taxrate = abc;
                lblDepartment.Content = btnContent.Content;
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
                if (e.Key == Key.Enter || e.Key == Key.Tab)
                {
                    //string code = textBox1.Text.Remove(textBox1.Text.Length - 1, 1);
                    SqlConnection con = new SqlConnection(conString);
                    string query = "select Scancode,Description,UnitRetail,@qty as quantity,UnitRetail as Amount,Department.TaxRate from Item inner join Department on item.Department=Department.Department where Scancode=@password ";
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
                        string query1 = "select promotiongroup.ScanCode, promotiongroup.PromotionName,promotion.NewPrice,promotion.Quantity,promotion.pricereduce  from promotiongroup inner join promotion on promotiongroup.promotionname =  promotion.promotiongroup  where promotiongroup.ScanCode = @_scancode";
                        SqlCommand cmd1 = new SqlCommand(query1, con);
                        cmd1.Parameters.AddWithValue("@_scancode", _scancode);
                        SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                        con.Open();
                        sda1.Fill(dt1);
                        con.Close();
                    }

                    var sum = dt1.AsEnumerable().Count();  //dt.AsEnumerable().Sum(x => Convert.ToInt32(x["Quantity"]));
                    foreach (DataRow itemDT in dt.AsEnumerable())
                    {
                        foreach (DataRow itemDT1 in dt1.AsEnumerable())
                        {
                            if (itemDT["ScanCode"].ToString() == itemDT1["ScanCode"].ToString())
                            {
                                if (sum >= Convert.ToInt32(itemDT1["Quantity"]))
                                {
                                    itemDT["UnitRetail"] = itemDT1["NewPrice"];
                                    itemDT["Amount"] = Convert.ToDecimal(itemDT["UnitRetail"]) * Convert.ToDecimal(itemDT["Quantity"]);
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
                txtTotal.Text = sum.ToString("0.00");
                txtQty.Text = Qtysum.ToString();
                taxtTotal.Text = Taxsum.ToString("0.00");
                grandTotal.Text = Total.ToString("0.00");
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
                    TxtCashReturn.Text = decimal.Parse(Convert.ToDecimal(decimal.Parse(TxtCashReceive.Text) - decimal.Parse(grandTotal.Text)).ToString("0.00")).ToString("0.00");
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
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
                string onlydate = date.Substring(0, 10);
                string onlytime = date.Substring(11);
                string totalAmt = txtTotal.Text;
                //string userName = lblusername.Content.ToString();
                string tax = taxtTotal.Text;
                string grandTotalAmt = grandTotal.Text;
                string cashRec = TxtCashReceive.Text;
                string cashReturn = TxtCashReturn.Text;
                string tranid = Convert.ToInt32(lblTranid.Content).ToString();

                string transaction = "insert into Transactions(EndDate,EndTime,GrossAmount,TaxAmount,GrandAmount,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + totalAmt + "','" + tax + "','" + grandTotalAmt + "','" + username + "','" + date + "')";
                SqlCommand cmd = new SqlCommand(transaction, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                if (tenderCode == "Cash")
                {
                    string tender = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + cashRec + "','" + tranid + "','" + username + "','" + date + "')";
                    SqlCommand cmdTender = new SqlCommand(tender, con);
                    con.Open();
                    cmdTender.ExecuteNonQuery();
                    con.Close();

                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + "-" + cashReturn + "','" + tranid + "','" + username + "','" + date + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
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
                    //dataRow[11] = 0;
                    //dataRow[12] = 0;
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
                txtQty.Text = "";
                grandTotal.Text = "";
                taxtTotal.Text = "";
                lblDate.Content = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
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
                Font minifont = new Font("Arial", 5);
                Font itemfont = new Font("Arial", 6);
                Font smallfont = new Font("Arial", 8);
                Font mediumfont = new Font("Arial", 10);
                Font largefont = new Font("Arial", 12);
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

                InsertHeaderStyleItem("Name. ", "quantity", "Amount. ", Offset);

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
                DrawSimpleString(DrawnBy, minifont, Offset, 15);
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
                Font minifont = new Font("Arial", 5);

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
                Font minifont = new Font("Arial", 5);
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
                Font itemfont = new Font("Arial", 6, System.Drawing.FontStyle.Bold);

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
                            DataTable dt_1 = new DataTable();
                            dt_1 = ((DataView)JRDGrid.ItemsSource).ToTable();

                            DataTable dt1 = new DataTable();
                            foreach (DataRow row in dt_1.AsEnumerable())
                            {
                                SqlConnection con = new SqlConnection(conString);
                                string _scancode = row["ScanCode"].ToString();
                                string query1 = "select promotiongroup.ScanCode, promotiongroup.PromotionName,promotion.NewPrice,promotion.Quantity,promotion.pricereduce  from promotiongroup inner join promotion on promotiongroup.promotionname =  promotion.promotiongroup  where promotiongroup.ScanCode = @_scancode";
                                SqlCommand cmd1 = new SqlCommand(query1, con);
                                cmd1.Parameters.AddWithValue("@_scancode", _scancode);
                                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                                con.Open();
                                sda1.Fill(dt1);
                                con.Close();
                            }

                            var sum = dt1.AsEnumerable().Count();  //dt.AsEnumerable().Sum(x => Convert.ToInt32(x["Quantity"]));
                            foreach (DataRow itemDT in dt_1.AsEnumerable())
                            {
                                foreach (DataRow itemDT1 in dt1.AsEnumerable())
                                {
                                    if (itemDT["ScanCode"].ToString() == itemDT1["ScanCode"].ToString())
                                    {
                                        if ((e.Row.Item as DataRowView).Row[0].ToString() == itemDT1["ScanCode"].ToString())
                                        {
                                            if (Convert.ToInt32(el.Text) >= Convert.ToInt32(itemDT1["Quantity"]))
                                            {
                                                itemDT["UnitRetail"] = itemDT1["NewPrice"];
                                                itemDT["Quantity"] = el.Text;
                                                itemDT["Amount"] = Convert.ToDecimal(itemDT["UnitRetail"]) * Convert.ToDecimal(el.Text);
                                            }
                                            else
                                            {
                                                DataTable newDT = new DataTable();
                                                SqlConnection con = new SqlConnection(conString);
                                                string query = "select Scancode,Description,UnitRetail,@qty as quantity,UnitRetail as Amount,Department.TaxRate from Item inner join Department on item.Department=Department.Department where Scancode=@password ";
                                                SqlCommand cmd = new SqlCommand(query, con);
                                                cmd.Parameters.AddWithValue("@password", itemDT["ScanCode"].ToString());
                                                cmd.Parameters.AddWithValue("@qty", 1);
                                                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                                                con.Open();
                                                sda.Fill(newDT);
                                                con.Close();
                                                foreach (var newItem in newDT.AsEnumerable())
                                                {
                                                    if (itemDT["ScanCode"].ToString() == newItem["ScanCode"].ToString())
                                                    {
                                                        itemDT["UnitRetail"] = newItem["UnitRetail"];
                                                        itemDT["Quantity"] = newItem["quantity"];
                                                        itemDT["Amount"] = Convert.ToDecimal(itemDT["UnitRetail"]) * Convert.ToDecimal(itemDT["Quantity"]);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            JRDGrid.ItemsSource = dt_1.DefaultView;
                            TotalEvent();
                            //(e.Row.Item as DataRowView).Row[5] = Convert.ToDecimal(el.Text) * Convert.ToDecimal((e.Row.Item as DataRowView).Row[2]);
                            //TotalEvent();
                        }
                    }
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
                if (!Directory.Exists(errorpath))
                {
                    Directory.CreateDirectory(errorpath);
                }

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + "log.txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "File Name :" + errorFileName + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line + " Error Page Url:" + " " + exurl + line + "User Host IP:" + " " + hostIp + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
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

        private void JdGrid_delete_click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)JRDGrid.SelectedItem;
            dt.Rows.Remove(row.Row);
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
            catch (Exception ex) { }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                btnConform.Visibility = Visibility.Visible;
            }
            catch (Exception ex) { }
        }

        private void btnConform_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button_Click_1();
                btnConform.Visibility = Visibility.Hidden;
            }
            catch (Exception ex) { }
        }
    }
}
