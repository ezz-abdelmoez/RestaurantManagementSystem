using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using SalesManagement.files.classes;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for CateController.xaml
    /// </summary>
    public partial class CateController : UserControl
    {
        //private MainWindow main;
        int maxOrder = 0, id;
        private string query;
        private DataTable data = new DataTable();
        private MainCtr mainCtr;

        public CateController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;
            showPrintPlace();
            showCategory("");

        }

        private void showPrintPlace()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                print_place_combo.ItemsSource =
                    DB.getData("SELECT * FROM PrintPlace").DefaultView;
                print_place_combo.SelectedValuePath = "ID";
                print_place_combo.DisplayMemberPath = "place";
                print_place_combo.SelectedIndex = 0;
            }), DispatcherPriority.Background);
        }

        public void showCategory(string sender)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    data.Clear();
                    query = "";
                    if (sender == "search_cate_tb_TextChanged")
                    {
                        query += $" AND Categ.cate LIKE '{search_cate_tb.Text}%'";
                    }
                    else
                        search_cate_tb.Text = "";

                    data = DB.getData("SELECT Categ.*, PrintPlace.place FROM Categ LEFT JOIN PrintPlace " +
                                      "ON Categ.print_place = PrintPlace.ID WHERE ok=0 " + query +
                                      " ORDER BY Categ.order_no;");

                    maxOrder = 0;
                    foreach (DataRow row in data.Rows)
                    {
                        maxOrder++;
                        row["mos"] = maxOrder.ToString();
                    }

                    listview.ItemsSource = null;
                    listview.ItemsSource = data.DefaultView;
                    cate_count_lb.Content = maxOrder.ToString();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message.ToString());
                }
            }), DispatcherPriority.Background);
        }

        private void add_new_cate_img(object sender, MouseButtonEventArgs e)
        {
            // add_new_cate_border.Visibility = Visibility.Visible;
        }

        private void go_back_img(object sender, MouseButtonEventArgs e)
        {
            // add_new_cate_border.Visibility = Visibility.Collapsed;
        }
            
        private void save_new_cate_img(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (cate_name_tb.Text.Length == 0)
                    MessageBox.Show("Please Enter Category Name");
                else
                {
                    maxOrder = 0;
                    try
                    {
                        try
                        {
                            maxOrder = Convert.ToInt32(DB.getData("SELECT MAX(order_no) " +
                                                                  "FROM Categ").Rows[0][0]);
                            maxOrder++;
                        }
                        catch
                        {
                            maxOrder = 1;
                        }

                        maxOrder++;
                        DB.Insert("INSERT INTO Categ(cate, order_no, print_place) " +
                                  $"VALUES('{cate_name_tb.Text}', {(object)this.maxOrder}, " +
                                  $"'{print_place_combo.SelectedValue}')");

                        MessageBox.Show("Category Added Successfully");
                        cate_name_tb.Text = "";
                        showCategory("");
                        // add_new_cate_border.Visibility = Visibility.Collapsed;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message.ToString());
                    }
                }
            }), DispatcherPriority.Background);
        }

        private void search_cate_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            showCategory("search_cate_tb_TextChanged");
        }

        private void change_cate_keydown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                
                if ((sender as TextBox).Text.Length > 0)
                {
                    id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE Categ SET cate='{(sender as TextBox).Text}' WHERE ID={id}");
                    MessageBox.Show("Edite done");
                }
            }
        }

        private void change_arrnge_keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {

                if ((sender as TextBox).Text.Length > 0)
                {
                    id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE Categ SET order_no={(sender as TextBox).Text} WHERE ID={id}");
                    MessageBox.Show("Edite done");
                }
            }
        }

        private void order_no_tb_changed(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void delete_cate_img(object sender, MouseButtonEventArgs e)
        {
            id = Convert.ToInt32(((sender as Image).DataContext as DataRowView).Row["ID"]);
            if (MessageBox.Show("Are you sure to delete", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.Yes)
            {
                DB.Delete("DELETE FROM Categ WHERE ID=" + id);
                showCategory("");
            }
        }



        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (multiEdit_stackpanel.Visibility == Visibility.Visible)
            {
                multiEdit_stackpanel.Visibility = Visibility.Collapsed;
                listview.UnselectAll();
                listview.SelectionMode = SelectionMode.Single;
            }
            else
            {
                multiEdit_stackpanel.Visibility = Visibility.Visible;
                listview.SelectionMode = SelectionMode.Multiple;
            }
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                listview.SelectAll();
            }catch{}
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                listview.UnselectAll();
            }catch{}
        }

        private void delete_selected_items_img(object sender, MouseButtonEventArgs e)
        {
            
            if (MessageBox.Show("Are you Sure!", "Delete", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            Dispatcher.BeginInvoke(new Action(() =>
            {

                foreach (DataRowView item in listview.SelectedItems)
                {
                    DB.Delete("DELETE FROM Categ WHERE ID=" + item.Row["ID"]);
                }

                showCategory("");
            }), DispatcherPriority.Background);

        }
    }

}
