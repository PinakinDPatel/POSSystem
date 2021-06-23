using System.Collections.Generic;
using System.Configuration;
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
        //string constring = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        string username = App.Current.Properties["username"].ToString();
        public Item()
        {
            InitializeComponent();
            List<string> cmbList = new List<string>();
            SqlConnection con = new SqlConnection(conString);
            string queryD = "Select Department from Department";
            SqlCommand cmdD = new SqlCommand(queryD, con);
            SqlDataAdapter sdaD = new SqlDataAdapter(cmdD);
            DataTable dtD = new DataTable();
            sdaD.Fill(dtD);

            foreach (DataRow row in dtD.Rows)
            {
                cmbList.Add(row.ItemArray[0].ToString());
            }
            drpDepartment.ItemsSource = cmbList;
        }
    }
}
