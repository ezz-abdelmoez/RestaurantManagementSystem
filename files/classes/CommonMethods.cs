using SalesManagement.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Application = System.Windows.Forms.Application;

namespace SalesManagement.files.classes
{
    class CommonMethods
    {
        public static bool flag = false;
        public static bool boolean = false;
        public static string query = "";
        public static string opDir = "";
        public static string langCode = "";
        public static string themeColor = "";
        public static string cpuId = "";
        public static string boisSerialNum = "";
        public static string uniqueId = "";
        public static double totalNumber = 0;
        public static double totalQty = 0;
        public static double totalPrice = 0;
        public static double lack = 0;
        public static double totalVendorValue = 0;
        public static double remain = 0;
        public static double balance = 0;
        public static float x = 0;
        public static float y = 0;
        public static float width = 0;
        public static float height = 0;
        public static int id = 0;
        public static int billNo = 0;
        public static int mos = 0;
        public static int orderNumber = 0;
        public static string reportTxt = "";
        public static string name = "";
        public static DataTable data;
        public static DataTable data1;
        public static DataRowView dataRowView;
        public static DateTime dateTime;
        public static IEnumerator enumerator;
        public static DataRow dataRow;
        public static string imgsPath= Path.GetDirectoryName(Application.ExecutablePath)+"\\Files\\Images";


        public static void copyFile(string imgName, string imgDir)
        {
            try
            {
                if (!Directory.Exists(imgsPath))
                    Directory.CreateDirectory(imgsPath);
                if (!File.Exists(imgsPath))
                {
                    File.Copy(imgDir,
                        imgsPath + "\\" + imgName,
                        true);
                }
            }catch{}
        }


        public static bool checkPassword(string pass, int userId)
        {
            encryptString(ref pass);
            if (pass ==
                DB.getData($"SELECT [password] FROM Users WHERE ID={userId}").Rows[0][0].ToString())
            {
                //MessageBox.Show("true "+pass);
                return true;
            }
            else
            {
                //MessageBox.Show("false " + pass);
                return false;
            }
        }


        // encryption 
        private static string key = "adef@@kfxcbv@";
        public static void encryptString(ref string pass)
        {
            if (string.IsNullOrEmpty(pass)) pass = "";
            pass += key;
            var passwordByte = Encoding.UTF8.GetBytes(pass);
            pass = Convert.ToBase64String(passwordByte);
        }
        // end of encryption

        public static void setDate(ref TextBox month, ref TextBox year)
        {
            month.Text = DateTime.Now.Month.ToString();
            year.Text = DateTime.Now.Year.ToString();
        }


        // sales reports
        public static DataTable getSalesReport()
        {
            data = new DataTable();

            data = DB.getData(
                "SELECT Sales.ID, Sales.no_food, Sales.price, Sales.operation_date, " +
                "Food.food, Categ.cate, Sales.order_no, Users.ID, Users.user_name " +
                "FROM Users RIGHT JOIN (Categ RIGHT JOIN (Food RIGHT JOIN Sales ON Food.ID = Sales.food_id) " +
                "ON Categ.ID = Food.categ) ON Users.ID = Sales.user_id");

            return data;
        }

        // show deliveries
        public static void showDelv(ref ComboBox combo)
        {
            data = new DataTable();
            data = DB.getData("SELECT * FROM Delivery WHERE deleted=0");
            combo.ItemsSource =  data.DefaultView;
            combo.SelectedValuePath = "ID";
            combo.DisplayMemberPath = "delivery_man";
        }

        // show clients
        public static DataTable showClients(ref ComboBox combo, string tel="", string name = "")
        {
            data = new DataTable();
            if (tel.Length > 0)
                tel += $" WHERE tel like '{tel}%'";

            if (name.Length > 0 && tel.Length > 0)
                tel += $" AND client like '{name}%'";
            else if(tel.Length == 0 && name.Length> 0)
                tel += $" WHERE client like '{name}%'";

            data = DB.getData("SELECT * FROM Client "+tel);
            combo.ItemsSource = data.DefaultView;
            combo.SelectedValuePath = "ID";
            combo.DisplayMemberPath = "client";
            return data;
        }

        /// <summary>
        /// show jobs from employees
        /// </summary>
        public static void getJobs(ref ComboBox combo)
        {
            data = new DataTable();
            data = DB.getData("SELECT eccup FROM Emp GROUP BY eccup HAVING eccup is not null " +
                "ORDER BY eccup");
            combo.ItemsSource = data.DefaultView;
            combo.DisplayMemberPath = "eccup";
            combo.SelectedValuePath = "eccup";
        }


        public static void decimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationKeyPress.pay_previewTextInput(sender, e, 'n');
        }


        public static void getUsers(ref ComboBox combo)
        {
            data = new DataTable();
            data = DB.getData("SELECT * FROM Users WHERE ok=0 ORDER BY user_name ASC");
            combo.ItemsSource = data.DefaultView;
            combo.DisplayMemberPath = "user_name";
            combo.SelectedValuePath = "ID";
            combo.SelectedIndex = 0;
        }
        public static bool defaultDelete(string title="")
        {
            if (title == "")
                title = LocalizedLangs.Instance["delete"].ToString();

            if (MessageBox.Show("are_you_sure", title, MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                return true;
            else return false;
        }

        public static void getVendors(ref ComboBox combo)
        {
            data = new DataTable();
            data = DB.getData("SELECT * FROM Vendor WHERE ok=0");
            combo.ItemsSource = data.DefaultView;
            combo.DisplayMemberPath = "vendor";
            combo.SelectedValuePath = "ID";
            combo.SelectedIndex = 0;
        }

        public static void getStoreItem(ref ComboBox combo)
        {
            data = new DataTable();
            data = DB.getData("SELECT item_type FROM StoreItem " +
                "GROUP BY item_type ORDER BY item_type");
            combo.ItemsSource = data.DefaultView;
            combo.SelectedValuePath = "item_type";
            combo.DisplayMemberPath = "item_type";
            
        }

        public static void getStoreItemAll(ref ComboBox combo)
        {
            data = new DataTable();
            data = DB.getData("SELECT * FROM StoreItem ");
            
            combo.ItemsSource = data.DefaultView;
            combo.SelectedValuePath = "ID";
            combo.DisplayMemberPath = "item_name";
        }

        public static DataTable getCate(ref ComboBox combo)
        {
            data = new DataTable();
            data = DB.getData("SELECT * FROM Categ");
            combo.ItemsSource = data.DefaultView;
            combo.SelectedValuePath = "ID";
            combo.DisplayMemberPath = "cate";
            return data;
        }

        public static DataTable getFood(ref ComboBox combo, int id = -1)
        {
            string query = "SELECT * FROM Food WHERE ok=0 ";
            if (id != -1)
                query += " AND categ=" + id;
            data = new DataTable();
            data = DB.getData(query);
            combo.ItemsSource = data.DefaultView;
            combo.DisplayMemberPath = "food";
            combo.SelectedValuePath = "ID";
            return data;
        }

        public static void message(string m)
        {
            MessageBox.Show(m);
        }
    }
}
