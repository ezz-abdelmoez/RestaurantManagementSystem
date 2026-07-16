using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SalesManagement.files.classes;
using SalesManagement.files.controllers;
using SalesManagement.files.windows;
using SalesManagement.Resources;
using WPFLocalizeExtension.Engine;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SalesManagement.files
{
    /// <summary>
    /// Interaction logic for MainCtr.xaml
    /// </summary>
    public partial class MainCtr : UserControl
    {
        public static PopupWindow popWin;
        private bool isTopMost = false;
        int currentMenu = -1;
       
  
        private BackgroundWorker worker;
        public DispatcherTimer rectimer;
        private bool isEnabled_keydownSales = false;

        // controllers
        public SalesController salesCtrl;
        public SalesReportController salesReportCtrl;
        public DeliveryOrdersController delvOrderCtrl;
        public ClientController clientCtrl;
        public OrdersController orderCtrl;
        public PurchaseController purchaseCtrl;
        public SuppliersController supCtrl;
        public StoreController storeCtrl;
        public CategoryController itemsCtrl;
        public PreparationController prepCtrl;
        public CateController categCtrl;
        public OfferController offerCtrl;
        public SafeController safeCtrl;
        public CashierController cashierCtrl;
        public CashierAccounts casheierAccsCtrl;
        public ExpensesController expensesCtrl;
        public SettingsController settingsCtrl;
        public DeliveryController deliveryCtrl;
        public EmployeeController empCtrl;
        public KitchenController kitchen;

        public static MainCtr mainCtr;
        private TabItem tab;

        public MainCtr(TabItem tab)
        {
            InitializeComponent();
            mainCtr = this;
            this.tab = tab;
            tab.Header = LocalizedLangs.Instance["main"];

            // var methodInfo = new StackTrace().GetFrame(1).GetMethod();
            // var className = methodInfo.ReflectedType.Name;
            // MessageBox.Show(className);

            userAccess(MainWindow.userAccess);

      
            CommonMethods.langCode = SalesManagement.Properties.Settings.Default.languageCode;
            if (CommonMethods.langCode == "ar-EG")
                this.FlowDirection = FlowDirection.RightToLeft;
            else
                this.FlowDirection = FlowDirection.LeftToRight;

         
        }

     


        
       

        // check is user admin or not to show ui windows for admin and for normal user
        public void userAccess(bool isAdmin)
        {
            if (!isAdmin)
            {
                (salesLB.Items[3] as ListBoxItem).Visibility = Visibility.Collapsed;
                (salesLB.Items[4] as ListBoxItem).Visibility = Visibility.Collapsed;

                (settingsLB.Items[0] as ListBoxItem).Visibility = Visibility.Collapsed;
                (settingsLB.Items[1] as ListBoxItem).Visibility = Visibility.Collapsed;
                (settingsLB.Items[2] as ListBoxItem).Visibility = Visibility.Collapsed;

                accounts_menu_btn.Visibility = Visibility.Collapsed;
                accountsLB.Visibility = Visibility.Collapsed;

                (itemsLB.Items[0] as ListBoxItem).Visibility = Visibility.Collapsed;
                (itemsLB.Items[2] as ListBoxItem).Visibility = Visibility.Collapsed;
                (itemsLB.Items[3] as ListBoxItem).Visibility = Visibility.Collapsed;

                purchase_menu_btn.Visibility = Visibility.Collapsed;
                purchLB.Visibility = Visibility.Collapsed;
            }
            else
            {
                (salesLB.Items[3] as ListBoxItem).Visibility = Visibility.Visible;
                (salesLB.Items[4] as ListBoxItem).Visibility = Visibility.Visible;

                (settingsLB.Items[0] as ListBoxItem).Visibility = Visibility.Visible;
                (settingsLB.Items[1] as ListBoxItem).Visibility = Visibility.Visible;
                (settingsLB.Items[2] as ListBoxItem).Visibility = Visibility.Visible;

                accounts_menu_btn.Visibility = Visibility.Visible;
                accountsLB.Visibility = Visibility.Visible;

                (itemsLB.Items[0] as ListBoxItem).Visibility = Visibility.Visible;
                (itemsLB.Items[2] as ListBoxItem).Visibility = Visibility.Visible;
                (itemsLB.Items[3] as ListBoxItem).Visibility = Visibility.Visible;

                purchase_menu_btn.Visibility = Visibility.Visible;
                purchLB.Visibility = Visibility.Visible;
            }
        }


         public void showPage(string pageName)
        {
            switch (pageName)
            {
                case "sales":
                    salesLB.SelectedIndex = 0;
                    salesLB_SelectionChanged(null, null);
                    break;
                case "orders":
                    salesLB.SelectedIndex = 1;
                    salesLB_SelectionChanged(null, null);
                    break;
                case "delvOrders":
                    // salesLB.SelectedIndex = 2;
                    // try{
                    //     popWin.Close();
                    // }catch{}
                    // salesLB_SelectionChanged(null, null);
                    break;
                case "salesReport":
                    salesLB.SelectedIndex = 3;
                    salesLB_SelectionChanged(null, null);
                    break;
                case "client":
                    salesLB.SelectedIndex = 4;
                    salesLB_SelectionChanged(null, null);
                    break;
                case "purchase":
                    purchLB.SelectedIndex = 0;
                    purchLB_SelectionChanged(null, null);
                    break;
                case "sup":
                    purchLB.SelectedIndex = 1;
                    purchLB_SelectionChanged(null, null);
                    break;
                case "store":
                    purchLB.SelectedIndex = 2;
                    purchLB_SelectionChanged(null, null);
                    break;
                case "items":
                    itemsLB.SelectedIndex = 0;
                    itemsLB_SelectionChanged(null, null);
                    break;
                case "safe":
                    accountsLB.SelectedIndex = 0;
                    accountsLB_SelectionChanged(null, null);
                    break;
                case "cashier":
                    accountsLB.SelectedIndex = 1;
                    accountsLB_SelectionChanged(null, null);
                    break;
                case "cashierAccs":
                    accountsLB.SelectedIndex = 2;
                    accountsLB_SelectionChanged(null, null);
                    break;
                case "expenses":
                    accountsLB.SelectedIndex = 3;
                    accountsLB_SelectionChanged(null, null);
                    break;
            }
        }


         private void menu_btn_MouseEnter(object sender, MouseEventArgs e)
        {
            CommonMethods.query = (sender as Button).Name;
            menusBorder_br.Visibility = Visibility.Visible;
            switch (CommonMethods.query)
            {
                case "sales_menu_btn":
                    salesLB.Visibility = Visibility.Visible;
                    break;
                case "purchase_menu_btn":
                    purchLB.Visibility = Visibility.Visible;
                    break;
                case "items_menu_btn":
                    itemsLB.Visibility = Visibility.Visible;
                    break;
                case "accounts_menu_btn":
                    accountsLB.Visibility = Visibility.Visible;
                    break;
                case "settings_menu_btn":
                    settingsLB.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void menu_btn_MouseLeave(object sender, MouseEventArgs e)
        {
            CommonMethods.query = (sender as Button).Name;
            switch (CommonMethods.query)
            {
                case "sales_menu_btn":
                    salesLB.Visibility = Visibility.Hidden;
                    break;
                case "purchase_menu_btn":
                    purchLB.Visibility = Visibility.Hidden;
                    break;
                case "items_menu_btn":
                    itemsLB.Visibility = Visibility.Hidden;
                    break;
                case "accounts_menu_btn":
                    accountsLB.Visibility = Visibility.Hidden;
                    break;
                case "settings_menu_btn":
                    settingsLB.Visibility = Visibility.Hidden;
                    break;
            }
            menusBorder_br.Visibility = Visibility.Collapsed;
        }

        private void ListBox_menus_MouseEnter(object sender, MouseEventArgs e)
        {
            CommonMethods.query = (sender as ListBox).Name;
            menusBorder_br.Visibility = Visibility.Visible;
            switch (CommonMethods.query)
            {
                case "salesLB":
                    salesLB.Visibility = Visibility.Visible;
                    break;
                case "purchLB":
                    purchLB.Visibility = Visibility.Visible;
                    break;
                case "itemsLB":
                    itemsLB.Visibility = Visibility.Visible;
                    break;
                case "accountsLB":
                    accountsLB.Visibility = Visibility.Visible;
                    break;
                case "settingsLB":
                    settingsLB.Visibility = Visibility.Visible;
                    break;
            }
            
        }

        private void ListBox_menus_MouseLeave(object sender, MouseEventArgs e)
        {
            CommonMethods.query = (sender as ListBox).Name;
            switch (CommonMethods.query)
            {
                case "salesLB":
                    salesLB.Visibility = Visibility.Hidden;
                    break;
                case "purchLB":
                    purchLB.Visibility = Visibility.Hidden;
                    break;
                case "itemsLB":
                    itemsLB.Visibility = Visibility.Hidden;
                    break;
                case "accountsLB":
                    accountsLB.Visibility = Visibility.Hidden;
                    break;
                case "settingsLB":
                    settingsLB.Visibility = Visibility.Hidden;
                    break;
            }
            menusBorder_br.Visibility = Visibility.Collapsed;
        }

        private void mainWindow_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mainGrid.Children.RemoveAt(0);
                unSelectItemListBox();
                if (isEnabled_keydownSales)
                {
                    try
                    {
                        Window.GetWindow(this).KeyDown -= salesCtrl.UserControl_KeyDown;
                    }
                    catch { }
                    isEnabled_keydownSales = false;
                }
            }
            catch
            {

            }
        }

        private void salesLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(currentMenu != 0)
                unSelectItemListBox();
            currentMenu = 0;

            CommonMethods.id = salesLB.SelectedIndex;

            if (CommonMethods.id == 0 || CommonMethods.id == 1 || 
                CommonMethods.id == 3 || CommonMethods.id == 4)
            {
                try { 
                    mainGrid.Children.RemoveAt(0);
                }
                catch { }
                try
                {
                    Window.GetWindow(this).KeyDown -= salesCtrl.UserControl_KeyDown;
                }
                catch { }
            }
            switch (CommonMethods.id)
            {
                case 0:
                    if(salesCtrl == null)
                    {
                        salesCtrl = new SalesController(this);
                    }
                    tab.Header = LocalizedLangs.Instance["sales_page"];
                    mainGrid.Children.Add((UIElement)salesCtrl);
                    Window.GetWindow(this).KeyDown += salesCtrl.UserControl_KeyDown;
                    isEnabled_keydownSales = true;
                    break;
                case 1:
                    if(orderCtrl == null)
                    {
                        orderCtrl = new OrdersController(this);
                    }
                    // showPopup((UIElement)orderCtrl, "Orders");
                    tab.Header = LocalizedLangs.Instance["orders"];
                    mainGrid.Children.Add((UIElement)orderCtrl);
                    break;
                case 2:
                    if(delvOrderCtrl == null)
                    {
                        delvOrderCtrl = new DeliveryOrdersController(this);
                    }
                    showPopup((UIElement)delvOrderCtrl, "Delivery Orders");
                    // mainGrid.Children.Add((UIElement)delvOrderCtrl);
                    break;
                case 3:
                    if(salesReportCtrl == null)
                    {
                        salesReportCtrl = new SalesReportController(this);
                    }
                    tab.Header = LocalizedLangs.Instance["sales"];
                    mainGrid.Children.Add((UIElement) salesReportCtrl);
                    break;
                case 4:
                    if(clientCtrl == null)
                    {
                        clientCtrl = new ClientController(this);
                    }
                    tab.Header = LocalizedLangs.Instance["clients"];
                    mainGrid.Children.Add((UIElement) clientCtrl);
                    break;

            }
            
        }

        public void showPopup(UIElement element, string title)
        {

            try
            {
                popWin.Close();
            }
            catch
            { }
            popWin = new PopupWindow();
            popWin.Title1.Content = title;
            popWin.popupMainGrid.Children.Add(element);
            popWin.Show();


            ////isTopMost = popWin.Topmost;
            ////popWin.Topmost = true;
            ////popWin.Topmost = isTopMost;
            ////popWin.Focus();
        }

        private void purchLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (currentMenu != 1)
                unSelectItemListBox();
            currentMenu = 1;
            CommonMethods.id = (purchLB.SelectedIndex);
            try { mainGrid.Children.RemoveAt(0); } catch { }
            try { Window.GetWindow(this).KeyDown -= salesCtrl.UserControl_KeyDown; }catch { }
            switch (CommonMethods.id)
            {
                case 0:
                    purchaseCtrl = new PurchaseController(this);
                    tab.Header = LocalizedLangs.Instance["purchases"];
                    mainGrid.Children.Add((UIElement)purchaseCtrl);
                    break;
                case 1:
                    supCtrl = new SuppliersController(this);
                    tab.Header = LocalizedLangs.Instance["sups"];
                    mainGrid.Children.Add((UIElement)supCtrl);
                    break;
                case 2:
                    storeCtrl = new StoreController(this);
                    tab.Header = LocalizedLangs.Instance["store"];
                    mainGrid.Children.Add((UIElement)storeCtrl);
                    break;

            }
            
        }

        private void itemsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (currentMenu != 2)
                unSelectItemListBox();
            currentMenu = 2;
            CommonMethods.id = itemsLB.SelectedIndex;
            try { Window.GetWindow(this).KeyDown -= salesCtrl.UserControl_KeyDown; }catch { }
            if (CommonMethods.id == 0)
            {
                try { mainGrid.Children.RemoveAt(0); }
                catch { }
            }
            switch (CommonMethods.id)
            {
                case 0:
                    itemsCtrl = new CategoryController(this);
                    tab.Header = LocalizedLangs.Instance["items"];
                    mainGrid.Children.Add((UIElement)itemsCtrl);
                    break;
                case 1:
                    prepCtrl = new PreparationController(this);
                    showPopup((UIElement)prepCtrl, LocalizedLangs.Instance["prep"]);
                    break;
                case 2:
                    categCtrl = new CateController(this);
                    showPopup((UIElement)categCtrl, LocalizedLangs.Instance["categ"]);
                    break;
                case 3:
                    offerCtrl = new OfferController(this);
                    showPopup((UIElement)offerCtrl, LocalizedLangs.Instance["offers"]);
                    break;
                case 4:
                    unSelectItemListBox();
                    try
                    {
                        kitchen.Close();
                    }catch{}
                    kitchen = new KitchenController(this);
                    kitchen.WindowState = WindowState.Maximized;
                    kitchen.Show();
                    break;
            }
            
        }

        private void accountsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (currentMenu != 3)
                unSelectItemListBox();
            currentMenu = 3;
            CommonMethods.id = accountsLB.SelectedIndex;
            try { Window.GetWindow(this).KeyDown -= salesCtrl.UserControl_KeyDown; }catch { }
            if (CommonMethods.id == 0 || CommonMethods.id == 2 || CommonMethods.id == 3)
            {
                try { mainGrid.Children.RemoveAt(0); }
                catch { }
            }
            switch (CommonMethods.id)
            {
                case 0:
                    safeCtrl = new SafeController(this);
                    tab.Header = LocalizedLangs.Instance["safe"];
                    mainGrid.Children.Add((UIElement)safeCtrl);
                    break;
                case 1:
                    cashierCtrl = new CashierController(this);
                    showPopup((UIElement)cashierCtrl, LocalizedLangs.Instance["cashier"]);
                    break;
                case 2:
                    casheierAccsCtrl = new CashierAccounts(this);
                    tab.Header = LocalizedLangs.Instance["cashier_accounts"];
                    mainGrid.Children.Add((UIElement)casheierAccsCtrl);
                    break;
                case 3:
                    expensesCtrl = new ExpensesController(this);
                    tab.Header = LocalizedLangs.Instance["expenses"];
                    mainGrid.Children.Add((UIElement)expensesCtrl);
                    break;
            }
            
        }

        private void settingsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (currentMenu != 4)
                unSelectItemListBox();
            currentMenu = 4;
            CommonMethods.id = (sender as ListBox).SelectedIndex;
            try { Window.GetWindow(this).KeyDown -= salesCtrl.UserControl_KeyDown; }catch { }
            if (CommonMethods.id == 1)
            {
                try { mainGrid.Children.RemoveAt(0); }
                catch { }
            }
            switch (CommonMethods.id)
            {
                case 0:
                    settingsCtrl = new SettingsController(this);
                    showPopup((UIElement)settingsCtrl, LocalizedLangs.Instance["settings"]);
                    break;
                case 1:
                    empCtrl = new EmployeeController(this);
                    tab.Header = LocalizedLangs.Instance["emp"];
                    mainGrid.Children.Add((UIElement)empCtrl);
                    break;
                case 2:
                    deliveryCtrl = new DeliveryController(this);
                    showPopup((UIElement)deliveryCtrl, LocalizedLangs.Instance["delivery"]);
                    break;

            }
            
        }

        public void unSelectItemListBox()
        {

            switch (currentMenu)
            {
                case 0:
                    salesLB.UnselectAll();
                    break;
                case 1:
                    purchLB.UnselectAll();
                    break;
                case 2:
                    itemsLB.UnselectAll();
                    break;
                case 3:
                    accountsLB.UnselectAll();
                    break;
                case 4:
                    settingsLB.UnselectAll();
                    break;
            }
        }

     

        


        


    

    }
}
