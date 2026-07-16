using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Windows;

namespace SalesManagement.files.classes
{
    class DB
    {
        public static OleDbDataAdapter oleDb = new OleDbDataAdapter();
        public static DataTable dataTable;



        // public static OleDbConnection con = new OleDbConnection("Data Source=DESKTOP-TMQGP73\\SQLEXPRESS;Initial Catalog=foodSqlDB;Integrated Security=True");
        public static OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\FoodDb.mdb;User ID=admin");
        public static OleDbCommand cmd;

        public static DataTable getData(string query)
        {
            try
            {
                dataTable = new DataTable();
                con.Open();
                oleDb = new OleDbDataAdapter(new OleDbCommand(query, con)
                {
                    Connection = con
                });
                oleDb.Fill(dataTable);
                con.Close();
            }
            catch(Exception err)
            {
                con.Close();
                MessageBox.Show("select err: " + err.Message.ToString());
            }
            
            
            return dataTable;
        }

        public static void Insert(string query)
        {
            try
            {
                con.Open();
                oleDb.InsertCommand = new OleDbCommand(query, con);
                oleDb.InsertCommand.ExecuteNonQuery();

                con.Close();
            }
            catch(Exception err)
            {
                con.Close();
                MessageBox.Show("db insert err: "+err.Message.ToString());
            }

        }

        public static void Update(string query)
        {
            try
            {
                con.Open();

                cmd = new OleDbCommand(query, con);
                cmd.ExecuteNonQuery();
                //new OleDbCommand(query, con).ExecuteScalar();

                con.Close();


            }
            catch (Exception err)
            {
                con.Close();
                MessageBox.Show("db err: " + err.Message.ToString());
            }

        }

        public static void Delete(string query)
        {
            try
            {
                con.Open();
                oleDb.DeleteCommand = con.CreateCommand();
                oleDb.DeleteCommand.CommandText = query;
                oleDb.DeleteCommand.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception err)
            {
                con.Close();
                MessageBox.Show("db err: " + err.Message.ToString());
            }

        }

    }
}
