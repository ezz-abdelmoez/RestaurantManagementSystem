using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace SalesManagement.files.classes
{
    public class CheckTables : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush azure = (Brush)Brushes.Azure;
            Brush brush = !(bool)value ? (Brush)Brushes.Blue : (Brush)Brushes.GreenYellow;
            Label label = new Label();
            label.BeginInit();
            label.Background = brush;
            label.EndInit();
            return (object)label.Background;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return parameter;
            throw new Exception("EqualityToBooleanConverter: It's false, I won't bind back.");
        }
    }
}
