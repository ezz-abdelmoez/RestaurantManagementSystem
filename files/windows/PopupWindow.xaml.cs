using SalesManagement.files.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SalesManagement.files.windows
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {

        public PopupWindow()
        {
            CommonMethods.langCode = SalesManagement.Properties.Settings.Default.languageCode;
            if (CommonMethods.langCode == "ar-EG")
                this.FlowDirection = FlowDirection.RightToLeft;
            else
                this.FlowDirection = FlowDirection.LeftToRight;

            InitializeComponent();
            
            
        }

        private void Exit_img(object sender, MouseButtonEventArgs e)
        {
            try
            {
                popupMainGrid.Children.RemoveAt(0);
            }
            catch { }
            this.Close();
        }

        private void Min_Max_Window_img(object sender, MouseButtonEventArgs e)
        {
            WindowBasicControl.minMaxWin(this);
        }

        private void hide_window_img(object sender, MouseButtonEventArgs e)
        {
            WindowBasicControl.hideWin(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainCtr.mainCtr.unSelectItemListBox();
        }

        private void drag_window_md(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton != MouseButton.Left)
                    return;
                this.DragMove();
            }
            catch { }
            
        }
    }

    
}
