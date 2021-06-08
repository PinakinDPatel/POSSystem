﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        string constring = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        public Account()
        {
            InitializeComponent();
            Datable();
        }

        private void Datable()
        {
            SqlConnection con = new SqlConnection(constring);
            string queryDG = "Select * from Account";
            SqlCommand cmdDG = new SqlCommand(queryDG, con);
            SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
            DataTable dtDG = new DataTable();
            sdaDG.Fill(dtDG);
            this.dgAccount.ItemsSource = dtDG.AsDataView();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(constring);
            string queryS = "Select Name from Account where Name=@account";
            SqlCommand cmd = new SqlCommand(queryS, con);
            cmd.Parameters.AddWithValue("@account", txtaccount.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("UserName Or Password Already Exist!");
            }
            else
            {
                string time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
                string queryI = "Insert into Account(Name,Head,Address,Mobile,Email,CreateOn)Values(@account,@head,@address,@mobile,@email,@time)";
                SqlCommand cmdI = new SqlCommand(queryI, con);
                cmdI.Parameters.AddWithValue("@account", txtaccount.Text);
                cmdI.Parameters.AddWithValue("@head", drphead.Text);
                cmdI.Parameters.AddWithValue("@address", txtAddress.Text);
                cmdI.Parameters.AddWithValue("@mobile", txtMobile.Text);
                cmdI.Parameters.AddWithValue("@email", txtEmail.Text);
                cmdI.Parameters.AddWithValue("@time", time);
                con.Open();
                cmdI.ExecuteNonQuery();
                con.Close();
                Datable();
                txtaccount.Text = "";
                txtAddress.Text = "";
                txtEmail.Text = "";
                txtMobile.Text = "";
                drphead.Text = "";
            }
        }
    }
}
