using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for AddPromotionItem.xaml
    /// </summary>
    public partial class AddPromotionItem : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        string username = App.Current.Properties["username"].ToString();
        DataTable dt = new DataTable();
        public AddPromotionItem()
        {
            InitializeComponent();
            TextBox tb = new TextBox();
            tb.KeyDown += new KeyEventHandler(OnKeyDownHandler);
            LoadProGroup();
        }

        private void LoadProGroup()
        {
            try
            {
                dt.Clear();
                ugProGroup.Children.Clear();
                SqlConnection con = new SqlConnection(conString);
                string queryCustomer = "select promotiongroupid,PromotionName from PromotionGroup union all select Groupid, ProGroup from ProGroup where ProGroup not in(Select promotionName from PromotionGroup)";
                SqlCommand cmdcustomer = new SqlCommand(queryCustomer, con);
                SqlDataAdapter sdacustomer = new SqlDataAdapter(cmdcustomer);
                //DataTable dt = new DataTable();
                sdacustomer.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; ++i)
                {
                    Button button = new Button();
                    TextBlock TB = new TextBlock();
                    TB.Text = dt.Rows[i].ItemArray[1].ToString();
                    TB.TextAlignment = TextAlignment.Center;
                    TB.TextWrapping = TextWrapping.Wrap;
                    button.Content = TB;

                    button.Width = 175;
                    button.Height = 100;
                    button.Margin = new Thickness(8);
                    string abc = dt.Rows[i].ItemArray[0].ToString();
                    button.Click += (sender, e) => { button1_Click(sender, e, TB.Text, abc); };

                    this.ugProGroup.HorizontalAlignment = HorizontalAlignment.Center;
                    this.ugProGroup.VerticalAlignment = VerticalAlignment.Top;
                    this.ugProGroup.Columns = 7;
                    this.ugProGroup.Children.Add(button);

                }
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
        }

        private void button1_Click(object sender, RoutedEventArgs e, string text, string abc)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryPGI = "select * from PromotionGroup where PromotionName='" + text + "'";
                SqlCommand cmdPGI = new SqlCommand(queryPGI, con);
                SqlDataAdapter sdaPGI = new SqlDataAdapter(cmdPGI);
                DataTable dtPGI = new DataTable();
                sdaPGI.Fill(dtPGI);
                if (dtPGI.Rows.Count > 0)
                {
                    gProGroup.Visibility = Visibility.Hidden;
                    gAddItem.Visibility = Visibility.Visible;
                    dgPromotionItem.CanUserAddRows = false;
                    this.dgPromotionItem.ItemsSource = dtPGI.AsDataView();
                    lblname.Content = text;
                }
                else
                {
                    Add.Visibility = Visibility.Hidden;
                    AddForm.Visibility = Visibility.Visible;
                    txtName.Text = text;
                    lblProGroupId.Content = abc;
                    btnSave.Content = "Update";
                    btndelete.Visibility = Visibility.Visible;
                    AddItem.Visibility = Visibility.Visible;
                }
            }

            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        
        public AddPromotionItem(int id, string proname) : this()
        {
            //proid = id;
            //name = proname;
            //lblname.Content = name;
            //FillDatatable();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            //Promotion Pro = new Promotion();
            //Pro.Show();
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                var code = TxtBarcode.Text;
                var length = code.Length;
                if (length == 12)
                {
                    code = code.Remove(code.Length - 1);
                }
                if (length == 8)
                {
                    var last1 = code.Remove(code.Length - 1);
                    var last2 = last1.Substring(last1.Length - 1);
                    var first3 = code.Remove(code.Length - 5);
                    var first4 = code.Remove(code.Length - 4);
                    var last5 = code.Substring(code.Length - 5);
                    var second3 = last5.Remove(last5.Length - 2);
                    var last4 = code.Substring(code.Length - 4);
                    var second2 = last4.Remove(last4.Length - 2);
                    if (Convert.ToInt32(last2) == 0)
                    {
                        code = first3 + "00000" + second3;
                    }
                    else if (Convert.ToInt32(last2) == 1)
                    {
                        code = first3 + "10000" + second3;
                    }
                    else if (Convert.ToInt32(last2) == 3)
                    {
                        code = first4 + "00000" + second2;
                    }
                    else if (Convert.ToInt32(last2) == 4)
                    {
                        code = code.Remove(code.Length - 3) + "00000" + code.Substring(code.Length - 3).Remove(code.Substring(code.Length - 3).Length - 2);
                    }
                    else if (Convert.ToInt32(last2) == 2)
                    {
                        code = first3 + "20000" + second3;
                    }
                    else
                    {
                        int num = 0;
                        code = code.Remove(code.Length - 2) + num + num + num + num + last2;
                    }
                }
                TxtBarcode.Text = code;
                string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                //string code = textBox1.Text.Remove(textBox1.Text.Length - 1, 1);
                SqlConnection con = new SqlConnection(conString);
                string query = "insert into PromotionGroup(PromotionName,ScanCode,Description,Enterby,EnterOn) select @proname,ScanCode, Description,@enterby,@enteron from Item where ScanCode = @password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@password", TxtBarcode.Text);
                cmd.Parameters.AddWithValue("@proname", lblname.Content);
                cmd.Parameters.AddWithValue("@enteron", time);
                cmd.Parameters.AddWithValue("@enterby", username);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                TxtBarcode.Text = "";
                FillDatatable();
            }
        }
        private void FillDatatable()
        {
            SqlConnection con = new SqlConnection(conString);
            string querypg = "Select * from PromotionGroup where PromotionName=@proname";
            SqlCommand cmdpg = new SqlCommand(querypg, con);
            cmdpg.Parameters.AddWithValue("@proname", lblname.Content);
            SqlDataAdapter sdapg = new SqlDataAdapter(cmdpg);
            DataTable dtpg = new DataTable();
            sdapg.Fill(dtpg);
            dgPromotionItem.CanUserAddRows = false;
            this.dgPromotionItem.ItemsSource = dtpg.AsDataView();
        }

        private void onDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dgPromotionItem.SelectedItem;
                row.Delete();

                int rowsAffected;
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE from PromotionGroup WHERE PromotionGroupId = " + row["PromotionGroupId"], conn);
                    cmd.Connection.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                if (rowsAffected > 0)
                    dt.AcceptChanges();
                else
                    dt.RejectChanges();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private static String ErrorlineNo, Errormsg, errorFileName, extype, ErrorLocation, exurl, hostIp;

        private void Goback_Click(object sender, RoutedEventArgs e)
        {
            gProGroup.Visibility = Visibility.Visible;
            gAddItem.Visibility = Visibility.Hidden;
            Add.Visibility = Visibility.Visible;
            AddForm.Visibility = Visibility.Hidden;
            txtName.Text = "";
            lblProGroupId.Content = "";
            btnSave.Content = "Save";
            btndelete.Visibility = Visibility.Hidden;
            AddItem.Visibility = Visibility.Hidden;
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            string queryPGI = "select * from PromotionGroup where PromotionName='" + txtName.Text + "'";
            SqlCommand cmdPGI = new SqlCommand(queryPGI, con);
            SqlDataAdapter sdaPGI = new SqlDataAdapter(cmdPGI);
            DataTable dtPGI = new DataTable();
            dtPGI.Clear();
            sdaPGI.Fill(dtPGI);
            gProGroup.Visibility = Visibility.Hidden;
            gAddItem.Visibility = Visibility.Visible;
            dgPromotionItem.CanUserAddRows = false;
            this.dgPromotionItem.ItemsSource = dtPGI.AsDataView();
            lblname.Content = txtName.Text;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand("Delete From ProGroup Where GroupId='" + lblProGroupId.Content + "'", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                LoadProGroup();
                txtName.Text = "";
                lblProGroupId.Content = "";
                btndelete.Visibility = Visibility.Hidden;
                btnSave.Content = "Save";
                AddItem.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                var results = from myRow in dt.AsEnumerable()
                              where myRow.Field<string>("ProGroup") == txtName.Text
                              select myRow;
                int i = results.Count();
                if (results.Count() == 0)
                {
                    string query = "";
                    string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                    SqlConnection con = new SqlConnection(conString);
                    if (lblProGroupId.Content is null)
                        lblProGroupId.Content = "";

                    if (lblProGroupId.Content.ToString() == "")
                        query = "insert into ProGroup(ProGroup,Createby,EnterOn)Values(@password,@enterby,@enteron)";
                    else
                        query = "Update ProGroup Set ProGroup=@password,Createby=@enterby,EnterOn=@enteron Where GroupId='" + lblProGroupId.Content + "'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@password", txtName.Text);
                    cmd.Parameters.AddWithValue("@enteron", time);
                    cmd.Parameters.AddWithValue("@enterby", username);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    txtName.Text = "";
                    lblProGroupId.Content = "";
                    btnSave.Content = "Save";
                    btndelete.Visibility = Visibility.Hidden;
                    AddItem.Visibility = Visibility.Hidden;
                    LoadProGroup();
                }
                else { MessageBox.Show("Group is Already Exists !"); }
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            try
            {
                Add.Visibility = Visibility.Hidden;
                AddForm.Visibility = Visibility.Visible;
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
        }

        public static void SendErrorToText(Exception ex, string errorFileName)
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
    }
}
