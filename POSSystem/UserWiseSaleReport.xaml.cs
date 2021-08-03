using System;
using System.Collections.Generic;
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
    /// Interaction logic for UserWiseSaleReport.xaml
    /// </summary>
    public partial class UserWiseSaleReport : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "UserWiseSalesReport.cs";
        public UserWiseSaleReport()
        {
            InitializeComponent();
            fromDate.SelectedDate = DateTime.Now.Date;
            toDate.SelectedDate = DateTime.Now.Date;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Datable(string fromDate, string toDate)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryDG = "[dbo].[sp_UserWiseSaleReport] @startdate,@enddate";
                SqlCommand cmdDG = new SqlCommand(queryDG, con);
                cmdDG.Parameters.AddWithValue("@startdate", Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd"));
                cmdDG.Parameters.AddWithValue("@enddate", Convert.ToDateTime(toDate).ToString("yyyy-MM-dd"));
                SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
                DataTable dt = new DataTable();
                con.Open();
                sdaDG.Fill(dt);
                con.Close();

                string grossAmtTotal = dt.AsEnumerable().Sum(x => Convert.ToDecimal(x["GrossAmount"])).ToString();
                string taxAmtTotal = dt.AsEnumerable().Sum(x => Convert.ToDecimal(x["TaxAmount"])).ToString();
                string receiveAmtTotal = dt.AsEnumerable().Sum(x => Convert.ToDecimal(x["Receive"])).ToString();
                string cashAmtTotal = dt.AsEnumerable().Sum(x => Convert.ToDecimal(x["Cash"])).ToString();
                string checkAmtTotal = dt.AsEnumerable().Sum(x => Convert.ToDecimal(x["Chec"])).ToString();
                string cardAmtTotal = dt.AsEnumerable().Sum(x => Convert.ToDecimal(x["Card"])).ToString();
                string loanAmtTotal = dt.AsEnumerable().Sum(x => Convert.ToDecimal(x["Loan"])).ToString();
                string ExpAmtTotal = dt.AsEnumerable().Sum(x => Convert.ToDecimal(x["Exp"])).ToString();

                saleDG.ItemsSource = dt.DefaultView;
                saleDG.CanUserAddRows = false;

                tSale.Header = grossAmtTotal;
                tTax.Header = taxAmtTotal;
                tReceive.Header = receiveAmtTotal;
                tCash.Header = cashAmtTotal;
                tCheck.Header = checkAmtTotal;
                tCard.Header = cardAmtTotal;
                tLoan.Header = loanAmtTotal;
                tExpence.Header = ExpAmtTotal;

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void btn_click_daterange(object sender, RoutedEventArgs e)
        {
            Datable(fromDate.SelectedDate.Value.ToString(), toDate.SelectedDate.Value.ToString());
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
