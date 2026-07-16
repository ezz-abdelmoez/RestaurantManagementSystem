using SalesManagement.files.classes;
using SalesManagement.files.windows;
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
    /// Interaction logic for EmployeeController.xaml
    /// </summary>
    public partial class EmployeeController : UserControl
    {
        private PopupWindow popWin;
        private MainCtr mainCtr;
        public EmployeeController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                new_date_hiring_dp.SelectedDate = DateTime.Now.Date;
                spend_date_dp.SelectedDate = DateTime.Now.Date;

                // showJobs();
                // showEmp("");
            }), DispatcherPriority.Background);
        }

        // show jobs (eccups) for employees
        private void showJobs()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData("SELECT eccup FROM Emp GROUP BY eccup HAVING eccup is not null " +
                                                "ORDER BY eccup");
                jobs_combo.ItemsSource = CommonMethods.data.DefaultView;
                jobs_combo.SelectedValuePath = "eccup";
                jobs_combo.DisplayMemberPath = "eccup";
                new_eccup_combo.ItemsSource = CommonMethods.data.DefaultView;
                new_eccup_combo.SelectedValuePath = "eccup";
                new_eccup_combo.DisplayMemberPath = "eccup";
            }), DispatcherPriority.Background);
        }

        // show employees listview
        public DataTable showEmp(string s, bool flag = true)
        {

            CommonMethods.query = "SELECT * FROM Emp WHERE deleted=0 ";
            if (jobs_combo.SelectedIndex != -1 && flag)
                CommonMethods.query += $" AND eccup like '{jobs_combo.SelectedValue}' ";
            if (search_tb.Text.Length > 0 && s == "search_tb")
                CommonMethods.query += $" AND emp like '{search_tb.Text}%' ";
            else
                search_tb.Text = "";

            CommonMethods.data = new DataTable();
            CommonMethods.data = DB.getData(CommonMethods.query + " ORDER BY emp");
            empListview.ItemsSource = CommonMethods.data.DefaultView;

            CommonMethods.mos = 0;
            CommonMethods.lack = 0;
            CommonMethods.totalNumber = 0;

            foreach(DataRow row in CommonMethods.data.Rows)
            {
                row["mos"] = CommonMethods.mos+=1;
                CommonMethods.totalNumber += Convert.ToDouble(row["ratb"]);
                CommonMethods.lack += Convert.ToDouble(row["solfa"]);
            }
            tot_salary_tb.Text = CommonMethods.totalNumber.ToString();
            tot_borrow_tb.Text = CommonMethods.lack.ToString();
            num_tb.Text = CommonMethods.mos.ToString();
            return CommonMethods.data;
        }

        private void add_employee_img_mouse_down(object sender, MouseButtonEventArgs e)
        {
            //showPopup((UIElement)new AddEmpController(), "Add New Employee");
            new_emp_border.Visibility = Visibility.Visible;
        }

        private void showPopup(UIElement element, string title, float width=0)
        {
            popWin = new PopupWindow();
            popWin.Title1.Content = title;
            if(width != 0)
                popWin.Width = width;
            popWin.popupMainGrid.Children.Add(element);
            popWin.ShowDialog();
        }

        private void emp_spends_img_mouse_down(object sender, MouseButtonEventArgs e)
        {
            borrow_border.Visibility = Visibility.Visible;
            spend_year_tb.Text = DateTime.Now.Year.ToString();
            spend_month_tb.Text = DateTime.Now.Month.ToString();
            showSpendEmp();
            showSpendList();
        }

        private void showSpendEmp()
        {
            borrow_emp_combo.ItemsSource = showEmp("", false).DefaultView;
            borrow_emp_combo.SelectedValuePath = "ID";
            borrow_emp_combo.DisplayMemberPath = "emp";
            borrow_emp_combo.SelectedIndex = 0;
        }

        private void showSpendList()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData("SELECT * FROM Spending WHERE " +
                                                $"YEAR(spend_date)='{spend_year_tb.Text}' " +
                                                $"AND MONTH(spend_date)='{spend_month_tb.Text}' " +
                                                $"AND emp_id = {borrow_emp_combo.SelectedValue} ORDER BY spend_date");
                spendListview.ItemsSource = CommonMethods.data.DefaultView;
                CommonMethods.totalNumber = 0;
                foreach (DataRow row in CommonMethods.data.Rows)
                    CommonMethods.totalNumber += Convert.ToDouble(row[1]);
                tot_borrow_tb.Text = CommonMethods.totalNumber.ToString();
            }), DispatcherPriority.Background);
        }

        private void emp_holidays_mouse_down(object sender, MouseButtonEventArgs e)
        {
            showPopup((UIElement)new HolidaysController(this), "Holidays");
        }

        private void jobs_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (jobs_combo != null)
                showEmp("", true);
        }

        private void search_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (search_tb != null && search_tb.Text.Length > 0)
                showEmp("search_tb"); 
        }

        private void hide_new_emp_ui_click(object sender, RoutedEventArgs e)
        {
            new_emp_border.Visibility = Visibility.Collapsed;
        }

        private void save_new_emp_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (new_emp_name_tb.Text.Length == 0)
                    MessageBox.Show("Enter Employee Name");
                else if (new_emp_salary_tb.Text.Length == 0)
                    MessageBox.Show("Enter Salary");
                else if (new_eccup_combo.Text.Length == 0)
                    MessageBox.Show("Enter new eccup or choose one");
                else if (!new_date_hiring_dp.SelectedDate.HasValue)
                    MessageBox.Show("Enter the Date of hiring");
                else
                {
                    if (new_holiday_balance_tb.Text.Length == 0)
                        new_holiday_balance_tb.Text = "0";
                    DB.Insert("INSERT INTO Emp(emp, tel, address, ratb, eccup, deleted," +
                              " work_hours, vac_no) " +
                              $"VALUES('{new_emp_name_tb.Text}', '{new_emp_phone_tb.Text}', " +
                              $"'{new_address_tb.Text}', " +
                              $"{new_emp_salary_tb.Text}, '{new_eccup_combo.Text}', " +
                              $"0, {new_emp_wh_tb.Text}, {new_holiday_balance_tb.Text})");

                    new_emp_name_tb.Text = "";
                    new_emp_salary_tb.Text = "";
                    new_address_tb.Text = "";
                    new_emp_phone_tb.Text = "";

                    showJobs();
                    showEmp("");
                    showSpendList();
                    MessageBox.Show("Done");

                }
            }), DispatcherPriority.Background);
        }

        private void add_new_emp_borrow_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (spend_value_tb.Text.Length == 0)
                    MessageBox.Show(LocalizedLangs.Instance["enter_spend_value"]);
                else if (spend_desc_combo.SelectedIndex == -1)
                    MessageBox.Show(LocalizedLangs.Instance["enter_desc"]);
                else if (!spend_date_dp.SelectedDate.HasValue)
                    MessageBox.Show(LocalizedLangs.Instance["enter_spend_date"]);
                else
                {
                    CommonMethods.opDir = "";
                    if (spend_desc_combo.SelectedIndex == 0)
                        CommonMethods.opDir = LocalizedLangs.Instance["spends"];
                    else if (spend_desc_combo.SelectedIndex == 1)
                        CommonMethods.opDir = LocalizedLangs.Instance["salary"];

                    DB.Insert("INSERT INTO Spending([cash], [discrption], [notes], [user_id], [spend_date], " +
                              "[emp_id]) " +
                              $"VALUES({spend_value_tb.Text}, '{CommonMethods.opDir}', " +
                              $"'{spend_notes_tb.Text}', {MainWindow.userId}, {spend_date_dp.SelectedDate.Value.ToOADate()}, " +
                              $"{borrow_emp_combo.SelectedValue})");

                    if (spend_desc_combo.SelectedIndex == 0)
                    {
                        DB.Update("UPDATE Emp SET solfa=solfa+" + spend_value_tb.Text
                                                                + " WHERE ID=" + borrow_emp_combo.SelectedValue);
                        showEmp("");
                    }

                    CommonMethods.billNo = Convert.ToInt32(DB.getData("SELECT MAX(ID) FROM Spending").Rows[0][0]);
                    CommonMethods.balance =
                        Convert.ToInt32(DB.getData("SELECT balance FROM SafeBalance").Rows[0][0]) -
                        Convert.ToDouble(spend_value_tb.Text);

                    DB.Insert("INSERT INTO Safe(cash_in, discrption, [state], bill_no, p_date, " +
                              "user_id, person) " +
                              $"VALUES({spend_value_tb.Text}, 'Spends', 'Purchases', {CommonMethods.billNo}, " +
                              $"{spend_date_dp.SelectedDate.Value.ToOADate()}, {MainWindow.userId}, '{CommonMethods.opDir}'" +
                              $")");

                    DB.Update("UPDATE SafeBalance SET balance=" + CommonMethods.balance);

                    spend_value_tb.Text = "";
                    spend_notes_tb.Text = "";

                    showSpendList();

                    MessageBox.Show("Done");

                }
            }), DispatcherPriority.Background);
        }

        private void hide_borrow_border(object sender, MouseButtonEventArgs e)
        {
            borrow_border.Visibility = Visibility.Collapsed;
        }

        private void borrow_emp_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (borrow_emp_combo != null && borrow_emp_combo.SelectedIndex != -1)
                showSpendList();
        }

        private void spend_month_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (borrow_emp_combo != null && borrow_emp_combo.SelectedIndex != -1)
                showSpendList();
        }

        private void eccup_change_lv_tb(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update("UPDATE Emp SET eccup='" + (sender as TextBox).Text + "' " +
                        "WHERE ID="+CommonMethods.id);
                }
            }
        }

        private void emp_name_change_lv_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update("UPDATE Emp SET emp='" + (sender as TextBox).Text + "' " +
                        "WHERE ID=" + CommonMethods.id);
                }
            }
        }

        private void emp_address_change_lv_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update("UPDATE Emp SET address='" + (sender as TextBox).Text + "' " +
                        "WHERE ID=" + CommonMethods.id);
                }
            }
        }

        private void emp_phone_change_lv_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update("UPDATE Emp SET tel='" + (sender as TextBox).Text + "' " +
                        "WHERE ID=" + CommonMethods.id);
                }
            }

        }

        private void emp_salary_change_lv_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update("UPDATE Emp SET ratb=" + (sender as TextBox).Text
                        +" WHERE ID=" + CommonMethods.id);
                    showEmp("");
                }
            }
        }

        private void emp_work_hours_change_lv_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update("UPDATE Emp SET work_hours=" + (sender as TextBox).Text
                        + " WHERE ID=" + CommonMethods.id);
                }
            }
        }

        private void emp_holiday_change_lv_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update("UPDATE Emp SET vac_no=" + (sender as TextBox).Text
                        + " WHERE ID=" + CommonMethods.id);
                }
            }
        }

        

        private void delete_emp_img_mouse_down(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (CommonMethods.defaultDelete())
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as Image).DataContext as DataRowView).Row["ID"]);
                    DB.Delete($"DELETE * FROM Emp WHERE ID={CommonMethods.id}");
                    showEmp("");
                    CommonMethods.id = Convert.ToInt32(jobs_combo.SelectedIndex);
                    showJobs();
                    try
                    {
                        jobs_combo.SelectedIndex = CommonMethods.id;
                    }
                    catch
                    {
                    }
                }
            }), DispatcherPriority.Background);
        }

        private void delete_spend_mouse_down_img(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (CommonMethods.defaultDelete())
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as Image).DataContext as DataRowView).Row["ID"]);

                    DB.Delete($"DELETE * FROM Spending WHERE ID={CommonMethods.id}");

                    CommonMethods.lack = Convert.ToDouble(DB.getData("SELECT cash_in FROM Safe WHERE discrption like " +
                                                                     $"'Spends' AND bill_no={CommonMethods.id}")
                        .Rows[0][0]);

                    DB.Update("UPDATE SafeBalance SET balance=balance+"
                              + CommonMethods.lack);

                    DB.Delete("DELETE * FROM Safe WHERE discrption like 'Spends' AND " +
                              $"bill_no={CommonMethods.id}");

                    showSpendList();
                }
            }), DispatcherPriority.Background);
        }

        private void salary_details_window_img_md(object sender, MouseButtonEventArgs e)
        {
            showPopup((UIElement)new AttendController(this), "Attends & Salaries", 1500);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void TextBox_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }
    }
}
