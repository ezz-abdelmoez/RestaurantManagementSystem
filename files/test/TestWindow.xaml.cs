using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SalesManagement.files.test
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();

            
        }

        private void send_message(object sender, RoutedEventArgs e)
        {
            //@"/SalesManagement;component/icon.png
            WebBrowser1.Source = new Uri(@"/SalesManagement;component/files/js/test.js");
            WebBrowser1.InvokeScript("test", new Object[] { tbox.Text });
        }
    }

}
