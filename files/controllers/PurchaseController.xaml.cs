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
    /// Interaction logic for PurchaseController.xaml
    /// </summary>
    public partial class PurchaseController : UserControl
    {
        private MainCtr mainCtr;
        public PurchaseController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                pay_date.SelectedDate = DateTime.Now;

                CommonMethods.getVendors(ref vendor_combo);
            
                CommonMethods.getStoreItemAll(ref store_item_combo);
                store_item_combo.SelectedIndex = 0;

            }), DispatcherPriority.Background);
            
            
            
        }

      

        private void showPurchases()
        {
            DB.getData("SELECT StoreAction.ID, StoreItem.item_name, StoreAction.act_qty, StoreAction.buy_price, " +
                       "");
        }

        private void showActions()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.totalNumber = 0;
                CommonMethods.totalQty = 0;
                CommonMethods.totalPrice = 0;
                CommonMethods.mos = 1;

                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData(
                    $"SELECT StoreAction.ID, StoreItem.item_name, StoreAction.act_qty, " +
                    "StoreAction.buy_price, StoreAction.act_qty * StoreAction.buy_price AS tot, " +
                    "StoreAction.mos, StoreAction.item_id FROM StoreAction INNER JOIN StoreItem ON " +
                    $" StoreAction.item_id = StoreItem.ID WHERE StoreAction.preb=true AND StoreAction.vendor_id={vendor_combo.SelectedValue}"
                    + " ORDER BY StoreAction.ID"
                );


                foreach (DataRow row in CommonMethods.data.Rows)
                {
                    CommonMethods.totalPrice += Convert.ToDouble(row["tot"]);
                    CommonMethods.totalQty += Convert.ToDouble(row["act_qty"]);
                    row["mos"] = CommonMethods.mos++;
                }

                num_tb.Text = CommonMethods.data.Rows.Count.ToString();
                tot_price_tb.Text = CommonMethods.totalPrice.ToString();
                tot_qty_tb.Text = CommonMethods.totalQty.ToString();
                purchaseListview.ItemsSource = CommonMethods.data.DefaultView;

            }), DispatcherPriority.Background);
        }

        private void Store_item_combo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (store_item_combo.SelectedIndex != -1)
            {
                buy_price_tb.Text = (store_item_combo.SelectedItem as DataRowView)["price"].ToString();
            }
        }

        private void Qty_tb_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (qty_tb.Text.Length == 0)
            {
                total_tb.Text = "";
            }
            else
            {
                try
                {
                    total_tb.Text = (Convert.ToDouble(qty_tb.Text) * Convert.ToDouble(buy_price_tb.Text)).ToString();
                }
                catch
                {
                }
            }
        }

        private void add_purchase_btn_click(object sender, RoutedEventArgs e)
        {
            if (store_item_combo.SelectedIndex == -1)
                MessageBox.Show(LocalizedLangs.Instance["choose_categ"]);
            else if (qty_tb.Text.Length == 0)
                MessageBox.Show(LocalizedLangs.Instance["enter_qty"]);
            else
            {
                DB.Insert($"INSERT INTO StoreAction(vendor_id, item_id, act_qty, buy_price, preb) " +
                          $"VALUES({vendor_combo.SelectedValue}, {store_item_combo.SelectedValue},{qty_tb.Text}, {buy_price_tb.Text}, 1)");
                //MessageBox.Show("Added");
                showActions();
            }
        }

        private void save_purchase_btn_click(object sender, RoutedEventArgs e)
        {
            if (purchaseListview.Items.Count > 0) {
                try
                {
                    saveBills(false);
                }
                catch { }
            }
        }

        private void saveBills(bool re)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                if (!pay_date.SelectedDate.HasValue)
                    MessageBox.Show("Enter Payment Date");
                else
                {
                    if (purchaseListview.Items.Count > 0)
                    {
                        // to add bill number assume id as bill number
                        CommonMethods.billNo = 1;
                        try
                        {
                            CommonMethods.billNo +=
                                Convert.ToInt32(DB.getData("SELECT MAX(bill_no) FROM StoreAction").Rows[0][0]);
                        }
                        catch
                        {
                        }

                        //CommonMethods.data = new DataTable();
                        //CommonMethods.data = (purchaseListview.ItemsSource as DataTable);
                        CommonMethods.totalVendorValue = 0;
                        foreach (DataRowView row in purchaseListview.Items)
                        {
                            CommonMethods.totalQty = Convert.ToDouble(row["act_qty"]);
                            CommonMethods.opDir = LocalizedLangs.Instance["enter"];
                            if (re)
                            {
                                CommonMethods.totalQty *= -1;
                                CommonMethods.opDir = LocalizedLangs.Instance["return"];
                            }

                            DB.Update($"UPDATE StoreItem SET qty=qty+{CommonMethods.totalQty}, " +
                                      $"price={row["buy_price"]} WHERE ID={row["item_id"]}");
                            DB.Update($"UPDATE StoreAction SET preb=0, " +
                                      $"remain={DB.getData($"SELECT qty FROM StoreItem WHERE ID={row["item_id"]}").Rows[0][0]}, " +
                                      $"act_date={pay_date.SelectedDate.Value.ToOADate()}, bill_no={CommonMethods.billNo}, " +
                                      $"act_txt='{CommonMethods.opDir}', act_dir={re} WHERE ID={row["ID"]}");
                            CommonMethods.totalVendorValue +=
                                Convert.ToDouble(row["act_qty"]) * Convert.ToDouble(row["buy_price"]);
                        }

                        DB.Update(
                            $"UPDATE Vendor SET amount=amount+{CommonMethods.totalVendorValue} WHERE ID={vendor_combo.SelectedValue}");
                        CommonMethods.opDir = LocalizedLangs.Instance["bill"];
                        if (re)
                            CommonMethods.opDir = LocalizedLangs.Instance["return"];
                        CommonMethods.remain =
                            Convert.ToDouble(DB
                                .getData($"SELECT amount FROM Vendor WHERE ID={vendor_combo.SelectedValue}")
                                .Rows[0][0]);
                        DB.Insert(
                            $"INSERT INTO Purchase(vendor_id, bill_no, pay_or_add, buy_value, buy_date, remain, user_id) " +
                            $"VALUES({vendor_combo.SelectedValue}, {CommonMethods.billNo}, '{CommonMethods.opDir}', {CommonMethods.totalVendorValue}, " +
                            $"{pay_date.SelectedDate.Value.ToOADate()}," +
                            $"{CommonMethods.remain}," +
                            $"{MainWindow.userId})");

                    }

                    if (pay_tb.Text.Length > 0)
                    {
                        DB.Update(
                            $"UPDATE Vendor SET amount=amount-{pay_tb.Text}, net_update=0 WHERE ID={vendor_combo.SelectedValue}");
                        CommonMethods.totalVendorValue =
                            Convert.ToDouble(DB
                                .getData($"SELECT amount FROM Vendor WHERE ID={vendor_combo.SelectedValue}")
                                .Rows[0][0]);
                        DB.Insert(
                            "INSERT INTO Purchase(vendor_id, bill_no, pay_or_add, buy_value, buy_date, remain, user_id) " +
                            $"VALUES({vendor_combo.SelectedValue}, 0, '{LocalizedLangs.Instance["pay"]}', {pay_tb.Text}, {pay_date.SelectedDate.Value.ToOADate()}, " +
                            $"{CommonMethods.totalVendorValue}, {MainWindow.userId})");

                        // TODO:: M.SAFE IN MAIN WINDOW
                        MainWindow.safe(false, Convert.ToDouble(pay_tb.Text), LocalizedLangs.Instance["sup_pay"],
                            vendor_combo.Text, pay_date.SelectedDate.Value);
                    }

                    MessageBox.Show(LocalizedLangs.Instance["done"]);

                    pay_tb.Text = "";

                    CommonMethods.id = vendor_combo.SelectedIndex;
                    CommonMethods.getVendors(ref vendor_combo);
                    vendor_combo.SelectedIndex = CommonMethods.id;


                    tot_price_tb.Text = "0";

                }

            }), DispatcherPriority.Background);
        }

        private void Vendor_combo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vendor_combo.SelectedIndex != -1)
            {
                vendor_value_tb.Text = ((DataRowView)vendor_combo.SelectedItem)["amount"].ToString();
                showActions();
            }
        }

        private void delete_store_item_image_md(object sender, MouseButtonEventArgs e)
        {
            if (CommonMethods.defaultDelete())
            {
                
                CommonMethods.id = Convert.ToInt32(((sender as Image).DataContext as DataRowView).Row["ID"]);
                DB.Delete($"DELETE * FROM StoreAction WHERE ID={CommonMethods.id}");
                showActions();
            }
        }

        private void return_purchase_btn_click(object sender, RoutedEventArgs e)
        {
            if (purchaseListview.Items.Count > 0)
            {
                try
                {
                    saveBills(true);
                }
                catch { }
            }
        }

        private void qty_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.purchaseCtrl = null;
            mainCtr.showPage("purchase");
        }
    }
}
