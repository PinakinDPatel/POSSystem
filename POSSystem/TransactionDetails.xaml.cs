using Microsoft.Reporting.WinForms;
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
    /// Interaction logic for TransactionDetails.xaml
    /// </summary>
    public partial class TransactionDetails : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "TransactionDetails.cs";
        public TransactionDetails()
        {
            InitializeComponent();
            fromDate.SelectedDate = DateTime.Now.Date;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Datable(string fromDate,string TxtTranId)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryDG = "select Tran_id,transactions.EndDate,Transactions.EndTime,Convert(Decimal(10,2),GrossAmount)as GrossAmount,Convert(Decimal(10,2),TaxAmount)as TaxAmount,Convert(Decimal(10,2),GrandAmount)as GrandAmount,transactions.CreateBy,ScanCode,descripation,Convert(int,quantity) as quantity,Convert(Decimal(10,2),price)as price,Convert(Decimal(10,2),salesitem.amount)as amount,TenderCode,(tender.Amount-Coalesce(Change,0))as TenderAmount from transactions inner join salesitem on transactions.Tran_id=SalesItem.TransactionId and Transactions.EndDate=SalesItem.EndDate inner join Tender on transactions.Tran_id=tender.TransactionId and Transactions.EndDate=tender.EndDate where Transactions.EndDate=@fromDate and Tran_id=@tranid";
                SqlCommand cmdDG = new SqlCommand(queryDG, con);
                cmdDG.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(fromDate).ToString("yyyy/MM/dd"));
                cmdDG.Parameters.AddWithValue("@tranid", TxtTranId);
                SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
                DataTable dt = new DataTable();
                dt.Clear();
                con.Open();
                sdaDG.Fill(dt);
                con.Close();
                var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                rptTranDetails.LocalReport.ReportPath = Path + "Reports\\TranDetails.rdlc";
                rptTranDetails.LocalReport.DataSources.Clear();
                rptTranDetails.LocalReport.DataSources.Add(rds);
                rptTranDetails.RefreshReport();
                rptTranDetails.ZoomMode = ZoomMode.PageWidth;
                //transactionDG.ItemsSource = dt.DefaultView;
                //transactionDG.CanUserAddRows = false;

                //lblUser.Content = dt.Rows[0].ItemArray[6];
                //lblSale.Content = dt.Rows[0].ItemArray[3];
                //lblTax.Content = dt.Rows[0].ItemArray[4];
                //lblGrandAmount.Content = dt.Rows[0].ItemArray[5];
                //lblTenderAmount.Content = dt.Rows[0].ItemArray[13];
                //lblTenderCode.Content = dt.Rows[0].ItemArray[12];
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void btn_click_daterange(object sender, RoutedEventArgs e)
        {
        //    lblUser.Content = "";
        //    lblSale.Content = "";
        //    lblTax.Content = "";
        //    lblGrandAmount.Content = "";
        //    lblTenderAmount.Content = "";
        //    lblTenderCode.Content = "";
            Datable(fromDate.SelectedDate.Value.ToString(),TxtTranId.Text);
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
