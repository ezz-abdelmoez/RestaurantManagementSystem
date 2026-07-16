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
    /// Interaction logic for StoreController.xaml
    /// </summary>
    public partial class StoreController : UserControl
    {
        private MainCtr mainCtr;
        public StoreController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                en_leave_date.SelectedDate = DateTime.Now;

                showDeparts();

                showStoreItems("");

                showStoreAction();
            }), DispatcherPriority.Background);
        }

        private void showStoreAction()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (storeItemListview.SelectedIndex != -1)
                {
                    CommonMethods.data = new DataTable();

                    CommonMethods.query = " * ";
                    if (action_num_view_tb.Text.Length == 0)
                        CommonMethods.query = $"TOP {action_num_view_tb.Text} * ";

                    CommonMethods.data = DB.getData(
                        $"SELECT {CommonMethods.query} FROM StoreAction WHERE preb=0 and item_id={((DataRowView)storeItemListview.SelectedItem)["ID"]} " +
                        $"ORDER BY ID DESC");
                    storeActionListview.DataContext = CommonMethods.data.DefaultView;

                }
            }), DispatcherPriority.Background);
        }

        private void showDeparts()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.getStoreItem(ref search_store_item_combo);

                CommonMethods.getStoreItem(ref edit_store_item_combo);

                CommonMethods.getStoreItem(ref new_store_item_combo);
            }), DispatcherPriority.Background);
        }

        private void showStoreItems(string sender)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                tot_tb.Text = "";
                CommonMethods.query = "";
                if (search_tb.Text.Length > 0 && sender == "search_tb")
                    CommonMethods.query += $" AND StoreItem.item_name like '{search_tb.Text}%'";
                if (search_store_item_combo.SelectedIndex != -1)
                    CommonMethods.query += $" AND StoreItem.item_type like '{search_store_item_combo.SelectedValue}%'";

                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData("SELECT * FROM StoreItem WHERE ok=0 "
                                                + CommonMethods.query +
                                                " ORDER BY item_name");
                storeItemListview.DataContext = CommonMethods.data.DefaultView;

                storeItemListview.SelectedIndex = 0;
                tot_tb.Text = storeItemListview.Items.Count.ToString();
                if (tot_tb.Text == "0" || tot_tb.Text == ".")
                    tot_tb.Text = CommonMethods.data.Rows.Count.ToString();
            }), DispatcherPriority.Background);
        }

        private void  edit_item_store_image(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (storeItemListview.SelectedIndex == -1)
                    MessageBox.Show(LocalizedLangs.Instance["choose_from_list"]);
                else if (edit_name_tb.Text.Length == 0)
                    MessageBox.Show(LocalizedLangs.Instance["item_name"]);
                else if (edit_qty_tb.Text.Length == 0)
                    MessageBox.Show(LocalizedLangs.Instance["enter_qty"]);
                else if (edit_store_item_combo.Text.Length == 0)
                    MessageBox.Show(LocalizedLangs.Instance["choose_department"]);
                else
                {
                    if (edit_min_tb.Text.Length == 0)
                        edit_min_tb.Text = "0";
                    DB.Update($"UPDATE StoreItem SET item_name='{edit_name_tb.Text}', " +
                              $"qty={edit_qty_tb.Text}, item_type='{edit_store_item_combo.Text}', " +
                              $"min_qty={edit_min_tb.Text} WHERE ID={((DataRowView)storeItemListview.SelectedItem)["ID"]}");

                    DB.Insert($"INSERT INTO StoreAction(item_id, act_txt, remain, act_date)" +
                              $" VALUES({((DataRowView)storeItemListview.SelectedItem)["ID"]}, " +
                              $"'{LocalizedLangs.Instance["settlement"]}', {Convert.ToInt32(edit_qty_tb.Text)}, {DateTime.Now.Date.ToOADate()})");


                    MessageBox.Show(LocalizedLangs.Instance["done"]);
                    edit_qty_tb.Text = "";
                    edit_name_tb.Text = "";
                    edit_min_tb.Text = "";
                    showDeparts();
                    CommonMethods.id = storeItemListview.SelectedIndex;
                    showStoreItems("");
                    storeItemListview.SelectedIndex = CommonMethods.id;

                }
            }), DispatcherPriority.Background);
        }

        private void save_new_item_store_image(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (new_name_tb.Text.Length == 0)
                    MessageBox.Show(LocalizedLangs.Instance["item_name"]);
                else if (new_qty_tb.Text.Length == 0)
                    MessageBox.Show(LocalizedLangs.Instance["enter_qty"]);
                else if (new_price_tb.Text.Length == 0)
                    MessageBox.Show(LocalizedLangs.Instance["enter_price"]);
                else if (new_store_item_combo.Text.Length == 0)
                    MessageBox.Show(LocalizedLangs.Instance["choose_department"]);
                else
                {
                    if (new_min_tb.Text.Length == 0)
                        new_min_tb.Text = "0";

                    DB.Insert("INSERT INTO StoreItem(item_name, qty, item_type, min_qty, price) " +
                              $"VALUES('{new_name_tb.Text}', {new_qty_tb.Text}, " +
                              $"'{new_store_item_combo.Text}', {new_min_tb.Text}, {new_price_tb.Text}) ");
                    MessageBox.Show(LocalizedLangs.Instance["done"]);
                    new_name_tb.Text = "";
                    new_min_tb.Text = "";
                    new_qty_tb.Text = "";
                    new_price_tb.Text = "";

                    showDeparts();
                    showStoreItems("");
                    border_add_new_store_item.Visibility = Visibility.Collapsed;
                }
            }), DispatcherPriority.Background);
        }

        private void hide_new_store_item_border(object sender, MouseButtonEventArgs e)
        {
            border_add_new_store_item.Visibility = Visibility.Collapsed;
        }

        private void show_new_store_item_border(object sender, RoutedEventArgs e)
        {
            border_add_new_store_item.Visibility = Visibility.Visible;
        }

        private void search_store_item_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showStoreItems("");
        }

        private void search_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            showStoreItems("search_tb");
        }

        private void StoreItemListview_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            CommonMethods.dataRowView = ((sender as ListView).SelectedItem as DataRowView);
            if (CommonMethods.dataRowView != null)
            {
                edit_store_item_combo.SelectedValue = CommonMethods.dataRowView.Row["item_type"].ToString();
                edit_name_tb.Text = CommonMethods.dataRowView.Row["item_name"].ToString();
                edit_qty_tb.Text = CommonMethods.dataRowView.Row["qty"].ToString();
                edit_min_tb.Text = CommonMethods.dataRowView.Row["min_qty"].ToString();
                
                
                showStoreAction();
            }

        }

        private void register_enter_learve_btn(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (en_leave_qty_tb.Text.Length == 0)
                    MessageBox.Show(LocalizedLangs.Instance["enter_qty"]);
                else if (!en_leave_date.SelectedDate.HasValue)
                    MessageBox.Show(LocalizedLangs.Instance["date"]);
                else
                {
                    CommonMethods.query = "";
                    bool actDir = false;
                    if (en_leave_combo.SelectedIndex == 0)
                    {
                        actDir = true;
                        CommonMethods.query += $"UPDATE StoreItem SET qty=qty+{en_leave_qty_tb.Text} " +
                                               $"WHERE ID={((DataRowView)storeItemListview.SelectedItem)["ID"]}";
                    }
                    else
                    {
                        CommonMethods.query += $"UPDATE StoreItem SET qty=qty-{en_leave_qty_tb.Text} " +
                                               $"WHERE ID={((DataRowView)storeItemListview.SelectedItem)["ID"]}";
                    }

                    DB.Update(CommonMethods.query);
                    CommonMethods.query = "";
                    CommonMethods.totalQty = Convert.ToDouble(DB.getData($"SELECT qty FROM StoreItem WHERE ID=" +
                                                                         $"{((DataRowView)storeItemListview.SelectedItem)["ID"]}")
                        .Rows[0][0]);
                    DB.Insert("INSERT INTO StoreAction(item_id, act_txt, act_dir, act_qty, remain, act_date) " +
                              $"VALUES({((DataRowView)storeItemListview.SelectedItem)["ID"]}, " +
                              $"'{en_leave_combo.Text}', " +
                              $"{actDir} , {en_leave_qty_tb.Text},{CommonMethods.totalQty}, {en_leave_date.SelectedDate.Value.ToOADate()})");
                    CommonMethods.id = storeItemListview.SelectedIndex;
                    showStoreItems("");
                    storeItemListview.SelectedIndex = CommonMethods.id;
                    MessageBox.Show("Success");
                    //showStoreAction();
                }
            }), DispatcherPriority.Background);
        }

        private void delete_item_store_btn(object sender, MouseButtonEventArgs e)
        {
            if (CommonMethods.defaultDelete())
            {
                CommonMethods.id = (int)(((sender as Image).DataContext as DataRowView).Row["ID"]);
                DB.Delete($"DELETE * FROM StoreItem WHERE ID={CommonMethods.id}");
                DB.Delete($"DELETE * FROM StoreAction WHERE item_id={CommonMethods.id}");
                
                showDeparts();
                
                CommonMethods.id = storeItemListview.SelectedIndex;
                showStoreItems("");
                storeItemListview.SelectedIndex = CommonMethods.id;
            }
        }

        private void edit_qty_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }

        private void action_num_view_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void action_num_view_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            showStoreAction();
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.storeCtrl = null;
            mainCtr.showPage("store");
        }
    }
}
