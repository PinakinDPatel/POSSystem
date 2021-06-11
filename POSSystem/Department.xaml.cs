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
        DataTable dt = new DataTable();
        string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        public Department()
        {
            InitializeComponent();
            DeptGridV();

        }

        private void DeptGridV()
        {
            SqlConnection con = new SqlConnection(conString);
            string queryD = "Select DepartmentId,Department,DepartmentCode from department";
            SqlCommand cmd = new SqlCommand(queryD, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            DeptGrid.ItemsSource = dt.DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(conString);
            int lbl = Convert.ToInt32(lblDeptId.Content);
            string queryD = "Select Department,DepartmentCode from department where Department=@department or DepartmentCode=@deptCode";
            SqlCommand cmd = new SqlCommand(queryD, con);
            cmd.Parameters.AddWithValue("@department", TxtDepartment.Text);
            cmd.Parameters.AddWithValue("@deptCode", TxtDepartment_Code.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dtDept = new DataTable();
            sda.Fill(dtDept);
            con.Open();
            if (lbl == 0)
            {
                
                if (dtDept.Rows.Count > 0)
                {
                    MessageBox.Show("Department or DepartmentCode Already Exist!");
                }

                else
                {

                    string time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
                    string queryI = "Insert into Department(Department,DepartmentCode,CreateOn)Values(@department,@deptCode,@time)";
                    SqlCommand cmdI = new SqlCommand(queryI, con);
                    cmdI.Parameters.AddWithValue("@department", TxtDepartment.Text);
                    cmdI.Parameters.AddWithValue("@deptCode", TxtDepartment_Code.Text);
                    cmdI.Parameters.AddWithValue("@time", time);
                    cmdI.ExecuteNonQuery();
                    con.Close();
                    TxtDepartment.Text = "";
                    TxtDepartment_Code.Text = "";
                    DeptGridV();
                    lblDeptId.Content = 0;
                }

            }
            else
            {
                if (dtDept.Rows.Count > 0)
                {
                    MessageBox.Show("Department or DepartmentCode Already Exist!");
                }

                else
                {
                    string time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
                    string queryIU = "Update Department Set Department=@department,DepartmentCode=@deptCode,CreateOn=@time Where DepartmentId='" + lblDeptId.Content + "'";
                    SqlCommand cmdI = new SqlCommand(queryIU, con);
                    cmdI.Parameters.AddWithValue("@department", TxtDepartment.Text);
                    cmdI.Parameters.AddWithValue("@deptCode", TxtDepartment_Code.Text);
                    cmdI.Parameters.AddWithValue("@time", time);
                    con.Open();
                    cmdI.ExecuteNonQuery();
                    con.Close();
                    DeptGridV();
                    TxtDepartment.Text = "";
                    TxtDepartment_Code.Text = "";
                    lblDeptId.Content = 0;
                    btnDeptSave.Content = "Save";
                }
            }

        }
        private void onEdit(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)DeptGrid.SelectedItem;
            lblDeptId.Content = row["DepartmentId"].ToString();
            TxtDepartment.Text = row["Department"].ToString();
            TxtDepartment_Code.Text = row["DepartmentCode"].ToString();
            btnDeptSave.Content = "Update";

        }
        private void onDelete(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)DeptGrid.SelectedItem;
            row.Delete();

            int rowsAffected;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("DELETE from Department WHERE DepartmentId = " + row["DepartmentId"], conn);
                cmd.Connection.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            if (rowsAffected > 0)
                dt.AcceptChanges();
            else
                dt.RejectChanges();
            lblDeptId.Content = 0;
        }
    }
}
