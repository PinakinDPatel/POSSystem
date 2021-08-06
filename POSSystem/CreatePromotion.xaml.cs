using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
    public partial class CreatePromotion : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        string username = App.Current.Properties["username"].ToString();
        public CreatePromotion()
        {
            InitializeComponent();
        }
        string proid = "";
        public CreatePromotion(string proId, string ProName, string proDesc, string proNewPrice, string proPricereduce, string Qty, string startdate, string enddate) : this()
        {
            
            proid = proId;
            TxtPromotionName.Text = ProName;
            TxtDescription.Text =proDesc;
            TxtNewPrice.Text = proNewPrice;
            TxtPriceReduce.Text = proPricereduce;
            TxtQuantity.Text =Qty;
            DatePickerStart.Text = startdate;
            DatePickerEnd.Text =enddate;
            btnsave.Content = "UpDate";
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (proid == "")
            {
                string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt").Replace("-","/");
                SqlConnection con = new SqlConnection(conString);
                string queryI = "Insert into Promotion(PromotionName,Description,NewPrice,PriceReduce,Quantity,StartDate,EndDate,ScanData,EnterBy,EnterOn)Values(@promotionname,@description,@newprice,@pricereduce,@quantity,@startdate,@enddate,@scandata,@enterby,@enteron)";
                SqlCommand cmdI = new SqlCommand(queryI, con);
                cmdI.Parameters.AddWithValue("@promotionname", TxtPromotionName.Text);
                cmdI.Parameters.AddWithValue("@description", TxtDescription.Text);
                cmdI.Parameters.AddWithValue("@newprice", TxtNewPrice.Text);
                cmdI.Parameters.AddWithValue("@pricereduce", TxtPriceReduce.Text);
                cmdI.Parameters.AddWithValue("@quantity", TxtQuantity.Text);
                cmdI.Parameters.AddWithValue("@startdate", DatePickerStart.Text.Replace("-", "/"));
                cmdI.Parameters.AddWithValue("@enddate", DatePickerEnd.Text.Replace("-", "/"));
                cmdI.Parameters.AddWithValue("@scandata", txtScanData.Text);
                cmdI.Parameters.AddWithValue("@enterby", username);
                cmdI.Parameters.AddWithValue("@enteron", time);
                con.Open();
                cmdI.ExecuteNonQuery();
                con.Close();
                TxtPromotionName.Text = "";
                TxtDescription.Text = "";
                TxtNewPrice.Text = "";
                TxtPriceReduce.Text = "";
                TxtQuantity.Text = "";
                btnsave.Content = "Save";
                Promotion pro = new Promotion();
                pro.Show();
                this.Close();
            }
            else
            {
                string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt").Replace("-", "/");
                SqlConnection con = new SqlConnection(conString);
                string queryI = "Update Promotion set PromotionName=@promotionname,Description=@description,NewPrice=@newprice,PriceReduce=@pricereduce,Quantity=@quantity,StartDate=@startdate,EndDate=@enddate,ScanData=@scandata,EnterBy=@enterby,EnterOn=@enteron where PromotionId =@id";
                SqlCommand cmdI = new SqlCommand(queryI, con);
                cmdI.Parameters.AddWithValue("@promotionname", TxtPromotionName.Text);
                cmdI.Parameters.AddWithValue("@description", TxtDescription.Text);
                cmdI.Parameters.AddWithValue("@newprice", TxtNewPrice.Text);
                cmdI.Parameters.AddWithValue("@pricereduce", TxtPriceReduce.Text);
                cmdI.Parameters.AddWithValue("@quantity", TxtQuantity.Text);
                cmdI.Parameters.AddWithValue("@startdate", DatePickerStart.Text.Replace("-", "/"));
                cmdI.Parameters.AddWithValue("@enddate", DatePickerEnd.Text.Replace("-", "/"));
                cmdI.Parameters.AddWithValue("@scandata", txtScanData.Text);
                cmdI.Parameters.AddWithValue("@enterby", username);
                cmdI.Parameters.AddWithValue("@enteron", time);
                cmdI.Parameters.AddWithValue("@id", proid);
                con.Open();
                cmdI.ExecuteNonQuery();
                con.Close();
                Promotion pro = new Promotion();
                pro.Show();
                this.Close();
            }
            

        }
    }
}
