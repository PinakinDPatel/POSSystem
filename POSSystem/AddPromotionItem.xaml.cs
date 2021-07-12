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
    /// Interaction logic for AddPromotionItem.xaml
    /// </summary>
    public partial class AddPromotionItem : Window
    {
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        string username = App.Current.Properties["username"].ToString();
        public AddPromotionItem()
        {
            InitializeComponent();

            TextBox tb = new TextBox();
            tb.KeyDown += new KeyEventHandler(OnKeyDownHandler);
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                //string code = textBox1.Text.Remove(textBox1.Text.Length - 1, 1);
                SqlConnection con = new SqlConnection(conString);
                string query = "insert into PromotionGroup(ScanCode,Description) select ScanCode, Description from Item where ScanCode = @password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@password", TxtBarcode.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                TxtBarcode.Text = "";
                FillDatatable();
            }
        }
        private void FillDatatable()
        {
            SqlConnection con = new SqlConnection(conString);
            string queryS = "Select * from PromotionGroup";
            SqlCommand cmd = new SqlCommand(queryS, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dgPromotionItem.CanUserAddRows = false;
            this.dgPromotionItem.ItemsSource = dt.AsDataView();
        }
    }
}
