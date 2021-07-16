using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        // string constring = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();

        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Account.cs";

        DataTable dtDG = new DataTable();
        string username =App.Current.Properties["username"].ToString();
        public Account()
        {
            InitializeComponent();
            dtDG.Reset();
            Datable();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Datable()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryDG = "Select * from Account";
                SqlCommand cmdDG = new SqlCommand(queryDG, con);
                SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
                DataTable dtDG = new DataTable();
                sdaDG.Fill(dtDG);
                dgAccount.CanUserAddRows = false;
                this.dgAccount.ItemsSource = dtDG.AsDataView();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtaccount.Text == "")
                    txtaccount.BorderBrush = System.Windows.Media.Brushes.Red;
                if (drphead.Text == "")
                    cmb1BorderHead.BorderBrush = System.Windows.Media.Brushes.Red;
                if (txtAddress.Text == "")
                    txtAddress.BorderBrush = System.Windows.Media.Brushes.Red;
                if (txtMobile.Text == "")
                    txtMobile.BorderBrush = System.Windows.Media.Brushes.Red;
                if (txtEmail.Text == "")
                    txtEmail.BorderBrush = System.Windows.Media.Brushes.Red;

                if (txtaccount.Text != "" && drphead.Text != "" && txtAddress.Text != "" && txtMobile.Text != "" && txtEmail.Text != "")
                {
                    SqlConnection con = new SqlConnection(conString);
                    int lbl = Convert.ToInt32(lblAccountId.Content);
                    string queryS = "Select Name from Account where Name=@account";
                    SqlCommand cmd = new SqlCommand(queryS, con);
                    cmd.Parameters.AddWithValue("@account", txtaccount.Text);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    con.Close();
                    if (lbl == 0)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            MessageBox.Show("Account Already Exist!");
                        }
                        else
                        {

                            string time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
                            string queryI = "Insert into Account(Name,Head,Address,Mobile,Email,CreateOn,Createby)Values(@account,@head,@address,@mobile,@email,@time,@user)";
                            SqlCommand cmdI = new SqlCommand(queryI, con);
                            cmdI.Parameters.AddWithValue("@account", txtaccount.Text);
                            cmdI.Parameters.AddWithValue("@head", drphead.Text);
                            cmdI.Parameters.AddWithValue("@address", txtAddress.Text);
                            cmdI.Parameters.AddWithValue("@mobile", txtMobile.Text);
                            cmdI.Parameters.AddWithValue("@email", txtEmail.Text);
                            cmdI.Parameters.AddWithValue("@time", time);
                            cmdI.Parameters.AddWithValue("@user", username);
                            con.Open();
                            cmdI.ExecuteNonQuery();
                            con.Close();
                            Datable();
                            txtaccount.Text = "";
                            txtAddress.Text = "";
                            txtEmail.Text = "";
                            txtMobile.Text = "";
                            drphead.Text = "";
                            lblAccountId.Content = 0;
                        }
                    }
                    else
                    {
                        if (dt.Rows.Count > 0)
                        {
                            MessageBox.Show("Account Already Exist!");
                        }
                        else
                        {
                            string time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
                            string queryIU = "Update Account Set Name=@account,Head=@head,Mobile=@mobile,Address=@address,Email=@email,CreateOn=@time,CreateBy=@user Where AccountId='" + lblAccountId.Content + "'";
                            SqlCommand cmdI = new SqlCommand(queryIU, con);
                            cmdI.Parameters.AddWithValue("@account", txtaccount.Text);
                            cmdI.Parameters.AddWithValue("@head", drphead.Text);
                            cmdI.Parameters.AddWithValue("@address", txtAddress.Text);
                            cmdI.Parameters.AddWithValue("@mobile", txtMobile.Text);
                            cmdI.Parameters.AddWithValue("@email", txtEmail.Text);
                            cmdI.Parameters.AddWithValue("@time", time);
                            cmdI.Parameters.AddWithValue("@user", username);
                            con.Open();
                            cmdI.ExecuteNonQuery();
                            con.Close();
                            Datable();
                            txtaccount.Text = "";
                            txtAddress.Text = "";
                            txtEmail.Text = "";
                            txtMobile.Text = "";
                            drphead.Text = "";
                            lblAccountId.Content = 0;
                            btnSave.Content = "Save";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void onEdit(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dgAccount.SelectedItem;
                lblAccountId.Content = row["AccountId"].ToString();
                txtaccount.Text = row["Name"].ToString();
                txtAddress.Text = row["Address"].ToString();
                txtEmail.Text = row["Email"].ToString();
                txtMobile.Text = row["Mobile"].ToString();
                drphead.Text = row["Head"].ToString();
                btnSave.Content = "Update";
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }

        }
        private void onDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dgAccount.SelectedItem;
                row.Delete();

                int rowsAffected;
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE from Account WHERE AccountId = " + row["AccountId"], conn);
                    cmd.Connection.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                if (rowsAffected > 0)
                    dtDG.AcceptChanges();
                else
                    dtDG.RejectChanges();
                lblAccountId.Content = 0;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void textBox_txtaccount(object sender, TextChangedEventArgs e)
        {
            txtaccount.BorderBrush = System.Windows.Media.Brushes.Gray;
        }
        private void textBox_txtAddress(object sender, TextChangedEventArgs e)
        {
            txtAddress.BorderBrush = System.Windows.Media.Brushes.Gray;
        }
        private void textBox_txtMobile(object sender, TextChangedEventArgs e)
        {
            txtMobile.BorderBrush = System.Windows.Media.Brushes.Gray;
        }

        private void textBox_txtEmail(object sender, TextChangedEventArgs e)
        {
            txtEmail.BorderBrush = System.Windows.Media.Brushes.Gray;
        }
        private void drphead_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmb1BorderHead.BorderBrush = System.Windows.Media.Brushes.White;
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
    }
}
