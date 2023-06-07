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
using System.Drawing.Imaging;
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
using Image = System.Windows.Controls.Image;

namespace POSSystem
{
    public partial class MainWindow : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        private PrintDocument PrintDocument;
        private Graphics graphics;
        string tenderCode = "";
        int loyaltyCustomerCount = 0;
        string refund = "";
        int transId = 0;
        string categorytext = "";
        DataTable dt = new DataTable();
        DataTable dtVoidItem = new DataTable();
        DataTable dtdepartment = new DataTable(); // 4
        DataTable dtAccount = new DataTable(); //3
        DataTable dtItem = new DataTable();  // 1
        DataTable dtPromotion = new DataTable(); //2
        DataTable dtTransaction = new DataTable();
        DataTable dtAddCategory = new DataTable(); // 5
        DataTable dtCategory = new DataTable(); // 6
        DataTable dtstr = new DataTable();
        DataTable dtHold = new DataTable();
        DataTable dttranid = new DataTable(); // 7
        string username = App.Current.Properties["username"].ToString();
        string storeid = App.Current.Properties["StoreId"].ToString();
        string posId = App.Current.Properties["POSId"].ToString();
        string registerid = App.Current.Properties["RegisterId"].ToString();
        string date = DateTime.Now.ToString("yyyy/MM/dd");
        private static string ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "MainWindow.cs";
        string dataGridSelectedIndex = "";

        string txtGotFocusStr = string.Empty;
        int dtInndex = 0;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                lblLoyaltyId.Content = "";
                //Load TextBox and Label
                grandTotal.Content = "Pay $0.00";
                txtTotal.Content = "$0.00";
                taxtTotal.Content = "$0.00";
                lblDate.Content = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                lblusername.Content = username;

                //TextBox Change Event
                TextBox tb = new TextBox();
                tb.KeyDown += new KeyEventHandler(OnKeyDownHandler);
                tb.KeyDown += new KeyEventHandler(TxtCashReceive_KeyDown);
                tb.KeyDown += new KeyEventHandler(TxtBarcode_KeyDown);

                dt.Columns.Add("Scancode");
                dt.Columns.Add("Description");
                dt.Columns.Add("UnitRetail");
                dt.Columns.Add("TaxRate");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("Amount");
                dt.Columns.Add("Date");
                dt.Columns.Add("Time");
                dt.Columns.Add("TransactionId");
                dt.Columns.Add("CreateBy");
                dt.Columns.Add("CreateOn");
                dt.Columns.Add("PromotionName");
                dt.Columns.Add("Void");
                dt.Columns.Add("PromotionId");
                dt.Columns.Add("Oprice");
                dt.Columns.Add("bIsTrueId");
                dt.Columns.Add("LoyaltyId");
                dt.Columns.Add("Customer");
                dt.Columns.Add("StoreId");
                dt.Columns.Add("POSId");
                dt.Columns.Add("RegisterId");

                dtHold.Columns.Add("Scancode");
                dtHold.Columns.Add("Description");
                dtHold.Columns.Add("UnitRetail");
                dtHold.Columns.Add("TaxRate");
                dtHold.Columns.Add("Quantity");
                dtHold.Columns.Add("Amount");
                dtHold.Columns.Add("Date");
                dtHold.Columns.Add("Time");
                dtHold.Columns.Add("TransactionId");
                dtHold.Columns.Add("CreateBy");
                dtHold.Columns.Add("CreateOn");
                dtHold.Columns.Add("PromotionName");
                dtHold.Columns.Add("Void");
                dtHold.Columns.Add("Oprice");
                dtHold.Columns.Add("bIsTrueId");
                dtHold.Columns.Add("LoyaltyId");
                dtHold.Columns.Add("Customer");

                textBox1.Focus();

                dtVoidItem.Columns.Add("Scancode");
                dtVoidItem.Columns.Add("Description");
                dtVoidItem.Columns.Add("UnitRetail");
                dtVoidItem.Columns.Add("TaxRate");
                dtVoidItem.Columns.Add("Quantity");
                dtVoidItem.Columns.Add("Amount");
                dtVoidItem.Columns.Add("Void");
                dtVoidItem.Columns.Add("Oprice");

                LoadItem();
                LoadDepartment();
                loadDropdownCustomer();
                loadtransactionId();
                loadHold();
                addCategory1();
                Category();
                StoreDetails();
                //   FileTransfer();

                if (dtstr.Rows.Count == 0)
                {
                    MessageBox.Show("Please Fill Store Details");
                    App.Current.Properties["username"] = "";
                    lblusername.Content = "";
                    StoreDetails SD = new StoreDetails();
                    this.Close();
                    SD.Show();
                }

