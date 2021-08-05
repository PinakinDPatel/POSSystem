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
        }
        int proid = 0;
        string name = "";
        public AddPromotionItem(int id, string proname) : this()
        {
            proid = id;
            name = proname;
            lblname.Content = name;
            FillDatatable();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Promotion Pro = new Promotion();
            Pro.Show();
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                //string code = textBox1.Text.Remove(textBox1.Text.Length - 1, 1);
                SqlConnection con = new SqlConnection(conString);
                string query = "insert into PromotionGroup(PromotionName,ScanCode,Description,Enterby,EnterOn) select @proname,ScanCode, Description,@enterby,@enteron from Item where ScanCode = @password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@password", TxtBarcode.Text);
                cmd.Parameters.AddWithValue("@proname", name);
                cmd.Parameters.AddWithValue("@enteron", time);
                cmd.Parameters.AddWithValue("@enterby", username);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                TxtBarcode.Text = "";
                dt.Clear();
                FillDatatable();
            }
        }
        private void FillDatatable()
        {
            SqlConnection con = new SqlConnection(conString);
            string queryS = "Select * from PromotionGroup where PromotionName=@proname";
            SqlCommand cmd = new SqlCommand(queryS, con);
            cmd.Parameters.AddWithValue("@proname", name);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            sda.Fill(dt);
            dgPromotionItem.CanUserAddRows = false;
            this.dgPromotionItem.ItemsSource = dt.AsDataView();
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
