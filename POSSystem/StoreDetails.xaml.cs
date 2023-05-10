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
    /// <summary>
    /// Interaction logic for StoreDetails.xaml
    /// </summary>
    public partial class StoreDetails : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        DataTable dt = new DataTable();
        string username = App.Current.Properties["username"].ToString();

        public StoreDetails()
        {
            InitializeComponent();
            load();

        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //SqlConnection con = new SqlConnection(conString);
            //string QueryI = "";
            //if (lblStoreId.Content == null)
            //{
            //    QueryI = "Insert into StoreDetails(StoreName,StoreAddress,PhoneNumber,Email)Values('" + TxtName.Text + "','" + TxtAddress.Text + "','" + TxtPhone.Text + "','" + TxtEmail.Text + "')";
            //}
            //else
            //{
            //    QueryI = "Update StoreDetails Set StoreName='" + TxtName.Text + "',StoreAddress='" + TxtAddress.Text + "',PhoneNumber='" + TxtPhone.Text + "',Email='" + TxtEmail.Text + "' Where StoreId='" + lblStoreId.Content + "'";
            //}
            //SqlCommand cmdI = new SqlCommand(QueryI, con);
            //con.Open();
            //cmdI.ExecuteNonQuery();
            //con.Close();

            //load();
        }

        private void load()
        {

            SqlConnection con = new SqlConnection(conString);
            string queryD = "Select * from StoreDetails";
            SqlCommand cmd = new SqlCommand(queryD, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count != 0)
            {
                TxtName.Text = dt.Rows[0].ItemArray[1].ToString();
                TxtAddress.Text = dt.Rows[0].ItemArray[2].ToString();
                TxtPhone.Text = dt.Rows[0].ItemArray[3].ToString();
                TxtEmail.Text = dt.Rows[0].ItemArray[4].ToString();
                lblStoreId.Content = dt.Rows[0].ItemArray[0].ToString();
            }
        }
    }
}
