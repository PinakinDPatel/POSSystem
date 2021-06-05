using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        string conString = "";
        public Login()
        {
            InitializeComponent();
        }

        private void btnclick(object sender, RoutedEventArgs e)
        {
            string number = (sender as Button).Content.ToString();
            TxtPassword.Text += number;
        }

        private void TxtClear_Click(object sender, RoutedEventArgs e)
        {
            TxtPassword.Text = "";
        }

        private void TxtSignIn_Click(object sender, RoutedEventArgs e)
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
                this.Hide();
                string username = dt.Rows[0]["UserName"].ToString();
                //Mainpage frm = new Mainpage(username);
                //frm.Show();
                //this.Hide();

            }
            else
            {
                MessageBox.Show("Please Enter Correct Password");
            }
        }
    }
}
