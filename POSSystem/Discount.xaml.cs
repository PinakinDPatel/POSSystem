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
using System.Windows.Shapes;

namespace POSSystem
{
    public partial class Discount : Window
    {
        string conString = "Server=184.168.194.64; Database=db_POS; User ID = pinakin; Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        string username = "";
        public Discount()
        {
            InitializeComponent();
            DropDown();
            Load();
        }

        private void DropDown()
        {
            SqlConnection con = new SqlConnection(conString);
            string queryCustomer = "select distinct PromotionName from PromotionGroup";
            SqlCommand cmdcustomer = new SqlCommand(queryCustomer, con);
            SqlDataAdapter sdacustomer = new SqlDataAdapter(cmdcustomer);
            DataTable dt = new DataTable();
            sdacustomer.Fill(dt);
            cbItemGroup.ItemsSource = dt.DefaultView;
            cbItemGroup.DisplayMemberPath = "PromotionName";
        }

        private void Load()
        {
            ugDiscount.Children.Clear();
            SqlConnection con = new SqlConnection(conString);
            string queryCustomer = "select * from Promotion";
            SqlCommand cmdcustomer = new SqlCommand(queryCustomer, con);
            SqlDataAdapter sdacustomer = new SqlDataAdapter(cmdcustomer);
            DataTable dt = new DataTable();
            sdacustomer.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                Button button = new Button();
                TextBlock TB = new TextBlock();
                TB.Text = dt.Rows[i].ItemArray[1].ToString();
                TB.TextAlignment = TextAlignment.Center;
                TB.TextWrapping = TextWrapping.Wrap;
                button.Content = TB;

                button.Width = 175;
                button.Height = 100;
                button.Margin = new Thickness(8);
                string abc = dt.Rows[i].ItemArray[0].ToString();
                button.Click += (sender, e) => { button1_Click(sender, e, TB.Text, abc); };

                this.ugDiscount.HorizontalAlignment = HorizontalAlignment.Center;
                this.ugDiscount.VerticalAlignment = VerticalAlignment.Top;
                this.ugDiscount.Columns = 7;
                this.ugDiscount.Children.Add(button);

            }
        }

        private void button1_Click(object sender, RoutedEventArgs e, string text, string abc)
        {
            gridForm.Visibility = Visibility.Visible;
            grid1View.Visibility = Visibility.Hidden;
            SqlConnection con = new SqlConnection(conString);
            string queryCustomer = "select * from Promotion where PromotionId='" + abc + "'";
            SqlCommand cmdcustomer = new SqlCommand(queryCustomer, con);
            SqlDataAdapter sdacustomer = new SqlDataAdapter(cmdcustomer);
            DataTable dt = new DataTable();
            sdacustomer.Fill(dt);
            hdnID.Content = dt.Rows[0].ItemArray[0].ToString();
            TxtPromotionName.Text = dt.Rows[0].ItemArray[1].ToString();
            TxtDescription.Text = dt.Rows[0].ItemArray[2].ToString();
            cbItemGroup.Text = dt.Rows[0].ItemArray[3].ToString();
            TxtQuantity.Text = dt.Rows[0].ItemArray[6].ToString();
            TxtNewPrice.Text = dt.Rows[0].ItemArray[4].ToString();
            txtDiscount.Text = dt.Rows[0].ItemArray[13].ToString();
            datePickerStart.Text = dt.Rows[0].ItemArray[7].ToString();
            datePickerEnd.Text = dt.Rows[0].ItemArray[8].ToString();
            cbDiscountBy.Text = dt.Rows[0].ItemArray[14].ToString();

            btnsave.Visibility = Visibility.Hidden;
            gridupdate.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
        }

        private void Btnsave_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            string queryCustomer = "select PromotionId from promotion where DiscountBY='" + cbDiscountBy.Text + "' and PromotionGroup='" + cbItemGroup.Text + "'";
            SqlCommand cmdcustomer = new SqlCommand(queryCustomer, con);
            SqlDataAdapter sdacustomer = new SqlDataAdapter(cmdcustomer);
            DataTable dt = new DataTable();
            sdacustomer.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                if (hdnID.Content is null)
                    hdnID.Content = "";

