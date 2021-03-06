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
        string refund = "";
        int transId = 0;
        string categorytext = "";
        DataTable dt = new DataTable();
        DataTable dtVoidItem = new DataTable();
        DataTable dtdepartment = new DataTable();
        DataTable dtAccount = new DataTable();
        DataTable dtItem = new DataTable();
        DataTable dtTransaction = new DataTable();
        DataTable dtAddCategory = new DataTable();
        DataTable dtCategory = new DataTable();
        DataTable dtstr = new DataTable();
        DataTable dtHold = new DataTable();
        string username = App.Current.Properties["username"].ToString();
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
                dt.Columns.Add("Oprice");
                dt.Columns.Add("PROName");
                dt.Columns.Add("Qty");
                dt.Columns.Add("NewPrice");
                dt.Columns.Add("Discount");
                dt.Columns.Add("DiscountBY");
                dt.Columns.Add("SPromotionName");
                dt.Columns.Add("RPROName");
                dt.Columns.Add("RQty");
                dt.Columns.Add("RNewPrice");
                dt.Columns.Add("RDiscount");
                dt.Columns.Add("RPromotionName");
                dt.Columns.Add("LPROName");
                dt.Columns.Add("LQty");
                dt.Columns.Add("LNewPrice");
                dt.Columns.Add("LDiscount");
                dt.Columns.Add("LPromotionName");
                dt.Columns.Add("OPROName");
                dt.Columns.Add("OQty");
                dt.Columns.Add("ONewPrice");
                dt.Columns.Add("ODiscount");
                dt.Columns.Add("OPromotionName");
                dt.Columns.Add("LoyaltyId");
                dt.Columns.Add("Customer");

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
                dtHold.Columns.Add("PROName");
                dtHold.Columns.Add("Qty");
                dtHold.Columns.Add("NewPrice");
                dtHold.Columns.Add("Discount");
                dtHold.Columns.Add("DiscountBY");
                dtHold.Columns.Add("SPromotionName");
                dtHold.Columns.Add("RPROName");
                dtHold.Columns.Add("RQty");
                dtHold.Columns.Add("RNewPrice");
                dtHold.Columns.Add("RDiscount");
                dtHold.Columns.Add("RPromotionName");
                dtHold.Columns.Add("LPROName");
                dtHold.Columns.Add("LQty");
                dtHold.Columns.Add("LNewPrice");
                dtHold.Columns.Add("LDiscount");
                dtHold.Columns.Add("LPromotionName");
                dtHold.Columns.Add("OPROName");
                dtHold.Columns.Add("OQty");
                dtHold.Columns.Add("ONewPrice");
                dtHold.Columns.Add("ODiscount");
                dtHold.Columns.Add("OPromotionName");
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

                LoadDepartment();
                loadDropdownCustomer();
                loadtransactionId();
                loadHold();
                addCategory1();
                Category();
                LoadItem();
                // fill Store Details
                StoreDetails();
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
                SendErrorToText(ex, errorFileName);
            }
        }

        private void LoadItem()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string query = "select item.Scancode,item.Description,Convert(decimal(10,2),UnitRetail)as UnitRetail,Department.TaxRate,x.PromotionName AS PROName,x.Quantity as Qty,newprice,Discount,RProName,RQty,RNewPrice,RDiscount,LProName,LQty,LNewPrice,LDiscount,OProName,OQty,ONewPrice,ODiscount from Item inner join Department on rtrim(item.Department)=rtrim(Department.Department) left join(select scancode, Promotion.promotionName, newprice, Quantity, Discount,DiscountBy from promotiongroup inner join promotion on promotiongroup.promotionname = promotion.promotiongroup where Convert(date, GETDATE()) between Convert(date, startdate) and Convert(date, enddate) and DiscountBY='ScanData')as x on item.scancode = x.scancode left join(select scancode, Promotion.promotionName as RProName, newprice as RNewPrice, Quantity as RQty, Discount as RDiscount,DiscountBy as RDiscountBy from promotiongroup inner join promotion on promotiongroup.promotionname = promotion.promotiongroup where Convert(date, GETDATE()) between Convert(date, startdate) and Convert(date, enddate) and DiscountBY='Retailer')as y on item.scancode = y.scancode left join(select scancode, Promotion.promotionName as LProName, newprice as LNewPrice, Quantity as LQty, Discount as LDiscount,DiscountBy as LDiscountBy from promotiongroup inner join promotion on promotiongroup.promotionname = promotion.promotiongroup where Convert(date, GETDATE()) between Convert(date, startdate) and Convert(date, enddate) and DiscountBY='Loyalty')as z on item.scancode = z.scancode left join(select scancode, Promotion.promotionName as OProName, newprice as ONewPrice, Quantity as OQty, Discount as ODiscount,DiscountBy as ODiscountBy from promotiongroup inner join promotion on promotiongroup.promotionname = promotion.promotiongroup where Convert(date, GETDATE()) between Convert(date, startdate) and Convert(date, enddate) and DiscountBY='Other')as a on item.scancode = a.scancode";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dtItem);
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
        }

        private void loadDropdownCustomer()
        {
            try
            {
                dtAccount.Columns.Add("Name");
                DataRow newRow = dtAccount.NewRow();
                newRow["Name"] = "--Select--";
                dtAccount.Rows.Add(newRow);

                SqlConnection con = new SqlConnection(conString);
                string queryCustomer = "select Name,Mobile from Account where Head='Customers'";
                SqlCommand cmdcustomer = new SqlCommand(queryCustomer, con);
                SqlDataAdapter sdacustomer = new SqlDataAdapter(cmdcustomer);

                sdacustomer.Fill(dtAccount);
                cbcustomer.ItemsSource = dtAccount.DefaultView;
                cbcustomer.DisplayMemberPath = "Name";
                cbCustomer1.ItemsSource = dtAccount.DefaultView;
                cbCustomer1.DisplayMemberPath = "Name";
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
        }

        private void LoadDepartment()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryS = "Select Department,TaxRate,FilePath from Department";
                SqlCommand cmd1 = new SqlCommand(queryS, con);
                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                sda1.Fill(dtdepartment);
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
                            var fullpath = Path + "\\Image\\" + path;
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
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
        }

        private void addCategory1()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryAddCat1 = "select category,CategoryImage from addcategory";
                SqlCommand cmdAddCat1 = new SqlCommand(queryAddCat1, con);
                SqlDataAdapter sdaAddCat1 = new SqlDataAdapter(cmdAddCat1);
                sdaAddCat1.Fill(dtAddCategory);

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
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Category()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryS = "select Description,categoryimage,category from category";
                SqlCommand cmd1 = new SqlCommand(queryS, con);
                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                sda1.Fill(dtCategory);
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
            }
        }

        private void loadtransactionId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conString))
                {
                    string query1 = "select coalesce(max(convert(int,tran_id)),0)as tran_id from(SELECT tran_id FROM Transactions where EndDate='" + date + "' union all SELECT distinct TrasactionId FROM Hold)as x";
                    SqlCommand cmd2 = new SqlCommand(query1, conn);
                    SqlDataAdapter sdaT = new SqlDataAdapter(cmd2);
                    DataTable dttranid = new DataTable();
                    sdaT.Fill(dttranid);

                    lblTranid.Content = Convert.ToInt32(dttranid.Rows[0]["tran_id"].ToString()) + 1;
                    transId = Convert.ToInt32(dttranid.Rows[0]["tran_id"].ToString()) + 1;
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
            }
        }
        private void Button_Click_Sale_Save(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);

                if (e.Key == Key.Enter || e.Key == Key.Tab)
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

                    foreach (DataRow row in results)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["ScanCode"] = row.ItemArray[0].ToString();
                        newRow["Description"] = row.ItemArray[1].ToString();
                        if (refund == "")
                            newRow["Quantity"] = 1;
                        else
                            newRow["Quantity"] = -1;
                        newRow["UnitRetail"] = row.ItemArray[2].ToString();
                        newRow["Amount"] = row.ItemArray[2].ToString();
                        newRow["OPrice"] = row.ItemArray[2].ToString();
                        newRow["TaxRate"] = row.ItemArray[3].ToString();
                        newRow["PROName"] = row.ItemArray[4].ToString();
                        newRow["Qty"] = row.ItemArray[5].ToString();
                        newRow["NewPrice"] = row.ItemArray[6].ToString();
                        newRow["Discount"] = row.ItemArray[7].ToString();
                        newRow["RPROName"] = row.ItemArray[8].ToString();
                        newRow["RQty"] = row.ItemArray[9].ToString();
                        newRow["RNewPrice"] = row.ItemArray[10].ToString();
                        newRow["RDiscount"] = row.ItemArray[11].ToString();
                        newRow["LPROName"] = row.ItemArray[12].ToString();
                        newRow["LQty"] = row.ItemArray[13].ToString();
                        newRow["LNewPrice"] = row.ItemArray[14].ToString();
                        newRow["LDiscount"] = row.ItemArray[15].ToString();
                        newRow["OPROName"] = row.ItemArray[16].ToString();
                        newRow["OQty"] = row.ItemArray[17].ToString();
                        newRow["ONewPrice"] = row.ItemArray[18].ToString();
                        newRow["ODiscount"] = row.ItemArray[19].ToString();
                        dt.Rows.Add(newRow);
                    }
                    IEnumerable<DataRow> dt1 = (from row in dt.AsEnumerable()
                                                where row["Void"].ToString() != "1"
                                                select row).ToList<DataRow>();

                    int dCount = dt.AsEnumerable().Count() - 1;
                    if (dCount >= 0)
                    {
                        if (dt.Rows[dCount]["PROName"].ToString() != "")
                        {
                            DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "PROName", "Qty", "NewPrice", "Discount");

                            foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                            {
                                if (distinct["PROName"].ToString() != "")
                                {
                                    int sumCount = (from row in dt.AsEnumerable()
                                                    where row.Field<string>("PROName") == distinct["PROName"].ToString()
                                                    select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                    if (sumCount < 0)
                                        sumCount = sumCount * -1;
                                    foreach (var itemdt in dt.AsEnumerable())
                                    {
                                        if (itemdt["PROName"].ToString() == distinct["PROName"].ToString())
                                        {
                                            for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                            {
                                                if (sumCount == Convert.ToInt32(distinct["Qty"]) * i)
                                                {
                                                    if (itemdt["SPromotionName"] != itemdt["PROName"])
                                                    {
                                                        string price = "";
                                                        if (itemdt["NewPrice"].ToString() != "" && itemdt["NewPrice"].ToString() != "0")
                                                            price = (Convert.ToDecimal(itemdt["NewPrice"]) / Convert.ToInt32(itemdt["Qty"])).ToString();

                                                        if (price == "")
                                                        {
                                                            decimal odisc = 0;
                                                            decimal ldisc = 0;
                                                            decimal rdisc = 0;
                                                            if (itemdt["OPromotionName"].ToString() != "")
                                                                odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                                                            if (itemdt["LPromotionName"].ToString() != "")
                                                                ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                                                            if (itemdt["RPromotionName"].ToString() != "")
                                                                rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                                                            price = (Convert.ToDecimal(itemdt["Oprice"]) - odisc - ldisc - rdisc - Convert.ToDecimal(itemdt["Discount"])).ToString();
                                                        }
                                                        itemdt["SPromotionName"] = itemdt["PROName"];
                                                        itemdt["UnitRetail"] = price;
                                                        itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                                                        itemdt["PromotionName"] = "";
                                                        itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (dt.Rows[dCount]["RPROName"].ToString() != "")
                        {
                            DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "RPROName", "RQty", "RNewPrice", "RDiscount");

                            foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                            {
                                if (distinct["RPROName"].ToString() != "")
                                {
                                    int sumCount = (from row in dt.AsEnumerable()
                                                    where row.Field<string>("RPROName") == distinct["RPROName"].ToString()
                                                    select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                    if (sumCount < 0)
                                        sumCount = sumCount * -1;
                                    foreach (var itemdt in dt.AsEnumerable())
                                    {
                                        if (itemdt["RPROName"].ToString() == distinct["RPROName"].ToString())
                                        {
                                            for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                            {
                                                if (sumCount == Convert.ToInt32(distinct["RQty"]) * i)
                                                {
                                                    if (itemdt["RPromotionName"] != itemdt["RPROName"])
                                                    {

                                                        string price = "";


                                                        if (itemdt["RNewPrice"].ToString() != "" && itemdt["RNewPrice"].ToString() != "0")
                                                            price = (Convert.ToDecimal(itemdt["RNewPrice"]) / Convert.ToInt32(itemdt["RQty"])).ToString();

                                                        if (price == "")
                                                        {
                                                            decimal odisc = 0;
                                                            decimal ldisc = 0;
                                                            decimal sdisc = 0;
                                                            if (itemdt["OPromotionName"].ToString() != "")
                                                                odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                                                            if (itemdt["LPromotionName"].ToString() != "")
                                                                ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                                                            if (itemdt["SPromotionName"].ToString() != "")
                                                                sdisc = Convert.ToDecimal(itemdt["Discount"]);

                                                            price = (Convert.ToDecimal(itemdt["Oprice"]) - sdisc - ldisc - odisc - Convert.ToDecimal(itemdt["RDiscount"])).ToString();
                                                        }
                                                        itemdt["RPromotionName"] = itemdt["RPROName"];
                                                        itemdt["UnitRetail"] = price;
                                                        itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);

                                                        itemdt["PromotionName"] = "";
                                                        itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (dt.Rows[dCount]["LPROName"].ToString() != "")
                        {
                            DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "LPROName", "LQty", "LNewPrice", "LDiscount");

                            foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                            {
                                if (distinct["LPROName"].ToString() != "")
                                {
                                    int sumCount = (from row in dt.AsEnumerable()
                                                    where row.Field<string>("LPROName") == distinct["LPROName"].ToString() && row.Field<string>("Void") != "1"
                                                    select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                    if (sumCount < 0)
                                        sumCount = sumCount * -1;
                                    foreach (var itemdt in dt.AsEnumerable())
                                    {
                                        if (itemdt["LPROName"].ToString() == distinct["LPROName"].ToString())
                                        {
                                            for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                            {
                                                if (sumCount == Convert.ToInt32(distinct["LQty"]) * i)
                                                {
                                                    if (lblLoyaltyId.Content.ToString() != "")
                                                    {
                                                        if (itemdt["LPromotionName"] != itemdt["LPROName"])
                                                        {
                                                            string price = "";
                                                            if (itemdt["LNewPrice"].ToString() != "" && itemdt["LNewPrice"].ToString() != "0")
                                                                price = (Convert.ToDecimal(itemdt["LNewPrice"]) / Convert.ToInt32(itemdt["LQty"])).ToString();

                                                            if (price == "")
                                                            {
                                                                decimal odisc = 0;
                                                                decimal sdisc = 0;
                                                                decimal rdisc = 0;
                                                                if (itemdt["OPromotionName"].ToString() != "")
                                                                    odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                                                                if (itemdt["SPromotionName"].ToString() != "")
                                                                    sdisc = Convert.ToDecimal(itemdt["Discount"]);
                                                                if (itemdt["RPromotionName"].ToString() != "")
                                                                    rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                                                                price = (Convert.ToDecimal(itemdt["Oprice"]) - odisc - sdisc - rdisc - Convert.ToDecimal(itemdt["LDiscount"])).ToString();
                                                            }

                                                            itemdt["LPromotionName"] = itemdt["LPROName"];
                                                            itemdt["UnitRetail"] = price;
                                                            itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                                                            itemdt["LoyaltyId"] = lblLoyaltyId.Content.ToString();

                                                            itemdt["PromotionName"] = "";
                                                            itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (dt.Rows[dCount]["OPROName"].ToString() != "")
                        {
                            DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "OPROName", "OQty", "ONewPrice", "ODiscount");

                            foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                            {
                                if (distinct["OPROName"].ToString() != "")
                                {
                                    int sumCount = (from row in dt.AsEnumerable()
                                                    where row.Field<string>("OPROName") == distinct["OPROName"].ToString() && row.Field<string>("Void") != "1"
                                                    select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                    if (sumCount < 0)
                                        sumCount = sumCount * -1;
                                    foreach (var itemdt in dt.AsEnumerable())
                                    {
                                        if (itemdt["OPROName"].ToString() == distinct["OPROName"].ToString())
                                        {
                                            for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                            {
                                                if (sumCount == Convert.ToInt32(distinct["OQty"]) * i)
                                                {
                                                    if (itemdt["OPromotionName"] != itemdt["OPROName"])
                                                    {
                                                        string price = "";
                                                        if (itemdt["ONewPrice"].ToString() != "" && itemdt["ONewPrice"].ToString() != "0")
                                                            price = (Convert.ToDecimal(itemdt["ONewPrice"]) / Convert.ToInt32(itemdt["OQty"])).ToString();

                                                        if (price == "")
                                                        {
                                                            decimal ldisc = 0;
                                                            decimal sdisc = 0;
                                                            decimal rdisc = 0;
                                                            if (itemdt["LPromotionName"].ToString() != "")
                                                                ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                                                            if (itemdt["SPromotionName"].ToString() != "")
                                                                sdisc = Convert.ToDecimal(itemdt["Discount"]);
                                                            if (itemdt["RPromotionName"].ToString() != "")
                                                                rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                                                            price = (Convert.ToDecimal(itemdt["Oprice"]) - ldisc - sdisc - rdisc - Convert.ToDecimal(itemdt["ODiscount"])).ToString();
                                                        }

                                                        itemdt["OPromotionName"] = itemdt["OPROName"];
                                                        itemdt["UnitRetail"] = price;
                                                        itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);

                                                        itemdt["PromotionName"] = "";
                                                        itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
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
                        JRDGrid.ScrollIntoView(JRDGrid.Items[dCount]);
                        JRDGrid.SelectedIndex = dCount;
                        TotalEvent();
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
                SendErrorToText(ex, errorFileName);
            }
        }

        public void BarcodeMethod()
        {
            try
            {
                grPayment.Visibility = Visibility.Hidden;
                ugDepartment.Visibility = Visibility.Visible;
                btnShortKey.Visibility = Visibility.Visible;
                btnDept.Visibility = Visibility.Hidden;
                ugDepartment1.Visibility = Visibility.Hidden;
                ugCategory1.Visibility = Visibility.Hidden;
                ugCategory2.Visibility = Visibility.Hidden;
                ugAddcategory1.Visibility = Visibility.Hidden;
                ugAddcategory2.Visibility = Visibility.Hidden;
                GoBack.Visibility = Visibility.Hidden;
                gCustomer.Visibility = Visibility.Hidden;
                uGHold.Visibility = Visibility.Visible;
                gPriceCheck.Visibility = Visibility.Hidden;
                var code = textBox1.Text;
                if (code != "")
                {
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

                    foreach (DataRow row in results)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["ScanCode"] = row.ItemArray[0].ToString();
                        newRow["Description"] = row.ItemArray[1].ToString();
                        if (refund == "")
                            newRow["Quantity"] = 1;
                        else
                            newRow["Quantity"] = 1;
                        newRow["UnitRetail"] = row.ItemArray[2].ToString();
                        newRow["Amount"] = row.ItemArray[2].ToString();
                        newRow["OPrice"] = row.ItemArray[2].ToString();
                        newRow["TaxRate"] = row.ItemArray[3].ToString();
                        newRow["PROName"] = row.ItemArray[4].ToString();
                        newRow["Qty"] = row.ItemArray[5].ToString();
                        newRow["NewPrice"] = row.ItemArray[6].ToString();
                        newRow["Discount"] = row.ItemArray[7].ToString();
                        newRow["RPROName"] = row.ItemArray[8].ToString();
                        newRow["RQty"] = row.ItemArray[9].ToString();
                        newRow["RNewPrice"] = row.ItemArray[10].ToString();
                        newRow["RDiscount"] = row.ItemArray[11].ToString();
                        newRow["LPROName"] = row.ItemArray[12].ToString();
                        newRow["LQty"] = row.ItemArray[13].ToString();
                        newRow["LNewPrice"] = row.ItemArray[14].ToString();
                        newRow["LDiscount"] = row.ItemArray[15].ToString();
                        newRow["OPROName"] = row.ItemArray[16].ToString();
                        newRow["OQty"] = row.ItemArray[17].ToString();
                        newRow["ONewPrice"] = row.ItemArray[18].ToString();
                        newRow["ODiscount"] = row.ItemArray[19].ToString();
                        dt.Rows.Add(newRow);
                    }
                }
                int dCount = dt.AsEnumerable().Count() - 1;
                if (dCount >= 0)
                {
                    if (dt.Rows[dCount]["PROName"].ToString() != "")
                    {
                        DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "PROName", "Qty", "NewPrice", "Discount");

                        foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                        {
                            if (distinct["PROName"].ToString() != "")
                            {
                                int sumCount = (from row in dt.AsEnumerable()
                                                where row.Field<string>("PROName") == distinct["PROName"].ToString()
                                                select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                if (sumCount < 0)
                                    sumCount = sumCount * -1;
                                foreach (var itemdt in dt.AsEnumerable())
                                {
                                    if (itemdt["PROName"].ToString() == distinct["PROName"].ToString())
                                    {
                                        for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                        {
                                            if (sumCount == Convert.ToInt32(distinct["Qty"]) * i)
                                            {
                                                if (itemdt["SPromotionName"] != itemdt["PROName"])
                                                {
                                                    string price = "";
                                                    if (itemdt["NewPrice"].ToString() != "" && itemdt["NewPrice"].ToString() != "0")
                                                        price = (Convert.ToDecimal(itemdt["NewPrice"]) / Convert.ToInt32(itemdt["Qty"])).ToString();

                                                    if (price == "")
                                                    {
                                                        decimal odisc = 0;
                                                        decimal ldisc = 0;
                                                        decimal rdisc = 0;
                                                        if (itemdt["OPromotionName"].ToString() != "")
                                                            odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                                                        if (itemdt["LPromotionName"].ToString() != "")
                                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                                                        if (itemdt["RPromotionName"].ToString() != "")
                                                            rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - odisc - ldisc - rdisc - Convert.ToDecimal(itemdt["Discount"])).ToString();
                                                    }
                                                    itemdt["SPromotionName"] = itemdt["PROName"];
                                                    itemdt["UnitRetail"] = price;
                                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                                                    itemdt["PromotionName"] = "";
                                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (dt.Rows[dCount]["RPROName"].ToString() != "")
                    {
                        DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "RPROName", "RQty", "RNewPrice", "RDiscount");

                        foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                        {
                            if (distinct["RPROName"].ToString() != "")
                            {
                                int sumCount = (from row in dt.AsEnumerable()
                                                where row.Field<string>("RPROName") == distinct["RPROName"].ToString()
                                                select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                if (sumCount < 0)
                                    sumCount = sumCount * -1;
                                foreach (var itemdt in dt.AsEnumerable())
                                {
                                    if (itemdt["RPROName"].ToString() == distinct["RPROName"].ToString())
                                    {
                                        for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                        {
                                            if (sumCount == Convert.ToInt32(distinct["RQty"]) * i)
                                            {
                                                if (itemdt["RPromotionName"] != itemdt["RPROName"])
                                                {

                                                    string price = "";


                                                    if (itemdt["RNewPrice"].ToString() != "" && itemdt["RNewPrice"].ToString() != "0")
                                                        price = (Convert.ToDecimal(itemdt["RNewPrice"]) / Convert.ToInt32(itemdt["RQty"])).ToString();

                                                    if (price == "")
                                                    {
                                                        decimal odisc = 0;
                                                        decimal ldisc = 0;
                                                        decimal sdisc = 0;
                                                        if (itemdt["OPromotionName"].ToString() != "")
                                                            odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                                                        if (itemdt["LPromotionName"].ToString() != "")
                                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                                                        if (itemdt["SPromotionName"].ToString() != "")
                                                            sdisc = Convert.ToDecimal(itemdt["Discount"]);

                                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - sdisc - ldisc - odisc - Convert.ToDecimal(itemdt["RDiscount"])).ToString();
                                                    }
                                                    itemdt["RPromotionName"] = itemdt["RPROName"];
                                                    itemdt["UnitRetail"] = price;
                                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);

                                                    itemdt["PromotionName"] = "";
                                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (dt.Rows[dCount]["LPROName"].ToString() != "")
                    {
                        DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "LPROName", "LQty", "LNewPrice", "LDiscount");

                        foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                        {
                            if (distinct["LPROName"].ToString() != "")
                            {
                                int sumCount = (from row in dt.AsEnumerable()
                                                where row.Field<string>("LPROName") == distinct["LPROName"].ToString()
                                                select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                if (sumCount < 0)
                                    sumCount = sumCount * -1;
                                foreach (var itemdt in dt.AsEnumerable())
                                {
                                    if (itemdt["LPROName"].ToString() == distinct["LPROName"].ToString())
                                    {
                                        for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                        {
                                            if (sumCount == Convert.ToInt32(distinct["LQty"]) * i)
                                            {
                                                if (lblLoyaltyId.Content.ToString() != "")
                                                {
                                                    if (itemdt["LPromotionName"] != itemdt["LPROName"])
                                                    {
                                                        string price = "";
                                                        if (itemdt["LNewPrice"].ToString() != "" && itemdt["LNewPrice"].ToString() != "0")
                                                            price = (Convert.ToDecimal(itemdt["LNewPrice"]) / Convert.ToInt32(itemdt["LQty"])).ToString();

                                                        if (price == "")
                                                        {
                                                            decimal odisc = 0;
                                                            decimal sdisc = 0;
                                                            decimal rdisc = 0;
                                                            if (itemdt["OPromotionName"].ToString() != "")
                                                                odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                                                            if (itemdt["SPromotionName"].ToString() != "")
                                                                sdisc = Convert.ToDecimal(itemdt["Discount"]);
                                                            if (itemdt["RPromotionName"].ToString() != "")
                                                                rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                                                            price = (Convert.ToDecimal(itemdt["Oprice"]) - odisc - sdisc - rdisc - Convert.ToDecimal(itemdt["LDiscount"])).ToString();
                                                        }

                                                        itemdt["LPromotionName"] = itemdt["LPROName"];
                                                        itemdt["UnitRetail"] = price;
                                                        itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                                                        itemdt["LoyaltyId"] = lblLoyaltyId.Content.ToString();

                                                        itemdt["PromotionName"] = "";
                                                        itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (dt.Rows[dCount]["OPROName"].ToString() != "")
                    {
                        DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "OPROName", "OQty", "ONewPrice", "ODiscount");

                        foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                        {
                            if (distinct["OPROName"].ToString() != "")
                            {
                                int sumCount = (from row in dt.AsEnumerable()
                                                where row.Field<string>("OPROName") == distinct["OPROName"].ToString()
                                                select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                if (sumCount < 0)
                                    sumCount = sumCount * -1;
                                foreach (var itemdt in dt.AsEnumerable())
                                {
                                    if (itemdt["OPROName"].ToString() == distinct["OPROName"].ToString())
                                    {
                                        for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                        {
                                            if (sumCount == Convert.ToInt32(distinct["OQty"]) * i)
                                            {
                                                if (itemdt["OPromotionName"] != itemdt["OPROName"])
                                                {
                                                    string price = "";
                                                    if (itemdt["ONewPrice"].ToString() != "" && itemdt["ONewPrice"].ToString() != "0")
                                                        price = (Convert.ToDecimal(itemdt["ONewPrice"]) / Convert.ToInt32(itemdt["OQty"])).ToString();

                                                    if (price == "")
                                                    {
                                                        decimal ldisc = 0;
                                                        decimal sdisc = 0;
                                                        decimal rdisc = 0;
                                                        if (itemdt["LPromotionName"].ToString() != "")
                                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                                                        if (itemdt["SPromotionName"].ToString() != "")
                                                            sdisc = Convert.ToDecimal(itemdt["Discount"]);
                                                        if (itemdt["RPromotionName"].ToString() != "")
                                                            rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - ldisc - sdisc - rdisc - Convert.ToDecimal(itemdt["ODiscount"])).ToString();
                                                    }

                                                    itemdt["OPromotionName"] = itemdt["OPROName"];
                                                    itemdt["UnitRetail"] = price;
                                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);

                                                    itemdt["PromotionName"] = "";
                                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //if (dt.Rows[dCount]["PROName"].ToString() != "")
                    //{
                    //    DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "PROName", "Qty", "NewPrice", "Discount");

                    //    foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                    //    {
                    //        if (distinct["PROName"].ToString() != "")
                    //        {
                    //            int sumCount = (from row in dt.AsEnumerable()
                    //                            where row.Field<string>("PROName") == distinct["PROName"].ToString()
                    //                            select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                    //            foreach (var itemdt in dt.AsEnumerable())
                    //            {
                    //                if (itemdt["PROName"].ToString() == distinct["PROName"].ToString())
                    //                {
                    //                    for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                    //                        if (sumCount == Convert.ToInt32(distinct["Qty"]) * i)
                    //                        {
                    //                            if (itemdt["DiscountBy"].ToString() == "Loyalty")
                    //                            {
                    //                                if (lblLoyaltyId.Content.ToString() != "")
                    //                                {
                    //                                    string price = "";
                    //                                    if (itemdt["NewPrice"].ToString() != "" && itemdt["NewPrice"].ToString() != "0")
                    //                                        price = (Convert.ToDecimal(itemdt["NewPrice"]) / Convert.ToInt32(itemdt["Qty"])).ToString();

                    //                                    if (price == "")
                    //                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - Convert.ToDecimal(itemdt["Discount"])).ToString();

                    //                                    itemdt["PromotionName"] = itemdt["PROName"];
                    //                                    itemdt["UnitRetail"] = price;
                    //                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                    //                                    itemdt["LoyaltyId"] = lblLoyaltyId.Content.ToString();
                    //                                }
                    //                                else
                    //                                {
                    //                                    itemdt["LoyaltyId"] = "";
                    //                                    itemdt["PromotionName"] = "";
                    //                                    itemdt["UnitRetail"] = itemdt["Oprice"];
                    //                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                    //                                }
                    //                            }
                    //                            else
                    //                            {
                    //                                string price = "";
                    //                                if (itemdt["NewPrice"].ToString() != "" && itemdt["NewPrice"].ToString() != "0")
                    //                                    price = (Convert.ToDecimal(itemdt["NewPrice"]) / Convert.ToInt32(itemdt["Qty"])).ToString();

                    //                                if (price == "")
                    //                                    price = (Convert.ToDecimal(itemdt["Oprice"]) - Convert.ToDecimal(itemdt["Discount"])).ToString();

                    //                                itemdt["PromotionName"] = itemdt["PROName"];
                    //                                itemdt["UnitRetail"] = price;
                    //                                itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                    //                            }
                    //                            //string price = "";
                    //                            //if (itemdt["NewPrice"].ToString() != "" && itemdt["NewPrice"].ToString() != "0")
                    //                            //    price = (Convert.ToDecimal(itemdt["NewPrice"]) / Convert.ToInt32(itemdt["Qty"])).ToString();

                    //                            //if (price == "")
                    //                            //    price = (Convert.ToDecimal(itemdt["Oprice"]) - Convert.ToDecimal(itemdt["Discount"])).ToString();

                    //                            //itemdt["PromotionName"] = itemdt["PROName"];
                    //                            //itemdt["UnitRetail"] = price;
                    //                            //itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                    //                        }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    JRDGrid.ItemsSource = dt.DefaultView;
                    JRDGrid.Items.Refresh();
                    JRDGrid.ScrollIntoView(JRDGrid.Items[dCount]);
                    JRDGrid.SelectedIndex = dCount;
                    TotalEvent();
                }

                textBox1.Text = "";
                if (cbCustomer1.SelectedIndex == 0 && JRDGrid.Items.Count == 1)
                {
                    MessageBox.Show("Select Customer for apply loyalty");
                    gCustomer.Visibility = Visibility.Visible;
                    uGHold.Visibility = Visibility.Hidden;
                }

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void TotalEvent()
        {
            try
            {
                //  decimal sum = 0.00m;
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
                SendErrorToText(ex, errorFileName);
            }
        }

        private void TxtCashReceive_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Tab || e.Key == Key.Enter)
                {
                    var i = grandTotal.Content;
                    TxtCashReturn.Text = decimal.Parse(Convert.ToDecimal(decimal.Parse(TxtCashReceive.Text) - decimal.Parse(grandTotal.Content.ToString().Replace("Pay $", ""))).ToString("0.00")).ToString("0.00");
                    Button_Click_1();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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

                string transaction = "insert into Transactions(Tran_id,EndDate,EndTime,GrossAmount,TaxAmount,GrandAmount,CreateBy,CreateOn)Values('" + tranid + "','" + onlydate + "','" + onlytime + "','" + totalAmt + "','" + tax + "','" + grandTotalAmt + "','" + username + "','" + date + "')";
                SqlCommand cmd = new SqlCommand(transaction, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                if (tenderCode == "Cash")
                {
                    string tender = "";
                    if (refund == "")
                        tender = "insert into Tender(EndDate,Endtime,TenderCode,Amount,Change,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + cashRec + "','" + cashReturn + "','" + tranid + "','" + username + "','" + date + "')";
                    else
                        tender = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + username + "','" + date + "')";
                    SqlCommand cmdTender = new SqlCommand(tender, con);
                    con.Open();
                    cmdTender.ExecuteNonQuery();
                    con.Close();
                }
                else if (tenderCode == "Card")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + username + "','" + date + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                else if (tenderCode == "Customer")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,AccountName,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + cbcustomer.Text + "','" + username + "','" + date + "')";
                    SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                    con.Open();
                    cmdTender1.ExecuteNonQuery();
                    con.Close();
                }
                else if (tenderCode == "Check")
                {
                    string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CheckNo,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + TxtCheck.Text + "','" + username + "','" + date + "')";
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
                con.Open();
                objbulk.WriteToServer(dt);
                con.Close();
                //PrintDocument = new PrintDocument();
                //PrintDocument.PrintPage += new PrintPageEventHandler(FormatPage);
                //PrintDocument.Print();

                cbcustomer.Text = "";
                TxtCheck.Text = "";
                txtTotal.Content = "";

                grandTotal.Content = "Pay " + "$" + "0.00";
                taxtTotal.Content = "";
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
                SendErrorToText(ex, errorFileName);
            }
        }

        private void StoreDetails()
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string query = "select * from storedetails";
                SqlCommand cmdstore = new SqlCommand(query, con);
                SqlDataAdapter sdastore = new SqlDataAdapter(cmdstore);
                sdastore.Fill(dtstr);
            }
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
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

                DrawAtStart(dtstr.Rows[0]["StoreAddress"].ToString(), Offset);
                Offset = Offset + largeinc;
                DrawAtStart(dtstr.Rows[0]["PhoneNumber"].ToString(), Offset);

                Offset = Offset + largeinc;

                String underLine = "-------------------------------------";
                DrawLine(underLine, largefont, Offset, 0);

                Offset = Offset + largeinc;
                DrawAtStart("Transaction Id:" + lblTranid.Content, Offset);
                Offset = Offset + largeinc;

                DrawAtStart("Date: " + lblDate.Content, Offset);

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
                SendErrorToText(ex, errorFileName);
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
                        new SolidBrush(Color.Black), startX + 210, startY + Offset);
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
                      new SolidBrush(Color.Black), startX + 210, startY + Offset);
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

                                        int QA = qDT1 * (qDT / qDT1);
                                        if (dt.Rows[rowIndex]["NewPrice"].ToString() != "")
                                        {
                                            dt.Rows[rowIndex]["PromotionName"] = dt.Rows[rowIndex]["PROName"];
                                            dt.Rows[rowIndex]["Quantity"] = QA;
                                            dt.Rows[rowIndex]["UnitRetail"] = Convert.ToDecimal(dt.Rows[rowIndex]["NewPrice"]) / qDT1;
                                            dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                                        }
                                        else
                                        {
                                            dt.Rows[rowIndex]["PromotionName"] = dt.Rows[rowIndex]["PROName"];
                                            dt.Rows[rowIndex]["Quantity"] = QA;
                                            dt.Rows[rowIndex]["UnitRetail"] = (Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) - Convert.ToDecimal(dt.Rows[rowIndex]["Discount"]));
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
                                                newRow["Quantity"] = 1;
                                                newRow["UnitRetail"] = dt.Rows[rowIndex]["OPrice"];
                                                newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                                                newRow["OPrice"] = dt.Rows[rowIndex]["OPrice"];
                                                newRow["PromotionName"] = "";
                                                newRow["TaxRate"] = dt.Rows[rowIndex]["TaxRate"];
                                                newRow["PROName"] = dt.Rows[rowIndex]["PROName"];
                                                newRow["Qty"] = dt.Rows[rowIndex]["Qty"];
                                                newRow["NewPrice"] = dt.Rows[rowIndex]["NewPrice"];
                                                newRow["Discount"] = dt.Rows[rowIndex]["Discount"];
                                                newRow["DiscountBy"] = dt.Rows[rowIndex]["DiscountBy"];
                                                dt.Rows.Add(newRow);
                                            }
                                        }
                                    }

                                    int intv = qDT1 * (qDT / qDT1);
                                    decimal ab = qDT / qDT1;
                                    decimal decv = Convert.ToDecimal(qDT1) * Convert.ToDecimal(qDT) / Convert.ToDecimal(qDT1);

                                    ScanCodeFunction();
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
            }
        }

        private void SendErrorToText(Exception ex, string errorFileName)
        {
            var line = Environment.NewLine + Environment.NewLine;
            ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();
            MessageBox.Show("line -" + line + "</br> FileName -" + errorFileName, "Error " + ex);
            ErrorLocation = ex.Message.ToString();
            try
            {
                string filepath = System.AppDomain.CurrentDomain.BaseDirectory;
                string errorpath = filepath + "\\ErrorFiles\\";

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                Report rpt = new Report();
                rpt.Show();
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
                SendErrorToText(ex, errorFileName);
            }

        }

        //Shift close
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                //string queryHold = "select distinct TrasactionId from Hold";
                //SqlCommand cmdHold = new SqlCommand(queryHold, con);
                //SqlDataAdapter sdaHold = new SqlDataAdapter(cmdHold);
                //sdaHold.Fill(dtHold);

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

        private void ShiftClose(object sender, PrintPageEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string queryTrans = "select Count(tran_id)as Counts,sum(Convert(decimal(10,2),GrossAmount))as Sales,sum(Convert(decimal(10,2),TaxAmount))as Tax,sum(Convert(decimal(10,2),grandAmount))as Total,min(convert(datetime,createon))as SDate,Max(convert(datetime,createon))as EDate from transactions where ShiftClose is null and (void !=1 or void is Null)";
                SqlCommand cmdTrans = new SqlCommand(queryTrans, con);
                SqlDataAdapter sdaTrans = new SqlDataAdapter(cmdTrans);
                DataTable dtTrans = new DataTable();
                sdaTrans.Fill(dtTrans);

                string queryDept = "select Department,Sum(Convert(decimal(10,2),amt)) as amt from(select Department, Sum(Convert(decimal(10,2),Amount)) as amt from salesitem inner join item on salesitem.scancode = item.scancode where ShiftClose is null and(void != 1 or void is Null) group by Department Union all select Department,Sum(Convert(decimal(10,2),Amount)) as amt from salesitem inner join Department on salesitem.Descripation = Department.Department where ShiftClose is null and(void != 1 or void is Null) group by Department)as x group by Department";
                SqlCommand cmdDept = new SqlCommand(queryDept, con);
                SqlDataAdapter sdaDept = new SqlDataAdapter(cmdDept);
                DataTable dtDept = new DataTable();
                sdaDept.Fill(dtDept);

                string queryTender = "select tendercode,sum(Convert(decimal(10,2),amount)-coalesce(Convert(decimal(10,2),change),0))as amt from tender where ShiftClose is null group by tendercode";
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

                DrawAtStart("            " + dtstr.Rows[0]["StoreAddress"].ToString(), Offset);
                Offset = Offset + mediuminc;
                DrawAtStart("            " + dtstr.Rows[0]["PhoneNumber"].ToString(), Offset);

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

        //page close
        private void Button_Click_6(object sender, RoutedEventArgs e)
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                        ScanCodeFunction();
                        //DataTable distrinctPromotionName = dt.DefaultView.ToTable(true, "PROName");
                        //DataTable distrinctSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "PROName", "Qty", "NewPrice", "Discount");
                        //foreach (DataRow distrinctRow in distrinctPromotionName.AsEnumerable())
                        //{
                        //    if (distrinctRow["PROName"].ToString() != "")
                        //    {
                        //        int sumCount = 0;
                        //        for (int j = 0; j < distrinctSCANCODE.AsEnumerable().Count(); j++)
                        //        {
                        //            if (distrinctSCANCODE.Rows[j]["PROName"].ToString() != "")
                        //            {
                        //                for (int i = 0; i < dt.Rows.Count; i++)
                        //                {
                        //                    if (distrinctSCANCODE.Rows[j]["PROName"].ToString() == distrinctRow["PROName"].ToString())
                        //                    {
                        //                        if (distrinctSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                        //                        {
                        //                            sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                        //                            for (int K = 0; K < dt.Rows.Count; K++)
                        //                            {
                        //                                foreach (DataRow itemDT1 in distrinctSCANCODE.AsEnumerable())
                        //                                {
                        //                                    if (itemDT1["PROName"].ToString() == distrinctRow["PROName"].ToString())
                        //                                    {
                        //                                        int Y = sumCount / Convert.ToInt32(itemDT1["Qty"]);
                        //                                        for (int x = 1; x <= Y; x++)
                        //                                        {
                        //                                            if (sumCount == Convert.ToInt32(itemDT1["Qty"]) * x)
                        //                                            {
                        //                                                for (int z = 0; z <= i; z++)
                        //                                                {
                        //                                                    string dtProName = dt.Rows[z]["PROName"].ToString();
                        //                                                    string disProName = distrinctRow["PROName"].ToString();
                        //                                                    if (dtProName == disProName)
                        //                                                    {
                        //                                                        string price = "";
                        //                                                        if (itemDT1["NewPrice"].ToString() != "")
                        //                                                            price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                        //                                                        if (price == "")
                        //                                                            price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["Discount"])).ToString();

                        //                                                        dt.Rows[z]["PromotionName"] = itemDT1["PROName"];
                        //                                                        dt.Rows[z]["UnitRetail"] = price;
                        //                                                        dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
                        //                                                    }
                        //                                                }

                        //                                            }

                        //                                        }
                        //                                    }
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                }

                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
                JRDGrid.ItemsSource = dt.DefaultView;
                TotalEvent();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
            }
        }

        public void ScanCodeFunction()
        {
            try
            {
                DataTable distrinctPromotionName = dt.DefaultView.ToTable(true, "PROName");
                DataTable distrinctSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "PROName", "Qty", "NewPrice", "Discount");
                DataTable distrinctRPromotionName = dt.DefaultView.ToTable(true, "RPROName");
                DataTable distrinctRSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "RPROName", "RQty", "RNewPrice", "RDiscount");
                DataTable distrinctLPromotionName = dt.DefaultView.ToTable(true, "LPROName");
                DataTable distrinctLSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "LPROName", "LQty", "LNewPrice", "LDiscount");
                DataTable distrinctOPromotionName = dt.DefaultView.ToTable(true, "OPROName");
                DataTable distrinctOSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "OPROName", "OQty", "ONewPrice", "ODiscount");

                foreach (DataRow distrinctRow in distrinctPromotionName.AsEnumerable())
                {
                    if (distrinctRow["PROName"].ToString() != "")
                    {
                        int sumCount = 0;
                        for (int j = 0; j < distrinctSCANCODE.AsEnumerable().Count(); j++)
                        {
                            if (distrinctSCANCODE.Rows[j]["PROName"].ToString() == distrinctRow["PROName"].ToString())
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (distrinctSCANCODE.Rows[j]["PROName"].ToString() == dt.Rows[i]["PROName"].ToString())
                                    {
                                        //if (distrinctOSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                                        //{
                                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                                        //for (int K = 0; K < dt.Rows.Count; K++)
                                        //{
                                        if (sumCount < 0)
                                            sumCount = sumCount * -1;
                                        foreach (DataRow itemDT1 in distrinctSCANCODE.AsEnumerable())
                                        {
                                            if (itemDT1["PROName"].ToString() == distrinctRow["PROName"].ToString())
                                            {
                                                int Y = sumCount / Convert.ToInt32(itemDT1["Qty"]);
                                                for (int x = 1; x <= Y; x++)
                                                {
                                                    if (sumCount == Convert.ToInt32(itemDT1["Qty"]) * x)
                                                    {
                                                        for (int z = 0; z <= i; z++)
                                                        {
                                                            if (dt.Rows[z]["PROName"].ToString() == distrinctRow["PROName"].ToString())
                                                            {
                                                                string price = "";
                                                                if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                                                                    price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                                                                if (price == "")
                                                                {
                                                                    decimal ldisc = 0;
                                                                    decimal odisc = 0;
                                                                    decimal rdisc = 0;
                                                                    if (dt.Rows[z]["LPromotionName"].ToString() != "")
                                                                        ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
                                                                    if (dt.Rows[z]["OPromotionName"].ToString() != "")
                                                                        odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
                                                                    if (dt.Rows[z]["RPromotionName"].ToString() != "")
                                                                        rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
                                                                    price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - odisc - rdisc - Convert.ToDecimal(dt.Rows[z]["Discount"])).ToString();
                                                                }
                                                                dt.Rows[z]["SPromotionName"] = dt.Rows[z]["PROName"];
                                                                dt.Rows[z]["UnitRetail"] = price;
                                                                dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);


                                                                dt.Rows[z]["PromotionName"] = "";
                                                                dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
                                                            }
                                                        }

                                                    }

                                                }
                                            }
                                        }
                                        //    }
                                        //}
                                    }
                                }

                            }
                        }
                    }
                }
                foreach (DataRow distrinctRow in distrinctRPromotionName.AsEnumerable())
                {
                    if (distrinctRow["RPROName"].ToString() != "")
                    {
                        int sumCount = 0;
                        for (int j = 0; j < distrinctRSCANCODE.AsEnumerable().Count(); j++)
                        {
                            if (distrinctRSCANCODE.Rows[j]["RPROName"].ToString() == distrinctRow["RPROName"].ToString())
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (distrinctRSCANCODE.Rows[j]["RPROName"].ToString() == dt.Rows[i]["RPROName"].ToString())
                                    {
                                        //if (distrinctOSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                                        //{
                                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                                        //for (int K = 0; K < dt.Rows.Count; K++)
                                        //{
                                        if (sumCount < 0)
                                            sumCount = sumCount * -1;
                                        foreach (DataRow itemDT1 in distrinctRSCANCODE.AsEnumerable())
                                        {
                                            if (itemDT1["RPROName"].ToString() == distrinctRow["RPROName"].ToString())
                                            {
                                                int Y = sumCount / Convert.ToInt32(itemDT1["RQty"]);
                                                for (int x = 1; x <= Y; x++)
                                                {
                                                    if (sumCount == Convert.ToInt32(itemDT1["RQty"]) * x)
                                                    {
                                                        for (int z = 0; z <= i; z++)
                                                        {
                                                            if (dt.Rows[z]["RPROName"].ToString() == distrinctRow["RPROName"].ToString())
                                                            {
                                                                string price = "";
                                                                if (itemDT1["RNewPrice"].ToString() != "" && itemDT1["RNewPrice"].ToString() != "0")
                                                                    price = (Convert.ToDecimal(itemDT1["RNewPrice"]) / Convert.ToInt32(itemDT1["RQty"])).ToString();

                                                                if (price == "")
                                                                {
                                                                    decimal ldisc = 0;
                                                                    decimal odisc = 0;
                                                                    decimal sdisc = 0;
                                                                    if (dt.Rows[z]["LPromotionName"].ToString() != "")
                                                                        ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
                                                                    if (dt.Rows[z]["OPromotionName"].ToString() != "")
                                                                        odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
                                                                    if (dt.Rows[z]["SPromotionName"].ToString() != "")
                                                                        sdisc = Convert.ToDecimal(dt.Rows[z]["Discount"]);
                                                                    price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - odisc - sdisc - Convert.ToDecimal(dt.Rows[z]["RDiscount"])).ToString();
                                                                }
                                                                dt.Rows[z]["RPromotionName"] = dt.Rows[z]["RPROName"];
                                                                dt.Rows[z]["UnitRetail"] = price;
                                                                dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);


                                                                dt.Rows[z]["PromotionName"] = "";
                                                                dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
                                                            }
                                                        }

                                                    }

                                                }
                                            }
                                        }
                                        //    }
                                        //}
                                    }
                                }

                            }
                        }
                    }
                }
                foreach (DataRow distrinctRow in distrinctLPromotionName.AsEnumerable())
                {
                    if (lblLoyaltyId.Content is null)
                        lblLoyaltyId.Content = "";
                    if (distrinctRow["LPROName"].ToString() != "" && lblLoyaltyId.Content.ToString() != "")
                    {
                        int sumCount = 0;
                        for (int j = 0; j < distrinctLSCANCODE.AsEnumerable().Count(); j++)
                        {
                            if (distrinctLSCANCODE.Rows[j]["LPROName"].ToString() != "")
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (distrinctLSCANCODE.Rows[j]["LPROName"].ToString() == distrinctRow["LPROName"].ToString())
                                    {
                                        //if (distrinctLSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                                        //{
                                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                                        //for (int K = 0; K < dt.Rows.Count; K++)
                                        //{
                                        if (sumCount < 0)
                                            sumCount = sumCount * -1;
                                        foreach (DataRow itemDT1 in distrinctLSCANCODE.AsEnumerable())
                                        {
                                            if (itemDT1["LPROName"].ToString() == distrinctRow["LPROName"].ToString())
                                            {
                                                int Y = sumCount / Convert.ToInt32(itemDT1["LQty"]);
                                                for (int x = 1; x <= Y; x++)
                                                {
                                                    if (sumCount == Convert.ToInt32(itemDT1["LQty"]) * x)
                                                    {
                                                        for (int z = 0; z <= i; z++)
                                                        {
                                                            if (dt.Rows[z]["LPROName"].ToString() == distrinctRow["LPROName"].ToString())
                                                            {
                                                                string price = "";
                                                                if (itemDT1["LNewPrice"].ToString() != "")
                                                                    price = (Convert.ToDecimal(itemDT1["LNewPrice"]) / Convert.ToInt32(itemDT1["LQty"])).ToString();

                                                                if (price == "")
                                                                {
                                                                    decimal odisc = 0;
                                                                    decimal sdisc = 0;
                                                                    decimal rdisc = 0;
                                                                    if (dt.Rows[z]["OPromotionName"].ToString() != "")
                                                                        odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
                                                                    if (dt.Rows[z]["SPromotionName"].ToString() != "")
                                                                        sdisc = Convert.ToDecimal(dt.Rows[z]["SDiscount"]);
                                                                    if (dt.Rows[z]["RPromotionName"].ToString() != "")
                                                                        rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
                                                                    price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - odisc - sdisc - rdisc - Convert.ToDecimal(dt.Rows[z]["LDiscount"])).ToString();
                                                                    //price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["LDiscount"])).ToString();
                                                                }
                                                                dt.Rows[z]["LPromotionName"] = dt.Rows[z]["LPROName"];
                                                                dt.Rows[z]["UnitRetail"] = price;
                                                                dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
                                                                dt.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();

                                                                dt.Rows[z]["PromotionName"] = "";
                                                                dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();

                                                                //dt.Rows[z]["LPromotionName"] = itemDT1["LPROName"];
                                                                //dt.Rows[z]["UnitRetail"] = price;
                                                                //dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
                                                            }
                                                        }

                                                    }

                                                }
                                            }
                                        }
                                        //}
                                        //}
                                    }
                                }

                            }
                        }
                    }
                }
                foreach (DataRow distrinctRow in distrinctOPromotionName.AsEnumerable())
                {
                    if (distrinctRow["OPROName"].ToString() != "")
                    {
                        int sumCount = 0;
                        for (int j = 0; j < distrinctOSCANCODE.AsEnumerable().Count(); j++)
                        {
                            if (distrinctOSCANCODE.Rows[j]["OPROName"].ToString() == distrinctRow["OPROName"].ToString())
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (distrinctOSCANCODE.Rows[j]["OPROName"].ToString() == dt.Rows[i]["OPROName"].ToString())
                                    {
                                        //if (distrinctOSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                                        //{
                                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                                        //for (int K = 0; K < dt.Rows.Count; K++)
                                        //{
                                        if (sumCount < 0)
                                            sumCount = sumCount * -1;
                                        foreach (DataRow itemDT1 in distrinctOSCANCODE.AsEnumerable())
                                        {
                                            if (itemDT1["OPROName"].ToString() == distrinctRow["OPROName"].ToString())
                                            {
                                                int Y = sumCount / Convert.ToInt32(itemDT1["OQty"]);
                                                for (int x = 1; x <= Y; x++)
                                                {
                                                    if (sumCount == Convert.ToInt32(itemDT1["OQty"]) * x)
                                                    {
                                                        for (int z = 0; z <= i; z++)
                                                        {
                                                            if (dt.Rows[z]["OPROName"].ToString() == distrinctRow["OPROName"].ToString())
                                                            {
                                                                string price = "";
                                                                if (itemDT1["ONewPrice"].ToString() != "" && itemDT1["ONewPrice"].ToString() != "0")
                                                                    price = (Convert.ToDecimal(itemDT1["ONewPrice"]) / Convert.ToInt32(itemDT1["OQty"])).ToString();

                                                                if (price == "")
                                                                {
                                                                    decimal ldisc = 0;
                                                                    decimal sdisc = 0;
                                                                    decimal rdisc = 0;
                                                                    if (dt.Rows[z]["LPromotionName"].ToString() != "")
                                                                        ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
                                                                    if (dt.Rows[z]["SPromotionName"].ToString() != "")
                                                                        sdisc = Convert.ToDecimal(dt.Rows[z]["SDiscount"]);
                                                                    if (dt.Rows[z]["RPromotionName"].ToString() != "")
                                                                        rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
                                                                    price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - sdisc - rdisc - Convert.ToDecimal(dt.Rows[z]["ODiscount"])).ToString();
                                                                }
                                                                dt.Rows[z]["OPromotionName"] = dt.Rows[z]["OPROName"];
                                                                dt.Rows[z]["UnitRetail"] = price;
                                                                dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
                                                                dt.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();

                                                                dt.Rows[z]["PromotionName"] = "";
                                                                dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
                                                            }
                                                        }

                                                    }

                                                }
                                            }
                                        }
                                        //    }
                                        //}
                                    }
                                }

                            }
                        }
                    }
                }
                TotalEvent();

                //datatable = datatable.DefaultView.ToTable();
                //SqlConnection con = new SqlConnection(conString);

                //for (int i = 0; i < datatable.AsEnumerable().Count(); i++)
                //{
                //    if (Convert.ToInt32(datatable.Rows[i]["Quantity"]) == 1)
                //    {
                //        datatable.Rows[i]["PromotionName"] = "";
                //        datatable.Rows[i]["UnitRetail"] = datatable.Rows[i]["OPrice"];
                //        datatable.Rows[i]["Amount"] = Convert.ToDecimal(datatable.Rows[i]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[i]["Quantity"]);
                //    }
                //}
                //DataTable distrinctPromotionName = dt.DefaultView.ToTable(true, "PROName");
                //DataTable distrinctSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "PROName", "Qty", "NewPrice", "Discount", "DiscountBy");

                //foreach (DataRow distrinctRow in distrinctPromotionName.AsEnumerable())
                //{
                //    if (distrinctRow["PROName"].ToString() != "")
                //    {
                //        int sumCount = 0;
                //        for (int j = 0; j < distrinctSCANCODE.AsEnumerable().Count(); j++)
                //        {
                //            if (distrinctSCANCODE.Rows[j]["PROName"].ToString() != "")
                //            {
                //                for (int i = 0; i < datatable.Rows.Count; i++)
                //                {
                //                    if (distrinctSCANCODE.Rows[j]["PROName"].ToString() == distrinctRow["PROName"].ToString())
                //                    {
                //                        if (distrinctSCANCODE.Rows[j]["ScanCode"].ToString() == datatable.Rows[i]["ScanCode"].ToString())
                //                        {
                //                            sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(datatable.Rows[i]["Quantity"]);
                //                            for (int K = 0; K < datatable.Rows.Count; K++)
                //                            {
                //                                foreach (DataRow itemDT1 in distrinctSCANCODE.AsEnumerable())
                //                                {
                //                                    if (itemDT1["PROName"].ToString() == distrinctRow["PROName"].ToString())
                //                                    {
                //                                        int Y = sumCount / Convert.ToInt32(itemDT1["Qty"]);
                //                                        for (int x = 1; x <= Y; x++)
                //                                        {
                //                                            if (sumCount == Convert.ToInt32(itemDT1["Qty"]) * x)
                //                                            {
                //                                                for (int z = 0; z <= i; z++)
                //                                                {
                //                                                    string dtProName = datatable.Rows[z]["PROName"].ToString();
                //                                                    string disProName = distrinctRow["PROName"].ToString();
                //                                                    if (dtProName == disProName)
                //                                                    {
                //                                                        if (itemDT1["DiscountBy"].ToString() == "Loyalty")
                //                                                        {
                //                                                            if (lblLoyaltyId.Content.ToString() != "")
                //                                                            {
                //                                                                string price = "";
                //                                                                if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                //                                                                    price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                //                                                                if (price == "")
                //                                                                    price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["Discount"])).ToString();

                //                                                                datatable.Rows[z]["PromotionName"] = itemDT1["PROName"];
                //                                                                datatable.Rows[z]["UnitRetail"] = price;
                //                                                                datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                                datatable.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();
                //                                                            }
                //                                                            else
                //                                                            {
                //                                                                datatable.Rows[z]["LoyaltyId"] = "";
                //                                                                datatable.Rows[z]["PromotionName"] = "";
                //                                                                datatable.Rows[z]["UnitRetail"] = datatable.Rows[z]["Oprice"];
                //                                                                datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                            }
                //                                                        }
                //                                                        else
                //                                                        {
                //                                                            string price = "";
                //                                                            if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                //                                                                price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                //                                                            if (price == "")
                //                                                                price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["Discount"])).ToString();

                //                                                            datatable.Rows[z]["PromotionName"] = itemDT1["PROName"];
                //                                                            datatable.Rows[z]["UnitRetail"] = price;
                //                                                            datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                        }

                //                                                        //string price = "";
                //                                                        //if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                //                                                        //    price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                //                                                        //if (price == "")
                //                                                        //    price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) * Convert.ToDecimal(itemDT1["Discount"]) / 100)).ToString();

                //                                                        //datatable.Rows[z]["PromotionName"] = itemDT1["PROName"];
                //                                                        //datatable.Rows[z]["UnitRetail"] = price;
                //                                                        //datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                    }
                //                                                }

                //                                            }

                //                                            else if (sumCount > Convert.ToInt32(itemDT1["Qty"]) * x)
                //                                            {
                //                                                int q1 = sumCount - Convert.ToInt32(datatable.Rows[i]["Quantity"]);
                //                                                int oldqty = Convert.ToInt32(datatable.Rows[i]["Quantity"]);
                //                                                int q2 = Convert.ToInt32(itemDT1["Qty"]) - q1;
                //                                                int finalqty = oldqty - q2;
                //                                                if (datatable.Rows.Count == rowindex + 1)
                //                                                {
                //                                                    for (int z = 0; z <= i; z++)
                //                                                    {
                //                                                        string dtProName = datatable.Rows[z]["PROName"].ToString();
                //                                                        string disProName = distrinctRow["PROName"].ToString();
                //                                                        if (dtProName == disProName)
                //                                                        {
                //                                                            if (z == i)
                //                                                            {
                //                                                                if (q2 > 0)
                //                                                                {
                //                                                                    if (itemDT1["DiscountBy"].ToString() == "Loyalty")
                //                                                                    {
                //                                                                        if (lblLoyaltyId.Content.ToString() != "")
                //                                                                        {
                //                                                                            string price = "";
                //                                                                            if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                //                                                                                price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                //                                                                            if (price == "")
                //                                                                                price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["Discount"])).ToString();

                //                                                                            datatable.Rows[z]["PromotionName"] = itemDT1["PROName"];
                //                                                                            datatable.Rows[z]["UnitRetail"] = price;
                //                                                                            datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                                            datatable.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();
                //                                                                        }
                //                                                                        else
                //                                                                        {
                //                                                                            datatable.Rows[z]["LoyaltyId"] = "";
                //                                                                            datatable.Rows[z]["PromotionName"] = "";
                //                                                                            datatable.Rows[z]["UnitRetail"] = datatable.Rows[z]["Oprice"];
                //                                                                            datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                                        }
                //                                                                    }
                //                                                                    else
                //                                                                    {
                //                                                                        string price = "";
                //                                                                        if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                //                                                                            price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                //                                                                        if (price == "")
                //                                                                            price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["Discount"])).ToString();

                //                                                                        datatable.Rows[z]["PromotionName"] = itemDT1["PROName"];
                //                                                                        datatable.Rows[z]["UnitRetail"] = price;
                //                                                                        datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                                    }
                //                                                                    //string price = "";
                //                                                                    //if (itemDT1["NewPrice"].ToString() != "")
                //                                                                    //    price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                //                                                                    //if (price == "")
                //                                                                    //    price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) * Convert.ToDecimal(itemDT1["Discount"]) / 100)).ToString();

                //                                                                    //datatable.Rows[z]["Quantity"] = q2;
                //                                                                    //datatable.Rows[z]["PromotionName"] = itemDT1["PROName"];
                //                                                                    //datatable.Rows[z]["UnitRetail"] = price;
                //                                                                    //datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);

                //                                                                    for (int o = 1; o <= (oldqty - q2); o++)
                //                                                                    {
                //                                                                        DataRow newRow = datatable.NewRow();
                //                                                                        newRow["ScanCode"] = datatable.Rows[z]["ScanCode"];
                //                                                                        newRow["Description"] = datatable.Rows[z]["Description"];
                //                                                                        newRow["Quantity"] = 1;
                //                                                                        newRow["UnitRetail"] = datatable.Rows[z]["OPrice"];
                //                                                                        newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                //                                                                        newRow["OPrice"] = datatable.Rows[z]["OPrice"];
                //                                                                        newRow["TaxRate"] = datatable.Rows[z]["TaxRate"];
                //                                                                        newRow["PromotionName"] = "";
                //                                                                        newRow["PROName"] = dt.Rows[z]["PROName"];
                //                                                                        newRow["Qty"] = dt.Rows[z]["Qty"];
                //                                                                        newRow["NewPrice"] = dt.Rows[z]["NewPrice"];
                //                                                                        newRow["Discount"] = dt.Rows[z]["Discount"];
                //                                                                        newRow["DiscountBy"] = dt.Rows[z]["DiscountBy"];
                //                                                                        datatable.Rows.Add(newRow);
                //                                                                    }
                //                                                                }
                //                                                                else if (Convert.ToInt32(datatable.Rows[z]["Quantity"]) < Convert.ToInt32(datatable.Rows[z]["Qty"]))
                //                                                                {
                //                                                                    datatable.Rows[z]["UnitRetail"] = datatable.Rows[z]["OPrice"];
                //                                                                    datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                                }

                //                                                            }
                //                                                            else
                //                                                            {
                //                                                                if (itemDT1["DiscountBy"].ToString() == "Loyalty")
                //                                                                {
                //                                                                    if (lblLoyaltyId.Content.ToString() != "")
                //                                                                    {
                //                                                                        string price = "";
                //                                                                        if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                //                                                                            price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                //                                                                        if (price == "")
                //                                                                            price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["Discount"])).ToString();

                //                                                                        datatable.Rows[z]["PromotionName"] = itemDT1["PROName"];
                //                                                                        datatable.Rows[z]["UnitRetail"] = price;
                //                                                                        datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                                        datatable.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();
                //                                                                    }
                //                                                                    else
                //                                                                    {
                //                                                                        datatable.Rows[z]["LoyaltyId"] = "";
                //                                                                        datatable.Rows[z]["PromotionName"] = "";
                //                                                                        datatable.Rows[z]["UnitRetail"] = datatable.Rows[z]["Oprice"];
                //                                                                        datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                                    }
                //                                                                }
                //                                                                else
                //                                                                {
                //                                                                    string price = "";
                //                                                                    if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                //                                                                        price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                //                                                                    if (price == "")
                //                                                                        price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["Discount"])).ToString();

                //                                                                    datatable.Rows[z]["PromotionName"] = itemDT1["PROName"];
                //                                                                    datatable.Rows[z]["UnitRetail"] = price;
                //                                                                    datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                                }

                //                                                                //string price = "";
                //                                                                //if (itemDT1["NewPrice"].ToString() != "")
                //                                                                //    price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

                //                                                                //if (price == "")
                //                                                                //    price = (Convert.ToDecimal(datatable.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["Discount"])).ToString();

                //                                                                //if (Convert.ToInt32(datatable.Rows[z]["Quantity"]) >= Convert.ToInt32(datatable.Rows[z]["Qty"]))
                //                                                                //{
                //                                                                //    datatable.Rows[z]["PromotionName"] = itemDT1["PROName"];
                //                                                                //    datatable.Rows[z]["UnitRetail"] = price;
                //                                                                //    datatable.Rows[z]["Amount"] = Convert.ToDecimal(datatable.Rows[z]["UnitRetail"]) * Convert.ToDecimal(datatable.Rows[z]["Quantity"]);
                //                                                                //}
                //                                                            }
                //                                                        }
                //                                                    }
                //                                                }
                //                                            }
                //                                        }
                //                                    }
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                    //dt.Rows[i]["Quantity"] = Convert.ToDecimal(dt.Rows[i]["Quantity"]) + 1;
                    if (dt.Rows[i]["PROName"].ToString() != "" || dt.Rows[i]["RPROName"].ToString() != "" || dt.Rows[i]["LPROName"].ToString() != "" || dt.Rows[i]["OPROName"].ToString() != "")
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
                            newRow["PROName"] = dt.Rows[i]["PROName"].ToString();
                            newRow["Qty"] = dt.Rows[i]["Qty"].ToString();
                            newRow["NewPrice"] = dt.Rows[i]["NewPrice"].ToString();
                            newRow["Discount"] = dt.Rows[i]["Discount"].ToString();
                            newRow["RPROName"] = dt.Rows[i]["RPROName"].ToString();
                            newRow["RQty"] = dt.Rows[i]["RQty"].ToString();
                            newRow["RNewPrice"] = dt.Rows[i]["RNewPrice"].ToString();
                            newRow["RDiscount"] = dt.Rows[i]["RDiscount"].ToString();
                            newRow["LPROName"] = dt.Rows[i]["LPROName"].ToString();
                            newRow["LQty"] = dt.Rows[i]["LQty"].ToString();
                            newRow["LNewPrice"] = dt.Rows[i]["LNewPrice"].ToString();
                            newRow["LDiscount"] = dt.Rows[i]["LDiscount"].ToString();
                            newRow["OPROName"] = dt.Rows[i]["OPROName"].ToString();
                            newRow["OQty"] = dt.Rows[i]["OQty"].ToString();
                            newRow["ONewPrice"] = dt.Rows[i]["ONewPrice"].ToString();
                            newRow["ODiscount"] = dt.Rows[i]["ODiscount"].ToString();
                            dt.Rows.Add(newRow);
                        }
                        ScanCodeFunction();
                        //int qDT = Convert.ToInt32(dt.Rows[i]["Quantity"]);
                        //int qDT1 = Convert.ToInt32(dt.Rows[i]["Qty"]);

                        //if (qDT >= qDT1)
                        //{

                        //    int QA = qDT1 * (qDT / qDT1);
                        //    if (dt.Rows[i]["NewPrice"].ToString() != "" && dt.Rows[i]["NewPrice"].ToString() != "0")
                        //    {
                        //        dt.Rows[i]["PromotionName"] = dt.Rows[i]["PROName"];
                        //        dt.Rows[i]["Quantity"] = QA;
                        //        dt.Rows[i]["UnitRetail"] = Convert.ToDecimal(dt.Rows[i]["NewPrice"]) / qDT1;
                        //        dt.Rows[i]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"])).ToString("0.00");
                        //    }
                        //    else
                        //    {
                        //        dt.Rows[i]["PromotionName"] = dt.Rows[i]["PROName"];
                        //        dt.Rows[i]["Quantity"] = QA;
                        //        dt.Rows[i]["UnitRetail"] = Convert.ToDecimal(dt.Rows[i]["OPrice"]) - Convert.ToDecimal(dt.Rows[i]["Discount"]);
                        //        dt.Rows[i]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"])).ToString("0.00");
                        //    }
                        //    int QB = qDT - QA;
                        //    if (QB != 0)
                        //    {
                        //        for (int a = 0; a < QB; a++)
                        //        {
                        //            DataRow newRow = dt.NewRow();
                        //            newRow["ScanCode"] = dt.Rows[i]["ScanCode"];
                        //            newRow["Description"] = dt.Rows[i]["Description"];
                        //            newRow["Quantity"] = 1;
                        //            newRow["UnitRetail"] = dt.Rows[i]["OPrice"];
                        //            newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                        //            newRow["OPrice"] = dt.Rows[i]["OPrice"];
                        //            newRow["PromotionName"] = "";
                        //            newRow["TaxRate"] = dt.Rows[i]["TaxRate"];
                        //            newRow["PROName"] = dt.Rows[i]["PROName"];
                        //            newRow["Qty"] = dt.Rows[i]["Qty"];
                        //            newRow["NewPrice"] = dt.Rows[i]["NewPrice"];
                        //            newRow["Discount"] = dt.Rows[i]["Discount"];
                        //            newRow["DiscountBy"] = dt.Rows[i]["DiscountBy"];
                        //            dt.Rows.Add(newRow);
                        //        }
                        //    }
                        //}

                        //int intv = qDT1 * (qDT / qDT1);
                        //decimal ab = qDT / qDT1;
                        //decimal decv = Convert.ToDecimal(qDT1) * Convert.ToDecimal(qDT) / Convert.ToDecimal(qDT1);

                        //dt = ScanCodeFunction(dt, i);
                        JRDGrid.ItemsSource = dt.DefaultView;
                        JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                        JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                    }
                    else
                    {
                        dt.Rows[i]["Quantity"] = Convert.ToDecimal(dt.Rows[i]["Quantity"]) + 1;
                        dt.Rows[i]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"])).ToString("0.00");
                        JRDGrid.ItemsSource = dt.DefaultView;
                    }
                    TotalEvent();
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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
                        if (dt.Rows[i]["PROName"].ToString() != "")
                        {
                            int qDT = Convert.ToInt32(dt.Rows[i]["Quantity"]);
                            int qDT1 = Convert.ToInt32(dt.Rows[i]["Qty"]);

                            if (qDT >= qDT1)
                            {

                                int QA = qDT1 * (qDT / qDT1);
                                if (dt.Rows[i]["NewPrice"].ToString() != "" && dt.Rows[i]["NewPrice"].ToString() != "0")
                                {
                                    dt.Rows[i]["PromotionName"] = dt.Rows[i]["PROName"];
                                    dt.Rows[i]["Quantity"] = QA;
                                    dt.Rows[i]["UnitRetail"] = Convert.ToDecimal(dt.Rows[i]["NewPrice"]) / qDT1;
                                    dt.Rows[i]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"])).ToString("0.00");
                                }
                                else
                                {
                                    dt.Rows[i]["PromotionName"] = dt.Rows[i]["PROName"];
                                    dt.Rows[i]["Quantity"] = QA;
                                    dt.Rows[i]["UnitRetail"] = Convert.ToDecimal(dt.Rows[i]["OPrice"]) - Convert.ToDecimal(dt.Rows[i]["Discount"]);
                                    dt.Rows[i]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"])).ToString("0.00");
                                }
                                int QB = qDT - QA;
                                if (QB != 0)
                                {
                                    for (int a = 0; a < QB; a++)
                                    {
                                        DataRow newRow = dt.NewRow();
                                        newRow["ScanCode"] = dt.Rows[i]["ScanCode"];
                                        newRow["Description"] = dt.Rows[i]["Description"];
                                        newRow["Quantity"] = 1;
                                        newRow["UnitRetail"] = dt.Rows[i]["OPrice"];
                                        newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                                        newRow["OPrice"] = dt.Rows[i]["OPrice"];
                                        newRow["PromotionName"] = "";
                                        newRow["TaxRate"] = dt.Rows[i]["TaxRate"];
                                        newRow["PROName"] = dt.Rows[i]["PROName"];
                                        newRow["Qty"] = dt.Rows[i]["Qty"];
                                        newRow["NewPrice"] = dt.Rows[i]["NewPrice"];
                                        newRow["Discount"] = dt.Rows[i]["Discount"];
                                        newRow["DiscountBy"] = dt.Rows[i]["DiscountBy"];
                                        dt.Rows.Add(newRow);
                                    }
                                }
                            }

                            int intv = qDT1 * (qDT / qDT1);
                            decimal ab = qDT / qDT1;
                            decimal decv = Convert.ToDecimal(qDT1) * Convert.ToDecimal(qDT) / Convert.ToDecimal(qDT1);

                            ScanCodeFunction();

                        }
                        else
                        {
                            dt.Rows[i]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"])).ToString("0.00");
                        }
                        JRDGrid.ItemsSource = dt.DefaultView;
                        TotalEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Click_VoidTransaction(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt.Rows.Count != 0)
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

                    string transaction = "insert into Transactions(Tran_id,EndDate,EndTime,GrossAmount,TaxAmount,GrandAmount,CreateBy,CreateOn,Void)Values('" + tranid + "','" + onlydate + "','" + onlytime + "','" + totalAmt + "','" + tax + "','" + grandTotalAmt + "','" + username + "','" + date + "','1')";
                    SqlCommand cmd = new SqlCommand(transaction, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    if (tenderCode == "Cash")
                    {
                        string tender = "insert into Tender(EndDate,Endtime,TenderCode,Amount,Change,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + cashRec + "','" + cashReturn + "','" + tranid + "','" + username + "','" + date + "')";
                        SqlCommand cmdTender = new SqlCommand(tender, con);
                        con.Open();
                        cmdTender.ExecuteNonQuery();
                        con.Close();
                    }
                    else if (tenderCode == "Card")
                    {
                        string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + username + "','" + date + "')";
                        SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                        con.Open();
                        cmdTender1.ExecuteNonQuery();
                        con.Close();
                    }
                    else if (tenderCode == "Customer")
                    {
                        string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,AccountName,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + cbcustomer.Text + "','" + username + "','" + date + "')";
                        SqlCommand cmdTender1 = new SqlCommand(tender1, con);
                        con.Open();
                        cmdTender1.ExecuteNonQuery();
                        con.Close();
                    }
                    else if (tenderCode == "Check")
                    {
                        string tender1 = "insert into Tender(EndDate,Endtime,TenderCode,Amount,TransactionId,CheckNo,CreateBy,CreateOn)Values('" + onlydate + "','" + onlytime + "','" + tenderCode + "','" + grandTotalAmt + "','" + tranid + "','" + TxtCheck.Text + "','" + username + "','" + date + "')";
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                                dt.Rows[i]["SPromotionName"] = "";
                                dt.Rows[i]["RPromotionName"] = "";
                                dt.Rows[i]["LPromotionName"] = "";
                                dt.Rows[i]["OPromotionName"] = "";
                                dt.Rows[i]["LoyaltyId"] = "";
                                dt.Rows[i]["UnitRetail"] = dt.Rows[i]["Oprice"];
                                dt.Rows[i]["Amount"] = Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"]);
                            }

                            //dt = dt.DefaultView.ToTable();
                            //for (int i = 0; i < dt.AsEnumerable().Count(); i++)
                            //{
                            //    if (Convert.ToInt32(dt.Rows[i]["Quantity"]) == 1)
                            //    {
                            //        dt.Rows[i]["PromotionName"] = "";
                            //        dt.Rows[i]["UnitRetail"] = dt.Rows[i]["OPrice"];
                            //        dt.Rows[i]["Amount"] = Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"]);
                            //    }
                            //}
                            ScanCodeFunction();
                            //DataTable distrinctPromotionName = dt.DefaultView.ToTable(true, "PROName");
                            //DataTable distrinctSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "PROName", "Qty", "NewPrice", "Discount");
                            //DataTable distrinctRPromotionName = dt.DefaultView.ToTable(true, "RPROName");
                            //DataTable distrinctRSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "RPROName", "RQty", "RNewPrice", "RDiscount");
                            //DataTable distrinctLPromotionName = dt.DefaultView.ToTable(true, "LPROName");
                            //DataTable distrinctLSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "LPROName", "LQty", "LNewPrice", "LDiscount");
                            //DataTable distrinctOPromotionName = dt.DefaultView.ToTable(true, "OPROName");
                            //DataTable distrinctOSCANCODE = dt.DefaultView.ToTable(true, "ScanCode", "OPROName", "OQty", "ONewPrice", "ODiscount");

                            //foreach (DataRow distrinctRow in distrinctPromotionName.AsEnumerable())
                            //{
                            //    if (distrinctRow["PROName"].ToString() != "")
                            //    {
                            //        int sumCount = 0;
                            //        for (int j = 0; j < distrinctSCANCODE.AsEnumerable().Count(); j++)
                            //        {
                            //            if (distrinctSCANCODE.Rows[j]["PROName"].ToString() == distrinctRow["PROName"].ToString())
                            //            {
                            //                for (int i = 0; i < dt.Rows.Count; i++)
                            //                {
                            //                    if (distrinctSCANCODE.Rows[j]["PROName"].ToString() == dt.Rows[i]["PROName"].ToString())
                            //                    {
                            //                        //if (distrinctOSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                            //                        //{
                            //                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                            //                        //for (int K = 0; K < dt.Rows.Count; K++)
                            //                        //{
                            //                        foreach (DataRow itemDT1 in distrinctSCANCODE.AsEnumerable())
                            //                        {
                            //                            if (itemDT1["PROName"].ToString() == distrinctRow["PROName"].ToString())
                            //                            {
                            //                                int Y = sumCount / Convert.ToInt32(itemDT1["Qty"]);
                            //                                for (int x = 1; x <= Y; x++)
                            //                                {
                            //                                    if (sumCount == Convert.ToInt32(itemDT1["Qty"]) * x)
                            //                                    {
                            //                                        for (int z = 0; z <= i; z++)
                            //                                        {
                            //                                            if (dt.Rows[z]["PROName"].ToString() == distrinctRow["PROName"].ToString())
                            //                                            {
                            //                                                string price = "";
                            //                                                if (itemDT1["NewPrice"].ToString() != "" && itemDT1["NewPrice"].ToString() != "0")
                            //                                                    price = (Convert.ToDecimal(itemDT1["NewPrice"]) / Convert.ToInt32(itemDT1["Qty"])).ToString();

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
                            //                                                    price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - odisc - rdisc - Convert.ToDecimal(dt.Rows[z]["Discount"])).ToString();
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
                            //                        //    }
                            //                        //}
                            //                    }
                            //                }

                            //            }
                            //        }
                            //    }
                            //}
                            //foreach (DataRow distrinctRow in distrinctRPromotionName.AsEnumerable())
                            //{
                            //    if (distrinctRow["RPROName"].ToString() != "")
                            //    {
                            //        int sumCount = 0;
                            //        for (int j = 0; j < distrinctRSCANCODE.AsEnumerable().Count(); j++)
                            //        {
                            //            if (distrinctRSCANCODE.Rows[j]["RPROName"].ToString() == distrinctRow["RPROName"].ToString())
                            //            {
                            //                for (int i = 0; i < dt.Rows.Count; i++)
                            //                {
                            //                    if (distrinctRSCANCODE.Rows[j]["RPROName"].ToString() == dt.Rows[i]["RPROName"].ToString())
                            //                    {
                            //                        //if (distrinctOSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                            //                        //{
                            //                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                            //                        //for (int K = 0; K < dt.Rows.Count; K++)
                            //                        //{
                            //                        foreach (DataRow itemDT1 in distrinctRSCANCODE.AsEnumerable())
                            //                        {
                            //                            if (itemDT1["RPROName"].ToString() == distrinctRow["RPROName"].ToString())
                            //                            {
                            //                                int Y = sumCount / Convert.ToInt32(itemDT1["RQty"]);
                            //                                for (int x = 1; x <= Y; x++)
                            //                                {
                            //                                    if (sumCount == Convert.ToInt32(itemDT1["RQty"]) * x)
                            //                                    {
                            //                                        for (int z = 0; z <= i; z++)
                            //                                        {
                            //                                            if (dt.Rows[z]["RPROName"].ToString() == distrinctRow["RPROName"].ToString())
                            //                                            {
                            //                                                string price = "";
                            //                                                if (itemDT1["RNewPrice"].ToString() != "" && itemDT1["RNewPrice"].ToString() != "0")
                            //                                                    price = (Convert.ToDecimal(itemDT1["RNewPrice"]) / Convert.ToInt32(itemDT1["RQty"])).ToString();

                            //                                                if (price == "")
                            //                                                {
                            //                                                    decimal ldisc = 0;
                            //                                                    decimal odisc = 0;
                            //                                                    decimal sdisc = 0;
                            //                                                    if (dt.Rows[z]["LPromotionName"].ToString() != "")
                            //                                                        ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
                            //                                                    if (dt.Rows[z]["OPromotionName"].ToString() != "")
                            //                                                        odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
                            //                                                    if (dt.Rows[z]["SPromotionName"].ToString() != "")
                            //                                                        sdisc = Convert.ToDecimal(dt.Rows[z]["Discount"]);
                            //                                                    price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - odisc - sdisc - Convert.ToDecimal(dt.Rows[z]["RDiscount"])).ToString();
                            //                                                }
                            //                                                dt.Rows[z]["RPromotionName"] = dt.Rows[z]["RPROName"];
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
                            //                        //    }
                            //                        //}
                            //                    }
                            //                }

                            //            }
                            //        }
                            //    }
                            //}
                            //foreach (DataRow distrinctRow in distrinctLPromotionName.AsEnumerable())
                            //{
                            //    if (distrinctRow["LPROName"].ToString() != "" && lblLoyaltyId.Content.ToString()!="")
                            //    {
                            //        int sumCount = 0;
                            //        for (int j = 0; j < distrinctLSCANCODE.AsEnumerable().Count(); j++)
                            //        {
                            //            if (distrinctLSCANCODE.Rows[j]["LPROName"].ToString() != "")
                            //            {
                            //                for (int i = 0; i < dt.Rows.Count; i++)
                            //                {
                            //                    if (distrinctLSCANCODE.Rows[j]["LPROName"].ToString() == distrinctRow["LPROName"].ToString())
                            //                    {
                            //                        //if (distrinctLSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                            //                        //{
                            //                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                            //                        //for (int K = 0; K < dt.Rows.Count; K++)
                            //                        //{
                            //                        foreach (DataRow itemDT1 in distrinctLSCANCODE.AsEnumerable())
                            //                        {
                            //                            if (itemDT1["LPROName"].ToString() == distrinctRow["LPROName"].ToString())
                            //                            {
                            //                                int Y = sumCount / Convert.ToInt32(itemDT1["LQty"]);
                            //                                for (int x = 1; x <= Y; x++)
                            //                                {
                            //                                    if (sumCount == Convert.ToInt32(itemDT1["LQty"]) * x)
                            //                                    {
                            //                                        for (int z = 0; z <= i; z++)
                            //                                        {
                            //                                            if (dt.Rows[z]["LPROName"].ToString() == distrinctRow["LPROName"].ToString())
                            //                                            {
                            //                                                string price = "";
                            //                                                if (itemDT1["LNewPrice"].ToString() != "")
                            //                                                    price = (Convert.ToDecimal(itemDT1["LNewPrice"]) / Convert.ToInt32(itemDT1["LQty"])).ToString();

                            //                                                if (price == "")
                            //                                                {
                            //                                                    decimal odisc = 0;
                            //                                                    decimal sdisc = 0;
                            //                                                    decimal rdisc = 0;
                            //                                                    if (dt.Rows[z]["OPromotionName"].ToString() != "")
                            //                                                        odisc = Convert.ToDecimal(dt.Rows[z]["ODiscount"]);
                            //                                                    if (dt.Rows[z]["SPromotionName"].ToString() != "")
                            //                                                        sdisc = Convert.ToDecimal(dt.Rows[z]["SDiscount"]);
                            //                                                    if (dt.Rows[z]["RPromotionName"].ToString() != "")
                            //                                                        rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
                            //                                                    price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - odisc - sdisc - rdisc - Convert.ToDecimal(dt.Rows[z]["LDiscount"])).ToString();
                            //                                                    //price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - Convert.ToDecimal(itemDT1["LDiscount"])).ToString();
                            //                                                }
                            //                                                dt.Rows[z]["LPromotionName"] = dt.Rows[z]["LPROName"];
                            //                                                dt.Rows[z]["UnitRetail"] = price;
                            //                                                dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
                            //                                                dt.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();

                            //                                                dt.Rows[z]["PromotionName"] = "";
                            //                                                dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();

                            //                                                //dt.Rows[z]["LPromotionName"] = itemDT1["LPROName"];
                            //                                                //dt.Rows[z]["UnitRetail"] = price;
                            //                                                //dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
                            //                                            }
                            //                                        }

                            //                                    }

                            //                                }
                            //                            }
                            //                        }
                            //                        //}
                            //                        //}
                            //                    }
                            //                }

                            //            }
                            //        }
                            //    }
                            //}
                            //foreach (DataRow distrinctRow in distrinctOPromotionName.AsEnumerable())
                            //{
                            //    if (distrinctRow["OPROName"].ToString() != "")
                            //    {
                            //        int sumCount = 0;
                            //        for (int j = 0; j < distrinctOSCANCODE.AsEnumerable().Count(); j++)
                            //        {
                            //            if (distrinctOSCANCODE.Rows[j]["OPROName"].ToString() == distrinctRow["OPROName"].ToString())
                            //            {
                            //                for (int i = 0; i < dt.Rows.Count; i++)
                            //                {
                            //                    if (distrinctOSCANCODE.Rows[j]["OPROName"].ToString() == dt.Rows[i]["OPROName"].ToString())
                            //                    {
                            //                        //if (distrinctOSCANCODE.Rows[j]["ScanCode"].ToString() == dt.Rows[i]["ScanCode"].ToString())
                            //                        //{
                            //                        sumCount = Convert.ToInt32(sumCount) + Convert.ToInt32(dt.Rows[i]["Quantity"]);
                            //                        //for (int K = 0; K < dt.Rows.Count; K++)
                            //                        //{
                            //                        foreach (DataRow itemDT1 in distrinctOSCANCODE.AsEnumerable())
                            //                        {
                            //                            if (itemDT1["OPROName"].ToString() == distrinctRow["OPROName"].ToString())
                            //                            {
                            //                                int Y = sumCount / Convert.ToInt32(itemDT1["OQty"]);
                            //                                for (int x = 1; x <= Y; x++)
                            //                                {
                            //                                    if (sumCount == Convert.ToInt32(itemDT1["OQty"]) * x)
                            //                                    {
                            //                                        for (int z = 0; z <= i; z++)
                            //                                        {
                            //                                            if (dt.Rows[z]["OPROName"].ToString() == distrinctRow["OPROName"].ToString())
                            //                                            {
                            //                                                string price = "";
                            //                                                if (itemDT1["ONewPrice"].ToString() != ""&& itemDT1["ONewPrice"].ToString() != "0")
                            //                                                    price = (Convert.ToDecimal(itemDT1["ONewPrice"]) / Convert.ToInt32(itemDT1["OQty"])).ToString();

                            //                                                if (price == "")
                            //                                                {
                            //                                                    decimal ldisc = 0;
                            //                                                    decimal sdisc = 0;
                            //                                                    decimal rdisc = 0;
                            //                                                    if (dt.Rows[z]["LPromotionName"].ToString() != "")
                            //                                                        ldisc = Convert.ToDecimal(dt.Rows[z]["LDiscount"]);
                            //                                                    if (dt.Rows[z]["SPromotionName"].ToString() != "")
                            //                                                        sdisc = Convert.ToDecimal(dt.Rows[z]["SDiscount"]);
                            //                                                    if (dt.Rows[z]["RPromotionName"].ToString() != "")
                            //                                                        rdisc = Convert.ToDecimal(dt.Rows[z]["RDiscount"]);
                            //                                                    price = (Convert.ToDecimal(dt.Rows[z]["Oprice"]) - ldisc - sdisc - rdisc - Convert.ToDecimal(dt.Rows[z]["ODiscount"])).ToString();
                            //                                                }
                            //                                                dt.Rows[z]["OPromotionName"] = dt.Rows[z]["OPROName"];
                            //                                                dt.Rows[z]["UnitRetail"] = price;
                            //                                                dt.Rows[z]["Amount"] = Convert.ToDecimal(dt.Rows[z]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[z]["Quantity"]);
                            //                                                dt.Rows[z]["LoyaltyId"] = lblLoyaltyId.Content.ToString();

                            //                                                dt.Rows[z]["PromotionName"] = "";
                            //                                                dt.Rows[z]["PromotionName"] = dt.Rows[z]["SPromotionName"].ToString() + ", " + dt.Rows[z]["RPromotionName"].ToString() + ", " + dt.Rows[z]["LPromotionName"].ToString() + ", " + dt.Rows[z]["OPromotionName"].ToString();
                            //                                            }
                            //                                        }

                            //                                    }

                            //                                }
                            //                            }
                            //                        }
                            //                        //    }
                            //                        //}
                            //                    }
                            //                }

                            //            }
                            //        }
                            //    }
                            //}
                        }
                    }

                    JRDGrid.ItemsSource = dt.DefaultView;
                    if (JRDGrid.Items.Count > 1)
                    {
                        JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                        JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                    }
                    TotalEvent();
                }
                dataGridSelectedIndex = "";

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
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
                SendErrorToText(ex, errorFileName);
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
                        newRow["ScanCode"] = row.ItemArray[0].ToString();
                        newRow["Description"] = row.ItemArray[1].ToString();
                        newRow["UnitRetail"] = row.ItemArray[2].ToString();
                        newRow["TaxRate"] = row.ItemArray[3].ToString();
                        newRow["Quantity"] = row.ItemArray[4].ToString();
                        newRow["Amount"] = row.ItemArray[5].ToString();
                        newRow["Date"] = onlydate;
                        newRow["Time"] = onlytime;
                        newRow["TransactionId"] = tranid;
                        newRow["CreateBy"] = username;
                        newRow["CreateOn"] = date;
                        newRow["PromotionName"] = row.ItemArray[11].ToString();
                        newRow["Void"] = row.ItemArray[12].ToString();
                        newRow["Oprice"] = row.ItemArray[13].ToString();
                        newRow["PROName"] = row.ItemArray[14].ToString();
                        newRow["Qty"] = row.ItemArray[15].ToString();
                        newRow["NewPrice"] = row.ItemArray[16].ToString();
                        newRow["Discount"] = row.ItemArray[17].ToString();
                        newRow["DiscountBY"] = row.ItemArray[18].ToString();
                        newRow["SPromotionName"] = row.ItemArray[19].ToString();
                        newRow["RPROName"] = row.ItemArray[20].ToString();
                        newRow["RQty"] = row.ItemArray[21].ToString();
                        newRow["RNewPrice"] = row.ItemArray[22].ToString();
                        newRow["RDiscount"] = row.ItemArray[23].ToString();
                        newRow["RPromotionName"] = row.ItemArray[24].ToString();
                        newRow["LPROName"] = row.ItemArray[25].ToString();
                        newRow["LQty"] = row.ItemArray[26].ToString();
                        newRow["LNewPrice"] = row.ItemArray[27].ToString();
                        newRow["LDiscount"] = row.ItemArray[28].ToString();
                        newRow["LPromotionName"] = row.ItemArray[29].ToString();
                        newRow["OPROName"] = row.ItemArray[30].ToString();
                        newRow["OQty"] = row.ItemArray[31].ToString();
                        newRow["ONewPrice"] = row.ItemArray[32].ToString();
                        newRow["ODiscount"] = row.ItemArray[33].ToString();
                        newRow["OPromotionName"] = row.ItemArray[34].ToString();
                        newRow["LoyaltyId"] = row.ItemArray[35].ToString();
                        newRow["Customer"] = customer;
                        dtHold.Rows.Add(newRow);
                        //dataRow[6] = onlydate;
                        //dataRow[7] = onlytime;
                        //dataRow[8] = tranid;
                        //dataRow[9] = username;
                        //dataRow[10] = date;
                        //dataRow[36] = customer;
                    }

                    //SqlBulkCopy objbulk = new SqlBulkCopy(con);
                    //objbulk.DestinationTableName = "Hold";
                    //objbulk.ColumnMappings.Add("Scancode", "ScanCode");
                    //objbulk.ColumnMappings.Add("description", "Descripation");
                    //objbulk.ColumnMappings.Add("quantity", "Quantity");
                    //objbulk.ColumnMappings.Add("unitretail", "Price");
                    //objbulk.ColumnMappings.Add("Amount", "Amount");
                    //objbulk.ColumnMappings.Add("TaxRate", "TaxRate");
                    //objbulk.ColumnMappings.Add("Date", "EndDate");
                    //objbulk.ColumnMappings.Add("Time", "EndTime");
                    //objbulk.ColumnMappings.Add("PromotionName", "PromotionName");
                    //objbulk.ColumnMappings.Add("TransactionId", "TrasactionId");
                    //objbulk.ColumnMappings.Add("Void", "Void");
                    //objbulk.ColumnMappings.Add("Oprice", "OPrice");
                    //objbulk.ColumnMappings.Add("PROName", "ProName");
                    //objbulk.ColumnMappings.Add("Qty", "Qty");
                    //objbulk.ColumnMappings.Add("newprice", "NewPrice");
                    //objbulk.ColumnMappings.Add("Discount", "Discount");
                    //objbulk.ColumnMappings.Add("DiscountBy", "DiscountBy");
                    //objbulk.ColumnMappings.Add("SPromotionName", "SPromotionName");
                    //objbulk.ColumnMappings.Add("RPROName", "RPROName");
                    //objbulk.ColumnMappings.Add("RQty", "RQty");
                    //objbulk.ColumnMappings.Add("RNewPrice", "RNewPrice");
                    //objbulk.ColumnMappings.Add("RDiscount", "RDiscount");
                    //objbulk.ColumnMappings.Add("RPromotionName", "RPromotionName");
                    //objbulk.ColumnMappings.Add("LPROName", "LPROName");
                    //objbulk.ColumnMappings.Add("LQty", "LQty");
                    //objbulk.ColumnMappings.Add("LNewPrice", "LNewPrice");
                    //objbulk.ColumnMappings.Add("LDiscount", "LDiscount");
                    //objbulk.ColumnMappings.Add("LPromotionName", "LPromotionName");
                    //objbulk.ColumnMappings.Add("OPROName", "OPROName");
                    //objbulk.ColumnMappings.Add("OQty", "OQty");
                    //objbulk.ColumnMappings.Add("ONewPrice", "ONewPrice");
                    //objbulk.ColumnMappings.Add("ODiscount", "ODiscount");
                    //objbulk.ColumnMappings.Add("OPromotionName", "OPromotionName");
                    //objbulk.ColumnMappings.Add("LoyaltyId", "LoyaltyId");
                    //objbulk.ColumnMappings.Add("Customer", "Customer");
                    //con.Open();
                    //objbulk.WriteToServer(dt);
                    //con.Close();
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
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
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
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
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
                        newRow["ScanCode"] = row.ItemArray[0].ToString();
                        newRow["Description"] = row.ItemArray[1].ToString();
                        newRow["UnitRetail"] = row.ItemArray[2].ToString();
                        newRow["TaxRate"] = row.ItemArray[3].ToString();
                        newRow["Quantity"] = row.ItemArray[4].ToString();
                        newRow["Amount"] = row.ItemArray[5].ToString();
                        newRow["Date"] = row.ItemArray[6].ToString();
                        newRow["Time"] = row.ItemArray[7].ToString();
                        newRow["TransactionId"] = row.ItemArray[8].ToString();
                        newRow["CreateBy"] = row.ItemArray[9].ToString();
                        newRow["CreateOn"] = row.ItemArray[10].ToString();
                        newRow["PromotionName"] = row.ItemArray[11].ToString();
                        newRow["Void"] = row.ItemArray[12].ToString();
                        newRow["Oprice"] = row.ItemArray[13].ToString();
                        newRow["PROName"] = row.ItemArray[14].ToString();
                        newRow["Qty"] = row.ItemArray[15].ToString();
                        newRow["NewPrice"] = row.ItemArray[16].ToString();
                        newRow["Discount"] = row.ItemArray[17].ToString();
                        newRow["DiscountBY"] = row.ItemArray[18].ToString();
                        newRow["SPromotionName"] = row.ItemArray[19].ToString();
                        newRow["RPROName"] = row.ItemArray[20].ToString();
                        newRow["RQty"] = row.ItemArray[21].ToString();
                        newRow["RNewPrice"] = row.ItemArray[22].ToString();
                        newRow["RDiscount"] = row.ItemArray[23].ToString();
                        newRow["RPromotionName"] = row.ItemArray[24].ToString();
                        newRow["LPROName"] = row.ItemArray[25].ToString();
                        newRow["LQty"] = row.ItemArray[26].ToString();
                        newRow["LNewPrice"] = row.ItemArray[27].ToString();
                        newRow["LDiscount"] = row.ItemArray[28].ToString();
                        newRow["LPromotionName"] = row.ItemArray[29].ToString();
                        newRow["OPROName"] = row.ItemArray[30].ToString();
                        newRow["OQty"] = row.ItemArray[31].ToString();
                        newRow["ONewPrice"] = row.ItemArray[32].ToString();
                        newRow["ODiscount"] = row.ItemArray[33].ToString();
                        newRow["OPromotionName"] = row.ItemArray[34].ToString();
                        newRow["LoyaltyId"] = row.ItemArray[35].ToString();
                        newRow["Customer"] = row.ItemArray[36].ToString();
                        dt.Rows.Add(newRow);
                        //row.Delete();
                    }
                    List<DataRow> rowsToRemove = dtHold.AsEnumerable()
                                   .Where(r => r.Field<string>("TransactionId") == tb).ToList();
                    rowsToRemove.ForEach(dtHold.Rows.Remove);

                    //SqlConnection con = new SqlConnection(conString);
                    //string edate = Convert.ToDateTime(lblDate.Content).ToString("yyyy/MM/dd");
                    //string queryHold = "select Scancode,Descripation as Description,Quantity,Price as UnitRetail,Amount,TaxRate,EndDate as Date,EndTime as Time,PromotionName,TrasactionId as TransactionId,Void,Oprice,PROName,Qty,newprice,Discount,DiscountBy,SPromotionName,RPROName,RQty,RNewPrice,RDiscount,RPromotionName,LPROName,LQty,LNewPrice,LDiscount,LPromotionName,OPROName,OQty,ONewPrice,ODiscount,OPromotionName,LoyaltyId,Customer from Hold where TrasactionId=@transid";
                    //SqlCommand cmdHold = new SqlCommand(queryHold, con);
                    //cmdHold.Parameters.AddWithValue("@transid", tb);
                    //SqlDataAdapter sdaHold = new SqlDataAdapter(cmdHold);
                    //sdaHold.Fill(dt);





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
                SendErrorToText(ex, errorFileName);
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
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
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
            catch (Exception ex) { SendErrorToText(ex, errorFileName); }
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                    lblLoyaltyId.Content = dtAccount.Rows[cc]["Mobile"].ToString();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["UnitRetail"] = dt.Rows[i]["UnitRetail"];
                        dt.Rows[i]["Amount"] = Convert.ToDecimal(dt.Rows[i]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[i]["Quantity"]);
                        dt.Rows[i]["SPromotionName"] = "";
                        dt.Rows[i]["RPromotionName"] = "";
                        dt.Rows[i]["LPromotionName"] = "";
                        dt.Rows[i]["OPromotionName"] = "";
                        dt.Rows[i]["LoyaltyId"] = "";
                        dt.Rows[i]["Customer"] = "";
                        dt.Rows[i]["UnitRetail"] = dt.Rows[i]["UnitRetail"];
                    }
                    //dt = ScanCodeFunction(dt, cc);
                    ScanCodeFunction();
                    JRDGrid.ItemsSource = dt.DefaultView;
                    JRDGrid.Items.Refresh();
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
                SendErrorToText(ex, errorFileName);
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
                //SqlConnection con = new SqlConnection(conString);
                //string querya = "select CATEGORY  from Category  where Category = @Description";
                //SqlCommand cmda = new SqlCommand(querya, con);
                //cmda.Parameters.AddWithValue("@Description", tb.Text);
                //cmda.Parameters.AddWithValue("@qty", 1);
                //SqlDataAdapter sdaa = new SqlDataAdapter(cmda);
                //DataTable dta = new DataTable();
                //sdaa.Fill(dta);
                //int A = dta.Rows.Count;

                int A = (from DataRow row in dtCategory.Rows where (string)row["Category"] == tb select row).Count();

                if (A != 0)
                    button_Click_Category(sender, e, tb);
                else
                {
                    var results = from myRow in dtItem.AsEnumerable()
                                  where myRow.Field<string>("Description") == tb
                                  select myRow;

                    foreach (DataRow row in results)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["ScanCode"] = row.ItemArray[0].ToString();
                        newRow["Description"] = row.ItemArray[1].ToString();
                        if (refund == "")
                            newRow["Quantity"] = 1;
                        else
                            newRow["Quantity"] = -1;
                        newRow["UnitRetail"] = row.ItemArray[2].ToString();
                        newRow["Amount"] = row.ItemArray[2].ToString();
                        newRow["OPrice"] = row.ItemArray[2].ToString();
                        newRow["TaxRate"] = row.ItemArray[3].ToString();
                        newRow["PROName"] = row.ItemArray[4].ToString();
                        newRow["Qty"] = row.ItemArray[5].ToString();
                        newRow["NewPrice"] = row.ItemArray[6].ToString();
                        newRow["Discount"] = row.ItemArray[7].ToString();
                        newRow["RPROName"] = row.ItemArray[8].ToString();
                        newRow["RQty"] = row.ItemArray[9].ToString();
                        newRow["RNewPrice"] = row.ItemArray[10].ToString();
                        newRow["RDiscount"] = row.ItemArray[11].ToString();
                        newRow["LPROName"] = row.ItemArray[12].ToString();
                        newRow["LQty"] = row.ItemArray[13].ToString();
                        newRow["LNewPrice"] = row.ItemArray[14].ToString();
                        newRow["LDiscount"] = row.ItemArray[15].ToString();
                        newRow["OPROName"] = row.ItemArray[16].ToString();
                        newRow["OQty"] = row.ItemArray[17].ToString();
                        newRow["ONewPrice"] = row.ItemArray[18].ToString();
                        newRow["ODiscount"] = row.ItemArray[19].ToString();
                        dt.Rows.Add(newRow);
                    }
                    int dCount = dt.Rows.Count - 1;
                    if (dt.Rows[dCount]["PROName"].ToString() != "")
                    {
                        DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "PROName", "Qty", "NewPrice", "Discount");

                        foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                        {
                            if (distinct["PROName"].ToString() != "")
                            {
                                int sumCount = (from row in dt.AsEnumerable()
                                                where row.Field<string>("PROName") == distinct["PROName"].ToString()
                                                select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                if (sumCount < 0)
                                    sumCount = sumCount * -1;
                                foreach (var itemdt in dt.AsEnumerable())
                                {
                                    if (itemdt["PROName"].ToString() == distinct["PROName"].ToString())
                                    {
                                        for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                        {
                                            if (sumCount == Convert.ToInt32(distinct["Qty"]) * i)
                                            {
                                                if (itemdt["SPromotionName"] != itemdt["PROName"])
                                                {
                                                    string price = "";
                                                    if (itemdt["NewPrice"].ToString() != "" && itemdt["NewPrice"].ToString() != "0")
                                                        price = (Convert.ToDecimal(itemdt["NewPrice"]) / Convert.ToInt32(itemdt["Qty"])).ToString();

                                                    if (price == "")
                                                    {
                                                        decimal odisc = 0;
                                                        decimal ldisc = 0;
                                                        decimal rdisc = 0;
                                                        if (itemdt["OPromotionName"].ToString() != "")
                                                            odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                                                        if (itemdt["LPromotionName"].ToString() != "")
                                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                                                        if (itemdt["RPromotionName"].ToString() != "")
                                                            rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - odisc - ldisc - rdisc - Convert.ToDecimal(itemdt["Discount"])).ToString();
                                                    }
                                                    itemdt["SPromotionName"] = itemdt["PROName"];
                                                    itemdt["UnitRetail"] = price;
                                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                                                    itemdt["PromotionName"] = "";
                                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (dt.Rows[dCount]["RPROName"].ToString() != "")
                    {
                        DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "RPROName", "RQty", "RNewPrice", "RDiscount");

                        foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                        {
                            if (distinct["RPROName"].ToString() != "")
                            {
                                int sumCount = (from row in dt.AsEnumerable()
                                                where row.Field<string>("RPROName") == distinct["RPROName"].ToString()
                                                select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                if (sumCount < 0)
                                    sumCount = sumCount * -1;
                                foreach (var itemdt in dt.AsEnumerable())
                                {
                                    if (itemdt["RPROName"].ToString() == distinct["RPROName"].ToString())
                                    {
                                        for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                        {
                                            if (sumCount == Convert.ToInt32(distinct["RQty"]) * i)
                                            {
                                                if (itemdt["RPromotionName"] != itemdt["RPROName"])
                                                {

                                                    string price = "";


                                                    if (itemdt["RNewPrice"].ToString() != "" && itemdt["RNewPrice"].ToString() != "0")
                                                        price = (Convert.ToDecimal(itemdt["RNewPrice"]) / Convert.ToInt32(itemdt["RQty"])).ToString();

                                                    if (price == "")
                                                    {
                                                        decimal odisc = 0;
                                                        decimal ldisc = 0;
                                                        decimal sdisc = 0;
                                                        if (itemdt["OPromotionName"].ToString() != "")
                                                            odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                                                        if (itemdt["LPromotionName"].ToString() != "")
                                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                                                        if (itemdt["SPromotionName"].ToString() != "")
                                                            sdisc = Convert.ToDecimal(itemdt["Discount"]);

                                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - sdisc - ldisc - odisc - Convert.ToDecimal(itemdt["RDiscount"])).ToString();
                                                    }
                                                    itemdt["RPromotionName"] = itemdt["RPROName"];
                                                    itemdt["UnitRetail"] = price;
                                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);

                                                    itemdt["PromotionName"] = "";
                                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (dt.Rows[dCount]["LPROName"].ToString() != "")
                    {
                        DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "LPROName", "LQty", "LNewPrice", "LDiscount");

                        foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                        {
                            if (distinct["LPROName"].ToString() != "")
                            {
                                int sumCount = (from row in dt.AsEnumerable()
                                                where row.Field<string>("LPROName") == distinct["LPROName"].ToString()
                                                select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                if (sumCount < 0)
                                    sumCount = sumCount * -1;
                                foreach (var itemdt in dt.AsEnumerable())
                                {
                                    if (itemdt["LPROName"].ToString() == distinct["LPROName"].ToString())
                                    {
                                        for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                        {
                                            if (sumCount == Convert.ToInt32(distinct["LQty"]) * i)
                                            {
                                                if (lblLoyaltyId.Content.ToString() != "")
                                                {
                                                    if (itemdt["LPromotionName"] != itemdt["LPROName"])
                                                    {
                                                        string price = "";
                                                        if (itemdt["LNewPrice"].ToString() != "" && itemdt["LNewPrice"].ToString() != "0")
                                                            price = (Convert.ToDecimal(itemdt["LNewPrice"]) / Convert.ToInt32(itemdt["LQty"])).ToString();

                                                        if (price == "")
                                                        {
                                                            decimal odisc = 0;
                                                            decimal sdisc = 0;
                                                            decimal rdisc = 0;
                                                            if (itemdt["OPromotionName"].ToString() != "")
                                                                odisc = Convert.ToDecimal(itemdt["ODiscount"]);
                                                            if (itemdt["SPromotionName"].ToString() != "")
                                                                sdisc = Convert.ToDecimal(itemdt["Discount"]);
                                                            if (itemdt["RPromotionName"].ToString() != "")
                                                                rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                                                            price = (Convert.ToDecimal(itemdt["Oprice"]) - odisc - sdisc - rdisc - Convert.ToDecimal(itemdt["LDiscount"])).ToString();
                                                        }

                                                        itemdt["LPromotionName"] = itemdt["LPROName"];
                                                        itemdt["UnitRetail"] = price;
                                                        itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);
                                                        itemdt["LoyaltyId"] = lblLoyaltyId.Content.ToString();

                                                        itemdt["PromotionName"] = "";
                                                        itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (dt.Rows[dCount]["OPROName"].ToString() != "")
                    {
                        DataTable distrinctPromotion = dt.DefaultView.ToTable(true, "OPROName", "OQty", "ONewPrice", "ODiscount");

                        foreach (DataRow distinct in distrinctPromotion.AsEnumerable())
                        {
                            if (distinct["OPROName"].ToString() != "")
                            {
                                int sumCount = (from row in dt.AsEnumerable()
                                                where row.Field<string>("OPROName") == distinct["OPROName"].ToString()
                                                select row).Sum(r => Convert.ToInt32(r.Field<string>("Quantity")));
                                if (sumCount < 0)
                                    sumCount = sumCount * -1;
                                foreach (var itemdt in dt.AsEnumerable())
                                {
                                    if (itemdt["OPROName"].ToString() == distinct["OPROName"].ToString())
                                    {
                                        for (int i = 1; i <= dt.AsEnumerable().Count(); i++)
                                        {
                                            if (sumCount == Convert.ToInt32(distinct["OQty"]) * i)
                                            {
                                                if (itemdt["OPromotionName"] != itemdt["OPROName"])
                                                {
                                                    string price = "";
                                                    if (itemdt["ONewPrice"].ToString() != "" && itemdt["ONewPrice"].ToString() != "0")
                                                        price = (Convert.ToDecimal(itemdt["ONewPrice"]) / Convert.ToInt32(itemdt["OQty"])).ToString();

                                                    if (price == "")
                                                    {
                                                        decimal ldisc = 0;
                                                        decimal sdisc = 0;
                                                        decimal rdisc = 0;
                                                        if (itemdt["LPromotionName"].ToString() != "")
                                                            ldisc = Convert.ToDecimal(itemdt["LDiscount"]);
                                                        if (itemdt["SPromotionName"].ToString() != "")
                                                            sdisc = Convert.ToDecimal(itemdt["Discount"]);
                                                        if (itemdt["RPromotionName"].ToString() != "")
                                                            rdisc = Convert.ToDecimal(itemdt["RDiscount"]);
                                                        price = (Convert.ToDecimal(itemdt["Oprice"]) - ldisc - sdisc - rdisc - Convert.ToDecimal(itemdt["ODiscount"])).ToString();
                                                    }

                                                    itemdt["OPromotionName"] = itemdt["OPROName"];
                                                    itemdt["UnitRetail"] = price;
                                                    itemdt["Amount"] = Convert.ToDecimal(itemdt["UnitRetail"]) * Convert.ToDecimal(itemdt["Quantity"]);

                                                    itemdt["PromotionName"] = "";
                                                    itemdt["PromotionName"] = itemdt["SPromotionName"].ToString() + ", " + itemdt["RPromotionName"].ToString() + ", " + itemdt["LPromotionName"].ToString() + ", " + itemdt["OPromotionName"].ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    JRDGrid.ItemsSource = dt.DefaultView;
                    JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                    JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                    TotalEvent();
                    categorytext = "";
                }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
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
                SendErrorToText(ex, errorFileName);
            }
        }

        public void CellEditMethod()
        {
            try
            {
                int rowIndex = dt.Rows.Count - 1;
                DataRow dataRow = dt.Rows[rowIndex];
                if (dt.Rows[rowIndex]["PROName"].ToString() != "")
                {
                    int qDT = Convert.ToInt32(dt.Rows[rowIndex]["Quantity"]);
                    int qDT1 = Convert.ToInt32(dt.Rows[rowIndex]["Qty"]);

                    if (qDT >= qDT1)
                    {
                        int QA = qDT1 * (qDT / qDT1);
                        if (dt.Rows[rowIndex]["NewPrice"].ToString() != "")
                        {
                            dt.Rows[rowIndex]["PromotionName"] = dt.Rows[rowIndex]["PROName"];
                            dt.Rows[rowIndex]["Quantity"] = QA;
                            dt.Rows[rowIndex]["UnitRetail"] = Convert.ToDecimal(dt.Rows[rowIndex]["NewPrice"]) / qDT1;
                            dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                        }
                        else
                        {
                            dt.Rows[rowIndex]["PromotionName"] = dt.Rows[rowIndex]["PROName"];
                            dt.Rows[rowIndex]["Quantity"] = QA;
                            dt.Rows[rowIndex]["UnitRetail"] = Convert.ToDecimal(dt.Rows[rowIndex]["OPrice"]) - (Convert.ToDecimal(dt.Rows[rowIndex]["OPrice"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Discount"]) / 100);
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
                                newRow["Quantity"] = 1;
                                newRow["UnitRetail"] = dt.Rows[rowIndex]["OPrice"];
                                newRow["Amount"] = Convert.ToInt32(newRow["Quantity"]) * Convert.ToDecimal(newRow["UnitRetail"]);
                                newRow["OPrice"] = dt.Rows[rowIndex]["OPrice"];
                                newRow["PromotionName"] = "";
                                newRow["TaxRate"] = dt.Rows[rowIndex]["TaxRate"];
                                newRow["PROName"] = dt.Rows[rowIndex]["PROName"];
                                newRow["Qty"] = dt.Rows[rowIndex]["Qty"];
                                newRow["NewPrice"] = dt.Rows[rowIndex]["NewPrice"];
                                newRow["Discount"] = dt.Rows[rowIndex]["Discount"];
                                newRow["DiscountBy"] = dt.Rows[rowIndex]["DiscountBy"];
                                dt.Rows.Add(newRow);
                            }
                        }
                    }
                    else
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
                            newRow["PROName"] = dt.Rows[rowIndex]["PROName"];
                            newRow["PromotionName"] = "";
                            newRow["Qty"] = dt.Rows[rowIndex]["Qty"];
                            newRow["NewPrice"] = dt.Rows[rowIndex]["NewPrice"];
                            newRow["Discount"] = dt.Rows[rowIndex]["Discount"];
                            newRow["DiscountBy"] = dt.Rows[rowIndex]["DiscountBy"];

                            dt.Rows.Add(newRow);
                        }
                        DataRow dr = dt.Rows[rowIndex];
                        dt.Rows.Remove(dr);
                        dt.AcceptChanges();


                    }
                    int intv = qDT1 * (qDT / qDT1);
                    decimal ab = qDT / qDT1;
                    decimal decv = Convert.ToDecimal(qDT1) * Convert.ToDecimal(qDT) / Convert.ToDecimal(qDT1);

                    ScanCodeFunction();
                }
                else
                {
                    dt.Rows[rowIndex]["Amount"] = Convert.ToDecimal(Convert.ToDecimal(dt.Rows[rowIndex]["UnitRetail"]) * Convert.ToDecimal(dt.Rows[rowIndex]["Quantity"])).ToString("0.00");
                }
                JRDGrid.ItemsSource = dt.DefaultView;
                JRDGrid.ScrollIntoView(JRDGrid.Items[JRDGrid.Items.Count - 1]);
                JRDGrid.SelectedIndex = JRDGrid.Items.Count - 1;
                TotalEvent();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
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
    }
}
