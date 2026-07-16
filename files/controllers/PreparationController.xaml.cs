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
    /// Interaction logic for PreparationController.xaml
    /// </summary>
    public partial class PreparationController : UserControl
    {
        string query;
        int totalFoodQty = 0;
        DataTable data;
        private MainCtr mainCtr;

        public PreparationController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.getCate(ref cate_combo);
                cate_combo.SelectedIndex = -1;
                CommonMethods.getFood(ref item_name_combo);
                item_name_combo.SelectedIndex = -1;

                showExistFood();
                showPreparedFood();
            }), DispatcherPriority.Background);
        }

        private void showExistFood()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                query = "SELECT * FROM Food WHERE ok=0 AND qty > 0";
                if (cate_combo.SelectedIndex != -1)
                    query += " AND categ=" + cate_combo.SelectedValue;
                data = new DataTable();
                data = DB.getData(query);
                existListview.ItemsSource = data.DefaultView;
                totalFoodQty = 0;
                foreach (DataRow row in data.Rows)
                    totalFoodQty += Convert.ToInt32(row["qty"]);
                total_exist_qty_food_tb.Text = totalFoodQty.ToString();
                //MessageBox.Show(totalFoodQty.ToString());
            }), DispatcherPriority.Background);
        }

        private void showPreparedFood()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                query = "SELECT TOP 50 FoodQty.qty, Food.food, FoodQty.food_date " +
                        "FROM Food INNER JOIN FoodQty ON Food.ID = FoodQty.item_id " +
                        "WHERE Food.ok = 0";
                if (cate_combo.SelectedIndex != -1)
                    query += " AND FoodQty.item_id= " + cate_combo.SelectedValue;
                if (item_name_combo.SelectedIndex != -1)
                    query += " AND FoodQty.item_name=" + item_name_combo.SelectedValue;
                data = new DataTable();
                data = DB.getData(query);
                prebListview.ItemsSource = data.DefaultView;
                totalFoodQty = 0;
                foreach (DataRow row in data.Rows)
                    totalFoodQty += Convert.ToInt32(row["qty"]);
                total_preb_qty_food_tb.Text = totalFoodQty.ToString();
                //MessageBox.Show(prebListview.Items.Count.ToString());
            }), DispatcherPriority.Background);
        }

        private void register_preb_btn_click(object sender, RoutedEventArgs e)
        {
            if (cate_combo.SelectedIndex == -1)
                MessageBox.Show("Choose Category");
            else if (item_name_combo.SelectedIndex == -1)
                MessageBox.Show("Choose Food Name");
            else if (qty_tb.Text.Length == 0)
                MessageBox.Show("Enter Quantity");
            else
            {
                DB.Update("UPDATE Food set qty=qty+" + qty_tb.Text
                    + $" WHERE ID={item_name_combo.SelectedValue}");
                DB.Insert("INSERT INTO FoodQty (item_id, qty, food_date) " +
                    $"VALUES({item_name_combo.SelectedValue}, {qty_tb.Text}, " +
                    $"{DateTime.Now.ToOADate()})");

                cate_combo.SelectedIndex = -1;
                item_name_combo.SelectedIndex = -1;
                qty_tb.Text = "";

                showExistFood();
                showPreparedFood();
            }
        }

        private void cate_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CommonMethods.getFood(ref item_name_combo, Convert.ToInt32(cate_combo.SelectedValue));
        }

        private void qty_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }
    }
    
}
