using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
using System.Xml.Linq;

namespace InventorySystem.Pages
{
    public partial class Rooms : Page
    {
        Data.DB database = new Data.DB();

        public Rooms()
        {
            InitializeComponent();
            UpdateAdmins();
            cbAdmins3.SelectedItem = "No filter";
            UpdateRooms();
        }

        private void UpdateAdmins()
        {
            cbAdmins.Items.Clear();
            cbAdmins2.Items.Clear();
            cbAdmins3.Items.Clear();

            try {
                database.Open();
                MySqlCommand cmd = database.PrepareCommand("SELECT * FROM admins");
                MySqlDataReader reader = cmd.ExecuteReader();

                cbAdmins3.Items.Add("No filter");
                while (reader.Read())
                {
                    string name = reader["name"].ToString();
                    cbAdmins.Items.Add($"{name[0].ToString().ToUpper()}{name.Substring(1)}");
                    cbAdmins2.Items.Add($"{name[0].ToString().ToUpper()}{name.Substring(1)}");
                    cbAdmins3.Items.Add($"{name[0].ToString().ToUpper()}{name.Substring(1)}");
                }
            } finally {
                database.Close();
            }
        }

        private void UpdateRooms()
        {
            lvRooms.Items.Clear();

            try {
                database.Open();
                MySqlCommand cmd = database.PrepareCommand("SELECT * FROM rooms");
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) {
                    string name = reader["name"].ToString();
                    lvRooms.Items.Add($"{name[0].ToString().ToUpper()}{name.Substring(1)}");
                }

            } finally {
                database.Close();
            }
        }

        private void lvRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvRooms.SelectedItem == null) {
                editingStatus.Text = "No room chosen - please choose a room to edit";
                cbAdmins.IsEnabled = false;
                changeAdminBtn.IsEnabled = false;
                cbAdmins.SelectedItem = null;
            } else if (lvRooms.SelectedItems.Count > 1) {
                editingStatus.Text = "You can edit only 1 room at a time";
                cbAdmins.IsEnabled = false;
                changeAdminBtn.IsEnabled = false;
                cbAdmins.SelectedItem = null;
            } else {
                database.Open();
                MySqlCommand cmd = database.PrepareCommand($"SELECT * FROM admins WHERE id = (SELECT admin_id FROM rooms WHERE name = '{lvRooms.SelectedItem.ToString().ToLower()}')");
                MySqlDataReader reader = cmd.ExecuteReader();

                editingStatus.Text = $"Editing '{lvRooms.SelectedItem.ToString()}'";
                cbAdmins.IsEnabled = true;
                changeAdminBtn.IsEnabled = true;

                if (reader.Read()) {
                    cbAdmins.SelectedItem = $"{reader["name"].ToString()[0].ToString().ToUpper()}{reader["name"].ToString().Substring(1)}";
                }

                database.Close();
            }
        }

        private void changeAdminBtn_Click(object sender, RoutedEventArgs e)
        {
            try {
                database.Open();
                MySqlCommand cmd = database.PrepareCommand($"UPDATE rooms SET admin_id = (SELECT id FROM admins WHERE name = '{cbAdmins.SelectedItem.ToString().ToLower()}') WHERE id = (SELECT id FROM rooms WHERE name = '{lvRooms.SelectedItem.ToString().ToLower()}')");
                cmd.ExecuteNonQuery();
                database.Close();

                roomStatus.Text = $"Changed admin of room '{lvRooms.SelectedItem.ToString()}' to '{cbAdmins.SelectedItem.ToString()}'";
                lvRooms.SelectedItem = null;

                UpdateFilter();
            } catch (Exception err) {
                roomStatus.Text = err.ToString();
            }
        }

        private void deleteRoom_Click(object sender, RoutedEventArgs e)
        {
            var items = lvRooms.SelectedItems;
            int deleteCount = items.Count;
            var result = MessageBox.Show($"Are you sure you want to delete {deleteCount} rooms?", "Delete rooms", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes) {
                roomStatus.Text = "";
                try {
                    foreach (var item in items) {
                        database.Open();
                        MySqlCommand selectCmd = database.PrepareCommand($"SELECT * FROM items WHERE room_id = (SELECT id FROM rooms WHERE name = '{item.ToString().ToLower()}')");
                        MySqlDataReader reader = selectCmd.ExecuteReader();

                        if (reader.Read()) {
                            roomStatus.Text += $"Can't remove {item.ToString()} - it is not empty!\n";
                            database.Close();
                        } else {
                            database.Reopen();
                            roomStatus.Text += $"{item.ToString()} succesfully removed!\n";
                            MySqlCommand deleteCmd = database.PrepareCommand($"DELETE FROM rooms WHERE name = '{item.ToString().ToLower()}'");
                            deleteCmd.ExecuteNonQuery();
                            database.Close();
                        }
                    }

                    UpdateFilter();
                } catch {
                    roomStatus.Text = "Error while deleting admins";
                }
            }
        }

        private void addRoom_Click(object sender, RoutedEventArgs e)
        {
            try {
                string input = addRoomInput.Text.ToString().Trim().ToLower();

                if (input == "") {
                    roomStatus.Text = "Please enter a valid name";
                } else if (cbAdmins2.SelectedItem == null) {
                    roomStatus.Text = "Please choose an admin";
                } else {
                    database.Open();
                    MySqlCommand selectCmd = database.PrepareCommand($"SELECT * FROM rooms WHERE name = '{input}'");
                    MySqlDataReader reader = selectCmd.ExecuteReader();

                    if (reader.Read()) {
                        roomStatus.Text = $"The name '{input[0].ToString().ToUpper()}{input.Substring(1)}' is already taken!";
                        database.Close();
                    } else {
                        database.Reopen();
                        MySqlCommand insertCmd = database.PrepareCommand($"INSERT INTO rooms (name, admin_id) VALUES ('{input}', (SELECT id FROM admins WHERE name = '{cbAdmins2.SelectedItem.ToString().ToLower()}'))");
                        insertCmd.ExecuteNonQuery();
                        database.Close();

                        roomStatus.Text = $"Added room named '{input[0].ToString().ToUpper()}{input.Substring(1)}'";
                        addRoomInput.Text = "";
                        cbAdmins2.SelectedItem = null;


                        UpdateFilter();
                    }
                }
            } catch (Exception err) {
                roomStatus.Text = "Error adding a new room";
            }
        }

        public void UpdateFilter()
        {
            string selected = cbAdmins3.SelectedItem.ToString();
            cbAdmins3.SelectedItem = null;
            cbAdmins3.SelectedItem = selected;
        }

        private void cbAdmins3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAdmins3.SelectedItem == null) { return; }

            try {
                if (cbAdmins3.SelectedItem.ToString() == "No filter") {
                    UpdateRooms();
                } else {
                    database.Open();

                    MySqlCommand cmd = database.PrepareCommand($"SELECT * FROM rooms WHERE admin_id = (SELECT id FROM admins WHERE name = '{cbAdmins3.SelectedItem.ToString().ToLower()}')");
                    MySqlDataReader reader = cmd.ExecuteReader();

                    lvRooms.Items.Clear();
                    while (reader.Read())
                        lvRooms.Items.Add($"{reader["name"].ToString()[0].ToString().ToUpper()}{reader["name"].ToString().Substring(1)}");

                    database.Close();
                }
            } catch {
                roomStatus.Text = "Error while filtering rooms";
            }
        }
    }
}
