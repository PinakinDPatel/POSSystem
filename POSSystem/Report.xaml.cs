using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        public Report()
        {
            InitializeComponent();
        }
        // Day Close
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            string tenderQ = "Update tender set shiftClose=1, DayClose=@NowDate Where DayClose=0";
            SqlCommand tenderCMD = new SqlCommand(conString);
            tenderCMD.Parameters.AddWithValue("@time", date);
            string transQ = "Update Transactions set shiftClose=1, DayClose=@NowDate Department Where DayClose=0";
            SqlCommand transCMD = new SqlCommand(conString);
            transCMD.Parameters.AddWithValue("@time", date);
            string itemQ = "Update SalesItem set shiftClose=1, DayClose=@NowDate Department Where DayClose=0";
            SqlCommand itemCMD = new SqlCommand(conString);
            itemCMD.Parameters.AddWithValue("@time", date)

        }
    }
}
