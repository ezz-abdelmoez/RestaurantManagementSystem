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
    /// Interaction logic for ExpensesController.xaml
    /// </summary>
    public partial class ExpensesController : UserControl
    {
        private MainCtr mainCtr;
        public ExpensesController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                spend_date.SelectedDate = DateTime.Now;
                CommonMethods.setDate(ref month_tb, ref year_tb);

                day_tb.TextChanged += day_tb_TextChanged;
                month_tb.TextChanged += day_tb_TextChanged;
                year_tb.TextChanged += day_tb_TextChanged;

                isAdmin();
                showSpends();
            }), DispatcherPriority.Background);
        } 

        private void showSpends()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.query = $"SELECT * FROM Spending WHERE " +
                                      $"(YEAR(spend_date) = '{year_tb.Text}') AND " +
                                      $"(MONTH(spend_date) = '{month_tb.Text}') ";
                if (day_tb.Text.Length > 0)
                    CommonMethods.query += $" AND DAY(spend_date)='{day_tb.Text}' ";
                if (search_tb.Text.Length > 0)
                    CommonMethods.query += $" AND discrption like '{search_tb.Text}%'";
                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData(CommonMethods.query + " ORDER BY spend_date");
                spendListview.ItemsSource = CommonMethods.data.DefaultView;
                CommonMethods.totalNumber = 0;
                foreach (DataRow row in CommonMethods.data.Rows)
                    CommonMethods.totalNumber += Convert.ToDouble(row["cash"]);
                tot_spend_tb.Text = CommonMethods.totalNumber.ToString();
            }), DispatcherPriority.Background);
        }
        
        private void isAdmin()
        {
            if (Convert.ToBoolean(
                DB.getData($"SELECT is_admin FROM Users WHERE ID={MainWindow.userId}").Rows[0][0]))
                return;
            day_tb.Text = DateTime.Now.Day.ToString();
            day_tb.IsEnabled = false;
            month_tb.IsEnabled = false;
            year_tb.IsEnabled = false;
            spend_date.IsEnabled = false;
        }

        private void day_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (spendListview != null)
            {
                showSpends();
            }
        }

        private void add_new_spend_img_md(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!spend_date.SelectedDate.HasValue)
                    MessageBox.Show("Enter Date");
                else if (spend_des_tb.Text.Length == 0)
                    MessageBox.Show("Enter Spending's Description");
                else if (spend_value_tb.Text.Length == 0)
                    MessageBox.Show("Enter Spending's Value");
                else
                {
                    DB.Insert("INSERT INTO Spending (cash, discrption, notes, user_id, spend_date) " +
                              $" VALUES({spend_value_tb.Text}, '{spend_des_tb.Text}', '{notes_tb.Text}', " +
                              $"{MainWindow.userId}, {spend_date.SelectedDate.Value.ToOADate()})");

                    CommonMethods.billNo = Convert.ToInt32(
                        DB.getData("SELECT MAX(ID) FROM Spending").Rows[0][0]);


                    DB.Insert("INSERT INTO Safe(cash_in, discrption, state, bill_no," +
                              " p_date, user_id, person) " +
                              $"VALUES({spend_value_tb.Text}, 'Spendings', " +
                              $"'Purchase', {CommonMethods.billNo}, {spend_date.SelectedDate.Value.ToOADate()}," +
                              $"{MainWindow.userId}, " +
                              $"'{DB.getData($"SELECT user_name FROM Users WHERE ID={MainWindow.userId}").Rows[0][0]}')");

                    DB.Update($"UPDATE SafeBalance SET balance=balance-{Convert.ToDouble(spend_value_tb.Text)}");
                    try
                    {
                        //CommonMethods.balance = Convert.ToDouble(
                        //    DB.getData("SELECT balance FROM SafeBalance").Rows[0][0])
                        //    - Convert.ToDouble(spend_value_tb.Text);



                    }
                    catch
                    {

                    }

                    MessageBox.Show("Saved");
                    showSpends();


                }
            }), DispatcherPriority.Background);
        }

        private void delete_spend_image_md(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (CommonMethods.defaultDelete())
                {
                    CommonMethods.dataRowView = ((sender as Image).DataContext as DataRowView);
                    CommonMethods.totalPrice = Convert.ToDouble(
                        CommonMethods.dataRowView.Row["cash"]);
                    CommonMethods.id =
                        Convert.ToInt32(CommonMethods.dataRowView.Row["ID"]);

                    DB.Delete($"DELETE * FROM Spending WHERE ID={CommonMethods.id}");
                    DB.Delete($"DELETE * FROM Safe WHERE bill_no={CommonMethods.id}");
                    DB.Update($"UPDATE SafeBalance SET balance =balance+{CommonMethods.totalPrice}");
                    showSpends();
                }
            }), DispatcherPriority.Background);
        }

        private void value_key_down(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter){
                CommonMethods.dataRowView = ((sender as TextBox).DataContext as DataRowView);
                CommonMethods.id =
                    Convert.ToInt32(CommonMethods.dataRowView.Row["ID"]);
                DB.Update($"UPDATE Spending SET cash={(sender as TextBox).Text} WHERE ID={CommonMethods.id}");
            } 
        }

        private void desc_key_down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommonMethods.dataRowView = ((sender as TextBox).DataContext as DataRowView);
                CommonMethods.id =
                    Convert.ToInt32(CommonMethods.dataRowView.Row["ID"]);
                DB.Update($"UPDATE Spending SET discrption='{(sender as TextBox).Text}' WHERE ID={CommonMethods.id}");
            }
        }

        private void notes_key_down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommonMethods.dataRowView = ((sender as TextBox).DataContext as DataRowView);
                CommonMethods.id =
                    Convert.ToInt32(CommonMethods.dataRowView.Row["ID"]);
                DB.Update($"UPDATE Spending SET notes='{(sender as TextBox).Text}' WHERE ID={CommonMethods.id}");
            }
        }

        private void date_changes(object sender, SelectionChangedEventArgs e)
        {
            CommonMethods.dataRowView = ((sender as DatePicker).DataContext as DataRowView);
            CommonMethods.id =
                Convert.ToInt32(CommonMethods.dataRowView.Row["ID"]);
            DB.Update($"UPDATE Spending SET spend_date='{(sender as DatePicker).Text}' WHERE ID={CommonMethods.id}");

        }

        private void spend_value_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }

        private void day_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.expensesCtrl = null;
            mainCtr.showPage("expenses");
        }
    }
}
