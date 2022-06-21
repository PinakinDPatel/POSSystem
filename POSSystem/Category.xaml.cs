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
    /// <summary>
    /// Interaction logic for Category.xaml
    /// </summary>
    public partial class Category : Window
    {
        string conString = App.Current.Properties["ConString"].ToString();
        string username = App.Current.Properties["username"].ToString();
        private static String ErrorlineNo, Errormsg, extype, ErrorLocation, exurl, hostIp;
        string errorFileName = "Category.cs";

        public Category()
        {
            try
            {
                InitializeComponent();
                CateGridView();
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void CateGridView()
        {
            try
            {
                btnGoBack.Visibility = Visibility.Hidden;
                gCategory.Visibility = Visibility.Visible;
                gSubCategory.Visibility = Visibility.Hidden;
                ugCategory.Children.Clear();
                SqlConnection con = new SqlConnection(conString);
                string queryD = "Select category,CategoryImage from AddCategory";
                SqlCommand cmd = new SqlCommand(queryD, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; ++i)
                {
                    Button button = new Button();
                    Grid G = new Grid();
                    G.RowDefinitions.Add(new RowDefinition());
                    G.RowDefinitions.Add(new RowDefinition());
                    TextBlock TB = new TextBlock();
                    Image image = new System.Windows.Controls.Image();

                    TB.Text = dt.Rows[i].ItemArray[0].ToString();
                    TB.TextAlignment = TextAlignment.Center;
                    TB.TextWrapping = TextWrapping.Wrap;
                    button.Tag = TB.Text;
                    if (dt.Rows[i].ItemArray[1].ToString() != "")
                    {
                        var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                        var path = dt.Rows[i].ItemArray[1].ToString();
                        var fullpath = Path + "\\Image\\" + path;
                        image.Source = new BitmapImage(new Uri(fullpath));
                        image.Height = 50;
                        image.Width = 80;
                        image.Stretch = Stretch.Fill;
                    }
                    button.Width = 120;
                    button.Height = 80;
                    button.Margin = new Thickness(5);
                    button.Click += (sender, e) => { button_Click(sender, e); };
                    Grid.SetRow(image, 0);
                    G.Children.Add(image);
                    Grid.SetRow(TB, 1);
                    G.Children.Add(TB);
                    G.VerticalAlignment = VerticalAlignment.Bottom;
                    button.Content = G;
                    this.ugCategory.VerticalAlignment = VerticalAlignment.Top;
                    this.ugCategory.Columns = 9;
                    this.ugCategory.Children.Add(button);


                }

            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            hdnCategory.Content = (sender as Button).Tag;
            btnGoBack.Visibility = Visibility.Visible;
            ugSubCategory.Children.Clear();
            var text = hdnCategory.Content;
            SqlConnection con = new SqlConnection(conString);
            string queryC = "Select Description,CategoryImage from Category where Category='" + text + "'";
            SqlCommand cmd = new SqlCommand(queryC, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dtC = new DataTable();
            sda.Fill(dtC);
            if (dtC.Rows.Count != 0)
            {
                for (int i = 0; i < dtC.Rows.Count; ++i)
                {
                    hdnAddCategory.Content = text;
                    Button button = new Button();
                    Grid G = new Grid();
                    G.RowDefinitions.Add(new RowDefinition());
                    G.RowDefinitions.Add(new RowDefinition());
                    TextBlock TB = new TextBlock();
                    Image image = new System.Windows.Controls.Image();

                    TB.Text = dtC.Rows[i].ItemArray[0].ToString();
                    TB.TextAlignment = TextAlignment.Center;
                    TB.TextWrapping = TextWrapping.Wrap;
                    button.Tag = TB.Text;
                    if (dtC.Rows[i].ItemArray[1].ToString() != "")
                    {
                        var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                        var path = dtC.Rows[i].ItemArray[1].ToString();
                        var fullpath = Path + "\\Image\\" + path;
                        image.Source = new BitmapImage(new Uri(fullpath));
                        image.Height = 50;
                        image.Width = 80;
                        image.Stretch = Stretch.Fill;
                    }
                    button.Width = 120;
                    button.Height = 80;
                    button.Margin = new Thickness(5);
                    button.Click += new RoutedEventHandler(button_Click);
                    Grid.SetRow(image, 0);
                    G.Children.Add(image);
                    Grid.SetRow(TB, 1);
                    G.Children.Add(TB);
                    G.VerticalAlignment = VerticalAlignment.Bottom;
                    button.Content = G;
                    this.ugSubCategory.VerticalAlignment = VerticalAlignment.Top;
                    this.ugSubCategory.Columns = 9;
                    this.ugSubCategory.Children.Add(button);
                    gCategory.Visibility = Visibility.Hidden;
                    gSubCategory.Visibility = Visibility.Visible;
                   
                }
            }
            else
            {
                gCategory.Visibility = Visibility.Hidden;
                gSubCategory.Visibility = Visibility.Visible;
                string queryD = "Select CategoryId,Scancode,Description,CategoryImage,Category from Category where Description='" + text + "'";
                SqlCommand cmdD = new SqlCommand(queryD, con);
                SqlDataAdapter sdaD = new SqlDataAdapter(cmdD);
                DataTable dtCD = new DataTable();
                sdaD.Fill(dtCD);
                if (dtCD.Rows.Count != 0)
                {
                    lblCategoryId.Content = dtCD.Rows[0].ItemArray[0].ToString();
                    txtItem.Text = dtCD.Rows[0].ItemArray[1].ToString();
                    txtSubCate.Text = dtCD.Rows[0].ItemArray[2].ToString();
                    txtSubCateImage.Text = dtCD.Rows[0].ItemArray[3].ToString();
                    hdnAddCategory.Content = dtCD.Rows[0].ItemArray[4].ToString();
                    btnCateDelete.Visibility = Visibility.Visible;
                    btnCateDrill.Visibility = Visibility.Visible;
                    if (txtItem.Text != "")
                    {
                        btnCateDrill.Visibility = Visibility.Hidden;
                    }
                    btnDeptSave.Content = "Update";
                }
                else
                {
                    gCategory.Visibility = Visibility.Visible;
                    gSubCategory.Visibility = Visibility.Hidden;
                    string queryAddCategory = "Select CategoryId,Category,CategoryImage from AddCategory where Category='" + text + "'";
                    SqlCommand cmdAddCategory = new SqlCommand(queryAddCategory, con);
                    SqlDataAdapter sdaAddCategory = new SqlDataAdapter(cmdAddCategory);
                    DataTable dtAddCategory = new DataTable();
                    sdaAddCategory.Fill(dtAddCategory);
                    lblAddCategoryId.Content = dtAddCategory.Rows[0].ItemArray[0].ToString();
                    txtCategory.Text = dtAddCategory.Rows[0].ItemArray[1].ToString();
                    txtCategoryImg.Text = dtAddCategory.Rows[0].ItemArray[2].ToString();
                    hdnAddCategory.Content = text;
                    btnCDrill.Visibility = Visibility.Visible;
                    btnCDelete.Visibility = Visibility.Visible;
                    btnCSave.Content = "Update";
                }

            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click_Category(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string descr = "", rate = "";
                string time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss tt");

                if ((txtItem.Text != "" || txtSubCate.Text != ""))
                {
                    string AlreadyExist = "select * from (select Category from addcategory union all select Description from Category)as x where Category ='" + txtSubCate.Text + "'";
                    SqlCommand cmdAlreadyExist = new SqlCommand(AlreadyExist, con);
                    SqlDataAdapter sdaAlreadyExist = new SqlDataAdapter(cmdAlreadyExist);
                    DataTable dtAlreadyExist = new DataTable();
                    sdaAlreadyExist.Fill(dtAlreadyExist);
                    if (dtAlreadyExist.Rows.Count == 0)
                    {
                        if (txtItem.Text != "")
                        {
                            string queryD = "Select Description from Item where Scancode = '" + txtItem.Text + "'";
                            SqlCommand cmd = new SqlCommand(queryD, con);
                            SqlDataAdapter sda = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            if (dt.AsEnumerable().Count() != 0)
                            {
                                descr = dt.Rows[0]["Description"].ToString();
                            }
                            string queryI1 = "";
                            if (lblCategoryId.Content is null)
                            {
                                queryI1 = "Insert into Category(Category,ScanCode,Description,CreateOn,CreateBy,CategoryImage)Values(@Category,@ScanCode,@Description,@time,@CreateBy,@CategoryImage)";

                            }
                            else
                            {
                                queryI1 = "Update Category set Category=@Category,ScanCode=@ScanCode,Description=@Description,CreateOn=@time,CreateBy=@CreateBy,CategoryImage=@CategoryImage where CategoryId='" + lblCategoryId.Content + "'";
                            }
                            SqlCommand cmdI1 = new SqlCommand(queryI1, con);
                            cmdI1.Parameters.AddWithValue("@Category", hdnAddCategory.Content);
                            cmdI1.Parameters.AddWithValue("@ScanCode", txtItem.Text);
                            cmdI1.Parameters.AddWithValue("@Description", descr);
                            cmdI1.Parameters.AddWithValue("@CreateBy", username);
                            cmdI1.Parameters.AddWithValue("@time", time);
                            cmdI1.Parameters.AddWithValue("@CategoryImage", txtSubCateImage.Text);
                            con.Open();
                            cmdI1.ExecuteNonQuery();
                            con.Close();
                            hdnCategory.Content = null;
                            txtItem.Text = "";
                            txtSubCate.Text = "";
                            txtSubCateImage.Text = "";
                            btnDeptSave.Content = "Save";
                            btnCateDelete.Visibility = Visibility.Hidden;
                            btnCateDrill.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            string queryI = "";
                            if (lblCategoryId.Content is null)
                            {
                                queryI = "Insert into Category(Category,Description,CreateOn,CreateBy,CategoryImage)Values(@Category,@Description,@time,@CreateBy,@CategoryImage)";
                            }
                            else
                            {
                                queryI = "Update Category set Category=@Category,Description=@Description,CreateOn=@time,CreateBy=@CreateBy,CategoryImage=@CategoryImage where Categoryid='" + lblCategoryId.Content + "'";
                            }
                            SqlCommand cmdI = new SqlCommand(queryI, con);
                            cmdI.Parameters.AddWithValue("@Category", hdnAddCategory.Content);
                            cmdI.Parameters.AddWithValue("@Description", txtSubCate.Text);
                            cmdI.Parameters.AddWithValue("@CreateBy", username);
                            cmdI.Parameters.AddWithValue("@time", time);
                            cmdI.Parameters.AddWithValue("@CategoryImage", txtSubCateImage.Text);
                            con.Open();
                            cmdI.ExecuteNonQuery();
                            con.Close();
                            lblCategoryId.Content = null;
                            txtItem.Text = "";
                            txtSubCate.Text = "";
                            txtSubCateImage.Text = "";
                            btnDeptSave.Content = "Save";
                            btnCateDelete.Visibility = Visibility.Hidden;
                            btnCateDrill.Visibility = Visibility.Hidden;
                        }
                    CateGridView();
                    }
                    else { MessageBox.Show("'" + txtSubCate.Text + "' is Already Exists in Database"); }
                }
                
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void onDeleteCategory(object sender, RoutedEventArgs e)
        {

            SqlConnection con = new SqlConnection(conString);
            string query = "Delete from Category where Categoryid =@CategoryId";
            SqlCommand cmdI = new SqlCommand(query, con);
            cmdI.Parameters.AddWithValue("@CategoryId", lblCategoryId.Content);
            con.Open();
            cmdI.ExecuteNonQuery();
            con.Close();
            //button_Click(sender,e);
            CateGridView();
            txtItem.Text = "";
            txtSubCate.Text = "";
            txtSubCateImage.Text = "";
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            CateGridView();
            hdnAddCategory.Content = "";
            hdnCategory.Content = "";
            lblAddCategoryId.Content = null;
            lblCategoryId.Content = null;
            txtItem.Text = "";
            txtSubCate.Text = "";
            txtSubCateImage.Text = "";
            btnDeptSave.Content = "Save";
            btnCDrill.Visibility = Visibility.Hidden;
            btnCDelete.Visibility = Visibility.Hidden;
            btnCateDelete.Visibility = Visibility.Hidden;
            btnCateDrill.Visibility = Visibility.Hidden;
            btnCSave.Content = "Save";
            txtCategory.Text = "";
            txtCategoryImg.Text = "";
        }

        private void BtnCDrill_Click(object sender, RoutedEventArgs e)
        {
            var text = "";
            if (txtCategory.Text == "")
            {
                text = txtSubCate.Text;
            }
            else
            {
                text = txtCategory.Text;
            }
            SqlConnection con = new SqlConnection(conString);
            string queryC = "Select Description,CategoryImage from Category where Category='" + text + "'";
            SqlCommand cmd = new SqlCommand(queryC, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dtC = new DataTable();
            sda.Fill(dtC);

            for (int i = 0; i < dtC.Rows.Count; ++i)
            {

                Button button = new Button();
                Grid G = new Grid();
                G.RowDefinitions.Add(new RowDefinition());
                G.RowDefinitions.Add(new RowDefinition());
                TextBlock TB = new TextBlock();
                Image image = new System.Windows.Controls.Image();

                TB.Text = dtC.Rows[i].ItemArray[0].ToString();
                TB.TextAlignment = TextAlignment.Center;
                TB.TextWrapping = TextWrapping.Wrap;
                button.Tag = TB.Text;
                if (dtC.Rows[i].ItemArray[1].ToString() != "")
                {
                    var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                    var path = dtC.Rows[i].ItemArray[1].ToString();
                    var fullpath = Path + "\\Image\\" + path;
                    image.Source = new BitmapImage(new Uri(fullpath));
                    image.Height = 50;
                    image.Width = 80;
                    image.Stretch = Stretch.Fill;
                }
                button.Width = 120;
                button.Height = 80;
                button.Margin = new Thickness(5);
                button.Click += new RoutedEventHandler(button_Click);
                Grid.SetRow(image, 0);
                G.Children.Add(image);
                Grid.SetRow(TB, 1);
                G.Children.Add(TB);
                G.VerticalAlignment = VerticalAlignment.Bottom;
                button.Content = G;
                this.ugSubCategory.VerticalAlignment = VerticalAlignment.Top;
                this.ugSubCategory.Columns = 9;
                this.ugSubCategory.Children.Add(button);
            }
            gCategory.Visibility = Visibility.Hidden;
            gSubCategory.Visibility = Visibility.Visible;
            btnCateDrill.Visibility = Visibility.Hidden;
            btnCateDelete.Visibility = Visibility.Hidden;
            lblAddCategoryId.Content = null;
            lblCategoryId.Content = null;
            txtCategory.Text = "";
            txtSubCate.Text = "";
            txtSubCateImage.Text = "";
            txtItem.Text = "";
            txtCategoryImg.Text = "";
            btnCDrill.Visibility = Visibility.Hidden;
            btnCDelete.Visibility = Visibility.Hidden;
            btnCSave.Content = "Save";
            hdnAddCategory.Content = text;
        }

        private void btnAddSave_Click_Category(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string AlreadyExist = "select * from (select Category from addcategory union all select Description from Category)as x where Category ='" + txtCategory.Text + "'";
                SqlCommand cmdAlreadyExist = new SqlCommand(AlreadyExist, con);
                SqlDataAdapter sdaAlreadyExist = new SqlDataAdapter(cmdAlreadyExist);
                DataTable dtAlreadyExist = new DataTable();
                sdaAlreadyExist.Fill(dtAlreadyExist);
                if (dtAlreadyExist.Rows.Count == 0)
                {
                    string queryI = "";
                    if (lblAddCategoryId.Content is null)
                    {
                        queryI = "Insert into AddCategory(Category,CategoryImage)Values(@Category,@CategoryImage)";

                    }
                    else
                    {
                        queryI = "Update AddCategory set Category=@Category,CategoryImage=@CategoryImage where Categoryid='" + lblAddCategoryId.Content + "'";
                    }
                    SqlCommand cmdI = new SqlCommand(queryI, con);
                    cmdI.Parameters.AddWithValue("@Category", txtCategory.Text);
                    cmdI.Parameters.AddWithValue("@CategoryImage", txtCategoryImg.Text);
                    con.Open();
                    cmdI.ExecuteNonQuery();
                    con.Close();
                    txtCategory.Text = "";
                    txtCategoryImg.Text = "";
                    lblAddCategoryId.Content = null;
                    CateGridView();
                    btnCDrill.Visibility = Visibility.Hidden;
                    btnCDelete.Visibility = Visibility.Hidden;
                    btnCSave.Content = "Save";
                }
                else { MessageBox.Show("'" + txtCategory.Text + "' is Already Exists in Database"); }
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void Categorydelete_click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(conString);
                string query = "Delete from AddCategory where Categoryid =@CategoryId";
                SqlCommand cmdI = new SqlCommand(query, con);
                cmdI.Parameters.AddWithValue("@CategoryId", lblAddCategoryId.Content);
                con.Open();
                cmdI.ExecuteNonQuery();
                con.Close();
                //button_Click(hdnCategory.Content, e);
                CateGridView();
                txtCategory.Text = "";
                txtCategoryImg.Text = "";
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

        private void txtCategoryImg_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
                dlg.InitialDirectory = "c:\\";
                dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedFileName = System.IO.Path.GetFileName(dlg.FileName);
                    txtCategoryImg.Text = selectedFileName;
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(dlg.FileName);
                    bitmap.EndInit();
                    //ImageViewer1.Source = bitmap;
                }
                var sourcepath = dlg.FileName;


                var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                var fullpath = Path + "Image\\";
                System.IO.File.Copy(sourcepath, fullpath + System.IO.Path.GetFileName(sourcepath));
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }

        private void txtSubCateImage_GotFocus(object sender, RoutedEventArgs e)
                                                                                                                                                                                {
            try
            {
                System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
                dlg.InitialDirectory = "c:\\";
                dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedFileName = System.IO.Path.GetFileName(dlg.FileName);
                    txtSubCateImage.Text = selectedFileName;
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(dlg.FileName);
                    bitmap.EndInit();
                    //ImageViewer1.Source = bitmap;
                }
                var sourcepath = dlg.FileName;


                var Path = System.AppDomain.CurrentDomain.BaseDirectory;
                var fullpath = Path + "Image\\";
                System.IO.File.Copy(sourcepath, fullpath + System.IO.Path.GetFileName(sourcepath));
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, errorFileName);
            }
        }
    }
}
