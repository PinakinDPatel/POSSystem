using Microsoft.Reporting.WinForms;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for SalesReport.xaml
    /// </summary>
    public partial class SalesReport : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "SalesReport.cs";

        public SalesReport()
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
                string queryDG = "select Description,sum(cast(Amount as decimal(10,2)))as Amount,Type from dayclose where convert(date, Enddate) between convert(date, @fromDate) and convert(date, @toDate) Group by Description,Type";
                SqlCommand cmdDG = new SqlCommand(queryDG, con);
                cmdDG.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(fromDate).ToString("yyyy/MM/dd"));
                cmdDG.Parameters.AddWithValue("@toDate", Convert.ToDateTime(toDate).ToString("yyyy/MM/dd"));
                SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
                DataTable dt = new DataTable();
                sdaDG.Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    //deprtDG.ItemsSource = null;
                    //cashDG.ItemsSource = null;

                    //DataTable deptDT = (from row in dt.AsEnumerable() where row.Field<string>("Type") == "In" select row).CopyToDataTable();
                    //DataTable cashDT = (from row in dt.AsEnumerable() where row.Field<string>("Type") == "Out" select row).CopyToDataTable();

                    var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                    ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                    //ReportViewer rv1 = new ReportViewer();
                    rptUserReport.LocalReport.ReportPath = Path + "Reports\\Salesreport.rdlc";
                    rptUserReport.LocalReport.DataSources.Clear();
                    rptUserReport.LocalReport.DataSources.Add(rds);
                    rptUserReport.RefreshReport();
                    rptUserReport.ZoomMode = ZoomMode.PageWidth;


                    //string deptAmtTotal = deptDT.AsEnumerable().Sum(x => Convert.ToDecimal(x["Amount"])).ToString();
                    //string cashAmtTotal = cashDT.AsEnumerable().Sum(x => Convert.ToDecimal(x["Amount"])).ToString();
                    //string compareTotal1 = (Convert.ToDecimal(deptAmtTotal) - Convert.ToDecimal(cashAmtTotal)).ToString();

                    ////deptDT.Rows.Add("Total", deptAmtTotal);
                    ////cashDT.Rows.Add("Total", cashAmtTotal);

                    //deprtDG.ItemsSource = deptDT.DefaultView;
                    //deprtDG.CanUserAddRows = false;

                    //cashDG.ItemsSource = cashDT.DefaultView;
                    //cashDG.CanUserAddRows = false;

                    //inAmtTotal.Content = deptAmtTotal;
                    //outAmtTotal.Content = cashAmtTotal;
                    //lblShortOver.Content = compareTotal1;
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        //private void from_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var fromdate = fromDate.SelectedDate.ToString();
        //    var todate = toDate.SelectedDate.ToString();
        //    if (fromdate != "" && todate != "")
        //    {
        //        Datable(fromDate.SelectedDate.Value.ToString(), toDate.SelectedDate.Value.ToString());
        //    }

        //}

        //private void to_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var fromdate = fromDate.SelectedDate.ToString();
        //    var todate = toDate.SelectedDate.ToString();
        //    if (fromdate != "" && todate != "")
        //    {
        //        Datable(fromDate.SelectedDate.Value.ToString(), toDate.SelectedDate.Value.ToString());
        //    }
        //}

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
