using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                TextBox tb = new TextBox();
                tb.KeyDown += new KeyEventHandler(OnKeyDownHandler);
                TxtScanCode.Focus();
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

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                var code = TxtScanCode.Text;
                var length = code.Length;
                if (length == 12)
                {
                    code = code.Remove(code.Length - 1);
                }
                if (length == 8)
                {
                    var last = code.Substring(code.Length - 1);
                    var first3 = code.Remove(code.Length - 5);
                    var last5 = code.Substring(code.Length - 5);
                    var second3 = last5.Remove(last5.Length - 2);
                    if (Convert.ToInt32(last) == 0 || Convert.ToInt32(last) == 1 || Convert.ToInt32(last) == 2 || Convert.ToInt32(last) == 3 || Convert.ToInt32(last) == 4 || Convert.ToInt32(last) == 5)
                    {
                        code = first3 + 10000 + second3;
                    }
                    else
                    {
                        int num = 0;
                        code = first3 + num + num + num + num + num + second3;
                    }
                }

                SqlConnection con = new SqlConnection(conString);
                string queryi = "select * from Item right outer join (select '" + code + "' as code)as x on item.ScanCode=x.code";
                SqlCommand cmdi = new SqlCommand(queryi, con);
                SqlDataAdapter sdai = new SqlDataAdapter(cmdi);
                DataTable dti = new DataTable();
                sdai.Fill(dti);
                TxtScanCode.Text = dti.Rows[0].ItemArray[18].ToString();
                TxtDescription.Text = dti.Rows[0].ItemArray[3].ToString();
                drpDepartment.Text = dti.Rows[0].ItemArray[4].ToString();
                TxtMenufacturer.Text = dti.Rows[0].ItemArray[5].ToString();
                TxtUnitCase.Text = dti.Rows[0].ItemArray[9].ToString();
                TxtCaseCost.Text = dti.Rows[0].ItemArray[10].ToString();
                TxtUnitRetail.Text = dti.Rows[0].ItemArray[11].ToString();
                TxtCashDiscount.Text = dti.Rows[0].ItemArray[12].ToString();
                TxtMinAge.Text = dti.Rows[0].ItemArray[8].ToString();
                TxtTaxRate.Text = dti.Rows[0].ItemArray[13].ToString();
                int foodstamp;
                if (dti.Rows[0].ItemArray[7].ToString() == "")
                    foodstamp = 0;
                else
                    foodstamp = Convert.ToInt32(dti.Rows[0].ItemArray[7].ToString());
                if (foodstamp == 1)
                    TxtFoodStamp.IsChecked = true;
                TxtPayee.Text = dti.Rows[0].ItemArray[6].ToString();
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
                string date = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
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
                    string queryI = "Update item set ScanCode=@ScanCode,Description=@Description,Department=@Department,MinAge=@MinAge,Manufacturer=@Manufacturer,Payee=@Payee,FoodStamp=@FoodStamp,UnitCase=@UnitCase,CaseCost=@CaseCost,UnitRetail=@UnitRetail,CaseDiscount=@CaseDiscount,TaxRate=@TaxRate,CreateBy=@CreateBy,CreateOn=@CreateOn where ScanCode=@ScanCode";
                    SqlCommand cmdI = new SqlCommand(queryI, con);
                    cmdI.Parameters.AddWithValue("@ScanCode", TxtScanCode.Text);
                    cmdI.Parameters.AddWithValue("@Description", TxtDescription.Text);
                    cmdI.Parameters.AddWithValue("@Department", drpDepartment.Text);
                    cmdI.Parameters.AddWithValue("@Manufacturer", TxtMenufacturer.Text);
                    cmdI.Parameters.AddWithValue("@Payee", TxtPayee.Text);
                    if (TxtFoodStamp.IsChecked == true)
                        cmdI.Parameters.AddWithValue("@FoodStamp", 1);
                    else
                        cmdI.Parameters.AddWithValue("@FoodStamp", 0);
                    cmdI.Parameters.AddWithValue("@MinAge", TxtMinAge.Text);
                    cmdI.Parameters.AddWithValue("@UnitCase", TxtUnitCase.Text);
                    cmdI.Parameters.AddWithValue("@CaseCost", TxtCaseCost.Text);
                    cmdI.Parameters.AddWithValue("@UnitRetail", TxtUnitRetail.Text);
                    cmdI.Parameters.AddWithValue("@CaseDiscount", TxtCashDiscount.Text);
                    cmdI.Parameters.AddWithValue("@TaxRate", TxtTaxRate.Text);
                    cmdI.Parameters.AddWithValue("@CreateBy", lblusername.Content);
                    cmdI.Parameters.AddWithValue("@CreateOn", date);
                    cmdI.ExecuteNonQuery();
                    con.Close();
                }

                else
                {
                    string queryI = "Insert into item(ScanCode,Description,Department,Manufacturer,Payee,FoodStamp,UnitCase,CaseCost,UnitRetail,CaseDiscount,TaxRate,CreateBy,CreateOn,MinAge)Values(@ScanCode,@Description,@Department,@Manufacturer,@Payee,@FoodStamp,@UnitCase,@CaseCost,@UnitRetail,@CaseDiscount,@TaxRate,@CreateBy,@CreateOn,@MinAge)";
                    SqlCommand cmdI = new SqlCommand(queryI, con);
                    cmdI.Parameters.AddWithValue("@ScanCode", TxtScanCode.Text);
                    cmdI.Parameters.AddWithValue("@Description", TxtDescription.Text);
                    cmdI.Parameters.AddWithValue("@Department", drpDepartment.Text);
                    cmdI.Parameters.AddWithValue("@Manufacturer", TxtMenufacturer.Text);
                    cmdI.Parameters.AddWithValue("@Payee", TxtPayee.Text);
                    if (TxtFoodStamp.IsChecked == true)
                        cmdI.Parameters.AddWithValue("@FoodStamp", 1);
                    else
                        cmdI.Parameters.AddWithValue("@FoodStamp", 0);
                    cmdI.Parameters.AddWithValue("@MinAge", TxtMinAge.Text);
                    cmdI.Parameters.AddWithValue("@UnitCase", TxtUnitCase.Text);
                    cmdI.Parameters.AddWithValue("@CaseCost", TxtCaseCost.Text);
                    cmdI.Parameters.AddWithValue("@UnitRetail", TxtUnitRetail.Text);
                    cmdI.Parameters.AddWithValue("@CaseDiscount", TxtCashDiscount.Text);
                    cmdI.Parameters.AddWithValue("@TaxRate", TxtTaxRate.Text);
                    cmdI.Parameters.AddWithValue("@CreateBy", lblusername.Content);
                    cmdI.Parameters.AddWithValue("@CreateOn", date);
                    cmdI.ExecuteNonQuery();
                    con.Close();


                }
                TxtScanCode.Text = "";
                TxtDescription.Text = "";
                drpDepartment.Text = "";
                TxtMenufacturer.Text = "";
                TxtMinAge.Text = "";
                TxtPayee.Text = "";
                TxtFoodStamp.IsChecked = false;
                TxtUnitCase.Text = "";
                TxtCaseCost.Text = "";
                TxtUnitRetail.Text = "";
                TxtCashDiscount.Text = "";
                TxtTaxRate.Text = "";
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
