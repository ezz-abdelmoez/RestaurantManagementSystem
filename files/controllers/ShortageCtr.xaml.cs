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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SalesManagement.files.classes;
using SalesManagement.Resources;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for ShortageCtr.xaml
    /// </summary>
    public partial class ShortageCtr : UserControl
    {
        public ShortageCtr()
        {
            InitializeComponent();

            showShortsQty();
        }

        public void showShortsQty()
        {
            try
            {
                CommonMethods.data1 = new DataTable();
                CommonMethods.data1 = DB.getData("SELECT * FROM Food WHERE (qty < min_level)");
                shortsListView.DataContext = CommonMethods.data1.DefaultView;
            }
            catch
            {

            }
        }
    }
}
