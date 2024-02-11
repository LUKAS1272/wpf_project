using MySql.Data.MySqlClient;
using Mysqlx;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InventorySystem {
    public partial class MainWindow : Window
    {
        Data.DB database = new Data.DB(); // Creates a DB object

        public MainWindow()
        {
            InitializeComponent();
            main.Content = new Pages.Admins();
        }

        private void items_Click(object sender, RoutedEventArgs e)
        { main.Content = new Pages.Items(); }

        private void rooms_Click(object sender, RoutedEventArgs e)
        { main.Content = new Pages.Rooms(); }

        private void admins_Click(object sender, RoutedEventArgs e)
        { main.Content = new Pages.Admins(); }

        //private void Show_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        database.Open();
        //        MySqlCommand command = database.PrepareCommand("SELECT * FROM test");

        //        DataTable dt = new DataTable();
        //        dt.Load(command.ExecuteReader());
        //        dtGrid.DataContext = dt;

        //        StatusLabel.Content = "Success - diplayed table contents";
        //    }
        //    catch (Exception error)
        //    {
        //        StatusLabel.Content = $"Error";
        //    }
        //    finally
        //    {
        //        database.Close();
        //    }
        //}

        //private void AddRow_Click(object sender, RoutedEventArgs e)
        //{

        //    try
        //    {
        //        database.Open();

        //        if (ValidateInput(new string[] { username.Text, email.Text, password.Text }))
        //        {
        //            MySqlCommand command = database.PrepareCommand($"INSERT INTO tet (username, email, password) VALUES ('{username.Text}', '{email.Text}', '{password.Text}')");
        //            command.ExecuteNonQuery();

        //            StatusLabel.Content = $"Success - added row ('{username.Text}', '{email.Text}', '{password.Text}')";

        //            username.Text = "";
        //            email.Text = "";
        //            password.Text = "";
        //        }
        //        else
        //        {
        //            StatusLabel.Content = $"Error - not all inputs are filled";
        //        }
        //        return;
        //    }
        //    catch (Exception error)
        //    {
        //        StatusLabel.Content = $"Error - {error.ToString()}";
        //    }
        //    finally
        //    {
        //        database.Close();
        //    }
        //}
    }
}
