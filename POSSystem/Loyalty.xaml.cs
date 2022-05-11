using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for Loyalty.xaml
    /// </summary>
    public partial class Loyalty : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        string user = App.Current.Properties["username"].ToString();
        public Loyalty()
        {
            InitializeComponent();
            DropDown();
            btnDelete.Visibility = Visibility.Hidden;
            Load();
        }

        private void Load()
        {
            ugLoyalty.Children.Clear();
            SqlConnection con = new SqlConnection(conString);
            string queryCustomer = "select * from SCLoyalty";
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
                button.Click += (sender, e) => { button_Click(sender, e, TB.Text, abc); };

                this.ugLoyalty.HorizontalAlignment = HorizontalAlignment.Center;
                this.ugLoyalty.VerticalAlignment = VerticalAlignment.Top;
                this.ugLoyalty.Columns = 6;
                this.ugLoyalty.Children.Add(button);

            }
        }

        private void button_Click(object sender, RoutedEventArgs e, string text, string abc)
        {
            SqlConnection con = new SqlConnection(conString);
            string queryCustomer = "select * from SCLoyalty where SDLoyaltyId='" + abc + "'";
            SqlCommand cmdcustomer = new SqlCommand(queryCustomer, con);
            SqlDataAdapter sdacustomer = new SqlDataAdapter(cmdcustomer);
            DataTable dt = new DataTable();
            sdacustomer.Fill(dt);
            hdnId.Content = dt.Rows[0].ItemArray[0].ToString();
            txtDiscountName.Text = dt.Rows[0].ItemArray[1].ToString();
            txtItemCount.Text = dt.Rows[0].ItemArray[2].ToString();
            txtDiscount.Text = dt.Rows[0].ItemArray[3].ToString();
            txtPriceGroup.Text = dt.Rows[0].ItemArray[4].ToString();
            btnDelete.Visibility = Visibility.Visible;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DropDown()
        {
            SqlConnection con = new SqlConnection(conString);
            string queryCustomer = "select distinct PromotionName from PromotionGroup";
            SqlCommand cmdcustomer = new SqlCommand(queryCustomer, con);
            SqlDataAdapter sdacustomer = new SqlDataAdapter(cmdcustomer);
            DataTable dt = new DataTable();
            sdacustomer.Fill(dt);
            txtPriceGroup.ItemsSource = dt.DefaultView;
            txtPriceGroup.DisplayMemberPath = "PromotionName";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtDiscountName.Text != "")
            {
                if (txtItemCount.Text != "")
                {
                    if (txtDiscount.Text != "")
                    {
                        string nowTime = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
                        SqlConnection con = new SqlConnection(conString);
                        string query = "";
                        if (hdnId.Content is null)
                            hdnId.Content = "";
                        if (hdnId.Content.ToString() == "")
                        {
                            query = "Insert into SCLoyalty(Name,Quantity,Discount,ProGroup,EnterOn,EnterBy)values('" + txtDiscountName.Text + "','" + txtItemCount.Text + "','" + txtDiscount.Text + "','" + txtPriceGroup.Text + "','" + nowTime + "','" + user + "')";
                        }
                        else
                        {
                            query = "Update SCLoyalty set Name='" + txtDiscountName.Text + "',Quantity='" + txtItemCount.Text + "',Discount='" + txtDiscount.Text + "',ProGroup='" + txtPriceGroup.Text + "',EnterOn='" + nowTime + "',EnterBy='" + user + "' where SDLoyaltyId='" + hdnId.Content + "'";
                        }
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        txtDiscount.Text = "";
                        txtDiscountName.Text = "";
                        txtItemCount.Text = "";
                        txtPriceGroup.Text = "";
                        hdnId.Content = "";
                        Load();
                        btnDelete.Visibility = Visibility.Hidden;
                    }
                    else MessageBox.Show("Please Fill Discount");
                }
                else MessageBox.Show("Please Fill Item Count");
            }
            else MessageBox.Show("Please Fill Discount Name");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

            SqlConnection con = new SqlConnection(conString);
            string query = "Delete from scloyalty where SDLoyaltyId='" + hdnId.Content + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            Load();
            txtDiscount.Text = "";
            txtDiscountName.Text = "";
            txtItemCount.Text = "";
            txtPriceGroup.Text = "";
            hdnId.Content = "";
            btnDelete.Visibility = Visibility.Hidden;
        }
    }
}
