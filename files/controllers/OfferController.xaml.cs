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
    /// Interaction logic for OfferController.xaml
    /// </summary>
    public partial class OfferController : UserControl
    {
        private MainCtr mainCtr;
        public OfferController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.getCate(ref offer_cate_combo);
                offer_cate_combo.SelectedIndex = -1;
                CommonMethods.getFood(ref offer_item_combo);
                offer_item_combo.SelectedIndex = -1;

                showOffers();
            }), DispatcherPriority.Background);
        }

        private void showOffers()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                offerListview.ItemsSource = DB.getData(
                    "SELECT PriceChange.*, Food.food FROM PriceChange INNER JOIN Food ON PriceChange.item_id=Food.ID"
                ).DefaultView;
            }), DispatcherPriority.Background);
        }

        private void save_offer_img_mouse_down(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //if (offer_cate_combo.SelectedIndex == -1)
                //    MessageBox.Show("Choose Category");
                if (offer_item_combo.SelectedIndex == -1)
                    MessageBox.Show("Choose Item");
                else if (offer_tb.Text.Length == 0)
                    MessageBox.Show("Enter offer");
                else if (!end_date_datePicker.SelectedDate.HasValue)
                    MessageBox.Show("Enter End Date for the offer");
                else
                {
                    DB.Update("UPDATE Food SET price=" + offer_tb.Text +
                              ", offer=1 WHERE ID=" + offer_item_combo.SelectedValue);
                    DB.Insert("INSERT INTO PriceChange(item_id, new_price, old_price, end_time) " +
                              $"VALUES({offer_item_combo.SelectedValue}, {offer_tb.Text}, " +
                              $"{price_tb.Text}, {end_date_datePicker.SelectedDate.Value.ToOADate()})");
                    MessageBox.Show("Done");
                    showOffers();
                    CommonMethods.id = offer_item_combo.SelectedIndex;
                    defaultChanges();
                    offer_item_combo.SelectedIndex = CommonMethods.id;
                }
            }), DispatcherPriority.Background);
        }

        private void offer_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }

        private void offer_cate_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (offer_cate_combo.SelectedIndex != -1)
            {
                CommonMethods.getFood(ref offer_item_combo,
                    Convert.ToInt32(offer_cate_combo.SelectedValue));
                offer_item_combo.SelectedIndex = -1;
            }
            else
            {
                CommonMethods.getFood(ref offer_item_combo);
                offer_item_combo.SelectedIndex = -1;
            }
            price_tb.Text = "";
        }

        private void offer_item_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (offer_item_combo.SelectedIndex != -1)
            {
                price_tb.Text = CommonMethods.
                    data.Rows[offer_item_combo.SelectedIndex]["price"].ToString();
            }
            else
            {
                price_tb.Text = "";
            }
        }

        private void delete_offer_mouse_down(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!CommonMethods.defaultDelete()) return;

                CommonMethods.dataRowView = ((sender as Image).DataContext as DataRowView);
                CommonMethods.id = (int)CommonMethods.dataRowView.Row["ID"];
                DB.Update("UPDATE Food SET price=" + CommonMethods.dataRowView.Row["old_price"]
                                                   + ", offer=0 WHERE ID=" + CommonMethods.dataRowView.Row["item_id"]);
                DB.Delete("DELETE * FROM PriceChange WHERE ID=" + CommonMethods.dataRowView.Row["ID"]);
                showOffers();
                CommonMethods.id = offer_item_combo.SelectedIndex;
                defaultChanges();
                offer_item_combo.SelectedIndex = CommonMethods.id;
            }), DispatcherPriority.Background);
        }

        private void defaultChanges()
        {
            CommonMethods.getCate(ref offer_cate_combo);
            CommonMethods.getFood(ref offer_item_combo);
            price_tb.Text = "";
            offer_tb.Text = "";
        }
    }
}