                // Show/Hide
                ugAddcategory1.Visibility = Visibility.Hidden;
                ugAddcategory2.Visibility = Visibility.Hidden;
                ugDepartment.Visibility = Visibility.Visible;
                ugDepartment1.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "MainWindow");
            }
        }

        private void LoadItem()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conString))
                using (SqlCommand cmd = new SqlCommand("spPOSMainPage", conn))
                {
                    SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                    adapt.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adapt.SelectCommand.Parameters.Add(new SqlParameter("@storeid", SqlDbType.VarChar, 100));
                    adapt.SelectCommand.Parameters["@storeid"].Value = storeid;
                    adapt.SelectCommand.Parameters.Add(new SqlParameter("@date", SqlDbType.VarChar, 100));
                    adapt.SelectCommand.Parameters["@date"].Value = date;
                    adapt.SelectCommand.Parameters.Add(new SqlParameter("@RegisterId", SqlDbType.VarChar, 100));
                    adapt.SelectCommand.Parameters["@RegisterId"].Value = registerid;
                    adapt.SelectCommand.Parameters.Add(new SqlParameter("@POSId", SqlDbType.VarChar, 100));
                    adapt.SelectCommand.Parameters["@POSId"].Value = posId;
                    DataSet ds = new DataSet();
                    adapt.Fill(ds);
                    if (ds.Tables.Count > 0)
                    {
                        dtItem = ds.Tables[0];
                        dtPromotion = ds.Tables[1];
                        dtAccount = ds.Tables[2];
                        dtdepartment = ds.Tables[3];
                        dtAddCategory = ds.Tables[4];
                        dtCategory = ds.Tables[5];
                        dttranid = ds.Tables[6];

                        DataRow newRow = dtAccount.NewRow();
                        newRow["Name"] = "--Select--";
                        dtAccount.Rows.InsertAt(newRow, 0);
                    }
                }

            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "LoadItem"); }
        }


        //public void FileTransfer()
        //{
        //    string sourcePath = ConfigurationManager.AppSettings["sourcePath"].ToString();
        //    string DestinationPath = ConfigurationManager.AppSettings["DestinationPath"].ToString();
        //    foreach (string f in Directory.GetFiles(sourcePath))
        //    {
        //        try
        //        {
        //            string files = Path.GetFileName(f);
        //            if (!File.Exists(DestinationPath + "\\" + files))
        //                File.Create(DestinationPath + "\\" + files);
        //        }
        //        catch (Exception ex) { SendErrorToText(ex, errorFileName, "FileTransfer"); }
        //    }
        //}



        private void loadDropdownCustomer()
        {
            try
            {
                cbcustomer.ItemsSource = dtAccount.DefaultView;
                cbcustomer.DisplayMemberPath = "Name";
                cbCustomer1.ItemsSource = dtAccount.DefaultView;
                cbCustomer1.DisplayMemberPath = "Name";
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "loadDropdownCustomer"); }
        }

        private void LoadDepartment()
        {
            try
            {
                //SqlConnection con = new SqlConnection(conString);
                //string queryS = "Select Department,TaxRate,FilePath from Department";
                //SqlCommand cmd1 = new SqlCommand(queryS, con);
                //SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                //sda1.Fill(dtdepartment);

                if (dtdepartment.Rows.Count > 19)
                {
                    RightArrow.Visibility = Visibility.Visible;
                    RightArrow.IsEnabled = true;
                }
                for (int i = 0; i < dtdepartment.Rows.Count; ++i)
                {
                    Button button = new Button();
                    Grid G = new Grid();
                    G.RowDefinitions.Add(new RowDefinition());
                    G.RowDefinitions.Add(new RowDefinition());
                    TextBlock TB = new TextBlock();
                    Image image = new System.Windows.Controls.Image();
                    if (i <= 19)
                    {
                        TB.Text = dtdepartment.Rows[i].ItemArray[0].ToString();
                        TB.TextAlignment = TextAlignment.Center;
                        TB.FontSize = 16;
                        TB.TextWrapping = TextWrapping.Wrap;

                        if (dtdepartment.Rows[i].ItemArray[2].ToString() != "")
                        {
                            var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                            var path = dtdepartment.Rows[i].ItemArray[2].ToString();
                            var fullpath = Path + "Image\\" + path;
                            image.Source = new BitmapImage(new Uri(fullpath));
                            image.Height = 50;
                            image.Width = 80;
                            image.Stretch = Stretch.Fill;
                        }
                        button.Width = 120;
                        button.Height = 80;
                        button.Margin = new Thickness(5);
                        string abc = dtdepartment.Rows[i].ItemArray[1].ToString();
                        button.Click += (sender, e) => { button_Click(sender, e, TB.Text, abc); };
                        Grid.SetRow(image, 0);
                        G.Children.Add(image);
                        Grid.SetRow(TB, 1);
                        G.Children.Add(TB);
                        G.HorizontalAlignment = HorizontalAlignment.Center;
                        G.VerticalAlignment = VerticalAlignment.Bottom;
                        button.Content = G;
                        this.ugDepartment.HorizontalAlignment = HorizontalAlignment.Left;
                        this.ugDepartment.VerticalAlignment = VerticalAlignment.Top;
                        this.ugDepartment.Columns = 5;
                        this.ugDepartment.Children.Add(button);






                        // var size = System.Windows.SystemParameters.PrimaryScreenWidth;
                        //if (size == 1024 || size == 1366)
                        //{





                        //button.Content = new TextBlock()
                        //{
                        //    FontSize = 20,
                        //    Text = dtdepartment.Rows[i].ItemArray[0].ToString(),
                        //    TextAlignment = TextAlignment.Left,
                        //    TextWrapping = TextWrapping.Wrap
                        //};
                        //if (dtdepartment.Rows[i].ItemArray[2].ToString() != "")
                        //{
                        //    var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                        //    var path = dtdepartment.Rows[i].ItemArray[2].ToString();
                        //    var fullpath = Path + "\\Image\\" + path;
                        //    button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                        //}
                        //button.Width = 120;
                        //button.Height = 80;
                        //button.Foreground = new SolidColorBrush(Colors.Black);
                        //button.FontSize = 15;
                        //button.FontWeight = FontWeights.Bold;
                        //button.Margin = new Thickness(5);

                        //string abc = dtdepartment.Rows[i].ItemArray[1].ToString();
                        //button.Click += (sender, e) => { button_Click(sender, e, abc); };
                        //this.ugDepartment.HorizontalAlignment = HorizontalAlignment.Left;
                        //this.ugDepartment.VerticalAlignment = VerticalAlignment.Top;
                        ////ColumnDefinition cd = new ColumnDefinition();
                        ////cd.Width = GridLength.Auto;
                        //this.ugDepartment.Columns = 5;
                        //this.ugDepartment.Children.Add(button);

                        //}
                        //else if (size > 1900)
                        //{
                        //    button.Content = new TextBlock()
                        //    {
                        //        FontSize = 26,
                        //        Text = dtdepartment.Rows[i].ItemArray[0].ToString(),
                        //        TextAlignment = TextAlignment.Left,
                        //        TextWrapping = TextWrapping.Wrap
                        //    };
                        //    if (dtdepartment.Rows[i].ItemArray[2].ToString() != "")
                        //    {
                        //        var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                        //        var path = dtdepartment.Rows[i].ItemArray[2].ToString();
                        //        var fullpath = Path + "\\Image\\" + path;
                        //        button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                        //    }
                        //    button.Width = 230;
                        //    button.Height = 112;
                        //    button.Foreground = new SolidColorBrush(Colors.Black);
                        //    button.FontWeight = FontWeights.Bold;
                        //    button.Margin = new Thickness(5);

                        //    string abc = dtdepartment.Rows[i].ItemArray[1].ToString();
                        //    button.Click += (sender, e) => { button_Click(sender, e, abc); };
                        //    this.ugDepartment.HorizontalAlignment = HorizontalAlignment.Left;
                        //    this.ugDepartment.VerticalAlignment = VerticalAlignment.Top;
                        //    //ColumnDefinition cd = new ColumnDefinition();
                        //    //cd.Width = GridLength.Auto;
                        //    this.ugDepartment.Columns = 5;
                        //    this.ugDepartment.Children.Add(button);
                        //}
                    }
                    else if (i > 19)
                    {
                        TB.Text = dtdepartment.Rows[i].ItemArray[0].ToString();
                        TB.TextAlignment = TextAlignment.Center;
                        TB.TextWrapping = TextWrapping.Wrap;
                        TB.FontSize = 16;

                        if (dtdepartment.Rows[i].ItemArray[2].ToString() != "")
                        {
                            var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                            var path = dtdepartment.Rows[i].ItemArray[2].ToString();
                            var fullpath = Path + "\\Image\\" + path;
                            image.Source = new BitmapImage(new Uri(fullpath));
                            image.Height = 80;
                            image.Width = 80;
                            image.Stretch = Stretch.Fill;
                        }
                        button.Width = 120;
                        button.Height = 80;
                        button.Margin = new Thickness(5);
                        string abc = dtdepartment.Rows[i].ItemArray[1].ToString();
                        button.Click += (sender, e) => { button_Click(sender, e, TB.Text, abc); };
                        Grid.SetRow(image, 0);
                        G.Children.Add(image);
                        Grid.SetRow(TB, 1);
                        G.Children.Add(TB);
                        G.HorizontalAlignment = HorizontalAlignment.Center;
                        G.VerticalAlignment = VerticalAlignment.Bottom;
                        button.Content = G;
                        this.ugDepartment1.HorizontalAlignment = HorizontalAlignment.Left;
                        this.ugDepartment1.VerticalAlignment = VerticalAlignment.Top;
                        this.ugDepartment1.Columns = 5;
                        this.ugDepartment1.Children.Add(button);







                        // button.Content = new TextBlock()
                        // {
                        //     FontSize = 20,
                        //     Text = dtdepartment.Rows[i].ItemArray[0].ToString(),
                        //     TextAlignment = TextAlignment.Left,
                        //     TextWrapping = TextWrapping.Wrap
                        // };
                        // if (dtdepartment.Rows[i].ItemArray[2].ToString() != "")
                        // {
                        //     var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                        //     var path = dtdepartment.Rows[i].ItemArray[2].ToString();
                        //     var fullpath = Path + "\\Image\\" + path;
                        //     button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                        // }
                        // button.Width = 120;
                        // button.Height = 80;
                        // button.HorizontalAlignment = HorizontalAlignment.Left;
                        // button.VerticalAlignment = VerticalAlignment.Top;
                        // button.Foreground = new SolidColorBrush(Colors.Black);
                        // button.FontSize = 15;
                        // button.FontWeight = FontWeights.Bold;
                        // button.Margin = new Thickness(5);

                        // string abc = dtdepartment.Rows[i].ItemArray[1].ToString();
                        //// button.Click += (sender, e) => { button_Click(sender, e, abc); };
                        // this.ugDepartment1.HorizontalAlignment = HorizontalAlignment.Left;
                        // this.ugDepartment1.VerticalAlignment = VerticalAlignment.Top;
                        // //ColumnDefinition cd = new ColumnDefinition();
                        // //cd.Width = GridLength.Auto;
                        // this.ugDepartment1.Columns = 5;
                        // this.ugDepartment1.Children.Add(button);
                    }
                }
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "LoadDepartment"); }
        }

        private void addCategory1()
        {
            try
            {
                //SqlConnection con = new SqlConnection(conString);
                //string queryAddCat1 = "select category,CategoryImage from addcategory";
                //SqlCommand cmdAddCat1 = new SqlCommand(queryAddCat1, con);
                //SqlDataAdapter sdaAddCat1 = new SqlDataAdapter(cmdAddCat1);
                //sdaAddCat1.Fill(dtAddCategory);
                if (dtAddCategory.Rows.Count != 0)
                {
                    for (int i = 0; i < dtAddCategory.Rows.Count; i++)
                    {
                        Button button = new Button();
                        Grid G = new Grid();
                        G.RowDefinitions.Add(new RowDefinition());
                        G.RowDefinitions.Add(new RowDefinition());
                        TextBlock TB = new TextBlock();
                        Image image = new System.Windows.Controls.Image();
                        if (i <= 23)
                        {
                            //var size = System.Windows.SystemParameters.PrimaryScreenWidth;

                            TB.Text = dtAddCategory.Rows[i].ItemArray[0].ToString();
                            TB.TextAlignment = TextAlignment.Center;
                            TB.TextWrapping = TextWrapping.Wrap;
                            TB.FontSize = 16;
                            if (dtAddCategory.Rows[i].ItemArray[1].ToString() != "")
                            {
                                var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                                var path = dtAddCategory.Rows[i].ItemArray[1].ToString();
                                var fullpath = Path + "\\Image\\" + path;
                                image.Source = new BitmapImage(new Uri(fullpath));
                                image.Height = 70;
                                image.Width = 80;
                                image.Stretch = Stretch.Fill;
                            }
                            button.Width = 97;
                            button.Height = 78;
                            button.Margin = new Thickness(5);
                            button.Click += (sender, e) => { button_Click_Category(sender, e, TB.Text); };
                            Grid.SetRow(image, 0);
                            G.Children.Add(image);
                            Grid.SetRow(TB, 1);
                            G.Children.Add(TB);
                            G.HorizontalAlignment = HorizontalAlignment.Center;
                            G.VerticalAlignment = VerticalAlignment.Bottom;
                            button.Content = G;
                            this.ugAddcategory1.HorizontalAlignment = HorizontalAlignment.Left;
                            this.ugAddcategory1.VerticalAlignment = VerticalAlignment.Top;
                            this.ugAddcategory1.Columns = 6;
                            this.ugAddcategory1.Children.Add(button);

                            //button.Content = new TextBlock()
                            //{
                            //    FontSize = 15,
                            //    Text = dtAddCategory.Rows[i].ItemArray[0].ToString(),
                            //    TextAlignment = TextAlignment.Center,
                            //    TextWrapping = TextWrapping.Wrap
                            //};
                            //if (dtAddCategory.Rows[i].ItemArray[0].ToString() != "")
                            //{
                            //    var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                            //    var path = dtAddCategory.Rows[i].ItemArray[1].ToString();
                            //    if (path != "")
                            //    {
                            //        var fullpath = Path + "\\Image\\" + path;
                            //        button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                            //    }
                            //}
                            //button.Width = 97;
                            //button.Height = 78;
                            //button.HorizontalAlignment = HorizontalAlignment.Left;
                            //button.VerticalAlignment = VerticalAlignment.Top;

                            ////button.Foreground = new SolidColorBrush(Colors.Black);
                            //button.FontSize = 15;
                            //button.FontWeight = FontWeights.Bold;
                            //button.Effect = new DropShadowEffect()
                            //{ Color = Colors.BlueViolet };
                            //button.Margin = new Thickness(5, 5, 5, 5);
                            //string abc = dtAddCategory.Rows[i].ItemArray[0].ToString();
                            //this.ugAddcategory1.HorizontalAlignment = HorizontalAlignment.Left;
                            //this.ugAddcategory1.VerticalAlignment = VerticalAlignment.Top;
                            //button.Click += new RoutedEventHandler(button_Click_Category);
                            ////button.Click += (sender, e) => { button_Click_CategoryDescription(sender, e); };
                            //this.ugAddcategory1.Columns = 6;
                            //this.ugAddcategory1.Children.Add(button);
                        }
                        else if (i > 23)
                        {
                            //Button button = new Button();
                            TB.Text = dtAddCategory.Rows[i].ItemArray[0].ToString();
                            TB.TextAlignment = TextAlignment.Center;
                            TB.TextWrapping = TextWrapping.Wrap;
                            TB.FontSize = 16;
                            if (dtAddCategory.Rows[i].ItemArray[1].ToString() != "")
                            {
                                var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                                var path = dtAddCategory.Rows[i].ItemArray[1].ToString();
                                var fullpath = Path + "\\Image\\" + path;
                                image.Source = new BitmapImage(new Uri(fullpath));
                                image.Height = 70;
                                image.Width = 80;
                                image.Stretch = Stretch.Fill;
                            }
                            button.Width = 97;
                            button.Height = 78;
                            button.Margin = new Thickness(5);
                            button.Click += (sender, e) => { button_Click_Category(sender, e, TB.Text); };
                            Grid.SetRow(image, 0);
                            G.Children.Add(image);
                            Grid.SetRow(TB, 1);
                            G.Children.Add(TB);
                            G.HorizontalAlignment = HorizontalAlignment.Center;
                            G.VerticalAlignment = VerticalAlignment.Bottom;
                            button.Content = G;
                            this.ugAddcategory2.HorizontalAlignment = HorizontalAlignment.Left;
                            this.ugAddcategory2.VerticalAlignment = VerticalAlignment.Top;
                            this.ugAddcategory2.Columns = 6;
                            this.ugAddcategory2.Children.Add(button);

                            //button.Content = new TextBlock()
                            //{
                            //    FontSize = 15,
                            //    Text = dtAddCategory.Rows[i].ItemArray[0].ToString(),
                            //    TextAlignment = TextAlignment.Center,
                            //    TextWrapping = TextWrapping.Wrap
                            //};
                            //if (dtAddCategory.Rows[i].ItemArray[0].ToString() != "")
                            //{
                            //    var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                            //    var path = dtAddCategory.Rows[i].ItemArray[1].ToString();
                            //    if (path != "")
                            //    {
                            //        var fullpath = Path + "\\Image\\" + path;
                            //        button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                            //    }
                            //}
                            //button.Width = 97;
                            //button.Height = 78;
                            //button.HorizontalAlignment = HorizontalAlignment.Left;
                            //button.VerticalAlignment = VerticalAlignment.Top;

                            ////button.Foreground = new SolidColorBrush(Colors.White);
                            //button.FontSize = 15;
                            //button.FontWeight = FontWeights.Bold;
                            //button.Effect = new DropShadowEffect()
                            //{ Color = Colors.BlueViolet };
                            //button.Margin = new Thickness(5, 5, 5, 5);
                            //string abc = dtAddCategory.Rows[i].ItemArray[0].ToString();
                            //this.ugAddcategory2.HorizontalAlignment = HorizontalAlignment.Left;
                            //this.ugAddcategory2.VerticalAlignment = VerticalAlignment.Top;
                            //button.Click += new RoutedEventHandler(button_Click_Category);
                            ////button.Click += (sender, e) => { button_Click_CategoryDescription(sender, e); };
                            //this.ugAddcategory2.Columns = 6;
                            //this.ugAddcategory2.Children.Add(button);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "addCategory1");
            }
        }

        private void Category()
        {
            try
            {
                //SqlConnection con = new SqlConnection(conString);
                //string queryS = "select Description,categoryimage,category from category";
                //SqlCommand cmd1 = new SqlCommand(queryS, con);
                //SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                //sda1.Fill(dtCategory);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Category");
            }
        }

        private void Category1(object sender, RoutedEventArgs e)
        {
            try
            {
                Button SelectedButton = (Button)sender;
                ugCategory1.Children.Remove(SelectedButton);
                ugCategory1.Children.Clear();
                ugCategory2.Children.Remove(SelectedButton);
                ugCategory2.Children.Clear();
                btnShortKey.Visibility = Visibility.Hidden;
                btnDept.Visibility = Visibility.Visible;
                ugDepartment.Visibility = Visibility.Hidden;
                ugDepartment1.Visibility = Visibility.Hidden;
                ugAddcategory1.Visibility = Visibility.Hidden;
                ugAddcategory2.Visibility = Visibility.Hidden;
                ugCategory1.Visibility = Visibility.Visible;
                ugCategory2.Visibility = Visibility.Hidden;

                int j = 0;
                for (int i = 0; i < dtCategory.Rows.Count; i++)
                {
                    if (dtCategory.Rows[i].ItemArray[2].ToString() == categorytext)
                    {
                        Button button = new Button();
                        Grid G = new Grid();
                        G.RowDefinitions.Add(new RowDefinition());
                        G.RowDefinitions.Add(new RowDefinition());
                        TextBlock TB = new TextBlock();
                        Image image = new System.Windows.Controls.Image();
                        if (j <= 23)
                        {
                            TB.Text = dtCategory.Rows[i].ItemArray[0].ToString();
                            TB.TextAlignment = TextAlignment.Center;
                            TB.TextWrapping = TextWrapping.Wrap;
                            button.Tag = TB.Text;
                            TB.FontSize = 16;
                            if (dtCategory.Rows[i].ItemArray[1].ToString() != "")
                            {
                                var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                                var path = dtCategory.Rows[i].ItemArray[1].ToString();
                                var fullpath = Path + "\\Image\\" + path;
                                image.Source = new BitmapImage(new Uri(fullpath));
                                image.Height = 70;
                                image.Width = 80;
                                image.Stretch = Stretch.Fill;
                            }
                            button.Width = 97;
                            button.Height = 78;
                            button.Margin = new Thickness(5);
                            button.Click += new RoutedEventHandler(button_Click_Category_Description);
                            Grid.SetRow(image, 0);
                            G.Children.Add(image);
                            Grid.SetRow(TB, 1);
                            G.Children.Add(TB);
                            G.HorizontalAlignment = HorizontalAlignment.Center;
                            G.VerticalAlignment = VerticalAlignment.Bottom;
                            button.Content = G;
                            this.ugCategory1.HorizontalAlignment = HorizontalAlignment.Left;
                            this.ugCategory1.VerticalAlignment = VerticalAlignment.Top;
                            this.ugCategory1.Columns = 6;
                            this.ugCategory1.Children.Add(button);

                            // Button button = new Button();
                            // button.Content = new TextBlock()
                            // {
                            //     FontSize = 15,
                            //     Text = dtCategory.Rows[i].ItemArray[0].ToString(),
                            //     TextAlignment = TextAlignment.Center,
                            //     TextWrapping = TextWrapping.Wrap
                            // };
                            // if (dtCategory.Rows[i].ItemArray[0].ToString() != "")
                            // {
                            //     var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                            //     var path = dtCategory.Rows[i].ItemArray[1].ToString();
                            //     if (path != "")
                            //     {
                            //         var fullpath = Path + "\\Image\\" + path;
                            //         button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                            //     }
                            // }
                            // button.Width = 97;
                            // button.Height = 78;
                            // button.HorizontalAlignment = HorizontalAlignment.Left;
                            // button.VerticalAlignment = VerticalAlignment.Top;
                            // button.Margin = new Thickness(5);
                            //// button.Foreground = new SolidColorBrush(Colors.White);
                            // button.FontSize = 15;
                            // button.FontWeight = FontWeights.Bold;
                            // button.Effect = new DropShadowEffect()
                            // { Color = Colors.BlueViolet };
                            // button.Margin = new Thickness(5, 5, 5, 5);
                            // string abc = dtCategory.Rows[i].ItemArray[0].ToString();
                            // this.ugCategory1.HorizontalAlignment = HorizontalAlignment.Left;
                            // this.ugCategory1.VerticalAlignment = VerticalAlignment.Top;
                            //button.Click += new RoutedEventHandler(button_Click_Category_Description);
                            // this.ugCategory1.Columns = 6;
                            // this.ugCategory1.Children.Add(button);
                            j = j + 1;
                        }
                        else if (j > 23)
                        {
                            if (j > 23)
                            {
                                TB.Text = dtCategory.Rows[i].ItemArray[0].ToString();
                                TB.TextAlignment = TextAlignment.Center;
                                TB.TextWrapping = TextWrapping.Wrap;
                                TB.FontSize = 16;
                                if (dtCategory.Rows[i].ItemArray[1].ToString() != "")
                                {
                                    var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                                    var path = dtCategory.Rows[i].ItemArray[1].ToString();
                                    var fullpath = Path + "\\Image\\" + path;
                                    image.Source = new BitmapImage(new Uri(fullpath));
                                    image.Height = 70;
                                    image.Width = 80;
                                    image.Stretch = Stretch.Fill;
                                }
                                button.Width = 97;
                                button.Height = 78;
                                button.Margin = new Thickness(5);
                                button.Click += new RoutedEventHandler(button_Click_Category_Description);
                                //button.Click += (sender, e) => { button_Click_Category_Description(sender, e,); };
                                Grid.SetRow(image, 0);
                                G.Children.Add(image);
                                Grid.SetRow(TB, 1);
                                G.Children.Add(TB);
                                G.HorizontalAlignment = HorizontalAlignment.Center;
                                G.VerticalAlignment = VerticalAlignment.Bottom;
                                button.Content = G;
                                this.ugCategory2.HorizontalAlignment = HorizontalAlignment.Left;
                                this.ugCategory2.VerticalAlignment = VerticalAlignment.Top;
                                this.ugCategory2.Columns = 6;
                                this.ugCategory2.Children.Add(button);

                                // Button button = new Button();
                                // button.Content = new TextBlock()
                                // {
                                //     FontSize = 15,
                                //     Text = dtCategory.Rows[i].ItemArray[0].ToString(),
                                //     TextAlignment = TextAlignment.Center,
                                //     TextWrapping = TextWrapping.Wrap
                                // };
                                // if (dtCategory.Rows[i].ItemArray[0].ToString() != "")
                                // {
                                //     var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                                //     var path = dtCategory.Rows[i].ItemArray[1].ToString();
                                //     if (path != "")
                                //     {
                                //         var fullpath = Path + "\\Image\\" + path;
                                //         button.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(fullpath, UriKind.Relative)), Opacity = 0.95 };
                                //     }
                                // }
                                // button.Width = 97;
                                // button.Height = 78;
                                // button.HorizontalAlignment = HorizontalAlignment.Left;
                                // button.VerticalAlignment = VerticalAlignment.Top;
                                // button.Margin = new Thickness(5);
                                //// button.Foreground = new SolidColorBrush(Colors.White);
                                // button.FontSize = 15;
                                // button.FontWeight = FontWeights.Bold;
                                // button.Effect = new DropShadowEffect()
                                // { Color = Colors.BlueViolet };
                                // button.Margin = new Thickness(5, 5, 5, 5);
                                // string abc = dtCategory.Rows[i].ItemArray[0].ToString();
                                // this.ugCategory2.HorizontalAlignment = HorizontalAlignment.Left;
                                // this.ugCategory2.VerticalAlignment = VerticalAlignment.Top;
                                // button.Click += new RoutedEventHandler(button_Click_Category_Description);
                                // this.ugCategory2.Columns = 6;
                                // this.ugCategory2.Children.Add(button);
                                j = j + 1;
                            }
                        }
                    }
                }
                categorytext = "";
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Category1");
            }
        }


        private void loadtransactionId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    //string query1 = "select coalesce(max(convert(int,tran_id)),0)as tran_id from(SELECT tran_id FROM Transactions where EndDate='" + date + "' union all SELECT distinct TrasactionId FROM Hold)as x";
                    //SqlCommand cmd2 = new SqlCommand(query1, conn);
                    //SqlDataAdapter sdaT = new SqlDataAdapter(cmd2);
                    //DataTable dttranid = new DataTable();
                    //sdaT.Fill(dttranid);

                    if (dttranid.Rows.Count != 0)
                    {
                        lblTranid.Content = Convert.ToInt32(dttranid.Rows[0]["tran_id"].ToString()) + 1;
                        transId = Convert.ToInt32(dttranid.Rows[0]["tran_id"].ToString()) + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "loadtransactionId");
            }
        }
        string taxrate = "";
        void button_Click(object sender, RoutedEventArgs e, string xyz, string abc)
        {
            try
            {
                //var btnContent = sender as Button;
                //var tb = (TextBlock)btnContent.Content.;
                taxrate = abc;
                lblDepartment.Content = xyz;
                TxtBxStackPanel2.Visibility = Visibility.Visible;
                ugDepartment.Visibility = Visibility.Hidden;
                ugDepartment1.Visibility = Visibility.Hidden;
                txtDeptAmt.Focus();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "button_Click");
            }
        }
        private void Button_Click_Go_Back(object sender, RoutedEventArgs e)
        {
            try
            {
                ugDepartment.Visibility = Visibility.Visible;
                TxtBxStackPanel2.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_Go_Back");
            }
        }
        private void Button_Click_Sale_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal value;
                if (decimal.TryParse(txtDeptAmt.Text, out value))
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = 0;
                    dr[1] = lblDepartment.Content.ToString();
                    dr[2] = Convert.ToDecimal(txtDeptAmt.Text).ToString("0.00");
                    dr[3] = taxrate;
                    if (refund == "")
                        dr[4] = 1;
                    else
                        dr[4] = -1;
                    dr[5] = (decimal.Parse(txtDeptAmt.Text) * 1).ToString("0.00");
                    dt.Rows.Add(dr);
                    JRDGrid.ItemsSource = dt.DefaultView;
                    JRDGrid.Items.Refresh();
                    JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                    JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                    TotalEvent();
                    txtDeptAmt.Text = "";
                    ugDepartment.Visibility = Visibility.Visible;
                    TxtBxStackPanel2.Visibility = Visibility.Hidden;
                    textBox1.Focus();
                }
                else
                    MessageBox.Show("Please Enter Valid Value");
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_Sale_Save");
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);

                if (e.Key == Key.Enter || e.Key == Key.Tab)
                {

                    BarcodeMethod();
                    //grPayment.Visibility = Visibility.Hidden;
                    //ugDepartment.Visibility = Visibility.Visible;
                    //ugDepartment1.Visibility = Visibility.Hidden;
                    //btnShortKey.Visibility = Visibility.Visible;
                    //btnDept.Visibility = Visibility.Hidden;
                    //ugCategory1.Visibility = Visibility.Hidden;
                    //ugCategory2.Visibility = Visibility.Hidden;
                    //ugAddcategory1.Visibility = Visibility.Hidden;
                    //ugAddcategory2.Visibility = Visibility.Hidden;
                    //GoBack.Visibility = Visibility.Hidden;
                    //gCustomer.Visibility = Visibility.Hidden;
                    //uGHold.Visibility = Visibility.Visible;
                    //gPriceCheck.Visibility = Visibility.Hidden;
                    //var code = textBox1.Text;
                    //var length = code.Length;
                    //if (length == 12)
                    //{
                    //    code = code.Remove(code.Length - 1);
                    //}
                    //if (length == 8)
                    //{
                    //    var last1 = code.Remove(code.Length - 1);
                    //    var last2 = last1.Substring(last1.Length - 1);
                    //    var first3 = code.Remove(code.Length - 5);
                    //    var first4 = code.Remove(code.Length - 4);
                    //    var last5 = code.Substring(code.Length - 5);
                    //    var second3 = last5.Remove(last5.Length - 2);
                    //    var last4 = code.Substring(code.Length - 4);
                    //    var second2 = last4.Remove(last4.Length - 2);
                    //    if (Convert.ToInt32(last2) == 0)
                    //    {
                    //        code = first3 + "00000" + second3;
                    //    }
                    //    else if (Convert.ToInt32(last2) == 1)
                    //    {
                    //        code = first3 + "10000" + second3;
                    //    }
                    //    else if (Convert.ToInt32(last2) == 3)
                    //    {
                    //        code = first4 + "00000" + second2;
                    //    }
                    //    else if (Convert.ToInt32(last2) == 4)
                    //    {
                    //        code = code.Remove(code.Length - 3) + "00000" + code.Substring(code.Length - 3).Remove(code.Substring(code.Length - 3).Length - 2);
                    //    }
                    //    else if (Convert.ToInt32(last2) == 2)
                    //    {
                    //        code = first3 + "20000" + second3;
                    //    }
                    //    else
                    //    {
                    //        int num = 0;
                    //        code = code.Remove(code.Length - 2) + num + num + num + num + last2;
                    //    }
                    //}
                    //textBox1.Text = code;

                    //var results = from myRow in dtItem.AsEnumerable()
                    //              where myRow.Field<string>("ScanCode") == code
                    //              select myRow;

                    //foreach (DataRow row in results)
                    //{
                    //    DataRow newRow = dt.NewRow();
                    //    newRow["ScanCode"] = row.ItemArray[0].ToString();
                    //    newRow["Description"] = row.ItemArray[1].ToString();
                    //    if (refund == "")
                    //        newRow["Quantity"] = 1;
                    //    else
                    //        newRow["Quantity"] = -1;
                    //    newRow["UnitRetail"] = row.ItemArray[2].ToString();
                    //    newRow["Amount"] = row.ItemArray[2].ToString();
                    //    newRow["OPrice"] = row.ItemArray[2].ToString();
                    //    newRow["TaxRate"] = row.ItemArray[3].ToString();
                    //    newRow["PromotionId"] = row.ItemArray[4].ToString();
                    //    newRow["bIsTrue"] = "0";
                    //    //newRow["PROName"] = row.ItemArray[4].ToString();
                    //    //newRow["Qty"] = row.ItemArray[5].ToString();
                    //    //newRow["NewPrice"] = row.ItemArray[6].ToString();
                    //    //newRow["Discount"] = row.ItemArray[7].ToString();
                    //    //newRow["RPROName"] = row.ItemArray[8].ToString();
                    //    //newRow["RQty"] = row.ItemArray[9].ToString();
                    //    //newRow["RNewPrice"] = row.ItemArray[10].ToString();
                    //    //newRow["RDiscount"] = row.ItemArray[11].ToString();
                    //    //newRow["LPROName"] = row.ItemArray[12].ToString();
                    //    //newRow["LQty"] = row.ItemArray[13].ToString();
                    //    //newRow["LNewPrice"] = row.ItemArray[14].ToString();
                    //    //newRow["LDiscount"] = row.ItemArray[15].ToString();
                    //    //newRow["OPROName"] = row.ItemArray[16].ToString();
                    //    //newRow["OQty"] = row.ItemArray[17].ToString();
                    //    //newRow["ONewPrice"] = row.ItemArray[18].ToString();
                    //    //newRow["ODiscount"] = row.ItemArray[19].ToString();
                    //    //newRow["Type"] = row.ItemArray[20].ToString();
                    //    //newRow["RType"] = row.ItemArray[21].ToString();
                    //    //newRow["LType"] = row.ItemArray[22].ToString();
                    //    //newRow["OType"] = row.ItemArray[23].ToString();
                    //    dt.Rows.Add(newRow);
                    //}
                    //IEnumerable<DataRow> dt1 = (from row in dt.AsEnumerable()
                    //                            where row["Void"].ToString() != "1"
                    //                            select row).ToList<DataRow>();

                    //int dCount = dt.AsEnumerable().Count() - 1;
                    //if (dCount >= 0)
                    //{
                    //    if (loyaltyCustomerCount <= 5)
                    //    {
                    //        foreach (var item in dt.AsEnumerable())
                    //        {
                    //            int imdex = 0;
                    //            DataTable dataTableDistr = new DataTable();
                    //            dataTableDistr.Columns.Add("PromotionId");
                    //            dataTableDistr.Columns.Add("PromotionName");
                    //            dataTableDistr.Columns.Add("newprice");
                    //            dataTableDistr.Columns.Add("Quantity");
                    //            dataTableDistr.Columns.Add("Discount");
                    //            dataTableDistr.Columns.Add("Type");

                    //            var PromotionIdSpl = item["PromotionId"].ToString().Split(',').ToList();
                    //            foreach (string itemSpl in PromotionIdSpl)
                    //            {
                    //                foreach (var drObj in dtPromotion.AsEnumerable().Where(z => z["PromotionId"].ToString() == itemSpl).AsEnumerable())
                    //                {
                    //                    DataRow dr = drObj;
                    //                    dataTableDistr.Rows.Add(dr.ItemArray);
                    //                }
                    //                imdex++;
                    //            }

                    //            foreach (var itemPromo in dataTableDistr.AsEnumerable())
                    //            {
                    //                int sumCount = dt.AsEnumerable()
                    //                    .Where(x => x["PromotionId"].ToString().Split(',').Contains(itemPromo["PromotionId"].ToString())).ToList().Sum(s => Convert.ToInt32(s.Field<string>("Quantity")));// .Select(s =>  Convert.ToInt32(s.Field<string>("Quantity")));
                    //                                                                                                                                                                                      // MessageBox.Show(i + " " + itemPromo["PromotionId"] + " Sum " + sumCount);
                    //                foreach (var promotionspl in PromotionIdSpl.AsEnumerable())
                    //                {
                    //                    if (promotionspl == itemPromo["PromotionId"].ToString())
                    //                    {
                    //                        //if (lblLoyaltyId != null && lblLoyaltyId.Content != null)
                    //                        //{
                    //                        //    if (lblLoyaltyId.Content.ToString() != "")
                    //                        //    {
                    //                        if (itemPromo["Type"].ToString() == "Multy")
                    //                        {
                    //                            int _qty = Convert.ToInt32(itemPromo["Quantity"]);

                    //                            if (sumCount > _qty)
                    //                            {
                    //                                if ((sumCount % _qty) == 0)
                    //                                {
                    //                                    var v = sumCount % _qty;
                    //                                    if (sumCount != _qty)
                    //                                        _qty = sumCount;
                    //                                }
                    //                            }

                    //                            if (sumCount == _qty && Convert.ToInt32(item["bIsTrue"]) != 1)
                    //                            {
                    //                                string price = "";
                    //                                if (itemPromo["NewPrice"].ToString() != "" && itemPromo["NewPrice"].ToString() != "0")
                    //                                    price = (Convert.ToDecimal(itemPromo["NewPrice"]) / Convert.ToInt32(itemPromo["Quantity"])).ToString("0.00");

                    //                                if (price == "")
                    //                                {
                    //                                    price = (Convert.ToDecimal(item["UnitRetail"]) - Convert.ToDecimal(itemPromo["Discount"])).ToString("0.00");
                    //                                }
                    //                                item["UnitRetail"] = price;
                    //                                item["Amount"] = Convert.ToDecimal(item["UnitRetail"]) * Convert.ToDecimal(item["Quantity"]);
                    //                                item["bIsTrue"] = "1";
                    //                                // MessageBox.Show(" Price " + price);
                    //                            }
                    //                        }
                    //                        else
                    //                        {
                    //                            if (sumCount == Convert.ToInt32(itemPromo["Quantity"]))
                    //                            {
                    //                                string price = "";
                    //                                if (itemPromo["NewPrice"].ToString() != "" && itemPromo["NewPrice"].ToString() != "0")
                    //                                    price = (Convert.ToDecimal(itemPromo["NewPrice"]) / Convert.ToInt32(itemPromo["Quantity"])).ToString("0.00");

                    //                                if (price == "")
                    //                                {
                    //                                    price = (Convert.ToDecimal(item["UnitRetail"]) - Convert.ToDecimal(itemPromo["Discount"])).ToString("0.00");
                    //                                }
                    //                                item["UnitRetail"] = price;
                    //                                item["Amount"] = Convert.ToDecimal(item["UnitRetail"]) * Convert.ToDecimal(item["Quantity"]);
                    //                            }
                    //                        }
                    //                        //    }
                    //                        //}
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }

                    //    //foreach (var itemPromo in dtPromotion.AsEnumerable())
                    //    //{
                    //    //    int sumCount = dt.AsEnumerable()
                    //    //        .Where(x => x["PromotionId"].ToString().Split(',').Contains(itemPromo["PromotionId"].ToString())).ToList().Sum(s => Convert.ToInt32(s.Field<string>("Quantity")));// .Select(s =>  Convert.ToInt32(s.Field<string>("Quantity")));
                    //    //    int i = 1;
                    //    //    foreach (var item in dt.AsEnumerable())
                    //    //    {
                    //    //        // MessageBox.Show(i + " " + itemPromo["PromotionId"] + " Sum " + sumCount);
                    //    //        var PromotionIdSpl = item["PromotionId"].ToString().Split(',').ToList();
                    //    //        foreach (var promotionspl in PromotionIdSpl.AsEnumerable())
                    //    //        {

                    //    //            if (promotionspl == itemPromo["PromotionId"].ToString())
                    //    //            {
                    //    //                if (itemPromo["Type"].ToString() == "Once")
                    //    //                {
                    //    //                    if (sumCount == Convert.ToInt32(itemPromo["Quantity"]) * i)
                    //    //                    {
                    //    //                        string price = "";
                    //    //                        if (itemPromo["NewPrice"].ToString() != "" && itemPromo["NewPrice"].ToString() != "0")
                    //    //                            price = (Convert.ToDecimal(itemPromo["NewPrice"]) / Convert.ToInt32(itemPromo["Quantity"])).ToString("0.00");

                    //    //                        if (price == "")
                    //    //                        {
                    //    //                            price = (Convert.ToDecimal(item["UnitRetail"]) - Convert.ToDecimal(itemPromo["Discount"])).ToString("0.00");
                    //    //                        }
                    //    //                        item["UnitRetail"] = price;
                    //    //                        item["Amount"] = Convert.ToDecimal(item["UnitRetail"]) * Convert.ToDecimal(item["Quantity"]);

                    //    //                        // MessageBox.Show(" Price " + price);
                    //    //                    }
                    //    //                }
                    //    //            }
                    //    //        }
                    //    //        i++;
                    //    //    }
                    //    //}
                    //    //  }

                    //    //if (dt.Rows[dCount]["PROName"].ToString() != "")
                    //    //{
                    //    //    DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "PROName", "Qty", "NewPrice", "Discount");

                    //    //    foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                    //    //    {
                    //    //        if (distinct["PROName"].ToString() != "")
                    //    //        {
                    //    //            int sumCount = (from row in dt.AsEnumerable()
                    //    //                            where row.Field<string>("PROName") == distinct["PROName"].ToString()
                    //    //                            select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                    //    //            if (sumCount < 0)
                    //    //                sumCount = sumCount * -1;
                    //    //            foreach (var itemdt in dt.AsEnumerable())
                    //    //            {
                    //    //                if (itemdt["PROName"].ToString() == distinct["PROName"].ToString())
                    //    //                {
                    //    //                    for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                    //    //                    {
                    //    //                        if (itemdt["Type"].ToString() == "Once")
                    //    //                        {
                    //    //                            if (sumCount == Convert.ToInt32(distinct["Qty"]))
                    //    //                            {
                    //    //                                if (itemdt["SPromotionName"] != itemdt["PROName"])
                    //    //                                {
                    //    //                                    string price = "";
                    //    //                                    if (itemdt["NewPrice"].ToString() != "" && itemdt["NewPrice"].ToString() != "0")
                    //    //                                        price = (Convert.ToDecimal(itemdt["NewPrice"]) / Convert.ToInt32(itemdt["Qty"])).ToString("0.00");

                    //    //                                    if (price == "")
                    //    //                                    {
                    //    //                                        decimal odisc = 0;
                    //    //                                        decimal ldisc = 0;
                    //    //                                        decimal rdisc = 0;
                    //    //                                        if (itemdt["OPromotionName"].ToString() != "")
                    //    //                                            odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                    //    //                                        if (itemdt["LPromotionName"].ToString() != "")
                    //    //                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                    //    //                                        if (itemdt["RPromotionName"].ToString() != "")
                    //    //                                            rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                    //    //                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - odisc - ldisc - rdisc - Convert.ToDecimal(itemdt["Discount"])).ToString("0.00");
                    //    //                                    }
                    //    //                                    itemdt["SPromotionName"] = itemdt["PROName"];
                    //    //                                    itemdt["UnitRetail"] = price;
                    //    //                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                    //    //                                    itemdt["PromotionName"] = "";
                    //    //                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                    //    //                                }
                    //    //                            }
                    //    //                        }
                    //    //                        else
                    //    //                        {
                    //    //                            if (sumCount == Convert.ToInt32(distinct["Qty"]) * i)
                    //    //                            {
                    //    //                                if (itemdt["SPromotionName"] != itemdt["PROName"])
                    //    //                                {
                    //    //                                    string price = "";
                    //    //                                    if (itemdt["NewPrice"].ToString() != "" && itemdt["NewPrice"].ToString() != "0")
                    //    //                                        price = (Convert.ToDecimal(itemdt["NewPrice"]) / Convert.ToInt32(itemdt["Qty"])).ToString("0.00");

                    //    //                                    if (price == "")
                    //    //                                    {
                    //    //                                        decimal odisc = 0;
                    //    //                                        decimal ldisc = 0;
                    //    //                                        decimal rdisc = 0;
                    //    //                                        if (itemdt["OPromotionName"].ToString() != "")
                    //    //                                            odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                    //    //                                        if (itemdt["LPromotionName"].ToString() != "")
                    //    //                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                    //    //                                        if (itemdt["RPromotionName"].ToString() != "")
                    //    //                                            rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                    //    //                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - odisc - ldisc - rdisc - Convert.ToDecimal(itemdt["Discount"])).ToString("0.00");
                    //    //                                    }
                    //    //                                    itemdt["SPromotionName"] = itemdt["PROName"];
                    //    //                                    itemdt["UnitRetail"] = price;
                    //    //                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                    //    //                                    itemdt["PromotionName"] = "";
                    //    //                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                    //    //                                }
                    //    //                            }
                    //    //                        }
                    //    //                    }
                    //    //                }
                    //    //            }
                    //    //        }
                    //    //    }
                    //    //}
                    //    //if (dt.Rows[dCount]["RPROName"].ToString() != "")
                    //    //{
                    //    //    DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "RPROName", "RQty", "RNewPrice", "RDiscount");

                    //    //    foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                    //    //    {
                    //    //        if (distinct["RPROName"].ToString() != "")
                    //    //        {
                    //    //            int sumCount = (from row in dt.AsEnumerable()
                    //    //                            where row.Field<string>("RPROName") == distinct["RPROName"].ToString()
                    //    //                            select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                    //    //            if (sumCount < 0)
                    //    //                sumCount = sumCount * -1;
                    //    //            foreach (var itemdt in dt.AsEnumerable())
                    //    //            {
                    //    //                if (itemdt["RPROName"].ToString() == distinct["RPROName"].ToString())
                    //    //                {
                    //    //                    for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                    //    //                    {
                    //    //                        if (itemdt["RType"].ToString() == "Once")
                    //    //                        {
                    //    //                            if (sumCount == Convert.ToInt32(distinct["RQty"]))
                    //    //                            {
                    //    //                                if (itemdt["RPromotionName"] != itemdt["RPROName"])
                    //    //                                {
                    //    //                                    string price = "";
                    //    //                                    if (itemdt["RNewPrice"].ToString() != "" && itemdt["RNewPrice"].ToString() != "0")
                    //    //                                        price = (Convert.ToDecimal(itemdt["RNewPrice"]) / Convert.ToInt32(itemdt["RQty"])).ToString("0.00");
                    //    //                                    if (price == "")
                    //    //                                    {
                    //    //                                        decimal odisc = 0;
                    //    //                                        decimal ldisc = 0;
                    //    //                                        decimal sdisc = 0;
                    //    //                                        if (itemdt["OPromotionName"].ToString() != "")
                    //    //                                            odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                    //    //                                        if (itemdt["LPromotionName"].ToString() != "")
                    //    //                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                    //    //                                        if (itemdt["SPromotionName"].ToString() != "")
                    //    //                                            sdisc = Convert.ToDecimal(itemdt["Discount"]);
                    //    //                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - sdisc - ldisc - odisc - Convert.ToDecimal(itemdt["RDiscount"])).ToString("0.00");
                    //    //                                    }
                    //    //                                    itemdt["RPromotionName"] = itemdt["RPROName"];
                    //    //                                    itemdt["UnitRetail"] = price;
                    //    //                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                    //    //                                    itemdt["PromotionName"] = "";
                    //    //                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                    //    //                                }
                    //    //                            }
                    //    //                        }
                    //    //                        else
                    //    //                        {
                    //    //                            if (sumCount == Convert.ToInt32(distinct["RQty"]) * i)
                    //    //                            {
                    //    //                                if (itemdt["RPromotionName"] != itemdt["RPROName"])
                    //    //                                {

                    //    //                                    string price = "";


                    //    //                                    if (itemdt["RNewPrice"].ToString() != "" && itemdt["RNewPrice"].ToString() != "0")
                    //    //                                        price = (Convert.ToDecimal(itemdt["RNewPrice"]) / Convert.ToInt32(itemdt["RQty"])).ToString("0.00");

                    //    //                                    if (price == "")
                    //    //                                    {
                    //    //                                        decimal odisc = 0;
                    //    //                                        decimal ldisc = 0;
                    //    //                                        decimal sdisc = 0;
                    //    //                                        if (itemdt["OPromotionName"].ToString() != "")
                    //    //                                            odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                    //    //                                        if (itemdt["LPromotionName"].ToString() != "")
                    //    //                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                    //    //                                        if (itemdt["SPromotionName"].ToString() != "")
                    //    //                                            sdisc = Convert.ToDecimal(itemdt["Discount"]);

                    //    //                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - sdisc - ldisc - odisc - Convert.ToDecimal(itemdt["RDiscount"])).ToString("0.00");
                    //    //                                    }
                    //    //                                    itemdt["RPromotionName"] = itemdt["RPROName"];
                    //    //                                    itemdt["UnitRetail"] = price;
                    //    //                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);

                    //    //                                    itemdt["PromotionName"] = "";
                    //    //                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                    //    //                                }
                    //    //                            }
                    //    //                        }
                    //    //                    }
                    //    //                }
                    //    //            }
                    //    //        }
                    //    //    }
                    //    //}
                    //    //if (dt.Rows[dCount]["LPROName"].ToString() != "")
                    //    //{
                    //    //    if (loyaltyCustomerCount <= 5)
                    //    //    {
                    //    //        DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "LPROName", "LQty", "LNewPrice", "LDiscount");

                    //    //        foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                    //    //        {
                    //    //            if (distinct["LPROName"].ToString() != "")
                    //    //            {
                    //    //                int sumCount = (from row in dt.AsEnumerable()
                    //    //                                where row.Field<string>("LPROName") == distinct["LPROName"].ToString() && row.Field<string>("Void") != "1"
                    //    //                                select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                    //    //                if (sumCount < 0)
                    //    //                    sumCount = sumCount * -1;
                    //    //                foreach (var itemdt in dt.AsEnumerable())
                    //    //                {
                    //    //                    if (itemdt["LPROName"].ToString() == distinct["LPROName"].ToString())
                    //    //                    {
                    //    //                        for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                    //    //                        {
                    //    //                            if (itemdt["LType"].ToString() == "Once")
                    //    //                            {
                    //    //                                if (sumCount == Convert.ToInt32(distinct["LQty"]))
                    //    //                                {
                    //    //                                    if (lblLoyaltyId.Content is null)
                    //    //                                        lblLoyaltyId.Content = "";
                    //    //                                    if (lblLoyaltyId.Content.ToString() != "")
                    //    //                                    {
                    //    //                                        if (itemdt["LPromotionName"] != itemdt["LPROName"])
                    //    //                                        {
                    //    //                                            string price = "";
                    //    //                                            if (itemdt["LNewPrice"].ToString() != "" && itemdt["LNewPrice"].ToString() != "0")
                    //    //                                                price = (Convert.ToDecimal(itemdt["LNewPrice"]) / Convert.ToInt32(itemdt["LQty"])).ToString("0.00");

                    //    //                                            if (price == "")
                    //    //                                            {
                    //    //                                                decimal odisc = 0;
                    //    //                                                decimal sdisc = 0;
                    //    //                                                decimal rdisc = 0;
                    //    //                                                if (itemdt["OPromotionName"].ToString() != "")
                    //    //                                                    odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                    //    //                                                if (itemdt["SPromotionName"].ToString() != "")
                    //    //                                                    sdisc = Convert.ToDecimal(itemdt["Discount"]);
                    //    //                                                if (itemdt["RPromotionName"].ToString() != "")
                    //    //                                                    rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                    //    //                                                price = (Convert.ToDecimal(itemdt["Oprice"]) - odisc - sdisc - rdisc - (Convert.ToDecimal(itemdt["LDiscount"]) / Convert.ToInt32(itemdt["LQty"]))).ToString("0.00");
                    //    //                                            }

                    //    //                                            itemdt["LPromotionName"] = itemdt["LPROName"];
                    //    //                                            itemdt["UnitRetail"] = price;
                    //    //                                            itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                    //    //                                            itemdt["LoyaltyId"] = lblLoyaltyId.Content.ToString();

                    //    //                                            itemdt["PromotionName"] = "";
                    //    //                                            itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                    //    //                                        }
                    //    //                                    }
                    //    //                                }
                    //    //                            }
                    //    //                            else
                    //    //                            {
                    //    //                                if (sumCount == Convert.ToInt32(distinct["LQty"]) * i)
                    //    //                                {
                    //    //                                    if (lblLoyaltyId.Content is null)
                    //    //                                        lblLoyaltyId.Content = "";
                    //    //                                    if (lblLoyaltyId.Content.ToString() != "")
                    //    //                                    {
                    //    //                                        if (itemdt["LPromotionName"] != itemdt["LPROName"])
                    //    //                                        {
                    //    //                                            string price = "";
                    //    //                                            if (itemdt["LNewPrice"].ToString() != "" && itemdt["LNewPrice"].ToString() != "0")
                    //    //                                                price = (Convert.ToDecimal(itemdt["LNewPrice"]) / Convert.ToInt32(itemdt["LQty"])).ToString("0.00");

                    //    //                                            if (price == "")
                    //    //                                            {
                    //    //                                                decimal odisc = 0;
                    //    //                                                decimal sdisc = 0;
                    //    //                                                decimal rdisc = 0;
                    //    //                                                if (itemdt["OPromotionName"].ToString() != "")
                    //    //                                                    odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                    //    //                                                if (itemdt["SPromotionName"].ToString() != "")
                    //    //                                                    sdisc = Convert.ToDecimal(itemdt["Discount"]);
                    //    //                                                if (itemdt["RPromotionName"].ToString() != "")
                    //    //                                                    rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                    //    //                                                price = (Convert.ToDecimal(itemdt["Oprice"]) - odisc - sdisc - rdisc - (Convert.ToDecimal(itemdt["LDiscount"]) / Convert.ToInt32(itemdt["LQty"]))).ToString("0.00");
                    //    //                                            }

                    //    //                                            itemdt["LPromotionName"] = itemdt["LPROName"];
                    //    //                                            itemdt["UnitRetail"] = price;
                    //    //                                            itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                    //    //                                            itemdt["LoyaltyId"] = lblLoyaltyId.Content.ToString();

                    //    //                                            itemdt["PromotionName"] = "";
                    //    //                                            itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                    //    //                                        }
                    //    //                                    }
                    //    //                                }
                    //    //                            }
                    //    //                        }
                    //    //                    }
                    //    //                }
                    //    //            }
                    //    //        }
                    //    //    }
                    //    //}
                    //    //if (dt.Rows[dCount]["OPROName"].ToString() != "")
                    //    //{
                    //    //    DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "OPROName", "OQty", "ONewPrice", "ODiscount");

                    //    //    foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                    //    //    {
                    //    //        if (distinct["OPROName"].ToString() != "")
                    //    //        {
                    //    //            int sumCount = (from row in dt.AsEnumerable()
                    //    //                            where row.Field<string>("OPROName") == distinct["OPROName"].ToString() && row.Field<string>("Void") != "1"
                    //    //                            select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                    //    //            if (sumCount < 0)
                    //    //                sumCount = sumCount * -1;
                    //    //            foreach (var itemdt in dt.AsEnumerable())
                    //    //            {
                    //    //                if (itemdt["OPROName"].ToString() == distinct["OPROName"].ToString())
                    //    //                {
                    //    //                    for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                    //    //                    {
                    //    //                        if (itemdt["OType"].ToString() == "Once")
                    //    //                        {
                    //    //                            if (sumCount == Convert.ToInt32(distinct["OQty"]))
                    //    //                            {
                    //    //                                if (itemdt["OPromotionName"] != itemdt["OPROName"])
                    //    //                                {
                    //    //                                    string price = "";
                    //    //                                    if (itemdt["ONewPrice"].ToString() != "" && itemdt["ONewPrice"].ToString() != "0")
                    //    //                                        price = (Convert.ToDecimal(itemdt["ONewPrice"]) / Convert.ToInt32(itemdt["OQty"])).ToString("0.00");

                    //    //                                    if (price == "")
                    //    //                                    {
                    //    //                                        decimal ldisc = 0;
                    //    //                                        decimal sdisc = 0;
                    //    //                                        decimal rdisc = 0;
                    //    //                                        if (itemdt["LPromotionName"].ToString() != "")
                    //    //                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                    //    //                                        if (itemdt["SPromotionName"].ToString() != "")
                    //    //                                            sdisc = Convert.ToDecimal(itemdt["Discount"]);
                    //    //                                        if (itemdt["RPromotionName"].ToString() != "")
                    //    //                                            rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                    //    //                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - ldisc - sdisc - rdisc - Convert.ToDecimal(itemdt["ODiscount"])).ToString("0.00");
                    //    //                                    }

                    //    //                                    itemdt["OPromotionName"] = itemdt["OPROName"];
                    //    //                                    itemdt["UnitRetail"] = price;
                    //    //                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);

                    //    //                                    itemdt["PromotionName"] = "";
                    //    //                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                    //    //                                }
                    //    //                            }
                    //    //                        }
                    //    //                        else
                    //    //                        {
                    //    //                            if (sumCount == Convert.ToInt32(distinct["OQty"]) * i)
                    //    //                            {
                    //    //                                if (itemdt["OPromotionName"] != itemdt["OPROName"])
                    //    //                                {
                    //    //                                    string price = "";
                    //    //                                    if (itemdt["ONewPrice"].ToString() != "" && itemdt["ONewPrice"].ToString() != "0")
                    //    //                                        price = (Convert.ToDecimal(itemdt["ONewPrice"]) / Convert.ToInt32(itemdt["OQty"])).ToString("0.00");

                    //    //                                    if (price == "")
                    //    //                                    {
                    //    //                                        decimal ldisc = 0;
                    //    //                                        decimal sdisc = 0;
                    //    //                                        decimal rdisc = 0;
                    //    //                                        if (itemdt["LPromotionName"].ToString() != "")
                    //    //                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                    //    //                                        if (itemdt["SPromotionName"].ToString() != "")
                    //    //                                            sdisc = Convert.ToDecimal(itemdt["Discount"]);
                    //    //                                        if (itemdt["RPromotionName"].ToString() != "")
                    //    //                                            rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                    //    //                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - ldisc - sdisc - rdisc - Convert.ToDecimal(itemdt["ODiscount"])).ToString("0.00");
                    //    //                                    }

                    //    //                                    itemdt["OPromotionName"] = itemdt["OPROName"];
                    //    //                                    itemdt["UnitRetail"] = price;
                    //    //                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);

                    //    //                                    itemdt["PromotionName"] = "";
                    //    //                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                    //    //                                }
                    //    //                            }
                    //    //                        }
                    //    //                    }
                    //    //                }
                    //    //            }
                    //    //        }
                    //    //    }
                    //    //}

                    //    JRDGrid.ItemsSource = dt.DefaultView;
                    //    JRDGrid.Items.Refresh();
                    //    JRDGrid.ScrollIntoView(JRDGrid.Items[dCount]);
                    //    JRDGrid.SelectedIndex = dCount;
                    //    TotalEvent();
                    //}
                    //textBox1.Text = "";
                    //if (cbCustomer1.SelectedIndex == 0 && JRDGrid.Items.Count == 1)
                    //{
                    //    MessageBox.Show("Select Customer for apply loyalty");
                    //    gCustomer.Visibility = Visibility.Visible;
                    //    uGHold.Visibility = Visibility.Hidden;
                    //    gPriceCheck.Visibility = Visibility.Hidden;
                    //}

                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "OnKeyDownHandler");
            }
        }

        private void myDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    DataGrid grid = sender as DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                    {
                        DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                        DataRowView dr = (DataRowView)dgr.Item;

                        DataRow newRow = dt.NewRow();
                        newRow["ScanCode"] = dr["ScanCode"].ToString();
                        newRow["Description"] = dr["Description"].ToString();
                        if (refund == "")
                            newRow["Quantity"] = dr["Quantity"].ToString();
                        else
                            newRow["Quantity"] = (Convert.ToInt32(dr["Quantity"]) * -1).ToString();
                        newRow["UnitRetail"] = Convert.ToDecimal(dr["RetailPrice"].ToString());
                        newRow["Amount"] = (Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"])).ToString();
                        newRow["OPrice"] = Convert.ToDecimal(dr["RetailPrice"].ToString());
                        newRow["TaxRate"] = dr["TaxRate"].ToString();
                        newRow["PromotionId"] = dr["PromotionId"].ToString();
                        newRow["bIsTrueId"] = "";
                        dt.Rows.Add(newRow);

                        int dCount = dt.AsEnumerable().Count() - 1;
                        if (dCount >= 0)
                        {
                            PromotionApply();
                            JRDGrid.ScrollIntoView(JRDGrid.Items[dCount]);
                            JRDGrid.SelectedIndex = dCount;
                        }

                        textBox1.Text = "";
                        if (cbCustomer1.SelectedIndex == 0 && JRDGrid.Items.Count == 1)
                        {
                            MessageBox.Show("Select Customer for apply loyalty");
                            gCustomer.Visibility = Visibility.Visible;
                            uGHold.Visibility = Visibility.Hidden;
                            gPriceCheck.Visibility = Visibility.Hidden;
                        }
                    }
                }
                popgrid.Visibility = Visibility.Hidden;
                ugDepartment.Visibility = Visibility.Visible;
                ugDepartment1.Visibility = Visibility.Hidden;
                ugAddcategory1.Visibility = Visibility.Hidden;
                ugAddcategory2.Visibility = Visibility.Hidden;
                ugCategory1.Visibility = Visibility.Hidden;
                ugCategory2.Visibility = Visibility.Hidden;
                grPayment.Visibility = Visibility.Hidden;
                TxtBxStackPanel2.Visibility = Visibility.Hidden;
                gReceipt.Visibility = Visibility.Hidden;
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "myDataGrid_MouseDoubleClick"); }
        }

        public void BarcodeMethod()
        {
            try
            {
                grPayment.Visibility = Visibility.Hidden;
                ugDepartment.Visibility = Visibility.Visible;
                ugDepartment1.Visibility = Visibility.Hidden;
                btnShortKey.Visibility = Visibility.Visible;
                btnDept.Visibility = Visibility.Hidden;
                ugCategory1.Visibility = Visibility.Hidden;
                ugCategory2.Visibility = Visibility.Hidden;
                ugAddcategory1.Visibility = Visibility.Hidden;
                ugAddcategory2.Visibility = Visibility.Hidden;
                GoBack.Visibility = Visibility.Hidden;
                gCustomer.Visibility = Visibility.Hidden;
                uGHold.Visibility = Visibility.Visible;
                gPriceCheck.Visibility = Visibility.Hidden;
                var code = textBox1.Text;
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
                textBox1.Text = code;

                var results = from myRow in dtItem.AsEnumerable()
                              where myRow.Field<string>("ScanCode") == code
                              select myRow;


                if (results.Count() > 1)
                {
                    DataTable objDT = new DataTable();
                    objDT.Columns.Add("ScanCode");
                    objDT.Columns.Add("Description");
                    objDT.Columns.Add("Quantity");
                    objDT.Columns.Add("RetailPrice");
                    objDT.Columns.Add("TaxRate");
                    objDT.Columns.Add("PromotionId");
                    objDT.Clear();

                    foreach (var itemObj in results.AsEnumerable())
                    {
                        DataRow dr = objDT.NewRow();
                        dr["ScanCode"] = itemObj.Field<string>("ScanCode");
                        dr["Description"] = itemObj.Field<string>("Description");
                        dr["Quantity"] = itemObj.Field<int>("Quantity");
                        dr["RetailPrice"] = itemObj.Field<string>("RetailPrice");
                        dr["TaxRate"] = itemObj.Field<string>("TaxRate");
                        dr["PromotionId"] = itemObj.Field<string>("PromotionId");
                        objDT.Rows.Add(dr);
                    }
                    popgrid.ItemsSource = objDT.DefaultView;
                    popgrid.Visibility = Visibility.Visible;

                    ugDepartment.Visibility = Visibility.Hidden;
                    ugDepartment1.Visibility = Visibility.Hidden;
                    ugAddcategory1.Visibility = Visibility.Hidden;
                    ugAddcategory2.Visibility = Visibility.Hidden;
                    ugCategory1.Visibility = Visibility.Hidden;
                    ugCategory2.Visibility = Visibility.Hidden;
                    grPayment.Visibility = Visibility.Hidden;
                    TxtBxStackPanel2.Visibility = Visibility.Hidden;
                    gReceipt.Visibility = Visibility.Hidden;

                }
                else
                {
                    foreach (DataRow row in results)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["ScanCode"] = row["ScanCode"].ToString();
                        newRow["Description"] = row["Description"].ToString();
                        if (refund == "")
                            newRow["Quantity"] = 1;
                        else
                            newRow["Quantity"] = -1;
                        newRow["UnitRetail"] = row["UnitRetail"].ToString();
                        newRow["Amount"] = (Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(row["UnitRetail"])).ToString();
                        newRow["OPrice"] = row["UnitRetail"].ToString();
                        newRow["TaxRate"] = row["TaxRate"].ToString();
                        newRow["PromotionId"] = row["PromotionId"].ToString();
                        newRow["bIsTrueId"] = "";

                        dt.Rows.Add(newRow);
                    }

                    int dCount = dt.AsEnumerable().Count() - 1;
                    if (dCount >= 0)
                    {
                        PromotionApply();
                        JRDGrid.ScrollIntoView(JRDGrid.Items[dCount]);
                        JRDGrid.SelectedIndex = dCount;
                    }

                    textBox1.Text = "";
                    if (cbCustomer1.SelectedIndex == 0 && JRDGrid.Items.Count == 1)
                    {
                        MessageBox.Show("Select Customer for apply loyalty");
                        gCustomer.Visibility = Visibility.Visible;
                        uGHold.Visibility = Visibility.Hidden;
                        gPriceCheck.Visibility = Visibility.Hidden;
                    }
                }
            }

            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "BarcodeMethod");
            }
        }

        private void Tender_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tenderCode = (((((sender as Button).Content) as Grid).Children[0]) as Label).Content.ToString();
                if (tenderCode == "Cash")
                {
                    cashTxtPanel.Visibility = Visibility.Visible;
                    sp02.Visibility = Visibility.Hidden;
                    customerTxtPanel.Visibility = Visibility.Hidden;
                    checkTxtPanel.Visibility = Visibility.Hidden;
                    TxtCashReceive.Focus();
                }
                if (tenderCode == "Customer")
                {
                    cashTxtPanel.Visibility = Visibility.Hidden;
                    sp02.Visibility = Visibility.Hidden;
                    customerTxtPanel.Visibility = Visibility.Visible;
                    checkTxtPanel.Visibility = Visibility.Hidden;
                }
                if (tenderCode == "Check")
                {
                    checkTxtPanel.Visibility = Visibility.Visible;
                    cashTxtPanel.Visibility = Visibility.Hidden;
                    sp02.Visibility = Visibility.Hidden;
                    customerTxtPanel.Visibility = Visibility.Hidden;
                    TxtCheck.Focus();
                }
                if (tenderCode == "Card")
                {
                    cashTxtPanel.Visibility = Visibility.Hidden;
                    sp02.Visibility = Visibility.Visible;
                    customerTxtPanel.Visibility = Visibility.Hidden;
                    checkTxtPanel.Visibility = Visibility.Hidden;

                    MessageBox.Show("This is card payment !");
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Tender_Click");
            }
        }

        private void TotalEvent()
        {
            try
            {
                decimal sum = 0;
                decimal Qtysum = 0;
                decimal Taxsum = 0;
                decimal Total = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    string voiditem = dr.ItemArray[12].ToString();
                    string amounnt = dr.ItemArray[5].ToString();
                    string tax = dr.ItemArray[3].ToString();
                    if (voiditem != "1")
                    {
                        if (amounnt != "" && tax != "")
                        {
                            sum += decimal.Parse(amounnt);
                            Taxsum += decimal.Parse(tax) * decimal.Parse(amounnt) / 100;
                        }
                        else
                        {
                            sum = 0;
                            Taxsum = 0;
                        }
                        Qtysum += decimal.Parse(dr.ItemArray[4].ToString());
                    }
                }
                Total = sum + Taxsum;
                txtTotal.Content = '$' + sum.ToString("0.00");
                taxtTotal.Content = '$' + Taxsum.ToString("0.00");
                grandTotal.Content = "Pay " + '$' + Total.ToString("0.00");
                lblCount.Content = dt.AsEnumerable()
               .Count(row => row.Field<string>("Void") != "1");
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "TotalEvent");
            }
        }

        private void TxtCashReceive_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Tab || e.Key == Key.Enter)
                {
                    decimal value;
                    if (decimal.TryParse(TxtCashReceive.Text, out value))
                    {
                        TxtCashReturn.Text = decimal.Parse(Convert.ToDecimal(decimal.Parse(TxtCashReceive.Text) - decimal.Parse(grandTotal.Content.ToString().Replace("Pay $", ""))).ToString("0.00")).ToString("0.00");
                        if (decimal.Parse(TxtCashReceive.Text) - decimal.Parse(grandTotal.Content.ToString().Replace("Pay $", "")) >= 0)
                        {
                            Button_Click_1();
                        }
                        else { MessageBox.Show("Cash is Less then Bill Amount"); }
                    }
                    else { MessageBox.Show("Please Enter Valid Value"); }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "TxtCashReceive_KeyDown");
            }
        }

        private void Button_Click_1()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);

                PrintDocument = new PrintDocument();
                PrintDocument.PrintPage += new PrintPageEventHandler(FormatPage);
                PrintDocument.Print();

                foreach (DataRow row in dtVoidItem.Rows)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["ScanCode"] = row.ItemArray[0].ToString();
                    newRow["Description"] = row.ItemArray[1].ToString();
                    newRow["Quantity"] = row.ItemArray[4].ToString();
                    newRow["UnitRetail"] = row.ItemArray[2].ToString();
                    newRow["Amount"] = row.ItemArray[5].ToString();
                    newRow["OPrice"] = row.ItemArray[7].ToString();
                    newRow["TaxRate"] = row.ItemArray[3].ToString();
                    newRow["Void"] = 1;
                    dt.Rows.Add(newRow);
                }
                dtVoidItem.Clear();

                string date = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
                string onlydate = date.Substring(0, 10);
                string onlytime = date.Substring(11);
                string totalAmt = txtTotal.Content.ToString().Replace("$", "");
                string tax = taxtTotal.Content.ToString().Replace("$", "");
                string grandTotalAmt = grandTotal.Content.ToString().Replace("Pay $", "");
                string cashRec = TxtCashReceive.Text.Replace("$ ", "");
                string cashReturn = TxtCashReturn.Text.Replace("$ ", "");
                string tranid = Convert.ToInt32(lblTranid.Content).ToString();

                string transaction = "insert into Transactions(Tran_id,EndDate,EndTime,GrossAmount,TaxAmount,GrandAmount,CreateBy,CreateOn,StoreId,Register_id,POSId)Values('" + tranid + "','" + onlydate + "','" + onlytime + "','" + totalAmt + "','" + tax + "','" + grandTotalAmt + "','" + username + "','" + date + "','" + storeid + "','" + registerid + "','" + posId + "')";
                SqlCommand cmd = new SqlCommand(transaction, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                if (tenderCode == "Cash")
                {
                    string tender = "";
                    if (refund == "")
                        tender = "insert into Tender(EndDate,Endtime,TenderCode,Amount,Change,TransactionId,CreateBy,CreateOn,StoreId,POSId,RegisterId)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + cashRec + "','" + cashReturn + "','" + tranid + "','" + username + "','" + date + "','" + storeid + "','" + posId + "','" + registerid + "')";
                    else
                        tender = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn,StoreId,POSId,RegisterId)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + username + "','" + date + "','" + storeid + "','" + posId + "','" + registerid + "')";
                    SqlCommand cmdTender = new SqlCommand(tender, con);
                    con.Open();
                    cmdTender.ExecuteNonQuery();
                    con.Close();
                }
                else if (tenderCode == "Card")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn,storeid,posid,RegisterId)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + username + "','" + date + "','" + storeid + "','" + posId + "','" + registerid + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                else if (tenderCode == "Customer")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,AccountName,CreateBy,CreateOn,storeid,posid,RegisterId)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + cbcustomer.Text + "','" + username + "','" + date + "','" + storeid + "','" + posId + "','" + registerid + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                else if (tenderCode == "Check")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CheckNo,CreateBy,CreateOn,storeid,posid,RegisterId)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + TxtCheck.Text + "','" + username + "','" + date + "','" + storeid + "','" + posId + "','" + registerid + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                foreach (DataRow dataRow in dt.Rows)
                {
                    dataRow[6] = onlydate;
                    dataRow[7] = onlytime;
                    dataRow[8] = tranid;
                    dataRow[9] = username;
                    dataRow["StoreId"] = storeid;
                    dataRow["POSId"] = posId;
                    dataRow["RegisterId"] = registerid;
                }

                SqlBulkCopy objbulk = new SqlBulkCopy(con);
                objbulk.DestinationTableName = "SalesItem";
                objbulk.ColumnMappings.Add("scanCode", "ScanCode");
                objbulk.ColumnMappings.Add("description", "Descripation");
                objbulk.ColumnMappings.Add("quantity", "Quantity");
                objbulk.ColumnMappings.Add("unitretail", "Price");
                objbulk.ColumnMappings.Add("Amount", "Amount");
                objbulk.ColumnMappings.Add("Date", "EndDate");
                objbulk.ColumnMappings.Add("Time", "EndTime");
                objbulk.ColumnMappings.Add("PromotionName", "PromotionName");
                objbulk.ColumnMappings.Add("TransactionId", "TransactionId");
                objbulk.ColumnMappings.Add("CreateBy", "CreateBy");
                objbulk.ColumnMappings.Add("CreateOn", "CreateOn");
                objbulk.ColumnMappings.Add("Void", "Void");
                objbulk.ColumnMappings.Add("LoyaltyId", "loyaltyId");
                objbulk.ColumnMappings.Add("StoreId", "StoreId");
                objbulk.ColumnMappings.Add("POSId", "POSId");
                objbulk.ColumnMappings.Add("RegisterId", "RegisterId");
                con.Open();
                objbulk.WriteToServer(dt);
                con.Close();
                //PrintDocument = new PrintDocument();
                //PrintDocument.PrintPage += new PrintPageEventHandler(FormatPage);
                //PrintDocument.Print();
                if (lblLoyaltyId.Content.ToString() != "" && lblLoyaltyId.Content == null)
                {
                    var results = from myRow in dtAccount.AsEnumerable()
                                  where myRow.Field<string>("Name") == cbCustomer1.Text
                                  select myRow;
                    foreach (DataRow row in results)
                    {
                        row[2] = Convert.ToInt32(row[2]) + 1;
                    }
                }
                cbcustomer.Text = "";
                lblCount.Content = 0;
                TxtCheck.Text = "";
                txtTotal.Content = "$0.00";
                taxtTotal.Content = "$0.00";
                grandTotal.Content = "Pay " + "$" + "0.00";
                cbCustomer1.Text = "--Select--";
                lblLoyaltyId.Content = "";
                lblDate.Content = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
                dt.Clear();
                JRDGrid.Items.Refresh();
                refund = "";

                customerTxtPanel.Visibility = Visibility.Hidden;
                checkTxtPanel.Visibility = Visibility.Hidden;
                if (tenderCode != "Cash")
                {
                    cashTxtPanel.Visibility = Visibility.Hidden;
                    grPayment.Visibility = Visibility.Hidden;
                    ugDepartment.Visibility = Visibility.Visible;
                    TxtCashReturn.Text = "";
                    TxtCashReceive.Text = "";
                }
                tenderCode = "";

                transId = transId + 1;
                lblTranid.Content = transId;
                textBox1.Focus();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_1");
            }
        }

        private void StoreDetails()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string query = "select * from store where storeid = " + storeid + "";
                SqlCommand cmdstore = new SqlCommand(query, con);
                SqlDataAdapter sdastore = new SqlDataAdapter(cmdstore);
                sdastore.Fill(dtstr);
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "StoreDetails"); }
        }

        private void FormatPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                string count = dt.Rows.Count.ToString();
                graphics = e.Graphics;
                Font minifont = new Font("Arial", 7);
                Font itemfont = new Font("Arial", 8);
                Font smallfont = new Font("Arial", 10);
                Font mediumfont = new Font("Arial", 12);
                Font largefont = new Font("Arial", 14);
                Font headerfont = new Font("Arial", 16);
                int Offset = 10;
                int smallinc = 10, mediuminc = 12, largeinc = 15;
                graphics.DrawString(dtstr.Rows[0]["StoreName"].ToString(), headerfont,
                new SolidBrush(Color.Black), 22 + 22, 22);
                Offset = Offset + largeinc + 20;

                DrawAtStart("   " + dtstr.Rows[0]["Address"].ToString(), Offset);
                Offset = Offset + largeinc;
                DrawAtStart(dtstr.Rows[0]["PhoneNumber"].ToString(), Offset);

                Offset = Offset + largeinc;
                String underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 2);
                Offset = Offset + largeinc;
                graphics.DrawString("Sales Receipt", headerfont, new SolidBrush(Color.Black), 20 + 20, Offset);
                Offset = Offset + largeinc;
                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 3);
                Offset = Offset + largeinc;
                DrawAtStart("Register Id : " + registerid, Offset);
                Offset = Offset + largeinc;
                DrawAtStart("Transaction Id : " + lblTranid.Content, Offset);
                Offset = Offset + largeinc;

                DrawAtStart("Date : " + lblDate.Content, Offset);

                Offset = Offset + largeinc;
                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 2);

                Offset = Offset + largeinc;

                InsertHeaderStyleItem("Name. ", "Qty", "Amount. ", Offset);

                Offset = Offset + largeinc;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    InsertItem(dt.Rows[i]["Scancode"].ToString() + System.Environment.NewLine + dt.Rows[i]["description"].ToString(), dt.Rows[i]["quantity"].ToString(), dt.Rows[i]["Amount"].ToString(), Offset);
                    Offset = Offset + largeinc + 12;
                }

                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 2);

                Offset = Offset + largeinc;
                InsertHeaderStyleItem("Sub Total", "", txtTotal.Content.ToString(), Offset);
                Offset = Offset + largeinc;
                InsertHeaderStyleItem("Tax", "", taxtTotal.Content.ToString(), Offset);
                Offset = Offset + largeinc;
                InsertHeaderStyleItem("Amount Payble", "", grandTotal.Content.ToString().Replace("Pay ", ""), Offset);

                Offset = Offset + largeinc;
                Offset = Offset + largeinc;
                Offset = Offset + largeinc;
                InsertHeaderStyleItem("Total Item Count", "", count, Offset);
                Offset = Offset + largeinc;
                Offset = Offset + largeinc;
                Offset = Offset + 7;
                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 0);

                Offset = Offset + largeinc;
                String greetings = "Thanks for visiting us.";
                DrawSimpleString(greetings, mediumfont, Offset, 28);

                Offset = Offset + largeinc;
                underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 0);

                Offset = Offset + largeinc;
                string DrawnBy = "PSPCStore";
                DrawSimpleString(DrawnBy, minifont, Offset, 15);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "FormatPage");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt.Rows.Count == 0 && dtVoidItem.Rows.Count == 0 && dtHold.Rows.Count == 0)
                {
                    App.Current.Properties["username"] = "";
                    lblusername.Content = "";
                    Login login = new Login();
                    this.Close();
                    login.Show();
                }
                else
                    MessageBox.Show("Please Clear Transaction or Hold Transaction  or  Void Item Transaction ");
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_2");
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
                SendErrorToText(ex, errorFileName, "DrawAtStart");
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
                        new SolidBrush(Color.Black), startX + 210, startY + Offset);
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "InsertItem"); }
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
                         new SolidBrush(Color.Black), startX + 170, startY + Offset);
                graphics.DrawString(value1, itemfont,
                      new SolidBrush(Color.Black), startX + 200, startY + Offset);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "InsertHeaderStyleItem");
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
                SendErrorToText(ex, errorFileName, "DrawLine");
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
                SendErrorToText(ex, errorFileName, "DrawSimpleString");
            }
        }

        private void JRDGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                int rowIn = e.Row.GetIndex();
                if (rowIn == dt.Rows.Count - 1)
                {
                    if (e.EditAction == DataGridEditAction.Commit)
                    {
                        var column = e.Column as DataGridBoundColumn;
                        if (column != null)
                        {
                            var bindingPath = (column.Binding as Binding).Path.Path;
                            if (bindingPath == "Quantity")
                            {
                                int rowIndex = e.Row.GetIndex();
                                var el = e.EditingElement as TextBox;
                                DataRow dataRow = dt.Rows[rowIndex];
                                dt.Rows[rowIndex]["Quantity"] = el.Text;

                                if (dt.Rows[rowIndex]["PROName"].ToString() != "")
                                {
                                    int qDT = Convert.ToInt32(dt.Rows[rowIndex]["Quantity"]);
                                    int qDT1 = Convert.ToInt32(dt.Rows[rowIndex]["Qty"]);

                                    if (qDT >= qDT1)
                                    {
                                        int QA = 0;
                                        if (dt.Rows[rowIndex]["Type"].ToString() == "Once")
                                        {
                                            QA = qDT - qDT1;
                                        }
                                        else
                                        {
                                            QA = qDT1 * (qDT / qDT1);
                                        }
                                        string price = "";
                                        if (dt.Rows[rowIndex]["NewPrice"].ToString() != "")
                                        {
                                            price = (Convert.ToDecimal(dt.Rows[rowIndex]["NewPrice"]) / qDT1).ToString("0.00");
                                        }
                                        else
                                        {
                                            decimal odisc = 0;
                                            decimal ldisc = 0;
                                            decimal rdisc = 0;
                                            if (dt.Rows[rowIndex]["OPromotionName"].ToString() != "")
                                                odisc = Convert.ToDecimal(dt.Rows[rowIndex]["ODiscount"]);
                                            if (dt.Rows[rowIndex]["LPromotionName"].ToString() != "")
                                                ldisc = Convert.ToDecimal(dt.Rows[rowIndex]["LDiscount"]);
                                            if (dt.Rows[rowIndex]["RPromotionName"].ToString() != "")
                                                rdisc = Convert.ToDecimal(dt.Rows[rowIndex]["RDiscount"]);
                                            price = (Convert.ToDecimal(dt.Rows[rowIndex]["Oprice"]) - odisc - ldisc - rdisc - Convert.ToDecimal(dt.Rows[rowIndex]["Discount"]) / qDT1).ToString("0.00");
                                        }
                                        dt.Rows[rowIndex]["UnitRetail"] = price;
                                        dt.Rows[rowIndex]["SPromotionName"] = dt.Rows[rowIndex]["PROName"];
                                        dt.Rows[rowIndex]["Quantity"] = QA;
                                        dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");

                                        int QB = qDT - QA;
                                        if (QB != 0)
                                        {
                                            for (int a = 0; a < QB; a++)
                                            {
                                                DataRow newRow = dt.NewRow();
                                                newRow["ScanCode"] = dt.Rows[rowIndex]["ScanCode"];
                                                newRow["Description"] = dt.Rows[rowIndex]["Description"];
                                                if (refund == "")
                                                    newRow["Quantity"] = 1;
                                                else
                                                    newRow["Quantity"] = -1;
                                                newRow["UnitRetail"] = dt.Rows[rowIndex]["OPrice"];
                                                newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                                                newRow["OPrice"] = dt.Rows[rowIndex]["OPrice"];
                                                newRow["TaxRate"] = dt.Rows[rowIndex]["TaxRate"];
                                                newRow["PROName"] = dt.Rows[rowIndex]["PROName"];
                                                newRow["Qty"] = dt.Rows[rowIndex]["Qty"];
                                                newRow["NewPrice"] = dt.Rows[rowIndex]["NewPrice"];
                                                newRow["Discount"] = dt.Rows[rowIndex]["Discount"];
                                                newRow["Type"] = dt.Rows[rowIndex]["Type"];
                                                dt.Rows.Add(newRow);
                                            }
                                        }
                                    }

                                    int intv = qDT1 * (qDT / qDT1);
                                    decimal ab = qDT / qDT1;
                                    decimal decv = Convert.ToDecimal(qDT1) * Convert.ToDecimal(qDT) / Convert.ToDecimal(qDT1);

                                    PromotionApply();
                                }
                                else if (dt.Rows[rowIndex]["RPROName"].ToString() != "")
                                {
                                    int qDT = Convert.ToInt32(dt.Rows[rowIndex]["Quantity"]);
                                    int qDT1 = Convert.ToInt32(dt.Rows[rowIndex]["RQty"]);

                                    if (qDT >= qDT1)
                                    {
                                        int QA = 0;
                                        if (dt.Rows[rowIndex]["RType"].ToString() == "Once")
                                        {
                                            QA = qDT - qDT1;
                                        }
                                        else
                                        {
                                            QA = qDT1 * (qDT / qDT1);
                                        }
                                        string price = "";
                                        if (dt.Rows[rowIndex]["RNewPrice"].ToString() != "")
                                        {
                                            price = (Convert.ToDecimal(dt.Rows[rowIndex]["RNewPrice"]) / qDT1).ToString("0.00");
                                        }
                                        else
                                        {
                                            decimal odisc = 0;
                                            decimal ldisc = 0;
                                            decimal sdisc = 0;
                                            if (dt.Rows[rowIndex]["PromotionName"].ToString() != "")
                                                sdisc = Convert.ToDecimal(dt.Rows[rowIndex]["Discount"]);
                                            if (dt.Rows[rowIndex]["LPromotionName"].ToString() != "")
                                                ldisc = Convert.ToDecimal(dt.Rows[rowIndex]["LDiscount"]);
                                            if (dt.Rows[rowIndex]["OPromotionName"].ToString() != "")
                                                odisc = Convert.ToDecimal(dt.Rows[rowIndex]["ODiscount"]);
                                            price = (Convert.ToDecimal(dt.Rows[rowIndex]["Oprice"]) - sdisc - odisc - ldisc - Convert.ToDecimal(dt.Rows[rowIndex]["RDiscount"]) / qDT1).ToString("0.00");
                                        }
                                        dt.Rows[rowIndex]["UnitRetail"] = price;
                                        dt.Rows[rowIndex]["RPromotionName"] = dt.Rows[rowIndex]["RPROName"];
                                        dt.Rows[rowIndex]["Quantity"] = QA;
                                        dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                                        int QB = qDT - QA;
                                        if (QB != 0)
                                        {
                                            for (int a = 0; a < QB; a++)
                                            {
                                                DataRow newRow = dt.NewRow();
                                                newRow["ScanCode"] = dt.Rows[rowIndex]["ScanCode"];
                                                newRow["Description"] = dt.Rows[rowIndex]["Description"];
                                                if (refund == "")
                                                    newRow["Quantity"] = 1;
                                                else
                                                    newRow["Quantity"] = -1;
                                                newRow["UnitRetail"] = dt.Rows[rowIndex]["OPrice"];
                                                newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                                                newRow["OPrice"] = dt.Rows[rowIndex]["OPrice"];
                                                newRow["TaxRate"] = dt.Rows[rowIndex]["TaxRate"];
                                                newRow["RPROName"] = dt.Rows[rowIndex]["RPROName"];
                                                newRow["RQty"] = dt.Rows[rowIndex]["RQty"];
                                                newRow["RNewPrice"] = dt.Rows[rowIndex]["RNewPrice"];
                                                newRow["RDiscount"] = dt.Rows[rowIndex]["RDiscount"];
                                                newRow["RType"] = dt.Rows[rowIndex]["RType"];
                                                dt.Rows.Add(newRow);
                                            }
                                        }
                                    }

                                    int intv = qDT1 * (qDT / qDT1);
                                    decimal ab = qDT / qDT1;
                                    decimal decv = Convert.ToDecimal(qDT1) * Convert.ToDecimal(qDT) / Convert.ToDecimal(qDT1);

                                    PromotionApply();
                                }
                                else if (dt.Rows[rowIndex]["LPROName"].ToString() != "")
                                {
                                    int qDT = Convert.ToInt32(dt.Rows[rowIndex]["Quantity"]);
                                    int qDT1 = Convert.ToInt32(dt.Rows[rowIndex]["LQty"]);

                                    if (qDT >= qDT1)
                                    {
                                        int QA = 0;
                                        if (dt.Rows[rowIndex]["LType"].ToString() == "Once")
                                        {
                                            QA = qDT;
                                            string price = "";
                                            if (dt.Rows[rowIndex]["LNewPrice"].ToString() != "" && dt.Rows[rowIndex]["LNewPrice"].ToString() != "0")
                                            {
                                                price = Convert.ToDecimal(dt.Rows[rowIndex]["LNewPrice"]).ToString("0.00");
                                            }
                                            else
                                            {
                                                decimal odisc = 0;
                                                decimal rdisc = 0;
                                                decimal sdisc = 0;
                                                if (dt.Rows[rowIndex]["PromotionName"].ToString() != "")
                                                    sdisc = Convert.ToDecimal(dt.Rows[rowIndex]["Discount"]);
                                                if (dt.Rows[rowIndex]["RPromotionName"].ToString() != "")
                                                    rdisc = Convert.ToDecimal(dt.Rows[rowIndex]["RDiscount"]);
                                                if (dt.Rows[rowIndex]["OPromotionName"].ToString() != "")
                                                    odisc = Convert.ToDecimal(dt.Rows[rowIndex]["ODiscount"]);
                                                price = (Convert.ToDecimal(dt.Rows[rowIndex]["Oprice"]) - sdisc - odisc - rdisc - Convert.ToDecimal(dt.Rows[rowIndex]["LDiscount"]) / qDT).ToString("0.00");
                                            }
                                            dt.Rows[rowIndex]["UnitRetail"] = price;
                                            dt.Rows[rowIndex]["LPromotionName"] = dt.Rows[rowIndex]["LPROName"];
                                            dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");

                                        }
                                        else
                                        {
                                            QA = qDT1 * (qDT / qDT1);
                                            string price = "";
                                            if (dt.Rows[rowIndex]["LNewPrice"].ToString() != "" && dt.Rows[rowIndex]["LNewPrice"].ToString() != "0")
                                            {
                                                price = (Convert.ToDecimal(dt.Rows[rowIndex]["LNewPrice"])).ToString("0.00");
                                            }
                                            else
                                            {
                                                decimal odisc = 0;
                                                decimal rdisc = 0;
                                                decimal sdisc = 0;
                                                if (dt.Rows[rowIndex]["PromotionName"].ToString() != "")
                                                    sdisc = Convert.ToDecimal(dt.Rows[rowIndex]["Discount"]);
                                                if (dt.Rows[rowIndex]["RPromotionName"].ToString() != "")
                                                    rdisc = Convert.ToDecimal(dt.Rows[rowIndex]["RDiscount"]);
                                                if (dt.Rows[rowIndex]["OPromotionName"].ToString() != "")
                                                    odisc = Convert.ToDecimal(dt.Rows[rowIndex]["ODiscount"]);
                                                price = (Convert.ToDecimal(dt.Rows[rowIndex]["Oprice"]) - sdisc - odisc - rdisc - Convert.ToDecimal(dt.Rows[rowIndex]["LDiscount"]) / qDT1).ToString("0.00");
                                            }
                                            dt.Rows[rowIndex]["UnitRetail"] = price;
                                            dt.Rows[rowIndex]["LPromotionName"] = dt.Rows[rowIndex]["LPROName"];
                                            dt.Rows[rowIndex]["Quantity"] = QA;
                                            dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                                        }

                                        int QB = qDT - QA;
                                        if (QB != 0)
                                        {
                                            for (int a = 0; a < QB; a++)
                                            {
                                                DataRow newRow = dt.NewRow();
                                                newRow["ScanCode"] = dt.Rows[rowIndex]["ScanCode"];
                                                newRow["Description"] = dt.Rows[rowIndex]["Description"];
                                                if (refund == "")
                                                    newRow["Quantity"] = 1;
                                                else
                                                    newRow["Quantity"] = -1;
                                                newRow["UnitRetail"] = dt.Rows[rowIndex]["OPrice"];
                                                newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                                                newRow["OPrice"] = dt.Rows[rowIndex]["OPrice"];
                                                newRow["TaxRate"] = dt.Rows[rowIndex]["TaxRate"];
                                                newRow["LPROName"] = dt.Rows[rowIndex]["LPROName"];
                                                newRow["LQty"] = dt.Rows[rowIndex]["LQty"];
                                                newRow["LNewPrice"] = dt.Rows[rowIndex]["LNewPrice"];
                                                newRow["LDiscount"] = dt.Rows[rowIndex]["LDiscount"];
                                                newRow["LType"] = dt.Rows[rowIndex]["LType"];
                                                dt.Rows.Add(newRow);
                                            }
                                        }
                                    }

                                    int intv = qDT1 * (qDT / qDT1);
                                    decimal ab = qDT / qDT1;
                                    decimal decv = Convert.ToDecimal(qDT1) * Convert.ToDecimal(qDT) / Convert.ToDecimal(qDT1);

                                    PromotionApply();
                                }
                                else if (dt.Rows[rowIndex]["OPROName"].ToString() != "")
                                {
                                    int qDT = Convert.ToInt32(dt.Rows[rowIndex]["Quantity"]);
                                    int qDT1 = Convert.ToInt32(dt.Rows[rowIndex]["OQty"]);

                                    if (qDT >= qDT1)
                                    {

                                        int QA = 0;
                                        if (dt.Rows[rowIndex]["OType"].ToString() == "Once")
                                        {
                                            QA = qDT - qDT1;
                                        }
                                        else
                                        {
                                            QA = qDT1 * (qDT / qDT1);
                                        }
                                        string price = "";
                                        if (dt.Rows[rowIndex]["ONewPrice"].ToString() != "")
                                        {
                                            price = (Convert.ToDecimal(dt.Rows[rowIndex]["ONewPrice"]) / qDT1).ToString("0.00");
                                        }
                                        else
                                        {
                                            decimal odisc = 0;
                                            decimal rdisc = 0;
                                            decimal sdisc = 0;
                                            if (dt.Rows[rowIndex]["PromotionName"].ToString() != "")
                                                sdisc = Convert.ToDecimal(dt.Rows[rowIndex]["Discount"]);
                                            if (dt.Rows[rowIndex]["RPromotionName"].ToString() != "")
                                                rdisc = Convert.ToDecimal(dt.Rows[rowIndex]["RDiscount"]);
                                            if (dt.Rows[rowIndex]["LPromotionName"].ToString() != "")
                                                odisc = Convert.ToDecimal(dt.Rows[rowIndex]["LDiscount"]);
                                            price = (Convert.ToDecimal(dt.Rows[rowIndex]["Oprice"]) - sdisc - odisc - rdisc - Convert.ToDecimal(dt.Rows[rowIndex]["ODiscount"]) / qDT1).ToString("0.00");
                                        }
                                        dt.Rows[rowIndex]["UnitRetail"] = price;
                                        dt.Rows[rowIndex]["OPromotionName"] = dt.Rows[rowIndex]["OPROName"];
                                        dt.Rows[rowIndex]["Quantity"] = QA;
                                        dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                                        int QB = qDT - QA;
                                        if (QB != 0)
                                        {
                                            for (int a = 0; a < QB; a++)
                                            {
                                                DataRow newRow = dt.NewRow();
                                                newRow["ScanCode"] = dt.Rows[rowIndex]["ScanCode"];
                                                newRow["Description"] = dt.Rows[rowIndex]["Description"];
                                                newRow["Quantity"] = 1;
                                                newRow["UnitRetail"] = dt.Rows[rowIndex]["OPrice"];
                                                newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                                                newRow["OPrice"] = dt.Rows[rowIndex]["OPrice"];
                                                newRow["TaxRate"] = dt.Rows[rowIndex]["TaxRate"];
                                                newRow["OPROName"] = dt.Rows[rowIndex]["OPROName"];
                                                newRow["OQty"] = dt.Rows[rowIndex]["OQty"];
                                                newRow["ONewPrice"] = dt.Rows[rowIndex]["ONewPrice"];
                                                newRow["ODiscount"] = dt.Rows[rowIndex]["ODiscount"];
                                                newRow["OType"] = dt.Rows[rowIndex]["OType"];
                                                dt.Rows.Add(newRow);
                                            }
                                        }
                                    }

                                    int intv = qDT1 * (qDT / qDT1);
                                    decimal ab = qDT / qDT1;
                                    decimal decv = Convert.ToDecimal(qDT1) * Convert.ToDecimal(qDT) / Convert.ToDecimal(qDT1);

                                    PromotionApply();
                                }
                                else
                                {
                                    dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                                }
                                JRDGrid.ItemsSource = dt.DefaultView;
                                TotalEvent();
                            }
                        }
                    }
                }
                else
                {
                    dt.Rows[rowIn]["Quantity"] = dt.Rows[rowIn]["Quantity"];
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "JRDGrid_CellEditEnding");
            }
        }
        private void NumButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string number = (sender as Button).Content.ToString();

                if (txtGotFocusStr == "textBox1")
                {
                    grPayment.Visibility = Visibility.Hidden;
                    ugDepartment.Visibility = Visibility.Visible;
                    TxtCashReceive.Text = "";
                    TxtCashReturn.Text = "";
                    string textBox1Str = textBox1.Text;
                    textBox1.Text = textBox1Str + number;
                }
                if (txtGotFocusStr == "TxtCashReceive")
                {
                    string textBox1Str = TxtCashReceive.Text;
                    if (textBox1Str != "")
                    {
                        textBox1Str = (Convert.ToDecimal(textBox1Str) * 100).ToString("0.00");
                        textBox1Str = textBox1Str.Remove(textBox1Str.Length - 3);
                    }
                    TxtCashReceive.Text = (Convert.ToDecimal(textBox1Str + number) / 100).ToString("0.00");
                }
                if (txtGotFocusStr == "TxtCheck")
                {
                    string textBox1Str = TxtCheck.Text;
                    TxtCheck.Text = textBox1Str + number;
                }
                if (txtGotFocusStr == "txtBarcode")
                {
                    string textBox1Str = txtBarcode.Text;
                    txtBarcode.Text = textBox1Str + number;
                }
                if (txtGotFocusStr == "txtDeptAmt")
                {
                    string textBox1Str = txtDeptAmt.Text;
                    if (textBox1Str != "")
                    {
                        textBox1Str = (Convert.ToDecimal(textBox1Str) * 100).ToString("0.00");
                        textBox1Str = textBox1Str.Remove(textBox1Str.Length - 3);
                    }
                    txtDeptAmt.Text = (Convert.ToDecimal(textBox1Str + number) / 100).ToString("0.00");

                }
                if (txtGotFocusStr == "CellEditQty")
                {
                    int i = dt.Rows.Count - 1;
                    DataRow dataRow = dt.Rows[i];
                    string qty = dt.Rows[i]["Quantity"].ToString();
                    dt.Rows[i]["Quantity"] = qty + number;
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "NumButton_Click");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                Report rpt = new Report();
                rpt.Show();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_3");
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtGotFocusStr == "textBox1")
                {
                    textBox1.Text = "";
                }
                if (txtGotFocusStr == "TxtCashReceive")
                {
                    TxtCashReceive.Text = "";
                }
                if (txtGotFocusStr == "TxtCheck")
                {
                    TxtCheck.Text = "";
                }
                if (txtGotFocusStr == "txtDeptAmt")
                {
                    txtDeptAmt.Text = "";
                }
                if (txtGotFocusStr == "txtBarcode")
                {
                    txtBarcode.Text = "";
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_4");
            }

        }

        //Shift close
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                if (dtHold.Rows.Count == 0)
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

                    string tenderQ = "Update tender set shiftClose=@username Where StoreId = " + storeid + " and PosId = " + posId + " and   shiftClose is null";
                    SqlCommand tenderCMD = new SqlCommand(tenderQ, con);
                    tenderCMD.Parameters.AddWithValue("@username", i);
                    string transQ = "Update Transactions set shiftClose=@username Where StoreId = " + storeid + " and PosId = " + posId + " and  shiftClose is null";
                    SqlCommand transCMD = new SqlCommand(transQ, con);
                    transCMD.Parameters.AddWithValue("@username", i);
                    string itemQ = "Update SalesItem set shiftClose=@username Where StoreId = " + storeid + " and PosId = " + posId + " and  shiftClose is null";
                    SqlCommand itemCMD = new SqlCommand(itemQ, con);
                    itemCMD.Parameters.AddWithValue("@username", i);
                    //string expQ = "Update Expence set shiftClose=@username Where StoreId = " + storeid + " and PosId = " + posId + " and  shiftClose is null";
                    //SqlCommand expCMD = new SqlCommand(expQ, con);
                    //expCMD.Parameters.AddWithValue("@username", i);
                    //string RECQ = "Update Receive set shiftClose=@username Where StoreId = " + storeid + " and PosId = " + posId + " and  shiftClose is null";
                    //SqlCommand RECCMD = new SqlCommand(RECQ, con);
                    //RECCMD.Parameters.AddWithValue("@username", i);
                    con.Open();
                    tenderCMD.ExecuteNonQuery();
                    transCMD.ExecuteNonQuery();
                    itemCMD.ExecuteNonQuery();
                    //expCMD.ExecuteNonQuery();
                    //RECCMD.ExecuteNonQuery();
                    con.Close();
                }
                else { MessageBox.Show("Please Clear Hold Transaction"); }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_5");
            }
        }

        private void ShiftClose(object sender, PrintPageEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryTrans = "select Count(tran_id)as Counts,sum(Convert(decimal(10,2),GrossAmount))as Sales,sum(Convert(decimal(10,2),TaxAmount))as Tax,sum(Convert(decimal(10,2),grandAmount))as Total,min(convert(datetime,createon))as SDate,Max(convert(datetime,createon))as EDate from transactions where StoreId = " + storeid + " and POSId = " + posId + " and ShiftClose is null and (void !=1 or void is Null)";
                SqlCommand cmdTrans = new SqlCommand(queryTrans, con);
                SqlDataAdapter sdaTrans = new SqlDataAdapter(cmdTrans);
                DataTable dtTrans = new DataTable();
                sdaTrans.Fill(dtTrans);

                string queryDept = "select Department,Sum(Convert(decimal(10,2),amt)) as amt from(select Department, Sum(Convert(decimal(10,2),Amount)) as amt from salesitem inner join item on salesitem.scancode = item.scancode and salesitem.storeid = item.storeid where  salesitem.storeid = " + storeid + " and salesitem.POSId = " + posId + " and ShiftClose is null and(void != 1 or void is Null) group by Department Union all select Department,Sum(Convert(decimal(10,2),Amount)) as amt from salesitem inner join Department on salesitem.Descripation = Department.Department and salesitem.storeid = Department.storeid where  salesitem.storeid = " + storeid + " and salesitem.POSId = " + posId + " and ShiftClose is null and(void != 1 or void is Null) group by Department)as x group by Department";
                SqlCommand cmdDept = new SqlCommand(queryDept, con);
                SqlDataAdapter sdaDept = new SqlDataAdapter(cmdDept);
                DataTable dtDept = new DataTable();
                sdaDept.Fill(dtDept);

                string queryTender = "select tendercode,sum(Convert(decimal(10,2),amount)-coalesce(Convert(decimal(10,2),change),0))as amt from tender where  storeid = " + storeid + " and posid =" + posId + " and  ShiftClose is null group by tendercode";
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
                graphics.DrawString("    " + dtstr.Rows[0]["StoreName"].ToString(), headerfont,
                new SolidBrush(Color.Black), 22 + 22, 22);
                Offset = Offset + largeinc + 22;

                DrawAtStart("            " + dtstr.Rows[0]["Address"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("            " + dtstr.Rows[0]["PhoneNumber"].ToString(), Offset);

                Offset = Offset + mediuminc;
                String underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 0);

                Offset = Offset + mediuminc + 10;
                DrawAtStart("          Register :" + registerid, Offset);
                Offset = Offset + mediuminc;
                Offset = Offset + mediuminc;
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
                SendErrorToText(ex, errorFileName, "ShiftClose");
            }
        }

        //page close
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_6");
            }
        }

        private void textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                if (tb != null)
                {
                    txtGotFocusStr = tb.Name;
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "textbox_GotFocus");
            }
        }

        private void Department_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GoBack.Visibility = Visibility.Hidden;
                grPayment.Visibility = Visibility.Hidden;
                btnShortKey.Visibility = Visibility.Visible;
                btnDept.Visibility = Visibility.Hidden;
                ugDepartment.Visibility = Visibility.Visible;
                ugDepartment1.Visibility = Visibility.Hidden;
                ugAddcategory1.Visibility = Visibility.Hidden;
                ugAddcategory2.Visibility = Visibility.Hidden;
                ugCategory2.Visibility = Visibility.Hidden;
                ugCategory1.Visibility = Visibility.Hidden;
                gReceipt.Visibility = Visibility.Hidden;

                if (dtdepartment.Rows.Count > 19)
                {
                    LeftArrow.IsEnabled = false;
                    LeftArrow.Visibility = Visibility.Hidden;
                    RightArrow.Visibility = Visibility.Visible;
                    RightArrow.IsEnabled = true;
                }
                else
                {
                    LeftArrow.IsEnabled = false;
                    LeftArrow.Visibility = Visibility.Hidden;
                    RightArrow.Visibility = Visibility.Hidden;
                    RightArrow.IsEnabled = false;
                }
                categorytext = "";
                TxtCashReturn.Text = "";
                TxtCashReceive.Text = "";
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Department_Button_Click");
            }
        }

        private void ShortcutKey_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GoBack.Visibility = Visibility.Hidden;
                btnShortKey.Visibility = Visibility.Hidden;
                grPayment.Visibility = Visibility.Hidden;
                btnDept.Visibility = Visibility.Visible;
                ugDepartment.Visibility = Visibility.Hidden;
                ugDepartment1.Visibility = Visibility.Hidden;
                ugCategory1.Visibility = Visibility.Hidden;
                TxtBxStackPanel2.Visibility = Visibility.Hidden;
                ugAddcategory2.Visibility = Visibility.Hidden;
                gReceipt.Visibility = Visibility.Hidden;
                ugAddcategory1.Visibility = Visibility.Visible;
                if (dtAddCategory.Rows.Count > 23)
                {
                    LeftArrow.Visibility = Visibility.Hidden;
                    RightArrow.Visibility = Visibility.Visible;
                    LeftArrow.IsEnabled = false;
                    RightArrow.IsEnabled = true;
                }
                else
                {
                    LeftArrow.Visibility = Visibility.Hidden;
                    RightArrow.Visibility = Visibility.Hidden;
                    LeftArrow.IsEnabled = false;
                    RightArrow.IsEnabled = false;
                }
                TxtCashReturn.Text = "";
                TxtCashReceive.Text = "";
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "ShortcutKey_Button_Click");
            }
        }

        private void JdGrid_delete_click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                // Remove Record.
                DataGrid newGrid = new DataGrid();
                DataRowView row = (DataRowView)JRDGrid.SelectedItem;
                string str = row["PromotionName"].ToString();
                int removeRowQut = Convert.ToInt32(row["Quantity"]);
                dt.Rows.Remove(row.Row);
                dt.AcceptChanges();
                newGrid.ItemsSource = dt.DefaultView;
                dt = ((DataView)newGrid.ItemsSource).ToTable();
                if (str != "")
                {
                    if (dt.AsEnumerable().Count() != 0)
                    {
                        dt = dt.DefaultView.ToTable();
                        for (int i = 0; i < dt.AsEnumerable().Count(); i++)
                        {
                            if (Convert.ToInt32(dt.Rows[i]["Quantity"]) == 1)
                            {
                                dt.Rows[i]["PromotionName"] = "";
                                dt.Rows[i]["UnitRetail"] = dt.Rows[i]["OPrice"];
                                dt.Rows[i]["Amount"] = Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"]);
                            }
                        }
                        PromotionApply();
                    }
                }
                JRDGrid.ItemsSource = dt.DefaultView;
                TotalEvent();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "JdGrid_delete_click");
            }
        }

        private void TxtCheck_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Tab || e.Key == Key.Enter)
                {
                    Button_Click_1();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "TxtCheck_KeyDown");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                btnConform.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "ComboBox_SelectionChanged");
            }
        }

        private void btnConform_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button_Click_1();
                btnConform.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "btnConform_Click");
            }
        }

        //public void ScanCodeFunction()
        //{
        //    try
        //    {
        //        if (dt.AsEnumerable().Count() > 0)
        //        {
        //            if (loyaltyCustomerCount <= 5)
        //            {
        //                foreach (var item in dt.AsEnumerable())
        //                {
        //                    DataTable dataTableDistr = new DataTable();
        //                    dataTableDistr.Columns.Add("PromotionId");
        //                    dataTableDistr.Columns.Add("PromotionName");
        //                    dataTableDistr.Columns.Add("newprice");
        //                    dataTableDistr.Columns.Add("Quantity");
        //                    dataTableDistr.Columns.Add("Discount");
        //                    dataTableDistr.Columns.Add("Type");

        //                    var PromotionIdSpl = item["PromotionId"].ToString().Split(',').ToList();

        //                    foreach (string itemSpl in PromotionIdSpl)
        //                    {
        //                        foreach (var drObj in dtPromotion.AsEnumerable().Where(z => z["PromotionId"].ToString() == itemSpl).AsEnumerable())
        //                        {
        //                            DataRow dr = drObj;
        //                            dataTableDistr.Rows.Add(dr.ItemArray);
        //                        }
        //                    }

        //                    foreach (var itemPromo in dataTableDistr.AsEnumerable())
        //                    {
        //                        int sumCount = dt.AsEnumerable()
        //                            .Where(x => x["PromotionId"].ToString().Split(',').Contains(itemPromo["PromotionId"].ToString())).ToList().Sum(s => Convert.ToInt32(s.Field<string>("Quantity")));
        //                        if (sumCount < 0)
        //                            sumCount = sumCount * -1;

        //                        foreach (var promotionspl in PromotionIdSpl.AsEnumerable())
        //                        {
        //                            if (promotionspl == itemPromo["PromotionId"].ToString())
        //                            {
        //                                if (itemPromo["Type"].ToString() == "Multy")
        //                                {
        //                                    int _qty = Convert.ToInt32(itemPromo["Quantity"]);

        //                                    if (sumCount > _qty)
        //                                    {
        //                                        if ((sumCount % _qty) == 0)
        //                                        {
        //                                            if (sumCount != _qty)
        //                                                _qty = sumCount;
        //                                        }
        //                                    }
        //                                    if (sumCount == _qty)
        //                                    {
        //                                        if (item["bIsTrueId"].ToString().Split(',').ToList().Where(s => s.Contains(itemPromo["PromotionId"].ToString())).Count() == 0)
        //                                        {
        //                                            string price = "";
        //                                            if (itemPromo["NewPrice"].ToString() != "" && itemPromo["NewPrice"].ToString() != "0")
        //                                                price = (Convert.ToDecimal(itemPromo["NewPrice"]) / Convert.ToInt32(itemPromo["Quantity"])).ToString("0.00");
        //                                            if (price == "")
        //                                            {
        //                                                price = (Convert.ToDecimal(item["UnitRetail"]) - Convert.ToDecimal(itemPromo["Discount"])).ToString("0.00");
        //                                            }
        //                                            item["UnitRetail"] = price;
        //                                            item["Amount"] = Convert.ToDecimal(item["UnitRetail"]) * Convert.ToDecimal(item["Quantity"]);

        //                                            if (item["PromotionName"].ToString() == "")
        //                                            {
        //                                                item["PromotionName"] = itemPromo["PromotionName"].ToString();
        //                                                item["bIsTrueId"] = itemPromo["PromotionId"].ToString();
        //                                            }
        //                                            else
        //                                            {
        //                                                item["PromotionName"] = item["PromotionName"].ToString() + " , " + itemPromo["PromotionName"].ToString();
        //                                                item["bIsTrueId"] = item["bIsTrueId"].ToString() + " , " + itemPromo["PromotionId"].ToString();
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (sumCount == Convert.ToInt32(itemPromo["Quantity"]))
        //                                    {
        //                                        string price = "";
        //                                        if (itemPromo["NewPrice"].ToString() != "" && itemPromo["NewPrice"].ToString() != "0")
        //                                            price = (Convert.ToDecimal(itemPromo["NewPrice"]) / Convert.ToInt32(itemPromo["Quantity"])).ToString("0.00");
        //                                        if (price == "")
        //                                        {
        //                                            price = (Convert.ToDecimal(item["UnitRetail"]) - Convert.ToDecimal(itemPromo["Discount"])).ToString("0.00");
        //                                        }
        //                                        item["UnitRetail"] = price;
        //                                        item["Amount"] = Convert.ToDecimal(item["UnitRetail"]) * Convert.ToDecimal(item["Quantity"]);
        //                                        if (item["PromotionName"].ToString() == "")
        //                                            item["PromotionName"] = itemPromo["PromotionName"].ToString();
        //                                        else
        //                                            item["PromotionName"] = item["PromotionName"].ToString() + " , " + itemPromo["PromotionName"].ToString();
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            JRDGrid.ItemsSource = dt.DefaultView;
        //            JRDGrid.Items.Refresh();
        //            TotalEvent();
        //        }


        //        //DataTable distrinctPromotionName = dt.DefaultView.ToTable(true, "PROName");
        //        //DataTable distrinctSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "PROName", "Qty", "NewPrice", "Discount", "Type");
        //        //DataTable distrinctRPromotionName = dt.DefaultView.ToTable(true, "RPROName");
        //        //DataTable distrinctRSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "RPROName", "RQty", "RNewPrice", "RDiscount", "RType");
        //        //DataTable distrinctLPromotionName = dt.DefaultView.ToTable(true, "LPROName");
        //        //DataTable distrinctLSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "LPROName", "LQty", "LNewPrice", "LDiscount", "LType");
        //        //DataTable distrinctOPromotionName = dt.DefaultView.ToTable(true, "OPROName");
        //        //DataTable distrinctOSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "OPROName", "OQty", "ONewPrice", "ODiscount", "OType");

        //        //foreach (DataRow distrinctRow in distrinctPromotionName.AsEnumerable())
        //        //{
        //        //    if (distrinctRow["PROName"].ToString() != "")
        //        //    {
        //        //        int sumCount = 0;
        //        //        for (int j = 0; j < distrinctSCANCODE.AsEnumerable().Count(); j++)
        //        //        {
        //        //            if (distrinctSCANCODE.Rows[j]["PROName"].ToString() == distrinctRow["PROName"].ToString())
        //        //            {
        //        //                for (int i = 0; i < dt.Rows.Count; i++)
        //        //                {
        //        //                    if (distrinctSCANCODE.Rows[j]["PROName"].ToString() == dt.Rows[i]["PROName"].ToString())
        //        //                    {
        //        //                        //if (distrinctOSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
        //        //                        //{
        //        //                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
        //        //                        //for (int K = 0; K < dt.Rows.Count; K++)
        //        //                        //{
        //        //                        if (sumCount < 0)
        //        //                            sumCount = sumCount * -1;
        //        //                        foreach (DataRow itemDT1 in distrinctSCANCODE.AsEnumerable())
        //        //                        {
        //        //                            if (itemDT1["PROName"].ToString() == distrinctRow["PROName"].ToString())
        //        //                            {
        //        //                                int Y = sumCount / Convert.ToInt32(itemDT1["Qty"]);
        //        //                                for (int x = 1; x <= Y; x++)
        //        //                                {
        //        //                                    if (itemDT1["Type"].ToString() == "Once")
        //        //                                    {
        //        //                                        if (sumCount == Convert.ToInt32(itemDT1["Qty"]))
        //        //                                        {
        //        //                                            for (int z = 0; z <= i; z++)
        //        //                                            {
        //        //                                                if (dt.Rows[z]["PROName"].ToString() == distrinctRow["PROName"].ToString())
        //        //                                                {
        //        //                                                    string price = "";
        //        //                                                    if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
        //        //                                                        price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString("0.00");
        //        //                                                    if (price == "")
        //        //                                                    {
        //        //                                                        decimal ldisc = 0;
        //        //                                                        decimal odisc = 0;
        //        //                                                        decimal rdisc = 0;
        //        //                                                        if (dt.Rows[z]["LPromotionName"].ToString() != "")
        //        //                                                            ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
        //        //                                                        if (dt.Rows[z]["OPromotionName"].ToString() != "")
        //        //                                                            odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
        //        //                                                        if (dt.Rows[z]["RPromotionName"].ToString() != "")
        //        //                                                            rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
        //        //                                                        price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - odisc - rdisc - Convert.ToDecimal(dt.Rows[z]["Discount"])).ToString("0.00");
        //        //                                                    }
        //        //                                                    dt.Rows[z]["SPromotionName"] = dt.Rows[z]["PROName"];
        //        //                                                    dt.Rows[z]["UnitRetail"] = price;
        //        //                                                    dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
        //        //                                                    dt.Rows[z]["PromotionName"] = "";
        //        //                                                    dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
        //        //                                                }
        //        //                                            }
        //        //                                        }
        //        //                                    }
        //        //                                    else
        //        //                                    {
        //        //                                        if (sumCount == Convert.ToInt32(itemDT1["Qty"]) * x)
        //        //                                        {
        //        //                                            for (int z = 0; z <= i; z++)
        //        //                                            {
        //        //                                                if (dt.Rows[z]["PROName"].ToString() == distrinctRow["PROName"].ToString())
        //        //                                                {
        //        //                                                    string price = "";
        //        //                                                    if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
        //        //                                                        price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString("0.00");

        //        //                                                    if (price == "")
        //        //                                                    {
        //        //                                                        decimal ldisc = 0;
        //        //                                                        decimal odisc = 0;
        //        //                                                        decimal rdisc = 0;
        //        //                                                        if (dt.Rows[z]["LPromotionName"].ToString() != "")
        //        //                                                            ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
        //        //                                                        if (dt.Rows[z]["OPromotionName"].ToString() != "")
        //        //                                                            odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
        //        //                                                        if (dt.Rows[z]["RPromotionName"].ToString() != "")
        //        //                                                            rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
        //        //                                                        price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - odisc - rdisc - Convert.ToDecimal(dt.Rows[z]["Discount"])).ToString("0.00");
        //        //                                                    }
        //        //                                                    dt.Rows[z]["SPromotionName"] = dt.Rows[z]["PROName"];
        //        //                                                    dt.Rows[z]["UnitRetail"] = price;
        //        //                                                    dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
        //        //                                                    dt.Rows[z]["PromotionName"] = "";
        //        //                                                    dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
        //        //                                                }
        //        //                                            }
        //        //                                        }
        //        //                                    }
        //        //                                }
        //        //                            }
        //        //                        }
        //        //                        //    }
        //        //                        //}
        //        //                    }
        //        //                }

        //        //            }
        //        //        }
        //        //    }
        //        //}
        //        //foreach (DataRow distrinctRow in distrinctRPromotionName.AsEnumerable())
        //        //{
        //        //    if (distrinctRow["RPROName"].ToString() != "")
        //        //    {
        //        //        int sumCount = 0;
        //        //        for (int j = 0; j < distrinctRSCANCODE.AsEnumerable().Count(); j++)
        //        //        {
        //        //            if (distrinctRSCANCODE.Rows[j]["RPROName"].ToString() == distrinctRow["RPROName"].ToString())
        //        //            {
        //        //                for (int i = 0; i < dt.Rows.Count; i++)
        //        //                {
        //        //                    if (distrinctRSCANCODE.Rows[j]["RPROName"].ToString() == dt.Rows[i]["RPROName"].ToString())
        //        //                    {
        //        //                        //if (distrinctOSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
        //        //                        //{
        //        //                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
        //        //                        //for (int K = 0; K < dt.Rows.Count; K++)
        //        //                        //{
        //        //                        if (sumCount < 0)
        //        //                            sumCount = sumCount * -1;
        //        //                        foreach (DataRow itemDT1 in distrinctRSCANCODE.AsEnumerable())
        //        //                        {
        //        //                            if (itemDT1["RPROName"].ToString() == distrinctRow["RPROName"].ToString())
        //        //                            {
        //        //                                int Y = sumCount / Convert.ToInt32(itemDT1["RQty"]);
        //        //                                for (int x = 1; x <= Y; x++)
        //        //                                {
        //        //                                    if (itemDT1["RType"].ToString() == "Once")
        //        //                                    {
        //        //                                        if (sumCount == Convert.ToInt32(itemDT1["RQty"]))
        //        //                                        {
        //        //                                            for (int z = 0; z <= i; z++)
        //        //                                            {
        //        //                                                if (dt.Rows[z]["RPROName"].ToString() == distrinctRow["RPROName"].ToString())
        //        //                                                {
        //        //                                                    string price = "";
        //        //                                                    if (itemDT1["RNewPrice"].ToString() != "" && itemDT1["RNewPrice"].ToString() != "0")
        //        //                                                        price = (Convert.ToDecimal(itemDT1["RNewPrice"]) / Convert.ToInt32(itemDT1["RQty"])).ToString("0.00");

        //        //                                                    if (price == "")
        //        //                                                    {
        //        //                                                        decimal ldisc = 0;
        //        //                                                        decimal odisc = 0;
        //        //                                                        decimal sdisc = 0;
        //        //                                                        if (dt.Rows[z]["LPromotionName"].ToString() != "")
        //        //                                                            ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
        //        //                                                        if (dt.Rows[z]["OPromotionName"].ToString() != "")
        //        //                                                            odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
        //        //                                                        if (dt.Rows[z]["SPromotionName"].ToString() != "")
        //        //                                                            sdisc = Convert.ToDecimal(dt.Rows[z]["Discount"]);
        //        //                                                        price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - odisc - sdisc - Convert.ToDecimal(dt.Rows[z]["RDiscount"])).ToString("0.00");
        //        //                                                    }
        //        //                                                    dt.Rows[z]["RPromotionName"] = dt.Rows[z]["RPROName"];
        //        //                                                    dt.Rows[z]["UnitRetail"] = price;
        //        //                                                    dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);


        //        //                                                    dt.Rows[z]["PromotionName"] = "";
        //        //                                                    dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
        //        //                                                }
        //        //                                            }

        //        //                                        }
        //        //                                    }
        //        //                                    else
        //        //                                    {
        //        //                                        if (sumCount == Convert.ToInt32(itemDT1["RQty"]) * x)
        //        //                                        {
        //        //                                            for (int z = 0; z <= i; z++)
        //        //                                            {
        //        //                                                if (dt.Rows[z]["RPROName"].ToString() == distrinctRow["RPROName"].ToString())
        //        //                                                {
        //        //                                                    string price = "";
        //        //                                                    if (itemDT1["RNewPrice"].ToString() != "" && itemDT1["RNewPrice"].ToString() != "0")
        //        //                                                        price = (Convert.ToDecimal(itemDT1["RNewPrice"]) / Convert.ToInt32(itemDT1["RQty"])).ToString("0.00");

        //        //                                                    if (price == "")
        //        //                                                    {
        //        //                                                        decimal ldisc = 0;
        //        //                                                        decimal odisc = 0;
        //        //                                                        decimal sdisc = 0;
        //        //                                                        if (dt.Rows[z]["LPromotionName"].ToString() != "")
        //        //                                                            ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
        //        //                                                        if (dt.Rows[z]["OPromotionName"].ToString() != "")
        //        //                                                            odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
        //        //                                                        if (dt.Rows[z]["SPromotionName"].ToString() != "")
        //        //                                                            sdisc = Convert.ToDecimal(dt.Rows[z]["Discount"]);
        //        //                                                        price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - odisc - sdisc - Convert.ToDecimal(dt.Rows[z]["RDiscount"])).ToString("0.00");
        //        //                                                    }
        //        //                                                    dt.Rows[z]["RPromotionName"] = dt.Rows[z]["RPROName"];
        //        //                                                    dt.Rows[z]["UnitRetail"] = price;
        //        //                                                    dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);


        //        //                                                    dt.Rows[z]["PromotionName"] = "";
        //        //                                                    dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
        //        //                                                }
        //        //                                            }

        //        //                                        }
        //        //                                    }
        //        //                                }
        //        //                            }
        //        //                        }
        //        //                        //    }
        //        //                        //}
        //        //                    }
        //        //                }

        //        //            }
        //        //        }
        //        //    }
        //        //}
        //        //foreach (DataRow distrinctRow in distrinctLPromotionName.AsEnumerable())
        //        //{
        //        //    if (lblLoyaltyId.Content is null)
        //        //        lblLoyaltyId.Content = "";
        //        //    if (distrinctRow["LPROName"].ToString() != "" && lblLoyaltyId.Content.ToString() != "")
        //        //    {
        //        //        if (loyaltyCustomerCount <= 5)
        //        //        {
        //        //            int sumCount = 0;
        //        //            for (int j = 0; j < distrinctLSCANCODE.AsEnumerable().Count(); j++)
        //        //            {
        //        //                if (distrinctLSCANCODE.Rows[j]["LPROName"].ToString() != "")
        //        //                {
        //        //                    for (int i = 0; i < dt.Rows.Count; i++)
        //        //                    {
        //        //                        if (distrinctLSCANCODE.Rows[j]["LPROName"].ToString() == distrinctRow["LPROName"].ToString())
        //        //                        {
        //        //                            //if (distrinctLSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
        //        //                            //{
        //        //                            sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
        //        //                            //for (int K = 0; K < dt.Rows.Count; K++)
        //        //                            //{
        //        //                            if (sumCount < 0)
        //        //                                sumCount = sumCount * -1;
        //        //                            foreach (DataRow itemDT1 in distrinctLSCANCODE.AsEnumerable())
        //        //                            {
        //        //                                if (itemDT1["LPROName"].ToString() == distrinctRow["LPROName"].ToString())
        //        //                                {
        //        //                                    int Y = sumCount / Convert.ToInt32(itemDT1["LQty"]);
        //        //                                    for (int x = 1; x <= Y; x++)
        //        //                                    {
        //        //                                        if (itemDT1["LType"].ToString() == "Once")
        //        //                                        {
        //        //                                            if (sumCount == Convert.ToInt32(itemDT1["LQty"]))
        //        //                                            {
        //        //                                                for (int z = 0; z <= i; z++)
        //        //                                                {

        //        //                                                    if (dt.Rows[z]["LPROName"].ToString() == distrinctRow["LPROName"].ToString())
        //        //                                                    {
        //        //                                                        string price = "";
        //        //                                                        if (itemDT1["LNewPrice"].ToString() != "" && itemDT1["LNewPrice"].ToString() != "0")
        //        //                                                            price = (Convert.ToDecimal(itemDT1["LNewPrice"]) / Convert.ToInt32(itemDT1["LQty"])).ToString("0.00");

        //        //                                                        if (price == "")
        //        //                                                        {
        //        //                                                            decimal odisc = 0;
        //        //                                                            decimal sdisc = 0;
        //        //                                                            decimal rdisc = 0;
        //        //                                                            if (dt.Rows[z]["OPromotionName"].ToString() != "")
        //        //                                                                odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
        //        //                                                            if (dt.Rows[z]["SPromotionName"].ToString() != "")
        //        //                                                                sdisc = Convert.ToDecimal(dt.Rows[z]["SDiscount"]);
        //        //                                                            if (dt.Rows[z]["RPromotionName"].ToString() != "")
        //        //                                                                rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
        //        //                                                            price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - odisc - sdisc - rdisc - (Convert.ToDecimal(dt.Rows[z]["LDiscount"]) / Convert.ToInt32(dt.Rows[z]["LQty"]))).ToString("0.00");
        //        //                                                        }
        //        //                                                        dt.Rows[z]["LPromotionName"] = dt.Rows[z]["LPROName"];
        //        //                                                        dt.Rows[z]["UnitRetail"] = price;
        //        //                                                        dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
        //        //                                                        dt.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();

        //        //                                                        dt.Rows[z]["PromotionName"] = "";
        //        //                                                        dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();

        //        //                                                        //dt.Rows[z]["LPromotionName"] = itemDT1["LPROName"];
        //        //                                                        //dt.Rows[z]["UnitRetail"] = price;
        //        //                                                        //dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
        //        //                                                    }
        //        //                                                }

        //        //                                            }
        //        //                                        }
        //        //                                        else
        //        //                                        {
        //        //                                            if (sumCount == Convert.ToInt32(itemDT1["LQty"]) * x)
        //        //                                            {
        //        //                                                for (int z = 0; z <= i; z++)
        //        //                                                {

        //        //                                                    if (dt.Rows[z]["LPROName"].ToString() == distrinctRow["LPROName"].ToString())
        //        //                                                    {
        //        //                                                        string price = "";
        //        //                                                        if (itemDT1["LNewPrice"].ToString() != "" && itemDT1["LNewPrice"].ToString() != "0")
        //        //                                                            price = (Convert.ToDecimal(itemDT1["LNewPrice"]) / Convert.ToInt32(itemDT1["LQty"])).ToString("0.00");

        //        //                                                        if (price == "")
        //        //                                                        {
        //        //                                                            decimal odisc = 0;
        //        //                                                            decimal sdisc = 0;
        //        //                                                            decimal rdisc = 0;
        //        //                                                            if (dt.Rows[z]["OPromotionName"].ToString() != "")
        //        //                                                                odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
        //        //                                                            if (dt.Rows[z]["SPromotionName"].ToString() != "")
        //        //                                                                sdisc = Convert.ToDecimal(dt.Rows[z]["SDiscount"]);
        //        //                                                            if (dt.Rows[z]["RPromotionName"].ToString() != "")
        //        //                                                                rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
        //        //                                                            price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - odisc - sdisc - rdisc - (Convert.ToDecimal(dt.Rows[z]["LDiscount"]) / Convert.ToInt32(dt.Rows[z]["LQty"]))).ToString("0.00");
        //        //                                                            //price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - odisc - sdisc - rdisc - Convert.ToDecimal(dt.Rows[z]["LDiscount"])).ToString("0.00");
        //        //                                                            //price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["LDiscount"])).ToString("0.00");
        //        //                                                        }
        //        //                                                        dt.Rows[z]["LPromotionName"] = dt.Rows[z]["LPROName"];
        //        //                                                        dt.Rows[z]["UnitRetail"] = price;
        //        //                                                        dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
        //        //                                                        dt.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();

        //        //                                                        dt.Rows[z]["PromotionName"] = "";
        //        //                                                        dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();

        //        //                                                        //dt.Rows[z]["LPromotionName"] = itemDT1["LPROName"];
        //        //                                                        //dt.Rows[z]["UnitRetail"] = price;
        //        //                                                        //dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
        //        //                                                    }
        //        //                                                }

        //        //                                            }
        //        //                                        }
        //        //                                    }
        //        //                                }
        //        //                            }
        //        //                            //}
        //        //                            //}
        //        //                        }
        //        //                    }

        //        //                }
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //        //foreach (DataRow distrinctRow in distrinctOPromotionName.AsEnumerable())
        //        //{
        //        //    if (distrinctRow["OPROName"].ToString() != "")
        //        //    {
        //        //        int sumCount = 0;
        //        //        for (int j = 0; j < distrinctOSCANCODE.AsEnumerable().Count(); j++)
        //        //        {
        //        //            if (distrinctOSCANCODE.Rows[j]["OPROName"].ToString() == distrinctRow["OPROName"].ToString())
        //        //            {
        //        //                for (int i = 0; i < dt.Rows.Count; i++)
        //        //                {
        //        //                    if (distrinctOSCANCODE.Rows[j]["OPROName"].ToString() == dt.Rows[i]["OPROName"].ToString())
        //        //                    {
        //        //                        //if (distrinctOSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
        //        //                        //{
        //        //                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
        //        //                        //for (int K = 0; K < dt.Rows.Count; K++)
        //        //                        //{
        //        //                        if (sumCount < 0)
        //        //                            sumCount = sumCount * -1;
        //        //                        foreach (DataRow itemDT1 in distrinctOSCANCODE.AsEnumerable())
        //        //                        {
        //        //                            if (itemDT1["OPROName"].ToString() == distrinctRow["OPROName"].ToString())
        //        //                            {
        //        //                                int Y = sumCount / Convert.ToInt32(itemDT1["OQty"]);
        //        //                                for (int x = 1; x <= Y; x++)
        //        //                                {
        //        //                                    if (itemDT1["OType"].ToString() == "Once")
        //        //                                    {
        //        //                                        if (sumCount == Convert.ToInt32(itemDT1["OQty"]))
        //        //                                        {
        //        //                                            for (int z = 0; z <= i; z++)
        //        //                                            {
        //        //                                                if (dt.Rows[z]["OPROName"].ToString() == distrinctRow["OPROName"].ToString())
        //        //                                                {
        //        //                                                    string price = "";
        //        //                                                    if (itemDT1["ONewPrice"].ToString() != "" && itemDT1["ONewPrice"].ToString() != "0")
        //        //                                                        price = (Convert.ToDecimal(itemDT1["ONewPrice"]) / Convert.ToInt32(itemDT1["OQty"])).ToString("0.00");

        //        //                                                    if (price == "")
        //        //                                                    {
        //        //                                                        decimal ldisc = 0;
        //        //                                                        decimal sdisc = 0;
        //        //                                                        decimal rdisc = 0;
        //        //                                                        if (dt.Rows[z]["LPromotionName"].ToString() != "")
        //        //                                                            ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
        //        //                                                        if (dt.Rows[z]["SPromotionName"].ToString() != "")
        //        //                                                            sdisc = Convert.ToDecimal(dt.Rows[z]["SDiscount"]);
        //        //                                                        if (dt.Rows[z]["RPromotionName"].ToString() != "")
        //        //                                                            rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
        //        //                                                        price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - sdisc - rdisc - Convert.ToDecimal(dt.Rows[z]["ODiscount"])).ToString("0.00");
        //        //                                                    }
        //        //                                                    dt.Rows[z]["OPromotionName"] = dt.Rows[z]["OPROName"];
        //        //                                                    dt.Rows[z]["UnitRetail"] = price;
        //        //                                                    dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
        //        //                                                    dt.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();

        //        //                                                    dt.Rows[z]["PromotionName"] = "";
        //        //                                                    dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
        //        //                                                }
        //        //                                            }

        //        //                                        }
        //        //                                    }
        //        //                                    else
        //        //                                    {
        //        //                                        if (sumCount == Convert.ToInt32(itemDT1["OQty"]) * x)
        //        //                                        {
        //        //                                            for (int z = 0; z <= i; z++)
        //        //                                            {
        //        //                                                if (dt.Rows[z]["OPROName"].ToString() == distrinctRow["OPROName"].ToString())
        //        //                                                {
        //        //                                                    string price = "";
        //        //                                                    if (itemDT1["ONewPrice"].ToString() != "" && itemDT1["ONewPrice"].ToString() != "0")
        //        //                                                        price = (Convert.ToDecimal(itemDT1["ONewPrice"]) / Convert.ToInt32(itemDT1["OQty"])).ToString("0.00");

        //        //                                                    if (price == "")
        //        //                                                    {
        //        //                                                        decimal ldisc = 0;
        //        //                                                        decimal sdisc = 0;
        //        //                                                        decimal rdisc = 0;
        //        //                                                        if (dt.Rows[z]["LPromotionName"].ToString() != "")
        //        //                                                            ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
        //        //                                                        if (dt.Rows[z]["SPromotionName"].ToString() != "")
        //        //                                                            sdisc = Convert.ToDecimal(dt.Rows[z]["SDiscount"]);
        //        //                                                        if (dt.Rows[z]["RPromotionName"].ToString() != "")
        //        //                                                            rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
        //        //                                                        price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - sdisc - rdisc - Convert.ToDecimal(dt.Rows[z]["ODiscount"])).ToString("0.00");
        //        //                                                    }
        //        //                                                    dt.Rows[z]["OPromotionName"] = dt.Rows[z]["OPROName"];
        //        //                                                    dt.Rows[z]["UnitRetail"] = price;
        //        //                                                    dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
        //        //                                                    dt.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();

        //        //                                                    dt.Rows[z]["PromotionName"] = "";
        //        //                                                    dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
        //        //                                                }
        //        //                                            }

        //        //                                        }
        //        //                                    }
        //        //                                }
        //        //                            }
        //        //                        }
        //        //                        //    }
        //        //                        //}
        //        //                    }
        //        //                }

        //        //            }
        //        //        }
        //        //    }
        //        //}


        //        // TotalEvent();

        //    }
        //    catch (Exception ex)
        //    {
        //        SendErrorToText(ex, errorFileName, "ScanCodeFunction");
        //    }
        //}

        public void PromotionApply()
        {
            try
            {
                if (dt.AsEnumerable().Count() > 0)
                {
                    int sumCount = 0;
                    if (loyaltyCustomerCount <= 5)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            DataTable dataTableDistr = new DataTable();
                            dataTableDistr.Columns.Add("PromotionId");
                            dataTableDistr.Columns.Add("PromotionName");
                            dataTableDistr.Columns.Add("newprice");
                            dataTableDistr.Columns.Add("Quantity");
                            dataTableDistr.Columns.Add("Discount");
                            dataTableDistr.Columns.Add("Type");
                            var PromotionIdSpl = dt.Rows[i]["PromotionId"].ToString().Split(',').ToList();
                            foreach (string itemSpl in PromotionIdSpl)
                            {
                                foreach (var drObj in dtPromotion.AsEnumerable().Where(z => z["PromotionId"].ToString() == itemSpl).AsEnumerable())
                                {
                                    DataRow dr = drObj;
                                    dataTableDistr.Rows.Add(dr.ItemArray);
                                }
                            }

                            foreach (var itemPromo in dataTableDistr.AsEnumerable())
                            {
                                sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                                if (sumCount < 0)
                                    sumCount = sumCount * -1;

                                foreach (var promotionspl in PromotionIdSpl.AsEnumerable())
                                {
                                    if (promotionspl == itemPromo["PromotionId"].ToString())
                                    {
                                        if (itemPromo["Type"].ToString() == "Multy")
                                        {
                                            int _qty = Convert.ToInt32(itemPromo["Quantity"]);

                                            if (sumCount > _qty)
                                            {
                                                if ((sumCount % _qty) == 0)
                                                {
                                                    if (sumCount != _qty)
                                                        _qty = sumCount;
                                                }
                                            }
                                            for (int x = 1; x <= _qty; x++)
                                            {
                                                if (sumCount == _qty * x)
                                                {
                                                    for (int z = 0; z <= i; z++)
                                                    {
                                                        if (dt.Rows[z]["PromotionId"].ToString().Split(',').ToList().Where(s => s.Contains(itemPromo["PromotionId"].ToString())).Count() != 0)
                                                        {
                                                            if (dt.Rows[i]["bIsTrueId"].ToString().Split(',').ToList().Where(s => s.Contains(itemPromo["PromotionId"].ToString())).Count() == 0)
                                                            {
                                                                string price = "";
                                                                if (itemPromo["NewPrice"].ToString() != "" && itemPromo["NewPrice"].ToString() != "0")
                                                                    price = (Convert.ToDecimal(itemPromo["NewPrice"]) / Convert.ToInt32(itemPromo["Quantity"])).ToString("0.00");
                                                                if (price == "")
                                                                {
                                                                    price = (Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) - Convert.ToDecimal(itemPromo["Discount"])).ToString("0.00");
                                                                }
                                                                dt.Rows[i]["UnitRetail"] = price;
                                                                dt.Rows[i]["Amount"] = Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"]);

                                                                if (dt.Rows[i]["PromotionName"].ToString() == "")
                                                                {
                                                                    dt.Rows[i]["PromotionName"] = itemPromo["PromotionName"].ToString();
                                                                    dt.Rows[i]["bIsTrueId"] = itemPromo["PromotionId"].ToString();
                                                                }
                                                                else
                                                                {
                                                                    dt.Rows[i]["PromotionName"] = dt.Rows[i]["PromotionName"].ToString() + " , " + itemPromo["PromotionName"].ToString();
                                                                    dt.Rows[i]["bIsTrueId"] = dt.Rows[i]["bIsTrueId"].ToString() + " , " + itemPromo["PromotionId"].ToString();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                            if (sumCount == Convert.ToInt32(itemPromo["Quantity"]))
                                            {
                                                for (int z = 0; z <= i; z++)
                                                {
                                                    if (dt.Rows[z]["PromotionId"].ToString().Split(',').ToList().Where(s => s.Contains(itemPromo["PromotionId"].ToString())).Count() != 0)
                                                    {
                                                        string price = "";
                                                        if (itemPromo["NewPrice"].ToString() != "" && itemPromo["NewPrice"].ToString() != "0")
                                                            price = (Convert.ToDecimal(itemPromo["NewPrice"]) / Convert.ToInt32(itemPromo["Quantity"])).ToString("0.00");
                                                        if (price == "")
                                                        {
                                                            price = (Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) - Convert.ToDecimal(itemPromo["Discount"])).ToString("0.00");
                                                        }
                                                        dt.Rows[z]["UnitRetail"] = price;
                                                        dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
                                                        if (dt.Rows[z]["PromotionName"].ToString() == "")
                                                            dt.Rows[z]["PromotionName"] = itemPromo["PromotionName"].ToString();
                                                        else
                                                            dt.Rows[z]["PromotionName"] = dt.Rows[z]["PromotionName"].ToString() + " , " + itemPromo["PromotionName"].ToString();
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    JRDGrid.ItemsSource = dt.DefaultView;
                    JRDGrid.Items.Refresh();
                    TotalEvent();

                    //DataTable distrinctPromotionName = dt;
                    //DataTable distrinctSCANCODE = dtPromotion;
                    //foreach (DataRow distrinctRow in distrinctPromotionName.AsEnumerable())
                    //{
                    //    int sumCount = 0;
                    //    for (int j = 0; j < distrinctSCANCODE.AsEnumerable().Count(); j++)
                    //    {
                    //        if (distrinctSCANCODE.Rows[j]["PROName"].ToString() == distrinctRow["PROName"].ToString())
                    //        {
                    //            for (int i = 0; i < dt.Rows.Count; i++)
                    //            {
                    //                if (distrinctSCANCODE.Rows[j]["PROName"].ToString() == dt.Rows[i]["PROName"].ToString())
                    //                {
                    //                    sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                    //                    if (sumCount < 0)
                    //                        sumCount = sumCount * -1;
                    //                    foreach (DataRow itemDT1 in distrinctSCANCODE.AsEnumerable())
                    //                    {
                    //                        if (itemDT1["PROName"].ToString() == distrinctRow["PROName"].ToString())
                    //                        {
                    //                            int Y = sumCount / Convert.ToInt32(itemDT1["Qty"]);
                    //                            for (int x = 1; x <= Y; x++)
                    //                            {
                    //                                if (itemDT1["Type"].ToString() == "Once")
                    //                                {
                    //                                    if (sumCount == Convert.ToInt32(itemDT1["Qty"]))
                    //                                    {
                    //                                        for (int z = 0; z <= i; z++)
                    //                                        {
                    //                                            if (dt.Rows[z]["PROName"].ToString() == distrinctRow["PROName"].ToString())
                    //                                            {
                    //                                                string price = "";
                    //                                                if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                    //                                                    price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString("0.00");
                    //                                                if (price == "")
                    //                                                {
                    //                                                    decimal ldisc = 0;
                    //                                                    decimal odisc = 0;
                    //                                                    decimal rdisc = 0;
                    //                                                    if (dt.Rows[z]["LPromotionName"].ToString() != "")
                    //                                                        ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
                    //                                                    if (dt.Rows[z]["OPromotionName"].ToString() != "")
                    //                                                        odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
                    //                                                    if (dt.Rows[z]["RPromotionName"].ToString() != "")
                    //                                                        rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
                    //                                                    price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - odisc - rdisc - Convert.ToDecimal(dt.Rows[z]["Discount"])).ToString("0.00");
                    //                                                }
                    //                                                dt.Rows[z]["SPromotionName"] = dt.Rows[z]["PROName"];
                    //                                                dt.Rows[z]["UnitRetail"] = price;
                    //                                                dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
                    //                                                dt.Rows[z]["PromotionName"] = "";
                    //                                                dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
                    //                                            }
                    //                                        }
                    //                                    }
                    //                                }
                    //                                else
                    //                                {
                    //                                    if (sumCount == Convert.ToInt32(itemDT1["Qty"]) * x)
                    //                                    {
                    //                                        for (int z = 0; z <= i; z++)
                    //                                        {
                    //                                            if (dt.Rows[z]["PROName"].ToString() == distrinctRow["PROName"].ToString())
                    //                                            {
                    //                                                string price = "";
                    //                                                if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                    //                                                    price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString("0.00");

                    //                                                if (price == "")
                    //                                                {
                    //                                                    decimal ldisc = 0;
                    //                                                    decimal odisc = 0;
                    //                                                    decimal rdisc = 0;
                    //                                                    if (dt.Rows[z]["LPromotionName"].ToString() != "")
                    //                                                        ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
                    //                                                    if (dt.Rows[z]["OPromotionName"].ToString() != "")
                    //                                                        odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
                    //                                                    if (dt.Rows[z]["RPromotionName"].ToString() != "")
                    //                                                        rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
                    //                                                    price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - odisc - rdisc - Convert.ToDecimal(dt.Rows[z]["Discount"])).ToString("0.00");
                    //                                                }
                    //                                                dt.Rows[z]["SPromotionName"] = dt.Rows[z]["PROName"];
                    //                                                dt.Rows[z]["UnitRetail"] = price;
                    //                                                dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
                    //                                                dt.Rows[z]["PromotionName"] = "";
                    //                                                dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
                    //                                            }
                    //                                        }
                    //                                    }
                    //                                }
                    //                            }
                    //                        }
                    //                    }
                    //                    //    }
                    //                    //}
                    //                }
                    //            }

                    //        }
                    //    }
                    //}
                    //TotalEvent();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("promotionfunction" + ex);
            }

        }
        private void Button_DayClose(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dtHold.Rows.Count == 0)
                {
                    PrintDocument = new PrintDocument();
                    PrintDocument.PrintPage += new PrintPageEventHandler(DayClosePrint);
                    PrintDocument.Print();
                    InsertQuery();
                }
                else { MessageBox.Show("Please Clear Hold Transaction"); }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_DayClose");
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
            sql_cmnd.Parameters.AddWithValue("@storeId", SqlDbType.NVarChar).Value = storeid;
            sql_cmnd.Parameters.AddWithValue("@posId", SqlDbType.NVarChar).Value = posId;
            sql_cmnd.ExecuteNonQuery();
            con.Close();
        }


        private void DayClosePrint(object sender, PrintPageEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryTrans = "select Count(tran_id)as Counts,sum(convert(decimal(10,2),GrossAmount))as Sales,sum(convert(decimal(10,2),TaxAmount))as Tax,sum(convert(decimal(10,2),grandAmount))as Total,min(convert(datetime,createon))as SDate,Max(convert(datetime,createon))as EDate from transactions where Dayclose is null and (void !=1 or void is Null)";
                SqlCommand cmdTrans = new SqlCommand(queryTrans, con);
                SqlDataAdapter sdaTrans = new SqlDataAdapter(cmdTrans);
                DataTable dtTrans = new DataTable();
                sdaTrans.Fill(dtTrans);

                string queryDept = "select Department,Sum(convert(decimal(10,2),amt)) as amt from(select Department, Sum(convert(decimal(10,2),Amount)) as amt from salesitem inner join item on salesitem.scancode = item.scancode where dayclose is null and(void != 1 or void is Null) group by Department Union all select Department,Sum(convert(decimal(10,2),Amount)) as amt from salesitem inner join Department on salesitem.Descripation = Department.Department where dayclose is null and(void != 1 or void is Null) group by Department)as x group by Department";
                SqlCommand cmdDept = new SqlCommand(queryDept, con);
                SqlDataAdapter sdaDept = new SqlDataAdapter(cmdDept);
                DataTable dtDept = new DataTable();
                sdaDept.Fill(dtDept);

                string queryTender = "select tendercode,sum(convert(decimal(10,2),amount)-convert(decimal(10,2),(case when change='' then '0' else change end)))as amt from tender where dayclose is null group by tendercode";
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

                DrawAtStart("            " + dtstr.Rows[0]["Address"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("            " + dtstr.Rows[0]["PhoneNumber"].ToString(), Offset);

                Offset = Offset + mediuminc;
                String underLine = "-------------------------------------";
                DrawLine(underLine, mediumfont, Offset, 0);

                Offset = Offset + mediuminc + 10;
                DrawAtStart("       Register :" + registerid, Offset);
                Offset = Offset + mediuminc;
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
                SendErrorToText(ex, errorFileName, "DayClosePrint");
            }
        }


        void button_Click_Category(object sender, RoutedEventArgs e, string xyz)
        {
            try
            {
                GoBack.Visibility = Visibility.Visible;
                //var btnContent = sender as Button;
                //var tb = (TextBlock)btnContent.Content;
                categorytext = xyz;
                Category1(sender, e);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "button_Click_Category");
            }
        }

        private void GrandTotal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal gransTotali = Convert.ToDecimal(grandTotal.Content.ToString().Replace("Pay $", ""));
                if (gransTotali != 0)
                {
                    GoBack.Visibility = Visibility.Hidden;
                    LeftArrow.Visibility = Visibility.Hidden;
                    RightArrow.Visibility = Visibility.Hidden;
                    if (refund == "")
                    {
                        ugDepartment.Visibility = Visibility.Hidden;
                        ugDepartment1.Visibility = Visibility.Hidden;
                        ugAddcategory1.Visibility = Visibility.Hidden;
                        ugAddcategory2.Visibility = Visibility.Hidden;
                        ugCategory1.Visibility = Visibility.Hidden;
                        grPayment.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        tenderCode = "Cash";
                        if (gransTotali < 0)
                            Button_Click_1();
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "GrandTotal_Click");
            }
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridSelectedIndex != "")
                {
                    gCustomer.Visibility = Visibility.Hidden;
                    gPriceCheck.Visibility = Visibility.Hidden;
                    uGHold.Visibility = Visibility.Visible;

                    int i = Convert.ToInt32(dataGridSelectedIndex);
                    if (dt.Rows[i]["PromotionId"].ToString() != "")
                    {
                        for (int a = 0; a < 1; a++)
                        {
                            DataRow newRow = dt.NewRow();
                            newRow["ScanCode"] = dt.Rows[i]["ScanCode"].ToString();
                            newRow["Description"] = dt.Rows[i]["Description"].ToString();
                            newRow["Quantity"] = 1;
                            newRow["UnitRetail"] = dt.Rows[i]["OPrice"].ToString();
                            newRow["Amount"] = dt.Rows[i]["OPrice"].ToString();
                            newRow["OPrice"] = dt.Rows[i]["OPrice"].ToString();
                            newRow["TaxRate"] = dt.Rows[i]["TaxRate"].ToString();
                            newRow["PromotionId"] = dt.Rows[i]["PromotionId"].ToString();
                            newRow["bIsTrueId"] = "";
                            dt.Rows.Add(newRow);
                        }
                        PromotionApply();
                        JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                        JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                    }
                    else
                    {
                        dt.Rows[i]["Quantity"] = Convert.ToDecimal(dt.Rows[i]["Quantity"]) + 1;
                        dt.Rows[i]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"])).ToString("0.00");
                        JRDGrid.ItemsSource = dt.DefaultView;
                        TotalEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Plus_Click");
            }
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridSelectedIndex != "")
                {
                    gCustomer.Visibility = Visibility.Hidden;
                    gPriceCheck.Visibility = Visibility.Hidden;
                    uGHold.Visibility = Visibility.Visible;

                    int i = Convert.ToInt32(dataGridSelectedIndex);
                    if (Convert.ToDecimal(dt.Rows[i]["Quantity"]) > 1)
                    {
                        dt.Rows[i]["Quantity"] = Convert.ToDecimal(dt.Rows[i]["Quantity"]) - 1;
                        //if (dt.Rows[i]["PromotionId"].ToString() != "")
                        //{
                        //    int qDT = Convert.ToInt32(dt.Rows[i]["Quantity"]);
                        //    int qDT1 = Convert.ToInt32(dt.Rows[i]["Qty"]);

                        //    if (qDT >= qDT1)
                        //    {

                        //        int QA = qDT1 * (qDT / qDT1);
                        //        if (dt.Rows[i]["NewPrice"].ToString() != "" && dt.Rows[i]["NewPrice"].ToString() != "0")
                        //        {
                        //            dt.Rows[i]["PromotionName"] = dt.Rows[i]["PROName"];
                        //            dt.Rows[i]["Quantity"] = QA;
                        //            dt.Rows[i]["UnitRetail"] = Convert.ToDecimal(dt.Rows[i]["NewPrice"]) / qDT1;
                        //            dt.Rows[i]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"])).ToString("0.00");
                        //        }
                        //        else
                        //        {
                        //            dt.Rows[i]["PromotionName"] = dt.Rows[i]["PROName"];
                        //            dt.Rows[i]["Quantity"] = QA;
                        //            dt.Rows[i]["UnitRetail"] = Convert.ToDecimal(dt.Rows[i]["OPrice"]) - Convert.ToDecimal(dt.Rows[i]["Discount"]);
                        //            dt.Rows[i]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"])).ToString("0.00");
                        //        }
                        //        int QB = qDT - QA;
                        //        if (QB != 0)
                        //        {
                        //            for (int a = 0; a < QB; a++)
                        //            {
                        //                DataRow newRow = dt.NewRow();
                        //                newRow["ScanCode"] = dt.Rows[i]["ScanCode"];
                        //                newRow["Description"] = dt.Rows[i]["Description"];
                        //                newRow["Quantity"] = 1;
                        //                newRow["UnitRetail"] = dt.Rows[i]["OPrice"];
                        //                newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                        //                newRow["OPrice"] = dt.Rows[i]["OPrice"];
                        //                newRow["PromotionName"] = "";
                        //                newRow["TaxRate"] = dt.Rows[i]["TaxRate"];
                        //                newRow["PROName"] = dt.Rows[i]["PROName"];
                        //                newRow["Qty"] = dt.Rows[i]["Qty"];
                        //                newRow["NewPrice"] = dt.Rows[i]["NewPrice"];
                        //                newRow["Discount"] = dt.Rows[i]["Discount"];
                        //                newRow["DiscountBy"] = dt.Rows[i]["DiscountBy"];
                        //                dt.Rows.Add(newRow);
                        //            }
                        //        }
                        //    }

                        //    int intv = qDT1 * (qDT / qDT1);
                        //    decimal ab = qDT / qDT1;
                        //    decimal decv = Convert.ToDecimal(qDT1) * Convert.ToDecimal(qDT) / Convert.ToDecimal(qDT1);

                        //    ScanCodeFunction();

                        //}
                        //else
                        //{
                        dt.Rows[i]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"])).ToString("0.00");
                        //}
                        JRDGrid.ItemsSource = dt.DefaultView;
                        TotalEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Minus_Click");
            }
        }

        private void CashReceive(object sender, RoutedEventArgs e)
        {
            try
            {
                tenderCode = "Cash";
                decimal inumber = 0;
                decimal old = 0;
                decimal sum = 0;
                decimal sale = 0;
                decimal returned = 0;
                sale = Convert.ToDecimal(grandTotal.Content.ToString().Replace("Pay $", ""));
                inumber = Convert.ToDecimal((sender as Button).Content.ToString().Replace("$ ", ""));
                if (TxtCashReceive.Text != "")
                {
                    old = Convert.ToDecimal(TxtCashReceive.Text.Replace("$ ", ""));
                }
                else { old = 0; }

                sum = old + inumber;
                returned = sum - sale;
                TxtCashReceive.Text = "$ " + sum;
                TxtCashReturn.Text = "$ " + returned;
                if (sum >= sale)
                {
                    Button_Click_1();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "CashReceive");
            }
        }

        private void Click_VoidTransaction(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt.Rows.Count > 0 || dtVoidItem.Rows.Count > 0)
                {
                    foreach (DataRow row in dtVoidItem.Rows)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["ScanCode"] = row.ItemArray[0].ToString();
                        newRow["Description"] = row.ItemArray[1].ToString();
                        newRow["Quantity"] = row.ItemArray[4].ToString();
                        newRow["UnitRetail"] = row.ItemArray[2].ToString();
                        newRow["Amount"] = row.ItemArray[5].ToString();
                        newRow["OPrice"] = row.ItemArray[7].ToString();
                        newRow["TaxRate"] = row.ItemArray[3].ToString();
                        newRow["Void"] = 1;
                        dt.Rows.Add(newRow);
                    }
                    dtVoidItem.Clear();

                    SqlConnection con = new SqlConnection(conString);
                    string date = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
                    string onlydate = date.Substring(0, 10);
                    string onlytime = date.Substring(11);
                    string totalAmt = txtTotal.Content.ToString().Replace("$", "");
                    string tax = taxtTotal.Content.ToString().Replace("$", "");
                    string grandTotalAmt = grandTotal.Content.ToString().Replace("Pay $", "");
                    string cashRec = "0";
                    string cashReturn = "0";
                    string tranid = Convert.ToInt32(lblTranid.Content).ToString();

                    string transaction = "insert into Transactions(Tran_id,EndDate,EndTime,GrossAmount,TaxAmount,GrandAmount,CreateBy,CreateOn,Void,storeid,posid,Register_id)Values('" + tranid + "','" + onlydate + "','" + onlytime + "','" + totalAmt + "','" + tax + "','" + grandTotalAmt + "','" + username + "','" + date + "','1','" + storeid + "','" + posId + "','" + registerid + "')";
                    SqlCommand cmd = new SqlCommand(transaction, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    if (tenderCode == "Cash")
                    {
                        string tender = "insert into Tender(EndDate,Endtime,TenderCode,Amount,Change,TransactionId,CreateBy,CreateOn,storeid,posid,,RegisterId)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + cashRec + "','" + cashReturn + "','" + tranid + "','" + username + "','" + date + "','" + storeid + "','" + posId + "','" + registerid + "')";
                        SqlCommand cmdTender = new SqlCommand(tender, con);
                        con.Open();
                        cmdTender.ExecuteNonQuery();
                        con.Close();
                    }
                    else if (tenderCode == "Card")
                    {
                        string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn,storeid,posid,,RegisterId)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + username + "','" + date + "','" + storeid + "','" + posId + "','" + registerid + "')";
                        SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                        con.Open();
                        cmdTender1.ExecuteNonQuery();
                        con.Close();
                    }
                    else if (tenderCode == "Customer")
                    {
                        string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,AccountName,CreateBy,CreateOn,storeid,posid,,RegisterId)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + cbcustomer.Text + "','" + username + "','" + date + "','" + storeid + "','" + posId + "','" + registerid + "')";
                        SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                        con.Open();
                        cmdTender1.ExecuteNonQuery();
                        con.Close();
                    }
                    else if (tenderCode == "Check")
                    {
                        string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CheckNo,CreateBy,CreateOn,storeid,posid,,RegisterId)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + TxtCheck.Text + "','" + username + "','" + date + "','" + storeid + "','" + posId + "','" + registerid + "')";
                        SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                        con.Open();
                        cmdTender1.ExecuteNonQuery();
                        con.Close();
                    }
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        dataRow[6] = onlydate;
                        dataRow[7] = onlytime;
                        dataRow[8] = tranid;
                        dataRow[9] = username;
                        dataRow[10] = date;
                        dataRow[17] = '1';
                        dataRow["RegisterId"] = registerid;
                        dataRow["POSId"] = posId;
                        dataRow["StoreId"] =storeid;
                    }

                    SqlBulkCopy objbulk = new SqlBulkCopy(con);
                    objbulk.DestinationTableName = "SalesItem";
                    objbulk.ColumnMappings.Add("scanCode", "ScanCode");
                    objbulk.ColumnMappings.Add("description", "Descripation");
                    objbulk.ColumnMappings.Add("quantity", "Quantity");
                    objbulk.ColumnMappings.Add("unitretail", "Price");
                    objbulk.ColumnMappings.Add("Amount", "Amount");
                    objbulk.ColumnMappings.Add("Date", "EndDate");
                    objbulk.ColumnMappings.Add("Time", "EndTime");
                    objbulk.ColumnMappings.Add("PromotionName", "PromotionName");
                    objbulk.ColumnMappings.Add("TransactionId", "TransactionId");
                    objbulk.ColumnMappings.Add("CreateBy", "CreateBy");
                    objbulk.ColumnMappings.Add("CreateOn", "CreateOn");
                    objbulk.ColumnMappings.Add("Void", "Void");
                    objbulk.ColumnMappings.Add("RegisterId", "RegisterId");
                    objbulk.ColumnMappings.Add("POSId", "POSId");
                    objbulk.ColumnMappings.Add("StoreId", "StoreId");

                    con.Open();
                    objbulk.WriteToServer(dt);
                    con.Close();
                    TxtCashReturn.Text = "";
                    TxtCashReceive.Text = "";
                    cbcustomer.Text = "";
                    TxtCheck.Text = "";
                    txtTotal.Content = "";
                    tenderCode = "";
                    grandTotal.Content = "Pay " + "$" + "0.00";
                    taxtTotal.Content = "";
                    lblDate.Content = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
                    dt.Clear();
                    JRDGrid.Items.Refresh();
                    if (grPayment.Visibility.ToString() == "Visible")
                    {
                        cashTxtPanel.Visibility = Visibility.Hidden;
                        ugDepartment.Visibility = Visibility.Visible;
                        customerTxtPanel.Visibility = Visibility.Hidden;
                        checkTxtPanel.Visibility = Visibility.Hidden;
                        grPayment.Visibility = Visibility.Hidden;
                    }
                    //loadtransactionId();
                    transId = transId + 1;
                    lblTranid.Content = transId;
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Click_VoidTransaction");
            }
        }

        private void OnClick_PriceCheck(object sender, RoutedEventArgs e)
        {
            try
            {
                string visibility = gPriceCheck.Visibility.ToString();
                if (visibility == "Visible")
                {
                    gPriceCheck.Visibility = Visibility.Hidden;
                    uGHold.Visibility = Visibility.Visible;
                    gCustomer.Visibility = Visibility.Hidden;
                    textBox1.Focus();
                    btnPriceCheck.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    uGHold.Visibility = Visibility.Hidden;
                    gPriceCheck.Visibility = Visibility.Visible;
                    gCustomer.Visibility = Visibility.Hidden;
                    txtBarcode.Focus();
                    btnPriceCheck.Foreground = new SolidColorBrush(Colors.DeepPink);
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "OnClick_PriceCheck");
            }
        }

        private void TxtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);

                if (e.Key == Key.Enter || e.Key == Key.Tab)
                {
                    priceCheck();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "TxtBarcode_KeyDown");
            }
        }

        private void priceCheck()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                var code = txtBarcode.Text;
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
                txtBarcode.Text = code;
                decimal de = 0;
                var str = from myRow in dtItem.AsEnumerable()
                          where myRow.Field<string>("ScanCode") == code
                          select myRow;
                foreach (DataRow rows in str)
                {
                    de = Convert.ToDecimal(rows.ItemArray[2].ToString());
                }
                //string query = "select UnitRetail from Item where Item.Scancode=@password ";
                //SqlCommand cmd = new SqlCommand(query, con);
                //cmd.Parameters.AddWithValue("@password", code);
                //SqlDataAdapter sda = new SqlDataAdapter(cmd);
                //DataTable dtprice = new DataTable();
                //sda.Fill(dtprice);
                //lblUnitRetail.Content = Convert.ToString(Convert.ToDecimal(dtprice.Rows[0]["UnitRetail"].ToString()));
                //
                //de = Convert.ToDecimal(dtprice.Rows[0]["UnitRetail"]);
                lblUnitRetail.Content = "$ " + de.ToString("0.00");
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "priceCheck");
            }
        }

        private void JRDGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    dataGridSelectedIndex = dataGrid.SelectedIndex.ToString();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "JRDGrid_SelectionChanged");
            }
        }

        private void Button_Click_VoidItem(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridSelectedIndex != "")
                {
                    int isi = Convert.ToInt32(dataGridSelectedIndex);
                    string str = dt.Rows[isi]["PromotionName"].ToString();

                    DataRow newRow = dtVoidItem.NewRow();
                    newRow["Scancode"] = dt.Rows[isi]["ScanCode"].ToString();
                    newRow["Description"] = dt.Rows[isi]["Description"].ToString();
                    newRow["UnitRetail"] = dt.Rows[isi]["UnitRetail"].ToString();
                    newRow["TaxRate"] = dt.Rows[isi]["TaxRate"].ToString();
                    newRow["Quantity"] = dt.Rows[isi]["Quantity"].ToString();
                    newRow["Amount"] = dt.Rows[isi]["Amount"].ToString();
                    newRow["Void"] = "1";
                    newRow["Oprice"] = dt.Rows[isi]["Oprice"].ToString();
                    dtVoidItem.Rows.Add(newRow);

                    dt.Rows[isi].Delete();

                    if (str != "")
                    {
                        if (dt.AsEnumerable().Count() != 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dt.Rows[i]["PromotionName"] = "";
                                dt.Rows[i]["bIsTrueId"] = "";
                                dt.Rows[i]["LoyaltyId"] = "";
                                dt.Rows[i]["UnitRetail"] = dt.Rows[i]["Oprice"];
                                dt.Rows[i]["Amount"] = Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"]);
                            }
                            PromotionApply();
                        }
                    }
                    else
                    {
                        TotalEvent();
                    }

                    if (JRDGrid.Items.Count > 1)
                    {
                        JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                        JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                    }
                    //TotalEvent();
                }
               
                dataGridSelectedIndex = "";

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_VoidItem");
            }
        }

        private void Button_Click_Receipt(object sender, RoutedEventArgs e)
        {
            try
            {
                string visibility = gReceipt.Visibility.ToString();
                if (visibility == "Visible")
                {
                    gReceipt.Visibility = Visibility.Hidden;
                    Click_ClosegReceipt(e, e);
                }
                else
                {
                    gReceipt.Visibility = Visibility.Visible;
                    TxtBxStackPanel2.Visibility = Visibility.Hidden;
                    grPayment.Visibility = Visibility.Hidden;

                    ugDepartment.Visibility = Visibility.Hidden;
                    ugDepartment1.Visibility = Visibility.Hidden;
                    ugCategory1.Visibility = Visibility.Hidden;
                    TxtBxStackPanel2.Visibility = Visibility.Hidden;
                    ugAddcategory2.Visibility = Visibility.Hidden;
                    ugAddcategory1.Visibility = Visibility.Hidden;

                    SqlConnection con = new SqlConnection(conString);
                    string edate = Convert.ToDateTime(lblDate.Content).ToString("yyyy/MM/dd");
                    string querytrans = "select Tran_id as TransactionId,EndTime,TaxAmount,GrandAmount from transactions where enddate=@date and Void is null";
                    SqlCommand cmdTransaction = new SqlCommand(querytrans, con);
                    cmdTransaction.Parameters.AddWithValue("@date", edate);
                    SqlDataAdapter sdatrans = new SqlDataAdapter(cmdTransaction);

                    sdatrans.Fill(dtTransaction);
                    dgTransaction.CanUserAddRows = false;
                    dgTransaction.ItemsSource = dtTransaction.AsDataView();
                    //dgTransaction.ItemsSource = dtTrans.DefaultView;
                    //dgTransaction.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_Receipt");
            }
        }

        private void DgTransaction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    dt.Clear();
                    decimal total = 0;
                    int it = Convert.ToInt32(dataGrid.SelectedIndex.ToString());
                    lblTranid.Content = dtTransaction.Rows[it]["TransactionId"].ToString();
                    decimal grandAmount = Convert.ToDecimal(dtTransaction.Rows[it]["GrandAmount"].ToString());
                    decimal tax = Convert.ToDecimal(dtTransaction.Rows[it]["TaxAmount"].ToString());
                    total = grandAmount - tax;
                    lblDate.Content = Convert.ToDateTime(lblDate.Content).ToString("yyyy/MM/dd") + " " + dtTransaction.Rows[it]["EndTime"].ToString();
                    txtTotal.Content = '$' + Convert.ToDecimal(total).ToString("0.00");
                    taxtTotal.Content = '$' + Convert.ToDecimal(tax).ToString("0.00");
                    grandTotal.Content = "Pay " + '$' + Convert.ToDecimal(grandAmount).ToString("0.00");

                    SqlConnection con = new SqlConnection(conString);
                    string edate = Convert.ToDateTime(lblDate.Content).ToString("yyyy/MM/dd");
                    string querytrans = "select ScanCode,Descripation as Description,Quantity,Price,Amount from salesItem where TransactionId=@transid and enddate='" + edate + "' and Void is null";
                    SqlCommand cmdTransaction = new SqlCommand(querytrans, con);
                    cmdTransaction.Parameters.AddWithValue("@transid", lblTranid.Content);
                    SqlDataAdapter sdatrans = new SqlDataAdapter(cmdTransaction);
                    sdatrans.Fill(dt);
                    lblCount.Content = dt.Rows.Count;
                    JRDGrid.ItemsSource = dt.DefaultView;
                    JRDGrid.Items.Refresh();
                    JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                    JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                    grandTotal.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "DgTransaction_SelectionChanged");
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToString(lblTranid.Content) != "")
                {
                    PrintDocument = new PrintDocument();
                    PrintDocument.PrintPage += new PrintPageEventHandler(FormatPage);
                    PrintDocument.Print();
                    lblTranid.Content = "";
                    lblDate.Content =
                    txtTotal.Content = '$' + " " + "0.00";
                    taxtTotal.Content = '$' + " " + "0.00";
                    grandTotal.Content = "Pay " + '$' + " " + "0.00";
                    loadtransactionId();
                    lblDate.Content = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                    dt.Clear();
                    JRDGrid.Items.Refresh();
                    dtTransaction.Clear();
                    dgTransaction.Items.Refresh();
                    gReceipt.Visibility = Visibility.Hidden;
                    TxtBxStackPanel2.Visibility = Visibility.Hidden;
                    grPayment.Visibility = Visibility.Hidden;

                    ugDepartment.Visibility = Visibility.Visible;
                    ugCategory1.Visibility = Visibility.Hidden;
                    TxtBxStackPanel2.Visibility = Visibility.Hidden;
                    ugAddcategory2.Visibility = Visibility.Hidden;
                    ugAddcategory1.Visibility = Visibility.Hidden;
                    grandTotal.Visibility = Visibility.Visible;

                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "BtnPrint_Click");
            }
        }

        private void Button_Click_Refund(object sender, RoutedEventArgs e)
        {
            try
            {
                if (refund == "Refund")
                {
                    refund = "";
                    btnRefund.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    refund = "Refund";
                    btnRefund.Foreground = new SolidColorBrush(Colors.DeepPink);
                }
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "Button_Click_Refund"); }
        }

        private void Click_ClosegReceipt(object sender, RoutedEventArgs e)
        {
            try
            {
                lblCount.Content = "";
                lblTranid.Content = "";
                lblDate.Content =
                txtTotal.Content = '$' + " " + "0.00";
                taxtTotal.Content = '$' + " " + "0.00";
                grandTotal.Content = "Pay " + '$' + " " + "0.00";
                loadtransactionId();
                lblDate.Content = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");
                dt.Clear();
                JRDGrid.Items.Refresh();
                dtTransaction.Clear();
                dgTransaction.Items.Refresh();
                gReceipt.Visibility = Visibility.Hidden;
                TxtBxStackPanel2.Visibility = Visibility.Hidden;
                grPayment.Visibility = Visibility.Hidden;
                ugDepartment.Visibility = Visibility.Visible;
                ugCategory1.Visibility = Visibility.Hidden;
                ugAddcategory2.Visibility = Visibility.Hidden;
                ugAddcategory1.Visibility = Visibility.Hidden;
                grandTotal.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Click_ClosegReceipt");
            }
        }

        private void Hold_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    SqlConnection con = new SqlConnection(conString);
                    string date = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
                    string onlydate = date.Substring(0, 10);
                    string onlytime = date.Substring(11);
                    string totalAmt = txtTotal.Content.ToString().Replace("$", "");
                    string tax = taxtTotal.Content.ToString().Replace("$", "");
                    string grandTotalAmt = grandTotal.Content.ToString().Replace("Pay $", "");
                    string tranid = Convert.ToInt32(lblTranid.Content).ToString();
                    string customer = cbCustomer1.Text;

                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow newRow = dtHold.NewRow();
                        newRow["ScanCode"] = row["ScanCode"].ToString();
                        newRow["Description"] = row["Description"].ToString();
                        newRow["UnitRetail"] = row["UnitRetail"].ToString();
                        newRow["TaxRate"] = row["TaxRate"].ToString();
                        newRow["Quantity"] = row["Quantity"].ToString();
                        newRow["Amount"] = row["Amount"].ToString();
                        newRow["Date"] = onlydate;
                        newRow["Time"] = onlytime;
                        newRow["TransactionId"] = tranid;
                        newRow["CreateBy"] = username;
                        newRow["CreateOn"] = date;
                        newRow["PromotionName"] = row["PromotionName"].ToString();
                        newRow["Void"] = row["Void"].ToString();
                        newRow["Oprice"] = row["Oprice"].ToString();
                        newRow["LoyaltyId"] = row["LoyaltyId"].ToString();
                        newRow["bIsTrueId"] = row["bIsTrueId"].ToString();
                        newRow["Customer"] = customer;
                        dtHold.Rows.Add(newRow);
                    }
                    TxtCashReturn.Text = "";
                    TxtCashReceive.Text = "";
                    cbcustomer.Text = "";
                    cbCustomer1.Text = "--Select--";
                    lblCount.Content = "";
                    TxtCheck.Text = "";
                    txtTotal.Content = "";
                    tenderCode = "";
                    grandTotal.Content = "Pay " + "$" + "0.00";
                    taxtTotal.Content = "";
                    lblDate.Content = DateTime.Now.ToString("yyyy/MM/dd HH:MM:ss");
                    dt.Clear();
                    JRDGrid.Items.Refresh();
                    refund = "";
                    cashTxtPanel.Visibility = Visibility.Hidden;
                    //ugDepartment.Visibility = Visibility.Visible;
                    customerTxtPanel.Visibility = Visibility.Hidden;
                    checkTxtPanel.Visibility = Visibility.Hidden;
                    grPayment.Visibility = Visibility.Hidden;
                    //loadtransactionId();
                    if (transId == Convert.ToInt32(lblTranid.Content))
                    {
                        transId = transId + 1;
                    }
                    lblTranid.Content = transId;
                    loadHold();
                }
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "Hold_Click"); }
        }

        private void loadHold()
        {
            try
            {
                gCustomer.Visibility = Visibility.Hidden;
                uGHold.Visibility = Visibility.Visible;
                uGHold.Children.Clear();
                //dtHold.Reset();
                //SqlConnection con = new SqlConnection(conString);
                //string queryS = "Select distinct trasactionId from Hold";
                //SqlCommand cmd1 = new SqlCommand(queryS, con);
                //SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                //sda1.Fill(dtHold);
                DataTable distrinctTransactionId = dtHold.DefaultView.ToTable(true, "TransactionId");
                for (int i = 0; i < distrinctTransactionId.Rows.Count; ++i)
                {
                    Button button = new Button();
                    lblHoldTransaction.Content = "Hold Transaction";
                    var size = System.Windows.SystemParameters.PrimaryScreenWidth;

                    button.Content = new TextBlock()
                    {
                        FontSize = 20,
                        Text = dtHold.Rows[i].ItemArray[8].ToString(),
                        TextAlignment = TextAlignment.Left,
                        TextWrapping = TextWrapping.Wrap
                    };

                    button.Width = 70;
                    button.Height = 50;
                    button.HorizontalAlignment = HorizontalAlignment.Center;
                    button.VerticalAlignment = VerticalAlignment.Top;
                    button.Foreground = new SolidColorBrush(Colors.Black);
                    button.FontSize = 15;
                    button.FontWeight = FontWeights.Bold;
                    button.Margin = new Thickness(5);

                    string abc = dtHold.Rows[i].ItemArray[8].ToString();
                    button.Click += (sender, e) => { button_Click_Hold(sender, e, abc); };
                    this.uGHold.HorizontalAlignment = HorizontalAlignment.Center;
                    this.uGHold.VerticalAlignment = VerticalAlignment.Top;
                    //ColumnDefinition cd = new ColumnDefinition();
                    //cd.Width = GridLength.Auto;
                    this.uGHold.Columns = 4;
                    this.uGHold.Children.Add(button);
                    lblHoldTransaction.Content = "";
                }
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "loadHold"); }
        }

        private void button_Click_Hold(object sender, RoutedEventArgs e, string abc)
        {
            try
            {
                if (dt.Rows.Count == 0)
                {
                    TxtCashReceive.Text = "";
                    TxtCashReturn.Text = "";
                    grPayment.Visibility = Visibility.Hidden;
                    var btnContent = sender as Button;
                    string tb = ((TextBlock)btnContent.Content).Text;
                    lblTranid.Content = tb;
                    var results = from myRow in dtHold.AsEnumerable()
                                  where myRow.Field<string>("TransactionId") == tb
                                  select myRow;

                    foreach (DataRow row in results)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["ScanCode"] = row["ScanCode"].ToString();
                        newRow["Description"] = row["Description"].ToString();
                        newRow["UnitRetail"] = row["UnitRetail"].ToString();
                        newRow["TaxRate"] = row["TaxRate"].ToString();
                        newRow["Quantity"] = row["Quantity"].ToString();
                        newRow["Amount"] = row["Amount"].ToString();
                        newRow["Date"] = row["Date"].ToString();
                        newRow["Time"] = row["Time"].ToString();
                        newRow["TransactionId"] = row["TransactionId"].ToString();
                        newRow["CreateBy"] = row["CreateBy"].ToString();
                        newRow["CreateOn"] = row["CreateOn"].ToString();
                        newRow["PromotionName"] = row["PromotionName"].ToString();
                        newRow["Void"] = row["Void"].ToString();
                        newRow["Oprice"] = row["Oprice"].ToString();
                        newRow["LoyaltyId"] = row["LoyaltyId"].ToString();
                        newRow["Customer"] = row["Customer"].ToString();
                        newRow["bIsTrueId"] = row["bIsTrueId"].ToString();
                        dt.Rows.Add(newRow);
                    }
                    List<DataRow> rowsToRemove = dtHold.AsEnumerable()
                                   .Where(r => r.Field<string>("TransactionId") == tb).ToList();
                    rowsToRemove.ForEach(dtHold.Rows.Remove);

                    JRDGrid.ItemsSource = dt.DefaultView;
                    JRDGrid.Items.Refresh();
                    JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                    JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                    TotalEvent();
                    //string qholdDelete = "Delete from Hold where TrasactionId=@transid";
                    //SqlCommand cmdHoldDelete = new SqlCommand(qholdDelete, con);
                    //cmdHoldDelete.Parameters.AddWithValue("@transid", tb);
                    //con.Open();
                    //cmdHoldDelete.ExecuteNonQuery();
                    //con.Close();
                    Button SelectedButton = (Button)sender;
                    uGHold.Children.Remove(SelectedButton);
                    cbCustomer1.Text = dt.Rows[0]["Customer"].ToString();
                    lblLoyaltyId.Content = dt.Rows[0]["LoyaltyId"].ToString();
                    loadHold();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "button_Click_Hold");
            }
        }

        private void LeftArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ugDepartment1.Visibility.ToString() == "Visible")
                {
                    ugDepartment.Visibility = Visibility.Visible;
                    ugDepartment1.Visibility = Visibility.Hidden;
                }
                else if (ugAddcategory2.Visibility.ToString() == "Visible")
                {
                    ugAddcategory1.Visibility = Visibility.Visible;
                    ugAddcategory2.Visibility = Visibility.Hidden;
                }
                else if (ugCategory2.Visibility.ToString() == "Visible")
                {
                    Category1(sender, e);
                }
                RightArrow.IsEnabled = true;
                LeftArrow.IsEnabled = false;
                RightArrow.Visibility = Visibility.Visible;
                LeftArrow.Visibility = Visibility.Hidden;
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "LeftArrow_Click"); }
        }

        private void RightArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ugDepartment.Visibility.ToString() == "Visible")
                {
                    ugDepartment.Visibility = Visibility.Hidden;
                    ugDepartment1.Visibility = Visibility.Visible;
                }
                else if (ugAddcategory1.Visibility.ToString() == "Visible")
                {
                    ugAddcategory1.Visibility = Visibility.Hidden;
                    ugAddcategory2.Visibility = Visibility.Visible;
                }
                else if (ugCategory1.Visibility.ToString() == "Visible")
                {
                    //Category2(sender, e);
                    ugCategory1.Visibility = Visibility.Hidden;
                    ugCategory2.Visibility = Visibility.Visible;
                }
                RightArrow.IsEnabled = false;
                RightArrow.Visibility = Visibility.Hidden;
                LeftArrow.Visibility = Visibility.Visible;
                LeftArrow.IsEnabled = true;
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName, "RightArrow_Click"); }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grPayment.Visibility = Visibility.Hidden;
                GoBack.Visibility = Visibility.Hidden;
                ugAddcategory1.Visibility = Visibility.Visible;
                ugAddcategory2.Visibility = Visibility.Hidden;
                ugCategory1.Visibility = Visibility.Hidden;
                ugCategory2.Visibility = Visibility.Hidden;
                ugDepartment.Visibility = Visibility.Hidden;
                ugDepartment1.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "GoBack_Click");
            }
        }

        private void Button_Click_Customer(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gCustomer.Visibility.ToString() == "Hidden")
                {
                    gCustomer.Visibility = Visibility.Visible;
                    gPriceCheck.Visibility = Visibility.Hidden;
                    uGHold.Visibility = Visibility.Hidden;
                }
                else
                {
                    textBox1.Focus();
                    gCustomer.Visibility = Visibility.Hidden;
                    gPriceCheck.Visibility = Visibility.Hidden;
                    uGHold.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_Customer");
            }

        }

        private void CbCustomer1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cmb = sender as ComboBox;
                int cc = cmb.SelectedIndex;
                if (cc > 0)
                {
                    lblLoyaltyId.Content = dtAccount.Rows[cc]["LoyaltyId"].ToString();
                    loyaltyCustomerCount = Convert.ToInt32(dtAccount.Rows[cc]["Count"]);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["UnitRetail"] = dt.Rows[i]["OPrice"];
                        dt.Rows[i]["Amount"] = Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"]);
                        dt.Rows[i]["PromotionId"] = "";
                        dt.Rows[i]["LoyaltyId"] = "";
                        dt.Rows[i]["Customer"] = "";
                        dt.Rows[i]["UnitRetail"] = dt.Rows[i]["UnitRetail"];
                    }
                    PromotionApply();
                    if (JRDGrid.Items.Count != 0)
                    {
                        JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                        JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                    }
                    TotalEvent();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "CbCustomer1_SelectionChanged");
            }
        }

        private void NoSale_Click(object sender, RoutedEventArgs e)
        {
            Report rpt = new Report();
            rpt.Show();
        }

        private void button_Click_Category_Description(object sender, RoutedEventArgs e)
        {
            try
            {
                gCustomer.Visibility = Visibility.Hidden;
                uGHold.Visibility = Visibility.Visible;
                gPriceCheck.Visibility = Visibility.Hidden;

                var btnContent = (sender as Button);
                string tb = Convert.ToString(btnContent.Tag);

                //int A = (from DataRow row in dtCategory.Rows where (string)row["Category"] == tb select row).Count();
                int A = dtCategory.AsEnumerable().Where(c => c.Field<string>("category") == tb).Count();

                if (A != 0)
                    button_Click_Category(sender, e, tb);
                else
                {
                    var results = (from myRow in dtItem.AsEnumerable()
                                   where myRow.Field<string>("Description") == tb
                                   select myRow).AsEnumerable();

                    foreach (DataRow row in results)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["ScanCode"] = row["ScanCode"].ToString();
                        newRow["Description"] = row["Description"].ToString();
                        if (refund == "")
                            newRow["Quantity"] = 1;
                        else
                            newRow["Quantity"] = -1;
                        newRow["UnitRetail"] = row["UnitRetail"].ToString();
                        newRow["Amount"] = (Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(row["UnitRetail"])).ToString();
                        newRow["OPrice"] = row["UnitRetail"].ToString();
                        newRow["TaxRate"] = row["TaxRate"].ToString();
                        newRow["PromotionId"] = row["PromotionId"].ToString();
                        newRow["bIsTrueId"] = "";

                        dt.Rows.Add(newRow);
                    }

                    if (dt.Rows.Count != 0)
                    {
                        int dCount = dt.Rows.Count - 1;
                        PromotionApply();
                        JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                        JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                        categorytext = "";
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "button_Click_Category_Description");
            }
        }

        private void Button_Click_Enter(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtGotFocusStr == "textBox1")
                {
                    BarcodeMethod();
                }
                if (txtGotFocusStr == "TxtCashReceive")
                {
                    if (Convert.ToDecimal(TxtCashReceive.Text) >= Convert.ToDecimal(grandTotal.Content.ToString().Replace("Pay $", "")))
                    {
                        TxtCashReturn.Text = decimal.Parse(Convert.ToDecimal(decimal.Parse(TxtCashReceive.Text) - decimal.Parse(grandTotal.Content.ToString().Replace("Pay $", ""))).ToString("0.00")).ToString("0.00");
                        Button_Click_1();
                    }
                }
                if (txtGotFocusStr == "TxtCheck")
                {
                    Button_Click_1();
                }
                if (txtGotFocusStr == "txtDeptAmt")
                {
                    Button_Click_Sale_Save(sender, e);
                }
                if (txtGotFocusStr == "CellEditQty")
                {
                    CellEditMethod();
                }
                if (txtGotFocusStr == "txtBarcode")
                {
                    priceCheck();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "Button_Click_Enter");
            }
        }

        void selectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (JRDGrid.CurrentColumn != null)
                {
                    DataGridColumn column = JRDGrid.CurrentColumn;
                    if (column.Header != null)
                    {
                        if (column.Header.ToString() == "Quantity")
                        {
                            txtGotFocusStr = "CellEditQty";
                            dtInndex = column.DisplayIndex;

                            int i = dt.Rows.Count - 1;
                            DataRow dataRow = dt.Rows[i];
                            dt.Rows[i]["Quantity"] = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "selectedCellsChanged");
            }
        }

        public void CellEditMethod()
        {
            try
            {
                int rowIndex = dt.Rows.Count - 1;
                DataRow dataRow = dt.Rows[rowIndex];
                if (dt.Rows[rowIndex]["PromotionId"].ToString() != "")
                {
                    for (int a = 0; a < Convert.ToInt32(dt.Rows[rowIndex]["Quantity"]); a++)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["ScanCode"] = dt.Rows[rowIndex]["ScanCode"];
                        newRow["Description"] = dt.Rows[rowIndex]["Description"];
                        newRow["Quantity"] = 1;
                        newRow["UnitRetail"] = dt.Rows[rowIndex]["OPrice"];
                        newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                        newRow["OPrice"] = dt.Rows[rowIndex]["OPrice"];
                        newRow["TaxRate"] = dt.Rows[rowIndex]["TaxRate"];
                        newRow["PromotionName"] = "";
                        newRow["PromotionId"] = dt.Rows[rowIndex]["PromotionId"];
                        newRow["bIsTrueId"] = "";

                        dt.Rows.Add(newRow);
                    }
                    PromotionApply();
                }
                else
                {
                    dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                    JRDGrid.ItemsSource = dt.DefaultView;
                    TotalEvent();
                }
                JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName, "CellEditMethod");
            }
        }

        public bool isDecimal(string value)
        {
            try
            {
                Decimal.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SendErrorToText(Exception ex, string errorFileName, string funName)
        {
            var line = ex.Message; //Environment.NewLine + Environment.NewLine;
            ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();
            MessageBox.Show("Message : " + line + Environment.NewLine + "FileName : " + errorFileName + Environment.NewLine + "Function Name : " + funName, "Error");
            ErrorLocation = ex.Message.ToString();
            try
            {
                string filepath = System.AppDomain.CurrentDomain.BaseDirectory;
                string errorpath = filepath + "\\ErrorFiles\\";

                if (!Directory.Exists(errorpath))
                {
                    Directory.CreateDirectory(errorpath);
                }

                filepath = filepath + "log.txt";

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
