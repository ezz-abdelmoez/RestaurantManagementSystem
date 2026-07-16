using SalesManagement.files.classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using WPFLocalizeExtension.Engine;

namespace SalesManagement.Resources
{
    class LocalizedLangs
    {
        private LocalizedLangs()
        {

        }

        public static LocalizedLangs Instance { get; } = new LocalizedLangs();

        public void SetCulture(string langCode)
        {
            Properties.Settings.Default.languageCode = langCode;
            Properties.Settings.Default.Save();
            CommonMethods.langCode = Properties.Settings.Default.languageCode;
            LocalizeDictionary.Instance.Culture = new CultureInfo(CommonMethods.langCode);
        }

        public string this[string key]
        {
            get
            {
                return LocalizeDictionary.
                    Instance.GetLocalizedObject("SalesManagement", "Langs", key,
                    LocalizeDictionary.Instance.Culture) as string;
            }
        }
    }
}
