using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using System.Windows.Media.Effects;

namespace POSSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DropShadowEffect newDropShadowEffect = new DropShadowEffect();
            newDropShadowEffect.BlurRadius = 7;
            newDropShadowEffect.Direction = 180;
            newDropShadowEffect.Opacity = 95;
            newDropShadowEffect.ShadowDepth = 8;
            for (int i = 0; i < 10; ++i)
            {
                Button button = new Button()
                {
                    Content = string.Format("Button for {0}", i),
                   
                    Tag = i
                };
                button.Foreground = new SolidColorBrush(Colors.LightGray);
                button.Background = new SolidColorBrush(Colors.Blue);
               button.Effect = new DropShadowEffect()
               { BlurRadius = 3, ShadowDepth = 10 };
                // button.Effect.add
                button.Click += new RoutedEventHandler(button_Click);
              
                this.grid.Children.Add(button);
               
            }
        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            
            Console.WriteLine(string.Format("You clicked on the {0}. button.", (sender as Button).Tag));
            int i = Convert.ToInt32((sender as Button).Tag);
            insert(i);
            DataTable dt = new DataTable();
           // dt = (sele)
           

            MessageBox.Show(string.Format("You clicked on the {0}. button.", (sender as Button).Tag));
        }
        void insert(int i)
        {
            

        }

    }

}
