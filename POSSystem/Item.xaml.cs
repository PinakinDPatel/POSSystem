using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;


namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Item.xaml
    /// </summary>
    public partial class Item : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        string username = App.Current.Properties["username"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Item.cs";

        public Item()
        {
            try
            {
                InitializeComponent();
                lblusername.Content = username.ToString();
                List<string> cmbList = new List<string>();
                SqlConnection con = new SqlConnection(conString);
                string queryD = "Select Department from Department";
                SqlCommand cmdD = new SqlCommand(queryD, con);
                SqlDataAdapter sdaD = new SqlDataAdapter(cmdD);
                DataTable dtD = new DataTable();
                sdaD.Fill(dtD);

                foreach (DataRow row in dtD.Rows)
                {
                    cmbList.Add(row.ItemArray[0].ToString());
                }
                drpDepartment.ItemsSource = cmbList;
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
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
                SqlConnection con = new SqlConnection(conString);
                string queryD = "Select ScanCode from item where ScanCode=@ScanCode";
                SqlCommand cmd = new SqlCommand(queryD, con);
                cmd.Parameters.AddWithValue("@ScanCode", TxtScanCode.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("ScanCode Already Exist!");
                }

                else
                {
                    string queryI = "Insert into item(ScanCode,Description,Department,Manufacturer,Payee,FoodStamp,UnitCase,CaseCost,UnitRetail,CaseDiscount,TaxRate,CreateBy,CreateOn)Values(@ScanCode,@Description,@Department,@Manufacturer,@Payee,@FoodStamp,@UnitCase,@CaseCost,@UnitRetail,@CaseDiscount,@TaxRate,@CreateBy,@CreateOn)";
                    SqlCommand cmdI = new SqlCommand(queryI, con);
                    cmdI.Parameters.AddWithValue("@ScanCode", TxtScanCode.Text);
                    cmdI.Parameters.AddWithValue("@Description", TxtDescription.Text);
                    cmdI.Parameters.AddWithValue("@Department", drpDepartment.Text);
                    cmdI.Parameters.AddWithValue("@Manufacturer", TxtMenufacturer.Text);
                    cmdI.Parameters.AddWithValue("@Payee", TxtPayee.Text);
                    cmdI.Parameters.AddWithValue("@FoodStamp", TxtFoodStamp.Text);
                    //cmdI.Parameters.AddWithValue("@MinAge", TxtMinAge.Text);
                    cmdI.Parameters.AddWithValue("@UnitCase", TxtUnitCase.Text);
                    cmdI.Parameters.AddWithValue("@CaseCost", TxtCaseCost.Text);
                    cmdI.Parameters.AddWithValue("@UnitRetail", TxtUnitRetail.Text);
                    cmdI.Parameters.AddWithValue("@CaseDiscount", TxtCashDiscount.Text);
                    cmdI.Parameters.AddWithValue("@TaxRate", TxtTaxRate.Text);
                    cmdI.Parameters.AddWithValue("@CreateBy", lblusername.Content);
                    cmdI.Parameters.AddWithValue("@CreateOn", date);
                    cmdI.ExecuteNonQuery();
                    con.Close();

                    TxtScanCode.Text = "";
                    TxtDescription.Text = "";
                    drpDepartment.Text = "";
                    TxtMenufacturer.Text = "";
                    TxtPayee.Text = "";
                    TxtFoodStamp.Text = "";
                    TxtUnitCase.Text = "";
                    TxtCaseCost.Text = "";
                    TxtUnitRetail.Text = "";
                    TxtCashDiscount.Text = "";
                    TxtTaxRate.Text = "";
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
