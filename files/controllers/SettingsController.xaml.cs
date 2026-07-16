using SalesManagement.files.classes;
using SalesManagement.Resources;
using System;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using winforms = System.Windows.Forms;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for SettingsController.xaml
    /// </summary>
    public partial class SettingsController : UserControl
    {
        private MainCtr mainCtr;
        public SettingsController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;


            showUsersCombo();

            showOrder();

            showPrinters();
            
        }

        private void showPrinters()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (string installedPrinter in PrinterSettings.InstalledPrinters)
                    this.printers_combo.Items.Add((object)installedPrinter);
                try
                {
                    this.printers_combo.SelectedValue =
                        DB.getData("SELECT lan_printer FROM Printer").Rows[0][0].ToString();
                }
                catch
                {
                }
            }), DispatcherPriority.Background);
        }

        private void showOrder()
        {
            try
            {
                con_order_tb.Text = DB.getData("SELECT report_txt FROM Shift").Rows[0][0].ToString();
                delev_name_tb.Text = DB.getData("SELECT add_serv FROM Shift").Rows[0][0].ToString();
            }
            catch
            {

            }
        }

        private void showUsersCombo()
        {
            try
            {
                CommonMethods.getUsers(ref users_combo);
                is_admin_check.IsChecked = Convert.ToBoolean(
                    (users_combo.SelectedItem as DataRowView).Row["is_admin"]);
            }
            catch { }
        }

        private void update_user_password_isadmin_click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (users_combo.SelectedIndex == -1) return;
                CommonMethods.query = "";
                if (password_user.Password.Length > 0)
                {
                    CommonMethods.query = password_user.Password.ToString();
                    CommonMethods.encryptString(ref CommonMethods.query);
                    CommonMethods.query = $"[password] = '{CommonMethods.query}', ";
                    //MessageBox.Show(CommonMethods.query);
                }

                //MessageBox.Show(Convert.ToInt32(is_admin_check.IsChecked).ToString());

                DB.Update($"UPDATE Users SET  {CommonMethods.query} " +
                          $"is_admin={Convert.ToInt32(is_admin_check.IsChecked)} " +
                          $" WHERE ID={users_combo.SelectedValue}");
                password_user.Password = "";
                MessageBox.Show(LocalizedLangs.Instance["done"]);
            }), DispatcherPriority.Background);
        }

        private void delete_user_click(object sender, RoutedEventArgs e)
        {
            if (CommonMethods.defaultDelete())
            {
                DB.Update($"UPDATE Users set ok=1 WHERE ID={users_combo.SelectedValue}");
                MessageBox.Show("Deleted Successfully");
                showUsersCombo();
            }
        }

        private void create_new_emp_click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (new_user_name_tb.Text.Length == 0)
                    MessageBox.Show("Enter User Name");
                else if (new_pass_pb.Password.Length < 4)
                    MessageBox.Show("Password Must be more than 4 letters");
                else if (new_pass_pb.Password != new_re_pass_pb.Password)
                    MessageBox.Show("Passwords do not match");
                else if (new_acc_type.SelectedIndex == -1)
                    MessageBox.Show("Choose Account Type");
                else
                {
                    if (new_acc_type.SelectedIndex == 0) CommonMethods.id = 1;
                    else CommonMethods.id = 0;

                    DB.Insert("INSERT INTO Users(user_name, [password], is_admin) " +
                              $"VALUES('{new_user_name_tb.Text}', '{new_pass_pb.Password}', {CommonMethods.id})");
                    MessageBox.Show("Registered Successfully");
                    new_user_name_tb.Text = "";
                    new_pass_pb.Password = "";
                    new_re_pass_pb.Password = "";
                    showUsersCombo();
                }
            }), DispatcherPriority.Background);
        }

        private void delete_clients_click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (CommonMethods.checkPassword(current_user_pass.Password, MainWindow.userId)
                    && password_user.Password.Length > 0)
                {
                    DB.Delete("DELETE * FROM Client");
                    MessageBox.Show("All Client are Deleted Successfully");

                    current_user_pass.Password = "";
                }
                else
                    MessageBox.Show("Wrong Password");
            }), DispatcherPriority.Background);
        }

        private void reorder_tables_click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (CommonMethods.checkPassword(current_user_pass.Password, MainWindow.userId)
                    && password_user.Password.Length > 0)
                {
                    DB.Update("UPDATE Tables SET used=0");
                    DB.Delete("DELETE * FROM PreSales WHERE orde_type_no=1");

                    MessageBox.Show("Done");

                    current_user_pass.Password = "";
                }
                else
                    MessageBox.Show("Wrong Password");
            }), DispatcherPriority.Background);
        }

        private void delete_all_sales_click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (CommonMethods.checkPassword(current_user_pass.Password, MainWindow.userId)
                    && password_user.Password.Length > 0)
                {
                    DB.Delete("DELETE * FROM Sales");
                    DB.Delete("DELETE * FROM Consumption");

                    MessageBox.Show("Done");

                    current_user_pass.Password = "";
                }
                else
                    MessageBox.Show("Wrong Password");
            }), DispatcherPriority.Background);
        }

        private void delete_cates_items_click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (CommonMethods.checkPassword(current_user_pass.Password, MainWindow.userId)
                    && password_user.Password.Length > 0)
                {
                    DB.Delete("DELETE * FROM Categ");
                    DB.Delete("DELETE * FROM Food");
                    DB.Delete("DELETE * FROM Sales");
                    DB.Delete("DELETE * FROM PreSales");
                    DB.Delete("DELETE * FROM Consumption");

                    MessageBox.Show("Done");

                    current_user_pass.Password = "";
                }
                else
                    MessageBox.Show("Wrong Password");
            }), DispatcherPriority.Background);
        }

        private void create_backup_img_md(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var folderBrowserDialog = new winforms.FolderBrowserDialog();

                if (folderBrowserDialog.ShowDialog() == winforms.DialogResult.Cancel)
                    return;
                File.Copy("E:\\projects\\SalesManagement\\files\\database\\FoodDb.mdb",
                    folderBrowserDialog.SelectedPath + "\\" +
                    DateTime.Now.ToString("d-MMM-yyyy hh mm ss") + ".kdb");
                MessageBox.Show("Done");
            }), DispatcherPriority.Background);

        }

        private void restor_backup_img_md(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var openFileDialog = new winforms.OpenFileDialog();
                openFileDialog.Filter = "backup file db|*.kdb";
                if (openFileDialog.ShowDialog() == winforms.DialogResult.Cancel ||
                    MessageBox.Show(
                        "Do you want to continue?\nThe old database will be removed and replaced with the restore one",
                        "Restor Backup",
                        MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;
                File.Copy(openFileDialog.FileName, "FoodDb.mdb", true);
                MessageBox.Show("Done");
            }), DispatcherPriority.Background);
        }

        private void change_order_details_click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (con_order_tb.Text.Length == 0)
                {
                    MessageBox.Show("Enter Conclusion of the Order");
                }
                else
                {
                    if (delev_name_tb.Text.Length == 0)
                        delev_name_tb.Text = "0";
                    DB.Update($"UPDATE Shift SET report_txt='{con_order_tb.Text}', " +
                              $"add_serv = {delev_name_tb.Text}");
                    MessageBox.Show("Done");
                }
            }), DispatcherPriority.Background);
        }

        private void save_printer_click(object sender, RoutedEventArgs e)
        {
            if (printers_combo.SelectedIndex <= -1)
                return;
            DB.Update("UPDATE printer SET lan_printer ='" + printers_combo.Text + "'");
            MessageBox.Show("Done");
        }

        private void users_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(users_combo != null && users_combo.SelectedIndex != -1)
            {
                is_admin_check.IsChecked = Convert.ToBoolean((users_combo.SelectedItem as DataRowView).Row["is_admin"]);
            }
        }
    }
}
