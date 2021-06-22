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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for CreateUser.xaml
    /// </summary>
    public partial class CreateUser : Window
    {
        //string constring = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        string username = App.Current.Properties["username"].ToString();
        public CreateUser()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryS = "Select UserName,PassWord from UserRegi where UserName=@userName or Password=@password";
                SqlCommand cmd = new SqlCommand(queryS, con);
                cmd.Parameters.AddWithValue("@userName", txtUser.Text);
                cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("UserName Or Password Already Exist!");
                }
                else
                {
                    string time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
                    string queryI = "Insert into UserRegi(UserName,Password,CreateOn)Values(@userName,@password,@time)";
                    SqlCommand cmdI = new SqlCommand(queryI, con);
                    cmdI.Parameters.AddWithValue("@userName", txtUser.Text);
                    cmdI.Parameters.AddWithValue("@password", txtPassword.Text);
                    cmdI.Parameters.AddWithValue("@time", time);
                    con.Open();
                    cmdI.ExecuteNonQuery();
                    con.Close();
                    txtPassword.Text = "";
                    txtUser.Text = "";
                }
            }
            catch (Exception ex) { }
        }
    }
}
