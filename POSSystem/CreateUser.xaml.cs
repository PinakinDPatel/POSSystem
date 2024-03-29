﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for CreateUser.xaml
    /// </summary>
    public partial class CreateUser : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        string username = App.Current.Properties["username"].ToString();
        string StoreId = App.Current.Properties["StoreId"].ToString();

        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "CreateUser.cs";
        DataTable dtUser = new DataTable();
        public CreateUser()
        {
            try
            {
                if (StoreId != "" || StoreId != null)
                {
                    InitializeComponent();
                    Loaduser();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Loaduser()
        {
            SqlConnection con = new SqlConnection(conString);
            dtUser.Clear();
            string queryS = "Select * from UserRegi";
            SqlCommand cmd = new SqlCommand(queryS, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dtUser);
            dgUser.CanUserAddRows = false;
            dgUser.ItemsSource = dtUser.AsDataView();
        }

        private void onEdit(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dgUser.SelectedItem;
                hdnid.Content = row["UserRegiId"].ToString();
                txtUser.Text = row["UserName"].ToString();
                txtPassword.Text = row["PassWord"].ToString();
                txtRole.Text = row["RoleName"].ToString();
                if (txtRole.Text == "")
                    txtRole.SelectedIndex = 0;
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
                DataRowView row = (DataRowView)dgUser.SelectedItem;
                row.Delete();

                int rowsAffected;
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE from UserRegi WHERE UserRegiId = " + row["UserRegiId"], conn);
                    cmd.Connection.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                if (rowsAffected > 0)
                    dtUser.AcceptChanges();
                else
                    dtUser.RejectChanges();
                hdnid.Content = 0;
                Loaduser();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryS = "Select UserName,PassWord from UserRegi where UserName=@userName or Password=@password";
                SqlCommand cmd = new SqlCommand(queryS, con);
                cmd.Parameters.AddWithValue("@userName", txtUser.Text);
                cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                if (hdnid.Content is null)
                    hdnid.Content = "";
                string queryI = "";

                if (hdnid.Content.ToString() == "")
                {
                    if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("UserName Or Password Already Exist!");
                    }
                    else
                    {
                        queryI = "Insert into UserRegi(UserName,Password,CreateOn,StoreId,RoleName)Values(@userName,@password,@time,@storeId,@roleName)";
                        SqlCommand cmdI = new SqlCommand(queryI, con);
                        cmdI.Parameters.AddWithValue("@userName", txtUser.Text);
                        cmdI.Parameters.AddWithValue("@password", txtPassword.Text);
                        cmdI.Parameters.AddWithValue("@time", time);
                        cmdI.Parameters.AddWithValue("@storeId", StoreId);
                        cmdI.Parameters.AddWithValue("@roleName", txtRole.Text);
                        con.Open();
                        cmdI.ExecuteNonQuery();
                        con.Close();
                        txtPassword.Text = "";
                        txtUser.Text = "";
                        Loaduser();
                        txtRole.SelectedIndex = 0;
                    }
                }
                else
                {
                    queryI = "Update UserRegi set UserName=@userName,Password=@password,CreateOn=@time,StoreId=@storeId,RoleName=@roleName where UserRegiId='" + hdnid.Content + "'";

                    SqlCommand cmdI = new SqlCommand(queryI, con);
                    cmdI.Parameters.AddWithValue("@userName", txtUser.Text);
                    cmdI.Parameters.AddWithValue("@password", txtPassword.Text);
                    cmdI.Parameters.AddWithValue("@time", time);
                    cmdI.Parameters.AddWithValue("@storeId", StoreId);
                    cmdI.Parameters.AddWithValue("@roleName", txtRole.Text);
                    con.Open();
                    cmdI.ExecuteNonQuery();
                    con.Close();
                    txtPassword.Text = "";
                    txtUser.Text = "";
                    Loaduser();
                    txtRole.SelectedIndex = 0;
                    btnSave.Content = "Save";
                    hdnid.Content = "";
                }
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
