using SalesManagement.files.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for OrdersController.xaml
    /// </summary>
    public partial class OrdersController : UserControl
    {
        short num = 0;
        private MainCtr mainCtr;

        public OrdersController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.getUsers(ref admin_combo);
                
                //DB.getData("SELECT user_name FROM Users WHERE ID=" + MainWindow.userId)
                //.Rows[0][0].ToString();
                day_tb.Text = DateTime.Now.Day.ToString();
                month_tb.Text = DateTime.Now.Month.ToString();
                year_tb.Text = DateTime.Now.Year.ToString();

                day_tb.TextChanged += day_tb_TextChanged;
                month_tb.TextChanged += day_tb_TextChanged;
                year_tb.TextChanged += day_tb_TextChanged;
                order_type_combo.SelectionChanged += order_type_combo_SelectionChanged;
                admin_combo.SelectionChanged += order_type_combo_SelectionChanged;

                admin_combo.SelectedIndex = 0;
                admin_combo.SelectedValue = MainWindow.userId;

                showMaxBills();
                showSummaryBills();

            }), DispatcherPriority.Background);
            
        }

        private void showMaxBills(string qu= "ID>0", bool flag=false, string mes="")
        {
            CommonMethods.query = $"SELECT MAX(order_no) as ord_no_max FROM Sales WHERE  {qu}";
            if (admin_combo.SelectedIndex > -1)
                CommonMethods.query += $" AND user_id={admin_combo.SelectedValue} ";
            if (order_type_combo.SelectedIndex > 0)
                CommonMethods.query += $" AND order_type_no={order_type_combo.SelectedIndex-1} ";
           

            if(day_tb.Text.Length > 0)
                CommonMethods.query += $" AND DAY(operation_date)='{day_tb.Text}' ";
            if (month_tb.Text.Length > 0)
                CommonMethods.query += $" AND MONTH(operation_date)='{month_tb.Text}' ";
            if (year_tb.Text.Length > 0)
                CommonMethods.query += $" AND YEAR(operation_date)='{year_tb.Text}' ";

            

            try
            {
                CommonMethods.billNo = Convert.ToInt32(DB.getData(CommonMethods.query).Rows[0][0]);
                max_order_tb.Text = CommonMethods.billNo.ToString();
                showBills();
            }
            catch
            {
                if (flag)
                {
                    MessageBox.Show(mes);
                }
                else
                {
                    CommonMethods.data1 = new DataTable();
                    ordersListview.DataContext = CommonMethods.data1.DefaultView;
                    max_order_tb.Text = "0";
                    tot_price_order_lb.Content = "0";

                    order_user_tb.Text = "";
                    ord_date_tb.Text = "";
                    order_type_tb.Text = "";
                }
            }
        }

        private void showBills()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.query = "SELECT Sales.*, Users.user_name, Food.food FROM " +
                                      "Food INNER JOIN (Sales INNER JOIN Users ON Users.ID = Sales.user_id) " +
                                      "ON Food.ID=Sales.food_id WHERE Sales.order_no=" + CommonMethods.billNo;
                if (day_tb.Text.Length > 0)
                    CommonMethods.query += $" AND DAY(operation_date)='{day_tb.Text}' ";
                if (month_tb.Text.Length > 0)
                    CommonMethods.query += $" AND MONTH(operation_date)='{month_tb.Text}' ";
                if (year_tb.Text.Length > 0)
                    CommonMethods.query += $" AND YEAR(operation_date)='{year_tb.Text}' ";

             
                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData(CommonMethods.query);
                order_no_combo.ItemsSource = CommonMethods.data.DefaultView;
                order_no_combo.SelectedValuePath = "ID";
                order_no_combo.DisplayMemberPath = "ID";

                ordersListview.DataContext = CommonMethods.data.DefaultView;
                CommonMethods.totalNumber = 0;
                foreach (DataRow row in CommonMethods.data.Rows)
                    CommonMethods.totalNumber += Convert.ToDouble(row["price"]);
                tot_price_order_lb.Content = CommonMethods.totalNumber.ToString();
                try
                {
                    order_user_tb.Text = CommonMethods.data.Rows[0]["user_name"].ToString();
                    order_type_tb.Text = CommonMethods.data.Rows[0]["order_type"].ToString();
                    ord_date_tb.Text = CommonMethods.data.Rows[0]["operation_date"].ToString();
                }catch{}

                ordersListview.SelectedIndex = 0;
            }), DispatcherPriority.Background);
            
        }

        private void showSummaryBills()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.query = "SELECT Sum(Sales.no_food) AS tot_qty, " +
                                      "Sum(Sales.price) AS tot_price, " +
                                      "Sales.order_no, Users.user_name, Users.user_name " +
                                      "FROM Users RIGHT JOIN (Food RIGHT JOIN Sales " +
                                      "ON Food.ID = Sales.food_id) ON Users.ID = Sales.user_id " +
                                      "WHERE Food.ok=0 ";
                if (day_tb.Text.Length > 0)
                    CommonMethods.query += $" AND DAY(operation_date)='{day_tb.Text}' ";
                if (month_tb.Text.Length > 0)
                    CommonMethods.query += $" AND MONTH(operation_date)='{month_tb.Text}' ";
                if (year_tb.Text.Length > 0)
                    CommonMethods.query += $" AND YEAR(operation_date)='{year_tb.Text}' ";

                if (admin_combo.SelectedIndex > -1)
                    CommonMethods.query += $" AND Users.ID={admin_combo.SelectedValue} ";
                if (order_type_combo.SelectedIndex > 0)
                    CommonMethods.query += $" AND Sales.order_type_no={order_type_combo.SelectedIndex-1} ";


                CommonMethods.query += "GROUP BY Sales.order_no, Users.user_name  ";
                summaryListView.DataContext = DB.getData(CommonMethods.query).DefaultView;

                CommonMethods.data = new DataTable();
            }), DispatcherPriority.Background);
        }

        private void get_order_no_less_img_md(object sender, MouseButtonEventArgs e)
        {
            showMaxBills($"order_no < {CommonMethods.billNo}", true, "No bills");
        }

        private void get_order_no_bigger_img_md(object sender, MouseButtonEventArgs e)
        {
            showMaxBills($"order_no > {CommonMethods.billNo}", true, "No bills");
        }

        private void day_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    showMaxBills();
                    showSummaryBills();
                }
                catch { }
            }), DispatcherPriority.Background);
            //if (++num > 3)
            //    showMaxBills();
            
        }


        

        private void order_type_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    showMaxBills();
            //}
            //catch { }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if(admin_combo != null)
                {
                    showMaxBills();
                    showSummaryBills();
                }
            }), DispatcherPriority.Background);
        }

        private void ordersListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ordersListview.SelectedIndex > 0)
            {
                CommonMethods.dataRowView = ordersListview.SelectedItem as DataRowView;
                order_user_tb.Text = CommonMethods.dataRowView.Row["user_name"].ToString();
                order_type_tb.Text = CommonMethods.dataRowView.Row["order_type"].ToString();
                ord_date_tb.Text = CommonMethods.dataRowView.Row["operation_date"].ToString();
                
            }
        }

        private void year_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void SummaryListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CommonMethods.billNo = Convert.ToInt32((summaryListView.SelectedItem as DataRowView).Row["order_no"]);
                //MessageBox.Show("bill no " + CommonMethods.billNo);
                showBills();
            }catch{}
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.orderCtrl = null;
            mainCtr.showPage("orders");
        }
    }

    
}
