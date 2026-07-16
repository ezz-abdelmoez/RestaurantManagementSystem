using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace SalesManagement.files.classes
{
    class ValidationKeyPress
    {
        public static void pay_previewTextInput(object sender, TextCompositionEventArgs e, char type)
        {
            if(type == 'n')
            {
                e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
            }
            if (type == 'p')
            {
                e.Handled = !(new Regex(@"^[0-9]+([.][0-9]*)?").IsMatch(e.Text));
            }
            if(type == 'f')
            {
                e.Handled = new Regex("[^0-9.-]+").IsMatch(e.Text);
            }

            // [+-] ? ([0 - 9] *[.])?[0 - 9] +
        }
    }
}
