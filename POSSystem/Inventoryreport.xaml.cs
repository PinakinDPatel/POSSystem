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
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class InventoryReport : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "InventoryReport.cs";



        string username = App.Current.Properties["username"].ToString();
        DataTable dtDG = new DataTable();
        public InventoryReport()
        {
            InitializeComponent();
            txtStartDate.SelectedDate = DateTime.Now.Date;
            //Inventory();
        }
        private void onclick_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSerch_Click(object sender, RoutedEventArgs e)
        {
            Inventory();
        }
        private void Inventory()
        {
            try
            {
                string _Date = txtStartDate.Text;
                SqlConnection con = new SqlConnection(conString);
                string QueryCB = "[dbo].[sp_Inventory] @enddate";
                SqlCommand cmdCB = new SqlCommand(QueryCB, con);
                cmdCB.Parameters.AddWithValue("@enddate", Convert.ToDateTime(_Date).ToString("yyyy/MM/dd"));
                SqlDataAdapter sdaCB = new SqlDataAdapter(cmdCB);
                DataTable dtCB = new DataTable();
                sdaCB.Fill(dtCB);
                var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                ReportDataSource rds = new ReportDataSource("DataSet1", dtCB);
                //ReportViewer rv1 = new ReportViewer();
                rptInventory.LocalReport.ReportPath = Path + "Reports\\Inventory.rdlc";
                rptInventory.LocalReport.DataSources.Clear();
                rptInventory.LocalReport.DataSources.Add(rds);
                rptInventory.RefreshReport();
                rptInventory.ZoomMode = ZoomMode.PageWidth;
                //dgInventory.CanUserAddRows = false;
                //dgInventory.ItemsSource = dtCB.DefaultView;
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
