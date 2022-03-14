using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using SqlBulkTools;
using Newtonsoft.Json;
using System.Linq;
using System.Data.OleDb;
using ExcelDataReader;
using System.Windows.Input;
using System.Diagnostics;
using System.Text;
using System.Configuration;
using System.Web;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for ItemView.xaml
    /// </summary>
    public partial class ItemView : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "ItemView.cs";

        //string username = App.Current.Properties["username"].ToString();
        public ItemView()
        {
            try
            {
                InitializeComponent();
                //ItemLoad();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
        private void ItemLoad()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryD = "Select * from item";
                SqlCommand cmd = new SqlCommand(queryD, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dgitem.CanUserAddRows = false;
                dgitem.ItemsSource = dt.DefaultView;
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
                Application appl = new Application();
                appl.Shutdown();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Button_Click_Imaport(object sender, RoutedEventArgs e)
        {
            try
            {
                AddExport.Visibility = Visibility.Hidden;
                grdSecondPart.Visibility = Visibility.Hidden;
                //btnAddItem.Visibility = Visibility.Hidden;
                //btnImport.Visibility = Visibility.Hidden;
                //btnClose.Visibility = Visibility.Hidden;
                btnsave.Visibility = Visibility.Hidden;

                dgImport.Visibility = Visibility.Visible;
                grupload.Visibility = Visibility.Visible;
                btnback.Visibility = Visibility.Visible;
            }
            catch (Exception ex1)
            {
                SendErrorToText(ex1, errorFileName);
            }
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Item item = new Item();
                item.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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

                dgitem.Visibility = Visibility.Hidden;
                dgImport.Visibility = Visibility.Visible;
                BrowseButton.Visibility = Visibility.Hidden;
                btnsave.Visibility = Visibility.Visible;

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

        private void dataGrid1_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                var dropDown = new ComboBox()
                {
                    ItemsSource = new string[] { "Description", "ScanCode", "Department", "Manufacturer","UnitCase","TaxRate",
                        "UnitRetail","CaseDiscount","Payee","CaseCost","PriceGroup","FoodStamp","MinAge","MessureIn"}
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
        public static void trimData(DataTable dt)
        {
            try
            {
                foreach (DataColumn c in dt.Columns)
                    if (c.DataType == typeof(string))
                        foreach (DataRow r in dt.Rows)
                            r[c.ColumnName] = r[c.ColumnName].ToString().Trim();

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, "ItemView");
            }
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


                foreach (var item in dt.AsEnumerable())
                {
                    if (item["UnitRetail"].ToString().Contains("$"))
                    {
                        item["UnitRetail"] = item["UnitRetail"].ToString().Split('$')[1];
                    }
                }
                trimData(dt);

                var bulk = new BulkOperations();
                SqlConnection conn = new SqlConnection(conString);
                List<ItemModel> itemList = new List<ItemModel>();

                var JSONresult = JsonConvert.SerializeObject(dt);
                if (JSONresult != null)
                    itemList = JsonConvert.DeserializeObject<List<ItemModel>>(JSONresult).ToList();

                conn.Open();
                bulk.Setup<ItemModel>()
                    .ForCollection(itemList)
                    .WithTable("Item")
                    .AddAllColumns()
                    .BulkInsertOrUpdate()
                    .SetIdentityColumn(x => x.ItemId)
                    .MatchTargetOn(x => x.ScanCode)
                    .Commit(conn);
                conn.Close();

                string queryD = "Select * from item";
                SqlCommand cmd = new SqlCommand(queryD, conn);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dtItem = new DataTable();
                sda.Fill(dtItem);
                dgitem.CanUserAddRows = false;
                dgitem.ItemsSource = dtItem.DefaultView;

                dgitem.Visibility = Visibility.Visible;
                dgImport.Visibility = Visibility.Hidden;
                grupload.Visibility = Visibility.Hidden;
            }
            catch (Exception ex4)
            {
                SendErrorToText(ex4, errorFileName);
            }
        }

        private void BtnSearch_Click_Search(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Clear();
                dgitem.ItemsSource = null;
                dgitem.Items.Refresh();
                //dgitem.Items.Clear();
                //if (txtScanCode.Text == "")
                //{
                //    string commandText = "SELECT * FROM Item";
                //    SqlConnection connection = new SqlConnection(conString);
                //    SqlCommand command = new SqlCommand(commandText, connection);
                //    command.Parameters.AddWithValue("@scanode", txtScanCode.Text);
                //    connection.Open();
                //    SqlDataAdapter da = new SqlDataAdapter(command);
                //    da.Fill(dt);
                //    connection.Close();
                //}
                if (txtScanCode.Text != "")
                {
                    string commandText = "SELECT* FROM Item WHERE ScanCode = @scanode";
                    SqlConnection connection = new SqlConnection(conString);
                    SqlCommand command = new SqlCommand(commandText, connection);
                    command.Parameters.AddWithValue("@scanode", txtScanCode.Text);
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                    connection.Close();
                }
                else if (txtDescription.Text != "")
                {
                    string commandText = "SELECT* FROM Item WHERE Description = @Description";
                    SqlConnection connection = new SqlConnection(conString);
                    SqlCommand command = new SqlCommand(commandText, connection);
                    command.Parameters.AddWithValue("@Description", txtDescription.Text);
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                    connection.Close();
                }
                else if (txtDepartment.Text != "")
                {
                    string commandText = "SELECT* FROM Item WHERE Department = @Department";
                    SqlConnection connection = new SqlConnection(conString);
                    SqlCommand command = new SqlCommand(commandText, connection);
                    command.Parameters.AddWithValue("@Department", txtDepartment.Text);
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                    connection.Close();
                }
                else if (txtPayee.Text != "")
                {
                    string commandText = "SELECT* FROM Item WHERE Payee = @Payee";
                    SqlConnection connection = new SqlConnection(conString);
                    SqlCommand command = new SqlCommand(commandText, connection);
                    command.Parameters.AddWithValue("@Payee", txtPayee.Text);
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                    connection.Close();
                }
                else
                {
                    string commandText = "SELECT * FROM Item";
                    SqlConnection connection = new SqlConnection(conString);
                    SqlCommand command = new SqlCommand(commandText, connection);
                    command.Parameters.AddWithValue("@scanode", txtScanCode.Text);
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                    connection.Close();
                }
                dgitem.CanUserAddRows = false;
                dgitem.ItemsSource = dt.DefaultView;

                txtScanCode.Text = "";
                txtDescription.Text = "";
                txtDepartment.Text = "";
                txtPayee.Text = "";

                grdSecondPart.Visibility = Visibility.Hidden;
                grdSecondPart2.Visibility = Visibility.Visible;
                btnback.Visibility = Visibility.Visible;
            }
            catch (Exception ex5)
            {
                SendErrorToText(ex5, errorFileName);
            }
        }

        private void Btnback_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grupload.Visibility = Visibility.Hidden;
                grdSecondPart.Visibility = Visibility.Visible;
                AddExport.Visibility = Visibility.Visible;
                grdSecondPart2.Visibility = Visibility.Hidden;
                btnback.Visibility = Visibility.Hidden;

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Dgitem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
        }

        private void BtnSearch_Click_ChangeValue(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtChangeValue.Text == "")
                {
                    txtChangeValue.BorderBrush = System.Windows.Media.Brushes.Red;
                }
                if (cmbHeader.Text == "")
                {
                    cmb1Border.BorderBrush = System.Windows.Media.Brushes.Red;

                }
                if (txtChangeValue.Text != "" && cmbHeader.Text != "")
                {
                    DataTable dt = new DataTable();
                    string cmbHeaderName = cmbHeader.Text;
                    dt = ((DataView)dgitem.ItemsSource).ToTable();
                    foreach (DataColumn c in dt.Columns)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (c.ToString() == cmbHeaderName)
                            {
                                dr[cmbHeaderName] = txtChangeValue.Text;
                            }
                        }
                    }

                    var bulk = new BulkOperations();
                    SqlConnection conn = new SqlConnection(conString);
                    List<ItemModel> itemList = new List<ItemModel>();

                    var JSONresult = JsonConvert.SerializeObject(dt);
                    if (JSONresult != null)
                        itemList = JsonConvert.DeserializeObject<List<ItemModel>>(JSONresult).ToList();

                    conn.Open();
                    bulk.Setup<ItemModel>()
                        .ForCollection(itemList)
                        .WithTable("Item")
                        .AddAllColumns()
                        .BulkInsertOrUpdate()
                        .SetIdentityColumn(x => x.ItemId)
                        .MatchTargetOn(x => x.ScanCode)
                        .Commit(conn);
                    conn.Close();

                    cmbHeader.Text = "";
                    txtChangeValue.Text = "";

                    dgitem.CanUserAddRows = false;
                    dgitem.ItemsSource = dt.DefaultView;
                    grdSecondPart.Visibility = Visibility.Visible;
                    grdSecondPart2.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex6)
            {
                SendErrorToText(ex6, errorFileName);
            }
        }

        private void BtnSearch_Click_ExportCSV(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = ((DataView)dgitem.ItemsSource).ToTable();
                StringBuilder sb = new StringBuilder();
                string[] columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
                sb.AppendLine(string.Join(",", columnNames));
                foreach (DataRow row in dt.Rows)
                {
                    string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                    sb.AppendLine(string.Join(",", fields));
                }
                File.WriteAllText("test.csv", sb.ToString());

                StreamWriter sw = new StreamWriter("export.csv");
                sw.WriteLine(sb.ToString());
                sw.Close();
                Process.Start("export.csv");
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void ComboBox_SelectionChanged_Field(object sender, SelectionChangedEventArgs e)
        {
            try { 
            cmb1Border.BorderBrush = System.Windows.Media.Brushes.White;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void textBox_TextChanged_Value(object sender, TextChangedEventArgs e)
        {
            try { 
            txtChangeValue.BorderBrush = System.Windows.Media.Brushes.Gray;
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

public class ItemModel
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; }
    public string ScanCode { get; set; }
    public string Description { get; set; }
    public string Department { get; set; }
    public string Manufacturer { get; set; }
    public string Payee { get; set; }
    public string FoodStamp { get; set; }
    public string MinAge { get; set; }
    public string UnitCase { get; set; }
    public string CaseCost { get; set; }
    public string UnitRetail { get; set; }
    public string CaseDiscount { get; set; }
    public string TaxRate { get; set; }
    public string MessureIn { get; set; }
    public string PriceGroup { get; set; }
    public string StoreId { get; set; }
    public string CreateOn { get; set; }
    public string CreateBy { get; set; }
}

public class MemoryPostedFile : HttpPostedFileBase
{
    private readonly byte[] fileBytes;

    public MemoryPostedFile(byte[] fileBytes, string fileName = null)
    {
        this.fileBytes = fileBytes;
        this.FileName = fileName;
        this.InputStream = new MemoryStream(fileBytes);
    }

    public override int ContentLength => fileBytes.Length;

    public override string FileName { get; }

    public override Stream InputStream { get; }
}