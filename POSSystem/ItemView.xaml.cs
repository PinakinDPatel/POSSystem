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

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for ItemView.xaml
    /// </summary>
    public partial class ItemView : Window
    {
        string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        public ItemView()
        {
            InitializeComponent();
            ItemLoad();
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
                dgitem.ItemsSource = dt.DefaultView;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application appl = new Application();
            appl.Shutdown();
        }

        private void Button_Click_Imaport(object sender, RoutedEventArgs e)
        {
            try
            {
                dgitem.Visibility = Visibility.Hidden;
                btnAddItem.Visibility = Visibility.Hidden;
                btnImport.Visibility = Visibility.Hidden;
                btnClose.Visibility = Visibility.Hidden;
                btnItemsSave.Visibility = Visibility.Visible;
                dgImport.Visibility = Visibility.Visible;
                grupload.Visibility = Visibility.Visible;
            }
            catch (Exception ex1)
            {
                throw ex1;
            }
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {

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

                    //Stream stream =  result.InputStream;
                    FileStream stream = File.Open(openFileDlg.FileName, FileMode.Open, FileAccess.Read);

                    IExcelDataReader reader = null;

                    if (extension == ".xls")
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
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
                                UseHeaderRow = true
                            }
                        });
                        dt = dataSet.Tables[0];
                    }


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

                dgImport.ItemsSource = dt.DefaultView;
                dgImport.AutoGenerateColumns = true;
                dgImport.CanUserAddRows = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void dataGrid1_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                var dropDown = new ComboBox()
                {
                    ItemsSource = new string[] { "Description", "ScanCode", "Quantity", "Department", "Manufacturer",
                                             "UnitCase","CostAfterDiscount","Rate","UnitRetail","CaseDiscount","Payee"}
                };
                dropDown.Name = e.Column.Header.ToString();
                dropDown.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
                e.Column.Header = dropDown;
            }
            catch (Exception ex2)
            {
                throw ex2;
            }
        }

        List<string> strList1 = new List<String>();
        List<string> strList2 = new List<String>();
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
                throw ex3;
            }
        }

        private void Button_Click_Save_ImportFile(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                DataGrid dg = new DataGrid();
                dt = ((DataView)dgImport.ItemsSource).ToTable();
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
                dgitem.ItemsSource = dtItem.DefaultView;

                dgitem.Visibility = Visibility.Visible;
                dgImport.Visibility = Visibility.Hidden;
                grupload.Visibility = Visibility.Hidden;
            }
            catch (Exception ex4)
            {
                throw ex4;
            }
        }

        private void BtnSearch_Click_Search(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
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
                if (txtDescription.Text != "")
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
                if (txtDepartment.Text != "")
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
                if (txtPayee.Text != "")
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
                dgitem.ItemsSource = dt.DefaultView;

                txtScanCode.Text = "";
                txtDescription.Text = "";
                txtDepartment.Text = "";
                txtPayee.Text = "";

                grdSecondPart.Visibility = Visibility.Hidden;
                grdSecondPart2.Visibility = Visibility.Visible;
            }
            catch (Exception ex5)
            {
                throw ex5;
            }
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

                    dgitem.ItemsSource = dt.DefaultView;
                    grdSecondPart.Visibility = Visibility.Visible;
                    grdSecondPart2.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex6)
            {
                throw ex6;
            }
        }

        private void ComboBox_SelectionChanged_Field(object sender, SelectionChangedEventArgs e)
        {
            cmb1Border.BorderBrush = System.Windows.Media.Brushes.White;
        }

        private void textBox_TextChanged_Value(object sender, TextChangedEventArgs e)
        {
            txtChangeValue.BorderBrush = System.Windows.Media.Brushes.Gray;
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
    public string StoreId { get; set; }
    public string CreateOn { get; set; }
    public string CreateBy { get; set; }
}