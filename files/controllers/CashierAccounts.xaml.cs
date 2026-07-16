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
using System.Windows.Threading;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for CashierAccounts.xaml
    /// </summary>
    public partial class CashierAccounts : UserControl
    {
        private MainCtr mainCtr;
        public CashierAccounts(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.getUsers(ref user_combo);
                month_tb.Text = DateTime.Now.Month.ToString();
                year_tb.Text = DateTime.Now.Year.ToString();
            }), DispatcherPriority.Background);

        }

        private void showCashierAccounts()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.query = "SELECT USafe.*, Users.user_name FROM USafe INNER JOIN " +
                                      $"Users ON USafe.user_id=Users.ID WHERE YEAR(safe_date) = '{year_tb.Text}' ";
                if (month_tb.Text.Length > 0)
                    CommonMethods.query += $" AND MONTH(safe_date)='{month_tb.Text}' ";
                if (day_tb.Text.Length > 0)
                    CommonMethods.query += $" AND DAY(safe_date)='{day_tb.Text}' ";
                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData(CommonMethods.query + " ORDER BY safe_date DESC");
                accListview.ItemsSource = CommonMethods.data.DefaultView;

                CommonMethods.totalNumber = 0;
                CommonMethods.lack = 0;
                foreach (DataRow row in CommonMethods.data.Rows)
                {
                    CommonMethods.totalNumber += Convert.ToDouble(row["cash"]);
                    CommonMethods.lack += Convert.ToDouble(row["lack"]);
                }

                supply_tb.Text = CommonMethods.totalNumber.ToString();
                lack_tb.Text = CommonMethods.lack.ToString();
            }), DispatcherPriority.Background);
        }

        private void user_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (user_combo != null && user_combo.SelectedIndex != -1 && user_combo.Items.Count > 0)
                showCashierAccounts();
        }

        private void day_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (user_combo != null && user_combo.SelectedIndex > -1  && user_combo.Items.Count > 0)
                showCashierAccounts();
        }

        private void day_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
           mainCtr.casheierAccsCtrl = null;
           mainCtr.showPage("cashierAccs");
        }
    }

  
}
