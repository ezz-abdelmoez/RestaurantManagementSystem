using SalesManagement.files.classes;
using SalesManagement.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for DeliveryOrdersController.xaml
    /// </summary>
    public partial class DeliveryOrdersController : UserControl
    {
        private MainCtr mainCtr;
        public DeliveryOrdersController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.showDelv(ref delv_combo);
                delv_combo.SelectedIndex = 0;
                showDelvBills();
            }), DispatcherPriority.Background);
            
        }

        private void showDelvBills()
        {
            if (delv_combo.SelectedIndex == -1)
                return;
            CommonMethods.data = new DataTable();
            CommonMethods.data = DB.getData("SELECT Client.client, Sales.operation_date, Sales.order_no, Sales.food_id as fid," +
                " SUM(Sales.price) as tot FROM Sales INNER JOIN Client ON Sales.client=Client.ID " +
                "WHERE Sales.delivery_man="+delv_combo.SelectedValue+" and Sales.deliverd=true " +
                "GROUP BY Client.client, Sales.order_no, Sales.operation_date, Sales.food_id " +
                "ORDER BY Sales.order_no");
            //MessageBox.Show(CommonMethods.data.Rows.Count.ToString());
            ordDevListview.DataContext = CommonMethods.data.DefaultView;
            CommonMethods.totalNumber = 0;
            foreach (DataRow row in CommonMethods.data.Rows)
                CommonMethods.totalNumber += Convert.ToDouble(row["tot"]);
            tot_dev_order_price_tb.Text = CommonMethods.totalNumber.ToString();
        }

        private void delv_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                showDelvBills();
            }), DispatcherPriority.Background);
           
        }

        private void pay_all_delv_btn_click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                if (ordDevListview.Items.Count == 0)
                    return;
                if (CommonMethods.defaultDelete())
                {
                    for(int i = 0; i < ordDevListview.Items.Count; i++)
                    {
                        ordDevListview.SelectedIndex = i;
                        CommonMethods.dataRowView = ordDevListview.SelectedItem as DataRowView;
                        CommonMethods.dateTime = Convert.ToDateTime(CommonMethods.dataRowView.Row["operation_date"]);

                        DB.Update($"UPDATE Sales SET deliverd=false, safe_shift_no={MainWindow.shiftNo}, " +
                                  $"safe_date_time={DateTime.Now.ToOADate()}, user_id={MainWindow.userId} " +
                                  $"WHERE order_no={CommonMethods.dataRowView.Row["order_no"]} AND" +
                                  $" DAY(operation_date)={CommonMethods.dateTime.Day} AND " +
                                  $" MONTH(operation_date)={CommonMethods.dateTime.Month} AND" +
                                  $" YEAR(operation_date)={CommonMethods.dateTime.Year} ");

                        DB.Update($"UPDATE Users SET safe=safe+{CommonMethods.dataRowView.Row["tot"]} " +
                                  $"WHERE ID={MainWindow.userId}");

                    
                    }
                    showDelvBills();
                }

            }), DispatcherPriority.Background);
            
        }

        private void pay_delv_bill_img_md(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                if (CommonMethods.defaultDelete(LocalizedLangs.Instance["pay"]))
                {
                    CommonMethods.dataRowView = (sender as Image).DataContext as DataRowView;
                    CommonMethods.dateTime = Convert.ToDateTime(CommonMethods.dataRowView.Row["operation_date"]);

                    DB.Update($"UPDATE Sales SET deliverd=false, safe_shift_no={MainWindow.shiftNo}, " +
                              $"safe_date_time={DateTime.Now.ToOADate()}, user_id={MainWindow.userId} " +
                              $"WHERE order_no={CommonMethods.dataRowView.Row["order_no"]} AND " +
                              $" food_id={CommonMethods.dataRowView.Row["fid"]} AND" +
                              $" DAY(operation_date)={CommonMethods.dateTime.Day} AND " +
                              $" MONTH(operation_date)={CommonMethods.dateTime.Month} AND" +
                              $" YEAR(operation_date)={CommonMethods.dateTime.Year} ");

                    DB.Update($"UPDATE Users SET safe=safe+{CommonMethods.dataRowView.Row["tot"]} " +
                              $"WHERE ID={MainWindow.userId}");

                    showDelvBills();
                }

            }), DispatcherPriority.Background);
            
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.delvOrderCtrl = null;
            mainCtr.showPage("delvOrders");
        }
    }
    

 
}
