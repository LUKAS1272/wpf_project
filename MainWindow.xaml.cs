using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

namespace InventorySystem
{
    public partial class MainWindow : Window
    {
        //MySqlConnection connection = new MySqlConnection($"server=127.0.0.1; database=inventory_system; Uid=root; password=;");
        Data.DB database = new Data.DB();

        public MainWindow()
        {
            InitializeComponent();
            error.Visibility = Visibility.Hidden;
            success.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try {
                database.Open();
                MySqlCommand command = database.PrepareCommand("SELECT * FROM test");

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());
                dtGrid.DataContext = dt;

                error.Visibility = Visibility.Hidden;
                success.Visibility = Visibility.Visible;
            } catch (Exception) {
                error.Visibility = Visibility.Visible;
                success.Visibility = Visibility.Hidden;
            } finally {
                database.Close();
            }
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            try {
                database.Open();
                MySqlCommand command = database.PrepareCommand($"INSERT INTO test (username, email, password) VALUES ('{username.Text}', '{email.Text}', '{password.Text}')");
                command.ExecuteNonQuery();

                error.Visibility = Visibility.Hidden;
                success.Visibility = Visibility.Visible;

                username.Text = "";
                email.Text = "";
                password.Text = "";
            } catch {
                error.Visibility = Visibility.Visible;
                success.Visibility = Visibility.Hidden;
            } finally {
                database.Close();
            }
        }
    }
}
