using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
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

namespace InventorySystem.Pages
{
    public partial class Items : Page
    {
        Data.DB database = new Data.DB();

        public Items()
        {
            InitializeComponent();
            UpdateRooms();
            cbRooms3.SelectedItem = "No filter";
            cbAdmin.SelectedItem = "No filter";
            UpdateItems();
        }

        // Fix filtering
        private void UpdateItems(string roomFilter = "No filter", string adminFilter = "No filter", string dateFilter = "")
        {
            Console.WriteLine($"bbb: {adminFilter}");

            if (cbRooms3.SelectedItem == null) { return; }

            string cmdText = "SELECT * FROM items WHERE";
            try {

                if (dateFilter != "")
                    cmdText += $" ((start > '{dateFilter} 00:00:00' AND start < '{dateFilter} 23:59:59') OR (start < '{dateFilter} 00:00:00' AND (end > '{dateFilter} 00:00:00' OR end IS NULL)))";
                else
                    cmdText += " end IS NULL";

                if (roomFilter != "No filter")
                    cmdText += $" AND room_id = (SELECT id FROM rooms WHERE name = '{cbRooms3.SelectedItem.ToString().ToLower()}')";

                if (adminFilter != "No filter") {
                    database.Open();
                    MySqlCommand subCmd = database.PrepareCommand($"SELECT id FROM rooms WHERE admin_id = (SELECT id FROM admins WHERE name = '{cbAdmin.SelectedItem.ToString().ToLower()}')");
                    MySqlDataReader subReader = subCmd.ExecuteReader();

                    if (subReader.Read())
                        cmdText += $" AND (room_id = {subReader["id"]}";

                    while (subReader.Read())
                        cmdText += $" OR room_id = {subReader["id"]}";
                    
                    cmdText += $")";
                    database.Close();
                }

                itemStatus.Text = adminFilter;
                Console.WriteLine($"ccc: {adminFilter}");

                database.Open();
                MySqlCommand cmd = database.PrepareCommand(cmdText);
                MySqlDataReader reader = cmd.ExecuteReader();

                lvItems.Items.Clear();
                while (reader.Read())
                    lvItems.Items.Add($"{reader["name"].ToString()[0].ToString().ToUpper()}{reader["name"].ToString().Substring(1)}");

                database.Close();
            } catch (Exception err) {
                editingStatus.Text = "Error while filtering items";
            }
        }

        private void UpdateRooms()
        {
            cbRooms.Items.Clear();
            cbRooms2.Items.Clear();
            cbRooms3.Items.Clear();
            cbAdmin.Items.Clear();

            try {
                database.Open();
                MySqlCommand cmd = database.PrepareCommand("SELECT * FROM rooms");
                MySqlDataReader reader = cmd.ExecuteReader();

                cbRooms3.Items.Add("No filter");
                while (reader.Read()) {
                    string name = reader["name"].ToString();
                    cbRooms.Items.Add($"{name[0].ToString().ToUpper()}{name.Substring(1)}");
                    cbRooms2.Items.Add($"{name[0].ToString().ToUpper()}{name.Substring(1)}");
                    cbRooms3.Items.Add($"{name[0].ToString().ToUpper()}{name.Substring(1)}");
                }
            } finally {
                database.Close();
            }

            try {
                database.Open();
                MySqlCommand cmd = database.PrepareCommand("SELECT * FROM admins");
                MySqlDataReader reader = cmd.ExecuteReader();

                cbAdmin.Items.Add("No filter");
                while (reader.Read()) {
                    string name = reader["name"].ToString();
                    cbAdmin.Items.Add($"{name[0].ToString().ToUpper()}{name.Substring(1)}");
                }
            } finally {
                database.Close();
            }
        }

        private void changePropsBtn_Click(object sender, RoutedEventArgs e)
        {
            int price;

            if (int.TryParse(priceEdit.Text, out price)) { 
                try {
                    database.Open();
                    MySqlCommand cmd = database.PrepareCommand($"UPDATE items SET room_id = (SELECT id FROM rooms WHERE name = '{cbRooms.SelectedItem.ToString().ToLower()}') WHERE id = (SELECT id FROM items WHERE name = '{lvItems.SelectedItem.ToString().ToLower()}')");
                    cmd.ExecuteNonQuery();

                    database.Reopen();
                    cmd = database.PrepareCommand($"UPDATE items SET price = {price} WHERE name = '{lvItems.SelectedItem.ToString().ToLower()}'");
                    cmd.ExecuteNonQuery();
                    database.Close();

                    itemStatus.Text = $"Changed room of item '{lvItems.SelectedItem.ToString()}' to '{cbRooms.SelectedItem.ToString()}' and price to {priceEdit.Text.ToString()}";
                    lvItems.SelectedItem = null;

                    UpdateFilter();
                } catch (Exception err) {
                    itemStatus.Text = err.ToString();
                }
            } else {
                itemStatus.Text = "Please enter a valid price (only integers are supported)";
            }
        }

        private void cbRooms3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterChanged();
        }

