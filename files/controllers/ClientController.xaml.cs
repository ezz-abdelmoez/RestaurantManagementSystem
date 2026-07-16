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
    /// Interaction logic for ClientController.xaml
    /// </summary>
    public partial class ClientController : UserControl
    {
        private MainCtr mainCtr;
        public ClientController(MainCtr mainCtr)
        {
            InitializeComponent();
            this.mainCtr = mainCtr;

            showClients();
        }

        private void showClients()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                clientListview.DataContext = CommonMethods.showClients(ref client_combo, 
                    search_by_phone.Text, search_by_name_bp.Text).DefaultView;

                tot_tb.Text = client_combo.Items.Count.ToString();

            }), DispatcherPriority.Background);
            
        }

        private void search_by_name_bp_TextChanged(object sender, TextChangedEventArgs e)
        {
            showClients();
        }

        private void update_client_name_text_keydown(object sender, KeyEventArgs e)
        {
            if((sender as TextBox).Text.Length > 0 && e.Key == Key.Return)
            {
                CommonMethods.id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                DB.Update($"UPDATE Client SET client='{(sender as TextBox).Text}' " +
                    $"WHERE ID={CommonMethods.id}");
                MessageBox.Show("Updated");
                showClients();
            }
        }

        private void update_address_tb_keydown(object sender, KeyEventArgs e)
        {
            if ((sender as TextBox).Text.Length > 0 && e.Key == Key.Return)
            {
                CommonMethods.id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                DB.Update($"UPDATE Client SET address='{(sender as TextBox).Text}' " +
                    $"WHERE ID={CommonMethods.id}");
                MessageBox.Show("Updated");
                showClients();
            }
        }

        private void update_tel_tb_keydown(object sender, KeyEventArgs e)
        {
            if ((sender as TextBox).Text.Length > 0 && e.Key == Key.Return)
            {
                CommonMethods.id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                DB.Update($"UPDATE Client SET tel='{(sender as TextBox).Text}' " +
                    $"WHERE ID={CommonMethods.id}");
                MessageBox.Show("Updated");
                showClients();
            }
        }

        private void update_email_tb_keydown(object sender, KeyEventArgs e)
        {
            if ((sender as TextBox).Text.Length > 0 && e.Key == Key.Return)
            {
                CommonMethods.id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                DB.Update($"UPDATE Client SET email='{(sender as TextBox).Text}' " +
                    $"WHERE ID={CommonMethods.id}");
                MessageBox.Show("Updated");
                showClients();
            }
        }

        private void delete_client_img_mousedown(object sender, MouseButtonEventArgs e)
        {
            if (CommonMethods.defaultDelete())
            {
                CommonMethods.id = Convert.ToInt32(((sender as TextBox).DataContext as DataRowView).Row["ID"]);
                DB.Update($"DELETE * FROM Client " +
                    $"WHERE ID={CommonMethods.id}");
                MessageBox.Show("Deleted");
                showClients();
            }
        }

        private void add_new_client_img_mousedown(object sender, MouseButtonEventArgs e)
        {
            add_new_client();
        }

        private void client_name_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                add_new_client();
            }
        }

        private void add_new_client()
        {
            if (client_name_tb.Text.Length > 0)
            {
                DB.Insert($"INSERT INTO Client(client, tel, address, email) " +
                    $"VALUES('{client_name_tb.Text}', '{client_tel_tb.Text}', " +
                    $"'{client_address_tb.Text}', '{client_email_tb.Text}')");
                MessageBox.Show("Added");
                client_name_tb.Text = "";
                client_tel_tb.Text = "";
                client_address_tb.Text = "";
                client_email_tb.Text = "";
                showClients();
            }
            
        }

        private void search_by_phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }

        private void refresh_page_md(object sender, MouseButtonEventArgs e)
        {
            mainCtr.clientCtrl = null;
            mainCtr.showPage("client");
        }
    }

}
