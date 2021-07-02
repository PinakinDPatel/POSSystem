using System;
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
using System.Configuration;
using System.IO;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        //string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        string username = App.Current.Properties["username"].ToString();

        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Report.cs";

        public Report()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        // Day Close
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                shiftClose();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void shiftClose()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string tenderQ = "Update tender set shiftClose=@username Where shiftClose is null";
                SqlCommand tenderCMD = new SqlCommand(tenderQ, con);
                tenderCMD.Parameters.AddWithValue("@username", username);
                string transQ = "Update Transactions set shiftClose=@username Where shiftClose is null";
                SqlCommand transCMD = new SqlCommand(transQ, con);
                transCMD.Parameters.AddWithValue("@username", username);
                string itemQ = "Update SalesItem set shiftClose=@username Where shiftClose is null";
                SqlCommand itemCMD = new SqlCommand(itemQ, con);
                itemCMD.Parameters.AddWithValue("@username", username);
                string expQ = "Update Expence set shiftClose=@username Where shiftClose is null";
                SqlCommand expCMD = new SqlCommand(expQ, con);
                expCMD.Parameters.AddWithValue("@username", username);
                con.Open();
                tenderCMD.ExecuteNonQuery();
                transCMD.ExecuteNonQuery();
                itemCMD.ExecuteNonQuery();
                expCMD.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Dayclose()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                string tenderQ = "Update tender set DayClose=@NowDate Where DayClose is null";
                SqlCommand tenderCMD = new SqlCommand(tenderQ, con);
                tenderCMD.Parameters.AddWithValue("@NowDate", date);
                string transQ = "Update Transactions set DayClose=@Date Where DayClose is null";
                SqlCommand transCMD = new SqlCommand(transQ, con);
                transCMD.Parameters.AddWithValue("@Date", date);
                string itemQ = "Update SalesItem set DayClose=@Now Where DayClose is null";
                SqlCommand itemCMD = new SqlCommand(itemQ, con);
                itemCMD.Parameters.AddWithValue("@Now", date);
                string expeQ = "Update SalesItem set DayClose=@Now Where DayClose is null";
                SqlCommand expCMD = new SqlCommand(expeQ, con);
                expCMD.Parameters.AddWithValue("@Now", date);
                con.Open();
                tenderCMD.ExecuteNonQuery();
                transCMD.ExecuteNonQuery();
                itemCMD.ExecuteNonQuery();
                expCMD.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                shiftClose();
                Dayclose();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            StoreDetails SD = new StoreDetails();
            SD.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                Department dept = new Department();
                dept.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                Account Acc = new Account();
                Acc.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                ItemView item = new ItemView();
                item.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateUser user = new CreateUser();
                user.Show();
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