        private void lvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvItems.SelectedItems.Count != 1) {
                cbRooms.IsEnabled = false;
                changePropsBtn.IsEnabled = false;
                cbRooms.SelectedItem = null;
                priceEdit.Enabled = false;
                priceEdit.Text = "";

                if (lvItems.SelectedItems.Count == 0)
                    editingStatus.Text = "No item chosen - please choose an item to edit";
                else
                    editingStatus.Text = "You can edit only 1 item at a time";
            } else {
                database.Open();
                MySqlCommand cmd = database.PrepareCommand($"SELECT * FROM items WHERE name = '{lvItems.SelectedItem.ToString().ToLower()}' AND end IS NULL");
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    priceEdit.Text = reader["price"].ToString();

                database.Reopen();
                cmd = database.PrepareCommand($"SELECT * FROM rooms WHERE id = (SELECT room_id FROM items WHERE name = '{lvItems.SelectedItem.ToString().ToLower()}' AND end IS NULL)");
                reader = cmd.ExecuteReader();
                if (reader.Read())
                    cbRooms.SelectedItem = $"{reader["name"].ToString()[0].ToString().ToUpper()}{reader["name"].ToString().Substring(1)}";

                editingStatus.Text = $"Editing '{lvItems.SelectedItem.ToString()}'";
                cbRooms.IsEnabled = true;
                changePropsBtn.IsEnabled = true;
                priceEdit.Enabled = true;

                database.Close();
            }
        }

        private void deleteItem_Click(object sender, RoutedEventArgs e)
        {
            var items = lvItems.SelectedItems;
            int deleteCount = items.Count;
            var result = MessageBox.Show($"Are you sure you want to delete {deleteCount} items?", "Delete items", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes) {
                editingStatus.Text = "";
                try {
                    foreach (var item in items) {
                        database.Open();
                        MySqlCommand selectCmd = database.PrepareCommand($"UPDATE items SET end = current_timestamp() WHERE name = '{item.ToString().ToLower()}'");
                        MySqlDataReader reader = selectCmd.ExecuteReader();
                        database.Close();
                        editingStatus.Text += $"{item.ToString()} succesfully removed!\n";
                    }

                    UpdateFilter();
                } catch {
                    editingStatus.Text = "Error while deleting items";
                }
            }
        }

        private void addItem_Click(object sender, RoutedEventArgs e)
        {
            try {
                string inputName = itemName.Text.ToString().Trim().ToLower();
                string inputPrice = itemPrice.Text.ToString().Trim().ToLower();
                int price;

                if (inputName == "") {
                    itemStatus.Text = "Please enter a valid name";
                } else if (!int.TryParse(inputPrice, out price)) {
                    itemStatus.Text = "Please enter a valid price (only integers are supported)";
                } else if (cbRooms2.SelectedItem == null) {
                    itemStatus.Text = "Please choose a room";
                } else {
                    database.Open();
                    MySqlCommand selectCmd = database.PrepareCommand($"SELECT * FROM items WHERE name = '{inputName}' AND end IS NULL");
                    MySqlDataReader reader = selectCmd.ExecuteReader();

                    if (reader.Read()) {
                        itemStatus.Text = $"The item '{inputName[0].ToString().ToUpper()}{inputName.Substring(1)}' is already added!";
                        database.Close();
                    } else {
                        database.Reopen();
                        MySqlCommand insertCmd = database.PrepareCommand($"INSERT INTO items (name, price, room_id) VALUES ('{inputName}', {inputPrice}, (SELECT id FROM rooms WHERE name = '{cbRooms2.SelectedItem.ToString().ToLower()}'))");
                        insertCmd.ExecuteNonQuery();
                        database.Close();

                        itemStatus.Text = $"Added item '{inputName[0].ToString().ToUpper()}{inputName.Substring(1)}' with a value of {inputPrice}";
                        itemName.Text = "";
                        itemPrice.Text = "";
                        cbRooms2.SelectedItem = null;

                        UpdateFilter();
                    }
                }
            } catch (Exception err) {
                itemStatus.Text = "Error adding a new item";
            }
        }

        public void UpdateFilter()
        {
            string selectedRoom = cbRooms3.SelectedItem.ToString();
            cbRooms3.SelectedItem = null;
            cbRooms3.SelectedItem = selectedRoom;

            string selectedDate = date.Text.ToString();
            date.Text = "";
            date.Text = selectedDate;
        }

        public void FilterChanged()
        {
            string roomF = "No filter";
            string adminF = "No filter";
            string dateF = "";

            if (cbRooms3.SelectedItem != null)
                roomF = cbRooms3.SelectedItem.ToString();

            if (cbAdmin.SelectedItem != null)
                adminF = cbAdmin.SelectedItem.ToString();

            if (date.Text != "")
                dateF = String.Format("{0:yyyy-MM-dd}", date.SelectedDate);

            Console.WriteLine($"aaa: {adminF}");
            UpdateItems(roomF, adminF, dateF);
        }

        private void date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterChanged();
        }

        private void deleteDateFilter_Click(object sender, RoutedEventArgs e)
        {
            date.Text = "";
        }

        private void cbAdmin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterChanged();
        }
    }
}
