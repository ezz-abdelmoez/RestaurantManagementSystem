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
    /// Interaction logic for CashierController.xaml
    /// </summary>
    public partial class CashierController : UserControl
    {
        private MainCtr mainCtr;
        public CashierController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.getUsers(ref username_combo);
                month_tb.Text = DateTime.Now.Month.ToString();
                year_tb.Text = DateTime.Now.Year.ToString();
                user_values_tb.Text = (username_combo.SelectedItem as DataRowView)
                    .Row["safe"].ToString();
            }), DispatcherPriority.Background);
        }

        private void showCashierDetailsList()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.data = new DataTable();
                if (year_tb.Text.Length < 4)
                {
                    CommonMethods.data = DB.getData($"SELECT TOP 10 * FROM USafe WHERE " +
                                                    $"user_id={username_combo.SelectedValue} ORDER BY ID DESC ");
                }
                else
                {
                    CommonMethods.query = $"SELECT * FROM USafe WHERE user_id={username_combo.SelectedValue} " +
                                          $"AND YEAR(safe_date)='{year_tb.Text}' ";
                    if (month_tb.Text.Length > 0)
                        CommonMethods.query += $" AND MONTH(safe_date)='{month_tb.Text}' ";
                    if (day_tb.Text.Length > 0)
                        CommonMethods.query += $" AND DAY(safe_date)='{day_tb.Text}' ";
                    if (safe_comobo.SelectedIndex == 1)
                        CommonMethods.query += $" AND det like 'Supply' ";
                    if (safe_comobo.SelectedIndex == 2)
                        CommonMethods.query += $" AND det like 'Settlement' ";
                    CommonMethods.data = DB.getData(CommonMethods.query + " ORDER BY ID DESC ");
                }

                safeListview.ItemsSource = CommonMethods.data.DefaultView;
                safeListview.SelectedIndex = 0;
                CommonMethods.totalNumber = 0;
                CommonMethods.lack = 0;
                foreach (DataRow row in CommonMethods.data.Rows)
                {
                    if (row["det"].ToString() == "Supply")
                        CommonMethods.totalNumber += Convert.ToDouble(row["cash"]);
                    CommonMethods.lack += Convert.ToDouble(row["lack"]);
                }

                cash_tb.Text = CommonMethods.totalNumber.ToString();
                lack_tb.Text = CommonMethods.lack.ToString();

            }), DispatcherPriority.Background);
        }

        private void Username_combo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (username_combo.SelectedIndex > -1)
            {
                user_values_tb.Text = (username_combo.SelectedItem as DataRowView).Row["safe"].ToString();
                showCashierDetailsList();
            }
        }

        private void Day_tb_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (username_combo.SelectedIndex > -1 && username_combo.Items.Count > 0)
            {
                showCashierDetailsList();
            }
        }

        private void Safe_comobo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (username_combo.SelectedIndex > -1 && username_combo.Items.Count > 0)
            {
                showCashierDetailsList();
            }
        }

        private void settle_click_btn(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (lack_enter_tb.Text.Length > 0 && supply_enter_tb.Text.Length == 0)
                    MessageBox.Show("Enter supply");
                else
                {
                    if (supply_enter_tb.Text.Length > 0)
                    {
                        CommonMethods.totalNumber =
                            Convert.ToDouble(supply_enter_tb.Text);
                        CommonMethods.lack = 0;
                        if (lack_enter_tb.Text.Length > 0)
                            CommonMethods.lack = Convert.ToDouble(lack_enter_tb.Text);
                        CommonMethods.totalPrice =
                            Convert.ToDouble((username_combo.SelectedItem as DataRowView).Row["safe"]) -
                            CommonMethods.totalNumber - CommonMethods.lack;
                        DB.Update(
                            $"UPDATE Users SET safe={CommonMethods.totalPrice} WHERE ID={username_combo.SelectedValue}");
                        DB.Insert($"INSERT INTO USafe(cash, remain, det, user_id, safe_date, lack) " +
                                  $"VALUES({CommonMethods.totalNumber}, {CommonMethods.totalPrice}," +
                                  $" '{LocalizedLangs.Instance["supply"]}', {MainWindow.userId}, {DateTime.Now.ToOADate()}, {CommonMethods.lack})");
                        if (CommonMethods.totalNumber > 0)
                        {
                            DB.getData("SELECT MAX(ID) FROM USafe");
                            //todo:: m.safe
                            MainWindow.safe(true, CommonMethods.totalNumber, LocalizedLangs.Instance["cashier"],
                                LocalizedLangs.Instance["supply"], DateTime.Now);
                        }
                    }

                    if (remain_enter_tb.Text.Length > 0)
                    {
                        CommonMethods.remain = Convert.ToDouble(remain_enter_tb.Text);
                        CommonMethods.totalNumber = CommonMethods.remain -
                                                    Convert.ToDouble(
                                                        (username_combo.SelectedItem as DataRowView).Row["safe"]);
                        DB.Update(
                            $"UPDATE Users SET safe={CommonMethods.remain} WHERE ID={username_combo.SelectedValue}");
                        DB.Insert("INSERT INTO USafe(cash, remain, det, user_id, safe_date) " +
                                  $"VALUES({CommonMethods.totalNumber}, {CommonMethods.remain}, 'Settlement', {MainWindow.userId}, " +
                                  $"{DateTime.Now.ToOADate()})");
                    }

                    supply_enter_tb.Text = "";
                    remain_enter_tb.Text = "";
                    lack_enter_tb.Text = "";
                    showCashierDetailsList();
                    CommonMethods.getUsers(ref username_combo);
                    MessageBox.Show("Settlement Done");
                }
            }), DispatcherPriority.Background);
        }

        private void supply_enter_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }

        private void day_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.cashierCtrl = null;
            mainCtr.showPage("cashier");
        }
    }
}
