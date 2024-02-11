using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public partial class Admins : Page
    {
        Data.DB database = new Data.DB();

        public Admins()
        {
            InitializeComponent();
            UpdateAdmins();
        }

        private void UpdateAdmins()
        {
            lvAdmins.Items.Clear();

            try {
                database.Open();
                MySqlCommand cmd = database.PrepareCommand("SELECT * FROM admins");
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) {
                    string name = reader["name"].ToString();
                    lvAdmins.Items.Add($"{name[0].ToString().ToUpper()}{name.Substring(1)}");
                }

            } finally {
                database.Close();
            }
        }

        private void addAdmin_Click(object sender, RoutedEventArgs e)
        {
            try {
                string input = addAdminInput.Text.ToString().Trim().ToLower();
                
                if (input == "") {
                    adminStatus.Text = "Please enter a valid name";
                } else {
                    database.Open();
                    MySqlCommand selectCmd = database.PrepareCommand($"SELECT * FROM admins WHERE name = '{input}'");
                    MySqlDataReader reader = selectCmd.ExecuteReader();
                    
                    if (reader.Read()) {
                        adminStatus.Text = $"The name '{input[0].ToString().ToUpper()}{input.Substring(1)}' is already taken!";
                        database.Close();
                    } else {
                        database.Reopen();
                        MySqlCommand insertCmd = database.PrepareCommand($"INSERT INTO admins (name) VALUES ('{input}')");
                        insertCmd.ExecuteNonQuery();
                        database.Close();

                        adminStatus.Text = $"Added an admin named '{input[0].ToString().ToUpper()}{input.Substring(1)}'";
                        addAdminInput.Text = "";

                        UpdateAdmins();
                    }
                }
            } catch {
                adminStatus.Text = "Error adding a new admin";
            }
        }

        private void deleteAdmin_Click(object sender, RoutedEventArgs e)
        {
            var items = lvAdmins.SelectedItems;
            int deleteCount = items.Count;
            var result = MessageBox.Show($"Are you sure you want to delete {deleteCount} admins?", "Delete admins", MessageBoxButton.YesNo);
            
            if (result == MessageBoxResult.Yes) {
                adminStatus.Text = "";
                try {
                    foreach (var item in items) {
                        database.Open();
                        MySqlCommand selectCmd = database.PrepareCommand($"SELECT * FROM rooms WHERE admin_id = (SELECT id FROM admins WHERE name = '{item.ToString().ToLower()}')");
                        MySqlDataReader reader = selectCmd.ExecuteReader();

                        if (reader.Read()) {
                            adminStatus.Text += $"Can't remove {item.ToString()} - it is a current admin of at least one room!\n";
                            database.Close();
                        } else {
                            database.Reopen();
                            adminStatus.Text += $"{item.ToString()} succesfully removed!\n";
                            MySqlCommand deleteCmd = database.PrepareCommand($"DELETE FROM admins WHERE name = '{item.ToString().ToLower()}'");
                            deleteCmd.ExecuteNonQuery();
                            database.Close();
                        }
                    }

                    UpdateAdmins();
                } catch {
                    adminStatus.Text = "Error while deleting admins";
                }
            }
        }
    }
}
