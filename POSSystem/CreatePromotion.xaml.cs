using System;
using System.Collections.Generic;
using System.Configuration;
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
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        string username = App.Current.Properties["username"].ToString();
        public CreatePromotion()
        {
            InitializeComponent();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
            SqlConnection con = new SqlConnection(conString);
            string queryI = "Insert into Promotion(PromotionName,Description,NewPrice,PriceReduce,Quantity,StartDate,EndDate,EnterBy,EnterOn)Values(@promotionname,@description,@newprice,@pricereduce,@quantity,@startdate,@enddate,@enterby,@enteron)";
            SqlCommand cmdI = new SqlCommand(queryI, con);
            cmdI.Parameters.AddWithValue("@promotionname", TxtPromotionName.Text);
            cmdI.Parameters.AddWithValue("@description", TxtDescription.Text);
            cmdI.Parameters.AddWithValue("@newprice", TxtNewPrice.Text);
            cmdI.Parameters.AddWithValue("@pricereduce", TxtPriceReduce.Text);
            cmdI.Parameters.AddWithValue("@quantity", TxtQuantity.Text);
            cmdI.Parameters.AddWithValue("@startdate", DatePickerStart.Text);
            cmdI.Parameters.AddWithValue("@enddate", DatePickerEnd.Text);
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
        }
    }
}
