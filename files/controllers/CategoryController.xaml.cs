using Microsoft.Win32;
using SalesManagement.files.classes;
using SalesManagement.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Action = System.Action;
using Application = System.Windows.Forms.Application;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using Path = System.IO.Path;
using SelectionMode = System.Windows.Controls.SelectionMode;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace SalesManagement.files.controllers
{
    /// <summary>
    /// Interaction logic for CategoryController.xaml
    /// </summary>
    public partial class CategoryController : UserControl
    {
        private DataRowView dataRow;
        string query = "";
        int maxOrder, id, itemViewId;
        private MainCtr mainCtr;

        public CategoryController(MainCtr mainCtr)
        {
            InitializeComponent();

            this.mainCtr = mainCtr;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                CommonMethods.getCate(ref cate_combo);
                cate_combo.SelectedIndex = -1;
                CommonMethods.getCate(ref new_cate_combo);
                new_cate_combo.SelectedIndex = -1;
                CommonMethods.getCate(ref vw_cate_combo);
                vw_cate_combo.SelectedIndex = -1;

            }), DispatcherPriority.Background);
           
            showItems("");
            showStoreItem();
        }

        private void showStoreItem()
        {
            reg_store_item_combo.DataContext = 
                DB.getData("SELECT * FROM StoreItem WHERE ok=0").DefaultView;
            reg_store_item_combo.DisplayMemberPath = "item_name";
            reg_store_item_combo.SelectedValuePath = "ID";
            reg_store_item_combo.SelectedIndex = 0;
        }

        private void showItemStore(ref int id)
        {
            try
            {
                reg_list_view.DataContext = DB.getData(
                    "SELECT ItemStore.ID, StoreItem.item_name, ItemStore.qty " +
                    "FROM (ItemStore INNER JOIN Food ON ItemStore.item_id = Food.ID) " +
                    "INNER JOIN StoreItem ON ItemStore.store_id = StoreItem.ID " +
                    "WHERE item_id=" + id
                    ).DefaultView;
            }
            catch
            {

            }
        }

        private void showItems(string sender)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    query = "SELECT Food.*, Categ.cate FROM Food INNER JOIN Categ ON Food.categ " +
                            " = Categ.ID WHERE Food.ok=0 ";

                    if (sender == "search_item_tb")
                        query += $" AND Food.food LIKE '{search_item_tb.Text}%' ";
                    else search_item_tb.Text = "";

                    if (cate_combo.SelectedIndex != -1)
                        query += $" AND Food.categ = {cate_combo.SelectedValue} ";

                    query += " ORDER BY Food.categ, Food.order_no ";
                    itemsListview.ItemsSource = DB.getData(query).DefaultView;
                    total_items_tb.Text = itemsListview.Items.Count.ToString();
                }
                catch (Exception err)
                {
                    CommonMethods.message("show items method ::: " + err.Message.ToString());
                }

            }), DispatcherPriority.Background);
        }

        private void save_new_item_btn_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            update_item_btn.Visibility = Visibility.Collapsed;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (new_cate_combo.SelectedIndex == -1)
                    CommonMethods.message("Choose Category");
                else if (new_parcode_tb.Text.Length == 0)
                    CommonMethods.message("Enter Parcode");
                else if (new_item_name_tb.Text.Length == 0)
                    CommonMethods.message("Enter Item Name");
                else if (new_price_tb.Text.Length == 0)
                    CommonMethods.message("Enter Price");
                //else if (new_letter_tb.Text.Length > 1)
                //    CommonMethods.message("Enter one letter only");
                else
                {
                    try
                    {
                        if (Convert.ToInt32(DB
                                .getData($"SELECT COUNT(ID) AS a FROM Food WHERE parcode = '{new_parcode_tb.Text}'")
                                .Rows[0][0]) > 0 && !updateItem)
                        {
                            CommonMethods.message(LocalizedLangs.Instance["change_parcode"]);
                            return;
                        }
                    }catch{}

                    if (new_min_tb.Text.Length == 0)
                        new_min_tb.Text = "0";
                    maxOrder = 0;
                    try
                    {
                        maxOrder = Convert.ToInt32(DB.getData("SELECT MAX(order_no) FROM Food WHERE categ=" +
                                                              $"{new_cate_combo.SelectedValue}").Rows[0][0]);
                        maxOrder++;
                    }
                    catch
                    {
                        maxOrder = 1;
                    }

                    CommonMethods.copyFile(img_name, img_path);

                    try
                    {
                        DB.Insert(
                            "INSERT INTO Food(food, price, categ, preb_tim, stop_d, " +
                            "order_no, letter, min_level, parcode, food_img)" +
                            $" VALUES('{new_item_name_tb.Text}', " +
                            $"{Convert.ToDouble(new_price_tb.Text)}, " +
                            $"{new_cate_combo.SelectedValue}, " +
                            $"0, true, {maxOrder}, '{new_letter_tb.Text}'," +
                            $" {new_min_tb.Text}, '{new_parcode_tb.Text}', '" +
                            $"{CommonMethods.imgsPath + "\\" + img_name}')");

                        new_price_tb.Text = "";
                        new_item_name_tb.Text = "";
                        new_parcode_tb.Text = "";
                        CommonMethods.message("Done");
                        showItems("");
                    }
                    catch (Exception err)
                    {
                        CommonMethods.message(err.Message.ToString());
                    }
                    (sender as Button).IsEnabled = true;
                    // border_new_item.Visibility = Visibility.Collapsed;
                }
            }), DispatcherPriority.Background);
        }

        private void new_item_mouse_down(object sender, MouseButtonEventArgs e)
        {
            if(add_new_item_border.Visibility == Visibility.Collapsed)
            {
                add_new_item_border.Visibility = Visibility.Visible;
            }
            else
            {
                add_new_item_border.Visibility = Visibility.Collapsed;
            }
        }

        private void update_btn_click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (new_cate_combo.SelectedIndex == -1)
                    CommonMethods.message("Choose Category");
                else if (new_parcode_tb.Text.Length == 0)
                    CommonMethods.message("Enter Parcode");
                else if (new_item_name_tb.Text.Length == 0)
                    CommonMethods.message("Enter Item Name");
                else if (new_price_tb.Text.Length == 0)
                    CommonMethods.message("Enter Price");
                //else if (new_letter_tb.Text.Length > 1)
                //    CommonMethods.message("Enter one letter only");
                else
                {
                    try
                    {
                        if (Convert.ToInt32(DB
                                .getData($"SELECT COUNT(ID) AS a FROM Food WHERE parcode = '{new_parcode_tb.Text}'")
                                .Rows[0][0]) > 0 && !updateItem)
                        {
                            CommonMethods.message(LocalizedLangs.Instance["change_parcode"]);
                            return;
                        }
                    }catch{}

                    if (new_min_tb.Text.Length == 0)
                        new_min_tb.Text = "0";
                    maxOrder = 0;
                    try
                    {
                        maxOrder = Convert.ToInt32(DB.getData("SELECT MAX(order_no) FROM Food WHERE categ=" +
                                                              $"{new_cate_combo.SelectedValue}").Rows[0][0]);
                        maxOrder++;
                    }
                    catch
                    {
                        maxOrder = 1;
                    }

                    if(img_name != "/icon.png")
                    {
                        CommonMethods.copyFile(img_name, img_path);
                    }
                    else
                    {
                        img_name = "icon.png";
                    }

                    try
                    {
                        DB.Update($"UPDATE Food SET food='{new_item_name_tb.Text}', " +
                                  $"price={Convert.ToDouble(new_price_tb.Text)}, " +
                                  $"categ={new_cate_combo.SelectedValue}, " +
                                  $"letter='{new_letter_tb.Text}', " +
                                  $"min_level={new_min_tb.Text}, " +
                                  $"parcode='{new_parcode_tb.Text}', " +
                                  $"food_img='{CommonMethods.imgsPath+"\\"+img_name}' WHERE ID={updatedItemId}");
                        
                        new_price_tb.Text = "";
                        new_item_name_tb.Text = "";
                        new_parcode_tb.Text = "";
                        CommonMethods.message("Done");
                        showItems("");
                    }
                    catch (Exception err)
                    {
                        CommonMethods.message(err.Message.ToString());
                    }
                    (sender as Button).IsEnabled = true;
                    update_item_btn.Visibility = Visibility.Collapsed;
                    // border_new_item.Visibility = Visibility.Collapsed;
                }
            }), DispatcherPriority.Background);
        }

        private void cate_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showItems("");
        }

        private void search_item_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            showItems("search_item_tb");
        }

        private void price_key_down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {

                if ((sender as TextBox).Text.Length > 0)
                {
                    id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE Food SET price={(sender as TextBox).Text} WHERE ID={id}");
                    MessageBox.Show("Edite done");
                }
            }
        }

        private void letter_key_down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {

                if ((sender as TextBox).Text.Length > 0 && (sender as TextBox).Text.Length == 1)
                {
                    id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE Food SET letter='{(sender as TextBox).Text}' WHERE ID={id}");
                    MessageBox.Show("Edite done");
                }
                else
                {
                    MessageBox.Show("one letter only");
                }
            }
        }

        private void min_key_down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {

                if ((sender as TextBox).Text.Length > 0)
                {
                    id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE Food SET min_level={(sender as TextBox).Text} WHERE ID={id}");
                    MessageBox.Show("Edite done");
                }
            }
        }

        private void parcode_key_down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {

                if ((sender as TextBox).Text.Length > 0)
                {
                    id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE Food SET parcode='{(sender as TextBox).Text}' WHERE ID={id}");
                    MessageBox.Show("Edite done");
                }
            }
        }

        private void view_save_edit_imag_md(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (vw_cate_combo.SelectedIndex == -1)
                    MessageBox.Show("Choose Category");
                else if (vw_item_tb.Text.Length == 0)
                    MessageBox.Show("Enter Item Name");
                else if (vw_price_tb.Text.Length == 0)
                    MessageBox.Show("Enter Price");
                else if (vw_order_no_tb.Text.Length == 0)
                    MessageBox.Show("Enter Order Number");
                else if (vw_letter_tb.Text.Length > 1)
                    MessageBox.Show("one letter only");
                else
                {
                    if (vw_preb_tim_tb.Text.Length == 0)
                        vw_preb_tim_tb.Text = "0";
                    DB.Update($"UPDATE Food SET food='{vw_item_tb.Text}', " +
                              $"price={vw_price_tb.Text}, " +
                              $"preb_tim={vw_preb_tim_tb.Text}, " +
                              $"letter='{vw_letter_tb.Text}', " +
                              $"order_no={vw_order_no_tb.Text}, " +
                              $"categ={vw_cate_combo.SelectedValue} " +
                              $"WHERE ID={id}");

                    MessageBox.Show("Saved");
                    showItems("");
                }
            }), DispatcherPriority.Background);
        }

        private void show_border_view_img(object sender, MouseButtonEventArgs e)
        {
            border_view_item.Visibility = Visibility.Visible;
        }

        private void view_item_btn_ls(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                dataRow = ((sender as Button).DataContext as DataRowView);
                itemViewId = Convert.ToInt32(dataRow.Row["ID"]);
                vw_cate_combo.SelectedValue = dataRow.Row["categ"];
                vw_item_tb.Text = dataRow.Row["food"].ToString();
                vw_price_tb.Text = dataRow.Row["price"].ToString();
                vw_order_no_tb.Text = dataRow.Row["order_no"].ToString();
                vw_letter_tb.Text = dataRow.Row["letter"].ToString();
                showItemStore(ref itemViewId);
                border_view_item.Visibility = Visibility.Visible;
            }), DispatcherPriority.Background);
        }

        private void hide_border_view_img(object sender, MouseButtonEventArgs e)
        {
            border_view_item.Visibility = Visibility.Collapsed;
        }

        private void register_item_store_btn(object sender, RoutedEventArgs e)
        {
            if (reg_store_item_combo.SelectedIndex == -1)
                MessageBox.Show("Choose Item");
            else if (reg_qty_tb.Text.Length == 0)
                MessageBox.Show("Enter Quatity");
            else
            {
                DB.Insert($"INSERT INTO ItemStore (item_id, store_id, qty) " +
                    $"VALUES({itemViewId}, {reg_store_item_combo.SelectedValue}, {reg_qty_tb.Text})");
                MessageBox.Show("Saved");
                // show data
                showItemStore(ref itemViewId);
            }

        }

        private void delete_item_img(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you Sure!", "Delete", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            id = Convert.ToInt32(((sender as Image).DataContext as DataRowView).Row["ID"]);
            DB.Delete("DELETE * FROM Food WHERE ID=" + id);
            DB.Delete("DELETE * FROM ItemStore WHERE item_id=" + id);
            showItems("");
        }

        private void new_price_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //ValidationKeyPress.pay_previewTextInput(sender, e, 'f');
        }

        private void new_min_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void delete_reg_store_item_img(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you Sure!", "Delete", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            id = Convert.ToInt32(((sender as Image).DataContext as DataRowView).Row["ID"]);
            DB.Delete("DELETE * FROM ItemStore WHERE ID=" + id);

            showItemStore(ref itemViewId);
        }

        private void item_key_down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {

                if ((sender as TextBox).Text.Length > 0)
                {
                    id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                    DB.Update($"UPDATE Food SET food='{(sender as TextBox).Text}' WHERE ID={id}");
                    MessageBox.Show("Edite done");
                }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (multiEdit_stackpanel.Visibility == Visibility.Visible)
            {
                multiEdit_stackpanel.Visibility = Visibility.Collapsed;
                itemsListview.UnselectAll();
                itemsListview.SelectionMode = SelectionMode.Single;
            }
            else
            {
                multiEdit_stackpanel.Visibility = Visibility.Visible;
                itemsListview.SelectionMode = SelectionMode.Multiple;
            }
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                itemsListview.SelectAll();
            }catch{}
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                itemsListview.UnselectAll();
            }catch{}
        }

        private void delete_selected_items_img(object sender, MouseButtonEventArgs e)
        {
            
            if (MessageBox.Show("Are you Sure!", "Delete", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            Dispatcher.BeginInvoke(new Action(() =>
            {

                foreach (DataRowView item in itemsListview.SelectedItems)
                {
                    DB.Delete("DELETE * FROM Food WHERE ID=" + item.Row["ID"]);
                    DB.Delete("DELETE * FROM ItemStore WHERE item_id=" + item.Row["ID"]);
                }

                showItems("");
            }), DispatcherPriority.Background);

        }


        private OpenFileDialog ofd;
        private string img_name, img_path;
        private void PickImage_OnClick(object sender, RoutedEventArgs e)
        {
            ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.MyPictures);
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;...";

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                categ_img.Source = new BitmapImage(new Uri(ofd.FileName));
                img_name = ofd.SafeFileName;
                img_path = ofd.FileName;
                // MessageBox.Show(ofd.SafeFileName + "\n" + ofd.FileName);
            }
            
        }

        private bool updateItem = false;
        private int updatedItemId = -1;
        private void edit_item_btn_ls(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                dataRow = ((sender as Button).DataContext as DataRowView);
                updatedItemId = Convert.ToInt32(dataRow.Row["ID"]);
                new_cate_combo.SelectedValue = dataRow.Row["categ"];
                new_item_name_tb.Text = dataRow.Row["food"].ToString();
                new_price_tb.Text = dataRow.Row["price"].ToString();
                new_letter_tb.Text = dataRow.Row["letter"].ToString();
                new_parcode_tb.Text = dataRow.Row["parcode"].ToString();
                new_min_tb.Text = dataRow.Row["min_level"].ToString();
                img_name = dataRow.Row["food_img"].ToString();
                try
                {
                    categ_img.Source = new BitmapImage(new Uri(img_name));
                }
                catch
                {
                    categ_img.Source = new BitmapImage(new Uri(@"/SalesManagement;component/icon.png", UriKind.Relative));
                }

                updateItem = true;
                add_new_item_border.Visibility = Visibility.Visible;
                update_item_btn.Visibility = Visibility.Visible;
            }), DispatcherPriority.Background);
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.itemsCtrl = null;
            mainCtr.showPage("items");
        }
    }

    
}
