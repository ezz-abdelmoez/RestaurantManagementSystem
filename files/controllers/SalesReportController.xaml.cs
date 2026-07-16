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
using SalesManagement.reports;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for SalesReportController.xaml
    /// </summary>
    public partial class SalesReportController : UserControl
    {
        private MainCtr mainCtr;
        public SalesReportController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                return_date_dp.SelectedDate = DateTime.Now;
                to_date_dp.SelectedDate = DateTime.Today.AddDays(1);
                //return_date_dp.SelectedDate = DateTime.Now;
                from_date_dp.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                CommonMethods.getCate(ref cate_combo);
                CommonMethods.getUsers(ref user_combo);
                user_combo.SelectedIndex = -1;

            }), DispatcherPriority.Background);
            

        }

     

        private void cate_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cate_combo.SelectedIndex > -1)
            {
                CommonMethods.getFood(ref item_combo, Convert.ToInt32(cate_combo.SelectedValue));
            }
            else
                CommonMethods.getFood(ref item_combo);
            showSales();
        }


        private void showConsumptions()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //MessageBox.Show(from_date_dp.SelectedDate.Value.ToOADate().ToString());
                consumedListview.DataContext = DB.getData(
                    "SELECT StoreItem.item_name, SUM(Consumption.qty) AS sum_qty, " +
                    "SUM(Consumption.price) AS sum_price FROM Consumption INNER JOIN " +
                    "StoreItem ON Consumption.store_item_id=StoreItem.ID " +
                    $"WHERE ((DateValue(Consumption.con_date)) BETWEEN {from_date_dp.SelectedDate.Value.ToOADate()} " +
                    $"AND {to_date_dp.SelectedDate.Value.ToOADate()}) " +
                    $"GROUP BY StoreItem.item_name ORDER BY StoreItem.item_name"
                ).DefaultView;
                //MessageBox.Show(consumedListview.Items.Count.ToString());
            }), DispatcherPriority.Background);
         
        }


        private void showSales()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                
                CommonMethods.totalPrice = 0;
                CommonMethods.totalQty = 0;
                CommonMethods.mos = 0;
                
                if (from_date_dp.SelectedDate.HasValue && to_date_dp.SelectedDate.HasValue)
                {
                    CommonMethods.name = ",Users.user_name ";
                    if (!user_name_checked.IsChecked.GetValueOrDefault())
                        CommonMethods.name = "";

                    CommonMethods.opDir = ",Sales.order_type ";
                    if (!type_checked.IsChecked.GetValueOrDefault())
                        CommonMethods.opDir = "";

                    CommonMethods.query = $"SELECT SUM(Sales.no_food) as qty, Food.food, SUM(Sales.price) AS total " +
                        $"{CommonMethods.name} {CommonMethods.opDir} FROM (Food INNER JOIN Sales ON " +
                        $"Food.ID = Sales.food_id) LEFT JOIN Users ON Sales.user_id=Users.ID " +
                        $"WHERE Sales.operation_date BETWEEN {from_date_dp.SelectedDate.Value.ToOADate()} " +
                        $"AND {to_date_dp.SelectedDate.Value.ToOADate()} ";


                    if (order_type_combo.SelectedIndex == 0)
                        CommonMethods.query += $" AND Sales.order_type LIKE '{LocalizedLangs.Instance["take_away"]}' ";
                    else if (order_type_combo.SelectedIndex == 1)
                        CommonMethods.query += $" AND Sales.order_type LIKE '{LocalizedLangs.Instance["hall"]}' ";
                    else if(order_type_combo.SelectedIndex == 2)
                        CommonMethods.query += $" AND Sales.order_type LIKE '{LocalizedLangs.Instance["delivery"]}' ";
                    else if(order_type_combo.SelectedIndex == 3)
                        CommonMethods.query += $" AND Sales.order_type LIKE '{LocalizedLangs.Instance["reservation"]}' ";

                    if (cate_combo.SelectedIndex > -1)
                        CommonMethods.query += $" AND Food.categ = {cate_combo.SelectedValue} ";
                    if (item_combo.SelectedIndex > -1)
                        CommonMethods.query += $" AND Food.ID = {item_combo.SelectedValue} ";
                    if (user_combo.SelectedIndex > -1)
                        CommonMethods.query += $" AND Sales.user_id={user_combo.SelectedValue} ";

                    CommonMethods.query += $" GROUP BY Food.food {CommonMethods.name} {CommonMethods.opDir} ";
                    if (food_name_radio.IsChecked.GetValueOrDefault())
                        CommonMethods.query += $" ORDER BY Food.food ";
                    else if (qty_radio.IsChecked.GetValueOrDefault())
                        CommonMethods.query += $" ORDER BY SUM(Sales.no_food) ";
                    else if (price_radio.IsChecked.GetValueOrDefault())
                        CommonMethods.query += $" ORDER BY SUM(Sales.price)";

                    if (desc_checked.IsChecked.GetValueOrDefault())
                        CommonMethods.query += " DESC ";


                    CommonMethods.data = new DataTable();
                    CommonMethods.data = DB.getData(CommonMethods.query);
                    salesListview.DataContext = CommonMethods.data.DefaultView;


                    foreach (DataRow row in CommonMethods.data.Rows)
                    {
                        CommonMethods.mos++;
                        CommonMethods.totalQty += Convert.ToDouble(row["qty"]);
                        CommonMethods.totalPrice += Convert.ToDouble(row["total"]);
                    }

                    mos_tb.Content = CommonMethods.mos.ToString();
                    tot_price_tb.Content = CommonMethods.totalPrice.ToString();
                    tot_qty_tb.Content = CommonMethods.totalQty.ToString();
                }
                else
                {
                    CommonMethods.data = new DataTable();
                    salesListview.DataContext = CommonMethods.data.DefaultView;
                }

            }), DispatcherPriority.Background);
            

            
            //MessageBox.Show(CommonMethods.data.Rows.Count.ToString());
        }

        private void user_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            showSales();

           
        }

        private void desc_checked_check(object sender, RoutedEventArgs e)
        {
            if(salesListview != null)
                showSales();
        }

        private void from_date_dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            showSales();
            if (consumedListview != null && from_date_dp.SelectedDate.HasValue
                && to_date_dp.SelectedDate.HasValue)
                showConsumptions();
        }

        #region ReturnsTab
        private void return_date_dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            showReturns();
        }

        private void return_order_num_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            showReturns();
        }

        private void showReturns()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                if (return_order_num_tb.Text.Length == 0)
                {
                    CommonMethods.data = new DataTable();
                    returnsListview.DataContext = CommonMethods.data.DefaultView;
                    tot_returns_price_tb.Text = "0";
                    return;
                }
                CommonMethods.query = "";
                if (return_order_num_tb.Text.Length > 0)
                    CommonMethods.query += $"WHERE Sales.order_no={return_order_num_tb.Text} ";
                else 
                    CommonMethods.query += " WHERE ";

                if(return_date_dp.SelectedDate.HasValue && return_order_num_tb.Text.Length > 0)
                    CommonMethods.query += " AND ";

                if (return_date_dp.SelectedDate.HasValue) 
                {
                    CommonMethods.query += $" YEAR(Sales.operation_date)={return_date_dp.SelectedDate.Value.Year} " +
                        $"AND MONTH(Sales.operation_date)={return_date_dp.SelectedDate.Value.Month} " +
                        $"AND DAY(Sales.operation_date)={return_date_dp.SelectedDate.Value.Day} ";

                }
                else
                {
                    CommonMethods.query += $"  DATE(Sales.operation_date)={DateTime.Now.Date.ToOADate()} ";
                }

                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData(
                        "SELECT Sales.ID, Sales.order_no, Food.food, Sales.price, Sales.no_food, Sales.food_id, Sales.food_size, " +
                        "Sales.preb_tim FROM Food INNER JOIN Sales ON Food.ID=Sales.food_id " +
                        CommonMethods.query
                        //$"AND YEAR(Sales.operation_date)={return_date_dp.SelectedDate.Value.Year} " +
                        //$"AND MONTH(Sales.operation_date)={return_date_dp.SelectedDate.Value.Month} " +
                        //$"AND DAY(Sales.operation_date)={return_date_dp.SelectedDate.Value.Day}"
                    );
                returnsListview.DataContext = CommonMethods.data.DefaultView;
                CommonMethods.totalPrice = 0;

                foreach (DataRow row in CommonMethods.data.Rows)
                    CommonMethods.totalPrice += Convert.ToDouble(row["price"]);
                tot_returns_price_tb.Text = CommonMethods.totalPrice.ToString();

            }), DispatcherPriority.Background);
            

        }
        #endregion

        private void return_items_btn_click(object sender, RoutedEventArgs e)
        {
            returnItems();
        }

       private void returnItems()
       {
           Dispatcher.Invoke(new Action(() =>
           {

               if (!return_date_dp.SelectedDate.HasValue)
                   MessageBox.Show(LocalizedLangs.Instance["enter_date"]);
               if (returnsListview.Items.Count == 0)
                   return;
               else if (CommonMethods.defaultDelete())
               {
                   for (int i = 0; i < returnsListview.Items.Count; i++)
                   {
                       returnsListview.SelectedIndex = i;
                       CommonMethods.dataRowView = returnsListview.SelectedItem as DataRowView;
                       DB.Delete($"DELETE * FROM Sales WHERE ID={CommonMethods.dataRowView.Row["ID"]}");

                       CommonMethods.data1 = new DataTable();
                       CommonMethods.data1 = DB.getData($"SELECT * FROM ItemStore " +
                                                        $"WHERE item_id={CommonMethods.dataRowView["food_id"]}");

                       foreach (DataRow row in CommonMethods.data1.Rows)
                       {
                           DB.Update($"UPDATE StoreItem SET " +
                                     $"qty=qty+{Convert.ToDouble(Convert.ToDouble(row["qty"]) * Convert.ToDouble(CommonMethods.dataRowView["no_food"]))} " +
                                     $"WHERE ID={row["store_id"]}");
                       }
                   }

                   MessageBox.Show(LocalizedLangs.Instance["done"]);
                   showReturns();
               }

           }), DispatcherPriority.Background);
            
        }

        private void return_tab_keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                returnItems();
        }

        private void return_order_num_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }


        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.salesReportCtrl = null;
            mainCtr.showPage("salesReport");
        }

        private ReportWindow reportWindow;
        private void print_sales_per_day_md(object sender, MouseButtonEventArgs e)
        {
            reportWindow = new ReportWindow();
            reportWindow.Show();
        }
    }
}
