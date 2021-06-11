using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for ItemView.xaml
    /// </summary>
    public partial class ItemView : Window
    {
        string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        public ItemView()
        {
            InitializeComponent();
            ItemLoad();
        }

        private void ItemLoad()
        {
            SqlConnection con = new SqlConnection(conString);
            string queryD = "Select * from item";
            SqlCommand cmd = new SqlCommand(queryD, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dgitem.ItemsSource = dt.DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application appl = new Application();
            appl.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            dgitem.Visibility = Visibility.Hidden;
            btnAddItem.Visibility= Visibility.Hidden;
            btnImport.Visibility = Visibility.Hidden;
            btnClose.Visibility = Visibility.Hidden;
            btnItemsSave.Visibility = Visibility.Visible;
            dgImport.Visibility = Visibility.Visible;
        }
    }
}
