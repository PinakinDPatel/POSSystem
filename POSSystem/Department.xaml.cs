using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;


namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Department.xaml
    /// </summary>
    public partial class Department : Window
    {
        string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        public Department()
        {
            InitializeComponent();
            DeptGridV();
            
        }

        private void DeptGridV()
        {
            SqlConnection con = new SqlConnection(conString);
            string queryD = "Select Department,DepartmentCode from department";
            SqlCommand cmd = new SqlCommand(queryD, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            DeptGrid.ItemsSource = dt.DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            string queryD = "Select Department from department where Department=@department";
            SqlCommand cmd = new SqlCommand(queryD, con);
            cmd.Parameters.AddWithValue("@department", TxtDepartment.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Open();
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("Department Already Exist!");
            }
            else
            {
                string time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
                string queryI = "Insert into Department(Department,CreateOn)Values(@department,@time)";
                SqlCommand cmdI = new SqlCommand(queryI, con);
                cmdI.Parameters.AddWithValue("@department", TxtDepartment.Text);
                cmdI.Parameters.AddWithValue("@time", time);
                cmdI.ExecuteNonQuery();
                con.Close();
                TxtDepartment.Text = "";
                DeptGridV();

            }
        }
    }
}
