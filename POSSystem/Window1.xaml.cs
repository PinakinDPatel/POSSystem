using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Management;
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
            uniqueid();

        }

        private void uniqueid()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mos.Get();
            string motherBoard = "";
            foreach (ManagementObject mo in moc)
            {
                motherBoard = (string)mo["SerialNumber"];
            }
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            popup1.IsOpen = false;
            btn1.Visibility = Visibility.Visible;
            btn2.Visibility = Visibility.Hidden;
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            popup1.IsOpen = true;
            //popup1.Placement = 
            btn1.Visibility = Visibility.Hidden;
            btn2.Visibility = Visibility.Visible;
        }
        private void NumButton_Click(object sender,RoutedEventArgs e)
        {

        }
    }
}