                if (hdnID.Content.ToString() == "")
                {
                    string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt").Replace("-", "/");
                    string queryI = "Insert into Promotion(PromotionGroup,PromotionName,Description,Quantity,NewPrice,Discount,StartDate,EndDate,DiscountBy,EnterBy,EnterOn)Values(@PromotionGroup,@promotionname,@description,@quantity,@newprice,@discount,@startdate,@enddate,@discountBy,@enterby,@enteron)";
                    SqlCommand cmdI = new SqlCommand(queryI, con);
                    cmdI.Parameters.AddWithValue("@promotionname", TxtPromotionName.Text);
                    cmdI.Parameters.AddWithValue("@description", TxtDescription.Text);
                    cmdI.Parameters.AddWithValue("@newprice", TxtNewPrice.Text);
                    cmdI.Parameters.AddWithValue("@discount", txtDiscount.Text);
                    cmdI.Parameters.AddWithValue("@quantity", TxtQuantity.Text);
                    cmdI.Parameters.AddWithValue("@startdate", datePickerStart.Text.Replace("-", "/"));
                    cmdI.Parameters.AddWithValue("@enddate", datePickerEnd.Text.Replace("-", "/"));
                    cmdI.Parameters.AddWithValue("@PromotionGroup", cbItemGroup.Text);
                    cmdI.Parameters.AddWithValue("@enterby", username);
                    cmdI.Parameters.AddWithValue("@enteron", time);
                    cmdI.Parameters.AddWithValue("@discountBy", cbDiscountBy.Text);
                    con.Open();
                    cmdI.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt").Replace("-", "/");
                    string queryI = "Update Promotion set PromotionGroup=@PromotionGroup, PromotionName=@promotionname,Description=@description,NewPrice=@newprice,Discount=@discount,Quantity=@quantity,StartDate=@startdate,EndDate=@enddate,DiscountBy=@discountBy,EnterBy=@enterby,EnterOn=@enteron where PromotionId =@id";
                    SqlCommand cmdI = new SqlCommand(queryI, con);
                    cmdI.Parameters.AddWithValue("@promotionname", TxtPromotionName.Text);
                    cmdI.Parameters.AddWithValue("@description", TxtDescription.Text);
                    cmdI.Parameters.AddWithValue("@newprice", TxtNewPrice.Text);
                    cmdI.Parameters.AddWithValue("@discount", txtDiscount.Text);
                    cmdI.Parameters.AddWithValue("@quantity", TxtQuantity.Text);
                    cmdI.Parameters.AddWithValue("@startdate", datePickerStart.Text.Replace("-", "/"));
                    cmdI.Parameters.AddWithValue("@enddate", datePickerEnd.Text.Replace("-", "/"));
                    cmdI.Parameters.AddWithValue("@PromotionGroup", cbItemGroup.Text);
                    cmdI.Parameters.AddWithValue("@enterby", username);
                    cmdI.Parameters.AddWithValue("@enteron", time);
                    cmdI.Parameters.AddWithValue("@discountBy", cbDiscountBy.Text);
                    cmdI.Parameters.AddWithValue("@id", hdnID.Content);
                    con.Open();
                    cmdI.ExecuteNonQuery();
                    con.Close();
                }
                Clear();
                gridForm.Visibility = Visibility.Hidden;
                grid1View.Visibility = Visibility.Visible;
                Load();
            }
            else
                MessageBox.Show("You Can Not Choose Same ItemGroup and Discount Offer By");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            string queryI = "Delete from Promotion where PromotionId =@id";
            SqlCommand cmdI = new SqlCommand(queryI, con);
            cmdI.Parameters.AddWithValue("@id", hdnID.Content);
            con.Open();
            cmdI.ExecuteNonQuery();
            con.Close();
            Clear();
            Load();
        }

        private void BtnAddDiscount_Click(object sender, RoutedEventArgs e)
        {
            gridForm.Visibility = Visibility.Visible;
            grid1View.Visibility = Visibility.Hidden;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            gridForm.Visibility = Visibility.Hidden;
            grid1View.Visibility = Visibility.Visible;
            Clear();
        }

        private void Clear()
        {
            hdnID.Content = "";
            cbItemGroup.Text = "";
            cbDiscountBy.Text = "";
            TxtQuantity.Text = "";
            TxtPromotionName.Text = "";
            TxtDescription.Text = "";
            TxtNewPrice.Text = "";
            txtDiscount.Text = "";
            datePickerStart.Text = "";
            datePickerEnd.Text = "";
            gridupdate.Visibility = Visibility.Hidden;
            btnsave.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
