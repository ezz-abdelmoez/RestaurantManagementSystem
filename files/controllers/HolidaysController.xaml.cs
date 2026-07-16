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
    /// Interaction logic for HolidaysController.xaml
    /// </summary>
    public partial class HolidaysController : UserControl
    {
        EmployeeController emp;
        public HolidaysController(EmployeeController e)
        {
            InitializeComponent();
            emp = e;
            month_tb.Text = DateTime.Now.Month.ToString();
            year_tb.Text = DateTime.Now.Year.ToString();

            CommonMethods.getJobs(ref jobs_combo);
            showEmps();
            showHolidaysList();
        }

        // show holidays listview
        private void showHolidaysList()
        {
            CommonMethods.query = "SELECT EmpVac.*, Emp.emp, Emp.eccup FROM Emp " +
                "INNER JOIN EmpVac ON EmpVac.emp_id = Emp.ID " +
                "WHERE EmpVac.ID>0 ";
            if (jobs_combo.SelectedIndex != -1)
                CommonMethods.query += $" AND Emp.eccup like '{jobs_combo.SelectedValue}' ";
            if (emp_combo.SelectedIndex > -1)
                CommonMethods.query += $" AND EmpVac.emp_id like {emp_combo.SelectedValue} ";
            if (year_tb.Text.Length > 0)
                CommonMethods.query += $" AND YEAR(EmpVac.act_date) like '{year_tb.Text}' ";
            if (month_tb.Text.Length > 0)
                CommonMethods.query += $" AND MONTH(EmpVac.act_date) like '{month_tb.Text}' ";

            holidayListview.ItemsSource = DB.getData(CommonMethods.query 
                + " ORDER BY EmpVac.ID").DefaultView;

        }

        private void showEmps()
        {
            CommonMethods.query = "SELECT * FROM Emp WHERE deleted=0 ";
            if (jobs_combo.SelectedIndex != -1)
                CommonMethods.query += $" AND eccup like '{jobs_combo.SelectedValue}' ";
            emp_combo.ItemsSource = DB.getData(CommonMethods.query).DefaultView;
            emp_combo.SelectedValuePath = "ID";
            emp_combo.DisplayMemberPath = "emp";
        }

        private void add_holiday_mouse_down_img(object sender, MouseButtonEventArgs e)
        {
            if (emp_combo.SelectedIndex == -1 && jobs_combo.SelectedIndex == -1)
                MessageBox.Show("Choose Employee or job");
            else if (value_tb.Text.Length == 0)
                MessageBox.Show("Enter The Value");
            else if (op_combo.SelectedIndex == -1)
                MessageBox.Show("Choose The Operation");
            else
            {
                if(op_combo.SelectedIndex == 0)
                {
                    if (emp_combo.SelectedIndex > -1)
                    {
                        DB.Insert("INSERT INTO EmpVac(emp_id, act_dir, act_type, act_date, quant) " +
                            $"VALUES({emp_combo.SelectedValue}, -1, '{op_combo.Text}', {DateTime.Now.Date.ToOADate()}, " +
                            $"{value_tb.Text})");
                        DB.Update("UPDATE Emp SET vac_no=vac_no-" + value_tb.Text
                            + " WHERE ID=" + emp_combo.SelectedValue);
                        
                        
                    }
                    else
                    {
                        MessageBox.Show("Choose The Employee");
                    }
                }
                else if (op_combo.SelectedIndex == 1)
                {
                    if (emp_combo.SelectedIndex > -1)
                    {
                        DB.Insert("INSERT INTO EmpVac (emp_id, act_dir, act_type, act_date, quant) " +
                            $"VALUES({emp_combo.SelectedValue}, 1, '{op_combo.Text}', " +
                            $"{DateTime.Now.Date.ToOADate()}, {value_tb.Text})");
                        DB.Update("UPDATE Emp SET vac_no=vac_no+" + value_tb.Text
                            + " WHERE ID=" + emp_combo.SelectedValue);
                        
                    }
                    else
                    {
                        CommonMethods.data = new DataTable();
                        CommonMethods.data = DB.getData($"SELECT ID FROM Emp WHERE eccup like " +
                            $"'{jobs_combo.SelectedValue}'");

                        foreach(DataRow row in CommonMethods.data.Rows)
                        {
                            DB.Insert("INSERT INTO EmpVac (emp_id, act_dir, act_type, act_date, quant) " +
                                $"VALUES({emp_combo.SelectedValue}, 1, '{op_combo.Text}', " +
                                $"{DateTime.Now.Date.ToOADate()}, {value_tb.Text})");
                            DB.Update("UPDATE Emp SET vac_no=vac_no+" + value_tb.Text 
                                + " WHERE ID=" + row[0]);
                        }
                    }
                }
                else if(op_combo.SelectedIndex == 2)
                {
                    if(emp_combo.SelectedIndex > -1)
                    {
                        DB.Insert("INSERT INTO EmpVac(emp_id, act_dir, act_type, act_date, quant) " +
                            $"VALUES({emp_combo.SelectedValue}, 0, '{op_combo.Text}', " +
                            $"'{DateTime.Now.Date}', {value_tb.Text})");
                        DB.Update("UPDATE Emp SET vac_no=" + value_tb.Text + 
                            " WHERE ID=" + emp_combo.SelectedValue);

                   
                    }
                    else
                    {
                        MessageBox.Show("Must Choose Employee First");
                    }
                }
                showHolidaysList();
                CommonMethods.id = emp_combo.SelectedIndex;
                showEmps();
                emp_combo.SelectedIndex = CommonMethods.id;
                MessageBox.Show("Done");
                emp.showEmp("");
            }
        }

        private void jobs_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (emp_combo != null)
            {
                showEmps();
                showHolidaysList();
            }
        }

        private void emp_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (emp_combo != null)
            {
                showHolidaysList();
                if (emp_combo.SelectedIndex > -1)
                    balance_tb.Text = (emp_combo.SelectedItem as DataRowView).Row["vac_no"].ToString();
                else
                    balance_tb.Text = "0";
            }
        }

        private void month_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(jobs_combo != null && emp_combo != null)
                showHolidaysList();
        }

        private void month_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }
    }
}
