using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using System.Windows.Media.Effects;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Drawing.Printing;
using Color = System.Drawing.Color;
using System.Windows.Data;
using System.Configuration;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Reflection;
using System.Linq;
using System.Security.Permissions;
using System.Net.Mail;
using System.Net;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        string username = App.Current.Properties["username"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Report.cs";
        private PrintDocument PrintDocument;
        private Graphics graphics;
        DataTable dtstr = new DataTable();

        public Report()
        {
            try
            {
                InitializeComponent();
                StoreDetails();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // Day Close
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryHold = "select distinct TrasactionId from Hold";
                SqlCommand cmdHold = new SqlCommand(queryHold, con);
                SqlDataAdapter sdaHold = new SqlDataAdapter(cmdHold);
                DataTable dthold = new DataTable();
                sdaHold.Fill(dthold);
                if (dthold.Rows.Count == 0)
                {
                    PrintDocument = new PrintDocument();
                    PrintDocument.PrintPage += new PrintPageEventHandler(ShiftClose);
                    PrintDocument.Print();

                    string queryshift = "select distinct shiftClose from Transactions where Dayclose is null";
                    SqlCommand cmdShift = new SqlCommand(queryshift, con);
                    SqlDataAdapter sdaShift = new SqlDataAdapter(cmdShift);
                    DataTable dtShift = new DataTable();
                    sdaShift.Fill(dtShift);
                    int i = dtShift.Rows.Count;

                    string tenderQ = "Update tender set shiftClose=@username Where shiftClose is null";
                    SqlCommand tenderCMD = new SqlCommand(tenderQ, con);
                    tenderCMD.Parameters.AddWithValue("@username", i);
                    string transQ = "Update Transactions set shiftClose=@username Where shiftClose is null";
                    SqlCommand transCMD = new SqlCommand(transQ, con);
                    transCMD.Parameters.AddWithValue("@username", i);
                    string itemQ = "Update SalesItem set shiftClose=@username Where shiftClose is null";
                    SqlCommand itemCMD = new SqlCommand(itemQ, con);
                    itemCMD.Parameters.AddWithValue("@username", i);
                    string expQ = "Update Expence set shiftClose=@username Where shiftClose is null";
                    SqlCommand expCMD = new SqlCommand(expQ, con);
                    expCMD.Parameters.AddWithValue("@username", i);
                    string RECQ = "Update Receive set shiftClose=@username Where shiftClose is null";
                    SqlCommand RECCMD = new SqlCommand(RECQ, con);
                    RECCMD.Parameters.AddWithValue("@username", i);
                    con.Open();
                    tenderCMD.ExecuteNonQuery();
                    transCMD.ExecuteNonQuery();
                    itemCMD.ExecuteNonQuery();
                    expCMD.ExecuteNonQuery();
                    RECCMD.ExecuteNonQuery();
                    con.Close();
                }
                else { MessageBox.Show("Please Clear Hold Transaction"); }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        void DrawAtStart(string text, int Offset)
        {
            try
            {
                int startX = 10;
                int startY = 5;
                Font minifont = new Font("Arial", 8);

                graphics.DrawString(text, minifont,
                         new SolidBrush(Color.Black), startX + 5, startY + Offset);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        void InsertItem(string key, string value, string value1, int Offset)
        {
            try
            {
                Font minifont = new Font("Arial", 8);
                int startX = 10;
                int startY = 5;

                graphics.DrawString(key, minifont,
                             new SolidBrush(Color.Black), startX + 5, startY + Offset);

                graphics.DrawString(value, minifont,
                         new SolidBrush(Color.Black), startX + 180, startY + Offset);
                graphics.DrawString(value1, minifont,
                        new SolidBrush(Color.Black), startX + 200, startY + Offset);
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
        }
        void InsertHeaderStyleItem(string key, string value, string value1, int Offset)
        {
            try
            {
                int startX = 10;
                int startY = 5;
                Font itemfont = new Font("Arial", 8, System.Drawing.FontStyle.Bold);

                graphics.DrawString(key, itemfont,
                             new SolidBrush(Color.Black), startX + 5, startY + Offset);

                graphics.DrawString(value, itemfont,
                         new SolidBrush(Color.Black), startX + 180, startY + Offset);
                graphics.DrawString(value1, itemfont,
                      new SolidBrush(Color.Black), startX + 200, startY + Offset);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }

        }
        void DrawLine(string text, Font font, int Offset, int xOffset)
        {
            try
            {
                int startX = 10;
                int startY = 5;
                graphics.DrawString(text, font,
                         new SolidBrush(Color.Black), startX + xOffset, startY + Offset);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        void DrawSimpleString(string text, Font font, int Offset, int xOffset)
        {
            try
            {
                int startX = 10;
                int startY = 5;
                graphics.DrawString(text, font,
                         new SolidBrush(Color.Black), startX + xOffset, startY + Offset);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void StoreDetails()
        {
            SqlConnection con = new SqlConnection(conString);
            string query = "select * from storedetails";
            SqlCommand cmdstore = new SqlCommand(query, con);
            SqlDataAdapter sdastore = new SqlDataAdapter(cmdstore);
            sdastore.Fill(dtstr);
        }
        private void ShiftClose(object sender, PrintPageEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryTrans = "select Count(tran_id)as Counts,sum(GrossAmount)as Sales,sum(TaxAmount)as Tax,sum(grandAmount)as Total,min(convert(datetime,createon))as SDate,Max(convert(datetime,createon))as EDate from transactions where ShiftClose is null and (void !=1 or void is Null)";
                SqlCommand cmdTrans = new SqlCommand(queryTrans, con);
                SqlDataAdapter sdaTrans = new SqlDataAdapter(cmdTrans);
                DataTable dtTrans = new DataTable();
                sdaTrans.Fill(dtTrans);

                string queryDept = "select Department,Sum(amt) as amt from(select Department, Sum(Amount) as amt from salesitem inner join Transactions on SalesItem.TransactionId = Transactions.Tran_id inner join item on salesitem.scancode = item.scancode where SalesItem.DayClose is null and(SalesItem.void != 1 or SalesItem.void is Null) and transactions.void is null group by Department Union all select Department,Sum(Amount) as amt from salesitem inner join Transactions on SalesItem.TransactionId = Transactions.Tran_id inner join Department on salesitem.Descripation = Department.Department where SalesItem.DayClose is null and(SalesItem.void != 1 or SalesItem.void is Null) and transactions.void is null group by Department)as x group by Department";
                SqlCommand cmdDept = new SqlCommand(queryDept, con);
                SqlDataAdapter sdaDept = new SqlDataAdapter(cmdDept);
                DataTable dtDept = new DataTable();
                sdaDept.Fill(dtDept);

                string queryTender = "select tendercode,sum(amount-coalesce(change,0))as amt from tender where ShiftClose is null group by tendercode";
                SqlCommand cmdTender = new SqlCommand(queryTender, con);
                SqlDataAdapter sdaTender = new SqlDataAdapter(cmdTender);
                DataTable dtTender = new DataTable();
                sdaTender.Fill(dtTender);

                graphics = e.Graphics;
                Font minifont = new Font("Arial", 7);
                Font itemfont = new Font("Arial", 8);
                Font smallfont = new Font("Arial", 10);
                Font mediumfont = new Font("Arial", 12);
                Font largefont = new Font("Arial", 14);
                Font headerfont = new Font("Arial", 16);
                int Offset = 10;
                int smallinc = 10, mediuminc = 12, largeinc = 15;
                graphics.DrawString("          " + dtstr.Rows[0]["StoreName"].ToString(), headerfont,
                new SolidBrush(Color.Black), 22 + 22, 22);
                Offset = Offset + largeinc + 22;

                DrawAtStart("              " + dtstr.Rows[0]["StoreAddress"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("              " + dtstr.Rows[0]["PhoneNumber"].ToString(), Offset);

                Offset = Offset + mediuminc;
                String underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 0);

                Offset = Offset + mediuminc + 10;
                DrawAtStart("Date From:       " + dtTrans.Rows[0]["SDate"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("Date To:           " + dtTrans.Rows[0]["EDate"].ToString(), Offset);
                Offset = Offset + smallinc;
                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 2);

                Offset = Offset + mediuminc + 10;

                DrawSimpleString("          Close Till", largefont, Offset, 15);

                Offset = Offset + mediuminc;
                Offset = Offset + mediuminc;
                Offset = Offset + mediuminc;
                DrawAtStart("Sales:          " + "                       " + dtTrans.Rows[0]["Sales"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("Tax:            " + "                        " + dtTrans.Rows[0]["Tax"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("Total:          " + "                        " + dtTrans.Rows[0]["Total"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("# Transactions: " + "                 " + dtTrans.Rows[0]["Counts"].ToString(), Offset);
                Offset = Offset + smallinc;
                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 2);

                Offset = Offset + largeinc;

                InsertHeaderStyleItem("Department Sales", "", "", Offset);

                Offset = Offset + largeinc;
                for (int i = 0; i < dtDept.Rows.Count; i++)
                {
                    InsertItem(dtDept.Rows[i]["Department"].ToString(), " ", dtDept.Rows[i]["amt"].ToString(), Offset);
                    Offset = Offset + largeinc;
                }

                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 2);

                Offset = Offset + largeinc;

                InsertHeaderStyleItem("Tender Sales. ", " ", " ", Offset);

                Offset = Offset + largeinc;
                for (int i = 0; i < dtTender.Rows.Count; i++)
                {
                    InsertItem(dtTender.Rows[i]["tendercode"].ToString(), " ", dtTender.Rows[i]["amt"].ToString(), Offset);
                    Offset = Offset + largeinc;
                }

                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 2);

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void DayClosePrint(object sender, PrintPageEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryTrans = "select Count(tran_id)as Counts,sum(GrossAmount)as Sales,sum(TaxAmount)as Tax,sum(grandAmount)as Total,min(convert(datetime,createon))as SDate,Max(convert(datetime,createon))as EDate from transactions where Dayclose is null and (void !=1 or void is Null)";
                SqlCommand cmdTrans = new SqlCommand(queryTrans, con);
                SqlDataAdapter sdaTrans = new SqlDataAdapter(cmdTrans);
                DataTable dtTrans = new DataTable();
                sdaTrans.Fill(dtTrans);

                string queryDept = "select Department,Sum(amt) as amt from(select Department, Sum(Amount) as amt from salesitem inner join item on salesitem.scancode = item.scancode where dayclose is null and(void != 1 or void is Null) group by Department Union all select Department,Sum(Amount) as amt from salesitem inner join Department on salesitem.Descripation = Department.Department where dayclose is null and(void != 1 or void is Null) group by Department)as x group by Department";
                SqlCommand cmdDept = new SqlCommand(queryDept, con);
                SqlDataAdapter sdaDept = new SqlDataAdapter(cmdDept);
                DataTable dtDept = new DataTable();
                sdaDept.Fill(dtDept);

                string queryTender = "select tendercode,sum(amount-coalesce(change,0))as amt from tender where dayclose is null group by tendercode";
                SqlCommand cmdTender = new SqlCommand(queryTender, con);
                SqlDataAdapter sdaTender = new SqlDataAdapter(cmdTender);
                DataTable dtTender = new DataTable();
                sdaTender.Fill(dtTender);

                graphics = e.Graphics;
                Font minifont = new Font("Arial", 7);
                Font itemfont = new Font("Arial", 8);
                Font smallfont = new Font("Arial", 10);
                Font mediumfont = new Font("Arial", 12);
                Font largefont = new Font("Arial", 14);
                Font headerfont = new Font("Arial", 16);
                int Offset = 10;
                int smallinc = 10, mediuminc = 12, largeinc = 15;
                graphics.DrawString("     " + dtstr.Rows[0]["StoreName"].ToString(), headerfont,
                new SolidBrush(Color.Black), 22 + 22, 22);
                Offset = Offset + largeinc + 22;

                DrawAtStart("            " + dtstr.Rows[0]["StoreAddress"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("            " + dtstr.Rows[0]["PhoneNumber"].ToString(), Offset);

                Offset = Offset + mediuminc;
                String underLine = "-------------------------------------";
                DrawLine(underLine, mediumfont, Offset, 0);

                Offset = Offset + mediuminc + 10;
                DrawAtStart("Date From:       " + dtTrans.Rows[0]["SDate"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("Date To:           " + dtTrans.Rows[0]["EDate"].ToString(), Offset);
                Offset = Offset + smallinc;
                underLine = "-------------------------------------";
                DrawLine(underLine, mediumfont, Offset, 2);

                Offset = Offset + mediuminc + 10;

                DrawSimpleString("            Day Close", largefont, Offset, 15);

                Offset = Offset + mediuminc;
                Offset = Offset + mediuminc;
                Offset = Offset + mediuminc;
                DrawAtStart("Sales:          " + "                       " + dtTrans.Rows[0]["Sales"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("Tax:            " + "                        " + dtTrans.Rows[0]["Tax"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("Total:          " + "                        " + dtTrans.Rows[0]["Total"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("# Transactions: " + "                 " + dtTrans.Rows[0]["Counts"].ToString(), Offset);
                Offset = Offset + smallinc;
                underLine = "-------------------------------------";
                DrawLine(underLine, mediumfont, Offset, 2);

                Offset = Offset + largeinc;

                InsertHeaderStyleItem("Department Sales", "", "", Offset);

                Offset = Offset + largeinc;
                for (int i = 0; i < dtDept.Rows.Count; i++)
                {
                    InsertItem(dtDept.Rows[i]["Department"].ToString(), " ", dtDept.Rows[i]["amt"].ToString(), Offset);
                    Offset = Offset + largeinc;
                }

                underLine = "-------------------------------------";
                DrawLine(underLine, mediumfont, Offset, 2);

                Offset = Offset + largeinc;

                InsertHeaderStyleItem("Tender Sales. ", " ", " ", Offset);

                Offset = Offset + largeinc;
                for (int i = 0; i < dtTender.Rows.Count; i++)
                {
                    InsertItem(dtTender.Rows[i]["tendercode"].ToString(), " ", dtTender.Rows[i]["amt"].ToString(), Offset);
                    Offset = Offset + largeinc;
                }

                underLine = "-------------------------------------";
                DrawLine(underLine, mediumfont, Offset, 2);

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Dayclose()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryHold = "select distinct TrasactionId from Hold";
                SqlCommand cmdHold = new SqlCommand(queryHold, con);
                SqlDataAdapter sdaHold = new SqlDataAdapter(cmdHold);
                DataTable dthold = new DataTable();
                sdaHold.Fill(dthold);
                if (dthold.Rows.Count == 0)
                {


                    PrintDocument = new PrintDocument();
                    PrintDocument.PrintPage += new PrintPageEventHandler(DayClosePrint);
                    PrintDocument.Print();
                    InsertQuery();
                    //var date = DateTime.Now.ToString("yyyy/MM/dd");
                    //string tenderQ = "Update tender set DayClose=@NowDate Where DayClose is null or DayClose=''";
                    //SqlCommand tenderCMD = new SqlCommand(tenderQ, con);
                    //tenderCMD.Parameters.AddWithValue("@NowDate", date);
                    //string transQ = "Update Transactions set DayClose=@Date Where DayClose is null or DayClose=''";
                    //SqlCommand transCMD = new SqlCommand(transQ, con);
                    //transCMD.Parameters.AddWithValue("@Date", date);
                    //string itemQ = "Update SalesItem set DayClose=@Now Where DayClose is null or DayClose=''";
                    //SqlCommand itemCMD = new SqlCommand(itemQ, con);
                    //itemCMD.Parameters.AddWithValue("@Now", date);
                    //string expeQ = "Update Expence set DayClose=@Now Where DayClose is null";
                    //SqlCommand expCMD = new SqlCommand(expeQ, con);
                    //expCMD.Parameters.AddWithValue("@Now", date);
                    //string RECQ = "Update Receive set DayClose=@Now Where DayClose is null";
                    //SqlCommand RECCMD = new SqlCommand(RECQ, con);
                    //RECCMD.Parameters.AddWithValue("@Now", date);
                    //con.Open();
                    //tenderCMD.ExecuteNonQuery();
                    //transCMD.ExecuteNonQuery();
                    //itemCMD.ExecuteNonQuery();
                    //expCMD.ExecuteNonQuery();
                    //RECCMD.ExecuteNonQuery();
                    //con.Close();
                }
                else { MessageBox.Show("Please Clear Hold Transaction"); }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void InsertQuery()
        {
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand sql_cmnd = new SqlCommand("sp_DayClose", con);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@dayclose", SqlDbType.NVarChar).Value = DateTime.Now.ToString("yyyy/MM/dd");
            sql_cmnd.Parameters.AddWithValue("@enterOn", SqlDbType.NVarChar).Value = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            sql_cmnd.Parameters.AddWithValue("@enterBy", SqlDbType.NVarChar).Value = username;
            sql_cmnd.ExecuteNonQuery();
            con.Close();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                //Button_Click(sender,e);
                Dayclose();
                //Send_Email();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Send_Email()
        {
            try
            {
                var fromAddress = new MailAddress("pspcstore@gmail.com", "From Name");
                var toAddress = new MailAddress("remotedeskop1111@gmail.com", "To Name");
                const string fromPassword = "9898926070";
                const string subject = "Subject";
                string body = "this is first line" + "\n" + "this is second line";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            StoreDetails SD = new StoreDetails();
            SD.Show();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            SalesReport Sr = new SalesReport();
            Sr.Show();
        }

        private void BtnPromotion_Click(object sender, RoutedEventArgs e)
        {
            Promotion P = new Promotion();
            P.Show();
        }

        private void BtnReceive_Click(object sender, RoutedEventArgs e)
        {
            Receive R = new Receive();
            R.Show();
        }

        private void BtnExpense_Click(object sender, RoutedEventArgs e)
        {
            Expence E = new Expence();
            E.Show();
        }
        private void BtnPurchase_Click(object sender, RoutedEventArgs e)
        {
            Purchase purchase = new Purchase();
            purchase.Show();
        }

        private void Button_Click_Shift_Close(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                Department dept = new Department();
                dept.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                Account Acc = new Account();
                Acc.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                ItemView item = new ItemView();
                item.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }

        }

        private void Category_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Category category = new Category();
                category.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }

        }

        private void UserWiseSale_Button_Click(object sender, RoutedEventArgs e)
        {
            UserWiseSaleReport UWSR = new UserWiseSaleReport();
            UWSR.Show();
        }

        private void TranDetails_Button_Click(object sender, RoutedEventArgs e)
        {
            TransactionDetails TD = new TransactionDetails();
            TD.Show();
        }

        private void Click_Inventory(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryReport inventory = new InventoryReport();
                inventory.Show();
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateUser user = new CreateUser();
                user.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }


        private void Button_Click_Reports(object sender, RoutedEventArgs e)
        {
            try
            {
                Report_.Visibility = Visibility;
                setting.Visibility = Visibility.Hidden;
                Entry.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_Setting(object sender, RoutedEventArgs e)
        {
            try
            {
                setting.Visibility = Visibility;
                Report_.Visibility = Visibility.Hidden;
                Entry.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_Entry(object sender, RoutedEventArgs e)
        {
            try
            {
                Entry.Visibility = Visibility;
                setting.Visibility = Visibility.Hidden;
                Report_.Visibility = Visibility.Hidden;
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
