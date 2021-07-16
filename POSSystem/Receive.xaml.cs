using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Receive.xaml
    /// </summary>
    public partial class Receive : Window
    {
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Receive.cs";
        string username = App.Current.Properties["username"].ToString();
        DataTable dtDG = new DataTable();
        public Receive()
        {
            InitializeComponent();
            txtDate.SelectedDate = DateTime.Now;
            ComboBox();
            Datable();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtDate.Text == "")
                    txtDate.BorderBrush = System.Windows.Media.Brushes.Red;
                if (cbReceive.Text == "")
                    cbReceive.BorderBrush = System.Windows.Media.Brushes.Red;
                if (txtAmount.Text == "")
                    txtAmount.BorderBrush = System.Windows.Media.Brushes.Red;

                if (txtDate.Text != "" && txtAmount.Text != "" && cbReceive.Text != "")
                {
                    SqlConnection con = new SqlConnection(conString);
                    int lbl = Convert.ToInt32(lblReceiveid.Content);
                    string vt = "Receive";
                    string time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                    string query = "";
                    if (lbl == 0)
                    {
                        query = "Insert into Receive(Date,Receive,Amount,Comment,VoucherType,CreateOn,Createby)Values(@date,@expence,@amount,@comment,@voucherType,@time,@user)";
                    }
                    else
                    {
                        query = "Update Receive Set Date=@date,Receive=@expence,Amount=@amount,Comment=@comment,VoucherType=@voucherType,CreateOn=@time,CreateBy=@user Where ReceiveId='" + lblReceiveid.Content + "'";
                    }
                    SqlCommand cmdI = new SqlCommand(query, con);
                    cmdI.Parameters.AddWithValue("@date", txtDate.Text);
                    cmdI.Parameters.AddWithValue("@expence", cbReceive.Text);
                    cmdI.Parameters.AddWithValue("@amount", txtAmount.Text);
                    cmdI.Parameters.AddWithValue("@comment", txtcomment.Text);
                    cmdI.Parameters.AddWithValue("@voucherType", vt);
                    cmdI.Parameters.AddWithValue("@time", time);
                    cmdI.Parameters.AddWithValue("@user", username);
                    con.Open();
                    cmdI.ExecuteNonQuery();
                    con.Close();

                    lblReceiveid.Content = "";
                    txtcomment.Text = "";
                    txtAmount.Text = "";
                    cbReceive.Text = "";
                    btnSave.Content = "Save";
                    Datable();

                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void ComboBox()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string QueryCB = "Select Name from Account where Head='Income'";
                SqlCommand cmdCB = new SqlCommand(QueryCB, con);
                SqlDataAdapter sdaCB = new SqlDataAdapter(cmdCB);
                DataTable dtCB = new DataTable();
                sdaCB.Fill(dtCB);
                cbReceive.ItemsSource = dtCB.DefaultView;
                cbReceive.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Datable()
        {
            try
            {
                dtDG.Reset();
                var date = Convert.ToDateTime(txtDate.SelectedDate).ToString("yyyy-MM-dd");
                SqlConnection con = new SqlConnection(conString);
                string queryDG = "Select * from Receive where Date='" + date + "'";
                SqlCommand cmdDG = new SqlCommand(queryDG, con);
                SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
                sdaDG.Fill(dtDG);
                dgReceive.CanUserAddRows = false;
                dgReceive.ItemsSource = dtDG.AsDataView();
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
                DataRowView row = (DataRowView)dgReceive.SelectedItem;
                lblReceiveid.Content = row["ReceiveId"].ToString();
                txtDate.Text = row["Date"].ToString();
                cbReceive.Text = row["Expence"].ToString();
                txtAmount.Text = row["Amount"].ToString();
                txtcomment.Text = row["Comment"].ToString();
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
                DataRowView row = (DataRowView)dgReceive.SelectedItem;
                row.Delete();

                int rowsAffected;
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE from Receive WHERE ReceiveId = " + row["ReceiveId"], conn);
                    cmd.Connection.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                if (rowsAffected > 0)
                    dtDG.AcceptChanges();
                else
                    dtDG.RejectChanges();
                lblReceiveid.Content = 0;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
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
