using SalesManagement.DataSets;
using SalesManagement.files.classes;
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
using SalesManagement.DataSets;
using System.Threading;
using System.Windows.Threading;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for SalesController.xaml
    /// </summary>
    public partial class SalesController : UserControl
    {
        private bool dotts = true;
        private bool gfocus = true;
        private Thread th1;
        //private MyPrinter printer;
        //private Printer1 printer1;
        private float discount = 0;

        delegate void init_funs_background(int state);

        private MainCtr mainCtr;

        public SalesController(MainCtr mainCtr)
        {
            InitializeComponent();

            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                init();
                showImgsCheck.Checked += show_imgs;
                showImgsCheck.Unchecked += hide_imgs;
                order_type_combo.SelectionChanged += order_type_combo_selectionChanged;
                order_type_combo.SelectedIndex = 0;
            }), DispatcherPriority.Background);

           
        }

        private void init()
        {
            showCates();
            showItems();
            CommonMethods.showDelv(ref dev_combo);
            CommonMethods.showClients(ref client_combo);

            showTables();
            tables_lv.SelectedIndex = 0;
            table_no_combo.SelectedIndex = 0;

            showPreSalesList();
            maxOrderNumber();
        }

        private void maxOrderNumber()
        {
            try
            {
                CommonMethods.orderNumber = Convert.ToInt32(
                    DB.getData("SELECT MAX(order_no) FROM Sales WHERE user_id="+MainWindow.userId
                    +$" AND DAY(operation_date)='{DateTime.Today.Day}' " +
                    $"AND MONTH(operation_date)='{DateTime.Today.Month}' " +
                    $"AND YEAR(operation_date)='{DateTime.Today.Year}' ").Rows[0][0].ToString()
                    );
                CommonMethods.orderNumber++;
            }
            catch(Exception e)
            {
                CommonMethods.orderNumber = 1;
            }
            CommonMethods.reportTxt = DB.getData("SELECT report_txt FROM Shift").Rows[0][0].ToString();
        }

        private void showPreSalesList()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                if (table_no_combo.SelectedIndex == -1)
                    table_no_combo.SelectedIndex = 0;
                CommonMethods.query = "SELECT Food.food, PreSales.price, PreSales.no_food, " +
                                      "PreSales.food_id, PreSales.food_size, PreSales.activ, PreSales.ID as preID " +
                                      "FROM Food INNER JOIN PreSales ON Food.[ID] = PreSales.[food_id] " +
                                      "WHERE PreSales.user_id=" + MainWindow.userId
                                      + " AND PreSales.order_type_no=" + order_type_combo.SelectedIndex;
                if (order_type_combo.SelectedIndex == 1)
                {
                    CommonMethods.query += $" AND PreSales.table_no={table_no_combo.SelectedValue} " +
                                           $"AND PreSales.chare_no={char_no.SelectedIndex + 1} ";
                }
                else
                {
                    CommonMethods.query += $" AND PreSales.table_no=0 " +
                                           $"AND PreSales.chare_no=0 ";
                }

                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData(CommonMethods.query +
                                                " ORDER BY PreSales.activ, PreSales.ID");
                presalelistview.ItemsSource = CommonMethods.data.DefaultView;
                CommonMethods.totalNumber = 0;
                foreach (DataRow row in CommonMethods.data.Rows)
                    CommonMethods.totalNumber += Convert.ToDouble(row["price"]);

                if ((bool)Dis10.IsChecked)
                    CommonMethods.totalNumber *= .9;
                else if ((bool)Dis20.IsChecked)
                    CommonMethods.totalNumber *= .8;
                else if ((bool)Dis25.IsChecked)
                    CommonMethods.totalNumber *= .75;
                else if ((bool)Dis50.IsChecked)
                    CommonMethods.totalNumber *= .5;


                final_tot_price_tb.Content = CommonMethods.totalNumber.ToString();
            }), DispatcherPriority.Background);
        }
        
        private void showCates()
        {
            cate_listbox.DataContext = CommonMethods.getCate(ref cate_combo).DefaultView;
            cate_listbox.SelectedValuePath = "ID";
            cate_listbox.DisplayMemberPath = "cate";
        }

        void showItems(int id=-1)
        {
            CommonMethods.data = new DataTable();
            CommonMethods.query = "SELECT Food.*, iif( ((Food.min_level > 0) and (Food.qty < Food.min_level)), 1, 0) as is_lack, " +
                "Categ.cate, Categ.order_no as cat_ord_no FROM Food INNER JOIN Categ ON " +
                "Food.categ = Categ.ID WHERE Food.ok=0 ";
            if (id > -1)
                CommonMethods.query += $" AND Food.categ = {cate_combo.SelectedValue} ";
            CommonMethods.data = DB.getData(CommonMethods.query+" ORDER BY Categ.order_no, Food.order_no");
            items_combo.DataContext = CommonMethods.data.DefaultView;
            items_combo.SelectedValuePath = "ID";
            items_combo.DisplayMemberPath = "food";
            foodListView.ItemsSource = CommonMethods.data.DefaultView;
        }

        void reloadItemsComboFood()
        {
            CommonMethods.data = new DataTable();
            CommonMethods.query = "SELECT Food.*, iif( ((Food.min_level > 0) and (Food.qty < Food.min_level)), 1, 0) as is_lack, " +
                "Categ.cate, Categ.order_no as cat_ord_no FROM Food INNER JOIN Categ ON " +
                "Food.categ = Categ.ID WHERE Food.ok=0 ";

            CommonMethods.data = DB.getData(CommonMethods.query + " ORDER BY Categ.order_no, Food.order_no");
            items_combo.DataContext = CommonMethods.data.DefaultView;
            items_combo.SelectedValuePath = "ID";
            items_combo.DisplayMemberPath = "food";
        }

        public void showTables()
        {
            CommonMethods.data1 = new DataTable();
            CommonMethods.data1 = DB.getData("SELECT * FROM Tables ORDER BY table_no");
            tables_lv.DataContext = CommonMethods.data1.DefaultView;
            table_no_combo.DataContext = CommonMethods.data1.DefaultView;
            table_no_combo.SelectedValuePath = "table_no";
            table_no_combo.DisplayMemberPath = "table_no";
            //tables_lv.SelectedIndex = 0;
            //table_no_combo.SelectedIndex = 0;
        }


     

        private void clientBtn_Click(object sender, RoutedEventArgs e)
        {
            showClientTab();
        }
        private void showClientTab()
        {
            orderBtn.Foreground = new SolidColorBrush(Colors.White);
            clientBtn.Foreground = new SolidColorBrush(Colors.Gold);
            orderFoodScrollViewer.Visibility = Visibility.Collapsed;
            clientBorderViewer.Visibility = Visibility.Visible;
        }

        private void orderBtn_Click(object sender, RoutedEventArgs e)
        {
            showOrderTab();
            
        }
        private void showOrderTab()
        {
            orderBtn.Foreground = new SolidColorBrush(Colors.Gold);
            clientBtn.Foreground = new SolidColorBrush(Colors.White);
            clientBorderViewer.Visibility = Visibility.Collapsed;
            orderFoodScrollViewer.Visibility = Visibility.Visible;
        }

        private void num_enter_btn_click(object sender, RoutedEventArgs e)
        {
            btnNO = (Button)sender;
            EnterNo(btnNO.Content.ToString(), sender);

            
        }
        Button btnNO;
        private void EnterNo(string number, object sender)
        {

            // when click on del btn 
            if (number == "del")
            {
                try
                {
                    if (total_qty_lb.Text.ToString() == "" || exist_qty_tb.Text.ToString().Trim() == "")
                    {
                        total_qty_lb.Text = "1";
                        exist_qty_tb.Text = total_qty_lb.Text.ToString();
                    }
                    else
                    {
                        exist_qty_tb.Text = exist_qty_tb.Text.Remove(exist_qty_tb.Text.Length - 1);
                        total_qty_lb.Text = exist_qty_tb.Text.ToString();
                    }
                }

                catch { }
            }
            else
            {
                try
                {
                    btnNO = (Button)sender;


                    if (total_qty_lb.Text.ToString().Trim() == "")
                    {
                        total_qty_lb.Text = number;
                        exist_qty_tb.Text = total_qty_lb.Text.ToString();
                    }
                    else
                    {
                        total_qty_lb.Text = exist_qty_tb.Text.ToString() + number;
                        exist_qty_tb.Text = total_qty_lb.Text.ToString();
                    }
                }
                catch { }
            }
        }

        int repeat = 0;
        private void number_enter_pad()
        {
            ++repeat;
            CommonMethods.id = (total_qty_lb.Text.Contains(".")) ? 4 : 6;
            // if (total_qty_lb.Text.ToString().Length > 4 && CommonMethods.query != "del")
            // {
            //     MessageBox.Show("Can not be more!");
            //     return;
            // }
            if (repeat >= 2)
            {
                repeat = 0;return;
            }
            switch (CommonMethods.query)
            {
                case "1":
                    total_qty_lb.Text += CommonMethods.query;
                    break;
                case "2":
                    total_qty_lb.Text += CommonMethods.query;
                    break;
                case "3":
                    total_qty_lb.Text += CommonMethods.query;
                    break;
                case "4":
                    total_qty_lb.Text += CommonMethods.query;
                    break;
                case "5":
                    total_qty_lb.Text += CommonMethods.query;
                    break;
                case "6":
                    total_qty_lb.Text += CommonMethods.query;
                    break;
                case "7":
                    total_qty_lb.Text += CommonMethods.query;

                    break;
                case "8":
                    total_qty_lb.Text += CommonMethods.query;
                    break;
                case "9":
                    total_qty_lb.Text += CommonMethods.query;
                    break;
                case "0":
                    total_qty_lb.Text += CommonMethods.query;
                    break;
                case ".":
                    if (!total_qty_lb.Text.Contains("."))
                    {
                        total_qty_lb.Text += ".";
                    }
                    break;
                case "del":
                    total_qty_lb.Text = "";
                    dotts = true;
                    break;
            }
            
        }

        private void cate_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            total_qty_lb.Text = "";
            if (items_combo != null && cate_combo.SelectedIndex > -1)
                showItems(Convert.ToInt32(cate_combo.SelectedValue));
            else if (items_combo != null)
                showItems();
        }

        private void show_all_food_btn_click(object sender, RoutedEventArgs e)
        {
            cate_listbox.UnselectAll();
            showItems();
            cate_combo.SelectedIndex = -1;
        }

        private void cate_listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //CommonMethods.id = Convert.ToInt32(((sender as ListBox).SelectedItem as DataRowView).Row["ID"]);
            CommonMethods.id = Convert.ToInt32((sender as ListBox).SelectedIndex);
            cate_combo.SelectedIndex = CommonMethods.id;
        }

        private void items_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (items_combo.SelectedIndex != -1)
            {
                exist_qty_tb.Text = (items_combo.SelectedItem as DataRowView).Row["qty"].ToString();
                price_tb.Text = (items_combo.SelectedItem as DataRowView).Row["price"].ToString();
                if (total_qty_lb.Text.Length > 0)
                {
                    total_price_tb.Text = (Convert.ToDouble((items_combo.SelectedItem as DataRowView).Row["price"]) *
                        Convert.ToDouble(total_qty_lb.Text)).ToString();
                }
            }
            else
                exist_qty_tb.Text = "";
        }

        private void foodListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void food_selected_btn_click(object sender, RoutedEventArgs e)
        {
            CommonMethods.dataRowView = (sender as Button).DataContext as DataRowView;
            items_combo.SelectedValue = CommonMethods.dataRowView["ID"];
            exist_qty_tb.Text = (items_combo.SelectedItem as DataRowView).Row["qty"].ToString();

            if (order_type_combo.SelectedIndex == 1 && table_no_combo.SelectedIndex == -1)
                MessageBox.Show("Choose Table Number");
            else
            {

                if (delv_value_tb.Text.Length == 0)
                {
                    double delv_value = addServ = Convert.ToDouble(DB.getData("SELECT add_serv FROM Shift").Rows[0][0].ToString());
                    delv_value_tb.Text = delv_value.ToString();
                }

                if (total_qty_lb.Text.Length == 0)
                {
                    total_qty_lb.Text = "1";
                    
                }
                total_price_tb.Text = (Convert.ToDouble(CommonMethods.dataRowView.Row["price"])*
                    Convert.ToDouble(total_qty_lb.Text)).ToString();

                if(order_type_combo.SelectedIndex != 2)
                    addServ = 0;

                if (order_type_combo.SelectedIndex != 1)
                {
                    
                    CommonMethods.data = new DataTable();
                    CommonMethods.data = DB.getData("SELECT ID FROM PreSales " +
                        $"WHERE activ=0 AND table_no=0 " +
                        $"AND order_type_no={order_type_combo.SelectedIndex} " +
                        $"AND user_id={MainWindow.userId} " +
                        $"AND food_id={CommonMethods.dataRowView["ID"]} ");
                    if (CommonMethods.data.Rows.Count >= 1)
                    {
                        DB.Update("UPDATE PreSales SET no_food=no_food+"+total_qty_lb.Text+
                            $", price=price+{CommonMethods.dataRowView["price"]} " +
                            $"WHERE ID={CommonMethods.data.Rows[0][0]}");
                    }
                    else
                    {
                        DB.Insert("INSERT INTO PreSales(no_food, food_id, operation_date, " +
                        "price, shift_no, order_type, preb_tim, report_txt, add_serv, user_id, " +
                        "table_no, chare_no, activ, order_type_no) " +
                        $"VALUES({total_qty_lb.Text}, {CommonMethods.dataRowView["ID"]}, " +
                        $"{DateTime.Now.ToOADate()}, {total_price_tb.Text}, 1, '{order_type_combo.Text}'," +
                        $"0, '{CommonMethods.reportTxt}', {addServ}, {MainWindow.userId}, " +
                        $"0, 0, " +
                        $"0, {order_type_combo.SelectedIndex})");
                    }
                   

                }
                else
                {
                    DB.Insert("INSERT INTO PreSales(no_food, food_id, operation_date, " +
                            "price, shift_no, order_type, preb_tim, report_txt, add_serv, user_id, " +
                            "table_no, chare_no, activ, order_type_no) " +
                            $"VALUES({total_qty_lb.Text}, {CommonMethods.dataRowView["ID"]}, " +
                            $"{DateTime.Now.ToOADate()}, {total_price_tb.Text}, 1, '{order_type_combo.Text}'," +
                            $"0, '', {addServ}, {MainWindow.userId}, " +
                            $"{table_no_combo.SelectedValue}, {char_no.SelectedIndex + 1}, " +
                            $"0, {order_type_combo.SelectedIndex})");
                }

                total_qty_lb.Text = "";
                barcode_search_tb.Text = "";
                if (order_type_combo.SelectedIndex == 1)
                {
                    DB.Update("UPDATE Tables SET used=1 WHERE table_no="
                        + table_no_combo.SelectedValue);
                    CommonMethods.id = table_no_combo.SelectedIndex;
                    showTables();
                    table_no_combo.SelectedIndex = CommonMethods.id;
                }
                    
                showPreSalesList();
                //maxOrderNumber();
            }
        }

        private void total_qty_lb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(price_tb.Text.Length > 0 && total_qty_lb.Text.Length > 0)
            {
                try
                {
                    CommonMethods.totalNumber = Convert.ToDouble(price_tb.Text) *
                        Convert.ToDouble(total_qty_lb.Text);
                    total_price_tb.Text = CommonMethods.totalNumber.ToString();
                }
                catch { }
            }
            else
            {
                total_price_tb.Text = "";
            }
        }

        private void order_type_combo_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sale_order_grid != null && dev_order_grid != null)
            {
                CommonMethods.id = (sender as ComboBox).SelectedIndex;
                if (CommonMethods.id > -1)
                {
                    if (CommonMethods.id == 1)
                    {
                        tables_lv_border.Visibility = Visibility;
                        sale_order_grid.Visibility = Visibility.Visible;
                        dev_order_grid.Visibility = Visibility.Collapsed;
                        dev_combo.Visibility = Visibility.Collapsed;
                    }
                    else if (CommonMethods.id == 2)
                    {
                        sale_order_grid.Visibility = Visibility.Collapsed;
                        dev_order_grid.Visibility = Visibility.Visible;

                        showClientTab();
                        tables_lv_border.Visibility = Visibility.Collapsed;

                        dev_combo.Visibility = Visibility.Visible;
                    }
                    else 
                    {
                        sale_order_grid.Visibility = Visibility.Collapsed;
                        dev_order_grid.Visibility = Visibility.Collapsed;

                        showOrderTab();
                        dev_combo.Visibility = Visibility.Collapsed;
                        tables_lv_border.Visibility = Visibility.Collapsed;

                    }

                }
            }
            if (presalelistview != null)
            {
                showPreSalesList();
            }
        }

        private void change_table_btn_click(object sender, RoutedEventArgs e)
        {
            
        }

        private void client_search_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //ValidationKeyPress.pay_previewTextInput(sender, e, 'p');
        }

        private void add_new_client_btn_click(object sender, RoutedEventArgs e)
        {
            if (client_name_tb.Text.Length == 0)
                MessageBox.Show("Enter Client's Name");
            else if (client_tel_tb.Text.Length == 0)
                MessageBox.Show("Enter Client's phone");
            else if (client_address_tb.Text.Length == 0)
                MessageBox.Show("Enter Client's Address");
            else
            {
                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData($"SELECT * FROM Client WHERE client=" +
                    $"'{client_name_tb.Text}'");
                if (CommonMethods.data.Rows.Count > 0)
                {
                    MessageBox.Show($"Client Name already exists ({client_name_tb.Text})");
                    return;
                }
                DB.Insert("INSERT INTO Client(client, tel, email, address) " +
                    $"VALUES('{client_name_tb.Text}', '{client_tel_tb.Text}', " +
                    $"'{client_email_tb.Text}', '{client_address_tb.Text}')");
                MessageBox.Show("Added Successfully");
                client_name_tb.Text = "";
                client_tel_tb.Text = "";
            }
        }

        private void update_client_details_btn_click(object sender, RoutedEventArgs e)
        {
            if (client_combo.SelectedIndex == -1)
                return;
            DB.Update($"UPDATE Client SET client='{client_name_tb.Text}', " +
                $"tel='{client_tel_tb.Text}', email='{client_email_tb.Text}', " +
                $"address='{client_address_tb.Text}' WHERE ID={client_combo.SelectedValue}");
            MessageBox.Show("Updated Successfully");
        }

        private void delete_client_btn_click(object sender, RoutedEventArgs e)
        {
            if (client_combo.SelectedIndex == -1)
                return;
            if (CommonMethods.defaultDelete())
            {
                DB.Delete($"DELETE * FROM Client WHERE ID={client_combo.SelectedValue}");
                
            }
        }

        private void client_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(client_name_tb != null && client_tel_tb != null && client_address_tb != null && client_email_tb != null)
            {
                if(client_combo.SelectedIndex != -1)
                {
                    client_name_tb.Text = (client_combo.SelectedItem as DataRowView).Row["client"].ToString();
                    client_tel_tb.Text = (client_combo.SelectedItem as DataRowView).Row["tel"].ToString();
                    client_address_tb.Text = (client_combo.SelectedItem as DataRowView).Row["address"].ToString();
                    client_email_tb.Text = (client_combo.SelectedItem as DataRowView).Row["email"].ToString();
                }
                else
                {
                    client_name_tb.Text = "";
                    client_tel_tb.Text = "";
                    client_address_tb.Text = "";
                    client_email_tb.Text = "";

                }
            }
        }

        private void table_no_combo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void client_search_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(client_combo != null)
            {
                if (client_search_tb.Text.Length > 0)
                {
                    CommonMethods.showClients(ref client_combo, client_search_tb.Text);
                    client_combo.SelectedIndex = 0;
                }else
                {
                    CommonMethods.showClients(ref client_combo);
                }
            }
        }

        private void new_order_del_presales_img_md(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                clear_presales_list();
            }), DispatcherPriority.Background);
        }

        private void clear_presales_list()
        {
            if (presalelistview.Items.Count == 0)
                return;
            try
            {
                for (int i = 0; i < presalelistview.Items.Count; i++)
                {
                    presalelistview.SelectedIndex = i;
                    DB.Delete($"DELETE * FROM PreSales WHERE ID={(presalelistview.SelectedItem as DataRowView).Row["preID"]}");
                }

                //showItems();
                showPreSalesList();
                //cate_combo.SelectedIndex = -1;
            }
            catch
            {

            }
        }

        private void delete_presales_list_items_img_md(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CommonMethods.id = Convert.ToInt32(((sender as Image).DataContext as DataRowView).Row["preID"]);
                DB.Delete("DELETE * FROM PreSales WHERE ID=" + CommonMethods.id);
                showPreSalesList();
                if (presalelistview.Items.Count == 0 && order_type_combo.SelectedIndex == 1)
                {
                    DB.Update($"UPDATE Tables SET used=0 WHERE table_no={table_no_combo.SelectedValue}");

                    CommonMethods.id = table_no_combo.SelectedIndex;
                    showTables();
                    table_no_combo.SelectedIndex = CommonMethods.id;
                    tables_lv.SelectedIndex = CommonMethods.id;
                }
            }catch{}
        }

        private void tables_lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //MessageBox.Show(e.);
            }
            catch { }
            if(table_no_combo != null && table_no_combo.SelectedIndex != tables_lv.SelectedIndex 
                )
            {
                table_no_combo.SelectedIndex = tables_lv.SelectedIndex;
                
                if (order_type_combo != null && order_type_combo.SelectedIndex != 1)
                    order_type_combo.SelectedIndex = 1;
                else if (presalelistview != null)
                    showPreSalesList();
            }
        }

        private void table_no_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tables_lv != null && tables_lv.SelectedIndex != table_no_combo.SelectedIndex)
            {
                tables_lv.SelectedIndex = table_no_combo.SelectedIndex;
            }
            
        }

        private void save_bill_img_click(object sender, MouseButtonEventArgs e)
        {
            if (presalelistview.Items.Count == 0)
                return;
            saveBill(true);
        }

        double addServ;
        private void saveBill(bool p)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (presalelistview.Items.Count == 0)
                    return;
                if (order_type_combo.SelectedIndex == 2 && dev_combo.SelectedIndex == -1)
                    MessageBox.Show("Choose Delivery");
                else if (order_type_combo.SelectedIndex == 2 && client_combo.SelectedIndex == -1)
                    MessageBox.Show("Choose Client");
                else
                {
                    if (order_type_combo.SelectedIndex == 2 || order_type_combo.SelectedIndex == 3)
                    {
                        CommonMethods.totalPrice = 0;
                        addServ = 0;
                        if (order_type_combo.SelectedIndex == 2)
                        {
                            double delv_value =
                                Convert.ToDouble(DB.getData("SELECT add_serv FROM Shift").Rows[0][0].ToString());
                            addServ = (delv_value_tb.Text.Length != 0)
                                ? Convert.ToDouble(delv_value_tb.Text)
                                : delv_value;
                        }

                        DB.Update(
                            $"UPDATE PreSales SET client={client_combo.SelectedValue}, address='{client_address_tb.Text}', " +
                            $"tel='{client_tel_tb.Text}', add_serv={addServ}, delivery_man={dev_combo.SelectedValue} " +
                            $"WHERE order_type_no={order_type_combo.SelectedIndex} AND table_no=0 " +
                            $"AND chare_no=0 AND user_id={MainWindow.userId} ");

                    }

                    CommonMethods.query =
                        $"UPDATE PreSales SET notes='{notes_tb.Text}', order_type='{order_type_combo.Text}', " +
                        $"discount=0 ";
                    if (order_type_combo.SelectedIndex != 1)
                        CommonMethods.query += $" , order_no={CommonMethods.orderNumber} ";
                    if (order_type_combo.SelectedIndex == 1)
                        DB.Update(CommonMethods.query +
                                  $"WHERE order_type_no={order_type_combo.SelectedIndex} AND table_no=0 " +
                                  $"AND chare_no=0 AND user_id={MainWindow.userId} ");


                    if (p)
                    {
                        //try
                        //{
                        //    printer = new MyPrinter(order_type_combo.SelectedIndex, (int)table_no_combo.SelectedValue, 
                        //        char_no.SelectedIndex+1, false);
                        //    printer.Run();
                        //    printer.Dispose();
                        //}
                        //catch (Exception ex)
                        //{
                        //    MessageBox.Show("First Print:- " + Environment.NewLine);
                        //}
                        try
                        {
                            CommonMethods.query = "SELECT Categ.print_place FROM PreSales " +
                                                  "INNER JOIN(Food INNER JOIN Categ ON Food.categ=Categ.ID) " +
                                                  $"ON PreSales.food_id=Food.ID WHERE " +
                                                  $"order_type_no={order_type_combo.SelectedIndex}  AND user_id={MainWindow.userId} ";
                            if (order_type_combo.SelectedIndex == 1)
                            {
                                CommonMethods.query += $"AND table_no ={table_no_combo.SelectedValue}" +
                                                       $" AND chare_no={char_no.SelectedIndex + 1}";
                            }

                            if (order_type_combo.SelectedIndex != 1)
                            {
                                CommonMethods.query += "AND table_no =0" +
                                                       $" AND chare_no=0";
                                CommonMethods.query += " AND PreSales.order_no= " + CommonMethods.orderNumber + " ";
                            }

                            CommonMethods.data = new DataTable();
                            CommonMethods.data = DB.getData(CommonMethods.query
                                                            + " GROUP BY Categ.print_place");
                            //foreach(DataRow row in CommonMethods.data.Rows)
                            //{
                            //    if(Convert.ToBoolean(DB.getData("SELECT print_one FROM PrintPlace WHERE ID="+
                            //        row[0]).Rows[0]))
                            //    {
                            //        printer1 = new Printer1(order_type_combo.SelectedIndex, (int)table_no_combo.SelectedValue,
                            //            char_no.SelectedIndex + 1, false, order_type_combo.SelectedIndex);
                            //        printer1.printerName = DB.getData("SELECT lan_printer FROM Printer").Rows[0].ToString();
                            //        printer1.Run();
                            //        printer1.Dispose();
                            //    }
                            //}

                        }
                        catch
                        {
                        }
                    }

                    if (order_type_combo.SelectedIndex == 1)
                    {
                        DB.Update(
                            $"UPDATE PreSales SET activ=1 WHERE (order_type_no={order_type_combo.SelectedIndex}) " +
                            $"AND (table_no={table_no_combo.SelectedValue}) AND (chare_no={char_no.SelectedIndex + 1}) AND " +
                            $"(user_id={MainWindow.userId})");
                    }
                    else
                    {
                        DB.Update(
                            $"UPDATE PreSales SET activ=1 WHERE (order_type_no={order_type_combo.SelectedIndex}) " +
                            $"AND (table_no=0) AND (chare_no=0) AND " +
                            $"(user_id={MainWindow.userId})");
                    }

                    notes_tb.Text = "";
                    if (order_type_combo.SelectedIndex != 1)
                    {
                        CommonMethods.data = new DataTable();
                        //CommonMethods.data = DB.getData("SELECT * FROM PreSales WHERE " +
                        //    $"order_type_no={order_type_combo.SelectedIndex} AND table_no={table_no_combo.SelectedValue} " +
                        //    $"AND chare_no={char_no.SelectedIndex+1} AND user_id={MainWindow.userId}");

                        CommonMethods.data = DB.getData("SELECT * FROM PreSales WHERE " +
                                                        $"order_type_no={order_type_combo.SelectedIndex} AND table_no=0 " +
                                                        $"AND chare_no=0 AND user_id={MainWindow.userId}");

                        CommonMethods.totalPrice = 0;
                        foreach (DataRow row in CommonMethods.data.Rows)
                        {
                            CommonMethods.totalPrice += Convert.ToDouble(row["price"]);
                            try
                            {
                                if (Convert.ToDouble(
                                        DB.getData("SELECT qty FROM Food WHERE ID=" + row["food_id"]).Rows[0][0])
                                    >= Convert.ToDouble(row["no_food"]))
                                    DB.Update("UPDATE Food SET qty=qty-" + row["no_food"] + " WHERE ID=" +
                                              row["food_id"]);
                            }
                            catch (Exception e)
                            {

                            }

                            //string str14 = (Convert.ToDouble(row1["price"]) * (double) (100 - num2) / 100.0).ToString();
                            // سعر التوصيل
                            CommonMethods.flag = false;
                            if (order_type_combo.SelectedIndex == 2)
                                CommonMethods.flag = true;

                            CommonMethods.name = "0";
                            CommonMethods.query = "0";
                            if (order_type_combo.SelectedIndex == 2)
                            {
                                CommonMethods.name = dev_combo.SelectedValue.ToString();
                                CommonMethods.query = client_combo.SelectedValue.ToString();
                            }

                            DB.Insert(
                                "INSERT INTO Sales(no_food, food_id, operation_date, price, shift_no, order_type, order_type_no, " +
                                "client, still_preb, food_size, preb_tim, order_no, user_id, discount, " +
                                "delivery_man,add_serv,  deliverd) " +
                                $"VALUES({row["no_food"]}, {row["food_id"]}, {DateTime.Now.ToOADate()}, {row["price"]}, " +
                                $"1, '{row["order_type"]}', {row["order_type_no"]}, {row["client"]}, 1, '{row["food_size"]}', " +
                                $"{row["preb_tim"]}, {CommonMethods.orderNumber}, {MainWindow.userId}, " +
                                $"{discount}, {CommonMethods.name},{addServ} ,{CommonMethods.flag})");

                            CommonMethods.id =
                                Convert.ToInt32(DB.getData("SELECT MAX(ID) FROM Sales").Rows[0][0].ToString());

                            CommonMethods.data1 = new DataTable();
                            CommonMethods.data1 = DB.getData(
                                "SELECT ItemStore.*, StoreItem.price FROM ItemStore INNER JOIN" +
                                " StoreItem ON ItemStore.store_id=StoreItem.ID " +
                                $" WHERE ItemStore.item_id={row["food_id"]}");

                            foreach (DataRow row1 in CommonMethods.data1.Rows)
                            {
                                DB.Update($"UPDATE StoreItem SET qty=qty-{row1["qty"]} " +
                                          $" WHERE ID={row1["store_id"]}");
                                try
                                {
                                    DB.Insert("INSERT INTO Consumption(sales_id, store_item_id, " +
                                              "qty, price, con_date) " +
                                              $"VALUES({CommonMethods.id}, {row1["store_id"]}, " +
                                              $"{row1["qty"]}, {row1["price"]}, {DateTime.Now.ToOADate()})");

                                }
                                catch
                                {
                                    MessageBox.Show("falied to insert (Consumptions)");
                                }

                            }
                        }

                        DB.Delete("DELETE * FROM PreSales " +
                                  $"WHERE order_type_no={order_type_combo.SelectedIndex} " +
                                  $"AND table_no=0 " +
                                  $"AND chare_no=0 " +
                                  $"AND user_id={MainWindow.userId}");
                        if (order_type_combo.SelectedIndex != 2)
                        {
                            DB.Update($"UPDATE Sales SET user_id={MainWindow.userId}, " +
                                      $"safe_shift_no={MainWindow.shiftNo} , " +
                                      $"safe_date_time={DateTime.Now.ToOADate()} WHERE order_no={CommonMethods.orderNumber} " +
                                      $"AND operation_date={DateTime.Now.ToOADate()} ");

                            DB.Update($"UPDATE Users SET safe=safe+{CommonMethods.totalPrice} " +
                                      $"WHERE ID={MainWindow.userId}");
                        }


                        client_combo.SelectedIndex = -1;
                        dev_combo.SelectedIndex = -1;
                    }

                    print(CommonMethods.orderNumber, Convert.ToInt32(order_type_combo.SelectedIndex.ToString()), true,
                        char_no.SelectedIndex + 1, Convert.ToInt32(table_no_combo.SelectedValue), 1);
                    maxOrderNumber();
                    showPreSalesList();
                    reloadItemsComboFood();
                    MessageBox.Show("Done");
                    discount = 0;
                    Dis0.IsChecked = true;
                }
            }), DispatcherPriority.Background);
        }

        private void print(int OrderNo, int OrderTypeToPrint, bool active,int chare_no, int table_no, int print_place)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                try
                {
                    DataSet1 ds = new DataSet1();
                    DataSets.DataSet1TableAdapters.OrderDeliveryTableAdapter aprintTble =
                        new DataSets.DataSet1TableAdapters.OrderDeliveryTableAdapter();

                    System.Drawing.Printing.PrintDocument pd =
                        new System.Drawing.Printing.PrintDocument();

                    decimal month = DateTime.Now.Month;
                    decimal day = DateTime.Now.Day;
                    decimal year = DateTime.Now.Year;


                    aprintTble.Fill(ds.OrderDelivery, OrderNo, month, day.ToString(), year);


                    if (OrderTypeToPrint == 0) // طباعه التيك اوي
                    {
                        if (CommonMethods.langCode == "ar-EG")
                        {
                            ReportOrder rpt = new ReportOrder();
                            rpt.SetDataSource(ds);
                            rpt.PrintOptions.PrinterName = pd.PrinterSettings.PrinterName;
                            rpt.PrintToPrinter(1, true, 0, 0);
                        }
                        else
                        {
                            ReportOrderEn rpt = new ReportOrderEn();
                            rpt.SetDataSource(ds);
                            rpt.PrintOptions.PrinterName = pd.PrinterSettings.PrinterName;
                            rpt.PrintToPrinter(1, true, 0, 0);
                        }
                    }
                    else if (OrderTypeToPrint == 1)
                    {
                        DataSets.DataSet1TableAdapters.OrderSala7TableAdapter Sala7Table
                            = new DataSets.DataSet1TableAdapters.OrderSala7TableAdapter();
                        Sala7Table.Fill(ds.OrderSala7, MainWindow.userId, active, chare_no, table_no, OrderTypeToPrint,
                            print_place, month, day.ToString(), year);
                        if (CommonMethods.langCode == "ar-EG")
                        {
                            ReportSala7 rpt = new ReportSala7();
                            rpt.SetDataSource(ds);
                            rpt.PrintOptions.PrinterName = pd.PrinterSettings.PrinterName;
                            rpt.PrintToPrinter(1, true, 0, 0);
                        }
                        else
                        {
                            ReportSala7En rpt = new ReportSala7En();
                            rpt.SetDataSource(ds);
                            rpt.PrintOptions.PrinterName = pd.PrinterSettings.PrinterName;
                            rpt.PrintToPrinter(1, true, 0, 0);
                        }
                    }
                    else if (OrderTypeToPrint == 2) // طباعه الدلفيري
                    {
                        if (CommonMethods.langCode == "ar-EG")
                        {
                            ReportDelivery rpt = new ReportDelivery();
                            rpt.SetDataSource(ds);
                            rpt.PrintOptions.PrinterName = pd.PrinterSettings.PrinterName;
                            rpt.PrintToPrinter(1, true, 0, 0);
                        }
                        else
                        {

                            ReportDeliveryEn rpt = new ReportDeliveryEn();
                            rpt.SetDataSource(ds);
                            rpt.PrintOptions.PrinterName = pd.PrinterSettings.PrinterName;
                            rpt.PrintToPrinter(1, true, 0, 0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }), DispatcherPriority.Background);
        }



        public void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
                saveBill(true);
            else if (e.Key == Key.F13)
                saveBill(false);
            else if (e.Key == Key.F11)
            {
                clear_presales_list();
            } else if (gfocus) {
                if (
                e.Key == Key.NumPad0 ||
                e.Key == Key.NumPad1 ||
                e.Key == Key.NumPad2 ||
                e.Key == Key.NumPad3 ||
                e.Key == Key.NumPad4 ||
                e.Key == Key.NumPad5 ||
                e.Key == Key.NumPad6 ||
                e.Key == Key.NumPad7 ||
                e.Key == Key.NumPad8 ||
                e.Key == Key.NumPad9
                    )
                {
                    CommonMethods.query = ((decimal)e.Key - 74).ToString();
                    //MessageBox.Show(CommonMethods.query);
                    number_enter_pad();
                   
                }
                else if (e.Key == Key.Back)
                {
                    CommonMethods.query = "del";
                    number_enter_pad();
                }
                CommonMethods.query = "";
            }
            else if (e.Key == Key.Right)
            {
                showOrderTab();
            }
            else if (e.Key == Key.Left)
            {
                showClientTab();
            }else if (e.Key == Key.Enter)
            {
                if (presalelistview.Items.Count == 0)
                    return;
                saveBill(true);
            }else if (e.Key == Key.Escape)
            {
                clear_presales_list();
            }
        }

        private void finish_table_btn_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                if (presalelistview.Items.Count == 0)
                    return;
                CommonMethods.mos = 0;
                if ((!this.Dis10.IsChecked.GetValueOrDefault() ? 0 : (this.Dis10.IsChecked.HasValue ? 1 : 0)) != 0)
                    CommonMethods.mos = 10;
                if ((!Dis20.IsChecked.GetValueOrDefault() ? 0 : (Dis20.IsChecked.HasValue ? 1 : 0)) != 0)
                    CommonMethods.mos = 20;
                if ((!Dis25.IsChecked.GetValueOrDefault() ? 0 : (Dis25.IsChecked.HasValue ? 1 : 0)) != 0)
                    CommonMethods.mos = 25;
                if ((!Dis50.IsChecked.GetValueOrDefault() ? 0 : (Dis50.IsChecked.HasValue ? 1 : 0)) != 0)
                    CommonMethods.mos = 50;

                maxOrderNumber();
                int chare_no = char_no.SelectedIndex + 1;
                int table_no = Convert.ToInt32(table_no_combo.SelectedValue.ToString());
                int order_type_no = Convert.ToInt32(order_type_combo.SelectedIndex.ToString());
                int print_place = 1;
                bool active = true;
                DB.Update($"UPDATE PreSales SET discount={CommonMethods.mos}, " +
                          $"activ = true, order_type='{order_type_combo.Text}', " +
                          $"order_no={CommonMethods.orderNumber}" +
                          $" WHERE table_no={table_no_combo.SelectedValue} and chare_no={chare_no} " +
                          $"and user_id={MainWindow.userId}");
                // هطبع هنا
                //  وانا بقول الطباعة مش شغالة ليه! 
                // .....
                print(CommonMethods.orderNumber, order_type_no, active, chare_no, table_no, print_place);





                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData(
                    $"SELECT * FROM PreSales WHERE table_no={table_no_combo.SelectedValue}" +
                    $" AND chare_no={chare_no} AND user_id={MainWindow.userId}");
                CommonMethods.totalPrice = 0;
                foreach (DataRow row in CommonMethods.data.Rows)
                {
                    CommonMethods.totalPrice += Convert.ToDouble(row["price"]);
                    if (Convert.ToInt32(DB.getData($"SELECT qty FROM Food WHERE ID={row["food_id"]}")
                            .Rows[0][0].ToString()) >= Convert.ToInt32(row["no_food"]))
                    {
                        DB.Update($"UPDATE Food SET qty=qty-{row["no_food"]} WHERE ID={row["food_id"]}");
                    }

                    CommonMethods.name = (order_type_combo.SelectedIndex != 2 || client_combo.SelectedIndex == -1)
                        ? "0"
                        : client_combo.SelectedValue.ToString();
                    DB.Insert(
                        "INSERT INTO Sales(no_food, food_id, operation_date, price, shift_no, order_type, order_type_no," +
                        "client, still_preb, food_size, preb_tim, order_no, user_id, discount) " +
                        $"VALUES({row["no_food"]}, {row["food_id"]}, {DateTime.Now.ToOADate()}, {row["price"]}, " +
                        $"{MainWindow.shiftNo}, '{row["order_type"]}', {row["order_type_no"]}, {CommonMethods.name}, 1, '{row["food_size"]}'," +
                        $"{row["preb_tim"]}, {CommonMethods.orderNumber}, {MainWindow.userId}, {row["discount"]})");

                    CommonMethods.id = Convert.ToInt32(DB.getData("SELECT MAX(ID) FROM Sales").Rows[0][0].ToString());
                    CommonMethods.data1 = new DataTable();
                    CommonMethods.data1 = DB.getData("SELECT ItemStore.*, StoreItem.price FROM ItemStore " +
                                                     "INNER JOIN StoreItem ON ItemStore.store_id=StoreItem.ID " +
                                                     $"WHERE ItemStore.item_id={row["food_id"]}");

                    foreach (DataRow row1 in CommonMethods.data1.Rows)
                    {
                        DB.Update(
                            $"UPDATE StoreItem SET qty=qty-{Convert.ToDouble(row["no_food"]) * Convert.ToDouble(row1["qty"])} " +
                            $"WHERE ID={row1["store_id"]}");

                        DB.Insert("INSERT INTO Consumption(sales_id, store_item_id, qty, con_date) " +
                                  $"VALUES({CommonMethods.id}, {row1["store_id"]}, {Convert.ToDouble(row["no_food"]) * Convert.ToDouble(row1["qty"])}, " +
                                  $"{DateTime.Now.ToOADate()})");
                    }
                }

                DB.Delete($"DELETE * FROM PreSales WHERE table_no={table_no_combo.SelectedValue} " +
                          $"AND chare_no={chare_no} AND user_id={MainWindow.userId}");

                DB.Update($"UPDATE Sales SET safe_shift_no=" +
                          $"{Convert.ToInt32(DB.getData($"SELECT shift_no FROM Users WHERE ID={MainWindow.userId}").Rows[0][0].ToString())} " +
                          $", safe_date_time=NOW() WHERE order_no={CommonMethods.orderNumber} " +
                          $"AND operation_date={DateTime.Now.ToOADate()}");

                DB.Update($"UPDATE Users SET safe=safe+{CommonMethods.totalPrice} " +
                          $"WHERE ID={MainWindow.userId}");

                DB.Update($"UPDATE Tables SET used=0 WHERE table_no={table_no_combo.SelectedValue}");

                CommonMethods.id = table_no_combo.SelectedIndex;
                showTables();
                table_no_combo.SelectedIndex = CommonMethods.id;
                tables_lv.SelectedIndex = CommonMethods.id;
                maxOrderNumber();
                showPreSalesList();

            }), DispatcherPriority.Background);
        }

        private void discount_checked(object sender, RoutedEventArgs e)
        {
            if (Dis0 != null && Dis10 != null && Dis20 != null && Dis25 != null && Dis50 != null)
            {
                resetcolor();
                CommonMethods.totalNumber = 0;
                for (int i = 0; i < presalelistview.Items.Count; i++)
                {
                    presalelistview.SelectedIndex = i;
                    CommonMethods.totalNumber +=
                        Convert.ToDouble((presalelistview.SelectedItem as DataRowView).Row["price"]);
                }
                if ((!Dis10.IsChecked.GetValueOrDefault() ? 0 : (Dis10.IsChecked.HasValue ? 1 : 0)) != 0)
                {
                    CommonMethods.totalNumber *= 0.9;
                    discount = 10;
                }
                if ((!Dis20.IsChecked.GetValueOrDefault() ? 0 : (Dis20.IsChecked.HasValue ? 1 : 0)) != 0)
                {
                    CommonMethods.totalNumber *= 0.8;
                    discount = 20;
                }
                if ((!Dis25.IsChecked.GetValueOrDefault() ? 0 : (Dis25.IsChecked.HasValue ? 1 : 0)) != 0)
                {
                    CommonMethods.totalNumber *= 0.75;
                    discount = 25;
                }
                if ((!Dis50.IsChecked.GetValueOrDefault() ? 0 : (Dis50.IsChecked.HasValue ? 1 : 0)) != 0)
                {
                    CommonMethods.totalNumber *= 0.5;
                    discount = 50;
                }
                final_tot_price_tb.Content = CommonMethods.totalNumber.ToString();

                ((System.Windows.Controls.Control)sender).BorderBrush = (Brush)new SolidColorBrush(Colors.Red);
                ((System.Windows.Controls.Control)sender).Foreground = (Brush)new SolidColorBrush(Colors.Red);
            }
        }

        private void resetcolor()
        {
            
            Dis0.Foreground = (Brush)new SolidColorBrush(Colors.Black);
            Dis10.Foreground = (Brush)new SolidColorBrush(Colors.Black);
            Dis20.Foreground = (Brush)new SolidColorBrush(Colors.Black);
            Dis25.Foreground = (Brush)new SolidColorBrush(Colors.Black);
            Dis50.Foreground = (Brush)new SolidColorBrush(Colors.Black);
            Dis0.BorderBrush = (Brush)new SolidColorBrush(Colors.Black);
            Dis10.BorderBrush = (Brush)new SolidColorBrush(Colors.Black);
            Dis20.BorderBrush = (Brush)new SolidColorBrush(Colors.Black);
            Dis25.BorderBrush = (Brush)new SolidColorBrush(Colors.Black);
            Dis50.BorderBrush = (Brush)new SolidColorBrush(Colors.Black);
            
        }

        private void price_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }

        private void client_search_tb_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void client_search_tb_GotFocus(object sender, RoutedEventArgs e)
        {
            gfocus = false;
        }

        private void client_search_tb_LostFocus(object sender, RoutedEventArgs e)
        {
            gfocus = true;
        }


        public static bool showItemImg = true;
        private void show_imgs(object sender, RoutedEventArgs e)
        {
            showItemImg = true;
            showItems();
        }

        
        private void hide_imgs(object sender, RoutedEventArgs e)
        {
            showItemImg = false;
            showItems();
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.salesCtrl = null;
            mainCtr.showPage("sales");
        }
    }

    
}
