using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for SafeController.xaml
    /// </summary>
    public partial class SafeController : UserControl
    {
        private MainCtr mainCtr;
        public SafeController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                showBalance();
                month_tb.Text = DateTime.Now.Month.ToString();
                year_tb.Text = DateTime.Now.Year.ToString();
                sattling_date.SelectedDate = DateTime.Now;

                month_tb.TextChanged += Year_tb_OnTextChanged;
                year_tb.TextChanged += Year_tb_OnTextChanged;

                showSafeList();
            }), DispatcherPriority.Background);
        }

        private void showSafeList()
        {
            try
            {
                CommonMethods.query = $"SELECT * FROM Safe WHERE MONTH(p_date)='{month_tb.Text}' " +
                                      $"AND YEAR(p_date)='{year_tb.Text}' ";
                if (search_name_tb.Text.Length > 0)
                    CommonMethods.query += $" AND person like '{search_name_tb.Text}%' ";
                if (search_date_tb.Text.Length > 0)
                    CommonMethods.query += $" AND DAY(p_date) = '{search_date_tb.Text}' ";
                if (search_desc_tb.Text.Length > 0)
                    CommonMethods.query += $" AND discrption like '{search_desc_tb.Text}%' ";
                safelistview.DataContext = DB.getData(CommonMethods.query + " ORDER BY ID DESC").DefaultView;
            }
            catch
            {
                
            }
            
        }
        private void showBalance()
        {
            try
            {
                current_balance_tb.Text =
                    DB.getData("SELECT balance FROM SafeBalance ORDER BY ID DESC").Rows[0][0].ToString();
            }
            catch
            {
                current_balance_tb.Text = "0";
            }
            
            
        }


        private void settling_click_btn(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!sattling_date.SelectedDate.HasValue)
                    MessageBox.Show("Enter Data");
                else if (current_balance_enter_tb.Text.Length == 0)
                    MessageBox.Show("Enter Balance");
                else
                {
                    CommonMethods.balance =
                        Convert.ToDouble(DB.getData("SELECT balance FROM SafeBalance ORDER BY ID DESC").Rows[0][0]);
                    DB.Insert("INSERT INTO Safe(cash_in, discrption, state, person, bill_no, p_date, user_id) " +
                              $"VALUES({current_balance_enter_tb.Text}-{CommonMethods.balance}, 'Settlement', 'Settlement', 0, 0, '{sattling_date.SelectedDate.Value}'," +
                              $"{MainWindow.userId})");
                    CommonMethods.id = Convert.ToInt32(DB.getData("SELECT MAX(ID) FROM SafeBalance").Rows[0][0]);
                    DB.Update(
                        $"UPDATE SafeBalance SET balance={current_balance_enter_tb.Text} WHERE ID={CommonMethods.id}");
                    MessageBox.Show(LocalizedLangs.Instance["done"]);
                    current_balance_enter_tb.Text = "";

                    showBalance();
                    showSafeList();
                }
            }), DispatcherPriority.Background);
        }

        private void month_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void current_balance_enter_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }

        private void Year_tb_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (safelistview != null)
            {
                showSafeList();
            }
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.safeCtrl = null;
            mainCtr.showPage("safe");
        }
    }
}
