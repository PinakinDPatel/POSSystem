using System;
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
            lblusername.Content = username.ToString();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
            SqlConnection con = new SqlConnection(conString);
            string queryD = "Select ScanCode from item where ScanCode=@ScanCode";
            SqlCommand cmd = new SqlCommand(queryD, con);
            cmd.Parameters.AddWithValue("@ScanCode", TxtScanCode.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Open();
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("ScanCode Already Exist!");
            }

            else
            {
                string queryI = "Insert into item(ScanCode,Description,Department,Manufacturer,Payee,FoodStamp,UnitCase,CaseCost,UnitRetail,CaseDiscount,TaxRate,CreateBy,CreateOn)Values(@ScanCode,@Description,@Department,@Manufacturer,@Payee,@FoodStamp,@UnitCase,@CaseCost,@UnitRetail,@CaseDiscount,@TaxRate,@CreateBy,@CreateOn)";
                SqlCommand cmdI = new SqlCommand(queryI, con);
                cmdI.Parameters.AddWithValue("@ScanCode", TxtScanCode.Text);
                cmdI.Parameters.AddWithValue("@Description", TxtDescription.Text);
                cmdI.Parameters.AddWithValue("@Department", drpDepartment.Text);
                cmdI.Parameters.AddWithValue("@Manufacturer", TxtMenufacturer.Text);
                cmdI.Parameters.AddWithValue("@Payee", TxtPayee.Text);
                cmdI.Parameters.AddWithValue("@FoodStamp", TxtFoodStamp.Text);
                //cmdI.Parameters.AddWithValue("@MinAge", TxtMinAge.Text);
                cmdI.Parameters.AddWithValue("@UnitCase", TxtUnitCase.Text);
                cmdI.Parameters.AddWithValue("@CaseCost", TxtCaseCost.Text);
                cmdI.Parameters.AddWithValue("@UnitRetail", TxtUnitRetail.Text);
                cmdI.Parameters.AddWithValue("@CaseDiscount", TxtCashDiscount.Text);
                cmdI.Parameters.AddWithValue("@TaxRate", TxtTaxRate.Text);
                cmdI.Parameters.AddWithValue("@CreateBy", lblusername.Content);
                cmdI.Parameters.AddWithValue("@CreateOn", date);
                cmdI.ExecuteNonQuery();
                con.Close();

                TxtScanCode.Text = "";
                TxtDescription.Text = "";
                drpDepartment.Text = "";
                TxtMenufacturer.Text = "";
                TxtPayee.Text = "";
                TxtFoodStamp.Text = "";
                TxtUnitCase.Text = "";
                TxtCaseCost.Text = "";
                TxtUnitRetail.Text = "";
                TxtCashDiscount.Text = "";
                TxtTaxRate.Text = "";
            }
        }
    }
}
