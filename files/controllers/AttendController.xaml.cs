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

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for AttendController.xaml
    /// </summary>
    public partial class AttendController : UserControl
    {
        private EmployeeController empController;
        public AttendController(EmployeeController em)
        {
            InitializeComponent();
            empController = em;
            month_tb.Text = DateTime.Now.Month.ToString();
            year_tb.Text = DateTime.Now.Year.ToString();
            showEmpDetails();
        }

        private void showEmpDetails()
        {
            label3:

            CommonMethods.data = new DataTable();
            CommonMethods.data = DB.getData(
                "SELECT EmpAction.*, Emp.emp, Emp.ratb, " +
                "(salary + reward + ( ( (salary/(work_hours*28) ) * (offer_time*2) ) ) - " +
                "(penality+ ((salary/28) * (late+vac+(absence*2) ) ) ) ) as net_salary " +
                "FROM EmpAction LEFT JOIN Emp ON EmpAction.emp_id = Emp.ID WHERE emp.deleted=0 " +
                $"AND MONTH(EmpAction.act_date) like '{month_tb.Text}' " +
                $"AND YEAR(EmpAction.act_date) like '{year_tb.Text}' ORDER BY Emp.emp"
                );

            attendListview.ItemsSource = CommonMethods.data.DefaultView;
            if (CommonMethods.data.Rows.Count == 0)
            {
                CommonMethods.dateTime = new DateTime(Convert.ToInt32(year_tb.Text),
                    Convert.ToInt32(month_tb.Text), 1);

                CommonMethods.data = new DataTable();
                CommonMethods.data = DB.getData("SELECT * FROM Emp WHERE deleted=0");
                CommonMethods.enumerator = CommonMethods.data.Rows.GetEnumerator();
                try
                {
                    while (CommonMethods.enumerator.MoveNext())
                    {
                        CommonMethods.dataRow = (DataRow)CommonMethods.enumerator.Current;
                        DB.Insert("INSERT INTO EmpAction (emp_id, act_date, salary, attend, " +
                            "absence, vac, late, penality, reward, offer_time) " +
                            $"VALUES({CommonMethods.dataRow["ID"]}, '{CommonMethods.dateTime}', " +
                            $"{CommonMethods.dataRow["ratb"]}, 0, 0, 0, 0, 0, 0, 0)");

                    }
                    goto label3;
                }
                finally
                {
                    if (CommonMethods.enumerator is IDisposable disposable)
                        disposable.Dispose();

                }
            }
            else
            {
                CommonMethods.totalNumber = 0;
                foreach (DataRow row in CommonMethods.data.Rows)
                    CommonMethods.totalNumber += Convert.ToDouble(row["net_salary"]);
                total.Text = CommonMethods.totalNumber.ToString();

                CommonMethods.enumerator = CommonMethods.data.Rows.GetEnumerator();
                try
                {
                    if (CommonMethods.enumerator.MoveNext())
                    {
                        if (Convert.ToBoolean(((DataRow)CommonMethods.enumerator.Current)["approved"]))
                        {
                            approve_border.Visibility = Visibility.Visible;
                            not_approve_border.Visibility = Visibility.Collapsed;
                            director_visa_btn.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            approve_border.Visibility = Visibility.Collapsed;
                            not_approve_border.Visibility = Visibility.Visible;
                            director_visa_btn.Visibility = Visibility.Visible;
                        }
                    }
                }
                finally
                {
                    if (CommonMethods.enumerator is IDisposable disposable)
                        disposable.Dispose();
                }
            }
        }

     

        private void show_details_btn(object sender, RoutedEventArgs e)
        {
            if (month_tb.Text.Length > 0 && year_tb.Text.Length > 0)
                showEmpDetails();
        }

        private void director_visa_btn_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataRow row in CommonMethods.data.Rows)
                DB.Update("UPDATE EmpAction SET approved=1 WHERE ID="+row["ID"]);
            MessageBox.Show("All Approved");
        }

        private void attend_change_tb(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE EmpAction SET attend={(sender as TextBox).Text}");

                    showEmpDetails();
                }
            }
        }

        private void holiday_change_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE EmpAction SET vac={(sender as TextBox).Text}");

                    showEmpDetails();
                }
            }
        }

        private void absence_change_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE EmpAction SET absence={(sender as TextBox).Text}");

                    showEmpDetails();
                }
            }
        }

        private void late_change_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE EmpAction SET late={(sender as TextBox).Text}");

                    showEmpDetails();
                }
            }
        }

        private void penality_change_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE EmpAction SET penality={(sender as TextBox).Text}");

                    showEmpDetails();
                }
            }
        }

        private void offer_time_change_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE EmpAction SET offer_time={(sender as TextBox).Text}");

                    showEmpDetails();
                }
            }
        }

        private void reward_change_tb(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32(
                        ((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE EmpAction SET reward={(sender as TextBox).Text}");

                    showEmpDetails();
                }
            }
        }

        private void month_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }
    }
}
