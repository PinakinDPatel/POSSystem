using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using System.Windows.Media.Effects;
using System.Data.SqlClient;
using System.Windows.Input;

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
            dt.Columns.Add("Total");
            con.Close();
            textBox1.Focus();

            //DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            //newDropShadowEffect.BlurRadius = 7;
            //newDropShadowEffect.Direction = 180;
            //newDropShadowEffect.Opacity = 95;
            //newDropShadowEffect.ShadowDepth = 8;
            //for (int i = 0; i < 10; ++i)
            //{
            //    Button button = new Button()
            //    {
            //        Content = string.Format("Button for {0}", i),

            //        Tag = i
            //    };
            //    button.Foreground = new SolidColorBrush(Colors.LightGray);
            //    button.Background = new SolidColorBrush(Colors.Blue);
            //   button.Effect = new DropShadowEffect()
            //   { BlurRadius = 3, ShadowDepth = 10 };
            //    // button.Effect.add
            //    button.Click += new RoutedEventHandler(button_Click);

            //    this.grid.Children.Add(button);

            //}
        }



        //private void EnterClicked(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == Key.Return)
        //    {
        //        Txtbarcode.Text = "You Entered: " + Txtbarcode.Text;
        //    }
        //    SqlConnection con = new SqlConnection(conString);
        //    string query = "select * from item where Scancode=@password ";
        //    SqlCommand cmd = new SqlCommand(query, con);

        //    cmd.Parameters.AddWithValue("@password", Txtbarcode.Text);
        //    SqlDataAdapter sda = new SqlDataAdapter(cmd);
        //    DataTable dt = new DataTable();
        //    sda.Fill(dt);

        //    JRDGrid.ItemsSource = dt.DefaultView;
        //}

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SqlConnection con = new SqlConnection(conString);
                   string query = "select Scancode,description,unitretail from item where Scancode=@password ";
                   SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@password", textBox1.Text);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                //DataTable dt = new DataTable();
                con.Open();
                sda.Fill(dt);
                con.Close();
                int i = Int32.Parse(dt.Rows[0]["unitretail"].ToString());

                dt.Rows[0]["Total"] = i * 10;
                JRDGrid.Items.Add(dt);
                //   JRDGrid.ItemsSource = dt.DefaultView;
                textBox1.Text = "";
                
                
            }
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
