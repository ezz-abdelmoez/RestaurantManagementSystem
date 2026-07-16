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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using SalesManagement.files.classes;

namespace SalesManagement.reports
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private SalesCR salerCr;

        public ReportWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
           
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            setSalesReport();
        }

        public void setSalesReport()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                reportViewer1.Owner = this;

                salerCr = new SalesCR();

                salerCr.Database.Tables["Sales"]
                    .SetDataSource(CommonMethods.getSalesReport());


                reportViewer1.ViewerCore.ReportSource = salerCr;

            }), DispatcherPriority.Background);

        }

    }
}
