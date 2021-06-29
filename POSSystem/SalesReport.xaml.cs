using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for SalesReport.xaml
    /// </summary>
    public partial class SalesReport : Window
    {
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "SalesReport.cs";

        public SalesReport()
        {
            InitializeComponent();
            fromDate.SelectedDate = DateTime.Now.Date;
            toDate.SelectedDate = DateTime.Now.Date;
            Datable(DateTime.Now.Date.ToString(), DateTime.Now.Date.ToString());
        }

        private void Datable(string fromDate, string toDate)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryDG = "select Department,max(Price) as Amount from Department Join SalesItem On Department.Department = SalesItem.Descripation where DayClose between @fromDate and @toDate group by Department";
                SqlCommand cmdDG = new SqlCommand(queryDG, con);
                cmdDG.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd"));
                cmdDG.Parameters.AddWithValue("@toDate", Convert.ToDateTime(toDate).ToString("yyyy-MM-dd"));
                SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
                DataTable dt = new DataTable();
                con.Open();
                sdaDG.Fill(dt);
                con.Close();
                this.deprtDG.ItemsSource = dt.AsDataView();
                deprtDG.CanUserAddRows = false;

                SqlConnection con1 = new SqlConnection(conString);
                string queryDG1 = "select TenderCode,max(Tender.Amount) as Amount from Tender  where DayClose between @fromDate1 and @toDate1 group by TenderCode union all select expence.VoucherType,max(expence.Amount) as Amount from expence  where DayClose between @fromDate1 and @toDate1 group by VoucherType";
                SqlCommand cmdDG1 = new SqlCommand(queryDG1, con1);
                cmdDG1.Parameters.AddWithValue("@fromDate1", Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd"));
                cmdDG1.Parameters.AddWithValue("@toDate1", Convert.ToDateTime(toDate).ToString("yyyy-MM-dd"));
                SqlDataAdapter sdaDG1 = new SqlDataAdapter(cmdDG1);
                DataTable dt1 = new DataTable();
                con1.Open();
                sdaDG1.Fill(dt1);
                con1.Close();
                cashDG.ItemsSource = dt1.AsDataView();
                cashDG.CanUserAddRows = false;

                // For Department Total.
                SqlConnection conTotal = new SqlConnection(conString);
                string queryDGTotal = "select max(Price) as Amount,max(TaxRate) as TaxRate from Department Join SalesItem On Department.Department = SalesItem.Descripation where DayClose between @fromDate2 and @toDate2";
                SqlCommand cmdDGTotal = new SqlCommand(queryDGTotal, conTotal);
                cmdDGTotal.Parameters.AddWithValue("@fromDate2", Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd"));
                cmdDGTotal.Parameters.AddWithValue("@toDate2", Convert.ToDateTime(toDate).ToString("yyyy-MM-dd"));
                SqlDataAdapter sdaDGTotal = new SqlDataAdapter(cmdDGTotal);
                DataTable dtTotal = new DataTable();
                conTotal.Open();
                sdaDGTotal.Fill(dtTotal);
                conTotal.Close();

                // For Cash Total.
                SqlConnection conTotal1 = new SqlConnection(conString);
                string queryDGTotal1 = "select max(Tender.Amount) as Amount from Tender  where DayClose between @fromDate12 and @toDate12 union all select max(expence.Amount) as Amount from expence  where expence.DayClose between @fromDate12 and @toDate12";
                SqlCommand cmdDGTotal1 = new SqlCommand(queryDGTotal1, conTotal1);
                cmdDGTotal1.Parameters.AddWithValue("@fromDate12", Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd"));
                cmdDGTotal1.Parameters.AddWithValue("@toDate12", Convert.ToDateTime(toDate).ToString("yyyy-MM-dd"));
                SqlDataAdapter sdaDGTotal1 = new SqlDataAdapter(cmdDGTotal1);
                DataTable dtTotal1 = new DataTable();
                conTotal1.Open();
                sdaDGTotal1.Fill(dtTotal1);
                conTotal1.Close();

                amountTotal.Content = dtTotal.Rows[0]["Amount"].ToString();
                taxTotal.Content = dtTotal.Rows[0]["TaxRate"].ToString();
                var _amountTaxTotal = Convert.ToDecimal(amountTotal.Content) + Convert.ToDecimal(taxTotal.Content);
                amountTaxTotal.Content = _amountTaxTotal.ToString();
                cashamountTotal.Content = dtTotal1.Rows[0]["Amount"].ToString();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void from_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var fromdate = fromDate.SelectedDate.ToString();
            var todate = toDate.SelectedDate.ToString();
            if (fromdate != "" && todate != "")
            {
                Datable(fromDate.SelectedDate.Value.ToString(), toDate.SelectedDate.Value.ToString());
            }

        }

        private void to_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var fromdate = fromDate.SelectedDate.ToString();
            var todate = toDate.SelectedDate.ToString();
            if (fromdate != "" && todate != "")
            {
                Datable(fromDate.SelectedDate.Value.ToString(), toDate.SelectedDate.Value.ToString());
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
