using System;
using System.Collections.Generic;
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
    /// Interaction logic for Category.xaml
    /// </summary>
    public partial class Category : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        string username = App.Current.Properties["username"].ToString();

        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Category.cs";
        public Category()
        {
            try
            {
                InitializeComponent();
                CateGridView();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void CateGridView()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryD = "Select * from Category";
                SqlCommand cmd = new SqlCommand(queryD, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                CategoryGrid.CanUserAddRows = false;
                CategoryGrid.ItemsSource = dt.DefaultView;

                string queryC = "Select * from AddCategory";
                SqlCommand cmdC = new SqlCommand(queryC, con);
                SqlDataAdapter sdaC = new SqlDataAdapter(cmdC);
                DataTable dtC = new DataTable();
                sdaC.Fill(dtC);
                DataTable _dt = dtC.DefaultView.ToTable(true, "Category");
                foreach (DataRow _dr in _dt.Rows)
                {
                    CbCategory.Items.Add(_dr["Category"].ToString());
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSave_Click_Category(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            string descr = "", rate = "";

            string queryD = "Select Description from Item where Scancode = '" + TxtItem.Text + "'";
            SqlCommand cmd = new SqlCommand(queryD, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            if (dt.AsEnumerable().Count() != 0)
            {
                descr = dt.Rows[0]["Description"].ToString();

            }

            string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
            string queryI = "Insert into Category(Category,ScanCode,Description,CreateOn,CreateBy)Values(@Category,@ScanCode,@Description,@time,@CreateBy)";
            SqlCommand cmdI = new SqlCommand(queryI, con);
            cmdI.Parameters.AddWithValue("@Category", CbCategory.Text);
            cmdI.Parameters.AddWithValue("@ScanCode", TxtItem.Text);
            cmdI.Parameters.AddWithValue("@Description", descr);
            cmdI.Parameters.AddWithValue("@CreateBy", username);
            cmdI.Parameters.AddWithValue("@time", time);
            con.Open();
            cmdI.ExecuteNonQuery();
            con.Close();
            TxtItem.Text = "";
            CbCategory.Text = "";

            string queryC = "Select * from Category";
            SqlCommand cmdC = new SqlCommand(queryC, con);
            SqlDataAdapter sdaC = new SqlDataAdapter(cmdC);
            DataTable dtC = new DataTable();
            sdaC.Fill(dtC);
            CategoryGrid.CanUserAddRows = false;
            CategoryGrid.ItemsSource = dtC.DefaultView;
        }
        private void btnAdd_Click_Category(object sender, RoutedEventArgs e)
        {
            SaveCategory.Visibility = Visibility.Hidden;
            AddCategory.Visibility = Visibility.Visible;
        }

        private void btnAddSave_Click_Category(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            string queryI = "Insert into AddCategory(Category)Values(@Category)";
            SqlCommand cmdI = new SqlCommand(queryI, con);
            cmdI.Parameters.AddWithValue("@Category", txtCategory.Text);
            con.Open();
            cmdI.ExecuteNonQuery();
            con.Close();
            txtCategory.Text = "";
            SaveCategory.Visibility = Visibility.Visible;
            AddCategory.Visibility = Visibility.Hidden;

            CbCategory.Items.Clear();
            string queryC = "Select * from AddCategory";
            SqlCommand cmdC = new SqlCommand(queryC, con);
            SqlDataAdapter sdaC = new SqlDataAdapter(cmdC);
            DataTable dtC = new DataTable();
            sdaC.Fill(dtC);
            DataTable _dt = dtC.DefaultView.ToTable(true, "Category");
            foreach (DataRow _dr in _dt.Rows)
            {
                CbCategory.Items.Add(_dr["Category"].ToString());
            }
        }

        private void CategoryGrid_delete_click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            DataRowView row = (DataRowView)CategoryGrid.SelectedItem;

            string query = "Delete from Category where Scancode =@ScanCode and Category = @Category";
            SqlCommand cmdI = new SqlCommand(query, con);
            cmdI.Parameters.AddWithValue("@Category", row.Row.ItemArray[1]);
            cmdI.Parameters.AddWithValue("@ScanCode", row.Row.ItemArray[2]);
            con.Open();
            cmdI.ExecuteNonQuery();
            con.Close();

            string queryD = "Select * from Category";
            SqlCommand cmd = new SqlCommand(queryD, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            CategoryGrid.CanUserAddRows = false;
            CategoryGrid.ItemsSource = dt.DefaultView;
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
