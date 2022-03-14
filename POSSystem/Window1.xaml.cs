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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            btn1.Visibility = Visibility.Visible;
            btn2.Visibility = Visibility.Hidden;
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            btn1.Visibility = Visibility.Hidden;
            btn2.Visibility = Visibility.Visible;
        }
    }
}
