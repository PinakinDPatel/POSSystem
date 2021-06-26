using System;
using System.Collections.Generic;
using System.Data;
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
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for DayClose.xaml
    /// </summary>
    public partial class DayClose : Window
    {
        DataTable dt = new DataTable();
        //string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();

        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "DayClose.cs";

        public DayClose()
        {

            try
            {
                InitializeComponent();

                SqlConnection con = new SqlConnection(conString);
                string queryDCR = "select tendercode,sum(cast(Amount as decimal(10,2))) as Amount from tender group by tendercode union all select 'Tax',sum(cast(TaxAmount as decimal(10, 2))) from transactions  union all select 'GrossAmount',sum(cast(GrossAmount as decimal(10, 2))) from transactions  ";
                SqlCommand cmd = new SqlCommand(queryDCR, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);


                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    if (dt.Rows[i].ItemArray[0].ToString() == "Cash")
                    {
                        TxtCash.Content = dt.Rows[i].ItemArray[1].ToString();
                    }
                    if (dt.Rows[i].ItemArray[0].ToString() == "Check")
                    {
                        TxtCheck.Content = dt.Rows[i].ItemArray[1].ToString();
                    }
                    if (dt.Rows[i].ItemArray[0].ToString() == "Card")
                    {
                        TxtCard.Content = dt.Rows[i].ItemArray[1].ToString();
                    }
                    if (dt.Rows[i].ItemArray[0].ToString() == "Tax")
                    {
                        TxtTax.Content = dt.Rows[i].ItemArray[1].ToString();
                    }
                    if (dt.Rows[i].ItemArray[0].ToString() == "GrossAmount")
                    {
                        TxtTaxable.Content = dt.Rows[i].ItemArray[1].ToString();
                    }
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

