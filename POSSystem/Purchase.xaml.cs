using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Web;
using ExcelDataReader;
using SqlBulkTools;
using Newtonsoft.Json;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Purchase.xaml
    /// </summary>
    public partial class Purchase : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        string username = App.Current.Properties["username"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Purchase.cs";
        DataTable dt = new DataTable();
        DataTable dtr = new DataTable();
        public Purchase()
        {
            InitializeComponent();
            txtDate.SelectedDate = DateTime.Now;
            ComboBox();
            Datable();
            dtr.Columns.Add("ScanCode");
            dtr.Columns.Add("Quantity");
            dtr.Columns.Add("Cost");
            dtr.Columns.Add("Retail");
            TextBox tb = new TextBox();
            tb.KeyDown += new KeyEventHandler(OnKeyDownHandler);
        }
        //vendor Dropdown
        private void ComboBox()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string QueryCB = "Select Name from Account where Head='Vendor'";
                SqlCommand cmdCB = new SqlCommand(QueryCB, con);
                SqlDataAdapter sdaCB = new SqlDataAdapter(cmdCB);
                DataTable dtCB = new DataTable();
                sdaCB.Fill(dtCB);
                cbVendor.ItemsSource = dtCB.DefaultView;
                cbVendor.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        //load purchase
        private void Datable()
        {
            try
            {
                dt.Reset();
                var date = Convert.ToDateTime(txtDate.SelectedDate).ToString("yyyy/MM/dd");
                SqlConnection con = new SqlConnection(conString);
                string queryDG = "Select * from Purchase where Date='" + date + "'";
                SqlCommand cmdDG = new SqlCommand(queryDG, con);
                SqlDataAdapter sdaDG = new SqlDataAdapter(cmdDG);
                sdaDG.Fill(dt);
                dgPurchase.CanUserAddRows = false;
                dgPurchase.ItemsSource = dt.AsDataView();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        //Window close
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Click_Goback(object sender, RoutedEventArgs e)
        {
            btnGoBack.Visibility = Visibility.Hidden;
            add.Visibility = Visibility.Visible;
            dgPurchase.Visibility = Visibility.Visible;
            dgRetail.Visibility = Visibility.Hidden;
            grupload.Visibility = Visibility.Hidden;
            addpurchase.Visibility = Visibility.Hidden;
            addRetailentry.Visibility = Visibility.Hidden;
        }
        private void TxtDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Datable();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DataRow newrow = dtr.NewRow();
            newrow["ScanCode"] = txtScancode.Text;
            newrow["Quantity"] = txtQty.Text;
            newrow["Cost"] = txtCost.Text;
            newrow["Retail"] = txtRAmount.Text;
            dtr.Rows.Add(newrow);
            dgRetail.CanUserAddRows = false;
            dgRetail.ItemsSource = dtr.AsDataView();
        }

        //Purchase Save
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtDate.Text == "")
                    txtDate.BorderBrush = System.Windows.Media.Brushes.Red;
                if (cbVendor.Text == "")
                    cbVendor.BorderBrush = System.Windows.Media.Brushes.Red;
                if (txtInvoiceno.Text == "")
                    txtInvoiceno.BorderBrush = System.Windows.Media.Brushes.Red;
                if (txtAmount.Text == "")
                    txtAmount.BorderBrush = System.Windows.Media.Brushes.Red;
                if (cbType.Text == "")
                    cbType.BorderBrush = System.Windows.Media.Brushes.Red;
                if (txtDate.Text != "" && txtInvoiceno.Text != "" && cbVendor.Text != "" && txtAmount.Text != "" && cbType.Text != "")
                {
                    var purchaseId = lblPurchaseId.Content;
                    SqlConnection con = new SqlConnection(conString);
                    string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                    string query = "";
                    decimal amount = Convert.ToDecimal(txtAmount.Text);
                    if (lblPurchaseId.Content.ToString() == "")
                    {
                        query = "Insert into Purchase(Date,InvoiceNo,Vendor,Type,Amount,CreateOn,Createby)Values(@date,@invono,@vendor,@type,@amount,@time,@user)";
                    }
                    else
                    {
                        int id = Convert.ToInt32(lblPurchaseId.Content);
                        query = "Update Purchase Set Date=@date,InvoiceNo=@invono,Vendor=@vendor,Type=@type,Amount=@amount,CreateOn=@time,CreateBy=@user Where PurchaseId='" + id + "'";

                    }
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@date", txtDate.Text);
                    cmd.Parameters.AddWithValue("@invono", txtInvoiceno.Text);
                    cmd.Parameters.AddWithValue("@vendor", cbVendor.Text);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@type", cbType.Text);
                    cmd.Parameters.AddWithValue("@time", time);
                    cmd.Parameters.AddWithValue("@user", username);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    txtAmount.Text = "";
                    txtInvoiceno.Text = "";
                    cbVendor.Text = "";
                    cbType.Text = "";
                    addpurchase.Visibility = Visibility.Hidden;
                    add.Visibility = Visibility.Visible;
                    btnGoBack.Visibility = Visibility.Hidden;
                    Datable();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_Import(object sender, RoutedEventArgs e)
        {
            grupload.Visibility = Visibility.Visible;
            addRetailentry.Visibility = Visibility.Hidden;
        }
        private void Button_Click_Save_ImportFile(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                DataGrid dg = new DataGrid();
                int selectedIndex = dgImport.SelectedIndex;
                dt = ((DataView)dgImport.ItemsSource).ToTable();

                if (selectedIndex >= 0)
                {
                    DataRow row = dt.Rows[selectedIndex];
                    dt.Rows.Remove(row);
                    dt.AcceptChanges();
                }

                for (int i = 0; i < strList2.Count; i++)
                {
                    if (i != 0)
                    {
                        dt = new DataTable();
                        dt = ((DataView)dg.ItemsSource).ToTable();
                    }
                    string getCol = strList1[i];
                    string getVal = strList2[i];
                    dt.Columns[getCol].ColumnName = getVal;
                    dg.ItemsSource = dt.DefaultView;
                }

                trimData(dt);
                dt.Columns.Add("UserId");
                dt.Columns.Add("EnterOn");
                dt.Columns.Add("PurchaseId");
                foreach (var itemdt in dt.AsEnumerable())
                {
                    itemdt["PurchaseId"] = purchaseId.Content;
                    itemdt["UserId"] = username;
                    itemdt["EnterOn"] = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                }
                var bulk = new BulkOperations();
                SqlConnection conn = new SqlConnection(conString);
                List<ItemRetailModel> itemList = new List<ItemRetailModel>();

                var JSONresult = JsonConvert.SerializeObject(dt);
                if (JSONresult != null)
                    itemList = JsonConvert.DeserializeObject<List<ItemRetailModel>>(JSONresult).ToList();

                conn.Open();
                bulk.Setup<ItemRetailModel>()
                    .ForCollection(itemList)
                    .WithTable("ItemRetail")
                    .AddAllColumns()
                    .BulkInsertOrUpdate()
                    .SetIdentityColumn(x => x.ItemRetailId)
                    .MatchTargetOn(x => x.ScanCode)
                    .Commit(conn);
                conn.Close();

                foreach (var itemdt in dt.AsEnumerable())
                {
                    string query = "Select ScanCode from Item where Scancode='"+itemdt["ScanCode"]+"'";
                    SqlCommand cmdI = new SqlCommand(query,conn);
                    SqlDataAdapter sdaI = new SqlDataAdapter(cmdI);
                    DataTable dtItemI = new DataTable();
                    sdaI.Fill(dtItemI);
                    string queryUpdate = "";
                    if (dtItemI.Rows.Count>0)
                    {
                        queryUpdate = "Update Item set CaseCost='" + itemdt["Cost"]+ "',CreateOn='" + itemdt["EnterOn"] + "',CreateBy='" + itemdt["UserId"] + "' where Scancode='" + itemdt["ScanCode"] + "'";
                        
                    }
                    else
                    {
                        queryUpdate = "Insert into Item(ScanCode,Description,CaseCost,CreateOn,CreateBy)Values('" + itemdt["ScanCode"] + "','" + itemdt["Description"] + "','" + itemdt["Cost"] + "','" + itemdt["EnterOn"] + "','" + itemdt["UserId"] + "') ";
                    }
                    SqlCommand cmdUI = new SqlCommand(queryUpdate, conn);
                    conn.Open();
                    cmdUI.ExecuteNonQuery();
                    conn.Close();
                }

                loadItemRetail();

                dgRetail.Visibility = Visibility.Visible;
                addRetailentry.Visibility = Visibility.Visible;
                dgImport.Visibility = Visibility.Hidden;
                grupload.Visibility = Visibility.Hidden;
            }
            catch (Exception ex4)
            {
                SendErrorToText(ex4, errorFileName);
            }
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                string[] headers;

                Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
                Nullable<bool> result = openFileDlg.ShowDialog();

                if (result == true)
                {
                    FileNameTextBox.Text = openFileDlg.FileName;
                    string extension = System.IO.Path.GetExtension(openFileDlg.FileName).ToLower();


                    byte[] bytes = System.IO.File.ReadAllBytes(openFileDlg.FileName);
                    HttpPostedFileBase objFile = (HttpPostedFileBase)new MemoryPostedFile(bytes);

                    Stream stream = objFile.InputStream;

                    IExcelDataReader reader = null;

                    if (extension == ".xls")
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = false
                            }
                        });
                        dt = dataSet.Tables[0];
                    }

                    if (extension == ".xlsx")
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = false
                            }
                        });
                        dt = dataSet.Tables[0];
                    }
                    stream.Close();

                    if (extension == ".csv")
                    {
                        using (StreamReader sr = new StreamReader(openFileDlg.FileName))
                        {
                            headers = sr.ReadLine().Split(',');
                            foreach (string header in headers)
                            {
                                dt.Columns.Add(header);
                            }

                            while (!sr.EndOfStream)
                            {
                                string[] rows = sr.ReadLine().Split(',');
                                if (rows.Length > 1)
                                {
                                    DataRow dr = dt.NewRow();
                                    for (int i = 0; i < headers.Length; i++)
                                    {
                                        if (i < rows.Length)
                                        {
                                            dr[i] = rows[i].Trim();
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                            }
                        }
                    }
                }

                dgRetail.Visibility = Visibility.Hidden;
                dgImport.Visibility = Visibility.Visible;
                BrowseButton.Visibility = Visibility.Hidden;
                btnImport.Visibility = Visibility.Visible;

                dgImport.ItemsSource = dt.DefaultView;
                dgImport.AutoGenerateColumns = true;
                dgImport.CanUserAddRows = false;

                DataGridCheckBoxColumn checkBoxColumn = new DataGridCheckBoxColumn();
                checkBoxColumn.Header = "Select";
                dgImport.Columns.Insert(0, checkBoxColumn);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }


        List<string> strList1 = new List<string>();
        List<string> strList2 = new List<string>();
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cb = (ComboBox)sender;
                string header = e.AddedItems[0].ToString();
                string headerGrid = cb.Name;
                strList1.Add(headerGrid); strList2.Add(header);
            }
            catch (Exception ex3)
            {
                SendErrorToText(ex3, errorFileName);
            }
        }

        private void DgImport_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                var dropDown = new ComboBox()
                {
                    ItemsSource = new string[] { "Description", "ScanCode", "Quantity", "Cost", "Amount" }
                };
                dropDown.Name = e.Column.Header.ToString();
                dropDown.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
                e.Column.Header = dropDown;
            }
            catch (Exception ex2)
            {
                SendErrorToText(ex2, errorFileName);
            }
        }

        private void onEdit(object sender, RoutedEventArgs e)
        {
            try
            {
                add.Visibility = Visibility.Hidden;
                addpurchase.Visibility = Visibility.Visible;
                DataRowView row = (DataRowView)dgPurchase.SelectedItem;
                lblPurchaseId.Content = row["PurchaseId"].ToString();
                txtDate.Text = row["Date"].ToString();
                cbType.Text = row["Type"].ToString();
                cbVendor.Text = row["Vendor"].ToString();
                txtAmount.Text = row["Amount"].ToString();
                txtInvoiceno.Text = row["InvoiceNo"].ToString();
                btnsave.Content = "Update";
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }

        }

        private void onDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dgPurchase.SelectedItem;
                row.Delete();

                int rowsAffected;
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE from Purchase WHERE PurchaseId = " + row["PurchaseId"], conn);
                    cmd.Connection.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                if (rowsAffected > 0)
                    dt.AcceptChanges();
                else
                    dt.RejectChanges();
                lblPurchaseId.Content = 0;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        
        private void loadItemRetail()
        {
            SqlConnection conn = new SqlConnection(conString);
            string queryD = "Select * from ItemRetail where purchaseId='" + purchaseId.Content + "'";
            SqlCommand cmd = new SqlCommand(queryD, conn);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dtItem = new DataTable();
            sda.Fill(dtItem);
            dgRetail.CanUserAddRows = false;
            dgRetail.ItemsSource = dtItem.DefaultView;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                var code = txtScancode.Text;
                var length = code.Length;
                if (length == 12)
                {
                    code = code.Remove(code.Length - 1);
                }
                if (length == 8)
                {
                    var last1 = code.Remove(code.Length - 1);
                    var last2 = last1.Substring(last1.Length - 1);
                    var first3 = code.Remove(code.Length - 5);
                    var first4 = code.Remove(code.Length - 4);
                    var last5 = code.Substring(code.Length - 5);
                    var second3 = last5.Remove(last5.Length - 2);
                    var last4 = code.Substring(code.Length - 4);
                    var second2 = last4.Remove(last4.Length - 2);
                    if (Convert.ToInt32(last2) == 0)
                    {
                        code = first3 + "00000" + second3;
                    }
                    else if (Convert.ToInt32(last2) == 1)
                    {
                        code = first3 + "10000" + second3;
                    }
                    else if (Convert.ToInt32(last2) == 3)
                    {
                        code = first4 + "00000" + second2;
                    }
                    else if (Convert.ToInt32(last2) == 4)
                    {
                        code = code.Remove(code.Length - 3) + "00000" + code.Substring(code.Length - 3).Remove(code.Substring(code.Length - 3).Length - 2);
                    }
                    else if (Convert.ToInt32(last2) == 2)
                    {
                        code = first3 + "20000" + second3;
                    }
                    else
                    {
                        int num = 0;
                        code = code.Remove(code.Length - 2) + num + num + num + num + last2;
                    }
                }
                txtScancode.Text = code;
            }
        }
        private void onRetail(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)dgPurchase.SelectedItem;
            purchaseId.Content = row["PurchaseId"].ToString();
            loadItemRetail();
            add.Visibility = Visibility.Hidden;
            dgPurchase.Visibility = Visibility.Hidden;
            btnGoBack.Visibility = Visibility.Visible;
            dgRetail.Visibility = Visibility.Visible;
            addpurchase.Visibility = Visibility.Hidden;
            addRetailentry.Visibility = Visibility.Visible;
        }
        //Error
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
        // Show Add Purchase Entry
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            add.Visibility = Visibility.Hidden;
            addpurchase.Visibility = Visibility.Visible;
            btnGoBack.Visibility = Visibility.Visible;
        }

        public static void trimData(DataTable dt)
        {
            foreach (DataColumn c in dt.Columns)
                if (c.DataType == typeof(string))
                    foreach (DataRow r in dt.Rows)
                        try
                        {
                            r[c.ColumnName] = r[c.ColumnName].ToString().Trim();
                        }
                        catch
                        { }
        }

        public class ItemRetailModel
        {
            public int ItemRetailId { get; set; }
            public string ScanCode { get; set; }
            public string Description { get; set; }
            public string Quantity { get; set; }
            public string Cost { get; set; }
            public string Amount { get; set; }
            public string PurchaseId { get; set; }
            public string StoreId { get; set; }
            public string UserId { get; set; }
            public string RegisterId { get; set; }
            public string EnterOn { get; set; }
        }
    }
}
