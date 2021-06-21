﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        public Report()
        {
            InitializeComponent();
        }
        // Day Close
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string tenderQ = "Update tender set shiftClose=1 Where shiftClose is null";
                SqlCommand tenderCMD = new SqlCommand(tenderQ, con);
                string transQ = "Update Transactions set shiftClose=1 Where shiftClose is null";
                SqlCommand transCMD = new SqlCommand(transQ, con);
                string itemQ = "Update SalesItem set shiftClose=1 Where shiftClose is null";
                SqlCommand itemCMD = new SqlCommand(itemQ, con);
                con.Open();
                tenderCMD.ExecuteNonQuery();
                transCMD.ExecuteNonQuery();
                itemCMD.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {

            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                string tenderQ = "Update tender set shiftClose=1, DayClose=@NowDate Where DayClose is null";
                SqlCommand tenderCMD = new SqlCommand(tenderQ, con);
                tenderCMD.Parameters.AddWithValue("@NowDate", date);
                string transQ = "Update Transactions set shiftClose=1, DayClose=@Date Where DayClose is null";
                SqlCommand transCMD = new SqlCommand(transQ, con);
                transCMD.Parameters.AddWithValue("@Date", date);
                string itemQ = "Update SalesItem set shiftClose=1, DayClose=@Now Where DayClose is null";
                SqlCommand itemCMD = new SqlCommand(itemQ, con);
                itemCMD.Parameters.AddWithValue("@Now", date);
                con.Open();
                tenderCMD.ExecuteNonQuery();
                transCMD.ExecuteNonQuery();
                itemCMD.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
