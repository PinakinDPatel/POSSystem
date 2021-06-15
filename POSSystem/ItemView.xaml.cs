using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
            SqlConnection con = new SqlConnection(conString);
            string queryD = "Select * from item";
            SqlCommand cmd = new SqlCommand(queryD, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dgitem.ItemsSource = dt.DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application appl = new Application();
            appl.Shutdown();
        }

        private void Button_Click_Imaport(object sender, RoutedEventArgs e)
        {
            dgitem.Visibility = Visibility.Hidden;
            btnAddItem.Visibility = Visibility.Hidden;
            btnImport.Visibility = Visibility.Hidden;
            btnClose.Visibility = Visibility.Hidden;
            btnItemsSave.Visibility = Visibility.Visible;
            dgImport.Visibility = Visibility.Visible;
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            string[] headers;

            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FileNameTextBox.Text = openFileDlg.FileName;
                string extension = System.IO.Path.GetExtension(openFileDlg.FileName).ToLower();

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
        private void dataGrid1_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var dropDown = new ComboBox() { ItemsSource = new string[] { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5" } };
            dropDown.Name = e.Column.Header.ToString();
            dropDown.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
            e.Column.Header = dropDown;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var HeaderTexts = dataGridUserSalesRep.Columns.Select(e => e.Header.ToString()).ToList();
            foreach (var col in dgImport.Columns)
            {
                //dgImport.Columns[0].HeaderText = "HeaderName";
            }
        }

        private void Button_Click_Save_ImportFile(object sender, RoutedEventArgs e)
        {
            var dsds = dgImport.ItemsSource;
        }
    }
}
