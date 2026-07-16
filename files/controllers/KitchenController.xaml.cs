using SalesManagement.files.classes;
using SalesManagement.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    /// Interaction logic for KitchenController.xaml
    /// </summary>
    public partial class KitchenController : Window
    {
        private MainCtr mainCtr;
        public KitchenController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            showAll();
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            showAll();
        }

        private void showAll()
        {
            showOrders();
            Showitems();
            showWait();
        }

        private void showOrders()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                clearall();

                CommonMethods.flag = false;
                CommonMethods.mos = 0;

                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData("SELECT Sales.order_no, Sales.order_type, " +
                                                "FIRST(Sales.operation_date) AS FirstOfopdate, Sales.seen FROM Sales " +
                                                "GROUP BY Sales.order_no, Sales.order_type, Sales.kitch, Sales.seen " +
                                                "HAVING (Sales.kitch=0) ORDER BY Sales.order_no");
                orders_listview.DataContext = CommonMethods.data.DefaultView;

                foreach (DataRow row in CommonMethods.data.Rows)
                {
                    CommonMethods.boolean = Convert.ToBoolean(row["seen"]);
                    if (!CommonMethods.boolean && !CommonMethods.flag)
                    {
                        ordtyp.Content = (object)row["order_type"].ToString();
                        ordno.Content = (object)row["order_no"].ToString();
                        CommonMethods.dateTime = Convert.ToDateTime(row["FirstOfopdate"]);
                        ordate.Content = CommonMethods.dateTime.ToString("yyyy/MM/dd");
                        ordtime.Content = CommonMethods.dateTime.ToString("HH:mm");
                        showOrderItems(row["order_no"].ToString(), this.ordlist);
                        CommonMethods.flag = true;
                    }
                    else if (CommonMethods.mos == 0 && CommonMethods.boolean)
                    {
                        CommonMethods.mos = 1;
                        ord1typ.Content = (object)row["order_type"].ToString();
                        ord1no.Content = (object)row["order_no"].ToString();
                        CommonMethods.dateTime = Convert.ToDateTime(row["FirstOfopdate"]);
                        ord1date.Content = CommonMethods.dateTime.ToString("yyyy/MM/dd");
                        ord1time.Content = CommonMethods.dateTime.ToString("HH:mm");
                        showOrderItems(row["order_no"].ToString(), this.ord1list);
                    }
                    else if (CommonMethods.mos == 1 && CommonMethods.boolean)
                    {
                        CommonMethods.mos = 2;
                        ord2typ.Content = (object)row["order_type"].ToString();
                        ord2no.Content = (object)row["order_no"].ToString();
                        CommonMethods.dateTime = Convert.ToDateTime(row["FirstOfopdate"]);
                        ord2date.Content = CommonMethods.dateTime.ToString("yyyy/MM/dd");
                        ord2time.Content = CommonMethods.dateTime.ToString("HH:mm");
                        showOrderItems(row["order_no"].ToString(), this.ord2list);
                    }
                    else if (CommonMethods.mos == 2 && CommonMethods.boolean)
                    {
                        CommonMethods.mos = 3;
                        ord3typ.Content = (object)row["order_type"].ToString();
                        ord3no.Content = (object)row["order_no"].ToString();
                        CommonMethods.dateTime = Convert.ToDateTime(row["FirstOfopdate"]);
                        ord3date.Content = CommonMethods.dateTime.ToString("yyyy/MM/dd");
                        ord3time.Content = CommonMethods.dateTime.ToString("HH:mm");
                        showOrderItems(row["order_no"].ToString(), this.ord3list);
                    }
                }
            }), DispatcherPriority.Background);
        }


        private void showOrderItems(string order_num, ListView orderItemList)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    CommonMethods.data1 = new DataTable();
                    CommonMethods.data1 = DB.getData("SELECT Food.food, Sales.no_food FROM Sales INNER JOIN Food " +
                                                     "ON Sales.food_id=Food.ID WHERE (Sales.order_no=" + order_num +
                                                     ")");
                    orderItemList.DataContext = CommonMethods.data1.DefaultView;
                }
                catch
                {
                }
            }), DispatcherPriority.Background);
        }

        private void Showitems()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    CommonMethods.data = new DataTable();
                    CommonMethods.data = DB.getData("SELECT Food.food, SUM(Sales.no_food) AS SumOfnofood " +
                                                    "FROM Sales INNER JOIN Food ON Sales.food_id=Food.ID" +
                                                    " WHERE (Sales.kitch=0) GROUP BY Food.food");
                    itemlist.DataContext = CommonMethods.data.DefaultView;
                }
                catch
                {
                }
            }), DispatcherPriority.Background);
        }

        private void showWait()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    waitno.Content = DB.getData("SELECT COUNT(Sales.order_no) AS CountOforder_no " +
                                                "FROM Sales GROUP BY Sales.kitch, Sales.seen " +
                                                "HAVING Sales.kitch=0 AND Sales.seen=0");

                }
                catch
                {
                }
            }), DispatcherPriority.Background);
        }

        private void clearall()
        {
            waitno.Content = (object)"0";
            ordtyp.Content = (object)"";
            ord1typ.Content = (object)"";
            ord2typ.Content = (object)"";
            ord3typ.Content = (object)"";
            ordno.Content = (object)"";
            ord1no.Content = (object)"";
            ord2no.Content = (object)"";
            ord3no.Content = (object)"";
            ordate.Content = (object)"";
            ord1date.Content = (object)"";
            ord2date.Content = (object)"";
            ord3date.Content = (object)"";
            ordtime.Content = (object)"";
            ord1time.Content = (object)"";
            ord2time.Content = (object)"";
            ord3time.Content = (object)"";
            CommonMethods.data = new DataTable();
            ordlist.DataContext = CommonMethods.data.DefaultView;
            ord1list.DataContext = CommonMethods.data.DefaultView;
            ord2list.DataContext = CommonMethods.data.DefaultView;
            ord3list.DataContext = CommonMethods.data.DefaultView;
        }

        private void close_img_md(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void seen_img_md(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DB.Update($"UPDATE Sales SET seen=1 WHERE order_no={ordno.Content}");
                showAll();
                MessageBox.Show(LocalizedLangs.Instance["seen"]);
            }
            catch { }
        }

        private void makeSeen(string order_num)
        {
            if (order_num == "")
                return;
            try
            {
                DB.Update($"UPDATE Sales SET kitch=1 WHERE order_no={order_num}");
                showAll();
                MessageBox.Show(LocalizedLangs.Instance["done"]);
            }
            catch { }
        }

        private void kitch_img_md_2(object sender, MouseButtonEventArgs e)
        {
            makeSeen(ord2no.Content.ToString());
        }

        private void kitch_img_md_3(object sender, MouseButtonEventArgs e)
        {
            makeSeen(ord3no.Content.ToString());
        }

        private void kitch_img_md_1(object sender, MouseButtonEventArgs e)
        {
            makeSeen(ord1no.Content.ToString());
        }

        private void calc_open_img_md(object sender, MouseButtonEventArgs e)
        {
            Process.Start("calc.exe");
        }

        private void Hide_Img(object sender, MouseButtonEventArgs e)
        {
            WindowBasicControl.hideWin(this);
        }

        private void min_max_img(object sender, MouseButtonEventArgs e)
        {
            WindowBasicControl.minMaxWin(this);
        }

   
    }
}
