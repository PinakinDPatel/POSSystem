using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();

            Grid G = new Grid();
            G.RowDefinitions.Add(new RowDefinition());
            G.RowDefinitions.Add(new RowDefinition());

            Button B = new Button();
            B.Height = 100;
            B.Width = 100;
            TextBlock TB = new TextBlock();
            TB.Text = "Hello World";
            TB.FontSize = 18;
            
            Image IMG = new Image();
            IMG.Source = new BitmapImage(new Uri(@"C:\Users\Admin\Downloads\Deli (2).jpg"));
            IMG.Height = 80;
            IMG.HorizontalAlignment = HorizontalAlignment.Center;

            Grid.SetRow(IMG, 0);
            G.Children.Add(IMG);
            Grid.SetRow(TB, 1);
            G.Children.Add(TB);
            B.Content = G;
            ug1.Children.Add(B);
            //g2.Children.Add(TB);
            B.Click += (sender, e) => { button_Click(sender, e, TB.Text); };






        }

        private void button_Click(object sender, RoutedEventArgs e, object abc)
        {

        }
    }
}
