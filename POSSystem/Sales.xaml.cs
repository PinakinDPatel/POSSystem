using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.SqlClient;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Sales : Window
    {
        string constring = "Server=184.168.194.64;Database=db_POS; User ID=pinakin;Password=PO$123456; Trusted_Connection=false;MultipleActiveResultSets=true";
        public Sales()
        {
            InitializeComponent();
            SqlConnection con = new SqlConnection(constring);
            string queryS = "Select Department from Department";
            SqlCommand cmd = new SqlCommand(queryS, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 7;
            newDropShadowEffect.Direction = 100;
            newDropShadowEffect.Opacity = 95;
            newDropShadowEffect.ShadowDepth = 2;
            
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                Button button = new Button()
                {
                    Content = dt.Rows[i].ItemArray[0],
                    Tag = i
                };
                button.Foreground = new SolidColorBrush(Colors.White);
                button.Background = new SolidColorBrush(Colors.Blue);
                
                button.Effect = new DropShadowEffect()
                { BlurRadius = 5, ShadowDepth = 2 };
                button.Margin = new Thickness(5, 5, 5, 5);
                // button.Effect.add
                button.Click += new RoutedEventHandler(button_Click);

                this.sp21.Children.Add(button);

            }
        }
        void button_Click(object sender, RoutedEventArgs e)
        {

            Console.WriteLine(string.Format("You clicked on the {0}. button.", (sender as Button).Tag));
            MessageBox.Show(e.ToString());
            TxtBxStackPanel2.Visibility = Visibility.Visible;
            sp21.Visibility = Visibility.Hidden;
        }
        void insert(int i)
        {


        }

        private void Button_Click_Sale_Save(object sender, RoutedEventArgs e)
        {
            sp21.Visibility = Visibility.Visible;
            TxtBxStackPanel2.Visibility = Visibility.Hidden;
        }
        
    }
}
