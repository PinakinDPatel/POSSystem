using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for Promotion.xaml
    /// </summary>
    public partial class Promotion : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        string username = App.Current.Properties["username"].ToString();
        DataTable dt = new DataTable();
        private static String ErrorlineNo, Errormsg, errorFileName, extype, ErrorLocation, exurl, hostIp;
        public Promotion()
        {
            InitializeComponent();

            SqlConnection con = new SqlConnection(conString);
            string query = "Select PromotionId,Promotion.PromotionName,Promotion.Description,NewPrice,PriceReduce,Quantity,StartDate,EndDate,coalesce(Count(PromotionGroup.PromotionName),0)as ItemCount from Promotion left outer join PromotionGroup on Promotion.PromotionName=PromotionGroup.PromotionName Group by Promotion.PromotionName,Promotion.Description,StartDate,EndDate,PromotionId,NewPrice,PriceReduce,Quantity";
            SqlCommand cmdDG = new SqlCommand(query, con);
            SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
           
            sdaDG.Fill(dt);
            dgAccount.CanUserAddRows = false;
            this.dgAccount.ItemsSource = dt.AsDataView();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            CreatePromotion Cp = new CreatePromotion();
            Cp.Show();
        }

        private void onAdd(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)dgAccount.SelectedItem;
            int id = Convert.ToInt32(row["PromotionId"].ToString());
            string ProName = row["promotionName"].ToString();
            AddPromotionItem Api = new AddPromotionItem(id,ProName);
            Api.Show();
        }
        private void onEdit(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dgAccount.SelectedItem;
                string proId = row["PromotionId"].ToString();
                string ProName = row["promotionName"].ToString();
                string proDesc = row["Description"].ToString();
                string proNewPrice = row["NewPrice"].ToString();
                string proPricereduce = row["PriceReduce"].ToString();
                string Qty = row["Quantity"].ToString();
                string startdate = row["StartDate"].ToString();
                string enddate = row["Enddate"].ToString();
                CreatePromotion cp = new CreatePromotion(proId,ProName,proDesc,proNewPrice,proPricereduce,Qty,startdate,enddate);
                cp.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }

        }
        private void onDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dgAccount.SelectedItem;
                row.Delete();

                int rowsAffected;
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE from Promotion WHERE PromotionId = " + row["PromotionId"], conn);
                    cmd.Connection.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                if (rowsAffected > 0)
                    dt.AcceptChanges();
                else
                    dt.RejectChanges();
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

