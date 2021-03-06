using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Windows.Input;
using System.Deployment.Application;
using System.Reflection;

namespace POSSystem
{
    public partial class Login : Window
    {
        string ServerName = ConfigurationManager.AppSettings["ServerName"];
        string DBName = ConfigurationManager.AppSettings["DBName"];
        string conString = "";
        string userConString = "Server=184.168.194.64; Database=db_POS; User ID = pinakin; Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";

        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Login.cs";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public Login()
        {
            try
            {
                TextBox tb = new TextBox();
                InitializeComponent();
                tb.KeyDown += new KeyEventHandler(OnKeyDownHandler);
                TxtPassword.Focus();

                lblVersion.Content = "POSSystem " + getRunningVersion();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private Version getRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
                TxtSignIn_Click(sender, e);
        }

        private void btnclick(object sender, RoutedEventArgs e)
        {
            try
            {
                string number = (sender as Button).Content.ToString();
                TxtPassword.Text += number;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void TxtClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtPassword.Text = "";
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void TxtSignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //SqlConnection con = new SqlConnection(conString);
                SqlConnection con = new SqlConnection(userConString);
                string query = "select * from userregi where password=@password ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@password", TxtPassword.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                //con.Open();
                sda.Fill(dt);
                //int i = cmd.ExecuteNonQuery();
                //con.Close();

                if (dt.Rows.Count > 0)
                {
                    App.Current.Properties["username"] = dt.Rows[0]["UserName"].ToString();
                    App.Current.Properties["StoreId"] = dt.Rows[0]["StoreId"].ToString();
                    App.Current.Properties["Role"] = dt.Rows[0]["RoleName"].ToString();
                    var s = App.Current.Properties["StoreId"].ToString();
                    if (App.Current.Properties["StoreId"].ToString() != "" || App.Current.Properties["StoreId"].ToString() != null)
                    {

                        conString = "Server=" + ServerName + ";Database=" + DBName + "; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
                        App.Current.Properties["ConString"] = conString;

                        MainWindow frm = new MainWindow();
                        frm.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Please Insert StoreDetails.");
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter Correct Password");
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
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
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt") + line + "File Name :" + errorFileName + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line + " Error Page Url:" + " " + exurl + line + "User Host IP:" + " " + hostIp + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt") + "-----------------");
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
