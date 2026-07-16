using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using WPFLocalizeExtension.Engine;

namespace SalesManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var langCode = SalesManagement.Properties.Settings.Default.languageCode;
            //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);

            LocalizeDictionary.Instance.Culture = new CultureInfo(langCode);

            base.OnStartup(e);
        }
    }
}
