using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Navigation;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        public Login()
        {
            InitializeComponent();
        }

        private void btnclick(object sender, RoutedEventArgs e)
        {
            try
            {
                string number = (sender as Button).Content.ToString();
                TxtPassword.Text += number;
            }
            catch (Exception ex) { }
        }

        private void TxtClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtPassword.Text = "";
            }
            catch (Exception ex) { }
        }

        private void TxtSignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string query = "select * from userregi where password=@password ";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@password", TxtPassword.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();

                if (dt.Rows.Count > 0)
                {

                    string username = dt.Rows[0]["UserName"].ToString();
                    MainWindow frm = new MainWindow(username);
                    Department Dept = new Department(username);
                    Dept.Show();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Please Enter Correct Password");
                }
            }
            catch (Exception ex) { }
        }
    }
}
