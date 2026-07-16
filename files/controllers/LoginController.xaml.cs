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
    /// Interaction logic for LoginController.xaml
    /// </summary>
    public partial class LoginController : UserControl
    {

        public LoginController()
        {
            InitializeComponent();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.getUsers(ref users_combo);
            }), DispatcherPriority.Background);
            
            //password.Focus();
        }

        private void login_btn_click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            login();
            (sender as Button).IsEnabled = true;
        }

        private void login()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (password.Password.Length == 0)
                {
                    MessageBox.Show(LocalizedLangs.Instance["enter_password"]);
                    return;
                }

                CommonMethods.query = password.Password;
                CommonMethods.encryptString(ref CommonMethods.query);
                CommonMethods.dataRowView = users_combo.SelectedItem as DataRowView;

                if (CommonMethods.dataRowView[2].ToString() == CommonMethods.query)
                {
                    MainWindow.userId = Convert.ToInt32(users_combo.SelectedValue.ToString());

                    DB.Update("UPDATE Users SET is_active=0");
                    DB.Update($"UPDATE Users SET is_active=1 WHERE ID={users_combo.SelectedValue}");

                    MainWindow main = new MainWindow();

                    if (Convert.ToBoolean(CommonMethods.dataRowView[3]) == false)
                    {
                        MainWindow.userAccess = false;
                    }
                    else
                    {
                        MainWindow.userAccess = true;
                        //main.salesReportCtrl = new SalesReportController();
                        //main.empCtrl = new EmployeeController(main);
                    }
                    
                    main.Show();
                    
                }
                else
                {
                    MessageBox.Show(LocalizedLangs.Instance["wrong_pass"]);
                    password.Password = "";
                }
            }), DispatcherPriority.Background);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // main.Close();
        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            //if(e.Key == Key.Return)
            //{
            //    login();
            //}
        }

        private void login_controller_keydown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                login();
                return;
            }else if(e.Key == Key.Escape)
            {
                // main.Close();
            }
        }
    }
}
