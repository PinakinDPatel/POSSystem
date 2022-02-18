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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string number = (sender as Button).Content.ToString();

            string textBox1Str = txt.Text;
            if (textBox1Str != "")
            {
                textBox1Str = (Convert.ToDecimal(textBox1Str) * 100).ToString();
                textBox1Str= textBox1Str.Remove(textBox1Str.Length - 3);
            }
            txt.Text =(Convert.ToDecimal(textBox1Str + number)/100).ToString();
        }
    }
}
