using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace SalesManagement.files.classes
{
    class WindowBasicControl
    {
        public static readonly Regex regex = new Regex("[^0-9.-]+"); 
        //regex that matches disallowed text

        public static bool IsNumber(string text)
        {
            return !regex.IsMatch(text);
        }

        public static void closeWin(Window win)
        {
            win.Close();
        }

        public static void minMaxWin(Window win)
        {
            if(win.WindowState == WindowState.Maximized)
            {
                win.WindowState = WindowState.Normal;
            }
            else
            {
                win.WindowState = WindowState.Maximized;
            }
        }

        public static void hideWin(Window win)
        {
            win.WindowState = WindowState.Minimized;
        }
    }
}
