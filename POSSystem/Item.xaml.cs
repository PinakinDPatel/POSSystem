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
            SqlConnection con = new SqlConnection(constring);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            string queryI = "Insert into item(ScanCode,Description,Department,Manufacturer,Payee,FoodStamp,MinAge,UnitCase,CaseCost,UnitRetail,CaseDiscount,TaxRate)Values(@ScanCode,@Description,@Department,@Manufacturer,@Payee,@FoodStamp,@MinAge,@UnitCase,@CaseCost,@UnitRetail,@CaseDiscount,@TaxRate)";
            SqlCommand cmdI = new SqlCommand(queryI, con);
            cmdI.Parameters.AddWithValue("@ScanCode", TxtScanCode.Text);
            cmdI.Parameters.AddWithValue("@Description", TxtDescription.Text);
            cmdI.Parameters.AddWithValue("@Department", drpDepartment.Text);
            cmdI.Parameters.AddWithValue("@Manufacturer", TxtMenufacturer.Text);
            cmdI.Parameters.AddWithValue("@Payee", TxtPayee.Text);
            cmdI.Parameters.AddWithValue("@FoodStamp", TxtFoodStamp.Text);
            cmdI.Parameters.AddWithValue("@MinAge", TxtMinAge.Text);
            cmdI.Parameters.AddWithValue("@UnitCase", TxtUnitCase.Text);
            cmdI.Parameters.AddWithValue("@CaseCost", TxtCaseCost.Text);
            cmdI.Parameters.AddWithValue("@UnitRetail", TxtUnitRetail.Text);
            cmdI.Parameters.AddWithValue("@CaseDiscount", TxtCashDiscount.Text);
            cmdI.Parameters.AddWithValue("@TaxRate", TxtTaxRate.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmdI);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Open();
            cmdI.ExecuteNonQuery();
            con.Close();
        }
    }
}
