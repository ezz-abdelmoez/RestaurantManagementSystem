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
using SalesManagement.files.classes;
using SalesManagement.Resources;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for SuppliersController.xaml
    /// </summary>
    public partial class SuppliersController : UserControl
    {
        private MainCtr mainCtr;
        public SuppliersController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;


            Dispatcher.BeginInvoke(new Action(() =>
            {
                month_tb.Text = DateTime.Now.Date.Month.ToString();
                year_tb.Text = DateTime.Now.Date.Year.ToString();


                pay_date.SelectedDate = DateTime.Now;
                CommonMethods.getVendors(ref vender_combo);

                //showAccounts();
                //showBills();
                //showBillsList();
            }), DispatcherPriority.Background);


        }

        private void showAccounts()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.data = new DataTable();
                if (vender_combo.SelectedIndex > -1)
                {
                    CommonMethods.query = $"SELECT * FROM Purchase WHERE vendor_id={vender_combo.SelectedValue} ";

                    if (day_tb.Text.Length > 0)
                        CommonMethods.query += $" AND DAY(buy_date)='{day_tb.Text}' ";
                    if (month_tb.Text.Length > 0)
                        CommonMethods.query += $" AND MONTH(buy_date)='{month_tb.Text}' ";
                    if (year_tb.Text.Length > 0)
                        CommonMethods.query += $" AND YEAR(buy_date)='{year_tb.Text}' ";

                    CommonMethods.data = DB.getData(CommonMethods.query);
                }

                accountsListview.DataContext = CommonMethods.data.DefaultView;
            }), DispatcherPriority.Background);
        }
        private void showBillsList()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.data = new DataTable();
                if (vender_combo.SelectedIndex > -1 && bill_combo.SelectedIndex > -1)
                {
                    CommonMethods.query = "";
                    if (bill_combo.SelectedIndex > -1)
                        CommonMethods.query = $"AND bill_no={bill_combo.SelectedValue}";

                    CommonMethods.data = DB.getData(
                        "SELECT StoreAction.ID, StoreItem.item_name, StoreAction.act_qty, " +
                        "StoreAction.buy_price, StoreAction.act_qty * StoreAction.buy_price as tot, " +
                        "StoreAction.mos, StoreAction.item_id FROM StoreAction INNER JOIN StoreItem " +
                        "ON StoreAction.item_id = StoreItem.ID WHERE preb=0 and vendor_id=" +
                        $"{vender_combo.SelectedValue} {CommonMethods.query} " +
                        $"ORDER BY StoreAction.ID");

                    CommonMethods.id = 1;
                    foreach (DataRow row in CommonMethods.data.Rows)
                        row["mos"] = CommonMethods.id++;


                }

                billsListview.DataContext = CommonMethods.data.DefaultView;
            }), DispatcherPriority.Background);
        }

        private void showBills()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.data = new DataTable();
                if (vender_combo.SelectedIndex > -1)
                {
                    CommonMethods.query = $"SELECT * FROM Purchase WHERE vendor_id={vender_combo.SelectedValue}" +
                                          $" AND bill_no > 0  ";
                    if (day_tb.Text.Length > 0)
                        CommonMethods.query += $" AND DAY(buy_date)='{day_tb.Text}' ";
                    if (month_tb.Text.Length > 0)
                        CommonMethods.query += $" AND MONTH(buy_date)='{month_tb.Text}' ";
                    if (year_tb.Text.Length > 0)
                        CommonMethods.query += $" AND YEAR(buy_date)='{year_tb.Text}' ";
                    CommonMethods.data = DB.getData(CommonMethods.query);

                }

                bill_combo.DataContext = CommonMethods.data.DefaultView;
                bill_combo.SelectedValuePath = "bill_no";
                bill_combo.DisplayMemberPath = "bill_no";
                bill_combo.SelectedIndex = 0;
            }), DispatcherPriority.Background);
        }

        private void show_new_sup_btn_click(object sender, RoutedEventArgs e)
        {
            border_new_sub.Visibility = Visibility.Visible;
        }

        private void hide_border_new_sup(object sender, RoutedEventArgs e)
        {
            border_new_sub.Visibility = Visibility.Collapsed;
        }

        private void save_new_sup_btn_click(object sender, RoutedEventArgs e)
        {
            if (new_sup_name_tb.Text.Length == 0)
                MessageBox.Show("Enter Supplier Name");
            else
            {
                DB.Insert("INSERT INTO Vendor(vendor, phone, email, address) " +
                          $"VALUES('{new_sup_name_tb.Text}', '{new_phone_tb.Text}', '{new_email_tb.Text}', '{new_address_tb.Text}')");

                CommonMethods.getVendors(ref vender_combo);

                new_sup_name_tb.Text = "";
                new_phone_tb.Text = "";
                new_address_tb.Text = "";
                new_email_tb.Text = "";
                border_new_sub.Visibility = Visibility.Collapsed;
            }
        }

        private void Bill_combo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bill_combo.SelectedIndex != -1)
            {
                bill_date.SelectedDate = Convert.ToDateTime((bill_combo.SelectedItem as DataRowView).Row["buy_date"]);
                showBillsList();
            }
        }

        private void Day_tb_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            showAccounts();
            showBills();
            showBillsList();
        }

        private void Vender_combo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vender_combo.SelectedIndex != -1)
            {
                vandor_amount_tb.Text = (vender_combo.SelectedItem as DataRowView).Row["amount"].ToString();
                showBills();
                showAccounts();
                showBillsList();
            }
        }

        private double buyValue;
        private void delete_account_image_md(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (CommonMethods.defaultDelete())
                {
                    CommonMethods.dataRowView = ((sender as Image).DataContext as DataRowView);
                    buyValue = Convert.ToDouble(CommonMethods.dataRowView.Row["buy_value"]);
                    if (Convert.ToInt32(CommonMethods.dataRowView.Row["bill_no"]) > 0)
                    {
                        buyValue *= -1;
                        CommonMethods.data = DB.getData(
                            $"SELECT * FROM StoreAction WHERE vendor_id={vender_combo.SelectedValue} " +
                            $"AND bill_no={CommonMethods.dataRowView.Row["bill_no"]}");
                        foreach (DataRow row in CommonMethods.data.Rows)
                        {
                            DB.Update($"UPDATE StoreItem SET qty=qty-{row["act_qty"]} WHERE ID={row["item_id"]}");
                            DB.Delete($"DELETE * FROM StoreAction WHERE ID={row["ID"]}");
                            DB.Update($"UPDATE StoreAction SET remain=remain-{row["act_qty"]} WHERE ID>{row["ID"]}");
                        }
                    }

                    DB.Update($"UPDATE Vendor SET amount=amount+{buyValue} WHERE ID={vender_combo.SelectedValue}");
                    DB.Delete($"DELETE * FROM Purchase WHERE ID={CommonMethods.dataRowView.Row["ID"]}");
                    DB.Update(
                        $"UPDATE Purchase SET remain=remain+{buyValue} WHERE vendor_id={vender_combo.SelectedValue} " +
                        $"AND ID={CommonMethods.dataRowView.Row["ID"]}");
                    MessageBox.Show("done");

                    CommonMethods.mos = vender_combo.SelectedIndex;
                    CommonMethods.getVendors(ref vender_combo);
                    vender_combo.SelectedIndex = CommonMethods.mos;

                    showAccounts();
                }
            }), DispatcherPriority.Background);
        }

        private void pay_btn_click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (pay_tb.Text.Length > 0)
                {
                    DB.Update($"UPDATE Vendor SET amount=amount-{pay_tb.Text}, " +
                              $"net_update=0 WHERE ID={vender_combo.SelectedValue}");
                    CommonMethods.remain =
                        Convert.ToDouble(DB.getData($"SELECT amount FROM Vendor " +
                                                    $"WHERE ID={vender_combo.SelectedValue}")
                            .Rows[0][0]);

                    DB.Insert(
                        $"INSERT INTO Purchase(vendor_id, bill_no, pay_or_add, buy_value, buy_date, remain, user_id) " +
                        $"VALUES({vender_combo.SelectedValue}, 0, '{LocalizedLangs.Instance["pay"]}', {pay_tb.Text}, " +
                        $"{pay_date.SelectedDate.Value.ToOADate()}, " +
                        $"{CommonMethods.remain}, {MainWindow.userId})");

                    DB.Update($"UPDATE Vendor SET amount={CommonMethods.remain} " +
                              $"WHERE ID={vender_combo.SelectedValue}");
                    // TODO:: m.safe
                    MainWindow.safe(false, Convert.ToDouble(pay_tb.Text), LocalizedLangs.Instance["sup_pay"],
                        vender_combo.Text, pay_date.SelectedDate.Value);
                    MessageBox.Show(LocalizedLangs.Instance["done"]);
                    CommonMethods.mos = vender_combo.SelectedIndex;
                    CommonMethods.getVendors(ref vender_combo);
                    vender_combo.SelectedIndex = CommonMethods.mos;
                    //showAccounts();
                }
            }), DispatcherPriority.Background);
        }

        private void day_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void pay_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.supCtrl = null;
            mainCtr.showPage("sup");
        }
    }

  
}
