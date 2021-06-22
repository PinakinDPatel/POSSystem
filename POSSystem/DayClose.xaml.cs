using System;
using System.Collections.Generic;
using System.Data;
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
using System.Data.SqlClient;
using System.Configuration;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for DayClose.xaml
    /// </summary>
    public partial class DayClose : Window
    {
        DataTable dt = new DataTable();
        //string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        public DayClose()
        {
            InitializeComponent();

            SqlConnection con = new SqlConnection(conString);
            string queryDCR = "select tendercode,sum(cast(Amount as decimal(10,2))) as Amount from tender group by tendercode union all select 'Tax',sum(cast(TaxAmount as decimal(10, 2))) from transactions  union all select 'GrossAmount',sum(cast(GrossAmount as decimal(10, 2))) from transactions  ";
            SqlCommand cmd = new SqlCommand(queryDCR, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);


            for(int i=0;i<dt.Rows.Count; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {

                    if (dt.Rows[i].ItemArray[j].ToString() == "Cash")
                    {
                        TxtCash.Content = dt.Rows[i].ItemArray[1].ToString();
                    }
                    if (dt.Rows[i].ItemArray[j].ToString() == "Check")
                    {
                        TxtCheck.Content = dt.Rows[i].ItemArray[1].ToString();
                    }
                    if (dt.Rows[i].ItemArray[j].ToString() == "Card")
                    {
                        TxtCard.Content = dt.Rows[i].ItemArray[1].ToString();
                    }
                    if (dt.Rows[i].ItemArray[j].ToString() == "Tax")
                    {
                        TxtTax.Content = dt.Rows[i].ItemArray[1].ToString();
                    }
                    if (dt.Rows[i].ItemArray[j].ToString() == "GrossAmount")
                    {
                        TxtTaxable.Content = dt.Rows[i].ItemArray[1].ToString();
                    }
                }
            }

        }
    }
}
