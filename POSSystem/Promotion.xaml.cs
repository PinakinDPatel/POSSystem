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
    /// Interaction logic for Promotion.xaml
    /// </summary>
    public partial class Promotion : Window
    {
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        string username = App.Current.Properties["username"].ToString();
        public Promotion()
        {
            InitializeComponent();

            SqlConnection con = new SqlConnection(conString);
            string query = "Select * from Promotion";
            SqlCommand cmdDG = new SqlCommand(query, con);
            SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
            DataTable dt = new DataTable();
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
            AddPromotionItem Api = new AddPromotionItem();
            Api.Show();
        }
    }
}
