using SalesManagement.files.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for DeliveryController.xaml
    /// </summary>
    public partial class DeliveryController : UserControl
    {
        private MainCtr mainCtr;
        public DeliveryController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            showDelivery();
        }

        private void showDelivery()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (search_tb.Text.Length > 0)
                    CommonMethods.query = $" AND delivery_man like '{search_tb.Text}%' ";
                else
                    CommonMethods.query = "";
                devListview.ItemsSource =
                    DB.getData("SELECT * FROM Delivery WHERE deleted=0 "
                               + CommonMethods.query + " ORDER BY delivery_man").DefaultView;

                total_tb.Text = devListview.Items.Count.ToString();
            }), DispatcherPriority.Background);
        }

        private void show_new_dev_border_img_md(object sender, MouseButtonEventArgs e)
        {
            if(new_dev_border.Visibility == Visibility.Visible)
                new_dev_border.Visibility = Visibility.Collapsed;
            else
                new_dev_border.Visibility = Visibility.Visible;
        }

        private void add_new_dev_img_md(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (dev_name_tb.Text.Length > 0)
                {
                    DB.Insert("INSERT INTO Delivery(delivery_man, tel, address, email, deleted) " +
                              $"VALUES('{dev_name_tb.Text}', '{dev_phone_tb.Text}', '{dev_address_tb.Text}', " +
                              $"'{dev_email_tb.Text}', 0)");
                    new_dev_border.Visibility = Visibility.Visible;
                    dev_name_tb.Text = "";
                    dev_phone_tb.Text = "";
                    showDelivery();
                    MessageBox.Show("Added Successfully");

                }

                new_dev_border.Visibility = Visibility.Collapsed;
            }), DispatcherPriority.Background);
        }

        private void search_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (devListview != null)
                showDelivery();
        }

        private void dev_name_change_keydown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                if((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32((
                        (sender as TextBox).DataContext as DataRowView).Row[0]);
                    DB.Update($"UPDATE Delivery SET delivery_man='{(sender as TextBox).Text}' " +
                        $"WHERE ID=" + CommonMethods.id);
                }
            }
        }

        private void dev_address_change_keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32((
                        (sender as TextBox).DataContext as DataRowView).Row[0]);
                    DB.Update($"UPDATE Delivery SET address='{(sender as TextBox).Text}' " +
                        $"WHERE ID=" + CommonMethods.id);
                }
            }
        }

        private void dev_phone_change_keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32((
                        (sender as TextBox).DataContext as DataRowView).Row[0]);
                    DB.Update($"UPDATE Delivery SET tel='{(sender as TextBox).Text}' " +
                        $"WHERE ID=" + CommonMethods.id);
                }
            }
        }

        private void dev_email_change_keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if ((sender as TextBox).Text.Length > 0)
                {
                    CommonMethods.id = Convert.ToInt32((
                        (sender as TextBox).DataContext as DataRowView).Row[0]);
                    DB.Update($"UPDATE Delivery SET email='{(sender as TextBox).Text}' " +
                        $"WHERE ID="+ CommonMethods.id);
                }
            }
        }

        private void delete_dev_img_md(object sender, MouseButtonEventArgs e)
        {
            if (CommonMethods.defaultDelete())
            {
                CommonMethods.id = Convert.ToInt32((
                        (sender as Image).DataContext as DataRowView).Row[0]);
                DB.Delete("DELETE * FROM Delivery WHERE ID=" + CommonMethods.id);
                showDelivery();
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }
    }
}
