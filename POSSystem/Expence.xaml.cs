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
    /// Interaction logic for Expence.xaml
    /// </summary>
    public partial class Expence : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Expence.cs";
        string username = App.Current.Properties["username"].ToString();
        DataTable dtDG = new DataTable();
        public Expence()
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
                if (cbExpence.Text == "")
                    cbExpence.BorderBrush = System.Windows.Media.Brushes.Red;
                if (txtAmount.Text == "")
                    txtAmount.BorderBrush = System.Windows.Media.Brushes.Red;

                if (txtDate.Text != "" && txtAmount.Text != "" && cbExpence.Text != "")
                {
                    SqlConnection con = new SqlConnection(conString);
                    int lbl = Convert.ToInt32(lblExpenceid.Content);
                    string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                    string query = "";
                    if (lbl == 0)
                    {
                        query = "Insert into Expence(Date,Expence,Amount,Comment,VoucherType,CreateOn,Createby)Values(@date,@expence,@amount,@comment,@voucherType,@time,@user)";
                    }
                    else
                    {
                        query = "Update Expence Set Date=@date,Expence=@expence,Amount=@amount,Comment=@comment,VoucherType=@voucherType,CreateOn=@time,CreateBy=@user Where ExpenceId='" + lblExpenceid.Content + "'";
                    }
                    SqlCommand cmdI = new SqlCommand(query, con);
                    cmdI.Parameters.AddWithValue("@date", txtDate.Text);
                    cmdI.Parameters.AddWithValue("@expence", cbExpence.Text);
                    cmdI.Parameters.AddWithValue("@amount", txtAmount.Text);
                    cmdI.Parameters.AddWithValue("@comment", txtcomment.Text);
                    cmdI.Parameters.AddWithValue("@voucherType", cbType.Text);
                    cmdI.Parameters.AddWithValue("@time", time);
                    cmdI.Parameters.AddWithValue("@user", username);
                    con.Open();
                    cmdI.ExecuteNonQuery();
                    con.Close();
                    
                    lblExpenceid.Content = "";
                    txtcomment.Text = "";
                    txtAmount.Text = "";
                    cbExpence.Text = "";
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
                string QueryCB = "Select Name from Account where Head='Expence'";
                SqlCommand cmdCB = new SqlCommand(QueryCB, con);
                SqlDataAdapter sdaCB = new SqlDataAdapter(cmdCB);
                DataTable dtCB = new DataTable();
                sdaCB.Fill(dtCB);
                cbExpence.ItemsSource = dtCB.DefaultView;
                cbExpence.DisplayMemberPath = "Name";
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
                SqlConnection con = new SqlConnection(conString);
                string queryDG = "Select * from Expence where Date='" + txtDate.Text + "'";
                SqlCommand cmdDG = new SqlCommand(queryDG, con);
                SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
                sdaDG.Fill(dtDG);
                dgExpence.CanUserAddRows = false;
                dgExpence.ItemsSource = dtDG.AsDataView();
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
                DataRowView row = (DataRowView)dgExpence.SelectedItem;
                lblExpenceid.Content = row["ExpenceId"].ToString();
                txtDate.Text = row["Date"].ToString();
                cbExpence.Text = row["Expence"].ToString();
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
                DataRowView row = (DataRowView)dgExpence.SelectedItem;
                row.Delete();

                int rowsAffected;
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE from Expence WHERE ExpenceId = " + row["ExpenceId"], conn);
                    cmd.Connection.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                if (rowsAffected > 0)
                    dtDG.AcceptChanges();
                else
                    dtDG.RejectChanges();
                lblExpenceid.Content = 0;
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
