using SalesManagement.files.classes;
using SalesManagement.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPFLocalizeExtension.Engine;
using Button = System.Windows.Controls.Button;
using FlowDirection = System.Windows.FlowDirection;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SalesManagement.files
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
         public LoginWindow()
        {
            InitializeComponent();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.getUsers(ref users_combo);
            }), DispatcherPriority.Background);

                       
            if (Properties.Settings.Default.languageCode == "ar-EG")
            {
                this.FlowDirection = FlowDirection.RightToLeft;
                lan_change_combo.SelectedIndex = 0;
            }
            else
            {
                this.FlowDirection = FlowDirection.LeftToRight;
                lan_change_combo.SelectedIndex = 1;
            }
            lan_change_combo.SelectionChanged += lan_change_combo_SelectionChanged;
            
            //password.Focus();
        }

         private void lan_change_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             Dispatcher.BeginInvoke(new Action(() =>
             {

                 CommonMethods.langCode = "";
            
                 if (lan_change_combo.SelectedIndex == 0)
                 {
                     Properties.Settings.Default.languageCode = "ar-EG";
                     this.FlowDirection =FlowDirection.RightToLeft;
                 }
                 else
                 {
                     Properties.Settings.Default.languageCode = "en-US";
                     this.FlowDirection = FlowDirection.LeftToRight;
                 }

                 Properties.Settings.Default.Save();
                 CommonMethods.langCode = Properties.Settings.Default.languageCode;
                 LocalizeDictionary.Instance.Culture = new CultureInfo(CommonMethods.langCode);

             }), DispatcherPriority.Background);
            

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
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
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
            this.Close();
        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            //if(e.Key == Key.Return)
            //{
            //    login();
            //}
        }


        private void LoginWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                login();
                return;
            }else if(e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
