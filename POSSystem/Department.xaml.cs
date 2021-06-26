using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Department.xaml
    /// </summary>
    public partial class Department : Window
    {
        DataTable dt = new DataTable();
        //string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        string conString = ConfigurationManager.ConnectionStrings["MegaPixelBizConn"].ToString();
        string username = App.Current.Properties["username"].ToString();

        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Department.cs";

        public Department()
        {
            try
            {
                InitializeComponent();
                DeptGridV();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }

        }

        private void DeptGridV()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryD = "Select DepartmentId,Department,DepartmentCode,TaxRate from department";
                SqlCommand cmd = new SqlCommand(queryD, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                DeptGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TxtDepartment.Text == "")
                    TxtDepartment.BorderBrush = System.Windows.Media.Brushes.Red;
                if (TxtDepartment_Code.Text == "")
                    TxtDepartment_Code.BorderBrush = System.Windows.Media.Brushes.Red;
                if (TxtTaxRate.Text == "")
                    TxtTaxRate.BorderBrush = System.Windows.Media.Brushes.Red;

                if (TxtDepartment.Text != "" && TxtDepartment_Code.Text != "" && TxtTaxRate.Text != "")
                {
                    string date = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
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

                            string queryI = "Insert into Department(Department,DepartmentCode,CreateOn,TaxRate,CreateBy)Values(@department,@deptCode,@time,@taxrate,@createby)";
                            SqlCommand cmdI = new SqlCommand(queryI, con);
                            cmdI.Parameters.AddWithValue("@department", TxtDepartment.Text);
                            cmdI.Parameters.AddWithValue("@deptCode", TxtDepartment_Code.Text);
                            cmdI.Parameters.AddWithValue("@time", date);
                            cmdI.Parameters.AddWithValue("@taxrate", TxtTaxRate.Text);
                            cmdI.Parameters.AddWithValue("@createby", username);
                            cmdI.ExecuteNonQuery();
                            con.Close();
                            TxtDepartment.Text = "";
                            TxtDepartment_Code.Text = "";
                            TxtTaxRate.Text = "";
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
                            string queryIU = "Update Department Set Department=@department,DepartmentCode=@deptCode,CreateOn=@time,TaxRate=@taxrate,CreateBy=@createby Where DepartmentId='" + lblDeptId.Content + "'";
                            SqlCommand cmdI = new SqlCommand(queryIU, con);
                            cmdI.Parameters.AddWithValue("@department", TxtDepartment.Text);
                            cmdI.Parameters.AddWithValue("@deptCode", TxtDepartment_Code.Text);
                            cmdI.Parameters.AddWithValue("@time", date);
                            cmdI.Parameters.AddWithValue("@taxrate", TxtTaxRate.Text);
                            cmdI.Parameters.AddWithValue("@createby", username);
                            con.Open();
                            cmdI.ExecuteNonQuery();
                            con.Close();
                            DeptGridV();
                            TxtDepartment.Text = "";
                            TxtDepartment_Code.Text = "";
                            TxtTaxRate.Text = "";
                            lblDeptId.Content = 0;
                            btnDeptSave.Content = "Save";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }

        }
        private void onEdit(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)DeptGrid.SelectedItem;
                lblDeptId.Content = row["DepartmentId"].ToString();
                TxtDepartment.Text = row["Department"].ToString();
                TxtDepartment_Code.Text = row["DepartmentCode"].ToString();
                TxtTaxRate.Text = row["TaxRate"].ToString();
                btnDeptSave.Content = "Update";
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }

        }
        private void onDelete(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void textBox_txtDeptName(object sender, TextChangedEventArgs e)
        {
            TxtDepartment.BorderBrush = System.Windows.Media.Brushes.Gray;
        }
        private void textBox_txtDeptCode(object sender, TextChangedEventArgs e)
        {
            TxtDepartment_Code.BorderBrush = System.Windows.Media.Brushes.Gray;
        }
        private void textBox_txtDeptRate(object sender, TextChangedEventArgs e)
        {
            TxtTaxRate.BorderBrush = System.Windows.Media.Brushes.Gray;
        }


        public static void SendErrorToText(Exception ex, string errorFileName)
        {
            var line = Environment.NewLine + Environment.NewLine;
            ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();

            ErrorLocation = ex.Message.ToString();
            try
            {
                string filepath = System.AppDomain.CurrentDomain.BaseDirectory;
                string errorpath = filepath + "\\ErrorFiles\\";
                if (!Directory.Exists(errorpath))
                {
                    Directory.CreateDirectory(errorpath);
                }

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + "log.txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "File Name :" + errorFileName + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line + " Error Page Url:" + " " + exurl + line + "User Host IP:" + " " + hostIp + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();

                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
