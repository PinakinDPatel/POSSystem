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

using RestSharp;
using Square.Models;
using System.Management;
using POSSystem.Common;

namespace POSSystem
{
    public partial class Login : Window
    {
        string ServerName = ConfigurationManager.AppSettings["ServerName"];
        string DBName = ConfigurationManager.AppSettings["DBName"];
        string conStoreId = AppCommon.Decrypt(ConfigurationManager.AppSettings["Key"]).Split('_')[0].ToString();
        string conPOSId = AppCommon.Decrypt(ConfigurationManager.AppSettings["Key"]).Split('_')[1].ToString();
        //string userConString = "Server=184.168.194.64; Database=db_POS; User ID = pinakin; Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        string userConString = "Server=184.168.194.64; User ID = pspcstore; Password=Prem#12681#; Trusted_Connection=false;MultipleActiveResultSets=true";
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
                //try
                //{
                //    var result = await client.DevicesApi.GetDeviceCodeAsync(id: "B3Z6NAMYQSMTM");
                //}
                //catch (ApiException e)
                //{
                //    Console.WriteLine("Failed to make the request");
                //    Console.WriteLine($"Response Code: {e.ResponseCode}");
                //    Console.WriteLine($"Exception: {e.Message}");
                //}
                //var deviceCode = new DeviceCode.Builder(productType: "TERMINAL_API").Name("Squardev").LocationId("02FGFVJR8HR1N").Build();
                //var strJSONContent = "{'device_code':{'product_type':'TERMINAL_API','name':'Squardev tet','location_id':'LDPQXMKET0HRC'},'idempotency_key':'80cb629e-f251-4506-ac94-5c5787bff22f'}";
                // var client = new RestSharp.RestClient("https://connect.squareup.com");
                // var request = new RestRequest("v2/devices/codes", Method.GET);
                // request.RequestFormat = RestSharp.DataFormat.Json;
                // request.AddHeader("Accept", "application/json");
                // request.AddHeader("Authorization", "Bearer 	EAAAESEEhUZHEQnyfESFSVtrgriytdOeliataJQ4gxfYom-yae_A2PFt1AazfMTG");
                // //setHeaders(request);
                // request.AddHeader("Square-Version", "2022-11-16");
                // request.AddParameter("application/json", strJSONContent, ParameterType.RequestBody);
                // var Response = client.Execute(request);
                // var r = Response.Content;
                PasswordBox tb = new PasswordBox();
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
                TxtPassword.Password += number;
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
                TxtPassword.Password = "";
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
                //var vvvv = AppCommon.Encrypt("000000");
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                ManagementObjectCollection moc = mos.Get();
                string motherBoard = "";
                foreach (ManagementObject mo in moc)
                {
                    motherBoard = (string)mo["SerialNumber"];
                }
                //   string [] str  =motherBoard.Split('/');
                //SqlConnection con = new SqlConnection(conString);
                SqlConnection con = new SqlConnection(userConString);
                string query = "select FirstName+' '+LastName as UserName,UserRegistration.StoreId,Register_id,RoleId from UserRegistration inner join Store on UserRegistration.storeid=Store.storeid inner join Register on Store.storeid=register.storeid  where password=@password and SerialNumber=@serialnumber and UserRegistration.storeid = " + conStoreId + "";
                // string query = "select UserName,userregi.StoreId,Register_id,RoleName from userregi inner join storeDetails on userregi.storeid=storeDetails.storeid inner join register on storeDetails.storeid=register.storeid where password=@password";// and SerialNumber=@serialnumber ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@password", AppCommon.Encrypt(TxtPassword.Password));
                cmd.Parameters.AddWithValue("@serialnumber", motherBoard);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    App.Current.Properties["username"] = dt.Rows[0]["UserName"].ToString();
                    App.Current.Properties["RegisterId"] = dt.Rows[0]["Register_id"].ToString();
                    App.Current.Properties["StoreId"] = dt.Rows[0]["StoreId"].ToString();
                    App.Current.Properties["Role"] = dt.Rows[0]["RoleId"].ToString();
                    var s = App.Current.Properties["StoreId"].ToString();
                    App.Current.Properties["POSId"] = conPOSId;
                    if (App.Current.Properties["StoreId"].ToString() != "" || App.Current.Properties["StoreId"].ToString() != null)
                    {

                        //conString = "Server=" + ServerName + ";Database=" + DBName + "; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
                        App.Current.Properties["ConString"] = userConString;

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
