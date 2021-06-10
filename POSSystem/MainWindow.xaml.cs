using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using System.Windows.Media.Effects;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Collections.Generic;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable dt = new DataTable();
        string conString = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        //string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\PSPCStore\POSSystem\POSSystem\Database1.mdf;Integrated Security=True";
        public MainWindow()
        {
            InitializeComponent();
            TextBox tb = new TextBox();
            tb.KeyDown += new KeyEventHandler(OnKeyDownHandler);
            SqlConnection con = new SqlConnection(conString);
            string query = "select Scancode,description,unitretail from item where Scancode=@password ";
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@password", textBox1.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            con.Open();
            sda.Fill(dt);
            dt.Columns.Add("quantity");
            dt.Columns.Add("Amount");
           
            con.Close();
            textBox1.Focus();

            string queryS = "Select Department from Department";
            SqlCommand cmd1 = new SqlCommand(queryS, con);
            SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
            DataTable dtdep = new DataTable();
            sda1.Fill(dtdep);
            con.Open();
            cmd1.ExecuteNonQuery();
            con.Close();

            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 5;
            newDropShadowEffect.Direction = 100;
            newDropShadowEffect.Opacity = 95;
            newDropShadowEffect.ShadowDepth = 2;

            for (int i = 0; i < dtdep.Rows.Count; ++i)
            {
                Button button = new Button()
                {
                    Content = dtdep.Rows[i].ItemArray[0],
                    Tag = i
                };
                button.Foreground = new SolidColorBrush(Colors.White);
                button.Background = new SolidColorBrush(Colors.DarkRed);
                button.Effect = new DropShadowEffect()
                { BlurRadius = 5, ShadowDepth = 2, Color = Colors.BlueViolet };
                button.Margin = new Thickness(5, 5, 5, 5);
                // button.Effect.add
                button.Click += new RoutedEventHandler(button_Click);

                this.sp21.Children.Add(button);


            }
        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(string.Format("You clicked on the {0}. button.", (sender as Button).Tag));
            //MessageBox.Show(e.ToString());
            var btnContent = sender as Button;
            lblDepartment.Content = btnContent.Content;
            TxtBxStackPanel2.Visibility = Visibility.Visible;
            sp21.Visibility = Visibility.Hidden;
        }
        private void Button_Click_Go_Back(object sender, RoutedEventArgs e)
        {
            sp21.Visibility = Visibility.Visible;
            TxtBxStackPanel2.Visibility = Visibility.Hidden;
        }
        private void Button_Click_Sale_Save(object sender, RoutedEventArgs e)
        {
            DataRow dr = dt.NewRow();
            dr[0] = 0;
            dr[1] = lblDepartment.Content.ToString();
            dr[2] = txtDeptAmt.Text;
            dr[3] = 1;
            dr[4] = (int.Parse(txtDeptAmt.Text) * 1).ToString();
            dt.Rows.Add(dr);
           
            JRDGrid.ItemsSource = dt.DefaultView;
            JRDGrid.Items.Refresh();
            txtDeptAmt.Text = "";

            sp21.Visibility = Visibility.Visible;
            TxtBxStackPanel2.Visibility = Visibility.Hidden;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SqlConnection con = new SqlConnection(conString);
                string query = "select Scancode,Description,UnitRetail from item where Scancode=@password ";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@password", textBox1.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                con.Open();
                sda.Fill(dt);
                con.Close();
                JRDGrid.ItemsSource = dt.DefaultView;
                JRDGrid.Items.Refresh();
                textBox1.Text = "";


            }
        }

        private void Cash_Click(object sender, RoutedEventArgs e)
        {
            cashTxtPanel.Visibility = Visibility.Visible;
            sp02.Visibility = Visibility.Hidden;
        }
        //void button_Click(object sender, RoutedEventArgs e)
        //{

        //    Console.WriteLine(string.Format("You clicked on the {0}. button.", (sender as Button).Tag));
        //    int i = Convert.ToInt32((sender as Button).Tag);
        //    insert(i);
        //    DataTable dt = new DataTable();
        //   // dt = (sele)


        //    MessageBox.Show(string.Format("You clicked on the {0}. button.", (sender as Button).Tag));
        //}
        //void insert(int i)
        //{
        //}
    }

}
public class Author
{
    public int Scancode { get; set; }
    public string Description { get; set; }
    public string Quantity { get; set; }
    public string UnitRetail { get; set; }
    public string Amount { get; set; }
}