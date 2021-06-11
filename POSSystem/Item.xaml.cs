using System.Data;
using System.Data.SqlClient;
using System.Windows;


namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Item.xaml
    /// </summary>
    public partial class Item : Window
    {
        string constring = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        public Item()
        {
            InitializeComponent();

            SqlConnection con = new SqlConnection(constring);
            string queryD = "Select Department from Department";
            SqlCommand cmdD = new SqlCommand(queryD, con);
            SqlDataAdapter sdaD = new SqlDataAdapter(cmdD);
            DataTable dtD = new DataTable();
            sdaD.Fill(dtD);

            drpDepartment.ItemsSource = dtD.DefaultView;

            
        }
    }
}
