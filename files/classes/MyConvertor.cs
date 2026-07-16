using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SalesManagement.files.controllers;

namespace SalesManagement.files.classes
{
    internal class MyConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (parameter != null)
                {
                    switch (System.Convert.ToInt32(parameter))
                    {
                        case 0:
                            if (SalesController.showItemImg)
                            {
                                return value;
                            }
                            else
                            {
                                return null;
                            }
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
