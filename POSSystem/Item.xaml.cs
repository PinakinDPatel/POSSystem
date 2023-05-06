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
            try
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
                        var last1 = code.Remove(code.Length - 1);
                        var last2 = last1.Substring(last1.Length - 1);
                        var first3 = code.Remove(code.Length - 5);
                        var first4 = code.Remove(code.Length - 4);
                        var last5 = code.Substring(code.Length - 5);
                        var second3 = last5.Remove(last5.Length - 2);
                        var last4 = code.Substring(code.Length - 4);
                        var second2 = last4.Remove(last4.Length - 2);
                        if (Convert.ToInt32(last2) == 0)
                        {
                            code = first3 + "00000" + second3;
                        }
                        else if (Convert.ToInt32(last2) == 1)
                        {
                            code = first3 + "10000" + second3;
                        }
                        else if (Convert.ToInt32(last2) == 3)
                        {
                            code = first4 + "00000" + second2;
                        }
                        else if (Convert.ToInt32(last2) == 4)
                        {
                            code = code.Remove(code.Length - 3) + "00000" + code.Substring(code.Length - 3).Remove(code.Substring(code.Length - 3).Length - 2);
                        }
                        else if (Convert.ToInt32(last2) == 2)
                        {
                            code = first3 + "20000" + second3;
                        }
                        else
                        {
                            int num = 0;
                            code = code.Remove(code.Length - 2) + num + num + num + num + last2;
                        }
                    }

                    SqlConnection con = new SqlConnection(conString);
                    string queryi = "select code,Description,Department,Manufacturer,UnitCase,CaseCost,UnitRetail,CaseDiscount,MinAge,TaxRate,Foodstamp,Payee,ItemId from Item right outer join (select '" + code + "' as code)as x on item.ScanCode=x.code";
                    SqlCommand cmdi = new SqlCommand(queryi, con);
                    SqlDataAdapter sdai = new SqlDataAdapter(cmdi);
                    DataTable dti = new DataTable();
                    sdai.Fill(dti);
                    TxtScanCode.Text = dti.Rows[0].ItemArray[0].ToString();
                    TxtDescription.Text = dti.Rows[0].ItemArray[1].ToString().Trim();
                    drpDepartment.Text = dti.Rows[0].ItemArray[2].ToString().Trim();
                    TxtMenufacturer.Text = dti.Rows[0].ItemArray[3].ToString().Trim();
                    TxtUnitCase.Text = dti.Rows[0].ItemArray[4].ToString();
                    TxtCaseCost.Text = dti.Rows[0].ItemArray[5].ToString();
                    TxtUnitRetail.Text = dti.Rows[0].ItemArray[6].ToString();
                    TxtCashDiscount.Text = dti.Rows[0].ItemArray[7].ToString();
                    TxtMinAge.Text = dti.Rows[0].ItemArray[8].ToString();
                    TxtTaxRate.Text = dti.Rows[0].ItemArray[9].ToString();
                    int foodstamp;
                    if (dti.Rows[0].ItemArray[10].ToString() == "")
                        foodstamp = 0;
                    else
                        foodstamp = Convert.ToInt32(dti.Rows[0].ItemArray[10].ToString());
                    if (foodstamp == 1)
                        TxtFoodStamp.IsChecked = true;
                    TxtPayee.Text = dti.Rows[0].ItemArray[11].ToString().Trim();
                    lblItemId.Content = dti.Rows[0].ItemArray[12].ToString().Trim();
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string date = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
                SqlConnection con = new SqlConnection(conString);
                //string queryD = "Select ScanCode from item where ScanCode=@ScanCode";
                //SqlCommand cmd = new SqlCommand(queryD, con);
                //cmd.Parameters.AddWithValue("@ScanCode", TxtScanCode.Text);
                //SqlDataAdapter sda = new SqlDataAdapter(cmd);
                //DataTable dt = new DataTable();
                //sda.Fill(dt);
                //con.Open();
                
                if (lblItemId.Content.ToString() != "")
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
                    con.Open();
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
                    con.Open();
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
                lblItemId.Content = "";
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
