using SalesManagement.files.classes;
using SalesManagement.files.controllers;
using SalesManagement.files.windows;
using SalesManagement.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFLocalizeExtension.Engine;
using System.Management;
using System.Data;
using System.IO;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using SalesManagement.files;
using Dragablz;

namespace SalesManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int userId { get; set; }
        public static int shiftNo = 1;
        public static int tabCount = 1;
        public static bool userAccess;
        private int AnimationCounter = 0;
        private List<string> strarray = new List<string>();

        
        private DispatcherTimer timer;
        private DoubleAnimation doubleAnimation = new DoubleAnimation();

        public static MainWindow mainWindow;

        public MainWindow()
        {
            InitializeComponent();
            
            if (Properties.Settings.Default.languageCode == "ar-EG")
            {
                this.FlowDirection = FlowDirection.RightToLeft;
                TabControl1.FlowDirection = FlowDirection.RightToLeft;
                lan_change_combo.SelectedIndex = 0;
            }
            else
            {
                this.FlowDirection = FlowDirection.LeftToRight;
                TabControl1.FlowDirection = FlowDirection.LeftToRight;
                lan_change_combo.SelectedIndex = 1;
            }
            lan_change_combo.SelectionChanged += lan_change_combo_SelectionChanged;

            checkTheme();
            mainWindow = this;
            
            //showPopup(new LoginController(this), "Login");

           
            // tabGrid.Visibility = Visibility.Collapsed;
            // login_grid.Visibility = Visibility.Visible;
            // login_grid.Children.Add((UIElement)new LoginController(this));

            showShortsQty();
            //setMarquee();
            //userAccess(false);

            // show time and date of today
            timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, (EventHandler)((param0, param1) =>
            {
                this.date_lb.Content = (object)DateTime.Now.ToString("yyyy/MM/dd", (IFormatProvider)new CultureInfo(CommonMethods.langCode));
                this.time_lb.Content = (object)DateTime.Now.ToString("hh:mm:ss tt", (IFormatProvider)new CultureInfo(CommonMethods.langCode));
                this.day_of_week_lb.Content = (object)DateTime.UtcNow.ToString("dddd", (IFormatProvider)new CultureInfo(CommonMethods.langCode));
            }), this.Dispatcher);

            // DragablzItem newTab = new DragablzItem();
            TabItem newTab = new TabItem();
            MainCtr newMainCtr = new MainCtr(newTab);
            //newTab.Header = "Main";
            newTab.Content = newMainCtr;
            TabControl1.Items.Add(newTab);

        }


        private ShortageCtr shortageCtr;
        private void showShortsWin_md(object sender, MouseButtonEventArgs e)
        {
            try { MainCtr.mainCtr.mainGrid.Children.RemoveAt(0); }
            catch { }
            shortageCtr = new ShortageCtr();
            MainCtr.mainCtr.mainGrid.Children.Add((UIElement)shortageCtr);
        }

        // show shorts of qty
        public void showShortsQty()
        {
            try
            {
                CommonMethods.data1 = new DataTable();
                CommonMethods.data1 = DB.getData("SELECT * FROM Food WHERE (qty < min_level)");
                foreach (DataRow row in CommonMethods.data1.Rows)
                {
                    strarray.Add(" " + row["food"] + " ");
                }
                strarray.Add(LocalizedLangs.Instance["created_by"]);
            }
            catch
            {

            }
        }

       

 
        

        public static void safe(bool dir, double cash, string disc, string person, DateTime p_date)
        {
            if (!dir)
                cash *= -1;
            DB.Update($"UPDATE SafeBalance SET balance=" +
                $"{Convert.ToDouble(DB.getData("SELECT balance From SafeBalance").Rows[0][0].ToString()) + cash}");

            DB.Insert("INSERT INTO Safe(cash_in, discrption, person, p_date) " +
                $"VALUES({cash}, '{disc}', '{person}', {p_date.ToOADate()})");
        }

        

    
        //
        // public void showmin()
        // {
        //     try
        //     {
        //         //screen.Delete();
        //         popWin.Close();
        //         popWin = null;
        //     }
        //     catch
        //     {
        //     }
        // }

        


       


        #region purchasedOrDemo
        
        // BOIS Serial Number 
        private string boisSerialNum()
        {
            Console.WriteLine("Win32_BIOS");
            foreach(ManagementObject managementObject in new ManagementObjectSearcher("select * from Win32_BIOS")
                .Get())
            {
                try
                {
                    CommonMethods.boisSerialNum = managementObject["SerialNumber"].ToString().Trim();
                    return CommonMethods.boisSerialNum;
                }
                catch { }
            }
            return string.Empty;
        }

        // get cup id
        private string getCPUID()
        {
            foreach(ManagementObject instance in new ManagementClass("win32_processor").GetInstances())
            {
                if(CommonMethods.cpuId == "")
                {
                    CommonMethods.cpuId = instance.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            return CommonMethods.cpuId;
        }

        private string getUniqueId()
        {
            boisSerialNum();
            getCPUID();
            return CommonMethods.cpuId.Substring(13) +
                CommonMethods.cpuId.Substring(1, 4) +
                CommonMethods.boisSerialNum +
                CommonMethods.cpuId.Substring(4, 4);
        }

        // check if demo version 
        private bool showDemo()
        {
            CommonMethods.flag = false;
            try
            {
                CommonMethods.mos = Convert.ToInt32(DB.getData("SELECT COUNT(ID) AS tot FROM Sales").Rows[0][0].ToString());
            }
            catch
            {
                CommonMethods.mos = 1;
            }
                
            // demobar.Value = CommonMethods.mos;
            if(CommonMethods.mos > 250)
            {
                if (CommonMethods.mos > 500)
                {
                    CommonMethods.flag = true;
                    int num = (int)System.Windows.MessageBox.Show(LocalizedLangs.Instance["trial_end"]);
                    this.Close();
                }
                else
                {
                    // this.demobar.Foreground = (Brush)Brushes.Red;
                    int num = (int)System.Windows.MessageBox.Show(LocalizedLangs.Instance["trial_almost"]);
                }
            }

            return CommonMethods.flag;
        }
        #endregion

        // end offers 
        private void endOffers()
        {
            CommonMethods.data = new DataTable();
            CommonMethods.data = DB.getData("SELECT PriceChange.*, Food.food FROM PriceChange INNER JOIN Food " +
                " ON PriceChange.item_id = food.ID");

            foreach(DataRow row in CommonMethods.data.Rows)
            {
                if(DateTime.Now > Convert.ToDateTime(row["end_time"]).AddHours(6))
                {
                    MessageBox.Show(row["food"].ToString());
                    DB.Update($"UPDATE Food SET price={row["old_price"]}, offer=0 WHERE ID={row["item_id"]}");
                    DB.Delete($"DELETE * FROM PriceChange WHERE ID={row[0]}");
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
               

                CommonMethods.uniqueId = getUniqueId();
                if (CommonMethods.uniqueId == "F6278BFMXL0410QR7FBFF" || CommonMethods.uniqueId == "F4378BFMXL1162VKHFBFF")
                    // this.demobar.Visibility = Visibility.Hidden;
                // else
                //     showDemo();
                if (CommonMethods.flag)
                    return;

                shiftNo = Convert.ToInt32(DB.getData("SELECT shift FROM Shift").Rows[0][0].ToString());
                // shiftno_tb.Content = shiftNo.ToString();

                endOffers();

                
            }), DispatcherPriority.Background);
        }

        private void upcli_DoWork(object sender, DoWorkEventArgs e)
        {
            CommonMethods.data = new DataTable();
            CommonMethods.data = DB.getData("SELECT order_type_no, table_no, chare_no FROM PreSales " +
                $"WHERE android_done=1 AND user_id={userId} " +
                $"GROUP BY order_type_no, table_no, chare_no");

            foreach(DataRow row in CommonMethods.data.Rows)
            {
                if(Convert.ToInt32(row["order_type_no"]) == 1)
                {
                    DB.Update($"UPDATE PreSales SET android_done = false WHERE " +
                        $"order_type_no={row["order_type_no"]} AND table_no={row["table_no"]} " +
                        $"AND chare_no={row["chare_no"]} and user_id={userId}");

                    try
                    {
                        CommonMethods.data1 = new DataTable();
                        CommonMethods.data1 = DB.getData("SELECT Categ.print_place FROM PreSales INNER JOIN " +
                            "(Food INNER JOIN Categ ON Food.categ=Categ.ID) ON PreSales.food_id=Food.ID " +
                            "GROUP Categ.print_place");
                        foreach(DataRow row1 in CommonMethods.data1.Rows)
                        {
                            // print
                        }
                    }
                    catch { }
                    DB.Update($"UPDATE PreSales SET activ=true WHERE order_type_no={row["order_type_no"]} " +
                        $"AND table_no={row["table_no"]} AND chare_no={row["chare_no"]} " +
                        $"AND user_id={userId} ");
                }
            }
        }

    

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton != MouseButton.Left)
                    return;
                this.DragMove();
            }
            catch
            {
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                CommonMethods.query = "D:\\SalesManagement";
                if (!Directory.Exists(CommonMethods.query))
                    Directory.CreateDirectory(CommonMethods.query);
                File.Copy("FoodDb.mdb", 
                    CommonMethods.query + "\\backup_" 
                                        + DateTime.Now.DayOfWeek.ToString() + ".kdb", true);
            }
            catch
            {
            }
        }


        private void add_tab_btn_click(object sender, RoutedEventArgs e)
        {
            if (TabControl1.Items.Count > 10) return;

            tabCount++;
            // TabControl1.Items.RemoveAt(TabControl1.Items.Count-1);
            TabItem newTab = new TabItem();
            MainCtr newMainCtr = new MainCtr(newTab);
            // newTab.Header = "Main";
            newTab.Content = newMainCtr;
            TabControl1.Items.Add(newTab);

        }

        private void remove_tab_btn_click(object sender, RoutedEventArgs e)
        {
            if (TabControl1.Items.Count > 1)
            {
                TabControl1.Items.RemoveAt(TabControl1.Items.Count-1);
            }
        }
        

        private void delete_tab_md(object sender, MouseButtonEventArgs e)
        {
            if (TabControl1.Items.Count > 1)
            {
                // MessageBox.Show(TabControl1.SelectedIndex.ToString()
                //     +"\n"+
                //     (sender as TabItem).DataContext.ToString());
                TabControl1.Items.RemoveAt(TabControl1.SelectedIndex);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void canMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            setMarquee();
        }
        
        private void setMarquee()
        {
            this.doubleAnimation = new DoubleAnimation();
            this.doubleAnimation.From = new double?(-tbmarquee.ActualWidth);
            this.doubleAnimation.To = new double?(canMain.ActualWidth);
            this.doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:5"));
            this.doubleAnimation.Completed += new EventHandler(this.doubleAnimation_Completed);
            this.tbmarquee.BeginAnimation(Canvas.LeftProperty, (AnimationTimeline)this.doubleAnimation);
        }
        

        private void doubleAnimation_Completed(object sender, EventArgs e)
        {
            try
            {
                ++this.AnimationCounter;
                if (this.AnimationCounter >= this.strarray.Count)
                    this.AnimationCounter = 0;
                Dispatcher.BeginInvoke(new Action(() =>  tbmarquee.Text = strarray[AnimationCounter])).Wait();
                Dispatcher.BeginInvoke(new Action (() => setMarquee()));
            }
            catch
            {
            }
        }

        // check and change theme
        private void checkTheme()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.themeColor = Properties.Settings.Default.ColorMode;
                if (CommonMethods.themeColor == "Dark")
                {
                    change_mode_img.Source = new BitmapImage(
                        new Uri("pack://application:,,,/SalesManagement;component/assets/light.png"));
                    newRes.Source =
                        new Uri("pack://application:,,,/SalesManagement;component/Dictionaries/DarkTheme.xaml",
                            UriKind.RelativeOrAbsolute);
                    MainWindow.mainWindow.Background = new ImageBrush(
                        new BitmapImage(
                            new Uri("pack://application:,,,/SalesManagement;component/dark_back.jpg")));
                }
                else if (CommonMethods.themeColor == "Light")
                {
                    change_mode_img.Source =
                        new BitmapImage(
                            new Uri("pack://application:,,,/SalesManagement;component/assets/dark.png"));
                    newRes.Source =
                        new Uri("pack://application:,,,/SalesManagement;component/Dictionaries/LightTheme.xaml",
                            UriKind.RelativeOrAbsolute);
                    // MainWindow.mainWindow.Background = new ImageBrush(
                    //     new BitmapImage(
                    //         new Uri("pack://application:,,,/SalesManagement;component/logorest.jpg")));
                    MainWindow.mainWindow.Background = new ImageBrush(
                        new BitmapImage(
                            new Uri("pack://application:,,,/SalesManagement;component/dark_back.jpg")));
                }

                App.Current.Resources.MergedDictionaries.Remove(App.Current.Resources.MergedDictionaries.Last());
                Application.Current.Resources.MergedDictionaries.Add(newRes);

            }), DispatcherPriority.Background);
        }

        ResourceDictionary newRes = new ResourceDictionary();
        private void change_theme_img_md(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                CommonMethods.themeColor = Properties.Settings.Default.ColorMode;
                if (CommonMethods.themeColor == "Dark")
                {
                    CommonMethods.themeColor = "Light";
                }
                else
                {
                    CommonMethods.themeColor = "Dark";
                }
                Properties.Settings.Default.ColorMode = CommonMethods.themeColor;
                Properties.Settings.Default.Save();
                checkTheme();

            }), DispatcherPriority.Background);
            
        }

        private void calc_open_img_md(object sender, MouseButtonEventArgs e)
        {
            Process.Start("calc.exe");
        }

        private void lan_change_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                CommonMethods.langCode = "";
            
                if (lan_change_combo.SelectedIndex == 0)
                {
                    Properties.Settings.Default.languageCode = "ar-EG";
                    this.FlowDirection = FlowDirection.RightToLeft;
                    TabControl1.FlowDirection = FlowDirection.RightToLeft;
                }
                else
                {
                    Properties.Settings.Default.languageCode = "en-US";
                    this.FlowDirection = FlowDirection.LeftToRight;
                    TabControl1.FlowDirection = FlowDirection.LeftToRight;
                }

                Properties.Settings.Default.Save();
                CommonMethods.langCode = Properties.Settings.Default.languageCode;
                LocalizeDictionary.Instance.Culture = new CultureInfo(CommonMethods.langCode);

            }), DispatcherPriority.Background);
            

        }

        private void min_max_img(object sender, MouseButtonEventArgs e)
        {
            WindowBasicControl.minMaxWin(Window.GetWindow(this));
        }

        private void Hide_Img(object sender, MouseButtonEventArgs e)
        {
            WindowBasicControl.hideWin(Window.GetWindow(this));
        }

        private void exit_img(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void logout_lb_mouse_down(object sender, MouseButtonEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
        

        private void drag_window_md(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
                MainWindow.mainWindow.DragMove();
        }

        private void lan_change_combo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
