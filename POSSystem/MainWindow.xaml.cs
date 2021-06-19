using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using System.Windows.Media.Effects;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Collections.Generic;

namespace POSSystem
{
    public partial class MainWindow : Window
    {
        string tenderCode = "";
        DataTable dt = new DataTable();
        string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";

        //string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\PSPCStore\POSSystem\POSSystem\Database1.mdf;Integrated Security=True";
        public MainWindow()
        {
            try
            {

                InitializeComponent();
                lblDate.Content = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");

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

                string queryS = "Select Department from Department";
                SqlCommand cmd1 = new SqlCommand(queryS, con);
                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                DataTable dtdep = new DataTable();
                sda1.Fill(dtdep);
                con.Open();
                cmd1.ExecuteNonQuery();
                con.Close();

                //Shadow Effect Of Button
                DropShadowEffect newDropShadowEffect = new DropShadowEffect();
                newDropShadowEffect.BlurRadius = 5;
                newDropShadowEffect.Direction = 100;
                newDropShadowEffect.Opacity = 95;
                newDropShadowEffect.ShadowDepth = 5;

                for (int i = 0; i < dtdep.Rows.Count; ++i)
                {
                    Button button = new Button()
                    {
                        Content = dtdep.Rows[i].ItemArray[0],
                        Tag = i
                    };
                    button.Foreground = new SolidColorBrush(Colors.White);
                    button.Background = new SolidColorBrush(Colors.DarkRed);
                    button.Effect = new DropShadowEffect()
                    { Color = Colors.BlueViolet };
                    button.Margin = new Thickness(5, 5, 5, 5);

                    // button.Effect.add
                    button.Click += new RoutedEventHandler(button_Click);

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
            }
            catch (Exception ex) { }
        }
        public MainWindow(string username) : this()
        {
            try
            {
                if (username != null)
                {
                    lblusername.Content = username;
                }
                else
                {
                    Login login = new Login();
                    login.Show();
                    this.Close();
                }
            }
            catch (Exception ex) { }
        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btnContent = sender as Button;
                lblDepartment.Content = btnContent.Content;
                TxtBxStackPanel2.Visibility = Visibility.Visible;
                sp21.Visibility = Visibility.Hidden;
            }
            catch (Exception ex) { }
        }
        private void Button_Click_Go_Back(object sender, RoutedEventArgs e)
        {
            try
            {
                sp21.Visibility = Visibility.Visible;
                TxtBxStackPanel2.Visibility = Visibility.Hidden;
            }
            catch (Exception ex) { }
        }
        private void Button_Click_Sale_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = lblDepartment.Content.ToString();
                dr[2] = Convert.ToDecimal(txtDeptAmt.Text).ToString("0.00");
                dr[3] = 9;
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
            catch (Exception ex) { }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter || e.Key == Key.Tab)
                {
                    SqlConnection con = new SqlConnection(conString);
                    string query = "select Scancode,Description,UnitRetail,@qty as quantity,UnitRetail as Amount,TaxRate from item where Scancode=@password ";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@password", textBox1.Text);
                    cmd.Parameters.AddWithValue("@qty", 1);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    con.Open();
                    sda.Fill(dt);
                    con.Close();
                    JRDGrid.ItemsSource = dt.DefaultView;
                    JRDGrid.Items.Refresh();
                    TotalEvent();
                    textBox1.Text = "";
                }
            }
            catch (Exception ex) { }
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
            catch (Exception ex) { }
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
                //Taxsum = sum * Taxsum / 100;
                //Taxsum += decimal.Parse(Convert.ToDecimal(decimal.Parse(dr.ItemArray[3].ToString()) * decimal.Parse(dr.ItemArray[5].ToString()) / 100).ToString());
                Total = sum + Taxsum;
                txtTotal.Text = sum.ToString("0.00");
                txtQty.Text = Qtysum.ToString();
                taxtTotal.Text = Taxsum.ToString("0.00");
                grandTotal.Text = Total.ToString("0.00");
            }
            catch (Exception ex) { }
        }

        private void TxtCashReceive_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Tab || e.Key == Key.Enter)
                {
                    TxtCashReturn.Text = decimal.Parse(Convert.ToDecimal(decimal.Parse(TxtCashReceive.Text) - decimal.Parse(grandTotal.Text)).ToString("0.00")).ToString("0.00");
                }
            }
            catch (Exception ex) { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
                string onlydate = date.Substring(0, 10);
                string onlytime = date.Substring(11);
                string totalAmt = txtTotal.Text;
                string userName = lblusername.Content.ToString();
                string tax = taxtTotal.Text;
                string grandTotalAmt = grandTotal.Text;
                string cashRec = TxtCashReceive.Text;
                string cashReturn = TxtCashReturn.Text;
                string tranid = Convert.ToInt32(lblTranid.Content).ToString();

                string transaction = "insert into Transactions(EndDate,EndTime,GrossAmount,TaxAmount,GrandAmount,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + totalAmt + "','" + tax + "','" + grandTotalAmt + "','" + userName + "','" + date + "')";
                SqlCommand cmd = new SqlCommand(transaction, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                if (tenderCode == "Cash")
                {
                    string tender = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + cashRec + "','" + tranid + "','" + userName + "','" + date + "')";
                    SqlCommand cmdTender = new SqlCommand(tender, con);
                    con.Open();
                    cmdTender.ExecuteNonQuery();
                    con.Close();

                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + "-" + cashReturn + "','" + tranid + "','" + userName + "','" + date + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                else if (tenderCode == "Card")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + userName + "','" + date + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                else if (tenderCode == "Customer")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,AccountName,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + cbcustomer.Text + "','" + userName + "','" + date + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CheckNo,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + TxtCheck.Text + "','" + userName + "','" + date + "')";
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
                    dataRow[9] = userName;
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
                objbulk.ColumnMappings.Add("TransactionId", "TransactionId");
                objbulk.ColumnMappings.Add("CreateBy", "CreateBy");
                objbulk.ColumnMappings.Add("CreateOn", "CreateOn");
                con.Open();
                objbulk.WriteToServer(dt);
                con.Close();

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
            }
            catch (Exception ex) { }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                lblusername.Content = "";
                Login login = new Login();
                this.Close();
                login.Show();
            }
            catch (Exception ex) { }
        }
    }
}
